using System;
using System.ComponentModel;

namespace Dataedo.App.Helpers.Extensions;

public static class BackgroundWorkerExtensions
{
	public static void ThrowIfCancellationPending(this BackgroundWorker worker)
	{
		if (worker != null && worker.CancellationPending)
		{
			throw new OperationCanceledException();
		}
	}
}
