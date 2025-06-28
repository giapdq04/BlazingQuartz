using Microsoft.Extensions.Logging;

using Quartz;

using Serilog;

using sLog = Serilog.Log;


namespace BlazeQuartz.Jobs
{
    [DisallowConcurrentExecution]
    public class ExecProgram : IJob
    {
        private readonly ILogger<ExecProgram> _logger;
        public ExecProgram(ILogger<ExecProgram> logger)
        {
            _logger = logger;
            Serilog.Log.Logger = new LoggerConfiguration()
                                      .WriteTo.Sink(new SQLiteSink())  // Custom SQLite Sink
                                      .CreateLogger();
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(30)))  
            {
                try
                {
                    JobDataMap dataMap = context.JobDetail.JobDataMap;
                    string? _conn_type = dataMap.GetString("CONN");
                    string? _exePath = dataMap.GetString("EXE_PATH");
                    string? _AgrsObj = dataMap.GetString("AGRSOBJ");

                    AgrsObj agrsObj = new AgrsObj
                    {
                        ExePath = _exePath,
                        ConnType = _conn_type,
                        Agrs = _AgrsObj
                    };

                    string result = await Task.Run(() => ExecFunc.RunExeAndGetOutput(agrsObj), cts.Token);

                    context.JobDetail.JobDataMap.Put("ExeResult", result);
                    Console.WriteLine($"Executable Output: {result}");
                    sLog.Information($"Executable Output: {result}");
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine("Job timed out!");
                    context.JobDetail.JobDataMap.Put("ExeResult", "Error: Job timed out!");
                    sLog.Error($"Job timed out! {ex}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing job: {ex.Message}");
                    context.JobDetail.JobDataMap.Put("ExeResult", $"Error: {ex.Message}");
                    sLog.Error($"Error executing job: {ex.Message}");
                }
            }
        }


        //public Task Execute(IJobExecutionContext context)
        //{
        //    JobDataMap dataMap = context.JobDetail.JobDataMap;
        //    string? _conn_type = dataMap.GetString("CONN");
        //    string? _exePath = dataMap.GetString("EXE_PATH"); 
        //    string? _AgrsObj = dataMap.GetString("AGRSOBJ");
        //    //string[] Agrs = _AgrsObj.Split(";");
        //    //string arg = string.Empty;
        //    //foreach (var item in Agrs)
        //    //{
        //    //    arg += item + " ";
        //    //}

        //    AgrsObj agrsObj = new AgrsObj
        //    {
        //        ExePath = _exePath,
        //        ConnType = _conn_type,
        //        Agrs = _AgrsObj
        //    };

        //    string result = ExecFunc.RunExeAndGetOutput(agrsObj);

        //    // Store result in Quartz JobDataMap
        //    context.JobDetail.JobDataMap.Put("ExeResult", result);

        //    Console.WriteLine($"Executable Output: {result}");

        //    return Task.CompletedTask;
        //}
    }
}
