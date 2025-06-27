
namespace B2B.Helper.EntityFrameworkCore.Data.Constants.Permissions
{
    public partial class AppPermissions
    {
        /// <summary>
        /// 
        /// </summary>
        public class Friend
        {
            /// <summary>
            /// 
            /// </summary>
            public class List
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "friend.list.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "friend.list.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "friend.list.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "friend.list.delete";
            }
            /// <summary>
            /// 
            /// </summary>
            public class Request
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "friend.request.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "friend.request.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "friend.request.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "friend.request.delete";
            }

            /// <summary>
            /// 
            /// </summary>
            public class Audit
            {
                /// <summary>
                /// 
                /// </summary>
                public const string VIEW = "friend.audit.view";

                /// <summary>
                /// 
                /// </summary>
                public const string CREATE = "friend.audit.create";

                /// <summary>
                /// 
                /// </summary>
                public const string UPDATE = "friend.audit.update";

                /// <summary>
                /// 
                /// </summary>
                public const string DELETE = "friend.audit.delete";
            }
        }
    }
}
