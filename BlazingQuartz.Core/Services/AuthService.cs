using BlazeQuartz.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BlazeQuartz.Core.Services
{
    public class AuthService
    {
        private static string _connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly IConfiguration _config;
        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<User> Login(User u)
        {
            _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
            using var connection = new SqliteConnection(_connectionString);

            string query = "SELECT * " +
                            "FROM Users " +
                            "WHERE USERNAME = @Username " +
                            "AND PASSWORD = @Password";
            var parameters = new DynamicParameters();
            parameters.Add("Username", u.Username);
            parameters.Add("Password", u.Password);

            var user = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            return user;
        }
    }
}
