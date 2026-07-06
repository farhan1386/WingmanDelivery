using Microsoft.Data.SqlClient;
using System.Data;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Repositories;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlConnection _connection;
        private SqlTransaction? _transaction;
        private bool _disposed;

        public IDbConnection Connection => _connection;
        public IDbTransaction? Transaction => _transaction;
        public InvokeDataModel Data { get; }
        public IDeliveryOrderRepository Orders => new DeliveryOrderRepository(this);
        public IDeliveryOrderLogsRepository Logs => new DeliveryOrderLogsRepository(this);

        public UnitOfWork(SqlConnection connection, InvokeDataModel data)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public async Task BeginAsync()
        {
            EnsureNotDisposed();
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync().ConfigureAwait(false);
            }
            if (_transaction == null)
            {
                _transaction = (SqlTransaction)await _connection.BeginTransactionAsync().ConfigureAwait(false);
            }
        }

        public async Task CommitAsync()
        {
            EnsureNotDisposed();
            if (_transaction == null) throw new InvalidOperationException("No transaction active.");
            try
            {
                await _transaction.CommitAsync().ConfigureAwait(false);
            }
            finally
            {
                await DisposeTransactionAsync().ConfigureAwait(false);
            }
        }

        public async Task RollbackAsync()
        {
            EnsureNotDisposed();
            if (_transaction == null) return;
            try
            {
                await _transaction.RollbackAsync().ConfigureAwait(false);
            }
            finally
            {
                await DisposeTransactionAsync().ConfigureAwait(false);
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
                _transaction = null;
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(UnitOfWork));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _transaction?.Dispose();
                _transaction = null;
                if (_connection.State == ConnectionState.Open) _connection.Close();
                _connection.Dispose();
            }
            catch
            {
                // consider logging instead of silent catch
            }
        }
    }
}
