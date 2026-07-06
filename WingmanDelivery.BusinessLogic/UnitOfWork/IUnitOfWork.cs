using System.Data;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IDeliveryOrderRepository Orders { get; }
        IDeliveryOrderLogsRepository Logs { get; }
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
        InvokeDataModel Data { get; }
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
