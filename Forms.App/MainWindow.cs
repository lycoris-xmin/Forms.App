using Forms.App.Main.Shared;
using Forms.App.Model;
using WinFormium.CefGlue;
using WinFormium.Sources.Formium.EventArgs;

namespace Forms.App.Main
{
    public class MainWindow : BaseFormium
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
            //
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="args"></param>
        /// <param name="mainFrame"></param>
        protected override void PageOnReady(PageLoadStartEventArgs args, CefFrame? mainFrame)
        {

        }
    }
}
