using Forms.App.Core.WsSocket.Impl;

namespace Forms.App.Core.WsSocket
{
    public interface IWsSocketServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="port"></param>
        /// <param name="path"></param>
        void Start<T>(int port, string path) where T : DefaultWebSocketBehavior, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Stop<T>() where T : DefaultWebSocketBehavior, new();
    }
}
