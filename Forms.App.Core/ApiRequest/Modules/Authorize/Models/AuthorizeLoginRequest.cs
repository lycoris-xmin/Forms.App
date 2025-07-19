using System.ComponentModel.DataAnnotations;

namespace Forms.App.Core.ApiRequest.Modules.Authorize.Models
{
    public class AuthorizeLoginRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        [Required]
        public string? OathCode { get; set; }

        /// <summary>
        /// 谷歌二次身份认证器
        /// </summary>
        public string? GoogleAuthenticator { get; set; }

        /// <summary>
        /// 记住我
        /// </summary>
        public bool Remember { get; set; } = true;
    }
}
