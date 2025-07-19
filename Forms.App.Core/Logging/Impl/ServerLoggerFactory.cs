using Forms.App.Model;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Forms.App.Core.Logging.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class ServerLoggerFactory : IServerLoggerFactory
    {
        private readonly Dictionary<string, SerilogLoggerFactory> SerilogLoggerFactoryMap;
        private readonly ILoggerFactory _factory;

        public ServerLoggerFactory(ILoggerFactory factory)
        {
            SerilogLoggerFactoryMap = new Dictionary<string, SerilogLoggerFactory>();

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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IServerLogger CreateLogger<T>(string? logFile) => CreateLogger(typeof(T), logFile);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="logFile"></param>
        /// <returns></returns>
        public IServerLogger CreateLogger(Type type, string? logFile)
        {
            if (!logFile.IsNullOrEmpty())
            {
                if (SerilogLoggerFactoryMap.TryGetValue(logFile!, out SerilogLoggerFactory? loggerFactory) && loggerFactory != null)
                {
                    var msLogger = loggerFactory.CreateLogger(type.FullName ?? type.Name);
                    return new ServerLogger(msLogger);
                }
                else
                {
                    var config = new LoggerConfiguration();

                    AppSettings.Serilog.SerilogOverrideOptions.ForEach(OverrideOption => config.MinimumLevel.Override(OverrideOption.Source, OverrideOption.MinLevel.ToEnum<LogEventLevel>()));

                    config.MinimumLevel.Is(AppSettings.Serilog.MinLevel.ToEnum<LogEventLevel>());

                    config.Enrich.FromLogContext();

                    if (AppSettings.Serilog.Console)
                        config.WriteTo.Console();

                    if (AppSettings.IsDebugger)
                        config.WriteTo.Debug();

                    if (AppSettings.Serilog.File)
                    {
                        var template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} - {Level:u3} - {SourceContext:l} - {Message:lj}{NewLine}{Exception}";
                        config.WriteTo.File(logFile!, outputTemplate: template, rollingInterval: RollingInterval.Day, shared: true, fileSizeLimitBytes: 10 * 1025 * 1024, rollOnFileSizeLimit: true);
                    }

                    loggerFactory = new SerilogLoggerFactory(config.CreateLogger(), dispose: true);

                    SerilogLoggerFactoryMap.Add(logFile!, loggerFactory);

                    var msLogger = loggerFactory.CreateLogger(type.FullName ?? type.Name);

                    return new ServerLogger(msLogger);
                }
            }
            else
            {
                // 默认使用全局 logger
                var logger = _factory.CreateLogger(type);
                return new ServerLogger(logger);
            }
        }
    }
}

