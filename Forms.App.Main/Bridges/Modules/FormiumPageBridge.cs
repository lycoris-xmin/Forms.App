using Forms.App.Model.Bridges.Modules;

namespace Forms.App.Main.Bridges.Modules
{
    public class FormiumPageBridge : IFormiumPageBridge
    {
        private readonly MainWindow _mainWindow;

        public FormiumPageBridge(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task GoForwardAsync()
        {
            _mainWindow.GoForward();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task GoBackAsync()
        {
            _mainWindow.GoBack();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ReloadAsync()
        {
            _mainWindow.Reload();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GoAsync(string url)
        {
            if (_mainWindow != null)
                _mainWindow.Url = url;

            await Task.CompletedTask;
        }
    }
}
