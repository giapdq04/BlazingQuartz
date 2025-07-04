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
            parameters.Add("Username", u.Username.ToUpper());
            parameters.Add("Password", u.Password);

            var user = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
            return user;
        }

        public async Task<(IEnumerable<User> users, int totalCount)> GetAllUsersAsync(int page, int pageSize, string searchTerm)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                int offset = page * pageSize;

                string query = "SELECT * FROM Users WHERE 1=1";
                string countQuery = "SELECT COUNT(*) FROM USERS WHERE 1=1";
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND USERNAME LIKE @SearchTerm";
                    countQuery += " AND USERNAME LIKE @SearchTerm";

                    parameters.Add("SearchTerm", $"%{searchTerm.ToUpper()}%");
                }

                query += " ORDER BY USERID LIMIT @PageSize OFFSET @Offset";
                parameters.Add("PageSize", pageSize);
                parameters.Add("Offset", offset);
                var users = await connection.QueryAsync<User>(query, parameters);
                countQuery += "";
                int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

                return (users, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users", ex);
            }
        }

        public async Task<bool> AddUser(User user)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "INSERT INTO USERS (USERNAME, PASSWORD, ROLE) " +
                                "VALUES (@Username, @Password, @Role)";
                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username.ToUpper());
                parameters.Add("Password", user.Password);
                parameters.Add("Role", user.Role);
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user", ex);
            }
        }

        public async Task<bool> DeleteUser(decimal userId)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "DELETE FROM USERS " +
                                "WHERE USERID = @userId";
                var parameters = new DynamicParameters();
                parameters.Add("userId", userId);

                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user", ex);
            }
        }

        public async Task<bool> EditUser(User user)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "UPDATE USERS " +
                                "SET USERNAME = @Username, " +
                                "PASSWORD = @Password, " +
                                "ROLE = @Role " +
                                "WHERE USERID = @UserId";
                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username.ToUpper());
                parameters.Add("Password", user.Password);
                parameters.Add("Role", user.Role);
                parameters.Add("UserId", user.UserId);
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error editing user", ex);
            }
        }
    }
}
