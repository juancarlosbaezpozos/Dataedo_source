namespace Dataedo.App.Classes.Synchronize;

public class ModuleSyncRow : BasicRow
{
	public string DescriptionPlain { get; set; }

	public string ErdLinkStyle { get; set; }

	public bool ErdShowTypes { get; set; }

	public string DisplayDocumentationNameMode { get; set; }

	public bool ErdShowNullable { get; set; }

	public ModuleSyncRow(int id, string title, string description, string descriptionPlain, string erdLinkStyle, bool erdShowTypes, string displayDocumentationNameMode, bool erdShowNullable)
	{
		base.Id = id;
		base.Title = title;
		base.Description = description;
		DescriptionPlain = descriptionPlain;
		ErdLinkStyle = erdLinkStyle;
		ErdShowTypes = erdShowTypes;
		DisplayDocumentationNameMode = displayDocumentationNameMode;
		ErdShowNullable = erdShowNullable;
	}
}
