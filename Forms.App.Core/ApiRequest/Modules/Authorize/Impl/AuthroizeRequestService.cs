using Forms.App.Core.ApiRequest.Modules.Authorize.Models;
using Forms.App.Model.Models.Api;

namespace Forms.App.Core.ApiRequest.Modules.Authorize.Impl
{
    public class AuthroizeRequestService : IAuthroizeRequestService
    {
        private ITransferRequestService Api { get; init; }

        public AuthroizeRequestService(ITransferRequestService api)
        {
            Api = api;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<DataOutput<AuthorizeValidationDataModel>?> ValidationAsync(AuthorizeValidationRequest input)
            => Api.PostAsync<DataOutput<AuthorizeValidationDataModel>>("/api/authorize/validation", input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<DataOutput<AuthorizeLoginDataModel>?> LoginAsync(AuthorizeLoginRequest input)
            => Api.PostAsync<DataOutput<AuthorizeLoginDataModel>>("/api/authorize/login", input);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<DataOutput<AuthorizeUserProfileDataModel>?> UserPorfileAsync()
            => Api.GetAsync<DataOutput<AuthorizeUserProfileDataModel>>("/api/authorize/userprofile");
    }
}
