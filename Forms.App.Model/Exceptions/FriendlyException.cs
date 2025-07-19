namespace Forms.App.Model.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendlyException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public string LogMessage { get; private set; }

        public Exception? Exception { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FriendlyException() : base("系统繁忙，请稍候再试")
        {
            LogMessage = "系统繁忙，请稍候再试";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public FriendlyException(string message) : base(message)
        {
            LogMessage = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logMessage"></param>
        public FriendlyException(string message, string logMessage) : base(message)
        {
            LogMessage = logMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logMessage"></param>
        public FriendlyException(string message, string logMessage, Exception exception) : base(message)
        {
            LogMessage = logMessage;
            Exception = exception;
        }
    }
}
