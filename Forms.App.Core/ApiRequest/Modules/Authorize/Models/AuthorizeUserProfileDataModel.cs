namespace Forms.App.Core.ApiRequest.Modules.Authorize.Models
{
    public class AuthorizeUserProfileDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Wechat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLogin { get; set; } = false;

        /// <summary>
        /// 是否设置了结算
        /// </summary>
        public bool IsSettlement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TenantType { get; set; }
    }
}
