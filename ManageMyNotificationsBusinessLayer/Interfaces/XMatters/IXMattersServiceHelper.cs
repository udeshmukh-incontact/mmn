using System.Net.Http;
using System.Threading.Tasks;

namespace ManageMyNotificationsBusinessLayer.Interfaces.XMatters
{
    public interface IXMattersServiceHelper
    {
        Task<TResponse> CallXMatters<TRequest, TResponse>(string url, HttpMethod httpMethod, TRequest requestBody = default(TRequest));
    }
}
