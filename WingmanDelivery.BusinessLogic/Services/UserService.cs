using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            return await _userRepository.Get();
        }

        public async Task<UserModel> FindUserAsync(Guid uid)
        {
            return await _userRepository.Find(uid);
        }

        public async Task<UserModel> GetByEmailAsync(string email)
        {
            // Useful helper mapping for logins or credential validation lookups
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<GridDataModel<UserModel>> GetUsersForGridAsync(FilterModel filter)
        {
            return await _userRepository.GetExtendedForGrid(filter);
        }

        public async Task<UserModel> CreateUserAsync(UserModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var createdUser = await _userRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return createdUser;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<UserModel> UpdateUserAsync(UserModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var updatedUser = await _userRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return updatedUser;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteUserAsync(UserModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var affectedCount = await _userRepository.Remove(model);
                await _unitOfWork.CommitAsync();
                return affectedCount;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}