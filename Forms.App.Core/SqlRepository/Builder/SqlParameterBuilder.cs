using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Forms.App.Core.SqlRepository.Builder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public class SqlParameterBuilder<T, TPrimary> where T : TableBaseEntity<TPrimary>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Type _TableType;

        /// <summary>
        /// 
        /// </summary>
        private readonly string _TableName;

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get => _TableName; }

        /// <summary>
        /// ctor
        /// </summary>
        public SqlParameterBuilder()
        {
            _TableType = typeof(T);
            var attr = _TableType.GetCustomAttribute<TableAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.Name))
                _TableName = attr.Name;
            else
                _TableName = _TableType.Name;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="tableName"></param>
        public SqlParameterBuilder(string tableName)
        {
            _TableName = tableName;
            _TableType = typeof(T);
        }

        /// <summary>
        /// 插入语句
        /// </summary>
        /// <param name="destObj"></param>
        /// <returns></returns>
        public SqlParameterEntity Insert(T destObj)
        {
            var sbColumn = new StringBuilder();
            var sbValues = new StringBuilder();
            var param = new List<DbParameter>();

            sbColumn.AppendFormat("INSERT INTO {0} (", TableName);
            sbValues.AppendFormat(" VALUES (");

            PropertyInfo[] properties = _TableType.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                var temp = ChangeType(prop.PropertyType, prop.GetValue(destObj, null));

                if (prop.Name.ToLower() == "id")
                {
                    // 如果是int或者long 则 并且值为0 则忽略
                    if ((prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)) && temp?.ToString() == "0")
                        continue;

                    // 如果是string且值为空 则忽略
                    if (prop.PropertyType == typeof(string) || string.IsNullOrEmpty(temp?.ToString()))
                        continue;
                }
                else if (prop.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion), StringComparison.CurrentCultureIgnoreCase))
                    continue;

                sbColumn.AppendFormat("{0},", prop.Name);
                sbValues.AppendFormat("?{0},", prop.Name);

                if (temp == null)
                    param.Add(CreateParameter<DbParameter>("?" + prop.Name, DBNull.Value));
                else
                    param.Add(CreateParameter<DbParameter>("?" + prop.Name, temp));
            }

            sbColumn.Replace(",", ")", sbColumn.Length - 1, 1);
            sbValues.Replace(",", ")", sbValues.Length - 1, 1);

            return new SqlParameterEntity()
            {
                Sql = sbColumn.ToString() + sbValues.ToString(),
                Parameter = param
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destList"></param>
        /// <returns></returns>
        public SqlParameterEntity Insert(List<T> destList)
        {
            var strAll = new StringBuilder();
            var param = new List<DbParameter>();

            var sbColumn = new StringBuilder();
            var sbValues = new StringBuilder();

            for (int i = 0; i < destList.Count; i++)
            {
                sbColumn.AppendFormat("INSERT INTO {0} (", TableName);
                sbValues.AppendFormat(" VALUES (");

                PropertyInfo[] properties = _TableType.GetProperties();
                foreach (PropertyInfo prop in properties)
                {
                    if (prop.GetCustomAttribute<NotMappedAttribute>() != null)
                        continue;

                    var temp = ChangeType(prop.PropertyType, prop.GetValue(destList[i], null));

                    if (prop.Name.ToLower() == "id")
                    {
                        // 如果是int或者long 则 并且值为0 则忽略
                        if ((prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)) && temp?.ToString() == "0")
                            continue;

                        // 如果是string且值为空 则忽略
                        if (prop.PropertyType == typeof(string) || string.IsNullOrEmpty(temp?.ToString()))
                            continue;
                    }
                    else if (prop.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion), StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    sbColumn.AppendFormat("{0},", prop.Name);
                    sbValues.AppendFormat("?{0}{1},", prop.Name, i);

                    if (temp == null)
                        param.Add(CreateParameter<DbParameter>("?" + prop.Name, DBNull.Value));
                    else
                        param.Add(CreateParameter<DbParameter>("?" + prop.Name + i, temp));
                }

                sbColumn.Replace(",", ")", sbColumn.Length - 1, 1);
                sbValues.Replace(",", ");", sbValues.Length - 1, 1);

                strAll.Append(sbColumn.ToString() + sbValues.ToString());

                sbColumn.Clear();
                sbValues.Clear();
            }

            return new SqlParameterEntity()
            {
                Sql = strAll.ToString(),
                Parameter = param
            };
        }

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <param name="destObj"></param>
        /// <returns></returns>
        public SqlParameterEntity Update(T destObj)
        {
            var sbUpdateColumn = new StringBuilder();

            sbUpdateColumn.AppendFormat("UPDATE {0} SET ", TableName);

            var sbWhere = new StringBuilder();

            var param = new List<DbParameter>();
            var properties = _TableType.GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                if (prop.Name.ToLower() == "id")
                    sbWhere.AppendFormat(" {0}=?{0} AND", prop.Name);
                else if (prop.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion), StringComparison.CurrentCultureIgnoreCase))
                    continue;
                else
                    sbUpdateColumn.AppendFormat("{0}=?{0},", prop.Name);

                var temp = ChangeType(prop.PropertyType, prop.GetValue(destObj, null));

                if (temp == null)
                    param.Add(CreateParameter<DbParameter>("?" + prop.Name, DBNull.Value));
                else
                    param.Add(CreateParameter<DbParameter>("?" + prop.Name, temp));
            }

            sbWhere.Replace("AND", "", sbWhere.Length - 3, 3);
            sbUpdateColumn.Replace(",", "", sbUpdateColumn.Length - 1, 1);

            sbUpdateColumn.AppendFormat("\r\nWHERE {0};", sbWhere.ToString());

            return new SqlParameterEntity()
            {
                Sql = sbUpdateColumn.ToString(),
                Parameter = param
            };
        }

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <param name="destList"></param>
        /// <returns></returns>
        public SqlParameterEntity Update(List<T> destList)
        {
            var strAll = new StringBuilder();
            var param = new List<DbParameter>();

            var sbUpdateColumn = new StringBuilder();
            var sbWhere = new StringBuilder();
            var properties = _TableType.GetProperties();

            for (int i = 0; i < destList.Count; i++)
            {
                sbUpdateColumn.AppendFormat("UPDATE {0} SET ", TableName);

                foreach (PropertyInfo prop in properties)
                {
                    if (prop.Name.ToLower() == "id")
                        sbWhere.AppendFormat(" {0}=?{0}{1} AND", prop.Name, i);
                    else if (prop.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion), StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    else
                        sbUpdateColumn.AppendFormat("{0}=?{0}{1},", prop.Name, i);

                    var temp = ChangeType(prop.PropertyType, prop.GetValue(destList[i], null));

                    if (temp == null)
                        param.Add(CreateParameter<DbParameter>("?" + prop.Name + i, DBNull.Value));
                    else
                        param.Add(CreateParameter<DbParameter>("?" + prop.Name + i, temp));

                }
                sbWhere.Replace("AND", "", sbWhere.Length - 3, 3);
                sbUpdateColumn.Replace(",", "", sbUpdateColumn.Length - 1, 1);
                sbUpdateColumn.AppendFormat("\r\nWHERE {0};", sbWhere.ToString());
                strAll.Append(sbUpdateColumn.ToString() + "\r\n");

                sbUpdateColumn.Clear();
                sbWhere.Clear();
            }

            return new SqlParameterEntity()
            {
                Sql = strAll.ToString(),
                Parameter = param
            };
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static TDbParameter CreateParameter<TDbParameter>(string paraName, object value) where TDbParameter : DbParameter
        {
            var parameter = Activator.CreateInstance<TDbParameter>();
            parameter.ParameterName = paraName;
            parameter.Value = value;
            return parameter;
        }

        /// <summary>
        /// 改变类型值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal object? ChangeType(Type type, object? value)
        {

            if (value == null && type.IsGenericType)
                return Activator.CreateInstance(type);

            if (value == null)
                return null;

            if (type == value.GetType())
            {
                if (type == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
                    return DateTime.Now;

                return value;
            }

            if (type.IsEnum)
                return value is string ? Enum.Parse(type, (value as string)!) : Enum.ToObject(type, value);

            if (type == typeof(bool) && typeof(int).IsInstanceOfType(value))
            {
                int temp = int.Parse(value.ToString()!);
                return temp != 0;
            }

            if (!type.IsInterface && type.IsGenericType)
            {
                Type type2 = type.GetGenericArguments()[0];
                var obj = ChangeType(type2, value);
                return Activator.CreateInstance(type, new object?[] { obj });
            }

            if (value is string && type == typeof(Guid))
                return new Guid((value as string)!);

            if (value is string && type == typeof(Version))
                return new Version((value as string)!);

            if (value is not IConvertible)
                return value;

            return Convert.ChangeType(value, type);
        }
    }
}
