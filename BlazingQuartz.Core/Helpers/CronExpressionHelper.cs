using System;
using Quartz;

namespace BlazeQuartz.Core.Helpers
{
    public static class CronExpressionHelper
    {
        public static bool IsValidExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }
    }
}

