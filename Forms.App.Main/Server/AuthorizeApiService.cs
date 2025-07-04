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
        public TestRe Test(string a)
        {
            this.Logger.Info(a);
            return  new TestRe();
        }
    }

    public class TestRe()
    {
        public string Name { get; set; } = "123";

        private string ddd { get; set; }

        public TestReCC CC { get; set; } = new TestReCC();
    }

    public class TestReCC()
    {
        public string Name { get; set; } = "123";

        private string ddd { get; set; }

        public string CC { get; set; }
    }
}
