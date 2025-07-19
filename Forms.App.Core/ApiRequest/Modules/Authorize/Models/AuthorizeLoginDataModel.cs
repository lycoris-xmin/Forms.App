namespace Forms.App.Core.ApiRequest.Modules.Authorize.Models
{
    public class AuthorizeLoginDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; set; } = default!;
    }
}
