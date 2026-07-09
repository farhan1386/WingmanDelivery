using Dapper;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        public NotificationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<NotificationModel>> Get()
        {
            var sql = $@"
                SELECT 
                    Notifications.f_uid,
                    Notifications.f_user_uid,
                    Notifications.f_message,
                    Notifications.f_type,
                    Notifications.f_status,
                    Notifications.f_create_date,
                    Notifications.f_create_by
                FROM 
                    [{_schema}].[t_notifications] Notifications
                WHERE 
                    Notifications.f_delete_date IS NULL";

            return await _connection.QueryAsync<NotificationModel>(sql, transaction: _transaction);
        }

        public async Task<NotificationModel> Find(Guid uid)
        {
            var sql = $@"
                SELECT 
                    Notifications.f_uid,
                    Notifications.f_user_uid,
                    Notifications.f_message,
                    Notifications.f_type,
                    Notifications.f_status,
                    Notifications.f_create_date,
                    Notifications.f_create_by
                FROM 
                    [{_schema}].[t_notifications] Notifications
                WHERE 
                    Notifications.f_delete_date IS NULL AND 
                    Notifications.f_uid = @f_uid";

            return await _connection.QueryFirstOrDefaultAsync<NotificationModel>(sql, new { f_uid = uid }, transaction: _transaction);
        }

        public async Task<IEnumerable<NotificationModel>> GetByUserUidAsync(Guid userUid)
        {
            var sql = $@"
                SELECT 
                    Notifications.f_uid,
                    Notifications.f_user_uid,
                    Notifications.f_message,
                    Notifications.f_type,
                    Notifications.f_status,
                    Notifications.f_create_date,
                    Notifications.f_create_by
                FROM 
                    [{_schema}].[t_notifications] Notifications
                WHERE 
                    Notifications.f_delete_date IS NULL AND 
                    Notifications.f_user_uid = @f_user_uid";

            return await _connection.QueryAsync<NotificationModel>(sql, new { f_user_uid = userUid }, transaction: _transaction);
        }

        public async Task<GridDataModel<NotificationModel>> GetExtendedForGrid(FilterModel filter)
        {
            var sql = $@"
                 WITH Notifications AS (
                        SELECT
                            Notifications.[f_uid],
                            Notifications.[f_user_uid],
                            Notifications.[f_message],
                            Notifications.[f_type],
                            Notifications.[f_status],
                            Notifications.[f_create_date],
                            Notifications.[f_create_by],
                            Notifications.[f_update_date],
                            Notifications.[f_update_by],
                            Notifications.[f_delete_date],
                            Notifications.[f_delete_by]
                        FROM
                            [{_schema}].[t_notifications] Notifications
                        WHERE
                            Notifications.f_delete_date IS NULL
                    )

                    SELECT 
                        COUNT(*) OVER() AS RowsCount,
                        Notifications.*
                    FROM
                        Notifications";

            var whereSql = String.Empty;
            var parameters = new DynamicParameters();

            if (!String.IsNullOrEmpty(filter.FilterString))
            {
                whereSql += $" WHERE {filter.FilterString}";
            }

            if (!String.IsNullOrEmpty(filter.SearchValue))
            {
                if (!String.IsNullOrEmpty(whereSql))
                {
                    whereSql += " AND ";
                }
                else
                {
                    whereSql += " WHERE ";
                }

                whereSql += $@"
                (
                    CAST([f_uid] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_user_uid] AS NVARCHAR(MAX)) LIKE @SearchParam OR 
                    [f_message] LIKE @SearchParam OR
                    CAST([f_type] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_status] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_create_date] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_create_by] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_update_date] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_update_by] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_delete_date] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_delete_by] AS NVARCHAR(MAX)) LIKE @SearchParam
                )";

                parameters.Add("@SearchParam", $"% {filter.SearchValue}%");
            }

            sql += whereSql;

            if (!string.IsNullOrEmpty(filter.OrderField))
            {
                var cleanOrderField = filter.OrderField.Replace("[", "").Replace("]", "");
                var direction = filter.Order?.ToUpper() == "DESC" ? "DESC" : "ASC";
                sql += $@"
                        ORDER BY
            [{cleanOrderField}] {direction}";
            }
            else
            {
                sql += @"
                    ORDER BY
                        [f_uid] DESC";
            }

            if (filter.Take != 0)
            {
                sql += $@"
                        OFFSET
                @Skip ROWS FETCH NEXT @Take ROWS ONLY";
                parameters.Add("@Skip", filter.Skip);
                parameters.Add("@Take", filter.Take);
            }

            var rows = await _connection.QueryAsync<NotificationModel>(sql, parameters, transaction: _transaction);

            return new GridDataModel<NotificationModel>
            {
                Count = rows.FirstOrDefault()?.RowsCount ?? 0,
                Items = rows
            };
        }

        public async Task<NotificationModel> Add(NotificationModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.UtcNow;
            model.f_create_by = _userUid;

            var sql = $@"
                INSERT INTO [{_schema}].[t_notifications]
                (
                    f_uid,
                    f_user_uid,
                    f_message,
                    f_type,
                    f_status,
                    f_create_date,
                    f_create_by
                )
                VALUES
                (
                    @f_uid,
                    @f_user_uid,
                    @f_message,
                    @f_type,
                    @f_status,
                    @f_create_date,
                    @f_create_by
                )";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<NotificationModel> Update(NotificationModel model)
        {
            model.f_update_date = DateTime.UtcNow;
            model.f_update_by = _userUid;

            var sql = $@"
                UPDATE 
                   [{_schema}].[t_notifications]
                SET 
                    [f_user_uid] = @f_user_uid,
                    [f_message] = @f_message,
                    [f_type] = @f_type,
                    [f_status] = @f_status,
                    [f_update_date] = @f_update_date,
                    [f_update_by] = @f_update_by
                WHERE 
                    [f_uid] = @f_uid";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<int> Remove(NotificationModel model)
        {
            model.f_delete_date = DateTime.UtcNow;
            model.f_delete_by = _userUid;

            var sql = $@"
                UPDATE 
                    [{_schema}].[t_notifications]
                SET
                    f_delete_by = @f_delete_by,
                    f_delete_date = @f_delete_date
                WHERE 
                    f_uid = @f_uid";

            return await _connection.ExecuteAsync(sql, model, _transaction);
        }
    }
}