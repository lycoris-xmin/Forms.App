using Forms.App.Main.JsObject.Builder;
using Forms.App.Model;
using WinFormium.CefGlue;

namespace Forms.App.Main.JsObject.Objects
{
    [JavaScriptRegister("Root")]
    internal class RootJavaScriptObject : JavaScriptObjectBuilder
    {
        public RootJavaScriptObject(CefBrowser? browser, InvokeOnUIThread invoke) : base(browser, invoke)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            // 
            AddMethod(GotoMainPage);

            // 
            AddMethod(GotoLaunchPage);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GotoMainPage() => Browser?.GetMainFrame().LoadUrl(AppSettings.Route.MainUrl);

        /// <summary>
        /// 
        /// </summary>
        private void GotoLaunchPage() => Browser?.GetMainFrame().LoadUrl(AppSettings.Route.LaunchUrl);
    }
}
