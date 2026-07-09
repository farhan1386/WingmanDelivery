using Dapper;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public class PaymentRepository : BaseRepository, IPaymentRepository
    {
        public PaymentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<PaymentModel>> Get()
        {
            var sql = $@"
                SELECT 
                    Payments.f_uid,
                    Payments.f_order_uid,
                    Payments.f_amount,
                    Payments.f_method,
                    Payments.f_status,
                    Payments.f_create_date,
                    Payments.f_create_by
                FROM 
                    [{_schema}].[t_payments] Payments
                WHERE 
                    Payments.f_delete_date IS NULL";

            return await _connection.QueryAsync<PaymentModel>(sql, transaction: _transaction);
        }

        public async Task<PaymentModel> Find(Guid uid)
        {
            var sql = $@"
                SELECT 
                    Payments.f_uid,
                    Payments.f_order_uid,
                    Payments.f_amount,
                    Payments.f_method,
                    Payments.f_status,
                    Payments.f_create_date,
                    Payments.f_create_by
                FROM 
                    [{_schema}].[t_payments] Payments
                WHERE 
                    Payments.f_delete_date IS NULL AND 
                    Payments.f_uid = @f_uid";

            return await _connection.QueryFirstOrDefaultAsync<PaymentModel>(sql, new { f_uid = uid }, transaction: _transaction);
        }

        public async Task<IEnumerable<PaymentModel>> GetByOrderUidAsync(Guid orderUid)
        {
            var sql = $@"
                SELECT 
                    Payments.f_uid,
                    Payments.f_order_uid,
                    Payments.f_amount,
                    Payments.f_method,
                    Payments.f_status,
                    Payments.f_create_date,
                    Payments.f_create_by
                FROM 
                    [{_schema}].[t_payments] Payments
                WHERE 
                    Payments.f_delete_date IS NULL AND 
                    Payments.f_order_uid = @f_order_uid";

            return await _connection.QueryAsync<PaymentModel>(sql, new { f_order_uid = orderUid }, transaction: _transaction);
        }

        public async Task<GridDataModel<PaymentModel>> GetExtendedForGrid(FilterModel filter)
        {
            var sql = $@"
                 WITH Payments AS (
                        SELECT
                            Payments.[f_uid],
                            Payments.[f_order_uid],
                            Payments.[f_amount],
                            Payments.[f_method],
                            Payments.[f_status],
                            Payments.[f_create_date],
                            Payments.[f_create_by],
                            Payments.[f_update_date],
                            Payments.[f_update_by],
                            Payments.[f_delete_date],
                            Payments.[f_delete_by]
                        FROM
                            [{_schema}].[t_payments] Payments
                        WHERE
                            Payments.f_delete_date IS NULL
                    )

                    SELECT 
                        COUNT(*) OVER() AS RowsCount,
                        Payments.*
                    FROM
                        Payments";

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
                    CAST([f_order_uid] AS NVARCHAR(MAX)) LIKE @SearchParam OR 
                    CAST([f_amount] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    [f_method] LIKE @SearchParam OR
                    [f_status] LIKE @SearchParam OR
                    CAST([f_create_date] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_create_by] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_update_date] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_update_by] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_delete_date] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_delete_by] AS NVARCHAR(MAX)) LIKE @SearchParam
                )";

                parameters.Add("@SearchParam", $"%{filter.SearchValue}%");
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

            var rows = await _connection.QueryAsync<PaymentModel>(sql, parameters, transaction: _transaction);

            return new GridDataModel<PaymentModel>
            {
                Count = rows.FirstOrDefault()?.RowsCount ?? 0,
                Items = rows
            };
        }

        public async Task<PaymentModel> Add(PaymentModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.UtcNow;
            model.f_create_by = _userUid;

            var sql = $@"
                INSERT INTO [{_schema}].[t_payments]
                (
                    f_uid,
                    f_order_uid,
                    f_amount,
                    f_method,
                    f_status,
                    f_create_date,
                    f_create_by
                )
                VALUES
                (
                    @f_uid,
                    @f_order_uid,
                    @f_amount,
                    @f_method,
                    @f_status,
                    @f_create_date,
                    @f_create_by
                )";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<PaymentModel> Update(PaymentModel model)
        {
            model.f_update_date = DateTime.UtcNow;
            model.f_update_by = _userUid;

            var sql = $@"
                UPDATE 
                   [{_schema}].[t_payments]
                SET 
                    [f_order_uid] = @f_order_uid,
                    [f_amount] = @f_amount,
                    [f_method] = @f_method,
                    [f_status] = @f_status,
                    [f_update_date] = @f_update_date,
                    [f_update_by] = @f_update_by
                WHERE 
                    [f_uid] = @f_uid";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<int> Remove(PaymentModel model)
        {
            model.f_delete_date = DateTime.UtcNow;
            model.f_delete_by = _userUid;

            var sql = $@"
                UPDATE 
                    [{_schema}].[t_payments]
                SET
                    f_delete_by = @f_delete_by,
                    f_delete_date = @f_delete_date
                WHERE 
                    f_uid = @f_uid";

            return await _connection.ExecuteAsync(sql, model, _transaction);
        }
    }
}