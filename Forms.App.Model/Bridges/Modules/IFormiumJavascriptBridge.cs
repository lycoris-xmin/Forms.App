namespace Forms.App.Model.Bridges.Modules
{
    public interface IFormiumJavascriptBridge
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="url"></param>
        /// <param name="line"></param>
        void Execute(string code, string url = "", int line = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="url"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        Task ExecuteAsync(string code, string url = "", int line = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="url"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        Task<T?> ExecuteAsync<T>(string code, string url = "", int line = 0);
    }
}
