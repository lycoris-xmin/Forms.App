using Forms.App.EntityFrameworkCore.Data.Attributes;

namespace Forms.App.EntityFrameworkCore.Data.Tables.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITableBaseEntity
    {
        /// <summary>
        /// 种子数据
        /// </summary>
        /// <returns></returns>
        List<object> SeedData();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    [TableIndex("TenantId")]
    public class TableBaseEntity<TPrimary> : ITableBaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Snowflake]
        [TableColumn(IsPrimary = true, IsIdentity = true)]
        public virtual TPrimary Id { get; set; } = default!;

        /// <summary>
        /// 乐观锁
        /// </summary>
        [TableColumn(IsRowVersion = true)]
        public virtual byte[]? RowVersion { get; set; }

        /// <summary>
        /// 商户编号
        /// </summary>
        public virtual long TenantId { get; set; }

        /// <summary>
        /// 种子数据
        /// </summary>
        /// <returns></returns>
        public virtual List<object> SeedData() => new();
    }
}
