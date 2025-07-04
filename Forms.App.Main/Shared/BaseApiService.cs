using Forms.App.Core.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.App.Main.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseApiService
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly IServerLogger Logger;

        public BaseApiService(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;

            var factory = serviceProvider.GetRequiredService<IServerLoggerFactory>();
            this.Logger = factory.CreateLogger(this.GetType());
        }
    }
}
