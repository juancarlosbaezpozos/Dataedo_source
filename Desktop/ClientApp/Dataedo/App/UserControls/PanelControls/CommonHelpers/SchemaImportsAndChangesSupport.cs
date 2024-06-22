using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls.SchemaImportsAndChanges;
using Dataedo.App.UserControls.SchemaImportsAndChanges.Model;
using Dataedo.Model.Data.SchemaImportsAndChanges;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;

namespace Dataedo.App.UserControls.PanelControls.CommonHelpers;

public class SchemaImportsAndChangesSupport
{
	private readonly BasePanelControl basePanelControl;

	private XtraTabPage SchemaImportsAndChangesXtraTabPage => basePanelControl.SchemaImportsAndChangesXtraTabPage;

	private SchemaImportsAndChangesUserControl SchemaImportsAndChangesUserControl => basePanelControl.SchemaImportsAndChangesUserControl;

	public SchemaImportsAndChangesSupport(BasePanelControl basePanelControl)
	{
		this.basePanelControl = basePanelControl;
		if (this.basePanelControl.SchemaImportsAndChangesUserControl != null)
		{
			this.basePanelControl.SchemaImportsAndChangesUserControl.ValueChanged += SchemaImportsAndChangesUserControl_ValueChanged;
			this.basePanelControl.SchemaImportsAndChangesUserControl.RefreshImportsCalled += SchemaImportsAndChangesUserControl_GenerateButtonClick;
			this.basePanelControl.SchemaImportsAndChangesUserControl.GoToObjectClick += SchemaImportsAndChangesUserControl_GoToObjectClick;
			this.basePanelControl.SchemaImportsAndChangesUserControl.TreeBeforeExpand += SchemaImportsAndChangesUserControl_BeforeExpand;
			this.basePanelControl.SchemaImportsAndChangesUserControl.ExpandAllCalled += SchemaImportsAndChangesUserControl_ExpandAllCalled;
		}
	}

	public void CloseEditors()
	{
		SchemaImportsAndChangesUserControl?.CloseEditors();
	}

	public bool UpdateSchemaImportsAndChangesComments(Form owner = null)
	{
		try
		{
			List<SchemaImportsAndChangesObjectModel> schemaImportsAndChangesObjects = SchemaImportsAndChangesUserControl.FlattenData?.Where((SchemaImportsAndChangesObjectModel x) => x.IsCommentModified)?.ToList();
			DB.SchemaImportsAndChanges.UpdateComments(schemaImportsAndChangesObjects);
			SchemaImportsAndChangesUserControl.IsChanged = false;
			return true;
		}
		catch (Exception exception)
		{
			StaticData.CrashedDatabaseType = basePanelControl?.CurrentNode?.DatabaseType;
			StaticData.CrashedDBMSVersion = basePanelControl?.CurrentNode?.DBMSVersion;
			GeneralExceptionHandling.Handle(exception, "Error while updating schema changes comments", owner);
			StaticData.ClearDatabaseInfoForCrashes();
			return false;
		}
	}

	private void SchemaImportsAndChangesUserControl_ExpandAllCalled(object sender, EventArgs e)
	{
		if (!(sender is TreeList treeList) || StaticData.IsProjectFile)
		{
			return;
		}
		try
		{
			treeList.BeginUpdate();
			basePanelControl.MainControl.SetWaitformVisibility(visible: true);
			LoadNodesForExpandedNodes(treeList);
			SchemaImportsAndChangesUserControl.ErrorOccurred = false;
			basePanelControl.MainControl.SetWaitformVisibility(visible: false);
		}
		catch (Exception exception)
		{
			SchemaImportsAndChangesUserControl.ErrorOccurred = true;
			basePanelControl.MainControl.SetWaitformVisibility(visible: false);
			StaticData.CrashedDatabaseType = basePanelControl?.CurrentNode?.DatabaseType;
			StaticData.CrashedDBMSVersion = basePanelControl?.CurrentNode?.DBMSVersion;
			GeneralExceptionHandling.Handle(exception, "Error while expanding imports.", treeList?.FindForm());
			StaticData.ClearDatabaseInfoForCrashes();
		}
		finally
		{
			treeList.EndUpdate();
			treeList.ForceInitialize();
		}
	}

