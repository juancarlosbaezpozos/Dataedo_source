using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DataProfiling;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Synchronization.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Classes;

public static class CommonFunctionsDatabase
{
	public class ModuleItemModel
	{
		public int ModuleId { get; set; }

		public string ModuleTitle { get; set; }

		public int DatabaseId { get; set; }

		public string DatabaseTitle { get; set; }

		public bool ShowDatabaseTitle { get; set; }

		public string DisplayName
		{
			get
			{
				if (!ShowDatabaseTitle)
				{
					return ModuleTitle;
				}
				return ModuleTitle + " (" + DatabaseTitle + ")";
			}
		}
	}

	public static void SetModuleWithDataSource(RepositoryItemCheckedComboBoxEdit repositoryItemCheckedComboBoxEdit, int databaseId)
	{
		List<DropdownModuleModel> list = new List<DropdownModuleModel>();
		foreach (ModuleWithoutDescriptionObject item in DB.Module.GetDataByDatabaseWithoutDescription(null))
		{
			DropdownModuleModel dropdownModuleModel = new DropdownModuleModel(item);
			dropdownModuleModel.SetDisplayName(databaseId);
			list.Add(dropdownModuleModel);
		}
		repositoryItemCheckedComboBoxEdit.DataSource = list;
		repositoryItemCheckedComboBoxEdit.DropDownRows = list.Count;
	}

	public static void CheckModulesBeforeSaving(int? databaseId, SharedObjectTypeEnum.ObjectType objectType, int objectId, ref ObjectIdName[] checkedObjects, DocumentationModulesUserControl documentationModulesUserControl, SplashScreenManager splashScreenManager, Form owner = null)
	{
		IEnumerable<int> exisitingModulesIds = from x in DB.Module.GetDataByDatabaseWithoutDescription(null)
			select x.ModuleId;
		ObjectIdName[] array = checkedObjects.Where((ObjectIdName x) => !exisitingModulesIds.Any((int y) => y == x.BaseId)).ToArray();
		if (array.Length != 0)
		{
			SetWaitFormVisibility(splashScreenManager, show: false);
			ShowMessageForNotExistingModules(array, owner);
			SetWaitFormVisibility(splashScreenManager, show: true);
			documentationModulesUserControl.GetCurrentRowModulesId(objectType, objectId);
			documentationModulesUserControl.Refresh();
			checkedObjects = checkedObjects.Except(array).ToArray();
		}
	}

