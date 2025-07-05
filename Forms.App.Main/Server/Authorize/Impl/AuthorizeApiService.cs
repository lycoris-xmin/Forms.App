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
        [ApiMethod]
        public async Task<List<TestBInput>> TestAsync(TestInput input)
        {
            Console.WriteLine(1);
            await Task.CompletedTask;
            return input.B;
        }
    }

    public class TestInput
    {
        public string A { get; set; }

        public List<TestBInput> B { get; set; }

        public TestBInput[] C { get; set; }

        public TestCInput D { get; set; }
    }

    public class TestBInput
    {
        public string A { get; set; }
    }

    public class TestCInput
    {
        public string A { get; set; }

        public Dictionary<string, string> B { get; set; }
    }
}
