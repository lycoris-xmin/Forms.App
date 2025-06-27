using Lycoris.Autofac.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Forms.App.Core.Logging.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class ServerLoggerFactory : IServerLoggerFactory
    {
        private readonly ILoggerFactory _factory;
        private readonly IHttpContextAccessor _context;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ServerLoggerFactory(ILoggerFactory factory, IHttpContextAccessor context, IServiceScopeFactory serviceScopeFactory)
        {
            _factory = factory;
            _context = context;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IServerLogger CreateLogger<T>()
        {
            var logger = _factory.CreateLogger<T>();
            return new ServerLogger(logger, _context, _serviceScopeFactory, typeof(T).FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IServerLogger CreateLogger(Type type)
        {
            var logger = _factory.CreateLogger(type);
            return new ServerLogger(logger, _context, _serviceScopeFactory, type.FullName);
        }
    }
}
