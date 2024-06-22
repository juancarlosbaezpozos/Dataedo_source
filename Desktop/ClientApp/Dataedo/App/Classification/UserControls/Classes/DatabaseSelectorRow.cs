using System.ComponentModel;
using System.Drawing;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classification.UserControls.Classes;

public class DatabaseSelectorRow
{
	[Browsable(false)]
	public int DatabaseId { get; set; }

	public bool IsSelected { get; set; }

	public Image Icon { get; set; }

	public string DatabaseName { get; set; }

	public int ClassifiedFieldNumber { get; set; }

	public DatabaseSelectorRow(int databaseId, string databaseName, string dbtype, string dbclass, int classifiedFieldNumber)
	{
		DatabaseId = databaseId;
		DatabaseName = databaseName;
		ClassifiedFieldNumber = classifiedFieldNumber;
		Icon = IconsSupport.GetDatabaseIconByName16(SharedDatabaseTypeEnum.StringToType(dbtype), SharedObjectTypeEnum.StringToType(dbclass));
	}
}
