using WebSocketSharp.Server;

namespace Forms.App.Core.WsSocket.Impl
{
    public class WsSocketServer : IWsSocketServer
    {
        private readonly Dictionary<string, WebSocketServer> _wsServer;

        public WsSocketServer()
        {
            _wsServer = new Dictionary<string, WebSocketServer>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="port"></param>
        public void Start<T>(int port, string path) where T : DefaultWebSocketBehavior, new()
        {
            var key = typeof(T).GetType().FullName!;

            if (_wsServer.TryGetValue(key, out WebSocketServer? value))
            {
                if (value != null && value.IsListening)
                    return;

                _wsServer[key] = new WebSocketServer(port);

                _wsServer[key].AddWebSocketService<T>(path);

                _wsServer[key].Start();
                return;
            }

            var wsServer = new WebSocketServer(port);

            wsServer.AddWebSocketService<T>(path);

            wsServer.Start();

            _wsServer.Add(key, wsServer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Stop<T>() where T : DefaultWebSocketBehavior, new()
        {
            var key = typeof(T).GetType().FullName!;

            if (_wsServer.TryGetValue(key, out WebSocketServer? value))
            {
                _wsServer[key].Stop();
                _wsServer.Remove(key);
            }
        }
    }
}
