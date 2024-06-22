using Dataedo.App.Classes.Synchronize;
using Dataedo.App.MenuTree;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataProfiling.Models;

public class TableSimpleData
{
	public SharedObjectTypeEnum.ObjectType ObjectType { get; private set; }

	public int TableId { get; private set; }

	public string TableName { get; private set; }

	public string Title { get; private set; }

	public string Schema { get; private set; }

	public string TableNameWithTitle
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(Title))
			{
				return TableName + " (" + Title + ")";
			}
			return TableName ?? "";
		}
	}

	public TableSimpleData(DBTreeNode treeNode)
	{
		TableId = treeNode.Id;
		TableName = treeNode.BaseName;
		Title = treeNode.Title;
		Schema = treeNode.Schema;
		ObjectType = treeNode.ObjectType;
	}

	public TableSimpleData(TableRow tableRow)
	{
		TableId = tableRow.Id;
		TableName = tableRow.Name;
		Title = tableRow.Title;
		Schema = tableRow.Schema;
		ObjectType = tableRow.ObjectType;
	}

	public TableSimpleData(int id, string name, string title, string schema, SharedObjectTypeEnum.ObjectType objectType)
	{
		TableId = id;
		TableName = name;
		Title = title;
		Schema = schema;
		ObjectType = objectType;
	}
}
