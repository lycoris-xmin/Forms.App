using Forms.App.Main.JsObject.Builder;
using Lycoris.Common.Extensions;
using WinFormium.CefGlue;
using WinFormium.Sources.JavaScript.JavaScriptEngine;

namespace Forms.App.Main.JsObject.Objects
{
    [JavaScriptRegister("Calculate")]
    internal class CalculateJavaScriptObject : JavaScriptObjectBuilder
    {
        public CalculateJavaScriptObject(CefBrowser? browser, InvokeOnUIThread invoke) : base(browser, invoke)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            this.AddMethod("Add", this.Addition);

            this.AddMethod("Sub", this.Subtraction);

            this.AddMethod("Multip", this.Multiplication);

            this.AddMethod("Division", this.Division);
        }

        /// <summary>
        /// 加法
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private string Addition(JavaScriptArray array)
        {
            var result = array.Select(x => x.GetDecimal()).Aggregate(0m, (a, b) => a + b);
            return result.ToString();
        }

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private string Subtraction(JavaScriptArray array)
        {
            if (!array.HasValue())
                return "0";

            if (array.Count == 1)
                return array.First().GetDecimal().ToString();

            var result = array.Skip(1).Select(x => x.GetDecimal()).Aggregate(array.First().GetDecimal(), (a, b) => a - b);
            return result.ToString();
        }

        /// <summary>
        /// 乘法
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private string Multiplication(JavaScriptArray array)
        {
            if (!array.HasValue())
                return "0";

            var result = 1m;

            foreach (var item in array.Select(x => x.GetDecimal()))
                result = result * item;

            return result.ToString();
        }

        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private string Division(JavaScriptArray array)
        {
            if (!array.HasValue())
                return "0";

            var result = 1m;

            foreach (var item in array.Select(x => x.GetDecimal()))
                result = result / item;

            return result.ToString();
        }
    }
}
