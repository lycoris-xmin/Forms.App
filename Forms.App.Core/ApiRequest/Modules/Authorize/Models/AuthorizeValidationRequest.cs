using System.ComponentModel.DataAnnotations;

namespace Forms.App.Core.ApiRequest.Modules.Authorize.Models
{
    public class AuthorizeValidationRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [Required, EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required, MinLength(6), MaxLength(18)]
        public string? Password { get; set; }
    }
}
