using System;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;

namespace Dataedo.App.LoginFormTools.Tools.FileRepository;

internal class OpenFileSupport : FileLoginSupport, IDisposable
{
	private readonly OpenFileDialog openFileDialog;

	public OpenFileSupport(Form parentWindow)
		: base(parentWindow)
	{
		openFileDialog = new OpenFileDialog();
		openFileDialog.DefaultExt = "dataedo";
		openFileDialog.Filter = "Dataedo project files|*.dataedo";
	}

	public OpenFileSupport(Form parentWindow, string path)
		: this(parentWindow)
	{
		if (path == null)
		{
			return;
		}
		try
		{
			openFileDialog.InitialDirectory = Path.GetDirectoryName(path);
			openFileDialog.FileName = Path.GetFileName(path);
			if (Path.GetExtension(path) != ".dataedo")
			{
				openFileDialog.Filter += "|All files|*.*";
			}
		}
		catch
		{
			openFileDialog.FileName = path;
		}
	}

	public void Dispose()
	{
		openFileDialog.Dispose();
	}

	public ConnectionResult OpenFile()
	{
		openFileDialog.InitialDirectory = LastConnectionInfo.LOGIN_INFO.FileRepositoryPath;
		DialogResult num = openFileDialog.ShowDialog(ParentWindow);
		StaticData.SetRepositoryData(RepositoriesDBHelper.GetRepositoryData(openFileDialog.FileName, isProjectFile: true, FileLoginSupport.GetConnectionString(openFileDialog.FileName)));
		if (num == DialogResult.OK)
		{
			return OpenProject(openFileDialog.FileName);
		}
		return ConnectionResult.Empty;
	}

	public ConnectionResult OpenProject(string filePath)
	{
		if (!File.Exists(filePath))
		{
			GeneralMessageBoxesHandling.Show("Check file path and try again", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, ParentWindow);
			return new ConnectionResult(null, false);
		}
		return TryLoginByPath(filePath);
	}
}
