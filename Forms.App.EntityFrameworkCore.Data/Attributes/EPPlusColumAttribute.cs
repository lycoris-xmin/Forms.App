namespace Forms.App.EntityFrameworkCore.Data.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EPPlusColumAttribute : Attribute
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 是否需要合并
        /// </summary>
        public bool IsMage { get; set; } = false;

        /// <summary>
        /// 可选 主要用于多链接导入的情况,部分字段需要舍弃
        /// </summary>
        public bool IsFlexible { get; set; } = false;
    }
}
