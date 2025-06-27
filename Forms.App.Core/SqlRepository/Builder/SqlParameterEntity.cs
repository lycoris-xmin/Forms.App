using System.Data.Common;

namespace Forms.App.Core.SqlRepository.Builder
{
    public class SqlParameterEntity
    {
        public string Sql { get; set; } = "";

        public IReadOnlyList<DbParameter> Parameter { get; set; } = new List<DbParameter>();
    }
}
