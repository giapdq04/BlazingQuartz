using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

using Quartz;
using Dapper;
using Serilog;

using System.Diagnostics;

namespace BlazeQuartz.Jobs
{
    
    public class Xml2Csv : IJob
    { 
        public Xml2Csv( )
        {
            Log.Logger = new LoggerConfiguration()
                   .WriteTo.Sink(new SQLiteSink())  // Custom SQLite Sink
                   .CreateLogger();
        }

        public Task Execute(IJobExecutionContext context)
        { 
            string exePath = @"C:\Users\Tai\source\repos\WindowsFormsApp1\WindowsFormsApp1\bin\Debug\WindowsFormsApp1.exe"; // Change to your exe path
             
            string result = RunExeAndGetOutput(exePath);

            // Store result in Quartz JobDataMap
            context.JobDetail.JobDataMap.Put("ExeResult", result);

            Log.Information(result);

            Console.WriteLine($"Executable Output: {result}");

            return Task.CompletedTask;
        }
        private string RunExeAndGetOutput(string exePath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath, 
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };


                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    return string.IsNullOrEmpty(error) ? output : error; // Return output or error
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return $"Error running exe: {ex.Message}";
            }
        }
    }
}
