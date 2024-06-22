using System.Windows.Forms;

namespace Dataedo.App.Tools;

public class SSLFileHelper
{
	private string GetFilePath(string extension = "pem")
	{
		string empty = string.Empty;
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = extension + " files (*." + extension + ")|*." + extension + "|All files (*.*)|*.*";
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			return openFileDialog.FileName;
		}
		return empty;
	}

	public string GetFilePathWithPrefix(string extension = "pem")
	{
		string filePath = GetFilePath(extension);
		if (string.IsNullOrEmpty(filePath))
		{
			return string.Empty;
		}
		return filePath ?? "";
	}
}
