using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.FeedbackWidgetData;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class ColumnDB : CommonDBSupport
{
	public ColumnDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public ColumnObject GetDataById(int tableId, int columnId, bool notDeletedOnly = false)
	{
		List<ColumnObject> columns = commands.Select.Columns.GetColumns(tableId, columnId, GetNotStatusValue(notDeletedOnly));
		if (columns.Count <= 0)
		{
			return null;
		}
		return columns[0];
	}

	public int? GetObjectIdByName(int databaseId, string schema, string tableName, string objectType, string columnName)
	{
		return commands.Select.Columns.GetObjectIdByName(databaseId, schema, tableName, objectType, columnName);
	}

	public ColumnViewObject GetDataByName(int databaseId, string schema, string tableName, string objectType, string columnName, string path)
	{
		return commands.Select.Columns.GetObjectByName(databaseId, schema, tableName, objectType, columnName, path);
	}

	public List<ColumnObject> GetDataByTable(int tableId, bool notDeletedOnly = false)
	{
		return commands.Select.Columns.GetColumns(tableId, GetNotStatusValue(notDeletedOnly));
	}

	public List<ColumnObjectExtended<ColumnObject>> GetHierarchicalDataByTable(int tableId, bool notDeletedOnly = false)
	{
		List<ColumnObjectExtended<ColumnObject>> list = (from x in commands.Select.Columns.GetColumns(tableId, GetNotStatusValue(notDeletedOnly))
			select new ColumnObjectExtended<ColumnObject>(x)).ToList();
		PrepareTreeStructure(list);
		return (from x in list
			where !x.ColumnObject.ParentId.HasValue
			orderby x.ColumnObject.Sort, x.ColumnObject.OrdinalPosition ?? 99999
			select x).ToList();
	}

	private void PrepareTreeStructure<T>(List<ColumnObjectExtended<T>> groupedColumns) where T : ColumnObject
	{
		int level = 1;
		while (true)
		{
			IEnumerable<ColumnObjectExtended<T>> enumerable = groupedColumns.Where((ColumnObjectExtended<T> x) => x.ColumnObject.Level == level);
			if (enumerable.Count() == 0 || !groupedColumns.Any((ColumnObjectExtended<T> x) => x.ColumnObject.Level == level + 1))
			{
				break;
			}
			foreach (ColumnObjectExtended<T> levelColumn in enumerable)
			{
				List<ColumnObjectExtended<T>> list = (from x in groupedColumns
					where x.ColumnObject.ParentId == levelColumn.ColumnObject.ColumnId
					orderby x.ColumnObject.Sort, x.ColumnObject.OrdinalPosition ?? 99999
					select x).ToList();
				list.ForEach(delegate(ColumnObjectExtended<T> x)
				{
					x.ParentColumn = levelColumn;
				});
				levelColumn.Subcolumns = new List<ColumnObjectExtended<T>>(list);
			}
			level++;
		}
	}

	public IEnumerable<ColumnWithTableAndDatabaseObject> GetColumnsWithTableAndDatabase(IEnumerable<string> fields)
	{
		return commands.Select.Columns.GetColumnsWithTableAndDatabase(fields);
	}

	public FeedbackWidgetDataObject GetColumnFeedbackWidgetData(int columnId)
	{
		return commands.Select.Columns.GetColumnFeedbackWidgetData(columnId);
	}

	public IEnumerable<ColumnRowSummary> GetColumnsByName(string name, CustomFieldsSupport customFieldsSupport)
	{
		return MapToColumnRowSummaryCollection(commands.Select.Columns.GetColumnsByColumnName(name), customFieldsSupport);
	}

	private IEnumerable<ColumnRowSummary> MapToColumnRowSummaryCollection(IEnumerable<ColumnViewObject> data, CustomFieldsSupport customFieldsSupport)
	{
		List<ColumnRowSummary> list = new List<ColumnRowSummary>();
		foreach (ColumnViewObject datum in data)
		{
			list.Add(new ColumnRowSummary(datum, customFieldsSupport));
		}
		return list;
	}

	public List<ColumnWithReferenceObject> GetDataByTableWithReferences(int tableId, bool notDeletedOnly = false)
	{
		return commands.Select.Columns.GetColumnsWithReferences(tableId, GetNotStatusValue(notDeletedOnly));
	}

	public BindingList<ColumnRow> GetDataObjectByTableId(bool contextShowSchema, ObjectRow objectRow, int tableId, bool notDeletedOnly = false, CustomFieldsSupport customFieldsSupport = null)
	{
		List<ColumnRow> list = (from ColumnWithReferenceObject column in GetDataByTableWithReferences(tableId, notDeletedOnly)
			select new ColumnRow(contextShowSchema, objectRow, column, customFieldsSupport) into x
			group x by x.Id).Select(delegate(IGrouping<int, ColumnRow> x)
		{
			ColumnRow columnRow = x.First();
			columnRow.ReferencesDataContainer.Data = x.SelectMany((ColumnRow y) => y.ReferencesDataContainer.Data).ToList();
			columnRow.UniqueConstraintsDataContainer.Data = x.SelectMany((ColumnRow y) => y.UniqueConstraintsDataContainer.Data).ToList();
			columnRow.DataLinksDataContainer.Data = x.SelectMany((ColumnRow y) => y.DataLinksDataContainer.Data).ToList();
			return columnRow;
		}).ToList();
		PrepareTreeStructure(list);
		IOrderedEnumerable<ColumnRow> specificLevelSubColumns = from x in list
			where !x.ParentId.HasValue
			orderby x.Sort, x.Position ?? 99999
			select x;
		List<ColumnRow> list2 = new List<ColumnRow>();
		CreateHierarchicalList(specificLevelSubColumns, list, list2);
		return new BindingList<ColumnRow>(list2);
	}

	private void PrepareTreeStructure(List<ColumnRow> groupedColumns)
	{
		int level = 1;
		while (true)
		{
			IEnumerable<ColumnRow> enumerable = groupedColumns.Where((ColumnRow x) => x.Level == level);
			if (enumerable.Count() == 0 || !groupedColumns.Any((ColumnRow x) => x.Level == level + 1))
			{
				break;
			}
			foreach (ColumnRow levelColumn in enumerable)
			{
				List<ColumnRow> list = groupedColumns.Where((ColumnRow x) => x.ParentId == levelColumn.Id).ToList();
				list.ForEach(delegate(ColumnRow x)
				{
					x.ParentColumn = levelColumn;
				});
				levelColumn.Subcolumns = new BindingList<ColumnRow>(list);
			}
			level++;
		}
	}

	private void CreateHierarchicalList(IEnumerable<ColumnRow> specificLevelSubColumns, List<ColumnRow> allColumns, List<ColumnRow> result)
	{
		if (specificLevelSubColumns == null)
		{
			return;
		}
		foreach (ColumnRow column in specificLevelSubColumns?.OrderBy((ColumnRow x) => x.Sort)?.ThenBy((ColumnRow x) => x.Position ?? 99999))
		{
			result.Add(column);
			CreateHierarchicalList(allColumns.Where((ColumnRow x) => x.ParentId == column.Id).ToList(), allColumns, result);
		}
	}

	public BindingList<ColumnRow> GetDataObjectByTableId(ObjectRow objectRow, int tableId, bool notDeletedOnly = false, CustomFieldsSupport customFieldsSupport = null)
	{
		List<ColumnRow> list = (from ColumnWithReferenceObject column in GetDataByTableWithReferences(tableId, notDeletedOnly)
			select new ColumnRow(objectRow, column, customFieldsSupport) into x
			group x by x.Id).Select(delegate(IGrouping<int, ColumnRow> x)
		{
			ColumnRow columnRow = x.First();
			columnRow.ReferencesDataContainer.Data = x.SelectMany((ColumnRow y) => y.ReferencesDataContainer.Data).ToList();
			columnRow.UniqueConstraintsDataContainer.Data = x.SelectMany((ColumnRow y) => y.UniqueConstraintsDataContainer.Data).ToList();
			columnRow.DataLinksDataContainer.Data = x.SelectMany((ColumnRow y) => y.DataLinksDataContainer.Data).ToList();
			return columnRow;
		}).ToList();
		PrepareTreeStructure(list.ToList());
		IOrderedEnumerable<ColumnRow> specificLevelSubColumns = from x in list
			where !x.ParentId.HasValue
			orderby x.Sort, x.Position ?? 99999
			select x;
		List<ColumnRow> list2 = new List<ColumnRow>();
		CreateHierarchicalList(specificLevelSubColumns, list, list2);
		return new BindingList<ColumnRow>(list2);
	}

	public List<ColumnObjectExtended<ColumnDocObject>> GetDataByTableDoc(int tableId)
	{
		List<ColumnObjectExtended<ColumnDocObject>> list = (from x in commands.Select.Columns.GetColumnsDoc(tableId)
			select new ColumnObjectExtended<ColumnDocObject>(x)).ToList();
		PrepareTreeStructure(list);
		IOrderedEnumerable<ColumnObjectExtended<ColumnDocObject>> specificLevelSubColumns = from x in list
			where !x.ColumnObject.ParentId.HasValue
			orderby x.ColumnObject.Sort, x.ColumnObject.OrdinalPosition ?? 99999
			select x;
		List<ColumnObjectExtended<ColumnDocObject>> result = new List<ColumnObjectExtended<ColumnDocObject>>();
		CreateHierarchicalList(specificLevelSubColumns, list, result);
		return result;
	}

	private void CreateHierarchicalList(IEnumerable<ColumnObjectExtended<ColumnDocObject>> specificLevelSubColumns, List<ColumnObjectExtended<ColumnDocObject>> allColumns, List<ColumnObjectExtended<ColumnDocObject>> result)
	{
		if (specificLevelSubColumns == null)
		{
			return;
		}
		foreach (ColumnObjectExtended<ColumnDocObject> column in specificLevelSubColumns?.OrderBy((ColumnObjectExtended<ColumnDocObject> x) => x.ColumnObject.Sort)?.ThenBy((ColumnObjectExtended<ColumnDocObject> x) => x.ColumnObject.OrdinalPosition ?? 99999))
		{
			result.Add(column);
			CreateHierarchicalList(allColumns.Where((ColumnObjectExtended<ColumnDocObject> x) => x.ColumnObject.ParentId == column.ColumnObject.ColumnId).ToList(), allColumns, result);
		}
	}

	public bool Update(Form owner, params ColumnRow[] rows)
	{
		if (rows != null)
		{
			try
			{
				Column[] array = rows.Select((ColumnRow x) => ConvertRowToItem(x)).ToArray();
				commands.Manipulation.Columns.UpdateColumns(array);
				Column[] array2 = array;
				foreach (Column column in array2)
				{
					DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Column, column.ColumnId);
				}
				CustomFieldContainer.SaveValuesForDefinition(rows.Select((ColumnRow x) => x.CustomFields), DB.CustomField.UpdateCustomFieldValues);
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating columns:", owner);
				return false;
			}
		}
		return true;
	}

	public bool UpdateColumns(IEnumerable<ColumnRow> columns, Dictionary<int, Dictionary<string, string>> oldCustomFields, Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory, int? databaseId = null, Form owner = null)
	{
		try
		{
			if (columns != null)
			{
				ColumnRow[] array = columns.Where((ColumnRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Unchanged).ToArray();
				if (DB.Column.Update(owner, array))
				{
					HistoryColumnsHelper.SaveTitleDescriptionCustomFieldsHistoryInUpdateColumns(oldCustomFields, titleAndDescriptionParametersForHistory, array, SharedObjectTypeEnum.ObjectType.Column, databaseId);
					ColumnRow[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].SetUnchanged();
					}
				}
				CustomFieldContainer.SaveValuesForDefinition(array.Select((ColumnRow x) => x.CustomFields), DB.CustomField.UpdateCustomFieldValues);
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating columns", owner);
			return false;
		}
	}

	public bool UpdateManualColumns(Form owner = null, params ColumnRow[] rows)
	{
		if (rows != null)
		{
			try
			{
				commands.Manipulation.Columns.UpdateManualColumns(rows.Select((ColumnRow x) => ConvertRowToManualItem(x)).ToArray());
				CustomFieldContainer.UpdateDefinitionValues(rows.SelectMany((ColumnRow x) => x.CustomFields.CustomFieldsData));
				CustomFieldContainer.SaveValuesForDefinition(rows.Select((ColumnRow x) => x.CustomFields), DB.CustomField.UpdateCustomFieldValues);
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating manual columns", owner);
				return false;
			}
		}
		return true;
	}

	public bool UpdateColumnsSort(Form owner, params ColumnRow[] rows)
	{
		if (rows != null)
		{
			try
			{
				commands.Manipulation.Columns.UpdateColumnsSort(rows.Select((ColumnRow x) => ConvertRowToManualItem(x)).ToArray());
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating columns", owner);
				return false;
			}
		}
		return true;
	}

	public bool UpdateColumnsTitleAndDescription(Form owner, params ColumnRow[] rows)
	{
		if (rows != null)
		{
			try
			{
				commands.Manipulation.Columns.UpdateColumnsTitleAndDescription(rows.Select((ColumnRow x) => ConvertRowToColumnWithDescriptionAndTitle(x)).ToArray());
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating manual columns", owner);
				return false;
			}
		}
		return true;
	}

	public bool UpdateColumnsWithoutSort(Form owner, params ColumnRow[] rows)
	{
		if (rows != null)
		{
			try
			{
				commands.Manipulation.Columns.UpdateColumnsWithoutSort(rows.Select((ColumnRow x) => ConvertRowToColumnWithoutSort(x)).ToArray());
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating manual columns", owner);
				return false;
			}
		}
		return true;
	}

	public bool InsertManualColumn(List<ColumnRow> rows, Form owner = null)
	{
		if (rows != null)
		{
			try
			{
				Column[] array = rows.Select((ColumnRow x) => ConvertRowToManualItem(x)).ToArray();
				commands.Manipulation.Columns.InsertManualColumns(array);
				foreach (ColumnRow row in rows)
				{
					Column column = array.FirstOrDefault((Column x) => x.UniqueId == row.UniqueId);
					row.Id = column?.ColumnId ?? row.Id;
					if (row.CustomFields != null)
					{
						row.CustomFields.SetObjectId(row.Id);
						row.CustomFields.SetObjectType(row.ObjectType);
					}
				}
				CustomFieldContainer.UpdateDefinitionValues(rows.Where((ColumnRow x) => x.CustomFields != null).SelectMany((ColumnRow x) => x.CustomFields.CustomFieldsData));
				CustomFieldContainer.SaveValuesForDefinition(from x in rows
					where x.CustomFields != null
					select x.CustomFields, DB.CustomField.UpdateCustomFieldValues);
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while inserting columns:", owner);
				return false;
			}
		}
		return true;
	}

	private Column ConvertRowToItem(ColumnRow row)
	{
		Column column = new Column
		{
			ColumnId = row.Id,
			Title = row.Title,
			Description = row.Description,
			Sort = row.Sort,
			DataType = row.DataType,
			DataLength = row.DataLength,
			Subtype = row.Type,
			Path = row.Path
		};
		SetCustomFields(column, row);
		return column;
	}

	private Column ConvertRowToManualItem(ColumnRow row)
	{
		Column column = new Column
		{
			UniqueId = row.UniqueId,
			Title = row.Title,
			Description = row.Description,
			TableId = row.TableId,
			Name = row.Name,
			DataType = row.DataTypeWithoutLength,
			DataLength = (string.IsNullOrWhiteSpace(row.DataLength) ? null : row.DataLength),
			Nullable = row.Nullable,
			DefaultValue = row.DefaultValue,
			IsIdentity = row.IsIdentity,
			ComputedFormula = row.ComputedFormula,
			Source = row.Source,
			Position = row.Position,
			ParentId = row.ParentId,
			ColumnId = row.Id,
			Sort = row.Sort,
			Subtype = row.Type,
			Path = row.Path,
			Level = row.Level
		};
		SetCustomFields(column, row);
		return column;
	}

	private Column ConvertRowToColumnWithDescriptionAndTitle(ColumnRow row)
	{
		return new Column
		{
			ColumnId = row.Id,
			Title = row.Title,
			Description = row.Description,
			Sort = row.Sort,
			Subtype = row.Type,
			Path = row.Path
		};
	}

	private Column ConvertRowToColumnWithoutSort(ColumnRow row)
	{
		return new Column
		{
			Title = row.Title,
			Description = row.Description,
			TableId = row.TableId,
			Name = row.Name,
			DataType = row.DataType,
			DataLength = row.DataLength,
			Nullable = row.Nullable,
			DefaultValue = row.DefaultValue,
			IsIdentity = row.IsIdentity,
			ComputedFormula = row.ComputedFormula,
			Position = row.Position,
			ColumnId = row.Id,
			Subtype = row.Type,
			Path = row.Path
		};
	}

	public bool UpdateColumnsConstraints(int databaseId, Form owner = null)
	{
		try
		{
			commands.Manipulation.Columns.UpdateColumnsConstraintsSetNotPrimaryKeys(databaseId);
			commands.Manipulation.Columns.UpdateColumnsConstraintsPrimaryKeys(databaseId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating columns constraints:", owner);
			return false;
		}
		return true;
	}

	public bool Delete(IEnumerable<int> dataTable, Form owner = null)
	{
		if (dataTable != null)
		{
			try
			{
				commands.Manipulation.Columns.DeleteColumns(dataTable.ToArray(), Dataedo.App.StaticData.IsProjectFile);
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating columns:", owner);
				return false;
			}
		}
		return true;
	}

	public int Synchronize(IEnumerable<ColumnRow> columns, string tableName, string schema, int tableId, string type, int databaseId, bool isDbAdded, int updateId, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		try
		{
			HistoryImportChangesHelper.CheckColumnChangesInColumnDB(columns, databaseId, tableId, isDbAdded, DB.History.SavingEnabled, customFieldsSupport);
			int result = commands.Synchronization.Columns.SynchronizeColumns(columns.Select((ColumnRow r) => ConvertRowToSynchronizationItem(r, tableName, schema, databaseId, updateId)).ToArray(), tableName, schema, type, databaseId, isDbAdded, updateId);
			HistoryImportChangesHelper.InsertHistoryRowsInColumnDB(columns, databaseId, tableId, isDbAdded);
			return result;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the columns:", owner);
			return -1;
		}
	}

	private ColumnForSynchronization ConvertRowToSynchronizationItem(ColumnRow row, string tableName, string schema, int databaseId, int updateId)
	{
		ColumnForSynchronization columnForSynchronization = new ColumnForSynchronization
		{
			BaseName = tableName,
			BaseSchema = schema,
			BaseId = databaseId,
			Name = row.Name,
			Position = row.Position,
			DataType = row.DataType,
			Description = row.Description,
			ConstraintType = row.ConstraintType,
			DataLength = row.DataLength,
			IsNullable = row.Nullable,
			DefaultValue = row.DefaultValue,
			IsIdentity = row.IsIdentity,
			IsComputed = row.IsComputed,
			ComputedFormula = row.ComputedFormula,
			DefaultDef = row.DefaultDef,
			IdentityDef = row.IdentityDef,
			UpdateId = updateId,
			Path = row.Path,
			Level = row.Level,
			Type = row.Type,
			Title = row.Title
		};
		SetCustomFields(columnForSynchronization, row);
		return columnForSynchronization;
	}

	public bool UpdateColumnFromManualImport(Column column)
	{
		try
		{
			commands.Manipulation.Columns.UpdateColumnFromManualImport(column);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating column:");
			return false;
		}
		return true;
	}

	public bool UpdateColumnsFromManualImport(Column[] columns, Action<Column, DbTransaction> additionalAction = null)
	{
		try
		{
			commands.Manipulation.Columns.UpdateColumnsFromManualImport(columns, null, additionalAction);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating columns:");
			return false;
		}
		return true;
	}

	public bool CheckIfColumnWithIdExists(int columnId)
	{
		return commands.Select.Columns.CheckIfColumnWithIdExists(columnId);
	}
}
