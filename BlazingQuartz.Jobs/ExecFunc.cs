using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeQuartz.Jobs
{
    public static class ExecFunc
    {
        //public static string RunExeAndGetOutput(AgrsObj agrsObj)
        //{
        //    try
        //    {
        //        ProcessStartInfo psi = new ProcessStartInfo
        //        {
        //            FileName = agrsObj.ExePath,
        //            Arguments = agrsObj.Agrs,
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };


        //        using Process process = Process.Start(psi);
        //        process.WaitForExit(TimeSpan.FromMinutes(30));
        //        //string output = process.StandardOutput.ReadToEnd();
        //        //string error = process.StandardError.ReadToEnd();

        //        //return string.IsNullOrEmpty(error) ? output : error; // Return output or error
        //        return "Ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error running exe: {ex.Message}";
        //    }
        //}
        public static async Task<string> RunExeAndGetOutput(AgrsObj agrsObj)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = agrsObj.ExePath,
                    Arguments = agrsObj.Agrs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process process = new Process { StartInfo = psi };
                process.Start();

                // Đọc output & error song song
                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();

                // Giới hạn thời gian chạy tối đa 30 phút
                if (await Task.WhenAny(Task.Delay(TimeSpan.FromMinutes(30)), process.WaitForExitAsync()) == Task.Delay(TimeSpan.FromMinutes(30)))
                {
                    process.Kill(); // Dừng process nếu quá thời gian
                    return "Error: Process timeout!";
                }

                string output = await outputTask;
                string error = await errorTask;

                return string.IsNullOrEmpty(error) ? output : error;
            }
            catch (Exception ex)
            {
                return $"Error running exe: {ex.Message}";
            }
        }

    }
}
