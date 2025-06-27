using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace Forms.App.Core.SqlRepository.Builder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public class SqlUpdateBuilder<T, TPrimary> : BaseSqlBuidler<T, TPrimary> where T : TableBaseEntity<TPrimary>
    {
        private readonly ISqlRepository<T, TPrimary> _repo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        public SqlUpdateBuilder(ISqlRepository<T, TPrimary> repo) : base(repo)
        {
            _repo = repo;
            _sql.Append($"UPDATE {repo.TableName} SET");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> Set<TSelector>(Expression<Func<T, TSelector>> selector, TSelector value)
        {
            var key = selector.GetMemberAccess();
            var valueType = typeof(TSelector);

            if (value == null)
            {
                _sql.Append($" {key.Name} = NULL,");
            }
            else if (new[] { typeof(int), typeof(uint), typeof(long), typeof(double), typeof(decimal), typeof(int?), typeof(long?), typeof(double?), typeof(decimal?) }.Contains(valueType))
            {
                _sql.Append($" {key.Name} = {value},");
            }
            else if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
            {
                var formattedDate = ((DateTime)(object)value!).ToString("yyyy-MM-dd HH:mm:ss");
                _sql.Append($" {key.Name} = '{formattedDate}',");
            }
            else if (valueType == typeof(string))
            {
                var escapedValue = value!.ToString()!.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} = '{escapedValue}',");
            }
            else if (valueType == typeof(bool) || valueType == typeof(bool?))
            {
                _sql.Append($" {key.Name} = {(Convert.ToBoolean(value) ? 1 : 0)},");
            }
            else if (valueType.IsEnum)
            {
                _sql.Append($" {key.Name} = {(int)(object)value!},");
            }
            else if (valueType.IsClass)
            {
                var jsonValue = value.ToJson(); // 假设你有一个扩展方法来转换对象为 JSON
                var escapedJson = jsonValue.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} = '{escapedJson}',");
            }
            else
            {
                throw new NotSupportedException($"Type '{valueType.Name}' is not supported.");
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSelector"></typeparam>
        /// <param name="condition"></param>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> SetIf<TSelector>(bool condition, Expression<Func<T, TSelector>> selector, TSelector value)
        {
            if (condition)
                return Set(selector, value);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> Where<TSelector>(Expression<Func<T, TSelector>> selector, TSelector value)
        {
            AppendWhereClause();

            var key = selector.GetMemberAccess();
            var valueType = typeof(TSelector);

            if (key.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion)))
            {
                var rowVersion = BitConverter.ToString((value as byte[])!).Replace("-", "");
                _sql.Append($" {key.Name} = 0x{rowVersion}");
            }
            else if (new[] { typeof(int), typeof(uint), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
            {
                _sql.Append($" {key.Name} = {value}");
            }
            else if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
            {
                var formattedDate = ((DateTime)(object)value!).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                _sql.Append($" {key.Name} = '{formattedDate}'");
            }
            else if (valueType == typeof(string))
            {
                var escapedValue = value!.ToString()!.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} = '{escapedValue}'");
            }
            else if (valueType == typeof(bool) || valueType == typeof(bool?))
            {
                _sql.Append($" {key.Name} = {(Convert.ToBoolean(value) ? 1 : 0)}");
            }
            else if (valueType.IsEnum)
            {
                _sql.Append($" {key.Name} = {(int)(object)value!}");
            }
            else if (valueType.IsClass)
            {
                var jsonValue = value.ToJson(); // 假设有 ToJson() 方法
                var escapedJson = jsonValue.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} = '{escapedJson}'");
            }
            else
            {
                throw new NotSupportedException($"Type '{valueType.Name}' is not supported in WHERE clause.");
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> WhereNot<TSelector>(Expression<Func<T, TSelector>> selector, TSelector value)
        {
            AppendWhereClause();

            var key = selector.GetMemberAccess();
            var valueType = typeof(TSelector);

            if (key.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion)))
            {
                var rowVersion = BitConverter.ToString((value as byte[])!).Replace("-", "");
                _sql.Append($" {key.Name} <> 0x{rowVersion}");
            }
            else if (new[] { typeof(int), typeof(uint), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
            {
                _sql.Append($" {key.Name} <> {value}");
            }
            else if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
            {
                var formattedDate = ((DateTime)(object)value!).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                _sql.Append($" {key.Name} <> '{formattedDate}'");
            }
            else if (valueType == typeof(string))
            {
                var escapedValue = value!.ToString()!.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} <> '{escapedValue}'");
            }
            else if (valueType == typeof(bool) || valueType == typeof(bool?))
            {
                _sql.Append($" {key.Name} <> {(Convert.ToBoolean(value) ? 1 : 0)}");
            }
            else if (valueType.IsEnum)
            {
                _sql.Append($" {key.Name} <> {(int)(object)value!}");
            }
            else if (valueType.IsClass)
            {
                var jsonValue = value.ToJson(); // 假设有 ToJson() 方法
                var escapedJson = jsonValue.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} <> '{escapedJson}'");
            }
            else
            {
                throw new NotSupportedException($"Type '{valueType.Name}' is not supported in WHERE clause.");
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> WhereIdIn(List<TPrimary> value)
        {
            AppendWhereClause();

            var valueType = typeof(TPrimary);
            if (new Type[] { typeof(int), typeof(uint), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
                _sql.Append($" Id IN ({string.Join(",", value)})");
            else
                _sql.Append($" Id IN ({string.Join(",", value.Select(x => $"'{x}'"))})");

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> WhereIdIn(params TPrimary[] value)
        {
            AppendWhereClause();

            var valueType = typeof(TPrimary);
            if (new Type[] { typeof(int), typeof(uint), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
                _sql.Append($" Id IN ({string.Join(",", value)})");
            else
                _sql.Append($" Id IN ({string.Join(",", value.Select(x => $"'{x}'"))})");

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> WhereIn<TSelector>(Expression<Func<T, TSelector>> selector, List<TSelector> value)
        {
            AppendWhereClause();

            var key = selector.GetMemberAccess();

            var valueType = typeof(TSelector);
            if (new Type[] { typeof(int), typeof(uint), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
            {
                _sql.Append($" {key.Name} IN ({string.Join(",", value)})");
            }
            else if (valueType == typeof(string))
            {
                _sql.Append($" {key.Name} IN ({string.Join(",", value.Cast<string>().Select(x => $"'{x}'"))})");
            }
            else if (valueType.IsEnum)
            {
                _sql.Append($" {key.Name} IN ({string.Join(",", value.Cast<Enum>().Select(x => Convert.ToInt32(x)))})");
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="isNull"></param>
        /// <returns></returns>
        public SqlUpdateBuilder<T, TPrimary> WhereNull(Expression<Func<T, object?>> selector, bool isNull)
        {
            AppendWhereClause();

            var key = selector.GetMemberAccess();

            if (isNull)
                _sql.Append($" {key.Name} IS NULL");
            else
                _sql.Append($" {key.Name} IS NOT NULL");

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToSql()
        {
            var tmp = base.ToSql();

            _sql.Append($"UPDATE {_repo.TableName} SET");

            return tmp;
        }
    }
}
