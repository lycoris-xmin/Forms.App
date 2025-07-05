using Forms.App.Main.Server.Authorize.Impl;

namespace Forms.App.Main.Server.Authorize
{
    public interface IAuthorizeApiService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<TestBInput>> TestAsync(TestInput input);
    }
}
