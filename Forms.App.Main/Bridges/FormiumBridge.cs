using Forms.App.Main.Bridges.Modules;
using Forms.App.Model.Bridges;
using Forms.App.Model.Bridges.Modules;

namespace Forms.App.Main.Bridges
{
    /// <summary>
    /// 
    /// </summary>
    public class FormiumBridge : IFormiumBridge
    {
        private readonly MainWindow _mainWindow;

        public FormiumBridge(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        public IFormiumPageBridge Page => new FormiumPageBridge(_mainWindow);

        /// <summary>
        /// 
        /// </summary>
        public IFormiumToastBridge Toast => new FormiumToastBridge(_mainWindow);

        /// <summary>
        /// 
        /// </summary>
        public IFormiumJavascriptBridge JavaScript => new FormiumJavascriptBridge(_mainWindow);

        /// <summary>
        /// 
        /// </summary>
        public IFormiumNotifyBridge Notify => new FormiumNotifyBridge(_mainWindow);

        /// <summary>
        /// 
        /// </summary>
        public IFormiumEventBridge Event => new FormiumEventBridge(_mainWindow);
    }
}
