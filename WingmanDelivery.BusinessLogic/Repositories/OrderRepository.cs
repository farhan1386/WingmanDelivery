using Dapper;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<OrderModel>> Get()
        {
            var sql = $@"
                SELECT 
                    Orders.f_uid,
                    Orders.f_customer_uid,
                    Orders.f_restaurant_uid,
                    Orders.f_status,
                    Orders.f_total_amount,
                    Orders.f_create_date,
                    Orders.f_create_by
                FROM 
                    [{_schema}].[t_orders] Orders
                WHERE 
                    Orders.f_delete_date IS NULL";

            return await _connection.QueryAsync<OrderModel>(sql, transaction: _transaction);
        }

        public async Task<OrderModel> Find(Guid uid)
        {
            var sql = $@"
                SELECT 
                    Orders.f_uid,
                    Orders.f_customer_uid,
                    Orders.f_restaurant_uid,
                    Orders.f_status,
                    Orders.f_total_amount,
                    Orders.f_create_date,
                    Orders.f_create_by
                FROM 
                    [{_schema}].[t_orders] Orders
                WHERE 
                    Orders.f_delete_date IS NULL AND 
                    Orders.f_uid = @f_uid";

            return await _connection.QueryFirstOrDefaultAsync<OrderModel>(sql, new { f_uid = uid }, transaction: _transaction);
        }

        public async Task<GridDataModel<OrderModel>> GetExtendedForGrid(FilterModel filter)
        {
            var sql = $@"
                 WITH Orders AS (
                        SELECT
                            Orders.[f_uid],
                            Orders.[f_customer_uid],
                            Orders.[f_restaurant_uid],
                            Orders.[f_status],
                            Orders.[f_total_amount],
                            Orders.[f_create_date],
                            Orders.[f_create_by],
                            Orders.[f_update_date],
                            Orders.[f_update_by],
                            Orders.[f_delete_date],
                            Orders.[f_delete_by]
                        FROM
                            [{_schema}].[t_orders] Orders
                        WHERE
                            Orders.f_delete_date IS NULL
                    )

                    SELECT 
                        COUNT(*) OVER() AS RowsCount,
                        Orders.*
                    FROM
                        Orders";

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
                    CAST([f_customer_uid] AS NVARCHAR(MAX)) LIKE @SearchParam OR 
                    CAST([f_restaurant_uid] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_status] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_total_amount] AS NVARCHAR(MAX)) LIKE @SearchParam OR
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

            var rows = await _connection.QueryAsync<OrderModel>(sql, parameters, transaction: _transaction);

            return new GridDataModel<OrderModel>
            {
                Count = rows.FirstOrDefault()?.RowsCount ?? 0,
                Items = rows
            };
        }

        public async Task<OrderModel> Add(OrderModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.UtcNow;
            model.f_create_by = _userUid;

            var sql = $@"
                INSERT INTO [{_schema}].[t_orders]
                (
                    f_uid,
                    f_customer_uid,
                    f_restaurant_uid,
                    f_status,
                    f_total_amount,
                    f_create_date,
                    f_create_by
                )
                VALUES
                (
                    @f_uid,
                    @f_customer_uid,
                    @f_restaurant_uid,
                    @f_status,
                    @f_total_amount,
                    @f_create_date,
                    @f_create_by
                )";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<OrderModel> Update(OrderModel model)
        {
            model.f_update_date = DateTime.UtcNow;
            model.f_update_by = _userUid;

            var sql = $@"
                UPDATE 
                   [{_schema}].[t_orders]
                SET 
                    [f_customer_uid] = @f_customer_uid,
                    [f_restaurant_uid] = @f_restaurant_uid,
                    [f_status] = @f_status,
                    [f_total_amount] = @f_total_amount,
                    [f_update_date] = @f_update_date,
                    [f_update_by] = @f_update_by
                WHERE 
                    [f_uid] = @f_uid";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<int> Remove(OrderModel model)
        {
            model.f_delete_date = DateTime.UtcNow;
            model.f_delete_by = _userUid;

            var sql = $@"
                UPDATE 
                    [{_schema}].[t_orders]
                SET
                    f_delete_by = @f_delete_by,
                    f_delete_date = @f_delete_date
                WHERE 
                    f_uid = @f_uid";

            return await _connection.ExecuteAsync(sql, model, _transaction);
        }

        public async Task<IEnumerable<OrderModel>> GetActiveOrdersAsync()
        {
            var sql = $@"
                SELECT 
                    Orders.f_uid,
                    Orders.f_customer_uid,
                    Orders.f_restaurant_uid,
                    Orders.f_status,
                    Orders.f_total_amount,
                    Orders.f_create_date,
                    Orders.f_create_by
                FROM 
                    [{_schema}].[t_orders] Orders
                WHERE 
                    Orders.f_status IN (0,1,2) AND 
                    Orders.f_delete_date IS NULL";

            return await _connection.QueryAsync<OrderModel>(sql, transaction: _transaction);
        }
    }
}