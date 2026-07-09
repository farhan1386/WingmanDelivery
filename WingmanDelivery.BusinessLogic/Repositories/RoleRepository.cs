using Dapper;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<RoleModel>> Get()
        {
            var sql = $@"
                SELECT 
                    Roles.f_uid,
                    Roles.f_role_name,
                    Roles.f_description,
                    Roles.f_active,
                    Roles.f_create_date,
                    Roles.f_create_by
                FROM 
                    [{_schema}].[t_roles] Roles
                WHERE 
                    Roles.f_delete_date IS NULL";

            return await _connection.QueryAsync<RoleModel>(sql, transaction: _transaction);
        }

        public async Task<RoleModel> Find(Guid uid)
        {
            var sql = $@"
                SELECT 
                    Roles.f_uid,
                    Roles.f_role_name,
                    Roles.f_description,
                    Roles.f_active,
                    Roles.f_create_date,
                    Roles.f_create_by
                FROM 
                    [{_schema}].[t_roles] Roles
                WHERE 
                    Roles.f_delete_date IS NULL AND 
                    Roles.f_uid = @f_uid";

            return await _connection.QueryFirstOrDefaultAsync<RoleModel>(sql, new { f_uid = uid }, transaction: _transaction);
        }

        public async Task<GridDataModel<RoleModel>> GetExtendedForGrid(FilterModel filter)
        {
            var sql = $@"
                 WITH Roles AS (
                        SELECT
                            Roles.[f_uid],
                            Roles.[f_role_name],
                            Roles.[f_description],
                            Roles.[f_active],
                            Roles.[f_create_date],
                            Roles.[f_create_by],
                            Roles.[f_update_date],
                            Roles.[f_update_by],
                            Roles.[f_delete_date],
                            Roles.[f_delete_by]
                        FROM
                            [{_schema}].[t_roles] Roles
                        WHERE
                            Roles.f_delete_date IS NULL
                    )

                    SELECT 
                        COUNT(*) OVER() AS RowsCount,
                        Roles.*
                    FROM
                        Roles";

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
                    [f_role_name] LIKE @SearchParam OR 
                    [f_description] LIKE @SearchParam OR
                    CAST([f_active] AS NVARCHAR(MAX)) LIKE @SearchParam OR
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

            var rows = await _connection.QueryAsync<RoleModel>(sql, parameters, transaction: _transaction);

            return new GridDataModel<RoleModel>
            {
                Count = rows.FirstOrDefault()?.RowsCount ?? 0,
                Items = rows
            };
        }

        public async Task<RoleModel> Add(RoleModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.UtcNow;
            model.f_create_by = _userUid;

            var sql = $@"
                INSERT INTO [{_schema}].[t_roles]
                (
                    f_uid,
                    f_role_name,
                    f_description,
                    f_active,
                    f_create_date,
                    f_create_by
                )
                VALUES
                (
                    @f_uid,
                    @f_role_name,
                    @f_description,
                    @f_active,
                    @f_create_date,
                    @f_create_by
                )";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<RoleModel> Update(RoleModel model)
        {
            model.f_update_date = DateTime.UtcNow;
            model.f_update_by = _userUid;

            var sql = $@"
                UPDATE 
                   [{_schema}].[t_roles]
                SET 
                    [f_role_name] = @f_role_name,
                    [f_description] = @f_description,
                    [f_active] = @f_active,
                    [f_update_date] = @f_update_date,
                    [f_update_by] = @f_update_by
                WHERE 
                    [f_uid] = @f_uid";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<int> Remove(RoleModel model)
        {
            model.f_delete_date = DateTime.UtcNow;
            model.f_delete_by = _userUid;

            var sql = $@"
                UPDATE 
                    [{_schema}].[t_roles]
                SET
                    f_delete_by = @f_delete_by,
                    f_delete_date = @f_delete_date
                WHERE 
                    f_uid = @f_uid";

            return await _connection.ExecuteAsync(sql, model, _transaction);
        }
    }
}