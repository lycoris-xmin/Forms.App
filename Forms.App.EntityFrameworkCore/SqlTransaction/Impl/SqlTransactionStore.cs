using Lycoris.Autofac.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Forms.App.EntityFrameworkCore.SqlTransaction.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class SqlTransactionStore : ISqlTransactionStore
    {
        /// <summary>
        /// 
        /// </summary>
        public long? CurrentTransaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStartingUow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbContextTransaction? Transaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNotNull { get => IsStartingUow && Transaction != null && CurrentTransaction.HasValue && CurrentTransaction.Value > 0; }
    }
}
