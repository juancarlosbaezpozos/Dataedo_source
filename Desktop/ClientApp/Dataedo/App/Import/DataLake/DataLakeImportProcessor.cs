using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Import.DataLake;

public class DataLakeImportProcessor
{
	private int databaseId;

	private Form parentWindow;

	private SplashScreenManager splashScreenManager;

	private readonly CustomFieldsSupport customFieldsSupport;

	public SharedObjectTypeEnum.ObjectType ObjectType;

	public IEnumerable<ObjectModel> ObjectModels { get; private set; }

	public IEnumerable<ObjectModel> AllModels { get; private set; }

	public DataLakeImportProcessor(int databaseId, SharedObjectTypeEnum.ObjectType objectType, Form parentWindow = null, SplashScreenManager splashScreenManager = null, CustomFieldsSupport customFieldsSupport = null)
	{
		this.databaseId = databaseId;
		ObjectType = objectType;
		this.parentWindow = parentWindow;
		this.splashScreenManager = splashScreenManager;
		this.customFieldsSupport = customFieldsSupport;
	}

	public bool ProcessImport(DataLakeTypeEnum.DataLakeType? dataLakeType, IEnumerable<ObjectModel> objectModels)
	{
		if (objectModels.Count() == 1)
		{
			ObjectModel objectModel = ProcessSingleObjectModel(objectModels.ElementAt(0), dataLakeType);
			ObjectModels = new List<ObjectModel>(new ObjectModel[1] { objectModel });
			AllModels = new List<ObjectModel>(new ObjectModel[1] { objectModel });
			return objectModel != null;
		}
		ImportChooseObjects importChooseObjects = new ImportChooseObjects(parentWindow, databaseId, dataLakeType, ObjectType, objectModels, customFieldsSupport);
		bool num = importChooseObjects.ShowDialog(parentWindow) == DialogResult.OK;
		if (num)
		{
			ObjectModels = ProcessObjectModels(importChooseObjects.ObjectModels.Where((ObjectModel x) => x.IsSelected), parentWindow);
			AllModels = importChooseObjects.ObjectModels.ToList();
			ImportFileTrackingHelper.TrackImportFileSaveSuccess(ObjectModels.SelectMany((ObjectModel o) => o.Fields).Count(), dataLakeType);
		}
		return num;
	}

	private ObjectModel ProcessSingleObjectModel(ObjectModel objectModel, DataLakeTypeEnum.DataLakeType? dataLakeType)
	{
		List<TableByDatabaseIdObject> existingObjects = DB.Table.GetTablesByDatabase(databaseId, ObjectType);
		bool flag = CheckObjectExists(existingObjects, objectModel.Name);
		objectModel.OriginalObjectExists = flag;
		objectModel.ObjectExists = flag;
		objectModel.IsSelected = !flag;
		if (flag)
		{
			int num = 1;
			while (existingObjects.Any((TableByDatabaseIdObject x) => CheckObjectExists(existingObjects, objectModel.Name)))
			{
				objectModel.Name = $"{objectModel.OriginalName} ({++num})";
				if (num == 0)
				{
					objectModel.Name = objectModel.OriginalName;
					break;
				}
			}
			objectModel.ObjectExists = false;
			objectModel.IsSelected = true;
			objectModel.SetNameAsCorrectedName();
		}
		ObjectModel objectModel2 = objectModel;
		if (objectModel2 != null && objectModel2.OriginalObjectExists)
		{
			ObjectModel objectModel3 = objectModel;
			if (objectModel3 != null && objectModel3.IsCorrectedNameSet && GeneralMessageBoxesHandling.Show("There already is an object named \"" + objectModel.OriginalName + "\"." + Environment.NewLine + "Do you want to continue?", "Object exists", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, null, 1, parentWindow).DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		DesignTableForm designTableForm = new DesignTableForm(parentWindow, databaseId, objectModel, UserTypeEnum.UserType.USER, SynchronizeStateEnum.SynchronizeState.New, forceExistanceValidation: true, validateOnStart: false, customFieldsSupport);
		ObjectModel obj = ((designTableForm.ShowDialog(parentWindow) == DialogResult.OK) ? objectModel : null);
		if (obj != null)
		{
			ImportFileTrackingHelper.TrackImportFileSaveSuccess(designTableForm.TableDesigner.InsertedColumns.Count(), dataLakeType);
		}
		objectModel.ObjectId = designTableForm.TableDesigner.TableId;
		return obj;
	}

	private bool CheckObjectExists(IEnumerable<TableByDatabaseIdObject> objects, string name)
	{
		return objects.Any((TableByDatabaseIdObject x) => ColumnsHelper.GetFullTableName(x.Schema, x.Name).Equals(ColumnsHelper.GetFullTableName(null, name)));
	}

	private IEnumerable<ObjectModel> ProcessObjectModels(IEnumerable<ObjectModel> objectModels, Form owner = null)
	{
		List<ObjectModel> list = new List<ObjectModel>();
		foreach (ObjectModel objectModel in objectModels)
		{
			try
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
				TableModel tableModel = new TableModel(databaseId, objectModel.Name, null, null, objectModel.Location, objectModel.ObjectType, objectModel.ObjectSubtype, objectModel.SchemaScript, objectModel.Source);
				objectModel.ObjectId = DB.Table.InsertManualTable(tableModel, owner);
				DB.Community.InsertFollowingToRepository(objectModel.ObjectType, objectModel.ObjectId);
				if (!objectModel.ObjectId.HasValue)
				{
					CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
					throw new Exception("Unable to insert values.");
				}
				DBTreeMenu.AddManualObjectToTree(databaseId, objectModel.ObjectId.Value, null, objectModel.Name, null, objectModel.ObjectType, objectModel.ObjectSubtype, objectModel.Source, null, SynchronizeStateEnum.SynchronizeState.New);
				list.Add(objectModel);
				DB.History.InsertHistoryRow(databaseId, objectModel.ObjectId, tableModel.Title, null, null, "tables", !string.IsNullOrEmpty(tableModel.Title), !string.IsNullOrEmpty(null), objectModel.ObjectType);
				List<ColumnRow> list2 = objectModel.Fields.Select((FieldModel x) => new ColumnRow
				{
					TableId = objectModel.ObjectId,
					Type = SharedObjectSubtypeEnum.TypeToString(x.ObjectType, x.ObjectSubtype),
					Name = x.Name,
					DataType = x.DataType,
					DataTypeWithoutLength = x.DataType,
					DataLength = x.DataTypeSize?.ToString(),
					Nullable = x.Nullable,
					Path = x.Path,
					Level = x.Level,
					Position = x.Position,
					RowState = ManagingRowsEnum.ManagingRows.ForAdding,
					Source = UserTypeEnum.UserType.USER,
					Title = x.Title,
					Description = x.Description
				}).ToList();
				TableDesigner.SetSortValues(list2);
				DB.Column.InsertManualColumn(list2, owner);
				HistoryColumnsHelper.InsertHistoryRowsInDataLakeImportProcessor(objectModel, list2, databaseId);
			}
			catch (Exception ex)
			{
				string text = SharedObjectSubtypeEnum.TypeToStringForSingle(objectModel.ObjectType, objectModel.ObjectSubtype);
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				GeneralMessageBoxesHandling.Show("Unable to add " + objectModel.Name + ".", "Unable to add " + text, MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, parentWindow);
			}
			finally
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
		}
		return list;
	}
}
