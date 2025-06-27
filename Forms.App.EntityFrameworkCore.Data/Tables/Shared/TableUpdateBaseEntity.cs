using Forms.App.EntityFrameworkCore.Data.Attributes;

namespace Forms.App.EntityFrameworkCore.Data.Tables.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    [TableIndex("UpdaterId")]
    public class TableUpdateBaseEntity<TPrimary> : TableCreateBaseEntity<TPrimary>
    {
        /// <summary>
        /// 更新者编号
        /// </summary>
        public long UpdaterId { get; set; } = 0;

        /// <summary>
        /// 更新时间
        /// </summary>
        [TableColumn(Required = false)]
        public DateTime? UpdateTime { get; set; }
    }
}
