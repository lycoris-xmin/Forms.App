using Castle.DynamicProxy;
using Forms.App.Core.Logging;
using Forms.App.Main.Interceptors.Shared;
using Lycoris.Autofac.Extensions;

namespace Forms.App.Main.Interceptors.ApiLogger
{
    [AutofacRegister(ServiceLifeTime.Transient, IsInterceptor = true)]
    public class ApiLoggerAsyncInterceptor : AsyncInterceptorHandler<ApiLoggerAttribute>
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
        public override void InterceptHandlger(IInvocation invocation, ApiLoggerAttribute? attribute)
        {
            base.InterceptHandlger(invocation, attribute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public override Task InterceptHandlgerAsync(IInvocation invocation, ApiLoggerAttribute? attribute)
        {
            return base.InterceptHandlgerAsync(invocation, attribute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invocation"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public override Task<TResult> InterceptHandlgerAsync<TResult>(IInvocation invocation, ApiLoggerAttribute? attribute)
        {
            return base.InterceptHandlgerAsync<TResult>(invocation, attribute);
        }
    }
}
