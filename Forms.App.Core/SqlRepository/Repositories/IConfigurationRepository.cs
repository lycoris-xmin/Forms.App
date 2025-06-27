using Forms.App.EntityFrameworkCore.Data.Tables;

namespace Forms.App.Core.SqlRepository.Repositories
{
    public interface IConfigurationRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<ConfigurationTable?> GetDataAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<string> GetConfigurationAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configIds"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetMultipleConfigurationAsync(params string[] configIds);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<T?> GetConfigurationAsync<T>(string configId) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task RemoveCacheAsync(string configId);
    }
}
