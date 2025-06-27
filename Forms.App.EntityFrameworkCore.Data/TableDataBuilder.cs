using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Lycoris.Common.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.XPath;

namespace Forms.App.EntityFrameworkCore.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class TableDataBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TableBudilerOption Build()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var tables = assembly.GetTypes()
                                 .Where(x => x.IsPublic && !x.IsAbstract)
                                 .Where(x => x.IsInterfaceFrom(typeof(ITableBaseEntity)))
                                 .Where(x => x.GetCustomAttribute<TableAttribute>(false) != null).ToList();

            var xmlDoc = new XPathDocument(Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml"));
            var navigator = xmlDoc.CreateNavigator();

            return new TableBudilerOption(assembly, tables, navigator);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableBudilerOption
    {
        /// <summary>
        /// 
        /// </summary>
        public TableBudilerOption(Assembly Assembly, List<Type>? Tables, XPathNavigator XPathNavigator)
        {
            this.Assembly = Assembly;
            this.Tables = Tables;
            this.XPathNavigator = XPathNavigator;
        }

        /// <summary>
        /// 
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<Type>? Tables { get; }

        /// <summary>
        /// 
        /// </summary>
        public XPathNavigator XPathNavigator { get; }
    }
}
