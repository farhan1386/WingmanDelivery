using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetRolesAsync();
        Task<RoleModel> FindRoleAsync(Guid uid);
        Task<GridDataModel<RoleModel>> GetRolesForGridAsync(FilterModel filter);
        Task<RoleModel> CreateRoleAsync(RoleModel model);
        Task<RoleModel> UpdateRoleAsync(RoleModel model);
        Task<int> DeleteRoleAsync(RoleModel model);
    }
}
