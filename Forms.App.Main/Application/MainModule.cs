using Forms.App.Main.Interceptors.ApiLogger;
using Lycoris.Autofac.Extensions;
using Lycoris.Autofac.Extensions.Impl;

namespace Forms.App.Main.Application
{
    public class MainModule : AutofacRegisterModule
    {
        public override void ModuleRegister(LycorisModuleBuilder builder)
        {
            builder.InterceptedBy<ApiLoggerInterceptor>();
        }
    }
}
