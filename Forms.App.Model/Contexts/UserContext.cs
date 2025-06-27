namespace Forms.App.Model.Contexts
{
    public class UserContext
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public long Id { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public string NickName { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public string Avatar { get; set; } = default!;
    }
}
