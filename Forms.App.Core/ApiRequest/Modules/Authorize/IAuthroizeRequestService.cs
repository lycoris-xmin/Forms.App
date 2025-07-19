using Forms.App.Core.ApiRequest.Modules.Authorize.Models;
using Forms.App.Model.Models.Api;

namespace Forms.App.Core.ApiRequest.Modules.Authorize
{
    public interface IAuthroizeRequestService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DataOutput<AuthorizeValidationDataModel>?> ValidationAsync(AuthorizeValidationRequest input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DataOutput<AuthorizeLoginDataModel>?> LoginAsync(AuthorizeLoginRequest input);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<DataOutput<AuthorizeUserProfileDataModel>?> UserPorfileAsync();
    }
}
