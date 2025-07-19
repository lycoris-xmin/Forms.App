using Forms.App.Core.WsSocket.Stores;
using Lycoris.Common.Extensions;

namespace Forms.App.Core.WsSocket.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WebSocketGroup
    {
        private readonly DefaultWebSocketBehavior _behavior;

        public WebSocketGroup(DefaultWebSocketBehavior behavior)
        {
            _behavior = behavior;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddGroup(string name)
        {
            //
            if (WebSocketStore.Groups.TryGetValue(name, out List<DefaultWebSocketBehavior>? clients))
            {
                clients ??= new List<DefaultWebSocketBehavior>();
                if (clients.Any(x => x.ConnectionId == _behavior.ConnectionId))
                    return;

                clients.Add(_behavior);
            }
            else
            {
                WebSocketStore.Groups.TryAdd(name, new List<DefaultWebSocketBehavior>() { _behavior });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void RemoveGroup(string name)
        {
            if (WebSocketStore.Groups.TryGetValue(name, out List<DefaultWebSocketBehavior>? clients))
            {
                clients ??= new List<DefaultWebSocketBehavior>();
                if (clients.Any(x => x.ConnectionId == _behavior.ConnectionId))
                    clients.Remove(_behavior);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DefaultWebSocketBehavior> ExcludeGroup(params string[] groups)
        {
            if (!groups.HasValue())
                return new List<DefaultWebSocketBehavior>();

            var clients = WebSocketStore.Groups.Where(x => !groups.Equals(x.Key)).SelectMany(x => x.Value).ToList();

            return clients.DistinctBy(x => x.ConnectionId).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DefaultWebSocketBehavior> IncludeGroup(params string[] groups)
        {
            if (!groups.HasValue())
                return new List<DefaultWebSocketBehavior>();

            var clients = WebSocketStore.Groups.Where(x => groups.Equals(x.Key)).SelectMany(x => x.Value).ToList();

            return clients.DistinctBy(x => x.ConnectionId).ToList();
        }
    }
}
