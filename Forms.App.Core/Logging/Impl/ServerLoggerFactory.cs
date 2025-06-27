using Lycoris.Autofac.Extensions;
using Microsoft.Extensions.Logging;

namespace Forms.App.Core.Logging.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class ServerLoggerFactory : IServerLoggerFactory
    {
        private readonly ILoggerFactory _factory;

        public ServerLoggerFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IServerLogger CreateLogger<T>()
        {
            var logger = _factory.CreateLogger<T>();
            return new ServerLogger(logger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IServerLogger CreateLogger(Type type)
        {
            var logger = _factory.CreateLogger(type);
            return new ServerLogger(logger);
        }
    }
}
