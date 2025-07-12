
using BlazeQuartz.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BlazeQuartz.Core.Services
{
    public class JobAssignmentService
    {
        private static string _connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly IConfiguration _config;
        public JobAssignmentService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<JobAssignment>> GetAllJobAssigments(string userId)
        {
            _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
            using var connection = new SqliteConnection(_connectionString);

            string query = "SELECT * FROM JOB_ASSIGNMENTS WHERE 1=1 ";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(userId))
            {
                query += " AND USER_ID = @userId OR GROUP_ID IN (" +
                            "SELECT GROUP_ID " +
                            "FROM USER_GROUP_MEMBER " +
                            "WHERE USER_ID = @userId);";
                parameters.Add("userId", Convert.ToInt32(userId));
            }

            var jobs = await connection.QueryAsync<JobAssignment>(query, parameters);
            return jobs;
        }
    }
}
