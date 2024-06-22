using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.Tools.MessageBoxes;

namespace Dataedo.App.Tools;

public static class Paths
{
	public static readonly string DataedoExtension;

	private static readonly string[] reservedNames;

	private static readonly List<char> invalidCharactersForEncoding;

	static Paths()
	{
		DataedoExtension = ".dataedo";
		reservedNames = new string[22]
		{
			"CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6",
			"COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7",
			"LPT8", "LPT9"
		};
		invalidCharactersForEncoding = new List<char>(Path.GetInvalidFileNameChars());
		invalidCharactersForEncoding.Add('[');
		invalidCharactersForEncoding.Add(']');
		invalidCharactersForEncoding.Add(' ');
	}

	public static bool IsNameReserved(string name)
	{
		return reservedNames.Contains(name.ToUpper());
	}

	public static string GetAdminConsolePath()
	{
		try
		{
			return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "DataedoAdministrationConsole.exe");
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static string GetUserTemplatesPath(DocFormatEnum.DocFormat? format)
	{
		try
		{
			string myDocumentsPath = LastConnectionInfo.MyDocumentsPath;
			string userTemplatesSubcatalogPath = GetUserTemplatesSubcatalogPath(format);
			if (string.IsNullOrWhiteSpace(myDocumentsPath) || string.IsNullOrWhiteSpace(userTemplatesSubcatalogPath))
			{
				return string.Empty;
			}
			string text = Path.Combine(myDocumentsPath, "Dataedo", "Templates", userTemplatesSubcatalogPath);
			if (!Directory.Exists(text) && !CreateCatalog(text))
			{
				return GetDefaultDocumentationAndFileRepositoryPath();
			}
			return text;
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	private static string GetUserTemplatesSubcatalogPath(DocFormatEnum.DocFormat? format)
	{
		return format switch
		{
			DocFormatEnum.DocFormat.PDF => "PDF", 
			DocFormatEnum.DocFormat.HTML => "HTML", 
			DocFormatEnum.DocFormat.Excel => "Excel", 
			_ => null, 
		};
	}

	public static string GetDefaultDocumentationAndFileRepositoryPath()
	{
		try
		{
			string myDocumentsPath = LastConnectionInfo.MyDocumentsPath;
			if (string.IsNullOrWhiteSpace(myDocumentsPath))
			{
				return string.Empty;
			}
			return Path.Combine(myDocumentsPath, "Dataedo");
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static bool CreateCatalog(string path)
	{
		try
		{
			return Directory.CreateDirectory(path) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string RemoveInvalidConnCharacters(string element)
	{
		return element.Replace("&", "&amp").Replace(">", "&gt").Replace("<", "&lt")
			.Replace("'", "&apos")
			.Replace("\"", "&quot");
	}

	public static void CheckAndSaveDefaultDocumentationPath()
	{
		try
		{
			if (string.IsNullOrEmpty(LastConnectionInfo.LOGIN_INFO?.DocumentationPath) || !Directory.Exists(LastConnectionInfo.LOGIN_INFO?.DocumentationPath))
			{
				if (Directory.Exists(GetDefaultDocumentationAndFileRepositoryPath()))
				{
					UpdateDocumentationSaveDirectoryPath(GetDefaultDocumentationAndFileRepositoryPath());
				}
				else if (CreateCatalog(GetDefaultDocumentationAndFileRepositoryPath()))
				{
					UpdateDocumentationSaveDirectoryPath(GetDefaultDocumentationAndFileRepositoryPath());
				}
				else
				{
					UpdateDocumentationSaveDirectoryPath(LastConnectionInfo.MyDocumentsPath);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static void UpdateDocumentationSaveDirectoryPath(string path)
	{
		LastConnectionInfo.SetDocumentationPath(path);
	}

	public static void CheckAndSaveDefaultFileRepositoryPath()
	{
		try
		{
			if (string.IsNullOrEmpty(LastConnectionInfo.LOGIN_INFO.FileRepositoryPath) || !Directory.Exists(LastConnectionInfo.LOGIN_INFO.FileRepositoryPath))
			{
				if (Directory.Exists(GetDefaultDocumentationAndFileRepositoryPath()))
				{
					UpdateFileRepositoryDirectoryPath(GetDefaultDocumentationAndFileRepositoryPath());
				}
				else if (CreateCatalog(GetDefaultDocumentationAndFileRepositoryPath()))
				{
					UpdateFileRepositoryDirectoryPath(GetDefaultDocumentationAndFileRepositoryPath());
				}
				else
				{
					UpdateFileRepositoryDirectoryPath(LastConnectionInfo.MyDocumentsPath);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static void UpdateFileRepositoryDirectoryPath(string path)
	{
		LastConnectionInfo.SetFileRepositoryPath(path);
	}

	public static string EncodeInvalidPathCharacters(string filename)
	{
		new StringBuilder();
		if (!filename.Any((char c) => invalidCharactersForEncoding.Contains(c)))
		{
			return filename;
		}
		return string.Join("", filename.Select(delegate(char c)
		{
			if (invalidCharactersForEncoding.Contains(c))
			{
				int num = c;
				return "[" + num + "]";
			}
			return c.ToString();
		}));
	}

	public static string RemoveInvalidFilePathCharacters(string filename, string replaceChar, bool removeInvalidFileNameCharsOnly = false)
	{
		string text = new string(Path.GetInvalidFileNameChars());
		if (!removeInvalidFileNameCharsOnly)
		{
			text += new string(Path.GetInvalidPathChars());
		}
		if (text != null && filename != null)
		{
			return new Regex("[" + Regex.Escape(text) + "]").Replace(filename, replaceChar);
		}
		return filename;
	}

	public static IEnumerable<char> GetInvalidCharactersFromPath(string path)
	{
		List<char> list = new List<char>();
		list.AddRange(from x in Path.GetInvalidPathChars()
			where path.Contains(x)
			select x);
		list.AddRange(from x in Path.GetInvalidFileNameChars()
			where path.Contains(x)
			select x);
		return list.Distinct();
	}

	public static bool IsFilePathCorrect(string catalogPath, string fileName, Form owner = null)
	{
		if (!Directory.Exists(catalogPath))
		{
			GeneralMessageBoxesHandling.Show("Check file location and try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
			return false;
		}
		if (string.IsNullOrWhiteSpace(fileName) || !IsValidName(fileName, catalogPath, showMessage: false, owner))
		{
			GeneralMessageBoxesHandling.Show("Check file location or file name and try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
			return false;
		}
		return true;
	}

	public static bool IsValidName(string fileName, string path, bool showMessage = true, Form owner = null)
	{
		bool flag = true;
		string str = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
		if (new Regex("[" + Regex.Escape(str) + "]").IsMatch(fileName))
		{
			if (showMessage)
			{
				GeneralMessageBoxesHandling.Show("The file name is not valid.", "Invalid file name", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			}
			return false;
		}
		flag = false;
		string message = string.Empty;
		try
		{
			new FileInfo(path);
			flag = true;
		}
		catch (ArgumentException ex)
		{
			message = ex.Message;
		}
		catch (PathTooLongException ex2)
		{
			message = ex2.Message;
		}
		catch (NotSupportedException ex3)
		{
			message = ex3.Message;
		}
		if (!flag)
		{
			if (showMessage)
			{
				GeneralMessageBoxesHandling.Show(message, "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			}
			return false;
		}
		return flag;
	}

	public static void OpenSavedFile(string path, Form owner = null)
	{
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Open saved file?", "Open", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, 1, owner);
		if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes || !File.Exists(path))
		{
			return;
		}
		try
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = path
			});
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to open file.", "Open", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
		}
	}

	public static void OpenExplorerAndSelectFolder(string path)
	{
		if (Directory.Exists(path))
		{
			string arguments = "/select, \"" + path + "\"";
			Process.Start("explorer.exe", arguments);
		}
	}

	public static string GetNextName(string path, string baseName, string extension, bool forFile = true, bool addExtenstion = false)
	{
		int i = 1;
		if (!forFile && !Directory.Exists(path))
		{
			return $"{baseName}{i}";
		}
		for (; CheckIfExists(Path.Combine(path, $"{baseName}{i}{extension}"), forFile); i++)
		{
		}
		return $"{baseName}{i}{(addExtenstion ? extension : null)}";
	}

	private static bool CheckIfExists(string path, bool forFile)
	{
		if (forFile)
		{
			return File.Exists(path);
		}
		return Directory.Exists(path);
	}
}
