namespace Forms.App.Core.Logging
{
    public interface IServerLoggerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IServerLogger CreateLogger<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logFile"></param>
        /// <returns></returns>
        IServerLogger CreateLogger<T>(string? logFile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IServerLogger CreateLogger(Type type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="logFile"></param>
        /// <returns></returns>
        IServerLogger CreateLogger(Type type, string? logFile);
    }
}

