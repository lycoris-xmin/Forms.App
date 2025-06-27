using Forms.App.Core.Logging;
using Forms.App.Main.JsObject;
using Forms.App.Model;
using WinFormium;
using WinFormium.Sources.Formium.EventArgs;
using WinFormium.Sources.Formium.Forms;
using WinFormium.Sources.Formium.Forms.@base;

namespace Forms.App.Main
{
    public class MainWindow : Formium
    {
        private readonly IServerLogger _logger;

        public MainWindow(IServerLoggerFactory factory)
        {
            this.Url = AppSettings.StartUrl;
            this.WindowState = WinFormium.Sources.Formium.FormiumWindowState.Maximized;
            _logger = factory.CreateLogger<MainWindow>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override FormStyle ConfigureWindowStyle(WindowStyleBuilder builder)
        {
            // 此处配置窗口的样式和属性，或留空以使用默认样式
            var style = builder.UseSystemForm();

            style.TitleBar = true;
            style.DefaultAppTitle = "商友联盟";

            return style;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnLoaded(BrowserEventArgs args)
        {
            if (AppSettings.IsDebugger && !this.HasDevTools)
                ShowDevTools();

            var frame = this.Browser?.GetMainFrame();

            if (frame != null)
            {
                var jsMapList = new List<JavaScriptObjectMap>
                {
                    new RootJavaScriptObject().Build()
                };

                var hbrjso = this.BeginRegisterJavaScriptObject(frame);

                jsMapList.ForEach(x => this.RegisterJavaScriptObject(hbrjso, x.Name, x.JsObject));

                this.EndRegisterJavaScriptObject(hbrjso);
            }

            base.OnLoaded(args);
        }
    }
}
