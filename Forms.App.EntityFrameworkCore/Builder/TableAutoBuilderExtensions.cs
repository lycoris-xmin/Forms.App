using Forms.App.EntityFrameworkCore.Builder.ColumnConverters;
using Forms.App.EntityFrameworkCore.Data.Attributes;
using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Forms.App.Model;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.XPath;

namespace Forms.App.EntityFrameworkCore.Builder
{
    /// <summary>
    /// 
    /// </summary>
    internal static class TableAutoBuilderExtensions
    {
        /// <summary>
        /// 自动注册表
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        /// <param name="tables"></param>
        /// <param name="navigator"></param>
        public static void TableAutoBuilder(this ModelBuilder builder, Assembly assembly, List<Type>? tables, XPathNavigator navigator)
        {
            if (!tables.HasValue())
                return;

            // 添加表
            foreach (var item in tables!)
            {
                // 注释
                var memberName = $"T:{item.FullName}";
                var summaryNode = navigator.SelectSingleNode($"/doc/members/member[@name='{memberName}']/summary");
                var comment = summaryNode?.InnerXml.Trim();
                comment ??= "";

                var _builder = builder.Entity(item).ToTable($"{AppSettings.Sql.TablePrefix}{item.GetCustomAttribute<TableAttribute>(false)!.Name}", t => t.HasComment(comment));

                var props = item!.GetProperties()
                                 .Where(x => x.PropertyType.IsPublic)
                                 .Where(x => !x.PropertyType.IsAbstract)
                                 .Where(x => x.GetCustomAttribute<NotMappedAttribute>(false) == null)
                                 .ToList();

                var notMapColumn = new List<string>();
                // 属性设置
                foreach (var p in props)
                {
                    var column = p.GetCustomAttribute<TableColumnAttribute>(false) ?? new TableColumnAttribute();

                    // 主键
                    if (column.IsPrimary)
                        _builder.HasKey(p.Name);

                    _builder.Property(p.Name).TableColumnAutoBuilder(_builder, p, column, navigator, assembly);
                }

                // 表索引
                var tableIndexs = GetTableIndexAttributes(item);

                if (tableIndexs.HasValue())
                {
                    foreach (var index in tableIndexs!)
                    {
                        if (!index.Properties.Any(x => props.Select(y => y.Name).Contains(x)))
                            continue;

                        if (props.Select(x => x.Name).Any(x => index.Properties.Contains(x)))
                        {
                            if (index.Properties.Length > 1)
                            {
                                if (index.IsUnique)
                                    _builder.HasIndex(index.Properties).IsUnique();
                                else
                                    _builder.HasIndex(index.Properties);
                            }
                            else
                            {
                                if (index.IsUnique)
                                    _builder.HasIndex(index.Properties.First()).IsUnique();
                                else
                                    _builder.HasIndex(index.Properties.First());
                            }
                        }
                    }
                }

                // 种子数据
                var instance = Activator.CreateInstance(item) as ITableBaseEntity;
                var data = instance!.SeedData();
                if (data.HasValue())
                    _builder.HasData(data);
            }
        }

