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

        /// <summary>
        /// 
        /// </summary>
        public void Test()
        {
            this.Logger.Info("1");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [ApiMethod("TestReturn")]
        public string Test(string a)
        {
            this.Logger.Info(a);
            return a;
        }
    }
}
