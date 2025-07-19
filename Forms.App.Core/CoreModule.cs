using Lycoris.Autofac.Extensions;
using Lycoris.Snowflakes;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.App.Core
{
    public class CoreModule : AutofacRegisterModule
    {
        public override void SerivceRegister(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSnowflake(opt => opt.StartTimeStamp = new DateTime(2024, 1, 1)).AsHelper();
        }
    }
}
