namespace Forms.App.Model.Bridges.Modules
{
    public interface IFormiumNotifyBridge
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        void Info(string title, string? message = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        void Warn(string title, string? message = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        void Success(string title, string? message = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        void Error(string title, string? message = null);
    }
}
