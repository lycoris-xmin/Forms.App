using Forms.App.EntityFrameworkCore.Data.Attributes;

namespace Forms.App.EntityFrameworkCore.Data.Tables.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    [TableIndex("CreatorId")]
    public class TableCreateBaseEntity<TPrimary> : TableBaseEntity<TPrimary>
    {
        /// <summary>
        /// 创建者编号
        /// </summary>
        public virtual long CreatorId { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
