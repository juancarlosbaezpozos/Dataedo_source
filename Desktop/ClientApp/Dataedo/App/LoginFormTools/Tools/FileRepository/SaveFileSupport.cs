using System;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Sql.SqlServerCe.ProjectCommands;

namespace Dataedo.App.LoginFormTools.Tools.FileRepository;

internal class SaveFileSupport : FileLoginSupport, IDisposable
{
	private readonly SaveFileDialog saveFileDialog;

	public SaveFileSupport(Form parentWindow)
		: base(parentWindow)
	{
		saveFileDialog = new SaveFileDialog();
		saveFileDialog.DefaultExt = "dataedo";
		saveFileDialog.Filter = "Dataedo project files | *.dataedo";
	}

	public void Dispose()
	{
		saveFileDialog.Dispose();
	}

	public bool SaveFile()
	{
		saveFileDialog.InitialDirectory = LastConnectionInfo.LOGIN_INFO.FileRepositoryPath;
		saveFileDialog.FileName = Paths.GetNextName(LastConnectionInfo.LOGIN_INFO.FileRepositoryPath, "Documentation", Paths.DataedoExtension);
		if (saveFileDialog.ShowDialog(ParentWindow) == DialogResult.OK)
		{
			return CreateNewProject(saveFileDialog.FileName);
		}
		return false;
	}

	private bool CreateNewProject(string path)
	{
		try
		{
			Project.SaveSampleProjectFile(path);
			if (string.IsNullOrWhiteSpace(path))
			{
				return false;
			}
			Paths.UpdateFileRepositoryDirectoryPath(Path.GetDirectoryName(path));
			return TryLoginByPath(path).IsSuccess;
		}
		catch (FileNotFoundException ex)
		{
			if (ex.Data?["CUSTOM_EXCEPTION"] as bool? == true)
			{
				GeneralMessageBoxesHandling.Show(ex.Message + Environment.NewLine + Environment.NewLine + "Please try reinstalling Dataedo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, ParentWindow);
			}
			else
			{
				GeneralExceptionHandling.Handle(ex, ParentWindow);
			}
		}
		catch (Exception exception)
		{
			GeneralFileExceptionHandling.Handle(exception, "Error while creating file repository.", ParentWindow);
		}
		return false;
	}
}
