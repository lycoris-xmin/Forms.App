using Forms.App.EntityFrameworkCore.Data.Tables;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Forms.App.Core.SqlRepository.Repositories.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class ConfigurationRepository : IConfigurationRepository
    {
        private const string CacheKey = "Configuration";
        private readonly ISqlRepository<ConfigurationTable, string> _repository;
        private readonly IMemoryCache _cache;

        public ConfigurationRepository(ISqlRepository<ConfigurationTable, string> repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task<ConfigurationTable?> GetDataAsync(string configId) => _repository.GetAsync(x => x.Id == configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<string> GetConfigurationAsync(string configId)
        {
            var cache = await GetCacheAsync(configId);
            if (!cache.IsNullOrEmpty())
                return cache!;

            cache = await _repository.GetSelectAsync(x => x.Id == configId, x => x.Value) ?? "";

            if (!cache.IsNullOrEmpty())
                await SetCacheAsync(configId, cache);

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configIds"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetMultipleConfigurationAsync(params string[] configIds)
        {
            var dic = configIds.ToDictionary(x => x, x => "");

            foreach (var item in configIds)
            {
                var cache = await GetCacheAsync(item);
                if (cache.IsNullOrEmpty())
                {
                    cache = await _repository.GetSelectAsync(x => x.Id == item, x => x.Value) ?? "";

                    if (!cache.IsNullOrEmpty())
                        await SetCacheAsync(item, cache);
                }

                dic[item] = cache!;
            }

            return dic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<T?> GetConfigurationAsync<T>(string configId) where T : class
        {
            var cache = await GetCacheAsync<T>(configId);
            if (cache != null)
                return cache;

            var data = await _repository.GetAsync(configId) ?? throw new ArgumentNullException(configId);

            if (!data.Value.IsNullOrEmpty())
                cache = data.Value.ToObject<T>() ?? default;

            if (cache != null)
                await SetCacheAsync(configId, cache);

            return cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task RemoveCacheAsync(string configId) => Task.Run(() => _cache.Remove($"{CacheKey}.{configId}"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        private Task<string?> GetCacheAsync(string configId) => Task.FromResult(_cache.Get($"{CacheKey}.{configId}") as string);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        private async Task<T?> GetCacheAsync<T>(string configId)
        {
            var value = await GetCacheAsync(configId);
            return value.ToObject<T>() ?? default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Task<bool> SetCacheAsync(string configId, string value)
        {
            try
            {
                _cache.Set($"{CacheKey}.{configId}", value);
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Task<bool> SetCacheAsync<T>(string configId, T value) where T : class
        {
            try
            {
                _cache.Set($"{CacheKey}.{configId}", value.ToJson());
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }
    }
}
