using Forms.App.Core.Contexts;
using Forms.App.Main.Application;
using Forms.App.Main.JsObject.Builder;
using Vanara.Extensions;
using WinFormium.CefGlue;

namespace Forms.App.Main.JsObject.Objects
{
    [JavaScriptRegister("Launch", Host = "launch.app.local")]
    public class LaunchJavaScriptObject : JavaScriptObjectBuilder
    {
        public LaunchJavaScriptObject(CefBrowser? browser, InvokeOnUIThread invoke) : base(browser, invoke)
        {
        }

        protected override void Initialize()
        {
            AddMethod(Run);

            AddMethod(GetLaunchStepAsync);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Run() => Task.Run(() => ApplicationHostedService.StartAsync());

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<LanchStepModel> GetLaunchStepAsync()
        {
            var data = new LanchStepModel()
            {
                Count = typeof(LaunchStepEnum).GetFields().Count(),
                StepCount = (int)FormAppContext.LaunchStep,
                StepDescription = FormAppContext.LaunchStep.GetDescription()!
            };

            await Task.Delay(Random.Shared.Next(500, 1500));

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        private class LanchStepModel
        {
            /// <summary>
            /// 
            /// </summary>
            public int Count { get; set; } = 0;

            /// <summary>
            /// 
            /// </summary>
            public int StepCount { get; set; } = 0;

            /// <summary>
            /// 
            /// </summary>
            public string StepDescription { get; set; } = default!;

            /// <summary>
            /// 
            /// </summary>
            public int Process { get => (int)Math.Ceiling(StepCount / (double)Count) * 100; }
        }
    }
}
