using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Tools;

namespace Dataedo.App.Forms.Support.DocWizardForm;

public class ExportFile
{
	public string Directory { get; set; }

	public string FileName { get; set; }

	public string Extension { get; set; }

	public bool Correct { get; set; }

	public string Title { get; set; }

	public string FullPath => Path.Combine(Directory, FileName + Extension);

	public ExportFile(int? databaseId, DocFormatEnum.DocFormat format = DocFormatEnum.DocFormat.PDF)
	{
		Directory = LastConnectionInfo.LOGIN_INFO.DocumentationPath;
		Extension = ExportDocTypes.GetExportTypes().FirstOrDefault((ExportDocTypesElem x) => x.Format == format)?.Extension;
		if (databaseId.HasValue)
		{
			Title = DB.Database.GetTitle(databaseId.Value);
			FileName = Paths.RemoveInvalidFilePathCharacters(Title, "_");
		}
	}

	public ExportFile(SaveFileDialog saveFileDialog)
	{
		Directory = saveFileDialog.FileName.Replace(Path.GetFileName(saveFileDialog.FileName), string.Empty);
		FileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
		Extension = Path.GetExtension(saveFileDialog.FileName);
		Correct = true;
	}

	public ExportFile(string directory, string fileName, string extension)
	{
		Directory = directory;
		FileName = fileName;
		Extension = extension;
	}
}
