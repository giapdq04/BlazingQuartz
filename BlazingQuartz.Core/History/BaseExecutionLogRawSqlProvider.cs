﻿using System;
namespace BlazeQuartz.Core.History
{
    public class BaseExecutionLogRawSqlProvider : IExecutionLogRawSqlProvider
    {
        public virtual string DeleteLogsByDays { get; } =
            @"DELETE FROM bqz_execution_logs
WHERE date_added_utc < {0}";
    }
}

