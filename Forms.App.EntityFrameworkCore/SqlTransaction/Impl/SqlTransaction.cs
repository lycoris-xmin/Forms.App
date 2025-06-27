using Forms.App.EntityFrameworkCore.Contexts;
using Lycoris.Autofac.Extensions;
using Lycoris.Snowflakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Forms.App.EntityFrameworkCore.SqlTransaction.Impl
{
    /// <summary>
    /// Mysql事务
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class SqlTransaction : ISqlTransaction
    {
        /// <summary>
        /// Mysql数据库上下文
        /// </summary>
        private readonly SqlServerContext _context;

        /// <summary>
        /// 事务状态
        /// </summary>
        private readonly ISqlTransactionStore _transactionStore;

        /// <summary>
        /// 事务状态
        /// </summary>
        public bool IsStartingUow { get => _transactionStore.IsStartingUow; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transactionStore"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlTransaction(SqlServerContext context, ISqlTransactionStore transactionStore)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _transactionStore = transactionStore;
        }

        /// <summary>
        /// 开启数据库事务
        /// </summary>
        /// <returns></returns>
        public long CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            var tmp = SnowflakeHelper.GetNextId();

            if (!_transactionStore.IsStartingUow)
            {
                _transactionStore.Transaction = GetDbContextTransaction(isolationLevel);
                _transactionStore.CurrentTransaction = tmp;
            }

            return tmp;
        }

        /// <summary>
        /// 开启数据库事务
        /// </summary>
        /// <returns></returns>
        public async Task<long> CreateTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            var tmp = await SnowflakeHelper.GetNextIdAsync();

            if (!_transactionStore.IsStartingUow)
            {
                _transactionStore.Transaction = await GetDbContextTransactionAsync(isolationLevel);
                _transactionStore.CurrentTransaction = tmp;
            }

            return tmp;
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="currentTransaction"></param>
        public void Commit(long currentTransaction)
        {
            if (_transactionStore.IsNotNull && _transactionStore.CurrentTransaction == currentTransaction)
            {
                _transactionStore.Transaction!.Commit();
                _transactionStore.IsStartingUow = false;
                _transactionStore.CurrentTransaction = null;
            }
        }

        /// <summary>
        /// 事务提交(异步)
        /// </summary>
        /// <param name="currentTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CommitAsync(long currentTransaction, CancellationToken cancellationToken = default)
        {
            if (_transactionStore.IsNotNull && _transactionStore.CurrentTransaction == currentTransaction)
            {
                await _transactionStore.Transaction!.CommitAsync(cancellationToken);
                _transactionStore.IsStartingUow = false;
                _transactionStore.CurrentTransaction = null;
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="currentTransaction"></param>
        public void Rollback(long currentTransaction)
        {
            if (_transactionStore.IsNotNull && _transactionStore.CurrentTransaction == currentTransaction)
            {
                _transactionStore.Transaction!.Rollback();
                _transactionStore.IsStartingUow = false;
                _transactionStore.CurrentTransaction = null;
            }
        }

        /// <summary>
        /// 事务回滚(异步)
        /// </summary>
        /// <param name="currentTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RollbackAsync(long currentTransaction, CancellationToken cancellationToken = default)
        {
            if (_transactionStore.IsNotNull && _transactionStore.CurrentTransaction == currentTransaction)
            {
                await _transactionStore.Transaction!.RollbackAsync(cancellationToken);
                _transactionStore.IsStartingUow = false;
                _transactionStore.CurrentTransaction = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="currentTransaction"></param>
        public void Dispose(long currentTransaction)
        {
            if (_transactionStore.IsNotNull && _transactionStore.CurrentTransaction == currentTransaction)
            {
                _transactionStore.Transaction!.Dispose();
                if (_transactionStore != null)
                    _transactionStore.IsStartingUow = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private IDbContextTransaction GetDbContextTransaction(IsolationLevel isolationLevel)
        {
            if (_transactionStore.IsStartingUow)
                throw new ArgumentException("Transaction error");

            // 使用 ADO.NET 获取事务
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var transaction = connection.BeginTransaction(isolationLevel);
            var dbTransaction = _context.Database.UseTransaction(transaction);
            _transactionStore.IsStartingUow = true;

            return dbTransaction!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<IDbContextTransaction> GetDbContextTransactionAsync(IsolationLevel isolationLevel)
        {
            if (_transactionStore.IsStartingUow)
                throw new ArgumentException("Transaction error");

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var transaction = connection.BeginTransaction(isolationLevel);
            var dbTransaction = _context.Database.UseTransaction(transaction);
            _transactionStore.IsStartingUow = true;

            return dbTransaction!;
        }
    }
}
