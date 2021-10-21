using System.Net.Http;


namespace InContact.Common.Branding
{
    public interface IPartnerBrandingServiceHelper
    {
        TResponse CallApi<TRequest, TResponse>(string url, HttpMethod httpMethod, TRequest requestBody = default(TRequest));
    }
}
