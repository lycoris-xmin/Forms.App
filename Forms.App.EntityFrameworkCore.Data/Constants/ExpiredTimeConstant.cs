namespace Forms.App.EntityFrameworkCore.Data.Constants
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpiredTimeConstant
    {
        /// <summary>
        /// 
        /// </summary>
        public class Device
        {
            /// <summary>
            /// 
            /// </summary>
            public const double ALIVE_MINUTES = 1.5;
        }

        /// <summary>
        /// 
        /// </summary>
        public class Command
        {
            /// <summary>
            /// 接取超时时间
            /// </summary>
            public const int ACCESS_EXPIRED_MINUTES = 5;

            /// <summary>
            /// 普通指令执行超时时间
            /// </summary>
            public const int DOING_EXPIRED_MINTES = 10;

            /// <summary>
            /// 任务指令执行超时时间
            /// </summary>
            public const int DOING_PLANTASK_EXPIRED_MINTES = 90;

            /// <summary>
            /// 任务指令执行超时时间
            /// </summary>
            public const int DOING_MULTIPLE_PLANTASK_EXPIRED_MINTES = 120;

            /// <summary>
            /// 任务指令等待付款执行超时时间
            /// </summary>
            public const int WAIT_PAY_PLANTASK_EXPIRED_MINTES = 18;
        }
    }
}
