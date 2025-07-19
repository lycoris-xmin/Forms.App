using Forms.App.Model.Bridges.Modules;
using Lycoris.Common.Extensions;

namespace Forms.App.Main.Bridges.Modules
{
    public class FormiumEventBridge : IFormiumEventBridge
    {
        private readonly MainWindow _mainWindow;

        public FormiumEventBridge(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public void Send(IFormiumEventTypeEnum @event) => SendMessage(@event, "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public void Send(IFormiumEventTypeEnum @event, string content) => SendMessage(@event, content);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void Send<T>(IFormiumEventTypeEnum @event, T content) where T : class, new()
        {
            var json = content.ToJson();
            SendMessage(@event, json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private void SendMessage(IFormiumEventTypeEnum @event, string content)
        {
            Task.Run(() =>
            {
                var js = $"window.$formMessage.send('{@event.ToString()}','{content}')";
                _mainWindow.ExecuteJavaScript(js);
            });
        }
    }
}
