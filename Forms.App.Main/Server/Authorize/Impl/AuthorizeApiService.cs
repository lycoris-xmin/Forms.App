using Forms.App.Main.JsObject.Objects;
using Forms.App.Main.Shared;
using Lycoris.Autofac.Extensions;

namespace Forms.App.Main.Server.Authorize.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [ApiModule("Authorize")]
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true, EnableInterceptor = true)]
    public class AuthorizeApiService : BaseApiService, IAuthorizeApiService
    {

    }
}
