using BlazeQuartz.Core.Enums;
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
                var parameters = new DynamicParameters();

                var roles = await connection.QueryAsync<Role>(query, parameters);

                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
