using Castle.DynamicProxy;
using Lycoris.Autofac.Extensions;

namespace Forms.App.Main.Interceptors.ApiLogger
{
    [AutofacRegister(ServiceLifeTime.Transient, IsInterceptor = true)]
    public class ApiLoggerInterceptor : IInterceptor
    {
        private readonly ApiLoggerAsyncInterceptor _AsyncInterceptor;

        public ApiLoggerInterceptor(ApiLoggerAsyncInterceptor AsyncInterceptor)
        {
            _AsyncInterceptor = AsyncInterceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            _AsyncInterceptor.ToInterceptor().Intercept(invocation);
        }
    }
}
