using Dapper;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public class OrderItemRepository : BaseRepository, IOrderItemsRepository
    {
        public OrderItemRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<OrderItemModel>> Get()
        {
            var sql = $@"
                SELECT 
                    OrderItems.f_uid,
                    OrderItems.f_order_uid,
                    OrderItems.f_menu_item_uid,
                    OrderItems.f_quantity,
                    OrderItems.f_create_date,
                    OrderItems.f_create_by
                FROM 
                    [{_schema}].[t_order_items] OrderItems
                WHERE 
                    OrderItems.f_delete_date IS NULL";

            return await _connection.QueryAsync<OrderItemModel>(sql, transaction: _transaction);
        }

        public async Task<OrderItemModel> Find(Guid uid)
        {
            var sql = $@"
                SELECT 
                    OrderItems.f_uid,
                    OrderItems.f_order_uid,
                    OrderItems.f_menu_item_uid,
                    OrderItems.f_quantity,
                    OrderItems.f_create_date,
                    OrderItems.f_create_by
                FROM 
                    [{_schema}].[t_order_items] OrderItems
                WHERE 
                    OrderItems.f_delete_date IS NULL AND 
                    OrderItems.f_uid = @f_uid";

            return await _connection.QueryFirstOrDefaultAsync<OrderItemModel>(sql, new { f_uid = uid }, transaction: _transaction);
        }

        public async Task<IEnumerable<OrderItemModel>> GetByOrderUidAsync(Guid orderUid)
        {
            var sql = $@"
                SELECT 
                    OrderItems.f_uid,
                    OrderItems.f_order_uid,
                    OrderItems.f_menu_item_uid,
                    OrderItems.f_quantity,
                    OrderItems.f_create_date,
                    OrderItems.f_create_by
                FROM 
                    [{_schema}].[t_order_items] OrderItems
                WHERE 
                    OrderItems.f_delete_date IS NULL AND 
                    OrderItems.f_order_uid = @f_order_uid";

            return await _connection.QueryAsync<OrderItemModel>(sql, new { f_order_uid = orderUid }, transaction: _transaction);
        }

        public async Task<GridDataModel<OrderItemModel>> GetExtendedForGrid(FilterModel filter)
        {
            var sql = $@"
                 WITH OrderItems AS (
                        SELECT
                            OrderItems.[f_uid],
                            OrderItems.[f_order_uid],
                            OrderItems.[f_menu_item_uid],
                            OrderItems.[f_quantity],
                            OrderItems.[f_create_date],
                            OrderItems.[f_create_by],
                            OrderItems.[f_update_date],
                            OrderItems.[f_update_by],
                            OrderItems.[f_delete_date],
                            OrderItems.[f_delete_by]
                        FROM
                            [{_schema}].[t_order_items] OrderItems
                        WHERE
                            OrderItems.f_delete_date IS NULL
                    )

                    SELECT 
                        COUNT(*) OVER() AS RowsCount,
                        OrderItems.*
                    FROM
                        OrderItems";

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
                    CAST([f_menu_item_uid] AS NVARCHAR(MAX)) LIKE @SearchParam OR
                    CAST([f_quantity] AS NVARCHAR(MAX)) LIKE @SearchParam OR
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

            var rows = await _connection.QueryAsync<OrderItemModel>(sql, parameters, transaction: _transaction);

            return new GridDataModel<OrderItemModel>
            {
                Count = rows.FirstOrDefault()?.RowsCount ?? 0,
                Items = rows
            };
        }

        public async Task<OrderItemModel> Add(OrderItemModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.UtcNow;
            model.f_create_by = _userUid;

            var sql = $@"
                INSERT INTO [{_schema}].[t_order_items]
                (
                    f_uid,
                    f_order_uid,
                    f_menu_item_uid,
                    f_quantity,
                    f_create_date,
                    f_create_by
                )
                VALUES
                (
                    @f_uid,
                    @f_order_uid,
                    @f_menu_item_uid,
                    @f_quantity,
                    @f_create_date,
                    @f_create_by
                )";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<OrderItemModel> Update(OrderItemModel model)
        {
            model.f_update_date = DateTime.UtcNow;
            model.f_update_by = _userUid;

            var sql = $@"
                UPDATE 
                   [{_schema}].[t_order_items]
                SET 
                    [f_order_uid] = @f_order_uid,
                    [f_menu_item_uid] = @f_menu_item_uid,
                    [f_quantity] = @f_quantity,
                    [f_update_date] = @f_update_date,
                    [f_update_by] = @f_update_by
                WHERE 
                    [f_uid] = @f_uid";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<int> Remove(OrderItemModel model)
        {
            model.f_delete_date = DateTime.UtcNow;
            model.f_delete_by = _userUid;

            var sql = $@"
                UPDATE 
                    [{_schema}].[t_order_items]
                SET
                    f_delete_by = @f_delete_by,
                    f_delete_date = @f_delete_date
                WHERE 
                    f_uid = @f_uid";

            return await _connection.ExecuteAsync(sql, model, _transaction);
        }
    }
}