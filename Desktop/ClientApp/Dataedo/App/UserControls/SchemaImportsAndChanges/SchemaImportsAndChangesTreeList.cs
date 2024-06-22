using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.SchemaImportsAndChanges.Model;
using Dataedo.Model.Data.SchemaImportsAndChanges;
using Dataedo.Shared.Enums;
using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;

namespace Dataedo.App.UserControls.SchemaImportsAndChanges;

public class SchemaImportsAndChangesTreeList : TreeList
{
	public delegate void SchemaImportsAndChangesObjectModelEventHandler(object sender, SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel);

	public delegate void SchemaImportsAndChangesObjectModelCancelEventHandler(object sender, CancelEventArgs e, SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel);

	public class TreeListViewInfoWithNoImageSupport : TreeListViewInfo
	{
		public TreeListViewInfoWithNoImageSupport(TreeList treeList)
			: base(treeList)
		{
		}

		protected override void CalcSelectImageBounds(RowInfo rowInfo, Rectangle indentBounds)
		{
			base.CalcSelectImageBounds(rowInfo, indentBounds);
			if (rowInfo.SelectImageIndex < 0)
			{
				rowInfo.SelectImageBounds = Rectangle.Empty;
			}
		}
	}

	private readonly Color addedColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesTreeAddedForeColor;

	private readonly Color updatedColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesTreeUpdatedForeColor;

	private readonly Color deletedColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesTreeDeletedForeColor;

	private readonly Color greyColor = SkinsManager.CurrentSkin.GreyColor;

	private SchemaImportsAndChangesObjectModel popupMenuObject;

	private IContainer components;

	private ToolTipController toolTipController;

	private ImageCollection reportTreeListImageCollection;

	private BarManager reportTreeListbarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem goToObjectBarButtonItem;

	private PopupMenu reportTreeListPopupMenu;

	[Browsable(true)]
	public event SchemaImportsAndChangesObjectModelEventHandler GoToObjectClick;

	public event SchemaImportsAndChangesObjectModelCancelEventHandler PopupShowing;

