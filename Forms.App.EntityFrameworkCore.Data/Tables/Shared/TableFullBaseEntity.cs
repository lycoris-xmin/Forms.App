using Forms.App.EntityFrameworkCore.Data.Attributes;

namespace Forms.App.EntityFrameworkCore.Data.Tables.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    [TableIndex("DeleterId")]
    public class TableFullBaseEntity<TPrimary> : TableUpdateBaseEntity<TPrimary>
    {
        /// <summary>
        /// 删除者编号
        /// </summary>
        public long DeleterId { get; set; } = 0;

        /// <summary>
        /// 删除时间
        /// </summary>
        [TableColumn(Required = false)]
        public DateTime? DeleteTime { get; set; }
    }
}