	private void LoadNodesForExpandedNodes(TreeList treeList)
	{
		if (!(treeList.DataSource is SchemaImportsAndChangesBindingList))
		{
			return;
		}
		List<SchemaImportsAndChangesObjectModel> list = new List<SchemaImportsAndChangesObjectModel>();
		List<int> list2 = new List<int>();
		foreach (SchemaImportsAndChangesObjectModel item in treeList.DataSource as SchemaImportsAndChangesBindingList)
		{
			bool flag = IsExpandFunctionalityLocked();
			if (SchemaImportsAndChangesUserControl.ErrorOccurred || item.ParentNode != null || item == null || item.Children?.Count != 0 || item == null)
			{
				continue;
			}
			SchemaImportsAndChangesObject data = item.Data;
			if (data != null && data.UpdateId.HasValue && !item.NoChangesFound)
			{
				if (!flag)
				{
					list.Add(item);
					list2.Add(item.Data.UpdateId.Value);
				}
				else
				{
					LoadUpgradeAccountBanner(treeList, item);
				}
			}
		}
		if (list2.Count() <= 0)
		{
			return;
		}
		foreach (SchemaImportsAndChangesObjectModel node in LoadNodeData(string.Join(",", list2)))
		{
			SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel = list.Where((SchemaImportsAndChangesObjectModel n) => n.Data.UpdateId == node.Data.UpdateId)?.FirstOrDefault();
			if (schemaImportsAndChangesObjectModel == null)
			{
				continue;
			}
			if (schemaImportsAndChangesObjectModel.IsObjectImportDateLevel)
			{
				foreach (SchemaImportsAndChangesObjectModel child in node.Children)
				{
					schemaImportsAndChangesObjectModel?.Children.Add(child);
				}
			}
			else if (!schemaImportsAndChangesObjectModel.IsObjectImportDateLevel)
			{
				schemaImportsAndChangesObjectModel?.Children.Add(node);
			}
		}
	}

	private void SchemaImportsAndChangesUserControl_BeforeExpand(object sender, BeforeExpandEventArgs e)
	{
		if (!(sender is TreeList treeList))
		{
			return;
		}
		if (treeList.CalcHitInfo(treeList.PointToClient(Control.MousePosition)).HitInfoType == HitInfoType.Button)
		{
			SchemaImportsAndChangesUserControl.ErrorOccurred = false;
			SchemaImportsAndChangesUserControl.ExpandAllRequested = false;
		}
		SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel = treeList.GetDataRecordByNode(e.Node) as SchemaImportsAndChangesObjectModel;
		if (SchemaImportsAndChangesUserControl.ExpandAllRequested || SchemaImportsAndChangesUserControl.ErrorOccurred || e.Node.ParentNode != null || schemaImportsAndChangesObjectModel == null || schemaImportsAndChangesObjectModel.Children?.Count != 0 || schemaImportsAndChangesObjectModel == null)
		{
			return;
		}
		SchemaImportsAndChangesObject data = schemaImportsAndChangesObjectModel.Data;
		if (data == null || !data.UpdateId.HasValue)
		{
			return;
		}
		bool num = IsExpandFunctionalityLocked();
		basePanelControl.MainControl.SetWaitformVisibility(visible: true);
		if (!num)
		{
			try
			{
				LoadNodeData(treeList, schemaImportsAndChangesObjectModel);
			}
			catch (Exception exception)
			{
				SchemaImportsAndChangesUserControl.ErrorOccurred = true;
				basePanelControl.MainControl.SetWaitformVisibility(visible: false);
				StaticData.CrashedDatabaseType = basePanelControl?.CurrentNode?.DatabaseType;
				StaticData.CrashedDBMSVersion = basePanelControl?.CurrentNode?.DBMSVersion;
				GeneralExceptionHandling.Handle(exception, "Error while expanding node.", treeList?.FindForm());
				StaticData.ClearDatabaseInfoForCrashes();
			}
		}
		else
		{
			LoadUpgradeAccountBanner(treeList, schemaImportsAndChangesObjectModel);
		}
		basePanelControl.MainControl.SetWaitformVisibility(visible: false);
	}

	private bool IsExpandFunctionalityLocked()
	{
		return !Functionalities.HasFunctionality(FunctionalityEnum.Functionality.SchemaChangeTracking);
	}

