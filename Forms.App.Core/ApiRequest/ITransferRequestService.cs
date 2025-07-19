using Forms.App.Core.Logging;

namespace Forms.App.Core.ApiRequest
{
    public interface ITransferRequestService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        void RegisterLogger(IServerLogger logger);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        Task<TResult?> GetAsync<TResult>(string url, object? queryParams = null) where TResult : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<TResult?> PostAsync<TResult>(string url, object? body = null) where TResult : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<TResult?> PutAsync<TResult>(string url, object? body = null) where TResult : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        Task<TResult?> DeleteAsync<TResult>(string url, object? queryParams = null) where TResult : class, new();
    }
}
