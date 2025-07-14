using BlazeQuartz.Core.Enums;
using BlazeQuartz.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace BlazeQuartz.Core.Services
{
    public class UserService
    {
        private static string _connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly IConfiguration _config;
        public UserService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<User> Login(User u)
        {
            _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
            using var connection = new SqliteConnection(_connectionString);

            string query = "SELECT USERS.*, USER_ROLES.ROLE_NAME ROLE " +
                "from USERS " +
                "inner join USER_ROLES " +
                "on USER_ROLES.ROLE_ID = USERS.ROLE_ID " +
                "WHERE USERNAME = @Username AND PASSWORD = @Password";
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

                string query = "SELECT U.USERID, U.USERNAME, U.PASSWORD, UR.ROLE_NAME AS ROLE, GROUP_CONCAT(G.GROUP_NAME, ', ') AS GROUP_NAME " +
                    "FROM Users U " +
                    "LEFT JOIN USER_GROUP_MEMBER GM " +
                    "ON U.USERID = GM.USER_ID " +
                    "LEFT JOIN USER_GROUPS G " +
                    "ON GM.GROUP_ID = G.ID " +
                    "LEFT JOIN USER_ROLES UR " +
                    "ON UR.ROLE_ID = U.ROLE_ID " +
                    "WHERE 1 = 1 ";
                string countQuery = "SELECT COUNT(*) FROM USERS WHERE 1=1";
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND USERNAME LIKE @SearchTerm";
                    countQuery += " AND USERNAME LIKE @SearchTerm";

                    parameters.Add("SearchTerm", $"%{searchTerm.ToUpper()}%");
                }

                query +=
                    "GROUP BY U.USERID, U.USERNAME, U.PASSWORD, UR.ROLE_NAME" +
                    " ORDER BY USERID LIMIT @PageSize OFFSET @Offset";
                parameters.Add("PageSize", pageSize);
                parameters.Add("Offset", offset);
                var users = await connection.QueryAsync<User>(query, parameters);
                int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

                return (users, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "SELECT u.USERID,u.USERNAME from USERS u";

                var roles = await connection.QueryAsync<User>(query);

                return roles;
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
                if (ex.Message == "SQLite Error 19: 'UNIQUE constraint failed: USERS.USERNAME'.")
                {
                    throw new Exception("This username already exists");
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<bool> AddUserToGroup(int userId, int groupId)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "insert into USER_GROUP_MEMBER (USER_ID, GROUP_ID)" +
                                "values (@userId, @groupId);";
                var parameters = new DynamicParameters();
                parameters.Add("userId", userId);
                parameters.Add("groupId", groupId);
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

        public async Task<bool> DeleteUserFromGroup(int userId, int groupId)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "delete from USER_GROUP_MEMBER " +
                                "where USER_ID = @userId " +
                                "and GROUP_ID = @groupId ";
                var parameters = new DynamicParameters();
                parameters.Add("userId", userId);
                parameters.Add("groupId", groupId);

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

        public async Task<(IEnumerable<User> users, int totalCount)> GetUsersByGroupId(int page, int pageSize, string searchTerm, int groupId)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                int offset = page * pageSize;

                string query = "select USERID, USERNAME, ROLE " +
                                "from USERS " +
                                "inner join USER_GROUP_MEMBER " +
                                "on USERS.USERID = USER_GROUP_MEMBER.USER_ID " +
                                "where USER_GROUP_MEMBER.GROUP_ID = @groupId ";
                string countQuery = "select count(*) from USERS " +
                                    "inner join USER_GROUP_MEMBER " +
                                    "on USERS.USERID = USER_GROUP_MEMBER.USER_ID " +
                                    "where USER_GROUP_MEMBER.GROUP_ID = @groupId;";
                var parameters = new DynamicParameters();
                parameters.Add("groupId", groupId);

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
                int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

                return (users, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users", ex);
            }
        }

        public async Task<(IEnumerable<User> users, int totalCount)> GetUsersNotInGroup(int page, int pageSize, string searchTerm, int groupId)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                int offset = page * pageSize;

                string query = "SELECT USERID, USERNAME, ROLE " +
                                "FROM USERS u " +
                                "LEFT JOIN USER_GROUP_MEMBER ugm " +
                                "ON u.USERID = ugm.USER_ID AND ugm.GROUP_ID = @groupId " +
                                "WHERE ugm.USER_ID IS NULL ";
                string countQuery = "SELECT count(*) " +
                                    "FROM USERS u " +
                                    "LEFT JOIN USER_GROUP_MEMBER ugm " +
                                    "ON u.USERID = ugm.USER_ID AND ugm.GROUP_ID = @groupId " +
                                    "WHERE ugm.USER_ID IS NULL ";
                var parameters = new DynamicParameters();
                parameters.Add("groupId", groupId);

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
                int totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

                return (users, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users", ex);
            }
        }
    }
}
