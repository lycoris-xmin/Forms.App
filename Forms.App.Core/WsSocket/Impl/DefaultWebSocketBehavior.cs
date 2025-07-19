using Forms.App.Core.WsSocket.Stores;
using Lycoris.Common.Extensions;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Forms.App.Core.WsSocket.Impl
{
    public class DefaultWebSocketBehavior : WebSocketBehavior
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionId { get; private set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        protected override void OnOpen()
        {
            this.ConnectionId = ID;

            WebSocketStore.Clients[this.ConnectionId] = this;

            OnConnectionAsync(this.ConnectionId).RunSync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClose(CloseEventArgs e)
        {
            var item = WebSocketStore.Clients.FirstOrDefault(kv => kv.Value == this);

            if (!string.IsNullOrEmpty(item.Key))
                WebSocketStore.Clients.TryRemove(item.Key, out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnConnectionAsync(string connectionId) => Task.CompletedTask;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        protected virtual Task OnDisConnectionAsync(string connectionId) => Task.CompletedTask;

        /// <summary>
        /// 
        /// </summary>
        protected WebSocketGroup Groups { get => new WebSocketGroup(this); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="content"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public async Task<bool> SendMessageAsync(string channel, string content, int timeoutSeconds = 30)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            this.SendAsync(new { channel, content }.ToJson(), success => tcs.TrySetResult(success));

            // 添加超时机制
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));

            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            // 超时未响应
            if (completedTask == timeoutTask)
                return false;

            return await tcs.Task;
        }
    }
}
