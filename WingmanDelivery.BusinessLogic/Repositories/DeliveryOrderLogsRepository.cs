using Dapper;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public class DeliveryOrderLogsRepository : BaseRepository, IDeliveryOrderLogsRepository
    {
        public DeliveryOrderLogsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<DeliveryLogsModel>> Get()
        {
            var sql = $@"
                SELECT 
                    Logs.f_uid, Logs.f_iid, Logs.f_delivery_uid, 
                    Logs.f_status_from, Logs.f_status_to, Logs.f_create_date, Logs.f_create_by
                FROM [{_schema}].[t_delivery_logs] Logs
                WHERE Logs.f_delete_date IS NULL";

            return await _connection.QueryAsync<DeliveryLogsModel>(
                sql,
                transaction: _transaction,
                commandTimeout: _commandTimeout
            );
        }

        public async Task<DeliveryLogsModel> Find(Guid uid)
        {
            var sql = $@"
                SELECT 
                    Logs.f_uid, Logs.f_iid, Logs.f_delivery_uid, 
                    Logs.f_status_from, Logs.f_status_to, Logs.f_create_date, Logs.f_create_by
                FROM [{_schema}].[t_delivery_logs] Logs
                WHERE Logs.f_delete_date IS NULL AND Logs.f_uid = @f_uid";

            var result = await _connection.QueryFirstOrDefaultAsync<DeliveryLogsModel>(
                sql,
                new { f_uid = uid },
                transaction: _transaction,
                commandTimeout: _commandTimeout
            );

            return result ?? throw new KeyNotFoundException($"No transactional audit log matching UID {uid} was found.");
        }

        public async Task<GridDataModel<DeliveryLogsModel>> GetExtendedForGrid(FilterModel filter)
        {
            var sql = $@"
                 WITH LogsCTE AS (
                        SELECT
                            Logs.[f_uid], Logs.[f_iid], Logs.[f_delivery_uid], 
                            Logs.[f_status_from], Logs.[f_status_to], Logs.[f_create_date], 
                            Logs.[f_create_by], Logs.[f_update_date], Logs.[f_update_by], 
                            Logs.[f_delete_date], Logs.[f_delete_by]
                        FROM [{_schema}].[t_delivery_logs] Logs
                        WHERE Logs.f_delete_date IS NULL
                    )
                    SELECT COUNT(*) OVER() AS RowsCount, LogsCTE.* FROM LogsCTE";

            var whereSql = string.Empty;
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(filter.FilterString))
            {
                whereSql += $" WHERE {filter.FilterString}";
            }

            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                whereSql += string.IsNullOrEmpty(whereSql) ? " WHERE " : " AND ";
                whereSql += @"(
                    CAST([f_iid] AS NVARCHAR) LIKE @SearchParam OR  
                    CAST([f_delivery_uid] AS NVARCHAR) LIKE @SearchParam OR 
                    CAST([f_status_from] AS NVARCHAR) LIKE @SearchParam OR
                    CAST([f_status_to] AS NVARCHAR) LIKE @SearchParam
                )";
                parameters.Add("@SearchParam", $"%{filter.SearchValue}%");
            }

            sql += whereSql;
            sql += !string.IsNullOrEmpty(filter.OrderField) ? $" ORDER BY [{filter.OrderField}] {filter.Order}" : " ORDER BY [f_uid] DESC";

            if (filter.Take != 0)
            {
                sql += " OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
                parameters.Add("@Skip", filter.Skip);
                parameters.Add("@Take", filter.Take);
            }

            var rows = await _connection.QueryAsync<DeliveryLogsModel>(
                sql,
                parameters,
                transaction: _transaction,
                commandTimeout: _commandTimeout
            );

            return new GridDataModel<DeliveryLogsModel>
            {
                Count = rows.FirstOrDefault()?.RowsCount ?? 0,
                Items = rows
            };
        }

        public async Task<DeliveryLogsModel> Add(DeliveryLogsModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.UtcNow;
            model.f_create_by = _userUid;

            var sql = $@"
                INSERT INTO [{_schema}].[t_delivery_logs]
                (f_uid, f_delivery_uid, f_status_from, f_status_to, f_create_date, f_create_by)
                VALUES
                (@f_uid, @f_delivery_uid, @f_status_from, @f_status_to, @f_create_date, @f_create_by)";

            await _connection.ExecuteAsync(
                sql,
                model,
                transaction: _transaction,
                commandTimeout: _commandTimeout
            );
            return model;
        }

        public async Task<DeliveryLogsModel> Update(DeliveryLogsModel model)
        {
            model.f_update_date = DateTime.UtcNow;
            model.f_update_by = _userUid;

            var sql = $@"
                UPDATE [{_schema}].[t_delivery_logs]
                SET [f_delivery_uid] = @f_delivery_uid,
                    [f_status_from] = @f_status_from,
                    [f_status_to] = @f_status_to,
                    [f_update_date] = @f_update_date,
                    [f_update_by] = @f_update_by
                WHERE [f_uid] = @f_uid";

            await _connection.ExecuteAsync(
                sql,
                model,
                transaction: _transaction,
                commandTimeout: _commandTimeout
            );
            return model;
        }

        public async Task<int> Remove(DeliveryLogsModel model)
        {
            model.f_delete_date = DateTime.UtcNow;
            model.f_delete_by = _userUid;

            var sql = $@"
                UPDATE [{_schema}].[t_delivery_logs]
                SET f_delete_by = @f_delete_by, f_delete_date = @f_delete_date
                WHERE f_uid = @f_uid";

            return await _connection.ExecuteAsync(
                sql,
                model,
                transaction: _transaction,
                commandTimeout: _commandTimeout
            );
        }
    }
}