using System.Windows.Forms;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class SSISPackageFileHelper
{
	public string FilePath { get; private set; } = string.Empty;


	private string GetFilePath(SSISConnectionTypeEnum.SSISConnectionType connectionType)
	{
		string text = "dtsx";
		if (connectionType == SSISConnectionTypeEnum.SSISConnectionType.ISPAC)
		{
			text = "ispac";
		}
		using (OpenFileDialog openFileDialog = new OpenFileDialog())
		{
			openFileDialog.Filter = text + " files (*." + text + ")|*." + text + "|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				FilePath = openFileDialog.FileName;
			}
		}
		return FilePath;
	}

	public string GetFilePathWithPrefix(SSISConnectionTypeEnum.SSISConnectionType connectionType)
	{
		string filePath = GetFilePath(connectionType);
		if (string.IsNullOrEmpty(filePath))
		{
			return string.Empty;
		}
		return filePath ?? "";
	}
}
