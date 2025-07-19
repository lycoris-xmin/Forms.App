using Forms.App.Model.Bridges.Modules;

namespace Forms.App.Main.Bridges.Modules
{
    public class FormiumNotifyBridge : IFormiumNotifyBridge
    {
        private readonly MainWindow _mainWindow;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3); // 最多3个并发

        public FormiumNotifyBridge(MainWindow mainWindow) => _mainWindow = mainWindow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void Info(string title, string? message = null) => Notify("info", title, message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void Warn(string title, string? message = null) => Notify("warning", title, message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void Success(string title, string? message = null) => Notify("success", title, message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void Error(string title, string? message = null) => Notify("error", title, message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private void Notify(string type, string title, string? message)
        {
            Task.Run(async () =>
            {
                await _semaphore.WaitAsync(); // 限制最大并发
                try
                {
                    _mainWindow.ExecuteJavaScript(GenerateExcuteCode(type, title, message));
                }
                finally
                {
                    _semaphore.Release(); // 执行完释放
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fnName"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GenerateExcuteCode(string fnName, string title, string? message)
        {
            var key = Guid.NewGuid().ToString("N");

            var js = $@"
                window.$notification.{fnName}({{
                  key: '{key}',
                  message: '{EscapeJsString(title)}',
                  description: '{EscapeJsString(message ?? "")}',
                }});
                ";

            return js;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string EscapeJsString(string input) => input.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");
    }
}
