using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.Model.Data.History;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Tools;

public class ProcedureDesigner
{
	private Dictionary<int, Dictionary<string, string>> customFieldsParametersForHistory = new Dictionary<int, Dictionary<string, string>>();

	private Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory = new Dictionary<int, ObjectWithTitleAndDescription>();

	public string DocumentationTitle { get; set; }

	public int ProcedureId { get; private set; }

	public string Schema { get; set; }

	public string Name { get; set; }

	public string Title { get; set; }

	public string Language { get; set; } = "SQL";


	public string FunctionType { get; set; }

	public UserTypeEnum.UserType Source { get; set; } = UserTypeEnum.UserType.USER;


	public SynchronizeStateEnum.SynchronizeState SynchronizeState { get; set; } = SynchronizeStateEnum.SynchronizeState.Synchronized;


	public int DatabaseId { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectTypeValue { get; set; } = SharedObjectTypeEnum.ObjectType.Procedure;


	public string ObjectType => SharedObjectTypeEnum.TypeToString(ObjectTypeValue);

	public string Definition { get; set; }

	public string OldSchema { get; set; }

	public string OldName { get; set; }

	public string OldTitle { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Type { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype OldType { get; set; }

	public CustomFieldsSupport CustomFieldsSupport { get; set; }

	public List<ParameterRow> DataSourceParameterRows { get; set; } = new List<ParameterRow>();


	public List<ParameterRow> RowsToRemove { get; set; } = new List<ParameterRow>();


	public bool IsChanged { get; internal set; }

	public ProcedureDesigner(SharedObjectTypeEnum.ObjectType type)
	{
		ObjectTypeValue = type;
	}

	public ProcedureDesigner(int tableId, string schema, string name, UserTypeEnum.UserType source, SharedObjectTypeEnum.ObjectType type, string definition, CustomFieldsSupport customFieldsSupport)
	{
		ProcedureId = tableId;
		Schema = schema;
		Name = name;
		CustomFieldsSupport = customFieldsSupport;
		Source = source;
		ObjectTypeValue = type;
		Definition = definition;
	}

	public void InsertProcedure(Form owner = null)
	{
		ObjectRow objectRow = new ObjectRow(Name, Schema, DocumentationTitle, ObjectTypeValue, null, null, null, Definition, FunctionType, Language, DatabaseId, null);
		objectRow.Title = Title;
		objectRow.Source = Source;
		objectRow.ObjectId = ProcedureId;
		objectRow.Subtype = Type;
		if (SynchronizeState == SynchronizeStateEnum.SynchronizeState.New)
		{
			InsertProcedure(objectRow, owner);
		}
		else
		{
			UpdateProcedure(objectRow, owner);
		}
		if (ProcedureId != 0)
		{
			InsertUpdateParameters(owner);
			RemoveParameterRowsFromDatabase(owner);
		}
	}

	private void InsertUpdateParameters(Form owner = null)
	{
		foreach (ParameterRow item in DataSourceParameterRows.Where((ParameterRow p) => p.RowState == ManagingRowsEnum.ManagingRows.Updated))
		{
			if (item.Id >= 0)
			{
				DB.Parameter.UpdateManualParameter(item, owner);
				customFieldsParametersForHistory.TryGetValue(item.Id, out var value);
				titleAndDescriptionParametersForHistory.TryGetValue(item.Id, out var value2);
				DB.History.InsertHistoryRow(DatabaseId, item.Id, item.Title, item.Description, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Parameter), saveTitle: false, HistoryGeneralHelper.CheckAreValuesDiffrent(value2?.Description, item.Description), SharedObjectTypeEnum.ObjectType.Parameter);
				HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnColumnSummary(item.Id, DatabaseId, item?.CustomFields?.CustomFieldsData, value, SharedObjectTypeEnum.ObjectType.Parameter);
			}
		}
		foreach (ParameterRow item2 in DataSourceParameterRows.Where((ParameterRow p) => p.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
		{
			int? num = DB.Parameter.InsertManualParameter(item2, ProcedureId, StaticData.IsProjectFile, owner);
			DB.History.InsertHistoryRow(DatabaseId, num, item2.Title, item2.Description, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Parameter), saveTitle: false, !string.IsNullOrEmpty(item2.Description), SharedObjectTypeEnum.ObjectType.Parameter);
			HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(num, item2?.CustomFields, DatabaseId, SharedObjectTypeEnum.ObjectType.Parameter);
		}
	}

	public void InsertProcedure(ObjectRow procedureModel, Form owner = null)
	{
		int? num = DB.Procedure.InsertManualProcedure(procedureModel, owner);
		if (num.HasValue)
		{
			ProcedureId = num.Value;
			DB.Community.InsertFollowingToRepository(procedureModel.Type, ProcedureId);
			DB.History.InsertHistoryRow(DatabaseId, ProcedureId, Title, null, null, "procedures", !string.IsNullOrEmpty(Title), saveDescription: false, procedureModel.ObjectTypeValue);
			WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectAdd();
			if (SynchronizeState == SynchronizeStateEnum.SynchronizeState.New)
			{
				SynchronizeState = SynchronizeStateEnum.SynchronizeState.Synchronized;
				DBTreeMenu.AddManualObjectToTree(DatabaseId, ProcedureId, Schema, Name, Title, ObjectTypeValue, Type, Source, null, SynchronizeState);
			}
		}
	}

	public void UpdateProcedure(ObjectRow procedureModel, Form owner = null)
	{
		DB.Procedure.UpdateManualProcedure(ProcedureId, Schema, Name, Title, SharedObjectSubtypeEnum.TypeToString(ObjectTypeValue, Type), Language, Definition, UserTypeEnum.TypeToString(Source), owner);
		DB.Community.InsertFollowingToRepository(ObjectTypeValue, ProcedureId);
		DB.History.InsertHistoryRow(DatabaseId, ProcedureId, Title, null, null, "procedures", HistoryGeneralHelper.CheckAreValuesDiffrent(Title, OldTitle), saveDescription: false, procedureModel.ObjectTypeValue);
		DB.Dependency.UpdateUserDependencies(Schema, Name, procedureModel.DocumentationHost, procedureModel.DatabaseName, OldSchema, OldName, SharedObjectSubtypeEnum.TypeToString(ObjectTypeValue, Type));
		DB.Database.UpdateDocumentationShowSchemaFlag(DatabaseId, owner);
		DBTreeMenu.SetSource(DatabaseId, SharedObjectTypeEnum.ObjectType.Table, OldName, OldSchema, Source);
		DBTreeMenu.RefreshNodeName(DatabaseId, ObjectTypeValue, Name, OldName, Schema, OldSchema, Title, SharedDatabaseTypeEnum.DatabaseType.Manual, withSchema: true, Type);
		WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectEdit();
	}

	public bool Exists()
	{
		return DB.Procedure.CheckIfIsProcedureRecordAlreadyExists(DatabaseId, Name, Schema, ObjectTypeValue, ProcedureId);
	}

	public void Remove(ParameterRow[] rowsToDelete)
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
			DataSourceParameterRows.Remove(rowsToDelete[num]);
			if (!RowsToRemove.Contains(rowsToDelete[num]))
			{
				RowsToRemove.Add(rowsToDelete[num]);
			}
		}
		IsChanged = true;
	}

	public void MoveBefore(ParameterRow row, ParameterRow referenceRow)
	{
		int num = DataSourceParameterRows.IndexOf(referenceRow);
		int num2 = DataSourceParameterRows.IndexOf(row);
		if (num >= 0 && num2 >= 0)
		{
			DataSourceParameterRows.Insert(num, row);
			DataSourceParameterRows.RemoveAt(num2);
		}
	}

	public void MoveAfter(ParameterRow row, ParameterRow referenceRow)
	{
		int num = DataSourceParameterRows.IndexOf(referenceRow);
		int num2 = DataSourceParameterRows.IndexOf(row);
		if (num >= 0 && num2 >= 0)
		{
			DataSourceParameterRows.Insert(num + 1, row);
			DataSourceParameterRows.RemoveAt(num2);
		}
	}

	public void SwapRows(ParameterRow row1, ParameterRow row2)
	{
		int index = DataSourceParameterRows.IndexOf(row2);
		Swap(row1, index);
	}

	public void Swap(ParameterRow row, int index)
	{
		if (index >= 0)
		{
			DataSourceParameterRows.Remove(row);
			DataSourceParameterRows.Insert(index, row);
		}
	}

	public bool HasDuplicatesOrEmptyNames()
	{
		foreach (ParameterRow parameterRow in DataSourceParameterRows.Where((ParameterRow x) => x.Source == UserTypeEnum.UserType.USER))
		{
			parameterRow.AlreadyExists = DataSourceParameterRows.Count((ParameterRow x) => x.Name?.ToLower().Equals(parameterRow.Name?.ToLower()) ?? false) > 1;
			parameterRow.IsNameEmpty = string.IsNullOrEmpty(parameterRow.Name);
		}
		return DataSourceParameterRows.Any((ParameterRow x) => x.AlreadyExists || x.IsNameEmpty);
	}

	private void RemoveParameterRowsFromDatabase(Form owner = null)
	{
		IEnumerable<int> ids = from x in RowsToRemove
			where x.RowState == ManagingRowsEnum.ManagingRows.Deleted
			select x.Id;
		DB.Parameter.Delete(ids, owner);
	}

	public void ModifySource()
	{
		if (Source == UserTypeEnum.UserType.DBMS && HasModifiedSourceProperties())
		{
			Source = UserTypeEnum.UserType.USER;
		}
	}

	public bool HasModifiedSourceProperties()
	{
		if (string.IsNullOrEmpty(OldName) == string.IsNullOrEmpty(Name) && (OldName ?? string.Empty).Equals(Name) && string.IsNullOrEmpty(OldSchema) == string.IsNullOrEmpty(Schema) && (OldSchema ?? string.Empty).Equals(Schema))
		{
			return !OldType.Equals(Type);
		}
		return true;
	}

	public void SaveOldParametersHistory()
	{
		BasicRow[] elements = DataSourceParameterRows?.ToArray();
		HistoryColumnsHelper.SaveColumnsOrParametrsOfOldCustomFields(elements, customFieldsParametersForHistory, titleAndDescriptionParametersForHistory);
	}
}
