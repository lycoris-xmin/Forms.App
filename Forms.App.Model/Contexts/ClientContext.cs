using Lycoris.Autofac.Extensions;

namespace Forms.App.Model.Contexts
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class ClientContext
    {
        /// <summary>
        /// 
        /// </summary>
        public UserContext User { get; set; } = default!;
    }
}
