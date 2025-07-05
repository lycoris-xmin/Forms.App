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

        [ApiMethod]
        public async Task TestAsync(TestInput input)
        {
            Console.WriteLine(1);
            await Task.CompletedTask;
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
