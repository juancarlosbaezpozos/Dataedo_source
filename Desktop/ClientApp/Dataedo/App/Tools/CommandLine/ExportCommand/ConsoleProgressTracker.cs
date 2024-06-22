using System;
using Dataedo.App.Tools.Export.Universal;

namespace Dataedo.App.Tools.CommandLine.ExportCommand;

internal class ConsoleProgressTracker : IProgressTracker
{
	private int LastProgress = -1;

	public void Done()
	{
		try
		{
			OnProgress(100);
			Console.WriteLine();
		}
		catch (Exception)
		{
		}
	}

	public void Log(string log)
	{
	}

	public void OnProgress(int percentage)
	{
		try
		{
			if (LastProgress != percentage)
			{
				DrawProgressBar(percentage);
				LastProgress = percentage;
			}
		}
		catch (Exception)
		{
		}
	}

	public void OnSubProgress(int percentage)
	{
	}

	public bool ShouldCancel()
	{
		try
		{
			return Console.KeyAvailable;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void Started()
	{
		OnProgress(0);
	}

	public void SubLog(string log)
	{
	}

	public void ThrowIfCanceled()
	{
		if (ShouldCancel())
		{
			throw new OperationCanceledException();
		}
	}

	public static void DrawProgressBar(int progress)
	{
		int num = 30;
		Console.CursorLeft = 0;
		Console.Write("[");
		Console.CursorLeft = num + 2;
		Console.Write("]");
		Console.CursorLeft = 1;
		float num2 = (float)num / 100f;
		int num3 = 1;
		for (int i = 0; (float)i < num2 * (float)progress; i++)
		{
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.CursorLeft = num3++;
			Console.Write(" ");
		}
		for (int j = num3; j <= num + 1; j++)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.CursorLeft = num3++;
			Console.Write(" ");
		}
		Console.CursorLeft = num + 3;
		Console.BackgroundColor = ConsoleColor.Black;
		Console.Write(" " + progress + "% ");
	}
}
