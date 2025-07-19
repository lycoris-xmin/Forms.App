using Lycoris.Autofac.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Forms.App.Core.Cached.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (absoluteExpiration.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpiration.Value);
            }

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<T?> GetAsync<T>(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out var value) ? (T?)value : default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<(bool Found, T? Value)> TryGetValueAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var val))
            {
                return Task.FromResult((true, (T?)val));
            }

            return Task.FromResult((false, default(T)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                return (T)cached!;
            }

            var result = await factory();

            var options = new MemoryCacheEntryOptions();
            if (absoluteExpiration.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpiration.Value);
            }

            _cache.Set(key, result, options);
            return result;
        }
    }
}
