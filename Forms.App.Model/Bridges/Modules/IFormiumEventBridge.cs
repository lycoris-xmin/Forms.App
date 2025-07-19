namespace Forms.App.Model.Bridges.Modules
{
    public interface IFormiumEventBridge
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        void Send(IFormiumEventTypeEnum @event);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        void Send(IFormiumEventTypeEnum @event, string content);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        void Send<T>(IFormiumEventTypeEnum @event, T content) where T : class, new();
    }

    /// <summary>
    /// 
    /// </summary>
    public enum IFormiumEventTypeEnum
    {
        /// <summary>
        /// 刷新设备列表
        /// </summary>
        REFRESH_DEVICE_LIST,

        /// <summary>
        /// 安装App
        /// </summary>
        INSTALL_APP,
    }
}
