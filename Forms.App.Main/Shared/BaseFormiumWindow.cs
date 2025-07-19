using Forms.App.Core.Logging;
using Forms.App.Main.JsObject;
using Forms.App.Model;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using WinFormium;
using WinFormium.CefGlue;
using WinFormium.Sources.Formium.EventArgs;
using WinFormium.Sources.Formium.Forms;
using WinFormium.Sources.Formium.Forms.@base;
using static Forms.App.Main.JsObject.Builder.JavaScriptObjectBuilder;

namespace Forms.App.Main.Shared
{
    public abstract class BaseFormiumWindow : Formium
    {
        /// <summary>
        /// 
        /// </summary>
        public CefBrowser? BrowserInstance { get => this.Browser; }

        /// <summary>
        /// 
        /// </summary>
        public InvokeOnUIThread InvokeThead { get => this.InvokeOnUIThread; }

        /// <summary>
        /// 
        /// </summary>
        protected readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// 日志
        /// </summary>
        protected readonly IServerLogger Logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="provider"></param>
        public BaseFormiumWindow(IServiceProvider provider)
        {
            this.ServiceProvider = provider;
            this.Logger = provider.GetRequiredService<IServerLoggerFactory>().CreateLogger(this.GetType());
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
            style.StartCentered = StartCenteredMode.CenterScreen;
            style.ShowInTaskbar = true;
            style.WindowState = WinFormium.Sources.Formium.FormiumWindowState.FullScreen;

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

            OnReady(args);

            base.OnLoaded(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnPageLoadStart(PageLoadStartEventArgs args)
        {
            var frame = this.Browser?.GetMainFrame();

            if (frame != null)
            {
                if (!frame.Url.IsNullOrEmpty())
                {
                    var registerMap = this.GetOrCreateJavaScriptObject(new Uri(frame.Url));

                    if (registerMap.HasValue())
                    {
                        var hbrjso = this.BeginRegisterJavaScriptObject(frame);

                        registerMap.ForEach(x => this.RegisterJavaScriptObject(hbrjso, x.Key, x.Value));

                        this.EndRegisterJavaScriptObject(hbrjso);
                    }
                }
            }

            PageOnReady(args, frame);

            base.OnPageLoadStart(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnPageAddressChange(PageAddressChangeEventArgs args)
        {
            this.Logger.Error($"page change -> {(args.Frame.IsMain ? "main frame" : "sub frame")} - {args.Address}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnPageLoadError(PageLoadErrorEventArgs args)
        {
            base.OnPageLoadError(args);
            this.Logger.Error($"page load error -> {args.ErrorText} - {args.ErrorCode}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected abstract void OnReady(BrowserEventArgs args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="mainFrame"></param>
        protected abstract void PageOnReady(PageLoadStartEventArgs args, CefFrame? mainFrame);
    }
}
