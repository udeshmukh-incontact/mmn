using System.Net.Http;
using System.Threading.Tasks;

namespace ManageMyNotificationsBusinessLayer.Interfaces
{
    public interface ICustomerNotificationsAPIHelper
    {
        Task<TResponse> CallApi<TRequest, TResponse>(string url, HttpMethod httpMethod, TRequest requestBody = default(TRequest));
    }
}
