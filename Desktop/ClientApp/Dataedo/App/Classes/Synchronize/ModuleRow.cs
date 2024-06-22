using System.Linq;
using Dataedo.App.Classes.Documentation;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ModuleRow : DatabaseRowBase
{
	public bool IsShown { get; set; }

	public bool ErdShowTypes { get; set; }

	public DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode DisplayDocumentationNameMode { get; set; }

	public override string ObjectId => Paths.RemoveInvalidFilePathCharacters(new string(base.Title.Replace(" ", "_").Take(40).ToArray()) + "_" + base.IdValue, "_");

	public override string Name
	{
		get
		{
			return base.Title;
		}
		set
		{
			Name = value;
		}
	}

	public int DatabaseId { get; set; }

	public bool ErdShowNullable { get; set; }

	public ModuleRow(ModuleObject row)
	{
		base.Id = row.ModuleId;
		base.Title = row.Title;
		IsShown = true;
		ErdShowTypes = row.ErdShowTypes.GetValueOrDefault();
		DisplayDocumentationNameMode = DisplayDocumentationNameModeEnum.ObjectToType(row.DisplayDocumentationNameMode);
		DatabaseId = row.DatabaseId;
		ErdShowNullable = row.ErdShowNullable;
	}

	public ModuleRow(ModuleObject row, CustomFieldsSupport customFieldsSupport, bool forXml)
	{
		base.Id = row.ModuleId;
		base.Title = row.Title;
		base.Description = row.Description;
		ErdShowTypes = row.ErdShowTypes.GetValueOrDefault();
		DisplayDocumentationNameMode = DisplayDocumentationNameModeEnum.ObjectToType(row.DisplayDocumentationNameMode);
		base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Module, base.Id, customFieldsSupport);
		base.CustomFields.RetrieveCustomFields(row);
		DatabaseId = row.DatabaseId;
		ErdShowNullable = row.ErdShowNullable;
	}

	public ModuleRow()
	{
		base.Id = -1;
		base.Title = ModuleDoc.objectsWithoutModuleName;
		IsShown = true;
	}

	public static ModuleRow GetGeneralModule()
	{
		return new ModuleRow();
	}
}
