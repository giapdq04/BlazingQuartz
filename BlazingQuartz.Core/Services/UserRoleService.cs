using BlazeQuartz.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BlazeQuartz.Core.Services
{
    public class UserRoleService
    {
        private static string _connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly IConfiguration _config;
        public UserRoleService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "SELECT * from USER_ROLES";

                var roles = await connection.QueryAsync<Role>(query);

                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddRole(Role role)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "INSERT INTO USER_ROLES(ROLE_NAME) " +
                                "VALUES(@roleName)";
                var parameters = new DynamicParameters();
                parameters.Add("roleName", role.Role_Name.ToUpper());
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                if (ex.Message == "SQLite Error 19: 'UNIQUE constraint failed: USER_ROLES.ROLE_NAME'.")
                {
                    throw new Exception("This role name already exists");
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<bool> EditRole(Role role)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "UPDATE USER_ROLES " +
                                "SET ROLE_NAME = @roleName " +
                                "WHERE ROLE_ID = @roleId";
                var parameters = new DynamicParameters();
                parameters.Add("roleName", role.Role_Name.ToUpper());
                parameters.Add("roleId", role.Role_Id);
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteRole(decimal Id)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "DELETE FROM USER_ROLES " +
                                "WHERE ROLE_ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);

                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
