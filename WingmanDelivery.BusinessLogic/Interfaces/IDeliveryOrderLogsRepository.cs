using System;
using System.Collections.Generic;
using System.Text;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Interfaces
{
    public interface IDeliveryOrderLogsRepository
    {
        Task<IEnumerable<DeliveryLogsModel>> Get();
        Task<DeliveryLogsModel> Find(Guid uid);
        Task<GridDataModel<DeliveryLogsModel>> GetExtendedForGrid(FilterModel filter);
        Task<DeliveryLogsModel> Add(DeliveryLogsModel model);
        Task<DeliveryLogsModel> Update(DeliveryLogsModel model);
        Task<int> Remove(DeliveryLogsModel model);
    }
}
