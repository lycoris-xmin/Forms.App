using Forms.App.Core.WsSocket.Impl;
using Lycoris.Common.Extensions;

namespace Forms.App.Core.WsSocket.Extensions
{
    public static class WebSocketExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="channel"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task SendMessageAsync(this List<DefaultWebSocketBehavior> clients, string channel, string content)
        {
            if (!clients.HasValue())
                return;

            foreach (var item in clients)
            {
                await item.SendMessageAsync(channel, content);
            }
        }
    }
}
