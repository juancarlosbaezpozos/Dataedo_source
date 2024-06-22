using System;

namespace Dataedo.Api.Configuration;

public class BackgroundProcessing
{
	public static readonly BackgroundProcessing Instance = new BackgroundProcessing();

	private bool Active;

	private BackgroundProcessing()
	{
		Active = false;
	}

	public bool IsProcessing()
	{
		lock (Instance)
		{
			return Active;
		}
	}

	public void Start()
	{
		lock (Instance)
		{
			if (Active)
			{
				throw new Exception("Cannot start background processing, because other job is active.");
			}
			Active = true;
		}
	}

	public void Stop()
	{
		lock (Instance)
		{
			if (!Active)
			{
				throw new Exception("Cannot stop background processing, because any job isn't active.");
			}
			Active = false;
		}
	}
}
