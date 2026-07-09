using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<RoleModel>> Get();
    Task<RoleModel> Find(Guid uid);
    Task<GridDataModel<RoleModel>> GetExtendedForGrid(FilterModel filter);
    Task<RoleModel> Add(RoleModel model);
    Task<RoleModel> Update(RoleModel model);
    Task<int> Remove(RoleModel model);
}