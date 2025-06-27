namespace B2B.Helper.EntityFrameworkCore.Data.Constants.Permissions
{
    public partial class AppPermissions
    {
        /// <summary>
        /// 
        /// </summary>
        public class Platform
        {
            /// <summary>
            /// 
            /// </summary>
            public class Tenant
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "platform.tenant.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "platform.tenant.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "platform.tenant.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "platform.tenant.delete";
            }

            /// <summary>
            /// 
            /// </summary>
            public class TenantUser
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "platform.tenant.user.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "platform.tenant.user.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "platform.tenant.user.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "platform.tenant.user.delete";
            }

            /// <summary>
            /// 
            /// </summary>
            public class AlipayLovePay
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "platform.alipay.lovepay.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "platform.alipay.lovepay.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "platform.alipay.lovepay.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "platform.alipay.lovepay.delete";
            }

            /// <summary>
            /// 
            /// </summary>
            public class CategoryRepeatPurchase
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "platform.category.repeatpurchase.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "platform.category.repeatpurchase.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "platform.category.repeatpurchase.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "platform.category.repeatpurchase.delete";
            }


            /// <summary>
            /// 
            /// </summary>
            public class Setting
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "platform.setting.view";
                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "platform.setting.update";

            }
        }
    }
}
