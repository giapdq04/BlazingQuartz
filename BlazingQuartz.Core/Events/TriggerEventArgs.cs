﻿using System;
using Quartz;

namespace BlazeQuartz.Core.Events
{
	public class TriggerEventArgs : EventArgs
	{
		public ITrigger Trigger { get; init; }
		public IJobExecutionContext JobExecutionContext { get; init; }
		public CancellationToken CancelToken { get; init; }

		public TriggerEventArgs(ITrigger trigger, IJobExecutionContext context, CancellationToken cancelToken = default(CancellationToken))
		{
			Trigger = trigger;
			JobExecutionContext = context;
			CancelToken = cancelToken;
		}
	}
}

