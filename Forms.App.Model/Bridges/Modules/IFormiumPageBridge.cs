namespace Forms.App.Model.Bridges.Modules
{
    public interface IFormiumPageBridge
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task GoForwardAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task GoBackAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ReloadAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task GoAsync(string url);
    }
}
