﻿using System;
using BlazeQuartz.Core.Data.Entities;

namespace BlazeQuartz.Core.History
{
    public interface IExecutionLogStore
    {
        bool Exists(ExecutionLog log);
        Task<int> DeleteLogsByDays(int daysToKeep, CancellationToken cancelToken = default);
        Task AddExecutionLog(ExecutionLog log, CancellationToken cancelToken = default);
        ValueTask UpdateExecutionLog(ExecutionLog log);
        Task SaveChangesAsync(CancellationToken cancelToken = default);
        Task MarkExecutingJobAsIncomplete(CancellationToken cancellToken = default);
    }
}

