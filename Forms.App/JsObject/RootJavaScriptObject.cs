using Forms.App.Model;
using WinFormium.CefGlue;

namespace Forms.App.Main.JsObject
{

    internal class RootJavaScriptObject : JavaScriptObjectBuilder
    {
        internal override string JsObjectName => "root";

        public RootJavaScriptObject(CefBrowser? browser, InvokeOnUIThread invoke) : base(browser, invoke)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            this.AddMethod(nameof(GotoMainPage), GotoMainPage);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GotoMainPage() => this.Browser?.GetMainFrame().LoadUrl(AppSettings.StartUrl);
    }
}
