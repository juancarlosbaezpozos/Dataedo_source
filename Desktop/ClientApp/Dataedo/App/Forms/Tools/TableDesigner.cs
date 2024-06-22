using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Tools;

public class TableDesigner
{
	private Dictionary<int, Dictionary<string, string>> customFieldsColumnForHistory = new Dictionary<int, Dictionary<string, string>>();

	private Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory = new Dictionary<int, ObjectWithTitleAndDescription>();

	private bool isChanged;

	public string DocumentationTitle { get; set; }

	public int TableId { get; private set; }

	public string Schema { get; set; }

	public string Name { get; set; }

	public string Title { get; set; }

	public string Location { get; set; }

	public UserTypeEnum.UserType Source { get; set; } = UserTypeEnum.UserType.USER;


	public SynchronizeStateEnum.SynchronizeState SynchronizeState { get; set; } = SynchronizeStateEnum.SynchronizeState.Synchronized;


	public int DatabaseId { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectTypeValue { get; set; } = SharedObjectTypeEnum.ObjectType.Table;


	public string ObjectType => SharedObjectTypeEnum.TypeToString(ObjectTypeValue);

	public string Definition { get; set; }

	public bool IsEditMode { get; private set; }

	public string OldSchema { get; set; }

	public string OldName { get; set; }

	public string OldTitle { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Type { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype OldType { get; set; }

	public IEnumerable<ColumnRow> InsertedColumns { get; private set; }

	public CustomFieldsSupport CustomFieldsSupport { get; set; }

	public bool IsTableChanged { get; set; }

	public bool IsChanged
	{
		get
		{
			if (!DataSourceColumns.Any((ColumnRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Added) && !isChanged)
			{
				return SynchronizeState == SynchronizeStateEnum.SynchronizeState.New;
			}
			return true;
		}
		set
		{
			isChanged = value;
		}
	}

	public List<ColumnRow> DataSourceColumns { get; set; } = new List<ColumnRow>();


	public List<ColumnRow> ColumnsToRemove { get; set; } = new List<ColumnRow>();


	public TableDesigner(SharedObjectTypeEnum.ObjectType type)
	{
		ObjectTypeValue = type;
	}

	public TableDesigner(int tableId, string schema, string name, UserTypeEnum.UserType source, SharedObjectTypeEnum.ObjectType type, string definition, CustomFieldsSupport customFieldsSupport)
	{
		TableId = tableId;
		Schema = schema;
		Name = name;
		CustomFieldsSupport = customFieldsSupport;
		IsEditMode = true;
		SetColumnsByTableId(TableId);
		Source = source;
		ObjectTypeValue = type;
		Definition = definition;
	}

	private void SetColumnsByTableId(int id)
	{
		List<ColumnRow> columns = (from x in DB.Column.GetDataByTable(id)
			select new ColumnRow(x, CustomFieldsSupport)
			{
				UniqueId = ColumnRow.UniqueIdSource++
			}).ToList();
		columns.ForEach(delegate(ColumnRow x)
		{
			x.ParentColumn = columns.FirstOrDefault((ColumnRow y) => x.ParentId == y.Id);
		});
		DataSourceColumns = columns;
		BasicRow[] elements = DataSourceColumns?.ToArray();
		HistoryColumnsHelper.SaveColumnsOrParametrsOfOldCustomFields(elements, customFieldsColumnForHistory, titleAndDescriptionParametersForHistory);
	}

	public bool HasModifiedSourceProperties()
	{
		if (string.IsNullOrEmpty(OldName) == string.IsNullOrEmpty(Name) && (OldName ?? string.Empty).Equals(Name) && string.IsNullOrEmpty(OldSchema) == string.IsNullOrEmpty(Schema) && (OldSchema ?? string.Empty).Equals(Schema))
		{
			return !OldType.Equals(Type);
		}
		return true;
	}

	public void ModifySource()
	{
		if (Source == UserTypeEnum.UserType.DBMS && HasModifiedSourceProperties())
		{
			Source = UserTypeEnum.UserType.USER;
		}
	}

	public void InsertTable(Form owner = null)
	{
		TableModel tableModel = new TableModel(DatabaseId, Name, Schema, Title, Location, ObjectTypeValue, Type, Definition, Source);
		TableId = DB.Table.InsertManualTable(tableModel, owner).Value;
		DB.History.InsertHistoryRow(DatabaseId, TableId, Title, null, null, "tables", !string.IsNullOrEmpty(Title), !string.IsNullOrEmpty(null), tableModel.ObjectType);
		DB.Community.InsertFollowingToRepository(tableModel.ObjectType, TableId);
		DB.Database.UpdateDocumentationShowSchemaFlag(DatabaseId, owner);
		SetColumnsTableId(TableId);
		DBTreeMenu.AddManualObjectToTree(DatabaseId, TableId, Schema, Name, Title, ObjectTypeValue, Type, Source, null, SynchronizeState);
		WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectAdd();
	}

	public void UpdateTable(Form owner = null)
	{
		DocumentationObject dataById = DB.Database.GetDataById(DatabaseId);
		ObjectWithTitleAndHTMLDescriptionHistory objectWithTitleAndHTMLDescriptionHistory = DB.History.SelectTableTitleAndHTMLDescription(TableId, ObjectTypeValue);
		DB.Table.UpdateManualTable(TableId, Schema, Name, Title, Location, SharedObjectSubtypeEnum.TypeToString(ObjectTypeValue, Type), Definition, ObjectTypeValue == SharedObjectTypeEnum.ObjectType.Structure || ObjectTypeValue == SharedObjectTypeEnum.ObjectType.View, ObjectTypeValue == SharedObjectTypeEnum.ObjectType.Structure, UserTypeEnum.TypeToString(Source), owner);
		DB.Community.InsertFollowingToRepository(ObjectTypeValue, TableId);
		bool saveTitle = Title != objectWithTitleAndHTMLDescriptionHistory?.Title;
		bool saveDescription = false;
		if (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(objectWithTitleAndHTMLDescriptionHistory?.Title))
		{
			saveTitle = false;
		}
		DB.History.InsertHistoryRow(DatabaseId, TableId, Title, null, null, "tables", saveTitle, saveDescription, ObjectTypeValue);
		DB.Dependency.UpdateUserDependencies(Schema, Name, dataById.Host, dataById.Name, OldSchema, OldName, SharedObjectSubtypeEnum.TypeToString(ObjectTypeValue, Type));
		DB.Database.UpdateDocumentationShowSchemaFlag(DatabaseId, owner);
		DBTreeMenu.SetSource(DatabaseId, SharedObjectTypeEnum.ObjectType.Table, OldName, OldSchema, Source);
		DBTreeMenu.RefreshNodeName(DatabaseId, ObjectTypeValue, Name, OldName, Schema, OldSchema, Title, SharedDatabaseTypeEnum.DatabaseType.Manual, withSchema: true, Type);
		WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectEdit();
	}

	public bool Exists(string schemaColumnName, string tableColumnName)
	{
		return DB.Table.CheckIfTableRecordAlreadyExists(DatabaseId, Name, Schema, ObjectTypeValue, TableId);
	}

	private ColumnRow[] GetColumnsToUpdate(UserTypeEnum.UserType source)
	{
		return DataSourceColumns.Where((ColumnRow x) => x.Source == source && x.RowState == ManagingRowsEnum.ManagingRows.Updated).ToArray();
	}

	public ColumnRow AddColumnAndGetColumnRow(ColumnRow parentColumn)
	{
		ColumnRow columnRow = new ColumnRow(TableId)
		{
			UniqueId = ++ColumnRow.UniqueIdSource,
			Name = $"column{DataSourceColumns.Count() + 1}",
			RowState = ManagingRowsEnum.ManagingRows.ForAdding,
			Source = UserTypeEnum.UserType.USER,
			Level = (parentColumn?.Level ?? 0) + 1,
			Path = parentColumn?.FullName,
			ParentColumn = parentColumn,
			IsSortChanged = true
		};
		columnRow.CustomFields = new CustomFieldContainer(CustomFieldsSupport);
		columnRow.CustomFields.RetrieveCustomFields();
		DataSourceColumns.Add(columnRow);
		IsChanged = true;
		return columnRow;
	}

	public void SaveColumns(Form owner = null)
	{
		SetSortValues(DataSourceColumns);
		InsertManualColumns(owner);
		UpdateColumns(owner);
		RemoveColumnsFromDatabase(owner);
	}

	public static void SetSortValues(List<ColumnRow> columns)
	{
		foreach (var item2 in from x in columns
			group x by new { x.ParentColumn, x.Level })
		{
			List<ColumnRow> list = item2.OrderBy((ColumnRow x) => x.NodeLevelPosition).ToList();
			ColumnRow item = list.LastOrDefault((ColumnRow x) => x.Source == UserTypeEnum.UserType.DBMS);
			int num = list.IndexOf(item);
			for (int i = 0; i < list.Count; i++)
			{
				ColumnRow columnRow = list[i];
				columnRow.Sort = i + 1;
				if (i > num && columnRow.Source == UserTypeEnum.UserType.USER)
				{
					columnRow.Sort += 100000;
				}
			}
		}
	}

	public void UpdatePaths(ColumnRow columnRow)
	{
		foreach (ColumnRow item in DataSourceColumns.Where((ColumnRow x) => x.ParentColumn == columnRow))
		{
			item.Level = columnRow.Level + 1;
			item.Path = columnRow?.FullName;
			if (item.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
			{
				item.SetModified();
			}
			UpdatePaths(item);
		}
	}

	private void RemoveColumnsFromDatabase(Form owner = null)
	{
		IEnumerable<int> dataTable = from x in ColumnsToRemove
			where x.RowState == ManagingRowsEnum.ManagingRows.Deleted
			select x.Id;
		DB.Column.Delete(dataTable, owner);
	}

	public void RemoveFromIgnoredObjects(Form owner = null)
	{
		List<IgnoredObject> list = new List<IgnoredObject>();
		list.Add(new IgnoredObject
		{
			DatabaseId = DatabaseId,
			Schema = Schema,
			Name = Name,
			ObjectType = ObjectType
		});
		DB.IgnoredObjects.Delete(list, owner);
	}

	private void UpdateColumns(Form owner = null)
	{
		foreach (ColumnRow insertedColumn in InsertedColumns)
		{
			foreach (ColumnRow item in DataSourceColumns.Where((ColumnRow x) => x.UniqueParentId == insertedColumn.UniqueId))
			{
				item.ParentId = insertedColumn.Id;
			}
		}
		UpdateDBMSColumns(owner);
		UpdateManualColumns(owner);
		UpdateColumnsSort(owner);
		foreach (ColumnRow item2 in DataSourceColumns.Where((ColumnRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Updated))
		{
			item2.RowState = ManagingRowsEnum.ManagingRows.Added;
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Column, item2.Id);
		}
	}

	private void UpdateDBMSColumns(Form owner = null)
	{
		ColumnRow[] columnsToUpdate = GetColumnsToUpdate(UserTypeEnum.UserType.DBMS);
		if (ObjectTypeValue == SharedObjectTypeEnum.ObjectType.Structure)
		{
			DB.Column.UpdateManualColumns(null, columnsToUpdate);
			HistoryColumnsHelper.SaveTitleDescriptionCustomFieldsHistoryInUpdateColumns(customFieldsColumnForHistory, titleAndDescriptionParametersForHistory, columnsToUpdate, SharedObjectTypeEnum.ObjectType.Column, DatabaseId);
		}
		else
		{
			DB.Column.UpdateColumns(columnsToUpdate, customFieldsColumnForHistory, titleAndDescriptionParametersForHistory, DatabaseId, owner);
		}
	}

	private void UpdateManualColumns(Form owner = null)
	{
		ColumnRow[] columnsToUpdate = GetColumnsToUpdate(UserTypeEnum.UserType.USER);
		ColumnRow[] array = columnsToUpdate;
		foreach (ColumnRow columnRow in array)
		{
			columnRow.CustomFields.SetObjectId(columnRow.Id);
			columnRow.CustomFields.SetObjectType(columnRow.ObjectType);
		}
		DB.Column.UpdateManualColumns(owner, columnsToUpdate);
		array = columnsToUpdate;
		foreach (ColumnRow columnRow2 in array)
		{
			customFieldsColumnForHistory.TryGetValue(columnRow2.Id, out var _);
			HistoryColumnsHelper.SaveTitleDescriptionCustomFieldsHistoryInUpdateColumns(customFieldsColumnForHistory, titleAndDescriptionParametersForHistory, columnsToUpdate, SharedObjectTypeEnum.ObjectType.Column, DatabaseId);
		}
	}

	private void UpdateColumnsSort(Form owner = null)
	{
		ColumnRow[] array = DataSourceColumns.Where((ColumnRow x) => x.IsSortChanged).ToArray();
		DB.Column.UpdateColumnsSort(owner, array);
		SetDefaultIsSortChanged(array);
	}

	private void SetDefaultIsSortChanged(ColumnRow[] columns)
	{
		for (int i = 0; i < columns.Length; i++)
		{
			columns[i].IsSortChanged = false;
		}
	}

	private void InsertManualColumns(Form owner = null)
	{
		List<ColumnRow> list = DataSourceColumns.Where((ColumnRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding).ToList();
		DB.Column.InsertManualColumn(list, owner);
		foreach (ColumnRow item in list)
		{
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Column, item.Id);
			DB.History.InsertHistoryRow(DatabaseId, item.Id, item.Title, item.Description, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(item.ObjectType), !string.IsNullOrEmpty(item.Title), !string.IsNullOrEmpty(item.Description), item.ObjectType);
			HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(item?.Id, item?.CustomFields, DatabaseId, item.ObjectType);
		}
		InsertedColumns = list;
	}

	public void Remove(ColumnRow[] rowsToDelete)
	{
		if (rowsToDelete.Count() == 0)
		{
			return;
		}
		for (int num = rowsToDelete.Length - 1; num >= 0; num--)
		{
			if (rowsToDelete[num].RowState != ManagingRowsEnum.ManagingRows.ForAdding)
			{
				rowsToDelete[num].RowState = ManagingRowsEnum.ManagingRows.Deleted;
			}
			ColumnsToRemove.Add(rowsToDelete[num]);
			DataSourceColumns.Remove(rowsToDelete[num]);
		}
		IsChanged = true;
	}

	public void MoveBefore(ColumnRow row, ColumnRow referenceRow)
	{
		int num = DataSourceColumns.IndexOf(referenceRow);
		int num2 = DataSourceColumns.IndexOf(row);
		if (num >= 0 && num2 >= 0)
		{
			DataSourceColumns.Insert(num, row);
			DataSourceColumns.RemoveAt(num2);
		}
	}

	public void MoveAfter(ColumnRow row, ColumnRow referenceRow)
	{
		int num = DataSourceColumns.IndexOf(referenceRow);
		int num2 = DataSourceColumns.IndexOf(row);
		if (num >= 0 && num2 >= 0)
		{
			DataSourceColumns.Insert(num + 1, row);
			DataSourceColumns.RemoveAt(num2);
		}
	}

	public void SwapColumns(ColumnRow row1, ColumnRow row2)
	{
		int index = DataSourceColumns.IndexOf(row2);
		Swap(row1, index);
	}

	public void Swap(ColumnRow row, int index)
	{
		if (index >= 0)
		{
			DataSourceColumns.Remove(row);
			DataSourceColumns.Insert(index, row);
			IsTableChanged = true;
			IsChanged = true;
		}
	}

	public void SetSortedColumnsToBottomAsUpdated(int startIndex)
	{
		if (startIndex > 0 && DataSourceColumns[startIndex - 1].Sort != ColumnsHelper.MaxColumnsSort)
		{
			SetColumnSortAsModified(startIndex);
		}
		else
		{
			SetAllColumnsSortAsModified();
		}
	}

	private void SetColumnSortAsModified(int startIndex)
	{
		for (int i = startIndex; i < DataSourceColumns.Count; i++)
		{
			if (DataSourceColumns[i].RowState == ManagingRowsEnum.ManagingRows.Added || DataSourceColumns[i].RowState == ManagingRowsEnum.ManagingRows.Updated)
			{
				DataSourceColumns[i].IsSortChanged = true;
			}
		}
	}

	private void SetAllColumnsSortAsModified()
	{
		foreach (ColumnRow dataSourceColumn in DataSourceColumns)
		{
			dataSourceColumn.IsSortChanged = true;
		}
	}

	public void SetSortedColumnsToTopAsUpdated(int finishIndex)
	{
		if (finishIndex == 0)
		{
			return;
		}
		for (int num = finishIndex; num >= 0; num--)
		{
			if (DataSourceColumns[num].RowState == ManagingRowsEnum.ManagingRows.Added || DataSourceColumns[num].RowState == ManagingRowsEnum.ManagingRows.Updated)
			{
				DataSourceColumns[num].IsSortChanged = true;
			}
		}
	}

	public void SetColumnAsModified(ColumnRow columnRow)
	{
		if (columnRow != null && DataSourceColumns.Contains(columnRow))
		{
			if (columnRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
			{
				columnRow.SetModified();
			}
			IsChanged = true;
		}
	}

	public void ResetSort()
	{
		foreach (ColumnRow dataSourceColumn in DataSourceColumns)
		{
			dataSourceColumn.Sort = ColumnsHelper.MaxColumnsSort;
			dataSourceColumn.IsSortChanged = true;
			if (dataSourceColumn.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
			{
				dataSourceColumn.RowState = ManagingRowsEnum.ManagingRows.Updated;
			}
		}
		DefaultSortDataSourceColumns();
	}

	private void DefaultSortDataSourceColumns()
	{
		DataSourceColumns = GetDefaultSortedColumns();
	}

	private List<ColumnRow> GetDefaultSortedColumns()
	{
		return (from x in DataSourceColumns
			orderby x.Sort, (!x.Position.HasValue) ? ColumnsHelper.MaxColumnsSort : x.Position.Value
			select x).ToList();
	}

	public bool HasDuplicatesOrEmptyNames()
	{
		foreach (ColumnRow column in DataSourceColumns.Where((ColumnRow x) => x.Source == UserTypeEnum.UserType.USER))
		{
			column.AlreadyExists = DataSourceColumns.Count((ColumnRow x) => x.Path == column.Path && (x.Name?.ToLower().Equals(column.Name.ToLower()) ?? false)) > 1;
			column.IsNameEmpty = string.IsNullOrEmpty(column.Name);
		}
		return DataSourceColumns.Any((ColumnRow x) => x.AlreadyExists || x.IsNameEmpty);
	}

	public void SetColumnsTableId(int tableId)
	{
		foreach (ColumnRow item in DataSourceColumns.Where((ColumnRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
		{
			item.TableId = tableId;
		}
	}
}
