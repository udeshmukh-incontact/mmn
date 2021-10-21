using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
namespace InContact.Common.Branding
{
    public class PartnerBrandingServiceHelper : IPartnerBrandingServiceHelper
    {
        private readonly string _username;
        private readonly string _password;
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;
        private const string PB_SERVICEDOWN = "The Partner Branding service is either down or url is incorrect.";

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public PartnerBrandingServiceHelper(HttpClient httpClient)
        {
            _username = ConfigurationManager.AppSettings["PartnerBrandingApiUsername"];
            _password = ConfigurationManager.AppSettings["PartnerBrandingApiPassword"];
            string url = ConfigurationManager.AppSettings["PartnerBrandingApiBaseUrl"];
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                _baseUrl = new Uri(url);
            _httpClient = httpClient;
        }

        public TResponse CallApi<TRequest, TResponse>(string url, HttpMethod httpMethod, TRequest requestBody = default(TRequest))
        {
            TResponse response = default(TResponse);
            HttpStatusCode statusCode;
            if (_baseUrl == null)
            {
                ServiceException ex = new ServiceException(PB_SERVICEDOWN);
                ex.Data.Add("type", "servicedown");
                throw ex;
            }

            using (var client = _httpClient)
            {
                client.BaseAddress = _baseUrl;
                client.Timeout = TimeSpan.FromHours(1);
                var headerValue = _username+':'+_password;
                headerValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(headerValue));
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", headerValue);
                var request = new HttpRequestMessage(httpMethod, url);


                if (requestBody != null)
                {
                    var serializedRequestBody = JsonConvert.SerializeObject(requestBody, _jsonSettings);
                    request.Content = new StringContent(serializedRequestBody, Encoding.UTF8, "application/json");
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                try
                {
                    using (var rawResponse = client.SendAsync(request).Result)
                    {
                        if (rawResponse.StatusCode == HttpStatusCode.ServiceUnavailable || rawResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            ServiceException ex = new ServiceException(PB_SERVICEDOWN);
                            ex.Data.Add("type", "servicedown");
                            throw ex;
                        }

                        var serializedResponseBody = JsonConvert.DeserializeObject(rawResponse.Content.ReadAsStringAsync().Result, _jsonSettings);
                        string actualMessage = rawResponse.Content.ReadAsStringAsync().Result;

                        if (rawResponse.IsSuccessStatusCode)
                            response = JsonConvert.DeserializeObject<TResponse>(actualMessage);
                        else
                            ProcessError(actualMessage);
                    }
                }
                catch (AggregateException ex)
                {
                    if (ex.GetBaseException().GetType() == typeof(HttpRequestException))
                    {
                        ServiceException srex = new ServiceException(PB_SERVICEDOWN);
                        srex.Data.Add("type", "servicedown");
                        throw srex;
                    }
                    else
                        throw;
                }
            }

            return response;
        }

        private void ProcessError(string actualMessage)
        {
            var customerNotificationsApiError = JsonConvert.DeserializeObject<Error>(actualMessage);
            if (customerNotificationsApiError.Code == "404")
            {
                NotFoundException ex = new NotFoundException(customerNotificationsApiError.Message);
                ex.Data.Add("type", "notfound");
                throw ex;
            }
            else if (customerNotificationsApiError.Code == "503")
            {
                ServiceException ex = new ServiceException(customerNotificationsApiError.Message);
                ex.Data.Add("type", "servicedown");
                throw ex;
            }
            else if (customerNotificationsApiError.Code == "422")
                throw new FormatException(customerNotificationsApiError.Message);
            else
            {
                string errorMessage = "CustomerNotificationsApi returned status code:"+ customerNotificationsApiError.Code+','+" Message:" +customerNotificationsApiError.Message;
                throw new Exception(errorMessage);
            }
        }
    }
}