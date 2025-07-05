using Forms.App.Core.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.App.Main.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseApiService
    {
        public IServiceProvider ServiceProvider { get; set; } = default!;

        private IServerLogger? _logger;

        protected IServerLogger Logger
        {
            get
            {
                if (_logger != null)
                    return _logger;

                var factory = ServiceProvider.GetRequiredService<IServerLoggerFactory>();
                _logger = factory.CreateLogger(this.GetType());

                return _logger;
            }
        }
    }
}
