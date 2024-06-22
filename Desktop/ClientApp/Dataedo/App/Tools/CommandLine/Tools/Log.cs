using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Log.Execution;

namespace Dataedo.App.Tools.CommandLine.Tools;

public class Log
{
	private const int AttachParentProcess = -1;

	private bool isConsoleAttached;

	private bool isVerboseLogging;

	private bool useBuffer;

	private int bufferSize = 100;

	private List<string> buffer;

	public string FilePath { get; set; }

	public bool IsLogEnabled => true;

	public bool IsFileLogEnabled => !string.IsNullOrEmpty(FilePath);

	public bool IsVerboseLogging
	{
		get
		{
			return isVerboseLogging;
		}
		set
		{
			isVerboseLogging = value;
			useBuffer = value;
		}
	}

	[DllImport("kernel32.dll")]
	private static extern int AttachConsole(int dwProcessId);

	[DllImport("kernel32.dll")]
	private static extern int FreeConsole();

	public Log()
	{
		try
		{
			FreeConsoleFromProcess();
			AttachConsoleToProcess();
		}
		catch (Exception)
		{
		}
		buffer = new List<string>((int)((double)bufferSize * 1.5));
	}

	public Log(string filePath)
		: this()
	{
		FilePath = filePath;
	}

	public void AttachConsoleToProcess()
	{
		try
		{
			FreeConsoleFromProcess();
			if (!isConsoleAttached)
			{
				isConsoleAttached = AttachConsole(-1) == 0;
			}
		}
		catch (Exception)
		{
		}
	}

	public void FreeConsoleFromProcess()
	{
		try
		{
			if (isConsoleAttached)
			{
				isConsoleAttached = FreeConsole() != 0;
			}
		}
		catch (Exception)
		{
		}
	}

	public void EnvironmentExit(int exitCode)
	{
		FreeConsoleFromProcess();
		Environment.Exit(exitCode);
	}

	public void Write(params string[] lines)
	{
		if (lines != null && lines.Length != 0)
		{
			lines[0] = FormatLine(lines[0]);
			WritePrivate(lines);
		}
	}

	public void Write(string line, int padTabsCount = 0)
	{
		if (IsLogEnabled)
		{
			WritePrivate(FormatLine(line, padTabsCount));
		}
	}

	public void Write(params HandlingResult[] handlingResults)
	{
		if (handlingResults != null && handlingResults.Length != 0)
		{
			Write((from x in handlingResults
				where x != null
				select x.ToString(IsVerboseLogging))?.ToArray());
		}
		FlushBuffer();
	}

	public void Write(IEnumerable<HandlingResult> handlingResults)
	{
		Write(handlingResults?.ToArray());
	}

	public void WriteSimple()
	{
		if (IsLogEnabled)
		{
			WritePrivate((string)null, consoleOnly: false, fileOnly: false);
		}
	}

	public void WriteSimple(params string[] lines)
	{
		if (IsLogEnabled)
		{
			WritePrivate(lines);
		}
	}

	public void FlushBuffer()
	{
		if (IsFileLogEnabled)
		{
			try
			{
				File.AppendAllLines(FilePath, buffer.ToArray());
				buffer.Clear();
			}
			catch (Exception)
			{
			}
		}
	}

	public void WriteToConsole(string[] lines, bool consoleOnly = true)
	{
		WritePrivate(lines, consoleOnly);
	}

	public void WriteToConsole(string line, int padTabsCount = 0)
	{
		WritePrivate(FormatLine(line, padTabsCount), consoleOnly: true);
	}

	public void WriteToConsoleSimple()
	{
		WritePrivate((string)null, consoleOnly: true, fileOnly: false);
	}

	public void WriteToConsoleSimple(params string[] lines)
	{
		WritePrivate(lines, consoleOnly: true);
	}

	public void WriteToFile(string[] lines, bool fileOnly = true)
	{
		WritePrivate(lines, consoleOnly: false, fileOnly);
	}

	public void WriteToFile(string line, int padTabsCount = 0)
	{
		WritePrivate(FormatLine(line, padTabsCount), consoleOnly: false, fileOnly: true);
	}

	public void WriteToFileSimple()
	{
		WritePrivate((string)null, consoleOnly: false, fileOnly: true);
	}

	public void WriteToFileSimple(string line)
	{
		WritePrivate(line, consoleOnly: false, fileOnly: true);
	}

	private void WritePrivate(string[] lines, bool consoleOnly = false, bool fileOnly = false)
	{
		string[] additionalData;
		if (ExecutionLog.IsLogEnabled)
		{
			Stopwatch stopwatch = new Stopwatch();
			additionalData = lines;
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP CMD", null, null, 2, 1, additionalData);
		}
		if (!consoleOnly && IsFileLogEnabled)
		{
			if (useBuffer)
			{
				PutInBuffer(lines);
			}
			else
			{
				try
				{
					File.AppendAllLines(FilePath, lines);
				}
				catch (Exception ex)
				{
					WriteToConsole("Error occurred while writing to file log (path: \"" + FilePath + "\").");
					WriteToConsole("Details:");
					WriteToConsole(ex.ToString());
				}
			}
		}
		if (fileOnly)
		{
			return;
		}
		additionalData = lines;
		foreach (string value in additionalData)
		{
			try
			{
				if (isConsoleAttached)
				{
					Console.WriteLine(value);
				}
			}
			catch (Exception)
			{
				AttachConsoleToProcess();
				if (isConsoleAttached)
				{
					Console.WriteLine(value);
				}
			}
		}
	}

	private void WritePrivate(string line, bool consoleOnly = false, bool fileOnly = false)
	{
		string[] lines = new string[1] { line };
		WritePrivate(lines, consoleOnly, fileOnly);
	}

	private void PutInBuffer(string[] lines)
	{
		buffer.AddRange(lines);
		if (buffer.Count >= bufferSize)
		{
			FlushBuffer();
		}
	}

	private string FormatLine(string line, int padTabsCount = 0)
	{
		return DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + new string('\t', padTabsCount) + " " + line;
	}
}