	public static void UpdateObjectFromRow(GridView gridView, ModuleWithoutDescriptionObject row, SharedObjectTypeEnum.ObjectType objectType, RepositoryItemCheckedComboBoxEdit modulesRepositoryItemCheckedComboBoxEdit, BaseSummaryUserControl baseSummaryUserControl, Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>> customFieldsCopyForHistory, Form owner = null)
	{
		try
		{
			if (!Licenses.CheckRepositoryVersionAfterLogin() || row == null)
			{
				return;
			}
			string title = PrepareValue.ToString(row.Title);
			int objectId = PrepareValue.ToInt(row.ModuleId).Value;
			_ = PrepareValue.ToInt(row.DatabaseId).Value;
			DatabaseTypeEnum.StringToType(row.DatabaseType);
			bool flag = false;
			IEnumerable<GridColumn> source = gridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field"));
			Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields = source.ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(baseSummaryUserControl.CustomFieldsSupport.GetField(y.FieldName), row.GetField(y.FieldName)?.ToString()));
			bool num = DB.Module.Update(objectId, title, customFields, null, null, "STRAIGHT", erdShowTypes: false, "EXTERNAL_ENTITIES_ONLY", erdShowNullable: false, owner);
			if (flag)
			{
				baseSummaryUserControl.SetParameters();
			}
			if (num)
			{
				CustomFieldContainer customFieldContainer = new CustomFieldContainer(objectType, objectId, baseSummaryUserControl.CustomFieldsSupport);
				customFieldContainer.RetrieveCustomFields(row);
				customFieldContainer.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
				foreach (var item in from x in source
					select new
					{
						Column = x,
						CustomField = (x.Tag as CustomFieldRowExtended)
					} into x
					where x?.CustomField.IsOpenDefinitionType ?? false
					select x)
				{
					item.CustomField.UpdateDefinitionValues(row, item.Column.FieldName);
				}
				new Task(delegate
				{
					DBTreeMenu.RefeshNodeTitle(objectId, title, objectType);
				}).Start();
			}
			if (baseSummaryUserControl.MainControl.ShowProgress && ProgressColumnHelper.IsProgressColumnChanged(gridView.GetSelectedCells(), baseSummaryUserControl.MainControl.ProgressType.ColumnName))
			{
				baseSummaryUserControl.MainControl.RefreshObjectProgress(showWaitForm: false, objectId, objectType);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
	}

	internal static void ProfileSelectedTables(GridView gridView, MetadataEditorUserControl metadataEditorUserControl)
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		int[] selectedRows = gridView.GetSelectedRows();
		IEnumerable<TableRow> enumerable = from x in selectedRows
			select gridView.GetRow(x) as ObjectWithModulesObject into x
			where x != null
			select new TableRow(x);
		if (enumerable == null || !enumerable.Any() || enumerable.Count() != selectedRows.Count())
		{
			return;
		}
		SharedDatabaseTypeEnum.DatabaseType? databaseType = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Database, enumerable.First().DatabaseId).DatabaseType;
		if (DataProfilingUtils.CanViewDataProfilingForms(databaseType, metadataEditorUserControl.FindForm()) && !DataProfilingUtils.CheckIfDataProfilingFormAlreadyOpened(metadataEditorUserControl.FindForm()))
		{
			new DataProfilingForm().SetParameters(enumerable.Select((TableRow x) => new TableSimpleData(x)), enumerable.First().DatabaseId, enumerable.First().DatabaseTitle, metadataEditorUserControl, databaseType);
		}
	}

	internal static void PreviewSampleData(GridView gridView, MetadataEditorUserControl metadataEditorUserControl)
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		if (!(gridView.GetFocusedRow() is ObjectWithModulesObject row))
		{
			return;
		}
		TableRow tableRow = new TableRow(row);
		if (tableRow == null)
		{
			return;
		}
		DBTreeNode dBTreeNode = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Database, tableRow.DatabaseId);
		if (!DataProfilingUtils.CanViewDataProfilingForms(dBTreeNode.DatabaseType, metadataEditorUserControl.FindForm()))
		{
			return;
		}
		using SampleDataForm sampleDataForm = new SampleDataForm();
		sampleDataForm.SetParameters(tableRow.Id, tableRow.Name, tableRow.Schema, tableRow.ObjectType, tableRow.DatabaseId, dBTreeNode.DatabaseType, dBTreeNode.DBMSVersion);
		sampleDataForm.ShowDialog();
	}

	public static string UpdateObjectFromRow(GridView gridView, ObjectWithModulesObject row, SharedObjectTypeEnum.ObjectType objectType, RepositoryItemCheckedComboBoxEdit modulesRepositoryItemCheckedComboBoxEdit, BaseSummaryUserControl baseSummaryUserControl, Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>> customFieldsCopyForHistory, Form owner = null)
	{
		try
		{
			if (Licenses.CheckRepositoryVersionAfterLogin() && row != null)
			{
				string title = row.Title;
				bool flag = false;
				int objectId = row.Id;
				int database_id = row.DatabaseId;
				SharedDatabaseTypeEnum.DatabaseType? databasetType = DatabaseTypeEnum.StringToType(row.DatabaseType);
				List<int> parsedModulesIds = new List<int>();
				new List<int>();
				string name = null;
				string schema = null;
				bool? multipleShemas = null;
				bool flag2 = false;
				if (objectType != SharedObjectTypeEnum.ObjectType.Module)
				{
					SharedObjectSubtypeEnum.StringToType(objectType, row.Subtype);
					name = row.Name;
					schema = row.Schema;
					string modulesId = row.ModulesId;
					multipleShemas = row.DatabaseMultipleSchemas;
					if (objectType == SharedObjectTypeEnum.ObjectType.Table || objectType == SharedObjectTypeEnum.ObjectType.View || objectType == SharedObjectTypeEnum.ObjectType.Structure)
					{
						DB.Module.GetModulesIdByObjectId(objectId, "tables_modules", "table_id");
						parsedModulesIds = GetParsedModulesIds(modulesId);
					}
					else if (objectType == SharedObjectTypeEnum.ObjectType.Procedure || objectType == SharedObjectTypeEnum.ObjectType.Function)
					{
						DB.Module.GetModulesIdByObjectId(objectId, "procedures_modules", "procedure_id");
						parsedModulesIds = GetParsedModulesIds(modulesId);
					}
					List<int?> exisitingModulesIds = DB.Module.GetModulesIdsByDatabase(null);
					if (parsedModulesIds.Any((int x) => !exisitingModulesIds.Any((int? y) => y == x)))
					{
						IEnumerable<ObjectIdName> enumerable = from x in (modulesRepositoryItemCheckedComboBoxEdit?.DataSource as List<DropdownModuleModel>).Select((DropdownModuleModel x) => new ObjectIdName
							{
								BaseId = (PrepareValue.ToInt(x.ModuleId) ?? (-1)),
								Name = PrepareValue.ToString(x.Name)
							}).ToList()
							where parsedModulesIds.Any((int y) => y == x.BaseId)
							select x;
						ObjectIdName[] array = enumerable.Where((ObjectIdName x) => !exisitingModulesIds.Any((int? y) => y == x.BaseId)).ToArray();
						ShowMessageForNotExistingModules(array, gridView?.GridControl?.FindForm());
						parsedModulesIds = (from x in enumerable.Except(array)
							select x.BaseId).ToList();
						flag2 = true;
					}
				}
				IEnumerable<GridColumn> source = gridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field"));
				Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields = source.ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(baseSummaryUserControl.CustomFieldsSupport.GetField(y.FieldName), row.GetField(y.FieldName)?.ToString()));
				switch (objectType)
				{
				case SharedObjectTypeEnum.ObjectType.Function:
				case SharedObjectTypeEnum.ObjectType.Procedure:
					flag = DB.Procedure.Update(database_id, objectId, objectType, title, parsedModulesIds, customFields, null, null, owner);
					break;
				case SharedObjectTypeEnum.ObjectType.Table:
				case SharedObjectTypeEnum.ObjectType.View:
				case SharedObjectTypeEnum.ObjectType.Structure:
					flag = DB.Table.Update(database_id, objectId, objectType, title, row.Location, parsedModulesIds, customFields, null, null, owner);
					break;
				case SharedObjectTypeEnum.ObjectType.Module:
					flag = DB.Module.Update(objectId, title, customFields, null, null, "STRAIGHT", erdShowTypes: false, "EXTERNAL_ENTITIES_ONLY", erdShowNullable: false, owner);
					break;
				}
				if (flag2)
				{
					baseSummaryUserControl.SetParameters();
				}
				if (flag)
				{
					CustomFieldContainer customFieldContainer = new CustomFieldContainer(objectType, objectId, baseSummaryUserControl.CustomFieldsSupport);
					customFieldContainer.RetrieveCustomFields(row);
					customFieldContainer.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
					foreach (var item in from x in source
						select new
						{
							Column = x,
							CustomField = (x.Tag as CustomFieldRowExtended)
						} into x
						where x?.CustomField.IsOpenDefinitionType ?? false
						select x)
					{
						item.CustomField.UpdateDefinitionValues(row, item.Column.FieldName);
					}
					new Task(delegate
					{
						if (objectType == SharedObjectTypeEnum.ObjectType.Module)
						{
							DBTreeMenu.RefeshNodeTitle(objectId, title, objectType);
						}
						else
						{
							DBTreeMenu.RefeshNodeTitle(database_id, title, objectType, name, schema, databasetType, multipleShemas.GetValueOrDefault());
						}
					}).Start();
				}
				if (baseSummaryUserControl.MainControl.ShowProgress && ProgressColumnHelper.IsProgressColumnChanged(gridView.GetSelectedCells(), baseSummaryUserControl.MainControl.ProgressType.ColumnName))
				{
					baseSummaryUserControl.MainControl.RefreshObjectProgress(showWaitForm: false, objectId, objectType);
				}
				return (parsedModulesIds != null) ? string.Join(",", parsedModulesIds) : null;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, gridView?.GridControl?.FindForm());
		}
		return null;
	}

	public static Dictionary<int, int> GetModulesAndDatabaseDictionary(IEnumerable<int> modulesId)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (int item in modulesId)
		{
			DBTreeNode dBTreeNode = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Module, item);
			dictionary.Add(item, dBTreeNode.DatabaseId);
		}
		return dictionary;
	}

	public static void ManageObjectInTree(int selectedObjectDatabaseId, IEnumerable<int> modulesIds, IEnumerable<int> oldModules, int objectId, string schema, string name, string title, UserTypeEnum.UserType source, SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subtype, MetadataEditorUserControl mainControl)
	{
		IEnumerable<int> newModules = modulesIds.Where((int x) => !oldModules.Contains(x));
		IEnumerable<int> removedFrom = oldModules.Where((int x) => !modulesIds.Contains(x));
		AddObjectsToTree(selectedObjectDatabaseId, objectId, schema, name, title, source, type, subtype, newModules);
		RemoveObjectFromModules(objectId, type, mainControl, removedFrom);
	}

	private static void AddObjectsToTree(int selectedObjectDatabaseId, int objectId, string schema, string name, string title, UserTypeEnum.UserType source, SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subtype, IEnumerable<int> newModules)
	{
		foreach (KeyValuePair<int, int> item in GetModulesAndDatabaseDictionary(newModules))
		{
			DBTreeMenu.AddObjectToModule(item.Value, objectId, item.Key, schema, name, title, source, type, subtype, selectedObjectDatabaseId);
		}
	}

	public static void RemoveObjectFromModules(int objectId, SharedObjectTypeEnum.ObjectType type, MetadataEditorUserControl mainControl, IEnumerable<int> removedFrom)
	{
		DBTreeNode dbNode = DBTreeMenu.FindDBNodeById(type, objectId);
		foreach (KeyValuePair<int, int> item in GetModulesAndDatabaseDictionary(removedFrom))
		{
			mainControl.RemoveFromUncheckedModule(item, dbNode);
		}
	}

	private static List<int> GetParsedModulesIds(string modulesIds)
	{
		string[] array = modulesIds?.Split(',');
		List<int> list = new List<int>();
		if (array != null)
		{
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (int.TryParse(array2[i], out var result))
				{
					list.Add(result);
				}
			}
		}
		return list;
	}

	private static void ShowMessageForNotExistingModules(ObjectIdName[] checkedNotExistingModules, Form owner = null)
	{
		if (checkedNotExistingModules.Length == 1)
		{
			GeneralMessageBoxesHandling.Show("Selected Subject Area " + checkedNotExistingModules[0].Name + " does not exist.", "Subject Area does not exist", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		else if (checkedNotExistingModules.Length > 1)
		{
			string text = string.Join(", ", checkedNotExistingModules.Select((ObjectIdName x) => x.Name));
			GeneralMessageBoxesHandling.Show("Selected Subject Areas " + text + " do not exist.", "Subject Areas do not exist", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
	}

	public static bool UpdateParameters(GridControl tableGrid, BindingList<int> deletedParameters, int? databaseId, Dictionary<int, Dictionary<string, string>> customFieldsColumnForHistory, Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory)
	{
		DB.Parameter.Delete(deletedParameters, tableGrid?.FindForm());
		if (tableGrid.DataSource != null)
		{
			List<ParameterRow> list = (tableGrid.DataSource as BindingList<ParameterRow>).Where((ParameterRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Unchanged).ToList();
			list.ForEach(delegate(ParameterRow x)
			{
				DB.Parameter.Update(x, tableGrid?.FindForm());
				x.SetUnchanged();
			});
			HistoryColumnsHelper.SaveTitleDescriptionCustomFieldsHistoryInUpdateColumns(customFieldsColumnForHistory, titleAndDescriptionParametersForHistory, list, SharedObjectTypeEnum.ObjectType.Parameter, databaseId);
			CustomFieldContainer.UpdateDefinitionValues(list.SelectMany((ParameterRow x) => x.CustomFields.CustomFieldsData));
			deletedParameters.Clear();
		}
		return true;
	}

	public static bool IsDeletedFromDB(object row)
	{
		if (row is BasicRow)
		{
			return IsDeletedFromDB(row as BasicRow);
		}
		if (row is IStatus)
		{
			return IsDeletedFromDB(row as IStatus);
		}
		return false;
	}

	private static bool IsDeletedFromDB(BasicRow row)
	{
		if (row == null)
		{
			return false;
		}
		return row.Status == SynchronizeStateEnum.SynchronizeState.Deleted;
	}

	private static bool IsDeletedFromDB(IStatus row)
	{
		if (row == null)
		{
			return false;
		}
		return SynchronizeStateEnum.DBStringToState(row.Status) == SynchronizeStateEnum.SynchronizeState.Deleted;
	}

	public static int[] GetDeletedRows(GridView gridView, bool isObject)
	{
		int[] selectedRows = gridView.GetSelectedRows();
		List<int> list = new List<int>();
		for (int i = 0; i < selectedRows.Count(); i++)
		{
			if (isObject)
			{
				if ((gridView.GetRow(selectedRows[i]) as BasicRow).CanBeDeleted())
				{
					list.Add(selectedRows[i]);
				}
			}
			else if (IsDeletedFromDB(gridView.GetRow(selectedRows[i])))
			{
				list.Add(selectedRows[i]);
			}
		}
		return list.ToArray();
	}

	public static bool AreRowsToDelete(GridView gridView, bool isObject)
	{
		int[] selectedRows = gridView.GetSelectedRows();
		for (int i = 0; i < selectedRows.Count(); i++)
		{
			gridView.GetRow(selectedRows[i]);
			if (isObject)
			{
				if ((gridView.GetRow(selectedRows[i]) as BasicRow).CanBeDeleted())
				{
					return true;
				}
			}
			else if (IsDeletedFromDB(gridView.GetRow(selectedRows[i])))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AreDeletableRows(GridView gridView, bool isObject)
	{
		int[] selectedRows = gridView.GetSelectedRows();
		for (int i = 0; i < selectedRows.Count(); i++)
		{
			gridView.GetRow(selectedRows[i]);
			if (isObject)
			{
				if ((gridView.GetRow(selectedRows[i]) as BasicRow).IsDeletable())
				{
					return true;
				}
			}
			else if (IsDeletedFromDB(gridView.GetRow(selectedRows[i])))
			{
				return true;
			}
		}
		return false;
	}

	public static void AddToIgnored(DBTreeNode dbTreeNode, Form owner = null)
	{
		if (dbTreeNode.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted && (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View || dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Function || dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure))
		{
			DB.IgnoredObjects.Insert(ConvertingToTables.ReloadIgnoredObjectsTable(dbTreeNode.DatabaseId, dbTreeNode.Schema, dbTreeNode.BaseName, SharedObjectTypeEnum.TypeToString(dbTreeNode.ObjectType)), owner);
		}
	}

	public static bool DeleteObjectsFromDatabase(SharedObjectTypeEnum.ObjectType? objectType, List<int> ids, Form owner = null)
	{
		if (ids == null || ids.Count == 0)
		{
			return true;
		}
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Database:
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			return DB.Database.Delete(ids.FirstOrDefault(), owner);
		case SharedObjectTypeEnum.ObjectType.Table:
			return DB.Table.Delete(ids, SharedObjectTypeEnum.ObjectType.Table, owner);
		case SharedObjectTypeEnum.ObjectType.View:
			return DB.Table.Delete(ids, SharedObjectTypeEnum.ObjectType.View, owner);
		case SharedObjectTypeEnum.ObjectType.Procedure:
			return DB.Procedure.Delete(ids, SharedObjectTypeEnum.ObjectType.Procedure, owner);
		case SharedObjectTypeEnum.ObjectType.Function:
			return DB.Procedure.Delete(ids, SharedObjectTypeEnum.ObjectType.Function, owner);
		case SharedObjectTypeEnum.ObjectType.Structure:
			return DB.Table.Delete(ids, SharedObjectTypeEnum.ObjectType.Structure, owner);
		case SharedObjectTypeEnum.ObjectType.Module:
			return DB.Module.Delete(ids, owner);
		default:
			return false;
		}
	}

	private static void CreateAskMessageForSingleObject(StringBuilder message, SharedObjectTypeEnum.ObjectType objectType, string name, ObjectEventArgs objectEventArgs, bool fromERD = false, UserTypeEnum.UserType source = UserTypeEnum.UserType.DBMS, bool addTypeInformation = true, bool isNameFormatted = false, bool addSourceInformation = true)
	{
		if (addTypeInformation)
		{
			message.Append("Are you sure you want to remove ").Append(SharedObjectTypeEnum.TypeToStringForSingleLower(objectType));
		}
		else
		{
			message.Append("Are you sure you want to remove");
		}
		if (!string.IsNullOrEmpty(name))
		{
			if (!isNameFormatted)
			{
				message.Append(" <b>").Append(name).Append("</b>");
			}
			else
			{
				message.Append(" " + name);
			}
		}
		if (objectEventArgs == null || !objectEventArgs.ModuleId.HasValue)
		{
			if (fromERD)
			{
				if (addSourceInformation)
				{
					message.Append(" from diagram?");
				}
				else
				{
					message.Append("?");
				}
				return;
			}
			if (addSourceInformation)
			{
				message.Append(" from repository?");
			}
			else
			{
				message.Append("?");
			}
			message.Append(Environment.NewLine).Append(Environment.NewLine);
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Module:
				message.Append("Subject Area description and object linking would be lost. Linked objects would remain.");
				break;
			case SharedObjectTypeEnum.ObjectType.Database:
				message.Append("All content would be lost.");
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				message.Append("All content would be lost. ");
				if (source == UserTypeEnum.UserType.DBMS)
				{
					message.Append(SharedObjectTypeEnum.TypeToStringForSingle(objectType));
					message.Append(" would be ignored from future database imports.");
				}
				break;
			default:
				message.Append("All content would be lost. ");
				break;
			case SharedObjectTypeEnum.ObjectType.Dependency:
			case SharedObjectTypeEnum.ObjectType.TermRelationship:
				break;
			}
		}
		else
		{
			message.Append(" from subject area <b>").Append(objectEventArgs.ModuleName).Append("</b>?");
		}
	}

	private static void CreateAskMessageForMultipleObjects(StringBuilder message, int[] selectedRowHandle, ColumnView gridView, SharedObjectTypeEnum.ObjectType objectType, ObjectEventArgs objectEventArgs, bool addTypeInformation = true, bool addSourceInformation = true)
	{
		message.Append("Are you sure you want to remove ").Append("<b>").Append(selectedRowHandle.Count())
			.Append(" ")
			.Append(SharedObjectTypeEnum.TypeToStringForSingleLower(objectType))
			.Append("s</b>");
		if (objectEventArgs == null || !objectEventArgs.ModuleId.HasValue)
		{
			if (addSourceInformation)
			{
				message.Append(" from repository?");
			}
			else
			{
				message.Append("?");
			}
			message.Append(Environment.NewLine).Append(Environment.NewLine);
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Module:
				message.Append("Subject Area description and object linking would be lost. Linked objects would remain.");
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
				message.Append("All content would be lost. ").Append(SharedObjectTypeEnum.TypeToStringForMenu(objectType)).Append(" would be ignored from future database imports.");
				break;
			default:
				message.Append("All content would be lost. ");
				break;
			case SharedObjectTypeEnum.ObjectType.TermRelationship:
				break;
			}
		}
		else if (addSourceInformation)
		{
			message.Append(" from subject area <b>").Append(objectEventArgs.ModuleName).Append("</b>?");
		}
		else
		{
			message.Append("?");
		}
	}

	private static void CreateAskMessageForMultipleNodes(StringBuilder message, int nodesCount)
	{
		message.Append($"Are you sure you want to remove <b>{nodesCount} objects</b> from diagram?");
	}

	public static bool AskIfDeleting(int[] selectedRowHandle, ColumnView gridView, SharedObjectTypeEnum.ObjectType objectType, ObjectEventArgs objectEventArgs = null, bool? contextShowSchema = null)
	{
		try
		{
			int num = selectedRowHandle.Count();
			if (!Licenses.CheckRepositoryVersionAfterLogin())
			{
				return false;
			}
			if (num == 0)
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (num == 1)
			{
				if (selectedRowHandle == null)
				{
					return false;
				}
				if (gridView == null)
				{
					return false;
				}
				int rowHandle = selectedRowHandle[0];
				object row = gridView.GetRow(rowHandle);
				if (objectType == SharedObjectTypeEnum.ObjectType.DataLink)
				{
					DataLinkObjectExtended dataLinkObjectExtended = row as DataLinkObjectExtended;
					string text = dataLinkObjectExtended.ElementType ?? dataLinkObjectExtended.ObjectType;
					string name = "between " + text.ToLower() + " <b>" + dataLinkObjectExtended.FullNameWithTitleForObject + "</b> and <b>" + dataLinkObjectExtended.TermTitle + "</b> " + dataLinkObjectExtended.TermType.ToLower();
					UserTypeEnum.UserType source = UserTypeEnum.ObjectToType(dataLinkObjectExtended.ObjectSource) ?? UserTypeEnum.UserType.DBMS;
					CreateAskMessageForSingleObject(stringBuilder, objectType, name, objectEventArgs, fromERD: false, source, addTypeInformation: true, isNameFormatted: true);
				}
				else if (row is IBasicDataExtended)
				{
					IBasicDataExtended obj = row as IBasicDataExtended;
					string name = obj.NameFormatted;
					UserTypeEnum.UserType source = UserTypeEnum.ObjectToType(obj.Source) ?? UserTypeEnum.UserType.DBMS;
					string name2 = name;
					UserTypeEnum.UserType source2 = source;
					bool isNameFormatted = objectType == SharedObjectTypeEnum.ObjectType.TermRelationship;
					CreateAskMessageForSingleObject(stringBuilder, objectType, name2, objectEventArgs, fromERD: false, source2, objectType != SharedObjectTypeEnum.ObjectType.TermRelationship, isNameFormatted, objectType != SharedObjectTypeEnum.ObjectType.TermRelationship);
				}
				else if (row is BaseDataObjectWithCustomFields)
				{
					BaseDataObjectWithCustomFields baseDataObjectWithCustomFields = row as BaseDataObjectWithCustomFields;
					string name = ((!string.IsNullOrEmpty(baseDataObjectWithCustomFields.Schema) && (contextShowSchema == true || DatabaseRow.GetShowSchema(baseDataObjectWithCustomFields.DatabaseDatabaseShowSchema, baseDataObjectWithCustomFields.DatabaseShowSchemaOverride))) ? (baseDataObjectWithCustomFields.Schema + "." + baseDataObjectWithCustomFields.Name) : baseDataObjectWithCustomFields.Name);
					UserTypeEnum.UserType source = UserTypeEnum.ObjectToType(baseDataObjectWithCustomFields.Source) ?? UserTypeEnum.UserType.DBMS;
					CreateAskMessageForSingleObject(stringBuilder, objectType, name, objectEventArgs, fromERD: false, source);
				}
				else if (row is INameSource)
				{
					INameSource obj2 = row as INameSource;
					string name = obj2.Name;
					UserTypeEnum.UserType source = UserTypeEnum.ObjectToType(obj2.Source) ?? UserTypeEnum.UserType.DBMS;
					CreateAskMessageForSingleObject(stringBuilder, objectType, name, objectEventArgs, fromERD: false, source);
				}
			}
			else
			{
				CreateAskMessageForMultipleObjects(stringBuilder, selectedRowHandle, gridView, objectType, objectEventArgs, objectType != SharedObjectTypeEnum.ObjectType.TermRelationship, objectType != SharedObjectTypeEnum.ObjectType.TermRelationship);
			}
			string text2 = "Delete";
			if (objectType == SharedObjectTypeEnum.ObjectType.TermRelationship)
			{
				text2 = ((num != 1) ? (text2 + " relationships") : (text2 + " relationship"));
			}
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(stringBuilder.ToString(), text2, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, null, 2, gridView.GridControl.FindForm());
			return handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.OK;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, gridView?.GridControl?.FindForm());
			return false;
		}
	}

	private static bool AskIfDeleting(DBTreeNode dbTreeNode, bool fromSubjectArea, Form owner = null)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin())
		{
			return false;
		}
		int moduleId = 0;
		string moduleName = string.Empty;
		if (fromSubjectArea)
		{
			DBTreeNode parentNode = dbTreeNode.ParentNode.ParentNode;
			moduleId = parentNode.Id;
			moduleName = parentNode.Name;
		}
		return AskIfDeleting(moduleId, moduleName, dbTreeNode.ObjectType, dbTreeNode.Name, dbTreeNode.TreeDisplayName, dbTreeNode.Source, fromSubjectArea, owner);
	}

	public static bool AskIfDeleting(int moduleId, string moduleName, SharedObjectTypeEnum.ObjectType objectType, string objectName, string objectDisplayName, UserTypeEnum.UserType? source, bool fromModule, Form owner = null)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin())
		{
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		ObjectEventArgs objectEventArgs = null;
		if (fromModule)
		{
			objectEventArgs = new ObjectEventArgs
			{
				ModuleId = moduleId,
				ModuleName = moduleName
			};
		}
		CreateAskMessageForSingleObject(stringBuilder, objectType, (objectType == SharedObjectTypeEnum.ObjectType.Module || objectType == SharedObjectTypeEnum.ObjectType.Database || objectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary || objectType == SharedObjectTypeEnum.ObjectType.Term) ? objectName : objectDisplayName, objectEventArgs, fromERD: false, source ?? UserTypeEnum.UserType.DBMS);
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(stringBuilder.ToString(), "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, null, 2, owner);
		if (handlingDialogResult == null)
		{
			return false;
		}
		return handlingDialogResult.DialogResult == DialogResult.OK;
	}

	public static bool AskIfDeleting(Link link, Form owner = null)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin())
		{
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		ObjectEventArgs objectEventArgs = null;
		CreateAskMessageForSingleObject(stringBuilder, SharedObjectTypeEnum.ObjectType.Relation, link.Name, objectEventArgs);
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(stringBuilder.ToString(), "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, null, 2, owner);
		if (handlingDialogResult == null)
		{
			return false;
		}
		return handlingDialogResult.DialogResult == DialogResult.OK;
	}

	public static bool AskIfDeleting(Node node, bool fromSubjectArea, string moduleName = null, Form owner = null)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin() || node == null)
		{
			return false;
		}
		if (fromSubjectArea)
		{
			return AskIfDeleting(node.SubjectAreaId, moduleName, node.ObjectType, node.DisplayName, node.DisplayName, node.TableSource, fromSubjectArea, owner);
		}
		StringBuilder stringBuilder = new StringBuilder();
		ObjectEventArgs objectEventArgs = null;
		SharedObjectTypeEnum.ObjectType objectType = ObjectTypeEnum.NodeTypeToObjectType(node.Type);
		CreateAskMessageForSingleObject(stringBuilder, objectType, node.DisplayName, objectEventArgs, fromERD: true);
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(stringBuilder.ToString(), "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, null, 2, owner);
		if (handlingDialogResult == null)
		{
			return false;
		}
		return handlingDialogResult.DialogResult == DialogResult.OK;
	}

	public static bool AskIfDeleting(Dataedo.App.Data.MetadataServer.Model.DependencyRow dependency, Form owner = null)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin() || dependency == null)
		{
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		ObjectEventArgs objectEventArgs = null;
		if (dependency.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation)
		{
			stringBuilder.Append("Are you sure want to remove relationship with <b>" + dependency.DisplayName + "</b> from repository?").Append(Environment.NewLine).Append(Environment.NewLine)
				.Append("All content would be lost.");
		}
		else
		{
			CreateAskMessageForSingleObject(stringBuilder, SharedObjectTypeEnum.ObjectType.Dependency, dependency.DisplayName, objectEventArgs);
		}
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(stringBuilder.ToString(), "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, null, 2, owner);
		if (handlingDialogResult == null)
		{
			return false;
		}
		return handlingDialogResult.DialogResult == DialogResult.OK;
	}

	public static bool AskIfDeleting(int nodesCount, Form owner = null)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin())
		{
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		CreateAskMessageForMultipleNodes(stringBuilder, nodesCount);
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(stringBuilder.ToString(), "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, null, 2, owner);
		if (handlingDialogResult == null)
		{
			return false;
		}
		return handlingDialogResult.DialogResult == DialogResult.OK;
	}

	public static bool DeleteObject(SplashScreenManager splashScreenManager, DBTreeNode dbTreeNode, Form owner = null)
	{
		if (!AskIfDeleting(dbTreeNode, fromSubjectArea: false, owner))
		{
			return false;
		}
		try
		{
			SetWaitFormVisibility(splashScreenManager, show: true);
			SharedObjectTypeEnum.ObjectType objectType = dbTreeNode.ObjectType;
			bool flag = ((objectType == SharedObjectTypeEnum.ObjectType.Term) ? DB.BusinessGlossary.Delete(new Term[1]
			{
				new Term
				{
					Id = dbTreeNode.Id,
					ParentId = dbTreeNode.ParentNode?.Id,
					DatabaseId = dbTreeNode.DatabaseId
				}
			}, owner) : DeleteObjectsFromDatabase(objectType, new List<int> { dbTreeNode.Id }, owner));
			if (flag)
			{
				if (dbTreeNode.Source != UserTypeEnum.UserType.USER)
				{
					AddToIgnored(dbTreeNode, owner);
					return flag;
				}
				return flag;
			}
			return flag;
		}
		finally
		{
			SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public static bool DeleteObjectFromModule(SplashScreenManager splashScreenManager, DBTreeNode dbTreeNode, Form owner = null)
	{
		bool result = false;
		if (AskIfDeleting(dbTreeNode, fromSubjectArea: true, owner))
		{
			try
			{
				SetWaitFormVisibility(splashScreenManager, show: true);
				return DB.Module.DeleteLinks(dbTreeNode, owner);
			}
			finally
			{
				SetWaitFormVisibility(splashScreenManager, show: false);
			}
		}
		return result;
	}

	public static void DeleteSelectedObjectsDB(GridView gridView, SharedObjectTypeEnum.ObjectType objectType, ObjectEventArgs objectEventArgs, MetadataEditorUserControl mainControl, BaseSummaryUserControl baseSummaryUserControl, Form owner = null)
	{
		bool flag = !objectEventArgs.ModuleId.HasValue;
		int[] selectedRows = gridView.GetSelectedRows();
		if (AskIfDeleting(selectedRows, gridView, objectType, objectEventArgs, baseSummaryUserControl.ContextShowSchema))
		{
			SetWaitFormVisibility(mainControl.SplashScreenManager, show: true);
			mainControl.MetadataTreeList.BeginUpdate();
			if (flag)
			{
				DeleteSelectedObjectsFromDatabase(gridView, objectType, mainControl, selectedRows, owner);
			}
			else
			{
				DeleteSelectedObjectsFromModule(gridView, objectType, objectEventArgs, selectedRows, owner);
			}
			mainControl.MetadataTreeList.EndUpdate();
			SetWaitFormVisibility(mainControl.SplashScreenManager, show: false);
		}
	}

	public static void DeleteSelectedObjectsFromDatabase(GridView gridView, SharedObjectTypeEnum.ObjectType objectType, MetadataEditorUserControl mainControl, int[] selectedRowHandle, Form owner = null)
	{
		object[] source = selectedRowHandle.Select((int x) => gridView.GetRow(x)).ToArray();
		ObjectWithModulesObject[] array = (from x in source
			where x is ObjectWithModulesObject
			select x as ObjectWithModulesObject).ToArray();
		List<int> list = array.Select((ObjectWithModulesObject x) => x.Id).ToList();
		HashSet<int> hashSet = array.Select((ObjectWithModulesObject x) => x.DatabaseId).ToHashSet();
		if (list.Count > 0 && DeleteObjectsFromDatabase(objectType, list, gridView?.GridControl?.FindForm()))
		{
			gridView.DeleteSelectedRows();
			ObjectWithModulesObject[] array2 = array;
			foreach (ObjectWithModulesObject objectWithModulesObject in array2)
			{
				DBTreeNode dBTreeNode = DBTreeMenu.FindDBNodeById(objectType, objectWithModulesObject.Id);
				if (dBTreeNode != null)
				{
					AddToIgnored(dBTreeNode, owner);
					DBTreeNode node = DBTreeMenu.FindMainObjectsFolder(dBTreeNode);
					DBTreeMenu.DeleteNode(dBTreeNode);
					(gridView.DataSource as List<ObjectWithModulesObject>).Remove(objectWithModulesObject);
					mainControl.RemoveFromModulesInTree(dBTreeNode);
					DBTreeMenu.RefreshFolderInDataBase(node);
				}
			}
		}
		IId[] array3 = (from x in source
			where !(x is ObjectWithModulesObject) && x is IId
			select x as IId).ToArray();
		List<int> list2 = array3.Select((IId x) => x.Id).ToList();
		bool flag;
		if (objectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			IEnumerable<Term> source2 = array3.Select((IId x) => new Term
			{
				Id = x.Id,
				ParentId = DBTreeMenu.FindDBNodeById(objectType, x.Id)?.ParentNode?.Id
			});
			flag = DB.BusinessGlossary.Delete(source2.ToArray(), owner);
		}
		else
		{
			flag = DeleteObjectsFromDatabase(objectType, list2, gridView?.GridControl?.FindForm());
		}
		if (list2.Count > 0 && flag)
		{
			gridView.DeleteSelectedRows();
			IId[] array4 = array3;
			foreach (IId id in array4)
			{
				DBTreeNode dBTreeNode2 = DBTreeMenu.FindDBNodeById(objectType, id.Id);
				if (dBTreeNode2 == null)
				{
					continue;
				}
				AddToIgnored(dBTreeNode2, owner);
				DBTreeNode node2 = DBTreeMenu.FindMainObjectsFolder(dBTreeNode2);
				DBTreeMenu.DeleteNode(dBTreeNode2);
				if (id is TermObject && dBTreeNode2.Nodes.Count > 0)
				{
					foreach (DBTreeNode node3 in dBTreeNode2.Nodes)
					{
						dBTreeNode2.ParentNode.Nodes.Add(node3);
						node3.ParentNode = dBTreeNode2.ParentNode;
					}
				}
				mainControl.TreeListHelpers.AggregateProgressUpIfProgressVisible(dBTreeNode2.ParentNode);
				mainControl.TreeListHelpers.AggregateProgressDownIfProgressVisible(dBTreeNode2.ParentNode);
				DBTreeMenu.RefreshFolderInDataBase(node2);
			}
		}
		foreach (int item in hashSet)
		{
			DB.Database.UpdateDocumentationShowSchemaFlag(item, owner);
		}
	}

	public static void DeleteSelectedObjectsFromModule(GridView gridView, SharedObjectTypeEnum.ObjectType objectType, ObjectEventArgs objectEventArgs, int[] selectedRowHandle, Form owner = null)
	{
		DB.Module.DeleteLinks(objectEventArgs.ModuleId, gridView, objectType, out var succeedObjectsIds);
		int objectId;
		for (int num = selectedRowHandle.Count() - 1; num >= 0; num--)
		{
			int rowHandle = selectedRowHandle[num];
			ObjectWithModulesObject objectWithModulesObject = gridView.GetRow(rowHandle) as ObjectWithModulesObject;
			objectId = objectWithModulesObject.Id;
			if (succeedObjectsIds.Where((int x) => x == objectId).Count() > 0)
			{
				gridView.DeleteRow(rowHandle);
				DBTreeNode dBTreeNode = DBTreeMenu.FindDBNodeByIdInModule(objectType, objectId, objectEventArgs.ModuleId);
				DBTreeNode node = DBTreeMenu.FindMainObjectsFolder(dBTreeNode);
				DBTreeMenu.DeleteNode(dBTreeNode);
				DBTreeMenu.RefreshModuleFolderInDataBase(node);
			}
		}
	}

	public static void RemoveObjectFromModule(int databaseId, int tableId, int moduleId, SharedObjectTypeEnum.ObjectType type)
	{
		DBTreeNode dBTreeNode = DBTreeMenu.FindModuleFolder(databaseId, type, moduleId);
		if (dBTreeNode != null)
		{
			DBTreeNode dBTreeNode2 = DBTreeMenu.FindDBNodeByIdInModule(type, tableId, moduleId);
			if (dBTreeNode2 != null)
			{
				DB.Module.DeleteLinks(dBTreeNode2);
				DBTreeMenu.DeleteNode(dBTreeNode2);
				DBTreeMenu.RefreshModuleFolderInDataBase(dBTreeNode);
			}
		}
	}

	public static void AddNewModule(GridView gridView, int databaseId, DBTreeNode modulesFolderNode, int? newModulePosition, CustomFieldsSupport customFieldsSupport, Form owner)
	{
		if (!(gridView?.DataSource is List<ModuleWithoutDescriptionObject> list))
		{
			return;
		}
		string text = TextInputForm.ShowForm("Add Subject Area", "Title:", null, 250, "OK", "Cancel", owner);
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		int num = DB.Module.Insert(databaseId, text, null, null, "STRAIGHT", erdShowTypes: false, "EXTERNAL_ENTITIES_ONLY", erdShowNullable: false, owner);
		if (num >= 0)
		{
			DB.History.InsertHistoryRow(databaseId, num, text, null, null, "modules", saveTitle: true, saveDescription: false, SharedObjectTypeEnum.ObjectType.Module);
			DB.Module.UpdateOrdinalPosition(num, newModulePosition ?? (list.Count + 1), owner);
			ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = new ModuleWithoutDescriptionObject
			{
				DatabaseId = databaseId,
				ModuleId = num,
				Id = num,
				Title = text
			};
			HistoryModulesHelper.SetHistoryOfNewModuleAddedToSummary(gridView, customFieldsSupport, text, num, moduleWithoutDescriptionObject);
			moduleWithoutDescriptionObject.Initialize();
			if (newModulePosition.HasValue && newModulePosition.Value < list.Count)
			{
				list.Insert(newModulePosition.Value, moduleWithoutDescriptionObject);
			}
			else
			{
				list.Add(moduleWithoutDescriptionObject);
			}
			WorkWithDataedoTrackingHelper.TrackNewSubjectAreaAdd();
			gridView.RefreshData();
			DBTreeMenu.ReloadModules(modulesFolderNode);
		}
	}

	public static void AddNewTable(GridView gridView, int databaseId, DBTreeNode tablesFolderNode, CustomFieldsSupport customFieldsSupport)
	{
		DesignTableForm designTableForm = new DesignTableForm(null, databaseId, tablesFolderNode.ContainedObjectsObjectType, customFieldsSupport);
		designTableForm.ShowDialog();
		if (designTableForm.DialogResult == DialogResult.OK)
		{
			ObjectWithModulesObject objectWithModulesObject = new ObjectWithModulesObject();
			objectWithModulesObject.Name = designTableForm.TableDesigner.Name;
			objectWithModulesObject.Schema = designTableForm.TableDesigner.Schema;
			objectWithModulesObject.Id = designTableForm.TableDesigner.TableId;
			objectWithModulesObject.DatabaseId = designTableForm.TableDesigner.DatabaseId;
			objectWithModulesObject.Source = "USER";
			objectWithModulesObject.Title = designTableForm.TableDesigner.Title;
			objectWithModulesObject.SubtypeDisplayText = SharedObjectSubtypeEnum.TypeToStringForSingle(tablesFolderNode.ContainedObjectsObjectType, designTableForm.TableDesigner.Type);
			objectWithModulesObject.ObjectType = SharedObjectTypeEnum.TypeToString(tablesFolderNode.ContainedObjectsObjectType);
			objectWithModulesObject.Subtype = SharedObjectSubtypeEnum.TypeToString(tablesFolderNode.ContainedObjectsObjectType, designTableForm.TableDesigner.Type);
			objectWithModulesObject.DatabaseTitle = designTableForm.TableDesigner.DocumentationTitle;
			objectWithModulesObject.Initialize();
			if (tablesFolderNode != null && tablesFolderNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
			{
				DB.Module.InsertManualTable(tablesFolderNode.ParentNode.Id, designTableForm.TableDesigner.TableId);
				DBTreeMenu.AddManualObjectToTree(designTableForm.TableDesigner.DatabaseId, designTableForm.TableDesigner.TableId, designTableForm.TableDesigner.Schema, designTableForm.TableDesigner.Name, designTableForm.TableDesigner.Title, designTableForm.TableDesigner.ObjectTypeValue, designTableForm.TableDesigner.Type, designTableForm.TableDesigner.Source, tablesFolderNode);
			}
			List<ObjectWithModulesObject> list = gridView.DataSource as List<ObjectWithModulesObject>;
			list.Add(objectWithModulesObject);
			gridView.GridControl.DataSource = (from x in list
				orderby x.Schema, x.Name
				select x).ToList();
			gridView.RefreshData();
		}
	}

	public static void DesignTable(GridView gridView, List<ObjectWithModulesObject> dataSource, CustomFieldsSupport customFieldsSupport)
	{
		if (!(gridView.GetFocusedRow() is ObjectWithModulesObject row))
		{
			return;
		}
		TableRow tableRow = new TableRow(row);
		if (tableRow != null)
		{
			string name = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Database, tableRow.DatabaseId).Name;
			DesignTableForm designTableForm = new DesignTableForm(tableRow.DatabaseId, tableRow.Id, tableRow.Schema, tableRow.Name, tableRow.Title, tableRow.Source, tableRow.ObjectType, tableRow.Subtype, name, customFieldsSupport);
			designTableForm.ShowDialog();
			if (designTableForm.DialogResult == DialogResult.OK)
			{
				ObjectWithModulesObject obj = gridView.GetFocusedRow() as ObjectWithModulesObject;
				obj.Name = designTableForm.TableDesigner.Name;
				obj.Schema = designTableForm.TableDesigner.Schema;
				obj.Title = designTableForm.TableDesigner.Title;
				obj.Source = UserTypeEnum.TypeToString(designTableForm.TableDesigner.Source);
				obj.Subtype = SharedObjectSubtypeEnum.TypeToString(tableRow.ObjectType, designTableForm.TableDesigner.Type);
				obj.SubtypeDisplayText = SharedObjectSubtypeEnum.TypeToStringForSingle(tableRow.ObjectType, designTableForm.TableDesigner.Type);
				gridView.RefreshData();
			}
		}
	}

	public static bool DeleteSelectedRows(GridView gridView, SharedObjectTypeEnum.ObjectType objectType)
	{
		bool result = false;
		try
		{
			int[] deletedRows = GetDeletedRows(gridView, isObject: false);
			if (AskIfDeleting(deletedRows, gridView, objectType))
			{
				for (int num = deletedRows.Count() - 1; num >= 0; num--)
				{
					if (IsDeletedFromDB(gridView.GetRow(deletedRows[num])))
					{
						gridView.DeleteRow(deletedRows[num]);
					}
					result = true;
				}
				return true;
			}
			return false;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, gridView?.GridControl?.FindForm());
			return result;
		}
	}

	public static bool DeleteSelectedObjects(GridView gridView, BindingList<int> deletedRelationsConstraintsRows, SharedObjectTypeEnum.ObjectType objectType)
	{
		bool result = false;
		try
		{
			int[] deletedRows = GetDeletedRows(gridView, isObject: true);
			if (AskIfDeleting(deletedRows, gridView, objectType))
			{
				for (int num = deletedRows.Count() - 1; num >= 0; num--)
				{
					BasicRow basicRow = gridView.GetRow(deletedRows[num]) as BasicRow;
					deletedRelationsConstraintsRows.Add(basicRow.Id);
					gridView.DeleteRow(deletedRows[num]);
					result = true;
				}
				return true;
			}
			return false;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, gridView?.GridControl?.FindForm());
			return result;
		}
	}

	public static void SetWaitFormVisibility(SplashScreenManager splashScreenManager, bool show, string description = null)
	{
		try
		{
			if (splashScreenManager == null)
			{
				return;
			}
			if (show && !splashScreenManager.IsSplashFormVisible)
			{
				splashScreenManager.ShowWaitForm();
				if (!string.IsNullOrWhiteSpace(description))
				{
					splashScreenManager.SetWaitFormDescription(description);
				}
			}
			else if (!show && splashScreenManager.IsSplashFormVisible)
			{
				splashScreenManager.CloseWaitForm();
			}
		}
		catch (Exception)
		{
		}
	}

	public static void AddNewProcedure(GridView gridView, int databaseId, DBTreeNode proceduresFolderNode, CustomFieldsSupport customFieldsSupport)
	{
		if (!(gridView.DataSource is List<ObjectWithModulesObject> list))
		{
			return;
		}
		DesignProcedureForm designProcedureForm = new DesignProcedureForm(null, databaseId, proceduresFolderNode.ContainedObjectsObjectType, customFieldsSupport);
		designProcedureForm.ShowDialog();
		if (designProcedureForm.DialogResult == DialogResult.OK)
		{
			ObjectWithModulesObject objectWithModulesObject = new ObjectWithModulesObject
			{
				Name = designProcedureForm.ProcedureDesigner.Name,
				Schema = designProcedureForm.ProcedureDesigner.Schema,
				Id = designProcedureForm.ProcedureDesigner.ProcedureId,
				DatabaseId = designProcedureForm.ProcedureDesigner.DatabaseId,
				Source = "USER",
				Title = designProcedureForm.ProcedureDesigner.Title,
				SubtypeDisplayText = SharedObjectSubtypeEnum.TypeToStringForSingle(proceduresFolderNode.ContainedObjectsObjectType, designProcedureForm.ProcedureDesigner.Type),
				ObjectType = SharedObjectTypeEnum.TypeToString(proceduresFolderNode.ContainedObjectsObjectType),
				Subtype = SharedObjectSubtypeEnum.TypeToString(proceduresFolderNode.ContainedObjectsObjectType, designProcedureForm.ProcedureDesigner.Type),
				DatabaseTitle = designProcedureForm.ProcedureDesigner.DocumentationTitle
			};
			objectWithModulesObject.Initialize();
			list.Add(objectWithModulesObject);
			gridView.GridControl.DataSource = (from x in list
				orderby x.Schema, x.Name
				select x).ToList();
			gridView.RefreshData();
		}
	}

	public static void DesignProcedureOrFunction(GridView gridView, List<ObjectWithModulesObject> dataSource, CustomFieldsSupport customFieldsSupport)
	{
		if (!(gridView?.GetFocusedRow() is ObjectWithModulesObject objectWithModulesObject))
		{
			return;
		}
		UserTypeEnum.UserType source = UserTypeEnum.ObjectToType(PrepareValue.ToString(objectWithModulesObject.Source)) ?? UserTypeEnum.UserType.DBMS;
		SharedObjectTypeEnum.ObjectType objectType = SharedObjectTypeEnum.StringToType(objectWithModulesObject.ObjectType) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
		SharedObjectSubtypeEnum.ObjectSubtype subtype = SharedObjectSubtypeEnum.StringToType(objectType, SharedObjectSubtypeEnum.DisplayStringToString(PrepareValue.ToString(objectWithModulesObject.Subtype)));
		using DesignProcedureForm designProcedureForm = new DesignProcedureForm(objectWithModulesObject.DatabaseId, objectWithModulesObject.Id, objectWithModulesObject.Schema, objectWithModulesObject.Name, objectWithModulesObject.Title, source, objectType, subtype, objectWithModulesObject.DatabaseTitle, customFieldsSupport);
		if (designProcedureForm.ShowDialog() == DialogResult.OK && gridView?.GetFocusedRow() is ObjectWithModulesObject objectWithModulesObject2)
		{
			objectWithModulesObject2.Name = designProcedureForm.ProcedureDesigner.Name;
			objectWithModulesObject2.Schema = designProcedureForm.ProcedureDesigner.Schema;
			objectWithModulesObject2.Title = designProcedureForm.ProcedureDesigner.Title;
			objectWithModulesObject2.Source = UserTypeEnum.TypeToString(designProcedureForm.ProcedureDesigner.Source);
			objectWithModulesObject2.Subtype = SharedObjectSubtypeEnum.TypeToString(objectType, designProcedureForm.ProcedureDesigner.Type);
			objectWithModulesObject2.SubtypeDisplayText = SharedObjectSubtypeEnum.TypeToStringForSingle(objectType, designProcedureForm.ProcedureDesigner.Type);
			gridView?.RefreshData();
		}
	}

	public static void AddNewTerm(GridView gridView, int databaseId, DBTreeNode termsFolderNode, Form owner)
	{
		if (!(gridView?.DataSource is List<TermObject> list))
		{
			return;
		}
		string text = string.Empty;
		TermTypeObject termTypeObject = null;
		DialogResult dialogResult = DialogResult.None;
		using (AddTermForm addTermForm = new AddTermForm())
		{
			dialogResult = addTermForm.ShowDialog();
			text = addTermForm.GetTermTitle();
			termTypeObject = addTermForm.GetTermType();
		}
		if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(text) && termTypeObject != null)
		{
			int? num = DB.BusinessGlossary.InsertTerm(databaseId, null, text, termTypeObject.TermTypeId);
			if (num.HasValue && !(num < 0))
			{
				TermObject term = DB.BusinessGlossary.GetTerm(num.Value);
				list.Add(term);
				gridView.RefreshData();
				DBTreeNode dBTreeNode = new DBTreeNode(termsFolderNode, num.Value, term.Title, SharedObjectTypeEnum.ObjectType.Term, "term", databaseId);
				DB.History.InsertHistoryRow(databaseId, num, term.Title, null, null, "glossary_terms", !string.IsNullOrWhiteSpace(term.Title), saveDescription: false, SharedObjectTypeEnum.ObjectType.Term);
				DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Term, num.Value);
				WorkWithDataedoTrackingHelper.TrackFirstInSessionTermAdd();
				dBTreeNode.CustomInfo = termTypeObject?.IconId;
				termsFolderNode.Nodes.Add(dBTreeNode);
				DBTreeMenu.RefreshFolderInDataBase(termsFolderNode);
			}
		}
	}
}
