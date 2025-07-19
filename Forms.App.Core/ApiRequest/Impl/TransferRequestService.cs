using Forms.App.Core.Logging;
using Forms.App.Model;
using Forms.App.Model.Contexts;
using Forms.App.Model.Models.Api;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using Polly;
using Polly.Retry;
using RestSharp;
using System.Diagnostics;
using System.Net;

namespace Forms.App.Core.ApiRequest.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Transient)]
    public partial class TransferRequestService : ITransferRequestService
    {
        private IServerLogger _logger;
        private readonly ClientContext _clientContext;

        private readonly RestClient _client;
        private readonly AsyncRetryPolicy<RestResponse> _retryPolicy;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
        private DateTime _lastRefreshTime = DateTime.MinValue;
        private string? _lastRefreshToken = null;

        public TransferRequestService(IServerLoggerFactory factory, ClientContext clientContext)
        {
            _logger = factory.CreateLogger<TransferRequestService>();
            _clientContext = clientContext;

            var options = new RestClientOptions(AppSettings.Application.Host)
            {
                ThrowOnAnyError = false,
                Timeout = TimeSpan.FromMinutes(3)
            };

            _client = new RestClient(options);

            // 配置 Polly 重试策略：最多重试 2 次，间隔 1 秒
            Func<HttpStatusCode, bool> condition
             = (statusCode) => statusCode == 0 || statusCode == HttpStatusCode.RequestTimeout || statusCode == HttpStatusCode.ServiceUnavailable;

            _retryPolicy = Policy.HandleResult<RestResponse>(r => condition.Invoke(r.StatusCode))
                                 .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(1));
        }

        public void RegisterLogger(IServerLogger logger) => _logger = logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public async Task<TResult?> GetAsync<TResult>(string url, object? queryParams = null) where TResult : class, new()
        {
            var response = await SendWithRetryAsync(() =>
            {
                var request = new RestRequest(url, Method.Get);

                if (queryParams != null)
                    request.AddObject(queryParams);

                return request;
            });

            if (response.IsSuccessful)
                return response.Content.ToTryObject<TResult>();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<TResult?> PostAsync<TResult>(string url, object? body = null) where TResult : class, new()
        {
            var response = await SendWithRetryAsync(() =>
            {
                var request = new RestRequest(url, Method.Post);

                if (body != null)
                    request.AddJsonBody(body.ToJson());

                return request;
            });

            if (response.IsSuccessful)
                return response.Content.ToTryObject<TResult>();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<TResult?> PutAsync<TResult>(string url, object? body = null) where TResult : class, new()
        {
            var response = await SendWithRetryAsync(() =>
            {
                var request = new RestRequest(url, Method.Put);
                if (body != null)
                    request.AddJsonBody(body.ToJson());
                return request;
            });

            if (response.IsSuccessful)
                return response.Content.ToTryObject<TResult>();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public async Task<TResult?> DeleteAsync<TResult>(string url, object? queryParams = null) where TResult : class, new()
        {
            var response = await SendWithRetryAsync(() =>
            {
                var request = new RestRequest(url, Method.Delete);

                if (queryParams != null)
                    request.AddObject(queryParams);

                return request;
            });

            if (response.IsSuccessful)
                return response.Content.ToTryObject<TResult>();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        private void AddDefaultHeaders(RestRequest request)
        {
            if (_clientContext != null && !_clientContext.Token.IsNullOrEmpty())
                request.AddOrUpdateHeader("X-Authorize", _clientContext.Token);

            request.AddOrUpdateHeader("X-Fingerprint", Guid.NewGuid().ToString());
            request.AddOrUpdateHeader("Host", new Uri(AppSettings.Application.Host).Host);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestFactory"></param>
        /// <returns></returns>
        private async Task<RestResponse> SendWithRetryAsync(Func<RestRequest> requestFactory)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                RestRequest request = null!;
                RestResponse response = null!;

                try
                {
                    request = requestFactory.Invoke();

                    AddDefaultHeaders(request);

                    LogRequest(request, isRetry: false);

                    response = await _client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var refreshed = await EnsureTokenRefreshedAsync();

                        if (refreshed)
                        {
                            var retryRequest = requestFactory.Invoke();

                            AddDefaultHeaders(retryRequest);

                            LogRequest(request, isRetry: true);

                            response = await _client.ExecuteAsync(retryRequest);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"request failed:{ex.Message}", ex);
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    if (response != null)
                        LogResponse(request, response, stopwatch.ElapsedMilliseconds);
                }

                return response;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> EnsureTokenRefreshedAsync()
        {
            var currentRefreshToken = _clientContext.RefershToken;

            await _refreshLock.WaitAsync();
            try
            {
                // 如果令牌在我们等待期间被其他线程刷新过，就不再刷新
                if (_lastRefreshToken == currentRefreshToken && (DateTime.Now - _lastRefreshTime).TotalSeconds < 30)
                {
                    return true;
                }

                var refreshed = await RefreshTokenAsync();
                if (refreshed)
                {
                    _lastRefreshToken = _clientContext.RefershToken;
                    _lastRefreshTime = DateTime.Now;
                }

                return refreshed;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RefreshTokenAsync()
        {
            try
            {
                var request = new RestRequest("/api/authorize/token/refresh", Method.Post);

                request.AddJsonBody(new { refreshToken = _clientContext.RefershToken });

                var response = await _client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var resp = response.Content.ToTryObject<DataOutput<AuthorizeRefreshTokenResponse>>();

                    if (resp != null && resp.Code == 0)
                    {
                        _clientContext.Token = resp.Data!.Token;
                        _clientContext.RefershToken = resp.Data!.RefreshToken;
                        return true;
                    }

                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isRetry"></param>
        private void LogRequest(RestRequest request, bool isRetry)
        {
            var url = _client.BuildUri(request);
            var method = request.Method.ToString().ToUpperInvariant();
            var prefix = isRetry ? "[Retry] " : "";

            string requestBody = string.Empty;

            // 提取 JSON 或文件描述信息
            foreach (var param in request.Parameters)
            {
                if (param.Type == ParameterType.RequestBody && param.Value != null)
                {
                    if (param.Value is FileParameter file)
                    {
                        requestBody = $"[File] {file.FileName}";
                    }
                    else
                    {
                        requestBody = Truncate(param.Value.ToString() ?? "", 500);
                    }
                }
            }

            _logger.Info($"{prefix}api request  -> {method} - {url} - {requestBody}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="elapsedMs"></param>
        private void LogResponse(RestRequest request, RestResponse response, long elapsedMs)
        {
            string responseBody = string.IsNullOrEmpty(response.Content)
                ? "<empty>"
                : Truncate(response.Content, 1000);

            _logger.Info($"api response -> {responseBody} - {(int)response.StatusCode} - {elapsedMs}ms");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private static string Truncate(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// 
        /// </summary>
        private class AuthorizeRefreshTokenResponse
        {
            /// <summary>
            /// 
            /// </summary>
            public string Token { get; set; } = default!;

            /// <summary>
            /// 
            /// </summary>
            public string RefreshToken { get; set; } = default!;
        }
    }
}
