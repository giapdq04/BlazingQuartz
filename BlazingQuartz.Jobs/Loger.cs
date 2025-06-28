using Microsoft.Data.Sqlite;
using Dapper;
using Serilog;
using Microsoft.Extensions.Logging;

namespace BlazeQuartz.Jobs
{ 
    // Custom SQLite Sink
    public class SQLiteSink : Serilog.Core.ILogEventSink
    {
        string connectionString = "Data Source=BlazingQuartzDb.db";
        private readonly string _connectionString;

        public SQLiteSink()
        {
            _connectionString = connectionString;
        }

        public void Emit(Serilog.Events.LogEvent logEvent)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                connection.Execute("INSERT INTO Logs (Level, Message) VALUES (@Level, @Message)", new
                {
                    Level = logEvent.Level.ToString(),
                    Message = logEvent.RenderMessage()
                });
            }
        }
    }
}
