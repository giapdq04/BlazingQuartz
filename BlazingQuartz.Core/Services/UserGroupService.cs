using BlazeQuartz.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BlazeQuartz.Core.Services
{
    public class UserGroupService
    {
        private static string _connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly IConfiguration _config;
        public UserGroupService(IConfiguration config)
        {
            _config = config;
        }


        public async Task<(IEnumerable<UserGroup> groups, int totalCount)> GetAllGroup(int page, int pageSize, string searchTerm)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                int offset = page * pageSize;

                string query = "SELECT * from USER_GROUPS WHERE 1=1";
                string countQuery = "SELECT COUNT(*) FROM USER_GROUPS WHERE 1=1";
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND GROUP_NAME LIKE @SearchTerm";
                    countQuery += " AND GROUP_NAME LIKE @SearchTerm";

                    parameters.Add("SearchTerm", $"%{searchTerm.ToUpper()}%");
                }

                query += " ORDER BY ID LIMIT @PageSize OFFSET @Offset";
                parameters.Add("PageSize", pageSize);
                parameters.Add("Offset", offset);

                var groups = await connection.QueryAsync<UserGroup>(query, parameters);
                int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

                return (groups, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users", ex);
            }
        }

        public async Task<IEnumerable<UserGroup>> GetAllGroups()
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "SELECT * FROM USER_GROUPS";

                var roles = await connection.QueryAsync<UserGroup>(query);

                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users", ex);
            }
        }

        public async Task<bool> AddGroup(UserGroup group)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "INSERT INTO USER_GROUPS (GROUP_NAME) VALUES (@groupName)";
                var parameters = new DynamicParameters();
                parameters.Add("groupName", group.GROUP_NAME.ToUpper());
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user", ex);
            }
        }

        public async Task<bool> EditGroup(UserGroup group)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "UPDATE USER_GROUPS " +
                                "SET GROUP_NAME = @groupName " +
                                "WHERE ID = @id";
                var parameters = new DynamicParameters();
                parameters.Add("groupName", group.GROUP_NAME.ToUpper());
                parameters.Add("id", group.ID);
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error editing user", ex);
            }
        }

        public async Task<bool> DeleteGroup(decimal Id)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "DELETE FROM USER_GROUPS " +
                                "WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);

                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user", ex);
            }
        }

    }


}
