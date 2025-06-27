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
        /// <param name="type"></param>
        /// <returns></returns>
        IServerLogger CreateLogger(Type type);
    }
}
