using Microsoft.Extensions.Logging;

namespace Forms.App.Core.Logging.Impl
{
    public class ServerLogger : IServerLogger
    {
        private readonly ILogger _logger;

        public ServerLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message) => _logger.LogInformation("{message}", message);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message) => _logger.LogWarning("{message}", message);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Warn(string message, Exception? ex) => _logger.LogWarning(ex, "{message}", message);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message) => _logger.LogError("{message}", message);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception? ex) => _logger.LogError(ex, "{message}", message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);
    }
}

