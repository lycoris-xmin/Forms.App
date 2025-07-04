using Forms.App.Main.JsObject.Objects;
using Forms.App.Main.Shared;
using Lycoris.Autofac.Extensions;

namespace Forms.App.Main.Server
{
    /// <summary>
    /// 
    /// </summary>
    [ApiModule("Authorize")]
    [AutofacRegister(ServiceLifeTime.Transient)]
    public class AuthorizeApiService : BaseApiService
    {
        public AuthorizeApiService(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}
