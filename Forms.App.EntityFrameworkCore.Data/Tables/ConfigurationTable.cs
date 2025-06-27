using Forms.App.EntityFrameworkCore.Data.Attributes;
using Forms.App.EntityFrameworkCore.Data.Constants;
using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Lycoris.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Forms.App.EntityFrameworkCore.Data.Tables
{
    /// <summary>
    /// 网站配置表
    /// </summary>
    [Table("Configuration")]
    public class ConfigurationTable : TableBaseEntity<string>
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string ConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 配置值
        /// </summary>
        [TableColumn(Sensitive = true, ColumnType = DbColumnType.TEXT)]
        public string Value { get; set; } = "";

        /// <summary>
        /// 配置保存格式
        /// </summary>
        [TableColumn(DefaultValue = ConfigurationValueTypeEnum.STRING)]
        public ConfigurationValueTypeEnum ValueType { get; set; } = ConfigurationValueTypeEnum.STRING;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<object> SeedData() => GetConfiguration(typeof(AppConfig).GetFields());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieids"></param>
        /// <returns></returns>
        private static List<object> GetConfiguration(FieldInfo[]? fieids)
        {
            var list = new List<object>();

            if (fieids != null && fieids.Length > 0)
            {
                foreach (var fieid in fieids)
                {
                    var attr = (ConfigurationAttribute?)Attribute.GetCustomAttribute(fieid, typeof(ConfigurationAttribute));
                    if (attr == null)
                        continue;

                    var data = new ConfigurationTable()
                    {
                        Id = (string?)fieid.GetRawConstantValue() ?? "",
                        ConfigName = attr.Description,
                        ValueType = attr.ValueType,
                        Value = attr.DefaultObject != null ? Activator.CreateInstance(attr.DefaultObject).ToJson(new JsonSerializerSettings()
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            NullValueHandling = NullValueHandling.Include,
                            DateFormatString = "yyyy-MM-dd HH:mm:ss",
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        }) : attr.DefaultValue ?? "",
                    };

                    list.Add(data);
                }
            }

            return list;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ConfigurationValueTypeEnum
    {
        /// <summary>
        /// 文本格式
        /// </summary>
        [Description("文本格式")]
        STRING = 0,
        /// <summary>
        /// Json格式
        /// </summary>
        [Description("Json格式")]
        JSON = 1
    }
}
