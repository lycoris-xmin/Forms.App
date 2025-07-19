using Microsoft.Extensions.Logging;

namespace Forms.App.Core.Logging
{
    public interface IServerLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message">��־����</param>
        void Info(string message);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message">��־����</param>
        void Warn(string message);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message">��־����</param>
        /// <param name="ex"><see cref="Exception"/>�쳣��Ϣ</param>
        void Warn(string message, Exception? ex);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message">��־����</param>
        void Error(string message);

        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="message">��־����</param>
        /// <param name="ex"><see cref="Exception"/></param>
        void Error(string message, Exception? ex);
    }
}

