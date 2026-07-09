using Dapper;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<UserModel>> Get()
        {
            var sql = $@"
                SELECT 
                    Users.f_uid,
                    Users.f_fname,
                    Users.f_lname,
                    Users.f_phone,
                    Users.f_email,
                    Users.f_password_hash,
                    Users.f_role_uid,
                    Users.f_create_date,
                    Users.f_create_by
                FROM 
                    [{_schema}].[t_users] Users
                WHERE 
                    Users.f_delete_date IS NULL";

            return await _connection.QueryAsync<UserModel>(sql, transaction: _transaction);
        }

        public async Task<UserModel> Find(Guid uid)
        {
            var sql = $@"
                SELECT 
                    Users.f_uid,
                    Users.f_fname,
                    Users.f_lname,
                    Users.f_phone,
                    Users.f_email,
                    Users.f_password_hash,
                    Users.f_role_uid,
                    Users.f_create_date,
                    Users.f_create_by
                FROM 
                    [{_schema}].[t_users] Users
                WHERE 
                    Users.f_delete_date IS NULL AND 
                    Users.f_uid = @f_uid";

            return await _connection.QueryFirstOrDefaultAsync<UserModel>(sql, new { f_uid = uid }, transaction: _transaction);
        }

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var sql = $@"
                SELECT 
                    Users.f_uid,
                    Users.f_fname,
                    Users.f_lname,
                    Users.f_phone,
                    Users.f_email,
                    Users.f_password_hash,
                    Users.f_role_uid,
                    Users.f_create_date,
                    Users.f_create_by
                FROM 
                    [{_schema}].[t_users] Users
                WHERE 
                    Users.f_delete_date IS NULL AND 
                    Users.f_email = @f_email";

            return await _connection.QueryFirstOrDefaultAsync<UserModel>(sql, new { f_email = email }, transaction: _transaction);
        }

        public async Task<GridDataModel<UserModel>> GetExtendedForGrid(FilterModel filter)
        {
            var sql = $@"
                 WITH Users AS (
                        SELECT
                            Users.[f_uid],
                            Users.[f_fname],
                            Users.[f_lname],
                            Users.[f_phone],
                            Users.[f_email],
                            Users.[f_password_hash],
                            Users.[f_role_uid],
                            Users.[f_create_date],
                            Users.[f_create_by],
                            Users.[f_update_date],
                            Users.[f_update_by],
                            Users.[f_delete_date],
                            Users.[f_delete_by]
                        FROM
                            [{_schema}].[t_users] Users
                        WHERE
                            Users.f_delete_date IS NULL
                    )

                    SELECT 
                        COUNT(*) OVER() AS RowsCount,
                        Users.*
                    FROM
                        Users";

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
                    [f_fname] LIKE @SearchParam OR
                    [f_lname] LIKE @SearchParam OR
                    [f_phone] LIKE @SearchParam OR
                    [f_email] LIKE @SearchParam OR
                    [f_password_hash] LIKE @SearchParam OR
                    CAST([f_role_uid] AS NVARCHAR(MAX)) LIKE @SearchParam OR
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

            var rows = await _connection.QueryAsync<UserModel>(sql, parameters, transaction: _transaction);

            return new GridDataModel<UserModel>
            {
                Count = rows.FirstOrDefault()?.RowsCount ?? 0,
                Items = rows
            };
        }

        public async Task<UserModel> Add(UserModel model)
        {
            model.f_uid = Guid.NewGuid();
            model.f_create_date = DateTime.UtcNow;
            model.f_create_by = _userUid;

            var sql = $@"
                INSERT INTO [{_schema}].[t_users]
                (
                    f_uid,
                    f_fname,
                    f_lname,
                    f_phone,
                    f_email,
                    f_password_hash,
                    f_role_uid,
                    f_create_date,
                    f_create_by
                )
                VALUES
                (
                    @f_uid,
                    @f_fname,
                    @f_lname,
                    @f_phone,
                    @f_email,
                    @f_password_hash,
                    @f_role_uid,
                    @f_create_date,
                    @f_create_by
                )";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<UserModel> Update(UserModel model)
        {
            model.f_update_date = DateTime.UtcNow;
            model.f_update_by = _userUid;

            var sql = $@"
                UPDATE 
                   [{_schema}].[t_users]
                SET 
                    [f_fname] = @f_fname,
                    [f_lname] = @f_lname,
                    [f_phone] = @f_phone,
                    [f_email] = @f_email,
                    [f_password_hash] = @f_password_hash,
                    [f_role_uid] = @f_role_uid,
                    [f_update_date] = @f_update_date,
                    [f_update_by] = @f_update_by
                WHERE 
                    [f_uid] = @f_uid";

            await _connection.ExecuteAsync(sql, model, _transaction);
            return model;
        }

        public async Task<int> Remove(UserModel model)
        {
            model.f_delete_date = DateTime.UtcNow;
            model.f_delete_by = _userUid;

            var sql = $@"
                UPDATE 
                    [{_schema}].[t_users]
                SET
                    f_delete_by = @f_delete_by,
                    f_delete_date = @f_delete_date
                WHERE 
                    f_uid = @f_uid";

            return await _connection.ExecuteAsync(sql, model, _transaction);
        }
    }
}