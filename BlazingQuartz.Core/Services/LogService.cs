using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;


namespace BlazeQuartz.Core.Services
{
    public class LogService
    {
        private static string _connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly IConfiguration _config;
        public LogService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<(IEnumerable<LogEntry> Items, int TotalCount)> GetFilteredLogsAsync(int page, int pageSize, string searchTerm, LogLevel level, int _cusid, string _Log_Module, string _log_type)
        {

            string dbPath = _config["AppSettings:dbPath"];
            _connectionString = $"Data Source={dbPath}";
            using var connection = new SqliteConnection(_connectionString);
            int offset = page * pageSize;

            string query = "SELECT * FROM Logs WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND Log_Message LIKE @SearchTerm";
                parameters.Add("SearchTerm", $"%{searchTerm}%");
            }

            if (level != LogLevel.All)
            {
                query += " AND Level = @Level";
                parameters.Add("Level", $"{level}");
            }
            if (_cusid != -1)
            {
                query += " AND cusid = @Cusid";
                parameters.Add("Cusid", $"{_cusid}");
            }
            if (_log_type != "Tất cả")
            {
                query += " AND log_module_type = @log_module_type";
                parameters.Add("log_module_type", $"{_log_type}");
            }

            if (_Log_Module != "-1")
            {
                query += " AND Log_Module = @Log_Module";
                parameters.Add("Log_Module", $"{_Log_Module}");
            }

            query += " ORDER BY ID desc LIMIT @PageSize OFFSET @Offset";
            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", offset);

             var logs = await connection.QueryAsync<LogEntry>(query, parameters);
            int totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Logs WHERE 1=1", parameters);

            return (logs, totalCount);
        }
    }
    // Định nghĩa model LogEntry
    public class LogEntry
    {
        public int Id { get; set; }
        public string Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Log_Message { get; set; }
        public string Log_Module { get; set; }
        public string Log_Module_Type { get; set; }
        public int Cusid { get; set; }
    }

    public enum LogLevel { All, Information, Warning, Error }
    public enum ModType { All, Imp, X12 }
}
