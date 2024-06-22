using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.MenuTree;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class BulkTableInserter
{
	private List<BulkTableModel> dataSource;

	private int databaseId;

	public List<TableModel> Tables { get; private set; }

	public SharedObjectTypeEnum.ObjectType ObjectTypeValue { get; set; } = SharedObjectTypeEnum.ObjectType.Table;


	public string ObjectType => SharedObjectTypeEnum.TypeToString(ObjectTypeValue);

	public BulkTableInserter(int databaseId, List<BulkTableModel> dataSource, SharedObjectTypeEnum.ObjectType objectType)
	{
		Tables = new List<TableModel>();
		this.dataSource = dataSource;
		this.databaseId = databaseId;
		ObjectTypeValue = objectType;
	}

	public void AddTableModels()
	{
		foreach (BulkTableModel item in dataSource)
		{
			TableModel manualTable = new TableModel(databaseId, item.TableName, item.Schema, string.Empty, string.Empty, ObjectTypeValue, SharedObjectSubtypeEnum.GetDefaultByMainType(ObjectTypeValue), null, UserTypeEnum.UserType.USER);
			if (!Tables.Any((TableModel x) => ((x.Schema == null && manualTable.Schema == null) || x.Schema.Equals(manualTable.Schema)) && x.Name.Equals(manualTable.Name)))
			{
				Tables.Add(manualTable);
			}
		}
	}

	public bool TableExists(TableModel table)
	{
		List<TableByDatabaseIdObject> tablesByDatabase = DB.Table.GetTablesByDatabase(databaseId, ObjectTypeValue);
		TableByDatabaseIdObject tableByDatabaseIdObject = tablesByDatabase.FirstOrDefault((TableByDatabaseIdObject x) => x.DatabaseId == table.DatabaseId && x.Schema == table.Schema && x.Name == table.Name);
		if (tableByDatabaseIdObject != null)
		{
			table.Source = UserTypeEnum.ObjectToType(tableByDatabaseIdObject.Source);
			table.Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Table, tableByDatabaseIdObject.Subtype);
		}
		return tablesByDatabase.Count((TableByDatabaseIdObject x) => (x.Schema?.ToLower() + "." + x.Name?.ToLower()).Equals(table.Schema?.ToLower() + "." + table.Name?.ToLower())) > 0;
	}

	public BulkAddColumnStatusEnum ColumnExists(List<ColumnObject> data, int tableId, ColumnRow column)
	{
		BulkAddColumnStatusEnum result = BulkAddColumnStatusEnum.None;
		IEnumerable<ColumnObject> source = data.Where((ColumnObject x) => x.Name.Equals(column.Name));
		if (!data.Any((ColumnObject x) => x.Name.Equals(column.Name)))
		{
			result = BulkAddColumnStatusEnum.Insert;
		}
		else
		{
			if (data.Any((ColumnObject x) => x.Name.Equals(column.Name) && x.Source.Equals("DBMS")))
			{
				result = BulkAddColumnStatusEnum.DBMSUpdate;
			}
			else if (data.Any((ColumnObject x) => x.Name.Equals(column.Name) && x.Source.Equals("USER")))
			{
				result = BulkAddColumnStatusEnum.UserUpdate;
			}
			column.Id = source.Select((ColumnObject x) => x.ColumnId).FirstOrDefault();
			column.Title = (string.IsNullOrEmpty(column.Title) ? source.Select((ColumnObject x) => x.Title).FirstOrDefault() : column.Title);
			column.Description = (string.IsNullOrEmpty(column.Description) ? source.Select((ColumnObject x) => x.Description).FirstOrDefault() : column.Description);
		}
		return result;
	}

	public void GetData(TableModel table)
	{
		TableIdAndColumnsCountModel tableIdAndColumnsCountModel = DB.Table.GetTableIdAndColumnsCountModel(databaseId, table.Name, table.Schema, table.ObjectType);
		table.TableId = tableIdAndColumnsCountModel.TableId;
		table.NextColumnSortValue = tableIdAndColumnsCountModel.ColumnsCount + 100001;
		table.AlreadyExists = true;
	}

	public void InsertTables(Form owner = null)
	{
		foreach (TableModel table in Tables)
		{
			if (string.IsNullOrEmpty(table.Name))
			{
				return;
			}
			if (TableExists(table))
			{
				GetData(table);
			}
			else
			{
				table.TableId = DB.Table.InsertManualTable(table, owner);
				table.Source = UserTypeEnum.UserType.USER;
				table.NextColumnSortValue = 100001;
				DB.Community.InsertFollowingToRepository(table.ObjectType, table.TableId);
			}
			InsertColumns(table.FullName, table, owner);
		}
		DB.Database.UpdateDocumentationShowSchemaFlag(databaseId, owner);
	}

	public void AddTablesToTree()
	{
		Tables.Where((TableModel x) => !x.AlreadyExists && !string.IsNullOrEmpty(x.Name)).ToList().ForEach(delegate(TableModel x)
		{
			DBTreeMenu.AddManualObjectToTree(databaseId, x.TableId.Value, x.Schema, x.Name, string.Empty, x.ObjectType, x.Subtype ?? SharedObjectSubtypeEnum.ObjectSubtype.Table, x.Source ?? UserTypeEnum.UserType.USER);
		});
	}

	internal void InsertIntoModules(DBTreeNode node)
	{
		foreach (TableModel table in Tables)
		{
			if (!table.AlreadyExists && !node.Nodes.Any((DBTreeNode x) => x.Id == table.TableId))
			{
				if (node.IsFolder && node.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
				{
					DB.Module.InsertManualTable(node.ParentNode.Id, table.TableId.Value);
					DBTreeMenu.AddManualObjectToTree(table.DatabaseId, table.TableId.Value, table.Schema, table.Name, string.Empty, table.ObjectType, table.Subtype ?? SharedObjectSubtypeEnum.GetDefaultByMainType(table.ObjectType), table.Source ?? UserTypeEnum.UserType.DBMS, node);
				}
				else if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Table && node.IsInModule)
				{
					DB.Module.InsertManualTable(node.ParentNode.ParentNode.Id, table.TableId.Value);
					DBTreeMenu.AddManualObjectToTree(table.DatabaseId, table.TableId.Value, table.Schema, table.Name, string.Empty, table.ObjectType, table.Subtype ?? SharedObjectSubtypeEnum.GetDefaultByMainType(table.ObjectType), table.Source ?? UserTypeEnum.UserType.DBMS, node.ParentNode);
				}
			}
		}
	}

	public void InsertColumns(string selector, TableModel table, Form owner = null)
	{
		List<ColumnObject> dataByTable = DB.Column.GetDataByTable(table.TableId.Value);
		List<ColumnRow> list = dataSource.ToLookup((BulkTableModel x) => x.FullName, (BulkTableModel x) => x.Column)[selector].ToList();
		List<ColumnRow> list2 = new List<ColumnRow>();
		List<ColumnRow> list3 = new List<ColumnRow>();
		List<ColumnRow> list4 = new List<ColumnRow>();
		foreach (ColumnRow item in list)
		{
			item.TableId = table.TableId;
			if (ColumnExists(dataByTable, table.TableId.Value, item) == BulkAddColumnStatusEnum.Insert)
			{
				list2.Add(item);
			}
			else if (ColumnExists(dataByTable, table.TableId.Value, item) == BulkAddColumnStatusEnum.DBMSUpdate)
			{
				list4.Add(item);
			}
			else if (ColumnExists(dataByTable, table.TableId.Value, item) == BulkAddColumnStatusEnum.UserUpdate)
			{
				list3.Add(item);
			}
			item.Sort = table.NextColumnSortValue;
		}
		InsertGroupedColumns(list2, table, owner);
		UpdateColumns(list3, list4, owner);
	}

	private void InsertGroupedColumns(List<ColumnRow> columns, TableModel table, Form owner = null)
	{
		foreach (IGrouping<string, ColumnRow> item in columns.ToLookup((ColumnRow x) => x.Name))
		{
			ColumnRow columnRow = item.LastOrDefault();
			columnRow.Sort = table.NextColumnSortValue;
			table.NextColumnSortValue++;
			DB.Column.InsertManualColumn(new List<ColumnRow> { columnRow }, owner);
			DB.History.InsertHistoryRow(databaseId, columnRow.Id, columnRow.Title, columnRow.Description, null, "columns", !string.IsNullOrEmpty(columnRow.Title), !string.IsNullOrEmpty(columnRow.Description), table.ObjectType);
		}
	}

	private void UpdateColumns(List<ColumnRow> userColumns, List<ColumnRow> dbmsColumns, Form owner = null)
	{
		foreach (IGrouping<string, ColumnRow> item in userColumns.ToLookup((ColumnRow x) => x.Name))
		{
			ColumnRow columnRow = item.LastOrDefault();
			HistoryColumnsHelper.CheckColumnsForHistory(columnRow, ObjectTypeValue);
			DB.Column.UpdateColumnsWithoutSort(owner, columnRow);
			DB.History.InsertHistoryRow(databaseId, columnRow.Id, columnRow.Title, columnRow.Description, null, "columns", columnRow.IsTitleChangedHistory, columnRow.IsDescriptionChangedHistory, ObjectTypeValue);
		}
		foreach (IGrouping<string, ColumnRow> item2 in dbmsColumns.ToLookup((ColumnRow x) => x.Name))
		{
			ColumnRow columnRow2 = item2.LastOrDefault();
			HistoryColumnsHelper.CheckColumnsForHistory(columnRow2, ObjectTypeValue);
			DB.Column.UpdateColumnsTitleAndDescription(owner, columnRow2);
			DB.History.InsertHistoryRow(databaseId, columnRow2.Id, columnRow2.Title, null, null, "columns", columnRow2.IsTitleChangedHistory, columnRow2.IsDescriptionChangedHistory, ObjectTypeValue);
		}
	}
}
