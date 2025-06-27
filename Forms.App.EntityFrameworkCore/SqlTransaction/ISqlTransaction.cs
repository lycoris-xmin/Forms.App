using System.Data;

namespace Forms.App.EntityFrameworkCore.SqlTransaction
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlTransaction
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsStartingUow { get; }

        /// <summary>
        /// 创建事务
        /// </summary>
        long CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        /// <summary>
        /// 创建事务
        /// </summary>
        Task<long> CreateTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="currentTransaction"></param>
        void Commit(long currentTransaction);

        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="currentTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CommitAsync(long currentTransaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="currentTransaction"></param>
        void Rollback(long currentTransaction);

        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="currentTransaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RollbackAsync(long currentTransaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="currentTransaction"></param>
        void Dispose(long currentTransaction);
    }
}
