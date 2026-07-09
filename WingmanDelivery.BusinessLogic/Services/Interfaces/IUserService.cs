using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetUsersAsync();
        Task<UserModel> FindUserAsync(Guid uid);
        Task<UserModel> GetByEmailAsync(string email);
        Task<GridDataModel<UserModel>> GetUsersForGridAsync(FilterModel filter);
        Task<UserModel> CreateUserAsync(UserModel model);
        Task<UserModel> UpdateUserAsync(UserModel model);
        Task<int> DeleteUserAsync(UserModel model);
    }
}