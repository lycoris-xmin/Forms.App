using Forms.App.EntityFrameworkCore.Builder;
using Forms.App.EntityFrameworkCore.SqlTransaction;
using Lycoris.Autofac.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Forms.App.EntityFrameworkCore.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, Self = true)]
    public class SqlServerContextScopedFactory : IDbContextFactory<SqlServerContext>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IDbContextFactory<SqlServerContext> _pooledFactory;

        /// <summary>
        /// 事务状态
        /// </summary>
        private readonly ISqlTransactionStore _transaction;

        /// <summary>
        /// 
        /// </summary>
        private readonly IPropertyAutoProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pooledFactory"></param>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        public SqlServerContextScopedFactory(IDbContextFactory<SqlServerContext> pooledFactory, ISqlTransactionStore transaction, IPropertyAutoProvider provider)
        {
            _pooledFactory = pooledFactory;
            _transaction = transaction;
            _provider = provider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SqlServerContext CreateDbContext()
        {
            var context = _pooledFactory.CreateDbContext();
            context.ServiceInitialization(_transaction, _provider);
            return context;
        }
    }
}
