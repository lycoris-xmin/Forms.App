using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace Forms.App.Core.SqlRepository.Builder
{
    public class SqlDeleteBuilder<T, TPrimary> : BaseSqlBuidler<T, TPrimary> where T : TableBaseEntity<TPrimary>
    {
        private readonly ISqlRepository<T, TPrimary> _repo;

        public SqlDeleteBuilder(ISqlRepository<T, TPrimary> repo) : base(repo)
        {
            _repo = repo;
            _sql.Append($"DELETE {repo.TableName} ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public SqlDeleteBuilder<T, TPrimary> DeleteAll(string TableName)
        {
            _sql.Clear();
            _sql.Append($"DELETE FROM {TableName}");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlDeleteBuilder<T, TPrimary> Where<TSelector>(Expression<Func<T, TSelector>> selector, TSelector value)
        {
            AppendWhereClause();

            var key = selector.GetMemberAccess();
            var valueType = typeof(TSelector);

            if (key.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion)))
            {
                var rowVersion = BitConverter.ToString((value as byte[])!).Replace("-", "");
                _sql.Append($" {key.Name} = 0x{rowVersion}");
            }
            else if (new[] { typeof(int), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
            {
                _sql.Append($" {key.Name} = {value}");
            }
            else if (valueType == typeof(DateTime))
            {
                var formattedDate = ((DateTime)(object)value!).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                _sql.Append($" {key.Name} = '{formattedDate}'");
            }
            else if (valueType == typeof(string))
            {
                var escapedValue = value!.ToString()!.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} = '{escapedValue}'");
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
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlDeleteBuilder<T, TPrimary> WhereIdIn(params TPrimary[] value)
        {
            AppendWhereClause();

            var valueType = typeof(TPrimary);
            if (new Type[] { typeof(int), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
                _sql.Append($" Id IN ({string.Join(",", value)})");
            else
                _sql.Append($" Id IN ({string.Join(",", value.Select(x => $"'{x}'"))})");

            return this;
        }

        public SqlDeleteBuilder<T, TPrimary> WhereIn<TSelector>(Expression<Func<T, TSelector>> selector, params TSelector[] value)
            => WhereIn(selector, value.ToList());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlDeleteBuilder<T, TPrimary> WhereIn<TSelector>(Expression<Func<T, TSelector>> selector, List<TSelector> value)
        {
            AppendWhereClause();

            var key = selector.GetMemberAccess();

            var valueType = typeof(TSelector);
            if (new Type[] { typeof(int), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
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
        public SqlDeleteBuilder<T, TPrimary> WhereNull(Expression<Func<T, object?>> selector, bool isNull)
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
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlDeleteBuilder<T, TPrimary> WhereNot<TSelector>(Expression<Func<T, TSelector>> selector, TSelector value)
        {
            AppendWhereClause();

            var key = selector.GetMemberAccess();
            var valueType = typeof(TSelector);

            if (key.Name.Equals(nameof(TableBaseEntity<TPrimary>.RowVersion)))
            {
                var rowVersion = BitConverter.ToString((value as byte[])!).Replace("-", "");
                _sql.Append($" {key.Name} <> 0x{rowVersion}");
            }
            else if (new[] { typeof(int), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
            {
                _sql.Append($" {key.Name} <> {value}");
            }
            else if (valueType == typeof(DateTime))
            {
                var formattedDate = ((DateTime)(object)value!).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                _sql.Append($" {key.Name} <> '{formattedDate}'");
            }
            else if (valueType == typeof(string))
            {
                var escapedValue = value!.ToString()!.Replace("'", "''"); // 防止SQL注入
                _sql.Append($" {key.Name} <> '{escapedValue}'");
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
        /// <returns></returns>
        public override string ToSql()
        {
            var tmp = base.ToSql();

            _sql.Append($"DELETE {_repo.TableName} ");

            return tmp;
        }
    }
}
