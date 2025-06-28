namespace BlazeQuartz.Core.History
{
    public interface IExecutionLogRawSqlProvider
    {
        string DeleteLogsByDays { get; }
    }
}