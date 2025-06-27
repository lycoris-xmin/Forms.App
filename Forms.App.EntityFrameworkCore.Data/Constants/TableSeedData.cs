namespace Forms.App.EntityFrameworkCore.Data.Constants
{
    /// <summary>
    /// 数据库初始种子数据常量
    /// </summary>
    public class TableSeedData
    {
        /// <summary>
        /// 用户表
        /// </summary>
        public class UserData
        {
            /// <summary>
            /// Id
            /// </summary>
            public const long SuperAdminId = 19940618;

            /// <summary>
            /// 管理员帐号
            /// </summary>
            public const string DefaultAdminEmail = "13000000000@163.com";

            /// <summary>
            /// 管理员初始密码
            /// </summary>
            public const string DefaultAdminPassword = "123456789";

            /// <summary>
            /// 默认用户头像
            /// </summary>
            public const string DefaultAvatar = "/avatar/default.webp";

            /// <summary>
            /// 默认初始密码
            /// </summary>
            public const string DefaultPassword = "123456789";
        }

        /// <summary>
        /// 
        /// </summary>
        public class RoleData
        {
            /// <summary>
            /// 超级管理员
            /// </summary>
            public const int SuperAdminRoleId = 1000;

            /// <summary>
            /// 超级管理员
            /// </summary>
            public const string SuperAdminRoleName = "超级管理员";

            /// <summary>
            /// 管理员
            /// </summary>
            public const int AdminRoleId = 1001;

            /// <summary>
            /// 管理员
            /// </summary>
            public const string AdminRoleName = "管理员";

            /// <summary>
            /// 商户
            /// </summary>
            public const int TenantRoleId = 1002;

            /// <summary>
            /// 商户
            /// </summary>
            public const string TenantRoleName = "商户";

            /// <summary>
            /// 商户员工
            /// </summary>
            public const int TenantUserRoleId = 1003;

            /// <summary>
            /// 商户员工
            /// </summary>
            public const string TenantUserRoleName = "商户员工";
        }
    }
}
