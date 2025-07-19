using Forms.App.Core.WsSocket.Impl;
using System.Collections.Concurrent;

namespace Forms.App.Core.WsSocket.Stores
{
    internal class WebSocketStore
    {
        /// <summary>
        /// 
        /// </summary>
        internal static readonly ConcurrentDictionary<string, DefaultWebSocketBehavior> Clients = new ConcurrentDictionary<string, DefaultWebSocketBehavior>();

        /// <summary>
        /// 
        /// </summary>
        internal static readonly ConcurrentDictionary<string, List<DefaultWebSocketBehavior>> Groups = new ConcurrentDictionary<string, List<DefaultWebSocketBehavior>>();
    }
}
