namespace Forms.App.Model.Bridges.Modules
{
    public interface IFormiumToastBridge
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void Info(string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void Warn(string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void Success(string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void Error(string msg);
    }
}
