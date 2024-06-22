using System.Collections.Generic;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout;

namespace Dataedo.App.Forms.Tools;

public static class MetadataToolTip
{
	private static readonly Dictionary<string, string> tooltips = new Dictionary<string, string>
	{
		["dbms_created"] = "Object's creation date in the database",
		["dbms_last_updated"] = "Object's last modification in the database",
		["first_imported"] = "Object's metadata creation date in Dataedo",
		["first_imported_by"] = "Dataedo user who created the object's metadata",
		["last_imported"] = "Object's last import start time according to database",
		["last_imported_by"] = "Dataedo user who last imported the object's metadata",
		["last_updated"] = "Object's last edition date according to Dataedo repository",
		["last_updated_by"] = "Dataedo user who last changed the object's metadata"
	};

	public static void SetLayoutControlItemToolTip(LayoutControlItem layout, string key)
	{
		layout.OptionsToolTip.ToolTip = tooltips[key];
	}

	public static void SetColumnToolTip(GridColumn column, string key)
	{
		column.ToolTip = tooltips[key];
	}
}