        /// <summary>
        /// 表列处理
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="p"></param>
        /// <param name="column"></param>
        /// <param name="navigator"></param>
        /// <param name="assembly"></param>
        private static void TableColumnAutoBuilder(this PropertyBuilder propertyBuilder, EntityTypeBuilder builder, PropertyInfo? p, TableColumnAttribute column, XPathNavigator navigator, Assembly assembly)
        {
            if (p == null)
                return;

            // 主键不做以下处理
            if (column.IsPrimary)
            {
                // 自增
                if (column.IsIdentity)
                {
                    if (p.PropertyType == typeof(long))
                    {
                        if (p.GetCustomAttribute<SnowflakeAttribute>(false) == null)
                            propertyBuilder.ValueGeneratedOnAdd();
                        else
                            propertyBuilder.ValueGeneratedNever();
                    }
                    else
                    {
                        propertyBuilder.ValueGeneratedOnAdd();
                    }
                }
                else
                {
                    propertyBuilder.ValueGeneratedNever();
                }
            }
            else
            {
                // 默认值
                if (column.DefaultValue != null)
                {
                    if (p.PropertyType == typeof(bool))
                        propertyBuilder.HasDefaultValue(column.DefaultValue).ValueGeneratedNever();
                    else
                        propertyBuilder.HasDefaultValue(column.DefaultValue);
                }
                else if (column.ColumnType.IsNullOrEmpty())
                {
                    if (p.PropertyType == typeof(string) && column.StringLength > 0)
                        propertyBuilder.HasDefaultValue("");
                    else if (new Type[] { typeof(int), typeof(long), typeof(ushort), typeof(uint) }.Contains(p.PropertyType))
                        propertyBuilder.HasDefaultValue(0);
                    else if (p.PropertyType == typeof(bool))
                        propertyBuilder.HasDefaultValue(false).ValueGeneratedNever();
                    else if (p.PropertyType == typeof(Enum))
                        propertyBuilder.HasDefaultValue(0);
                    else if (p.PropertyType == typeof(DateTime))
                        propertyBuilder.HasDefaultValueSql("CURRENT_TIMESTAMP");
                }

                if (column.JsonMap)
                {
                    builder.OwnsOne(p.Name, p.Name, valueBuiler =>
                    {
                        valueBuiler.ToJson();
                    });
                }
                else if (column.Sensitive)
                {
                    propertyBuilder.HasConversion<SqlSensitiveConverter>();
                }
            }

            if (column.IsRowVersion)
                propertyBuilder.IsRowVersion();

            // 映射数据库列类型
            if (!column.ColumnType.IsNullOrEmpty())
            {
                propertyBuilder.HasColumnType(column.ColumnType);
            }
            else if (p.PropertyType == typeof(string))
            {
                // 字符串长度限制
                if (column.StringLength > 0)
                    propertyBuilder.HasColumnType("NVARCHAR").HasMaxLength(column.StringLength);
                else
                    propertyBuilder.HasColumnType("NVARCHAR").HasMaxLength(300);
            }

            if (p.PropertyType == typeof(decimal))
            {
                if (column.Precision > 0 && column.Scale > 0)
                    propertyBuilder.HasPrecision(column.Precision, column.Scale);
                else if (column.Precision > 0)
                    propertyBuilder.HasPrecision(column.Precision);
            }

            // 必填
            propertyBuilder.IsRequired(column.Required);

            // 注释（列注释）
            var memberName = $"P:{p.DeclaringType!.Namespace}.{p.DeclaringType.Name}.{p.Name}";
            var summaryNode = navigator.SelectSingleNode($"/doc/members/member[@name='{memberName}']/summary");

            var comment = summaryNode?.InnerXml.Trim();
            comment ??= "";

            if (p.PropertyType.IsEnum)
            {
                var fields = p.PropertyType.GetFields(BindingFlags.Public | BindingFlags.Static);

                comment += "：";

                var _navigator = assembly != p.PropertyType.Assembly ? GetOtherAssemblySummary(p.PropertyType) : navigator;

                foreach (var item in fields)
                {
                    // 列枚举注释
                    // 由于枚举值存放位置可能不在当前程序集，所以如果当前程序集没有找到列
                    var value = item.GetValue(null);
                    summaryNode = _navigator?.SelectSingleNode($"/doc/members/member[@name='F:{p.PropertyType.FullName}.{value}']/summary");

                    comment += $"{(int)value!}-{summaryNode?.InnerXml.Trim() ?? ""},";
                }

                comment = comment.TrimEnd(',');
            }

            propertyBuilder.HasComment(comment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<TableIndexAttribute> GetTableIndexAttributes(Type? type)
        {
            var arrtibutes = new List<TableIndexAttribute>();

            if (type == null)
                return arrtibutes;

            // 遍历类型的所有基类  
            while (type != null && type != typeof(object))
            {
                // 检查类型是否是 TableFullBaseEntity<T> 的某个实例  
                if (type.IsGenericType)
                {
                    // 获取该类型上的 TableIndexAttribute  

                    if (new Type[] { typeof(TableFullBaseEntity<>), typeof(TableUpdateBaseEntity<>), typeof(TableCreateBaseEntity<>), typeof(TableBaseEntity<>) }.Contains(type.GetGenericTypeDefinition()))
                    {
                        var tmp = type.GetCustomAttributes(typeof(TableIndexAttribute), true).Cast<TableIndexAttribute>().ToList();
                        arrtibutes.AddRange(tmp);
                    }
                }
                else
                {
                    var tmp = type.GetCustomAttributes(typeof(TableIndexAttribute), true).Cast<TableIndexAttribute>().ToList();
                    arrtibutes.AddRange(tmp);
                }

                // 继续检查基类型  
                type = type.BaseType;
            }

            // 如果没有找到，返回空数组  
            return arrtibutes.DistinctBy(x => x.Properties.OrderBy(x => x.Length).ToJson()).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private static XPathNavigator? GetOtherAssemblySummary(Type propertyType)
        {
            try
            {
                var xmlDoc = new XPathDocument(Path.Combine(AppContext.BaseDirectory, $"{propertyType.Assembly.GetName().Name}.xml"));
                return xmlDoc.CreateNavigator();
            }
            catch
            {
                return null;
            }
        }
    }
}
