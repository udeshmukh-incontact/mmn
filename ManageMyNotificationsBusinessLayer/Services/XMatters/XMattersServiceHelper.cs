using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ManageMyNotificationsBusinessLayer.Config;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ManageMyNotificationsBusinessLayer.Services.XMatters
{
    public class XMattersServiceHelper : IXMattersServiceHelper
    {
        private readonly string _username;
        private readonly string _password;
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        public XMattersServiceHelper(HttpClient httpClient)
        {
            _username = ConfigurationManager.AppSettings["xmattersUserName"];
            _password = ConfigurationManager.AppSettings["xmattersPassword"];
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["xmattersBaseUrl"]) && Uri.IsWellFormedUriString(ConfigurationManager.AppSettings["xmattersBaseUrl"], UriKind.Absolute))
                _baseUrl = new Uri(ConfigurationManager.AppSettings["xmattersBaseUrl"]);
            _httpClient = httpClient;
            InitHttpClient();
        }

        private void InitHttpClient()
        {
            _httpClient.BaseAddress = _baseUrl;
            var headerValue = $"{_username}:{_password}";
            headerValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(headerValue));
            _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", headerValue);
        }

        public async Task<TResponse> CallXMatters<TRequest, TResponse>(string url, HttpMethod httpMethod, TRequest requestBody = default(TRequest))
        {
            if (_baseUrl == null)
                throw new ServiceException(Constants.ErrorMessage.XMATTERS_SERVICE_DOWN);
            TResponse response = default(TResponse);
            string errorMessage = null;
            HttpStatusCode statusCode;
            
            var request = new HttpRequestMessage(httpMethod, url);

            if (!object.Equals(requestBody, default(TRequest)))
            {
                var serializedRequestBody = JsonConvert.SerializeObject(requestBody, _jsonSettings);
                request.Content = new StringContent(serializedRequestBody, Encoding.UTF8, "application/json");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            try
            {
                using (HttpResponseMessage rawResponse = await _httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    if (rawResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
                        throw new ServiceException(Constants.ErrorMessage.XMATTERS_SERVICE_DOWN);
                    string actualMessage = await rawResponse.Content.ReadAsStringAsync();

                    if (rawResponse.IsSuccessStatusCode)
                    {
                        response = JsonConvert.DeserializeObject<TResponse>(actualMessage);
                    }
                    else
                    {
                        var xMattersError = JsonConvert.DeserializeObject<XMattersError>(actualMessage);
                        errorMessage =
                            $"XMatters returned status code: {xMattersError.Code}, Message: {xMattersError.Message}";
                        if (xMattersError.Code == "400")
                            throw new FormatException(errorMessage);
                        else if (xMattersError.Code == "401")
                            throw new ServiceException(errorMessage);
                        else if (xMattersError.Code == "404")
                            throw new NotFoundException(errorMessage);
                        else
                            throw new Exception(errorMessage);
                    }
                    statusCode = rawResponse.StatusCode;
                }
            }
            catch (AggregateException ex)
            {
                if (ex.GetBaseException().GetType() == typeof(HttpRequestException))
                    throw new ServiceException(Constants.ErrorMessage.XMATTERS_SERVICE_DOWN);
                else
                    throw;
            }

            return response;
        }
    }
}
