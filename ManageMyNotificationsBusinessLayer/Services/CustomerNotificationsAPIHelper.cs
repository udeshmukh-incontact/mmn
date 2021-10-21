using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ManageMyNotificationsBusinessLayer.Services
{
    public class CustomerNotificationsAPIHelper : ICustomerNotificationsAPIHelper
    {
        private readonly string _username;
        private readonly string _password;
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;
        private const string CN_SERVICEDOWN = "The CustomerNotificationsAPI service is either down or url is incorrect.";

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public CustomerNotificationsAPIHelper(HttpClient httpClient)
        {
            _username = ConfigurationManager.AppSettings["CustomerNotificationsApiUsername"];
            _password = ConfigurationManager.AppSettings["CustomerNotificationsApiPassword"];
            string url = ConfigurationManager.AppSettings["CustomerNotificationApiBaseUrl"];
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                _baseUrl = new Uri(url);
            _httpClient = httpClient;
        }

        public async Task<TResponse> CallApi<TRequest, TResponse>(string url, HttpMethod httpMethod, TRequest requestBody = default(TRequest))
        {
            TResponse response = default(TResponse);
            HttpClient client;
            if (_baseUrl == null)
            {
                ServiceException ex = new ServiceException(CN_SERVICEDOWN);
                ex.Data.Add("type", "servicedown");
                throw ex;
            }

            using ( client = _httpClient)
            {
                client.BaseAddress = _baseUrl;
                client.Timeout = TimeSpan.FromHours(1);
                var headerValue = $"{_username}:{_password}";
                headerValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(headerValue));
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", headerValue);
                var request = new HttpRequestMessage(httpMethod, url);


                if (!object.Equals(requestBody, default(TRequest)))
                {
                    var serializedRequestBody = JsonConvert.SerializeObject(requestBody, _jsonSettings);
                    request.Content = new StringContent(serializedRequestBody, Encoding.UTF8, "application/json");
                }
               
                try
                {
                    using (HttpResponseMessage rawResponse = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        if (rawResponse.StatusCode == HttpStatusCode.ServiceUnavailable || rawResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            ServiceException ex = new ServiceException(CN_SERVICEDOWN);
                            ex.Data.Add("type", "servicedown");
                            throw ex;
                        }
                        var actualMessage = await rawResponse.Content.ReadAsStringAsync();
                        var serializedResponseBody =  JsonConvert.DeserializeObject(actualMessage, _jsonSettings);

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
                        ServiceException srex = new ServiceException(CN_SERVICEDOWN);
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
                string errorMessage = $"CustomerNotificationsApi returned status code: {customerNotificationsApiError.Code}, Message: {customerNotificationsApiError.Message}";
                throw new Exception(errorMessage);
            }
        }
    }
}