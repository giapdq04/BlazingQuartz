using BlazeQuartz.Core.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BlazeQuartz.Core.Services
{
    public class TaskAssignmentService
    {
        private static string _connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly IConfiguration _config;
        public TaskAssignmentService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<TaskAssignment>> GetAllTaskAssigments(string userId)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);

                string query = "SELECT * FROM TASK_ASSIGNMENTS WHERE 1=1 ";
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(userId))
                {
                    query += " AND USER_ID = @userId OR GROUP_ID IN (" +
                                "SELECT GROUP_ID " +
                                "FROM USER_GROUP_MEMBER " +
                                "WHERE USER_ID = @userId);";
                    parameters.Add("userId", Convert.ToInt32(userId));
                }

                var jobs = await connection.QueryAsync<TaskAssignment>(query, parameters);
                return jobs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddTask(TaskAssignment task)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "INSERT INTO TASK_ASSIGNMENTS(job_name,trigger_name,USER_ID,GROUP_ID) " +
                    "VALUES(@JobName, @TriggerName, @USER_ID, @GROUP_ID)";
                var parameters = new DynamicParameters();
                parameters.Add("JobName", task.JOB_NAME);
                parameters.Add("TriggerName", task.TRIGGER_NAME);
                parameters.Add("USER_ID", task.USER_ID == 0 ? null : task.USER_ID);
                parameters.Add("GROUP_ID", task.GROUP_ID == 0 ? null : task.GROUP_ID);
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateTask(string oldJobName, string oldTriggerName, TaskAssignment task)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "UPDATE TASK_ASSIGNMENTS " +
                                "SET USER_ID = @USER_ID, " +
                                "GROUP_ID = @GROUP_ID," +
                                "job_name = @JobName," +
                                "trigger_name = @TriggerName " +
                                "WHERE job_name = @oldJobName and trigger_name = @oldTriggerName";
                var parameters = new DynamicParameters();
                parameters.Add("JobName", task.JOB_NAME);
                parameters.Add("TriggerName", task.TRIGGER_NAME);
                parameters.Add("USER_ID", task.USER_ID == 0 ? null : task.USER_ID);
                parameters.Add("GROUP_ID", task.GROUP_ID == 0 ? null : task.GROUP_ID);
                parameters.Add("oldJobName", oldJobName);
                parameters.Add("oldTriggerName", oldTriggerName);

                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TaskAssignment> GetTaskByJobNameAndTriggerName(string jobName, string triggerName)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "SELECT * from TASK_ASSIGNMENTS " +
                                "WHERE job_name = @JobName " +
                                "and trigger_name = @TriggerName";
                var parameters = new DynamicParameters();
                parameters.Add("JobName", jobName);
                parameters.Add("TriggerName", triggerName);

                // Sử dụng QueryFirstOrDefaultAsync thay vì QueryAsync
                var taskAssignment = await connection.QueryFirstOrDefaultAsync<TaskAssignment>(query, parameters);
                return taskAssignment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteTask(string jobName, string triggerName)
        {
            try
            {
                _connectionString = _config["ConnectionStrings:BlazingQuartzDb"];
                using var connection = new SqliteConnection(_connectionString);
                string query = "DELETE FROM TASK_ASSIGNMENTS " +
                                "WHERE job_name = @JobName and trigger_name = @TriggerName";
                var parameters = new DynamicParameters();
                parameters.Add("JobName", jobName);
                parameters.Add("TriggerName", triggerName);
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
