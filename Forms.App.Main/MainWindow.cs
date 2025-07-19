using Forms.App.Core.Contexts;
using Forms.App.Main.Bridges;
using Forms.App.Main.Shared;
using Forms.App.Model;
using WinFormium.CefGlue;
using WinFormium.Sources.Formium.EventArgs;

namespace Forms.App.Main
{
    public class MainWindow : BaseFormiumWindow
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="provider"></param>
        public MainWindow(IServiceProvider provider) : base(provider)
        {
            this.Url = AppSettings.Route.MainUrl;
            this.WindowState = WinFormium.Sources.Formium.FormiumWindowState.Maximized;
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="args"></param>
        protected override void OnReady(BrowserEventArgs args)
        {
            FormAppContext.RegisterFormium(this);
            FormAppContext.RegisterBrowser(this.Browser!);

            FormAppContext.Bridge = new FormiumBridge(this);

            this.KeyEvent += MainWindow_KeyEvent;
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="args"></param>
        /// <param name="mainFrame"></param>
        protected override void PageOnReady(PageLoadStartEventArgs args, CefFrame? mainFrame)
        {
            //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MainWindow_KeyEvent(object? sender, KeyEventEventArgs e)
        {
            if (e.KeyEvent.EventType == CefKeyEventType.KeyUp)
            {
                switch (e.KeyEvent.WindowsKeyCode)
                {
                    case 116:
                        {
                            // F5
                            if (AppSettings.IsDebugger)
                                e.Browser.Reload();
                        }
                        break;
                    case 123:
                        {
                            // F12
                            if (AppSettings.IsDebugger)
                                this.ShowDevTools();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
