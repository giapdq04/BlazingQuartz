using Microsoft.Extensions.Logging;

using Quartz;

using System.Diagnostics;

namespace BlazeQuartz.Jobs
{
    public class XmlHandle : IJob
    {
        private readonly ILogger<XmlHandle> _logger;
        public XmlHandle(ILogger<XmlHandle> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap; 
            string? _exePath = dataMap.GetString("EXE_PATH");
            int? _cusid = dataMap.GetInt("CUSID"); 
            string? _cus_name = dataMap.GetString("CUS_NAME");
            AgrsObj agrsObj = new AgrsObj
            {
                Cusid = (int)_cusid, 
                CusName = _cus_name,
                ExePath = _exePath
            };

            //string exePath = @"C:\Path\To\YourApp.exe"; // Change to your exe path
            _logger.LogInformation($"Path: {_exePath}");
            _logger.LogInformation($"Cusid: {agrsObj.Cusid}");  
            _logger.LogInformation($"CusName: {agrsObj.CusName}");
            string result = RunExeAndGetOutput(agrsObj);

            // Store result in Quartz JobDataMap
            context.JobDetail.JobDataMap.Put("ExeResult", result);

            Console.WriteLine($"Executable Output: {result}");

            return Task.CompletedTask;
        }
        private string RunExeAndGetOutput(AgrsObj agrsObj)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = agrsObj.ExePath,
                    Arguments = string.Format($"{agrsObj.CusName} {agrsObj.Cusid}"),
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
                return $"Error running exe: {ex.Message}";
            }
        }
    }
}
