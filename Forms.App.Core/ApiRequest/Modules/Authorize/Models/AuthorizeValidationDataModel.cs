namespace Forms.App.Core.ApiRequest.Modules.Authorize.Models
{
    public class AuthorizeValidationDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string OathCode { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public bool GoogleAuthenticator { get; set; }
    }
}
