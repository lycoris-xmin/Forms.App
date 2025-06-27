using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Lycoris.Common.Extensions;
using Microsoft.Data.Sqlite;
using System.Text;

namespace Forms.App.Core.SqlRepository.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseSqlBuidler<T, TPrimary> where T : TableBaseEntity<TPrimary>
    {
        private readonly ISqlRepository<T, TPrimary> _repo;

        public BaseSqlBuidler(ISqlRepository<T, TPrimary> repo)
        {
            _repo = repo;
        }

        protected StringBuilder _sql = new StringBuilder();

        /// <summary>
        /// 参数化 SQL 的 SqlParameter 列表
        /// </summary>
        public List<SqliteParameter> SqlParameters { get; private set; } = new();

        /// <summary>
        /// 
        /// </summary>
        protected void AppendWhereClause()
        {
            if (!_sql.ToString().Contains(" WHERE "))
            {
                _sql = new StringBuilder(_sql.ToString().TrimEnd(','));
                _sql.Append(" WHERE ");
            }
            else
            {
                _sql.Append(" AND ");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        protected string FormatValue(object? value)
        {
            if (value == null)
                return "null";

            var valueType = value.GetType();
            if (new Type[] { typeof(uint), typeof(int), typeof(long), typeof(double), typeof(decimal) }.Contains(valueType))
                return value.ToString()!;
            else if (valueType == typeof(DateTime))
                return $"'{(DateTime)value:yyyy-MM-dd HH:mm:ss.ffffff}'";
            else if (valueType == typeof(string))
                return $"'{value}'";
            else if (valueType == typeof(bool))
                return (bool)value ? "1" : "0";
            else if (valueType.IsEnum)
                return ((int)value).ToString();
            else if (valueType.IsClass)
                return $"'{value.ToJson()}'";
            else
                throw new NotSupportedException($"type {valueType} is not supported.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string ToSql()
        {
            var tmp = $"{_sql.ToString().TrimEnd(',')};";

            _sql.Clear();

            return tmp;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int ExcuteSave() => _repo.ExecuteNonQuery(@$"{ToSql()}", SqlParameters.ToArray());

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Task<int> ExcuteSaveAsync() => _repo.ExecuteNonQueryAsync(@$"{ToSql()}", SqlParameters.ToArray());
    }
}