	private void LoadUpgradeAccountBanner(TreeList treeList, SchemaImportsAndChangesObjectModel nodeData)
	{
		SchemaImportsAndChangesObjectModel item = new SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel.LicenseWitoutSCT, null, null, showSchema: false);
		nodeData.Children.Add(item);
		treeList.EndUpdate();
		treeList.ForceInitialize();
	}

	private void LoadNodeData(TreeList treeList, SchemaImportsAndChangesObjectModel nodeData)
	{
		List<SchemaImportsAndChangesObjectModel> list = LoadNodeData(nodeData?.Data?.UpdateId.Value.ToString());
		treeList.BeginUpdate();
		if (nodeData.IsObjectImportDateLevel)
		{
			foreach (SchemaImportsAndChangesObjectModel item in list)
			{
				foreach (SchemaImportsAndChangesObjectModel child in item.Children)
				{
					nodeData.Children.Add(child);
				}
			}
		}
		else
		{
			foreach (SchemaImportsAndChangesObjectModel item2 in list)
			{
				nodeData.Children.Add(item2);
			}
		}
		treeList.EndUpdate();
		treeList.ForceInitialize();
	}

	private void SchemaImportsAndChangesUserControl_ValueChanged(object sender, EventArgs e)
	{
		basePanelControl.SetTabPageTitle(isEdited: true, SchemaImportsAndChangesXtraTabPage, basePanelControl.Edit);
	}

	private void SchemaImportsAndChangesUserControl_GenerateButtonClick(object sender, EventArgs e)
	{
		if (!StaticData.IsProjectFile)
		{
			try
			{
				GenerateReportAction(null, SchemaImportsAndChangesUserControl?.FindForm());
				SchemaImportsAndChangesUserControl.ExpandAllRequested = false;
				SchemaImportsAndChangesUserControl.ErrorOccurred = false;
			}
			catch (Exception exception)
			{
				StaticData.CrashedDatabaseType = basePanelControl?.CurrentNode?.DatabaseType;
				StaticData.CrashedDBMSVersion = basePanelControl?.CurrentNode?.DBMSVersion;
				GeneralExceptionHandling.Handle(exception, "Error while loading imports.", basePanelControl?.SchemaImportsAndChangesUserControl?.FindForm());
				StaticData.ClearDatabaseInfoForCrashes();
			}
		}
	}

	private void SchemaImportsAndChangesUserControl_GoToObjectClick(object sender, SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel)
	{
		basePanelControl.MainControl.SelectSchemaImportsAndChangesObject(null, schemaImportsAndChangesObjectModel);
	}

	private List<SchemaImportsAndChangesObjectModel> LoadNodeData(string importsId)
	{
		if (basePanelControl.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			return DB.SchemaImportsAndChanges.GetDocumentationReport(basePanelControl.DatabaseId, basePanelControl.ShowSchema, SchemaImportsAndChangesUserControl.ShowAllImports, importsId);
		}
		if (basePanelControl.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			return DB.SchemaImportsAndChanges.GetModuleReport(basePanelControl.DatabaseId, basePanelControl.ObjectModuleId, basePanelControl.ShowSchema, SchemaImportsAndChangesUserControl.ShowAllImports, importsId);
		}
		return DB.SchemaImportsAndChanges.GetObjectReport(basePanelControl.DatabaseId, basePanelControl.ShowSchema, basePanelControl.ObjectSchema, basePanelControl.ObjectName, basePanelControl.ObjectType, SchemaImportsAndChangesUserControl.ShowAllImports, importsId);
	}

	private void GenerateReportAction(string importsId, Form owner = null)
	{
		try
		{
			if (SchemaImportsAndChangesUserControl.IsChanged)
			{
				switch (GeneralMessageBoxesHandling.Show("Comments were changed. Do you want to save them?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, owner)?.DialogResult)
				{
				case DialogResult.None:
				case DialogResult.Cancel:
					return;
				case DialogResult.Yes:
					if (UpdateSchemaImportsAndChangesComments(owner))
					{
						basePanelControl.SetCurrentTabPageTitle(isEdited: false, SchemaImportsAndChangesXtraTabPage);
					}
					break;
				case DialogResult.No:
					basePanelControl.SetCurrentTabPageTitle(isEdited: false, SchemaImportsAndChangesXtraTabPage);
					break;
				}
			}
			basePanelControl.MainControl.SetWaitformVisibility(visible: true);
			if (basePanelControl.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
			{
				List<SchemaImportsAndChangesObjectModel> documentationReport = DB.SchemaImportsAndChanges.GetDocumentationReport(basePanelControl.DatabaseId, basePanelControl.ShowSchema, SchemaImportsAndChangesUserControl.ShowAllImports, importsId);
				SchemaImportsAndChangesUserControl.SetData(basePanelControl.ObjectId, basePanelControl.ObjectType, basePanelControl.DatabaseId, null, documentationReport);
			}
			else if (basePanelControl.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				List<SchemaImportsAndChangesObjectModel> moduleReport = DB.SchemaImportsAndChanges.GetModuleReport(basePanelControl.DatabaseId, basePanelControl.ObjectModuleId, basePanelControl.ShowSchema, SchemaImportsAndChangesUserControl.ShowAllImports, importsId);
				SchemaImportsAndChangesUserControl.SetData(basePanelControl.ObjectId, basePanelControl.ObjectType, basePanelControl.DatabaseId, basePanelControl.ObjectModuleId, moduleReport);
			}
			else
			{
				List<SchemaImportsAndChangesObjectModel> objectReport = DB.SchemaImportsAndChanges.GetObjectReport(basePanelControl.DatabaseId, basePanelControl.ShowSchema, basePanelControl.ObjectSchema, basePanelControl.ObjectName, basePanelControl.ObjectType, SchemaImportsAndChangesUserControl.ShowAllImports, importsId);
				SchemaImportsAndChangesUserControl.SetData(basePanelControl.ObjectId, basePanelControl.ObjectType, basePanelControl.DatabaseId, basePanelControl.ObjectModuleId, objectReport);
			}
		}
		finally
		{
			basePanelControl.MainControl.SetWaitformVisibility(visible: false);
		}
	}
}
