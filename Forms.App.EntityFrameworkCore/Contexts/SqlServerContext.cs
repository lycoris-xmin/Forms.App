using Forms.App.EntityFrameworkCore.Builder;
using Forms.App.EntityFrameworkCore.Data;
using Forms.App.EntityFrameworkCore.SqlTransaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace Forms.App.EntityFrameworkCore.Contexts
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class SqlServerContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        private ISqlTransactionStore? _transaction;

        /// <summary>
        /// 
        /// </summary>
        private IPropertyAutoProvider? _provider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options"></param>
        public SqlServerContext([NotNull] DbContextOptions options) : base(options)
        {
            // 关闭DbContext默认事务
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        /// <summary>
        /// 数据库工厂初始化数据库上下文
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="provider"></param>
        public void ServiceInitialization(ISqlTransactionStore transaction, IPropertyAutoProvider provider)
        {
            _transaction = transaction;
            _provider = provider;
        }

        /// <summary>
        /// 实体映射配置
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 数据库表自动生成注册
            var tableBuilder = new TableDataBuilder().Build();
            builder.TableAutoBuilder(tableBuilder.Assembly, tableBuilder!.Tables, tableBuilder.XPathNavigator);

            // 执行基类处理
            base.OnModelCreating(builder);
        }

        /// <summary>
        /// 重写 SaveChanges 方法
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().ToList();

            PropertyAutoProvider(entities);

            // 没有自动开启事务的情况下,保证主从表插入,主从表更新开启事务。
            var isManualTransaction = false;

            if (Database.AutoTransactionBehavior == AutoTransactionBehavior.Never && _transaction != null && !_transaction.IsStartingUow && entities.Count > 1)
            {
                isManualTransaction = true;
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Always;
            }

            var result = base.SaveChanges();

            // 如果手工开启了自动事务,用完后关闭
            if (isManualTransaction)
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

            return result;
        }

        /// <summary>
        /// 重写 SaveChanges 方法
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries().ToList();

            PropertyAutoProvider(entities);

            // 没有自动开启事务的情况下,保证主从表插入,主从表更新开启事务。
            var isManualTransaction = false;

            if (Database.AutoTransactionBehavior == AutoTransactionBehavior.Never && _transaction != null && !_transaction.IsStartingUow && entities.Count > 1)
            {
                isManualTransaction = true;
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Always;
            }

            var result = await base.SaveChangesAsync();

            // 如果手工开启了自动事务,用完后关闭
            if (isManualTransaction)
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        private void PropertyAutoProvider(List<EntityEntry> entities)
        {
            foreach (var item in entities)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        _provider!.InsertIntercept(item);
                        break;
                    case EntityState.Modified:
                        _provider!.UpdateIntercept(item);
                        break;
                    case EntityState.Deleted:
                        _provider!.DeleteIntercept(item);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
