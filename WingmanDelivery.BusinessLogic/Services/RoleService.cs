using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoleModel>> GetRolesAsync()
        {
            return await _roleRepository.Get();
        }

        public async Task<RoleModel> FindRoleAsync(Guid uid)
        {
            return await _roleRepository.Find(uid);
        }

        public async Task<GridDataModel<RoleModel>> GetRolesForGridAsync(FilterModel filter)
        {
            return await _roleRepository.GetExtendedForGrid(filter);
        }

        public async Task<RoleModel> CreateRoleAsync(RoleModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var createdRole = await _roleRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return createdRole;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<RoleModel> UpdateRoleAsync(RoleModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var updatedRole = await _roleRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return updatedRole;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteRoleAsync(RoleModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var affectedCount = await _roleRepository.Remove(model);
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