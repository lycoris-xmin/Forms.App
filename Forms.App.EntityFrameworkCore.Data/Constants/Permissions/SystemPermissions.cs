namespace B2B.Helper.EntityFrameworkCore.Data.Constants.Permissions
{
    public partial class AppPermissions
    {
        /// <summary>
        /// 
        /// </summary>
        public class System
        {
            /// <summary>
            /// 
            /// </summary>
            public class User
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "system.user.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "system.user.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "system.user.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "system.user.delete";
            }

            /// <summary>
            /// 
            /// </summary>
            public class Role
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "system.role.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "system.role.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "system.role.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "system.role.delete";
            }

            /// <summary>
            /// 
            /// </summary>
            public class EnumDictionary
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "system.enum.dictionary.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "system.enum.dictionary.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "system.enum.dictionary.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "system.enum.dictionary.delete";
            }

            /// <summary>
            /// 
            /// </summary>
            public class Setting
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "system.setting.view";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "system.setting.update";
            }
        }
    }
}
