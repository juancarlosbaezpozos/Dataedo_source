using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.CommonFunctionsForPanels;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls.SummaryControls;

public class BaseSummaryUserControl : BaseUserControl
{
	public ObjectEventArgs ObjectEventArgs;

	protected GridView GridView { get; set; }

	protected DBTreeNode Node { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectType { get; protected set; }

	public MetadataEditorUserControl MainControl { get; set; }

	public CustomFieldsSupport CustomFieldsSupport { get; protected set; }

	public IEnumerable<DropdownModuleModel> Modules { get; set; }

	protected SummaryBulkCopy BulkCopy { get; set; }

	public IDragRowsBase DragRows { get; set; }

	public DBTreeMenu TreeMenu { get; set; }

	public bool ContextShowSchema { get; set; }

	public virtual void SetTooltips()
	{
	}

	public virtual void SetParameters(DBTreeNode node, CustomFieldsSupport customFieldsSupport, SharedObjectTypeEnum.ObjectType? objectType)
	{
		Node = node;
		CustomFieldsSupport = customFieldsSupport;
		ObjectType = objectType;
		BulkCopy.CustomFieldsSupport = CustomFieldsSupport;
		SetTooltips();
		ContextShowSchema = Node.ShowSchema;
	}

	public IEnumerable<DropdownModuleModel> GetDocumentationModules()
	{
		if (Modules != null)
		{
			return Modules.Where((DropdownModuleModel x) => x.DatabaseId == Node.DatabaseId);
		}
		return new List<DropdownModuleModel>();
	}

	public virtual void SetParameters()
	{
		if (Node != null && ObjectType.HasValue && CustomFieldsSupport != null)
		{
			SetParameters(Node, CustomFieldsSupport, ObjectType);
		}
	}

	public IEnumerable<ToolStripMenuItem> GetModulesSubmenu(IEnumerable<object> selectedObjects)
	{
		List<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
		foreach (DropdownModuleModel module in GetDocumentationModules())
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Image = Resources.module_16;
			toolStripMenuItem.Text = Escaping.EscapeTextForUI(module.Name);
			toolStripMenuItem.Click += delegate
			{
				BaseSummaryModuleHelper.AddObjectsToAssignedModule(selectedObjects, module.ModuleId, FindForm());
			};
			toolStripMenuItem.Click += delegate
			{
				GridView.RefreshData();
			};
			list.Add(toolStripMenuItem);
		}
		return list;
	}

	public IEnumerable<BarButtonItem> GetModulesSubmenuBarButtonItems(BarManager barManager, IEnumerable<object> selectedObjects)
	{
		List<BarButtonItem> list = new List<BarButtonItem>();
		foreach (DropdownModuleModel module in GetDocumentationModules())
		{
			if (selectedObjects.Count() != 1 || !BaseSummaryModuleHelper.IsAlreadyInModule(module.ModuleId, selectedObjects.First() as ObjectWithModulesObject))
			{
				BarButtonItem barButtonItem = new BarButtonItem(barManager, Escaping.EscapeTextForUI(module.Name));
				barButtonItem.ImageOptions.Image = Resources.module_16;
				barButtonItem.ItemClick += delegate
				{
					BaseSummaryModuleHelper.AddObjectsToAssignedModule(selectedObjects, module.ModuleId, FindForm());
				};
				barButtonItem.ItemClick += delegate
				{
					GridView.RefreshData();
				};
				list.Add(barButtonItem);
			}
		}
		return list;
	}

	public void AddToNewModule()
	{
		MainControl.AddNewModuleFromObjectsList();
		int id = MainControl.GetFocusedNode().Id;
		BaseSummaryModuleHelper.AddToNewModule(GridView, id);
		MainControl.MetadataTreeList.OptionsBehavior.Editable = true;
		MainControl.MetadataTreeList.ShowEditor();
	}

	public List<DBTreeNode> GetBusinessGlossaryNodes()
	{
		return MainControl.TreeListHelpers.GetBusinessGlossaryNodes();
	}

	public void AddNewBusinessGlossaryTerm(int? businessGlossaryId, IEnumerable<object> selectedObjects)
	{
		MainControl.BusinessGlossarySupport.AddNewBusinessGlossaryTerm(businessGlossaryId, FindForm(), selectedObjects.Select((object x) => new BusinessGlossarySupport.ObjectDefinition(x as ObjectWithModulesObject)).ToArray());
	}

	public virtual void CloseEditor()
	{
		GridView.CloseEditor();
	}

	public virtual void ClearData()
	{
	}

	public virtual void RefreshData()
	{
	}

	protected virtual void AddEvents()
	{
	}
}
