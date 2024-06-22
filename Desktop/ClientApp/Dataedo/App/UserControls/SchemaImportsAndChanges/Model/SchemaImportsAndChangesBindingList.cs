using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraTreeList;

namespace Dataedo.App.UserControls.SchemaImportsAndChanges.Model;

public class SchemaImportsAndChangesBindingList : BindingList<SchemaImportsAndChangesObjectModel>, TreeList.IVirtualTreeListData
{
	public SchemaImportsAndChangesBindingList()
	{
	}

	public SchemaImportsAndChangesBindingList(IList<SchemaImportsAndChangesObjectModel> list)
		: base(list)
	{
	}

	public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
	{
		if (info.Node is SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel)
		{
			switch (info.Column.FieldName)
			{
			case "DateOperationObject":
				info.CellData = schemaImportsAndChangesObjectModel.DateOperationObject;
				break;
			case "Operation":
				info.CellData = schemaImportsAndChangesObjectModel.Operation;
				break;
			case "Type":
				info.CellData = schemaImportsAndChangesObjectModel.Type;
				break;
			case "ConnectionDetails":
				info.CellData = schemaImportsAndChangesObjectModel.ConnectionDetails;
				break;
			case "Comments":
				info.CellData = schemaImportsAndChangesObjectModel.Comments;
				break;
			case "CommentedBy":
				info.CellData = schemaImportsAndChangesObjectModel.CommentedBy;
				break;
			case "CommentDate":
				info.CellData = schemaImportsAndChangesObjectModel.CommentDate;
				break;
			case "User":
				info.CellData = schemaImportsAndChangesObjectModel.User;
				break;
			case "DBMSVersion":
				info.CellData = schemaImportsAndChangesObjectModel.DBMSVersion;
				break;
			}
		}
	}

	public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
	{
		info.Children = (info.Node as SchemaImportsAndChangesObjectModel)?.Children;
	}

	public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
	{
		if (info.Node is SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel && info.Column.FieldName == "Comments")
		{
			schemaImportsAndChangesObjectModel.Comments = info.NewCellData as string;
		}
	}

	public IEnumerable<SchemaImportsAndChangesObjectModel> Flatten()
	{
		return base.Items.SelectMany((SchemaImportsAndChangesObjectModel x) => x.Flatten());
	}
}
