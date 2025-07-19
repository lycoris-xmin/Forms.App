using Forms.App.Model.Bridges.Modules;

namespace Forms.App.Main.Bridges.Modules
{
    public class FormiumToastBridge : IFormiumToastBridge
    {
        private readonly MainWindow _mainWindow;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5);

        public FormiumToastBridge(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Info(string msg) => ToastAsync("info", msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Warn(string msg) => ToastAsync("warning", msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Success(string msg) => ToastAsync("success", msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Error(string msg) => ToastAsync("error", msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        private void ToastAsync(string type, string msg)
        {
            Task.Run(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    _mainWindow.ExecuteJavaScript($"window.$message.{type}('{Escape(msg)}')");
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string Escape(string msg)
        {
            return msg.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
        }
    }
}
