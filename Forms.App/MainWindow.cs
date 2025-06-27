using Forms.App.Core.Logging;
using WinFormium;
using WinFormium.CefGlue;
using WinFormium.Sources.Formium.EventArgs;
using WinFormium.Sources.Formium.Forms;
using WinFormium.Sources.Formium.Forms.@base;

namespace Forms.App.Main
{
    public class MainWindow : Formium
    {
        private readonly IServerLogger _logger;

        public MainWindow()
        {
            Url = "https://www.google.com";
        }

        protected override FormStyle ConfigureWindowStyle(WindowStyleBuilder builder)
        {
            // 此处配置窗口的样式和属性，或留空以使用默认样式
            var style = builder.UseSystemForm();

            style.TitleBar = true;

            style.DefaultAppTitle = "My first WinFomrim app";

            return style;
        }

        protected override void OnLoaded(BrowserEventArgs args)
        {
            ShowDevTools();

            base.OnLoaded(args);
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            try
            {
                CefRuntime.QuitMessageLoop();   // 如果你用了 CEF 消息循环
                CefRuntime.Shutdown();          // 或 Cef.Shutdown();
            }
            catch (Exception)
            {
                throw;
            }

            base.OnClosing(args);
        }
    }
}
