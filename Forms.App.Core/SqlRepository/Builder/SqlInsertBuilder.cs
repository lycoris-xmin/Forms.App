using Forms.App.EntityFrameworkCore.Data.Tables.Shared;
using Microsoft.Data.Sqlite;

namespace Forms.App.Core.SqlRepository.Builder
{
    public class SqlInsertBuilder<T, TPrimary> : BaseSqlBuidler<T, TPrimary> where T : TableBaseEntity<TPrimary>
    {
        private readonly string TableName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        public SqlInsertBuilder(ISqlRepository<T, TPrimary> repo) : base(repo)
        {
            TableName = repo.TableName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SqlInsertBuilder<T, TPrimary> Insert(T entity)
        {
            _sql.Clear();

            var props = entity.GetType().GetProperties().Where(x => !x.Name.Equals("RowVersion", StringComparison.CurrentCultureIgnoreCase)).ToList();

            var columns = string.Join(",", props.Select(p => p.Name));
            var values = string.Join(",", props.Select(p => FormatValue(p.GetValue(entity)!)));

            _sql.Append($"INSERT INTO {TableName} ({columns}) VALUES ({values})");

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public SqlInsertBuilder<T, TPrimary> Insert(List<T> entities)
        {
            _sql.Clear();

            var columns = string.Join(",", typeof(T).GetProperties().Where(x => !x.Name.Equals("RowVersion", StringComparison.CurrentCultureIgnoreCase)).Select(p => p.Name));

            var values = string.Join(", ", entities.Select(entity => $"({string.Join(",", entity.GetType().GetProperties().Where(x => !x.Name.Equals("RowVersion", StringComparison.CurrentCultureIgnoreCase)).Select(p => FormatValue(p.GetValue(entity)!)))})"));

            _sql.Append($"INSERT INTO {TableName} ({columns}) VALUES {values}");

            return this;
        }

        /// <summary>
        /// 单实体插入（参数化）
        /// </summary>
        public SqlInsertBuilder<T, TPrimary> InsertBySqlParameters(T entity)
        {
            _sql.Clear();
            SqlParameters.Clear();

            var props = entity.GetType()
                .GetProperties()
                .Where(x => !x.Name.Equals("RowVersion", StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            var columns = string.Join(",", props.Select(p => p.Name));
            var parameters = string.Join(",", props.Select(p => "@" + p.Name));

            _sql.Append($"INSERT INTO {TableName} ({columns}) VALUES ({parameters})");

            foreach (var prop in props)
            {
                object value = prop.GetValue(entity) ?? DBNull.Value;
                SqlParameters.Add(new SqliteParameter("@" + prop.Name, value));
            }

            return this;
        }

        /// <summary>
        /// 多实体插入（参数化）
        /// </summary>
        public SqlInsertBuilder<T, TPrimary> InsertBySqlParameters(List<T> entities)
        {
            _sql.Clear();
            SqlParameters.Clear();

            if (entities == null || entities.Count == 0)
                return this;

            var props = typeof(T).GetProperties()
                .Where(x => !x.Name.Equals("RowVersion", StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            var columns = string.Join(",", props.Select(p => p.Name));
            var valuesList = new List<string>();

            for (int i = 0; i < entities.Count; i++)
            {
                var paramPlaceholders = new List<string>();

                foreach (var prop in props)
                {
                    string paramName = $"@{prop.Name}_{i}";
                    paramPlaceholders.Add(paramName);

                    object value = prop.GetValue(entities[i]) ?? DBNull.Value;
                    SqlParameters.Add(new SqliteParameter(paramName, value));
                }

                valuesList.Add("(" + string.Join(",", paramPlaceholders) + ")");
            }

            _sql.Append($"INSERT INTO {TableName} ({columns}) VALUES {string.Join(", ", valuesList)}");

            return this;
        }
    }
}