	public SchemaImportsAndChangesTreeList()
	{
		InitializeComponent();
		SharedObjectSubtypeEnum.ColumnSubtypes.ForEach(delegate(SharedObjectSubtypeEnum.ObjectSubtype x)
		{
			string text = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column, x).ToLower();
			if (x == SharedObjectSubtypeEnum.ObjectSubtype.Object)
			{
				text = "column_" + text;
			}
			reportTreeListImageCollection.AddImage(IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Column, x, UserTypeEnum.UserType.DBMS, SynchronizeStateEnum.SynchronizeState.New), text + "_" + SchemaChangeTypeEnum.TypeToStringForIcon(SchemaChangeTypeEnum.SchemaChangeType.Added) + "_16");
			reportTreeListImageCollection.AddImage(IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Column, x, UserTypeEnum.UserType.DBMS, SynchronizeStateEnum.SynchronizeState.Unsynchronized), text + "_" + SchemaChangeTypeEnum.TypeToStringForIcon(SchemaChangeTypeEnum.SchemaChangeType.Updated) + "_16");
			reportTreeListImageCollection.AddImage(IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Column, x, UserTypeEnum.UserType.DBMS, SynchronizeStateEnum.SynchronizeState.Deleted), text + "_" + SchemaChangeTypeEnum.TypeToStringForIcon(SchemaChangeTypeEnum.SchemaChangeType.Deleted) + "_16");
		});
	}

	protected override TreeListViewInfo CreateViewInfo()
	{
		return new TreeListViewInfoWithNoImageSupport(this);
	}

	private void SchemaImportsAndChangesTreeList_KeyDown(object sender, KeyEventArgs e)
	{
		TreeListNode treeListNode = base.FocusedNode;
		switch (e.KeyCode)
		{
		case Keys.Left:
			if (treeListNode != null && treeListNode.HasChildren && treeListNode != null && treeListNode.Expanded)
			{
				treeListNode.Expanded = false;
				e.Handled = true;
			}
			break;
		case Keys.Right:
			treeListNode = base.FocusedNode;
			if (treeListNode != null && treeListNode.HasChildren && treeListNode != null && !treeListNode.Expanded)
			{
				treeListNode.Expanded = true;
				e.Handled = true;
			}
			break;
		}
	}

	private void SchemaImportsAndChangesTreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		if (!(GetDataRecordByNode(e.Node) is SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel))
		{
			return;
		}
		if (schemaImportsAndChangesObjectModel.NoChangesFound)
		{
			e.Appearance.ForeColor = greyColor;
		}
		else
		{
			if (!(e.Column.FieldName == "DateOperationObject") && !(e.Column.FieldName == "Operation"))
			{
				return;
			}
			if (schemaImportsAndChangesObjectModel.Level != SchemaChangeLevelEnum.SchemaChangeLevel.Object && schemaImportsAndChangesObjectModel.Level != SchemaChangeLevelEnum.SchemaChangeLevel.Element)
			{
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			}
			if (schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Operation || e.Column.FieldName == "Operation" || schemaImportsAndChangesObjectModel.IsObjectImportDateLevel)
			{
				if (schemaImportsAndChangesObjectModel != null && schemaImportsAndChangesObjectModel.ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Added)
				{
					e.Appearance.ForeColor = addedColor;
				}
				else if (schemaImportsAndChangesObjectModel != null && schemaImportsAndChangesObjectModel.ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Updated)
				{
					e.Appearance.ForeColor = updatedColor;
				}
				else if (schemaImportsAndChangesObjectModel != null && schemaImportsAndChangesObjectModel.ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted)
				{
					e.Appearance.ForeColor = deletedColor;
				}
			}
		}
	}

	private void SchemaImportsAndChangesTreeList_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (GetDataRecordByNode(base.FocusedNode) is SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel && !schemaImportsAndChangesObjectModel.SupportsComments)
		{
			e.Cancel = true;
		}
	}

	private void SchemaImportsAndChangesTreeList_GetSelectImage(object sender, GetSelectImageEventArgs e)
	{
		Image nodeSelectImage = GetNodeSelectImage(e.Node);
		int num = ((nodeSelectImage != null) ? reportTreeListImageCollection.Images.IndexOf(nodeSelectImage) : (-1));
		if (num >= -1)
		{
			e.NodeImageIndex = num;
		}
		if (!(GetDataRecordByNode(e.Node) is SchemaImportsAndChangesObjectModel))
		{
			e.NodeImageIndex = 0;
		}
	}

	public Image GetNodeSelectImage(TreeListNode node)
	{
		if (!(GetDataRecordByNode(node) is SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel))
		{
			return null;
		}
		return GetNodeSelectImage(schemaImportsAndChangesObjectModel, useMainTypeOnly: false);
	}

	private Image GetNodeSelectImage(SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel, bool useMainTypeOnly)
	{
		string text = null;
		string text2 = null;
		if (schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || schemaImportsAndChangesObjectModel.IsObjectImportDateLevel)
		{
			text2 = ((!useMainTypeOnly) ? SharedObjectSubtypeEnum.TypeToString(schemaImportsAndChangesObjectModel.ObjectType, schemaImportsAndChangesObjectModel.ObjectSubtype)?.ToLower() : SharedObjectTypeEnum.TypeToString(schemaImportsAndChangesObjectModel.ObjectType)?.ToLower());
			string text3 = SchemaChangeTypeEnum.TypeToStringForIcon(schemaImportsAndChangesObjectModel.ChangeType);
			text = text2 + "_" + text3 + "_16";
		}
		else if (schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element)
		{
			text2 = ((!useMainTypeOnly) ? SharedObjectSubtypeEnum.TypeToString(schemaImportsAndChangesObjectModel.ElementType, schemaImportsAndChangesObjectModel.ElementSubtype)?.ToLower() : SharedObjectTypeEnum.TypeToString(schemaImportsAndChangesObjectModel.ElementType)?.ToLower());
			if (schemaImportsAndChangesObjectModel.ElementType == SharedObjectTypeEnum.ObjectType.Key)
			{
				SchemaImportsAndChangesObject schemaImportsAndChangesObject = schemaImportsAndChangesObjectModel.Data;
				text2 = ((schemaImportsAndChangesObject == null || schemaImportsAndChangesObject.PrimaryKey != true) ? ("unique_" + text2) : ("primary_" + text2));
				if (schemaImportsAndChangesObjectModel.Data.Disabled == true)
				{
					text2 += "_disabled";
				}
			}
			else if ((schemaImportsAndChangesObjectModel.ElementType == SharedObjectTypeEnum.ObjectType.Trigger && schemaImportsAndChangesObjectModel.ChangeType != SchemaChangeTypeEnum.SchemaChangeType.Deleted) || schemaImportsAndChangesObjectModel.ElementSubtype == SharedObjectSubtypeEnum.ObjectSubtype.Rule)
			{
				text2 = ((schemaImportsAndChangesObjectModel.Data.Disabled != true) ? (text2 + "_active") : (text2 + "_disabled"));
			}
			else if (schemaImportsAndChangesObjectModel.ElementType == SharedObjectTypeEnum.ObjectType.Parameter)
			{
				if (schemaImportsAndChangesObjectModel.Data.ParameterMode == "IN")
				{
					text2 += "_in";
				}
				else if (schemaImportsAndChangesObjectModel.Data.ParameterMode == "OUT")
				{
					text2 += "_out";
				}
				else if (schemaImportsAndChangesObjectModel.Data.ParameterMode == "INOUT")
				{
					text2 += "_inout";
				}
			}
			if (schemaImportsAndChangesObjectModel.ElementSubtype == SharedObjectSubtypeEnum.ObjectSubtype.Object)
			{
				text2 = "column_" + text2;
			}
			string text4 = SchemaChangeTypeEnum.TypeToStringForIcon(schemaImportsAndChangesObjectModel.ChangeType);
			text = text2 + "_" + text4 + "_16";
		}
		if (schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element || schemaImportsAndChangesObjectModel.IsObjectImportDateLevel)
		{
			Image image = null;
			if (reportTreeListImageCollection.Images.Keys.Contains(text))
			{
				return reportTreeListImageCollection.Images[text];
			}
			if (useMainTypeOnly || schemaImportsAndChangesObjectModel.IsMainType)
			{
				return reportTreeListImageCollection.Images["unresolved_16"];
			}
			return GetNodeSelectImage(schemaImportsAndChangesObjectModel, useMainTypeOnly: true);
		}
		return null;
	}

	private void SchemaImportsAndChangesTreeList_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		SchemaImportsAndChangesTreeList schemaImportsAndChangesTreeList = sender as SchemaImportsAndChangesTreeList;
		schemaImportsAndChangesTreeList.Focus();
		TreeListHitInfo treeListHitInfo = schemaImportsAndChangesTreeList.CalcHitInfo(new Point(e.X, e.Y));
		if (treeListHitInfo == null || treeListHitInfo.HitInfoType != HitInfoType.Cell || !(schemaImportsAndChangesTreeList.GetDataRecordByNode(treeListHitInfo.Node) is SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel) || schemaImportsAndChangesObjectModel.Level != SchemaChangeLevelEnum.SchemaChangeLevel.Object)
		{
			return;
		}
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		popupMenuObject = schemaImportsAndChangesObjectModel;
		this.PopupShowing?.Invoke(this, cancelEventArgs, popupMenuObject);
		if (cancelEventArgs == null || !cancelEventArgs.Cancel)
		{
			goToObjectBarButtonItem.Caption = "Go to " + Escaping.EscapeTextForUI(schemaImportsAndChangesObjectModel.ObjectDisplayName);
			if (!schemaImportsAndChangesObjectModel.Data.ObjectIdCurrent.HasValue)
			{
				goToObjectBarButtonItem.Caption += " (unavailable)";
				goToObjectBarButtonItem.Enabled = false;
			}
			else
			{
				goToObjectBarButtonItem.Enabled = true;
			}
			goToObjectBarButtonItem.Glyph = IconsSupport.GetObjectIcon(schemaImportsAndChangesObjectModel.ObjectType, schemaImportsAndChangesObjectModel.ObjectSubtype, UserTypeEnum.UserType.DBMS, null);
			reportTreeListPopupMenu.ShowPopup(Control.MousePosition);
		}
	}

	private void GoToObjectBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		this.GoToObjectClick?.Invoke(this, popupMenuObject);
	}

	private void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		TreeList treeList = e.SelectedControl as TreeList;
		if (treeList == this)
		{
			TreeListHitInfo treeListHitInfo = treeList.CalcHitInfo(e.ControlMousePosition);
			SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel = treeList.GetDataRecordByNode(treeListHitInfo.Node) as SchemaImportsAndChangesObjectModel;
			if (treeListHitInfo.HitInfoType == HitInfoType.SelectImage && schemaImportsAndChangesObjectModel != null && (schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element))
			{
				object @object = new TreeListCellToolTipInfo(treeListHitInfo.Node, treeListHitInfo.Column, null);
				e.Info = new ToolTipControlInfo(@object, schemaImportsAndChangesObjectModel.TypeWithActionText);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesTreeList));
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.reportTreeListImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.reportTreeListbarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.goToObjectBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.reportTreeListPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		((System.ComponentModel.ISupportInitialize)this.reportTreeListImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeListbarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeListPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this).BeginInit();
		base.SuspendLayout();
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
		this.reportTreeListImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("reportTreeListImageCollection.ImageStream");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_deleted_16, "column_deleted_16", typeof(Dataedo.App.Properties.Resources), 0);
		this.reportTreeListImageCollection.Images.SetKeyName(0, "column_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_new_16, "column_new_16", typeof(Dataedo.App.Properties.Resources), 1);
		this.reportTreeListImageCollection.Images.SetKeyName(1, "column_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_updated_16, "column_updated_16", typeof(Dataedo.App.Properties.Resources), 2);
		this.reportTreeListImageCollection.Images.SetKeyName(2, "column_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_deleted_16, "foreign_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 3);
		this.reportTreeListImageCollection.Images.SetKeyName(3, "foreign_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_new_16, "foreign_table_new_16", typeof(Dataedo.App.Properties.Resources), 4);
		this.reportTreeListImageCollection.Images.SetKeyName(4, "foreign_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_updated_16, "foreign_table_updated_16", typeof(Dataedo.App.Properties.Resources), 5);
		this.reportTreeListImageCollection.Images.SetKeyName(5, "foreign_table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_deleted_16, "function_deleted_16", typeof(Dataedo.App.Properties.Resources), 6);
		this.reportTreeListImageCollection.Images.SetKeyName(6, "function_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_new_16, "function_new_16", typeof(Dataedo.App.Properties.Resources), 7);
		this.reportTreeListImageCollection.Images.SetKeyName(7, "function_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_updated_16, "function_updated_16", typeof(Dataedo.App.Properties.Resources), 8);
		this.reportTreeListImageCollection.Images.SetKeyName(8, "function_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_deleted_16, "parameter_deleted_16", typeof(Dataedo.App.Properties.Resources), 9);
		this.reportTreeListImageCollection.Images.SetKeyName(9, "parameter_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_in_deleted_16, "parameter_in_deleted_16", typeof(Dataedo.App.Properties.Resources), 10);
		this.reportTreeListImageCollection.Images.SetKeyName(10, "parameter_in_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_in_new_16, "parameter_in_new_16", typeof(Dataedo.App.Properties.Resources), 11);
		this.reportTreeListImageCollection.Images.SetKeyName(11, "parameter_in_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_in_updated_16, "parameter_in_updated_16", typeof(Dataedo.App.Properties.Resources), 12);
		this.reportTreeListImageCollection.Images.SetKeyName(12, "parameter_in_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_inout_deleted_16, "parameter_inout_deleted_16", typeof(Dataedo.App.Properties.Resources), 13);
		this.reportTreeListImageCollection.Images.SetKeyName(13, "parameter_inout_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_inout_new_16, "parameter_inout_new_16", typeof(Dataedo.App.Properties.Resources), 14);
		this.reportTreeListImageCollection.Images.SetKeyName(14, "parameter_inout_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_inout_updated_16, "parameter_inout_updated_16", typeof(Dataedo.App.Properties.Resources), 15);
		this.reportTreeListImageCollection.Images.SetKeyName(15, "parameter_inout_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_new_16, "parameter_new_16", typeof(Dataedo.App.Properties.Resources), 16);
		this.reportTreeListImageCollection.Images.SetKeyName(16, "parameter_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_out_deleted_16, "parameter_out_deleted_16", typeof(Dataedo.App.Properties.Resources), 17);
		this.reportTreeListImageCollection.Images.SetKeyName(17, "parameter_out_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_out_new_16, "parameter_out_new_16", typeof(Dataedo.App.Properties.Resources), 18);
		this.reportTreeListImageCollection.Images.SetKeyName(18, "parameter_out_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_out_updated_16, "parameter_out_updated_16", typeof(Dataedo.App.Properties.Resources), 19);
		this.reportTreeListImageCollection.Images.SetKeyName(19, "parameter_out_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_updated_16, "parameter_updated_16", typeof(Dataedo.App.Properties.Resources), 20);
		this.reportTreeListImageCollection.Images.SetKeyName(20, "parameter_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_deleted_16, "primary_key_deleted_16", typeof(Dataedo.App.Properties.Resources), 21);
		this.reportTreeListImageCollection.Images.SetKeyName(21, "primary_key_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_disabled_new_16, "primary_key_disabled_new_16", typeof(Dataedo.App.Properties.Resources), 22);
		this.reportTreeListImageCollection.Images.SetKeyName(22, "primary_key_disabled_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_disabled_updated_16, "primary_key_disabled_updated_16", typeof(Dataedo.App.Properties.Resources), 23);
		this.reportTreeListImageCollection.Images.SetKeyName(23, "primary_key_disabled_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_new_16, "primary_key_new_16", typeof(Dataedo.App.Properties.Resources), 24);
		this.reportTreeListImageCollection.Images.SetKeyName(24, "primary_key_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_updated_16, "primary_key_updated_16", typeof(Dataedo.App.Properties.Resources), 25);
		this.reportTreeListImageCollection.Images.SetKeyName(25, "primary_key_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_deleted_16, "procedure_deleted_16", typeof(Dataedo.App.Properties.Resources), 26);
		this.reportTreeListImageCollection.Images.SetKeyName(26, "procedure_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_new_16, "procedure_new_16", typeof(Dataedo.App.Properties.Resources), 27);
		this.reportTreeListImageCollection.Images.SetKeyName(27, "procedure_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_updated_16, "procedure_updated_16", typeof(Dataedo.App.Properties.Resources), 28);
		this.reportTreeListImageCollection.Images.SetKeyName(28, "procedure_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_deleted_16, "relation_deleted_16", typeof(Dataedo.App.Properties.Resources), 29);
		this.reportTreeListImageCollection.Images.SetKeyName(29, "relation_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_new_16, "relation_new_16", typeof(Dataedo.App.Properties.Resources), 30);
		this.reportTreeListImageCollection.Images.SetKeyName(30, "relation_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_updated_16, "relation_updated_16", typeof(Dataedo.App.Properties.Resources), 31);
		this.reportTreeListImageCollection.Images.SetKeyName(31, "relation_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_deleted_16, "table_deleted_16", typeof(Dataedo.App.Properties.Resources), 32);
		this.reportTreeListImageCollection.Images.SetKeyName(32, "table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_new_16, "table_new_16", typeof(Dataedo.App.Properties.Resources), 33);
		this.reportTreeListImageCollection.Images.SetKeyName(33, "table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_updated_16, "table_updated_16", typeof(Dataedo.App.Properties.Resources), 34);
		this.reportTreeListImageCollection.Images.SetKeyName(34, "table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_active_new_16, "trigger_active_new_16", typeof(Dataedo.App.Properties.Resources), 35);
		this.reportTreeListImageCollection.Images.SetKeyName(35, "trigger_active_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_active_updated_16, "trigger_active_updated_16", typeof(Dataedo.App.Properties.Resources), 36);
		this.reportTreeListImageCollection.Images.SetKeyName(36, "trigger_active_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_deleted_16, "trigger_deleted_16", typeof(Dataedo.App.Properties.Resources), 37);
		this.reportTreeListImageCollection.Images.SetKeyName(37, "trigger_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_disabled_new_16, "trigger_disabled_new_16", typeof(Dataedo.App.Properties.Resources), 38);
		this.reportTreeListImageCollection.Images.SetKeyName(38, "trigger_disabled_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_disabled_updated_16, "trigger_disabled_updated_16", typeof(Dataedo.App.Properties.Resources), 39);
		this.reportTreeListImageCollection.Images.SetKeyName(39, "trigger_disabled_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_deleted_16, "unique_key_deleted_16", typeof(Dataedo.App.Properties.Resources), 40);
		this.reportTreeListImageCollection.Images.SetKeyName(40, "unique_key_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_disabled_new_16, "unique_key_disabled_new_16", typeof(Dataedo.App.Properties.Resources), 41);
		this.reportTreeListImageCollection.Images.SetKeyName(41, "unique_key_disabled_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_disabled_updated_16, "unique_key_disabled_updated_16", typeof(Dataedo.App.Properties.Resources), 42);
		this.reportTreeListImageCollection.Images.SetKeyName(42, "unique_key_disabled_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_new_16, "unique_key_new_16", typeof(Dataedo.App.Properties.Resources), 43);
		this.reportTreeListImageCollection.Images.SetKeyName(43, "unique_key_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_updated_16, "unique_key_updated_16", typeof(Dataedo.App.Properties.Resources), 44);
		this.reportTreeListImageCollection.Images.SetKeyName(44, "unique_key_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.unresolved_16, "unresolved_16", typeof(Dataedo.App.Properties.Resources), 45);
		this.reportTreeListImageCollection.Images.SetKeyName(45, "unresolved_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_deleted_16, "view_deleted_16", typeof(Dataedo.App.Properties.Resources), 46);
		this.reportTreeListImageCollection.Images.SetKeyName(46, "view_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_new_16, "view_new_16", typeof(Dataedo.App.Properties.Resources), 47);
		this.reportTreeListImageCollection.Images.SetKeyName(47, "view_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_updated_16, "view_updated_16", typeof(Dataedo.App.Properties.Resources), 48);
		this.reportTreeListImageCollection.Images.SetKeyName(48, "view_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_deleted_16, "collection_deleted_16", typeof(Dataedo.App.Properties.Resources), 49);
		this.reportTreeListImageCollection.Images.SetKeyName(49, "collection_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_new_16, "collection_new_16", typeof(Dataedo.App.Properties.Resources), 50);
		this.reportTreeListImageCollection.Images.SetKeyName(50, "collection_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_updated_16, "collection_updated_16", typeof(Dataedo.App.Properties.Resources), 51);
		this.reportTreeListImageCollection.Images.SetKeyName(51, "collection_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_deleted_16, "cube_deleted_16", typeof(Dataedo.App.Properties.Resources), 52);
		this.reportTreeListImageCollection.Images.SetKeyName(52, "cube_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_new_16, "cube_new_16", typeof(Dataedo.App.Properties.Resources), 53);
		this.reportTreeListImageCollection.Images.SetKeyName(53, "cube_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_updated_16, "cube_updated_16", typeof(Dataedo.App.Properties.Resources), 54);
		this.reportTreeListImageCollection.Images.SetKeyName(54, "cube_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_deleted_16, "custom_object_deleted_16", typeof(Dataedo.App.Properties.Resources), 55);
		this.reportTreeListImageCollection.Images.SetKeyName(55, "custom_object_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_new_16, "custom_object_new_16", typeof(Dataedo.App.Properties.Resources), 56);
		this.reportTreeListImageCollection.Images.SetKeyName(56, "custom_object_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_updated_16, "custom_object_updated_16", typeof(Dataedo.App.Properties.Resources), 57);
		this.reportTreeListImageCollection.Images.SetKeyName(57, "custom_object_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_deleted_16, "dimension_deleted_16", typeof(Dataedo.App.Properties.Resources), 58);
		this.reportTreeListImageCollection.Images.SetKeyName(58, "dimension_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_new_16, "dimension_new_16", typeof(Dataedo.App.Properties.Resources), 59);
		this.reportTreeListImageCollection.Images.SetKeyName(59, "dimension_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_updated_16, "dimension_updated_16", typeof(Dataedo.App.Properties.Resources), 60);
		this.reportTreeListImageCollection.Images.SetKeyName(60, "dimension_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_deleted_16, "editioning_view_deleted_16", typeof(Dataedo.App.Properties.Resources), 61);
		this.reportTreeListImageCollection.Images.SetKeyName(61, "editioning_view_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_new_16, "editioning_view_new_16", typeof(Dataedo.App.Properties.Resources), 62);
		this.reportTreeListImageCollection.Images.SetKeyName(62, "editioning_view_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_updated_16, "editioning_view_updated_16", typeof(Dataedo.App.Properties.Resources), 63);
		this.reportTreeListImageCollection.Images.SetKeyName(63, "editioning_view_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_deleted_16, "entity_deleted_16", typeof(Dataedo.App.Properties.Resources), 64);
		this.reportTreeListImageCollection.Images.SetKeyName(64, "entity_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_new_16, "entity_new_16", typeof(Dataedo.App.Properties.Resources), 65);
		this.reportTreeListImageCollection.Images.SetKeyName(65, "entity_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_updated_16, "entity_updated_16", typeof(Dataedo.App.Properties.Resources), 66);
		this.reportTreeListImageCollection.Images.SetKeyName(66, "entity_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_deleted_16, "external_object_deleted_16", typeof(Dataedo.App.Properties.Resources), 67);
		this.reportTreeListImageCollection.Images.SetKeyName(67, "external_object_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_new_16, "external_object_new_16", typeof(Dataedo.App.Properties.Resources), 68);
		this.reportTreeListImageCollection.Images.SetKeyName(68, "external_object_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_updated_16, "external_object_updated_16", typeof(Dataedo.App.Properties.Resources), 69);
		this.reportTreeListImageCollection.Images.SetKeyName(69, "external_object_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_deleted_16, "external_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 70);
		this.reportTreeListImageCollection.Images.SetKeyName(70, "external_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_new_16, "external_table_new_16", typeof(Dataedo.App.Properties.Resources), 71);
		this.reportTreeListImageCollection.Images.SetKeyName(71, "external_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_updated_16, "external_table_updated_16", typeof(Dataedo.App.Properties.Resources), 72);
		this.reportTreeListImageCollection.Images.SetKeyName(72, "external_table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_deleted_16, "file_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 73);
		this.reportTreeListImageCollection.Images.SetKeyName(73, "file_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_new_16, "file_table_new_16", typeof(Dataedo.App.Properties.Resources), 74);
		this.reportTreeListImageCollection.Images.SetKeyName(74, "file_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_updated_16, "file_table_updated_16", typeof(Dataedo.App.Properties.Resources), 75);
		this.reportTreeListImageCollection.Images.SetKeyName(75, "file_table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_deleted_16, "flat_file_deleted_16", typeof(Dataedo.App.Properties.Resources), 76);
		this.reportTreeListImageCollection.Images.SetKeyName(76, "flat_file_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_new_16, "flat_file_new_16", typeof(Dataedo.App.Properties.Resources), 77);
		this.reportTreeListImageCollection.Images.SetKeyName(77, "flat_file_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_updated_16, "flat_file_updated_16", typeof(Dataedo.App.Properties.Resources), 78);
		this.reportTreeListImageCollection.Images.SetKeyName(78, "flat_file_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_deleted_16, "graph_edge_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 79);
		this.reportTreeListImageCollection.Images.SetKeyName(79, "graph_edge_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_new_16, "graph_edge_table_new_16", typeof(Dataedo.App.Properties.Resources), 80);
		this.reportTreeListImageCollection.Images.SetKeyName(80, "graph_edge_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_updated_16, "graph_edge_table_updated_16", typeof(Dataedo.App.Properties.Resources), 81);
		this.reportTreeListImageCollection.Images.SetKeyName(81, "graph_edge_table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_deleted_16, "graph_node_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 82);
		this.reportTreeListImageCollection.Images.SetKeyName(82, "graph_node_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_new_16, "graph_node_table_new_16", typeof(Dataedo.App.Properties.Resources), 83);
		this.reportTreeListImageCollection.Images.SetKeyName(83, "graph_node_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_updated_16, "graph_node_table_updated_16", typeof(Dataedo.App.Properties.Resources), 84);
		this.reportTreeListImageCollection.Images.SetKeyName(84, "graph_node_table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_deleted_16, "graph_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 85);
		this.reportTreeListImageCollection.Images.SetKeyName(85, "graph_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_new_16, "graph_table_new_16", typeof(Dataedo.App.Properties.Resources), 86);
		this.reportTreeListImageCollection.Images.SetKeyName(86, "graph_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_updated_16, "graph_table_updated_16", typeof(Dataedo.App.Properties.Resources), 87);
		this.reportTreeListImageCollection.Images.SetKeyName(87, "graph_table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_deleted_16, "history_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 88);
		this.reportTreeListImageCollection.Images.SetKeyName(88, "history_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_new_16, "history_table_new_16", typeof(Dataedo.App.Properties.Resources), 89);
		this.reportTreeListImageCollection.Images.SetKeyName(89, "history_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_updated_16, "history_table_updated_16", typeof(Dataedo.App.Properties.Resources), 90);
		this.reportTreeListImageCollection.Images.SetKeyName(90, "history_table_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_deleted_16, "indexed_view_deleted_16", typeof(Dataedo.App.Properties.Resources), 91);
		this.reportTreeListImageCollection.Images.SetKeyName(91, "indexed_view_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_new_16, "indexed_view_new_16", typeof(Dataedo.App.Properties.Resources), 92);
		this.reportTreeListImageCollection.Images.SetKeyName(92, "indexed_view_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_updated_16, "indexed_view_updated_16", typeof(Dataedo.App.Properties.Resources), 93);
		this.reportTreeListImageCollection.Images.SetKeyName(93, "indexed_view_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_deleted_16, "materialized_view_deleted_16", typeof(Dataedo.App.Properties.Resources), 94);
		this.reportTreeListImageCollection.Images.SetKeyName(94, "materialized_view_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_new_16, "materialized_view_new_16", typeof(Dataedo.App.Properties.Resources), 95);
		this.reportTreeListImageCollection.Images.SetKeyName(95, "materialized_view_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_updated_16, "materialized_view_updated_16", typeof(Dataedo.App.Properties.Resources), 96);
		this.reportTreeListImageCollection.Images.SetKeyName(96, "materialized_view_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_deleted_16, "named_query_deleted_16", typeof(Dataedo.App.Properties.Resources), 97);
		this.reportTreeListImageCollection.Images.SetKeyName(97, "named_query_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_new_16, "named_query_new_16", typeof(Dataedo.App.Properties.Resources), 98);
		this.reportTreeListImageCollection.Images.SetKeyName(98, "named_query_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_updated_16, "named_query_updated_16", typeof(Dataedo.App.Properties.Resources), 99);
		this.reportTreeListImageCollection.Images.SetKeyName(99, "named_query_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_deleted_16, "object_deleted_16", typeof(Dataedo.App.Properties.Resources), 100);
		this.reportTreeListImageCollection.Images.SetKeyName(100, "object_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_new_16, "object_new_16", typeof(Dataedo.App.Properties.Resources), 101);
		this.reportTreeListImageCollection.Images.SetKeyName(101, "object_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_updated_16, "object_updated_16", typeof(Dataedo.App.Properties.Resources), 102);
		this.reportTreeListImageCollection.Images.SetKeyName(102, "object_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_deleted_16, "package_deleted_16", typeof(Dataedo.App.Properties.Resources), 103);
		this.reportTreeListImageCollection.Images.SetKeyName(103, "package_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_new_16, "package_new_16", typeof(Dataedo.App.Properties.Resources), 104);
		this.reportTreeListImageCollection.Images.SetKeyName(104, "package_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_updated_16, "package_updated_16", typeof(Dataedo.App.Properties.Resources), 105);
		this.reportTreeListImageCollection.Images.SetKeyName(105, "package_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_deleted_16, "rule_active_deleted_16", typeof(Dataedo.App.Properties.Resources), 106);
		this.reportTreeListImageCollection.Images.SetKeyName(106, "rule_active_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_new_16, "rule_active_new_16", typeof(Dataedo.App.Properties.Resources), 107);
		this.reportTreeListImageCollection.Images.SetKeyName(107, "rule_active_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_updated_16, "rule_active_updated_16", typeof(Dataedo.App.Properties.Resources), 108);
		this.reportTreeListImageCollection.Images.SetKeyName(108, "rule_active_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_deleted_16, "rule_disabled_deleted_16", typeof(Dataedo.App.Properties.Resources), 109);
		this.reportTreeListImageCollection.Images.SetKeyName(109, "rule_disabled_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_new_16, "rule_disabled_new_16", typeof(Dataedo.App.Properties.Resources), 110);
		this.reportTreeListImageCollection.Images.SetKeyName(110, "rule_disabled_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_updated_16, "rule_disabled_updated_16", typeof(Dataedo.App.Properties.Resources), 111);
		this.reportTreeListImageCollection.Images.SetKeyName(111, "rule_disabled_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_deleted_16, "search_index_deleted_16", typeof(Dataedo.App.Properties.Resources), 112);
		this.reportTreeListImageCollection.Images.SetKeyName(112, "search_index_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_new_16, "search_index_new_16", typeof(Dataedo.App.Properties.Resources), 113);
		this.reportTreeListImageCollection.Images.SetKeyName(113, "search_index_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_updated_16, "search_index_updated_16", typeof(Dataedo.App.Properties.Resources), 114);
		this.reportTreeListImageCollection.Images.SetKeyName(114, "search_index_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_deleted_16, "standard_object_deleted_16", typeof(Dataedo.App.Properties.Resources), 115);
		this.reportTreeListImageCollection.Images.SetKeyName(115, "standard_object_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_new_16, "standard_object_new_16", typeof(Dataedo.App.Properties.Resources), 116);
		this.reportTreeListImageCollection.Images.SetKeyName(116, "standard_object_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_updated_16, "standard_object_updated_16", typeof(Dataedo.App.Properties.Resources), 117);
		this.reportTreeListImageCollection.Images.SetKeyName(117, "standard_object_updated_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_deleted_16, "system_versioned_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 118);
		this.reportTreeListImageCollection.Images.SetKeyName(118, "system_versioned_table_deleted_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_new_16, "system_versioned_table_new_16", typeof(Dataedo.App.Properties.Resources), 119);
		this.reportTreeListImageCollection.Images.SetKeyName(119, "system_versioned_table_new_16");
		this.reportTreeListImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_updated_16, "system_versioned_table_updated_16", typeof(Dataedo.App.Properties.Resources), 120);
		this.reportTreeListImageCollection.Images.SetKeyName(120, "system_versioned_table_updated_16");
		this.reportTreeListbarManager.DockControls.Add(this.barDockControlTop);
		this.reportTreeListbarManager.DockControls.Add(this.barDockControlBottom);
		this.reportTreeListbarManager.DockControls.Add(this.barDockControlLeft);
		this.reportTreeListbarManager.DockControls.Add(this.barDockControlRight);
		this.reportTreeListbarManager.Form = this;
		this.reportTreeListbarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[1] { this.goToObjectBarButtonItem });
		this.reportTreeListbarManager.MaxItemId = 1;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.reportTreeListbarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(400, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 200);
		this.barDockControlBottom.Manager = this.reportTreeListbarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(400, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.reportTreeListbarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 200);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(400, 0);
		this.barDockControlRight.Manager = this.reportTreeListbarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 200);
		this.goToObjectBarButtonItem.Caption = "Go to object";
		this.goToObjectBarButtonItem.Id = 0;
		this.goToObjectBarButtonItem.Name = "goToObjectBarButtonItem";
		this.goToObjectBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(GoToObjectBarButtonItem_ItemClick);
		this.reportTreeListPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.goToObjectBarButtonItem)
		});
		this.reportTreeListPopupMenu.Manager = this.reportTreeListbarManager;
		this.reportTreeListPopupMenu.Name = "reportTreeListPopupMenu";
		this.AllowDrop = true;
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.Font = new System.Drawing.Font("Tahoma", 8.25f);
		base.OptionsBehavior.AllowExpandOnDblClick = false;
		base.OptionsCustomization.AllowFilter = false;
		base.OptionsFilter.AllowFilterEditor = false;
		base.OptionsFind.AllowFindPanel = false;
		base.OptionsMenu.ShowExpandCollapseItems = false;
		base.OptionsView.ShowHorzLines = false;
		base.OptionsView.ShowIndicator = false;
		base.OptionsView.ShowVertLines = false;
		base.SelectImageList = this.reportTreeListImageCollection;
		base.ToolTipController = this.toolTipController;
		base.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(SchemaImportsAndChangesTreeList_GetSelectImage);
		base.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(SchemaImportsAndChangesTreeList_NodeCellStyle);
		base.ShowingEditor += new System.ComponentModel.CancelEventHandler(SchemaImportsAndChangesTreeList_ShowingEditor);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(SchemaImportsAndChangesTreeList_KeyDown);
		base.MouseDown += new System.Windows.Forms.MouseEventHandler(SchemaImportsAndChangesTreeList_MouseDown);
		((System.ComponentModel.ISupportInitialize)this.reportTreeListImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeListbarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeListPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
