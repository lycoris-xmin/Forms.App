using Lycoris.Autofac.Extensions;

namespace Forms.App.Services.AppServices
{
    public interface ITestService
    {
        void Test();
    }

    [AutofacRegister(ServiceLifeTime.Transient)]
    public class TestService : ITestService
    {
        public void Test()
        {
            Console.WriteLine(1);
        }
    }
}
