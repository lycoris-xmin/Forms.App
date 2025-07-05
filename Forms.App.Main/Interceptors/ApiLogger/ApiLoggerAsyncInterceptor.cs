using Castle.DynamicProxy;
using Forms.App.Core.Logging;
using Forms.App.Main.Interceptors.Shared;
using Forms.App.Main.JsObject.Objects;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using System.Text;

namespace Forms.App.Main.Interceptors.ApiLogger
{
    [AutofacRegister(ServiceLifeTime.Transient, IsInterceptor = true)]
    public class ApiLoggerAsyncInterceptor : AsyncInterceptorHandler<ApiMethodAttribute>
    {
        private readonly IServerLogger _logger;

        public ApiLoggerAsyncInterceptor(IServerLoggerFactory factory)
        {
            _logger = factory.CreateLogger<ApiLoggerAsyncInterceptor>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        public override void InterceptHandlger(IInvocation invocation, ApiMethodAttribute? attribute)
        {
            var inputTime = this.InputLogger(invocation);

            try
            {
                invocation.Proceed();

                this.OutputLogger(inputTime, invocation);
            }
            catch (Exception ex)
            {
                this.OutputLogger(inputTime, invocation, ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public override async Task InterceptHandlgerAsync(IInvocation invocation, ApiMethodAttribute? attribute)
        {
            var inputTime = this.InputLogger(invocation);

            try
            {
                invocation.Proceed();
                var task = (Task)invocation.ReturnValue;
                await task;

                this.OutputLogger(inputTime, invocation);
            }
            catch (Exception ex)
            {
                this.OutputLogger(inputTime, invocation, ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public override async Task<TResult> InterceptHandlgerAsync<TResult>(IInvocation invocation, ApiMethodAttribute? attribute)
        {
            var inputTime = this.InputLogger(invocation);

            try
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                var result = await task;

                this.OutputLogger(inputTime, invocation, result);

                return result;
            }
            catch (Exception ex)
            {
                this.OutputLogger(inputTime, invocation, ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        private DateTime InputLogger(IInvocation invocation)
        {
            var mehtod = $"{invocation.TargetType.FullName}.{invocation.MethodInvocationTarget.Name}";

            try
            {
                this._logger.Info($"{mehtod} input{GetArgumentToJsonString(invocation)}");
            }
            catch
            {
                if (invocation.Arguments.HasValue())
                    this._logger.Info($"{mehtod} input - {invocation.Arguments.ToJson()}");
                else
                    this._logger.Info($"{mehtod} input");
            }

            return DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputTime"></param>
        /// <param name="invocation"></param>
        /// <param name="ex"></param>
        private void OutputLogger(DateTime inputTime, IInvocation invocation, Exception? ex = null)
        {
            var mehtod = $"{invocation.TargetType.Name}/{invocation.MethodInvocationTarget.Name}";
            if (ex == null)
                this._logger.Info($"{mehtod} output - {(DateTime.Now - inputTime).TotalMilliseconds:0.00}ms");
            else
                this._logger.Error($"{mehtod} output - {ex.Message} - {(DateTime.Now - inputTime).TotalMilliseconds:0.00}ms", ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="inputTime"></param>
        /// <param name="invocation"></param>
        /// <param name="result"></param>
        /// <param name="ex"></param>
        private void OutputLogger<TResult>(DateTime inputTime, IInvocation invocation, TResult result, Exception? ex = null)
        {
            var mehtod = $"{invocation.TargetType.FullName}.{invocation.MethodInvocationTarget.Name}";

            if (ex == null)
                this._logger.Info($"{mehtod} output - {GetResultToJsonString(result)} - {(DateTime.Now - inputTime).TotalMilliseconds:0.00}ms");
            else
                this._logger.Error($"{mehtod} output - {ex.Message} - {(DateTime.Now - inputTime).TotalMilliseconds:0.00}ms", ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private static string GetArgumentToJsonString(IInvocation invocation)
        {
            if (!invocation.Arguments.HasValue())
                return "";

            var argsType = invocation.Arguments.Select(x => x.GetType()).ToArray();
            if (!argsType.HasValue())
                return "";

            var str = new StringBuilder(" - ");

            if (argsType.Length == 1)
            {
                if (invocation.Arguments == null)
                    return "";

                if (argsType[0] == typeof(string))
                    str.Append((string)invocation.Arguments[0]);
                else if (!argsType[0].IsClass)
                    str.Append((string)invocation.Arguments[0]);
                else
                    str.Append(invocation.Arguments[0].ToJson());

                return str.ToString();
            }

            str.Append("{");

            for (int i = 0; i < argsType.Length; i++)
            {
                var type = argsType[i];

                if (invocation.Arguments.Length < i + 1)
                    continue;

                var arg = invocation.Arguments[i];

                if (arg == null)
                    str.Append($"\"{type.Name}\":null,");

                if (argsType[0] == typeof(string))
                    str.Append($"\"{type.Name}\":\"{(string)invocation.Arguments[0]}\"");
                else if (!argsType[0].IsClass)
                    str.Append($"\"{type.Name}\":\"{(string)invocation.Arguments[0]}\"");
                else
                    str.Append($"\"{type.Name}\":\"{invocation.Arguments[0].ToJson()}\"");
            }

            str.Append("}");

            return str.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string GetResultToJsonString<TResult>(TResult? result)
        {
            if (result == null)
                return "";

            var type = typeof(TResult);

            if (type == typeof(string))
                return (result as string)!;
            else if (!type.IsClass)
                return result.ToString()!;
            else
                return result.ToJson();
        }
    }
}
