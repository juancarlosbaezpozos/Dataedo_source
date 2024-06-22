using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.DDLGenerating;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.DDLGenerating;

public class DDLChooseObjectsUserControl : BaseUserControl
{
	private ScriptType.ScriptTypeEnum scriptType;

	private IContainer components;

	private NonCustomizableLayoutControl objectsLayoutControl;

	private HyperLinkEdit noneObjectsHyperLinkEdit;

	private HyperLinkEdit allObjectsHyperLinkEdit;

	private TreeList objectsTreeList;

	private TreeListColumn nameTreeListColumn;

	private LayoutControlGroup layoutControlGroup;

	private LayoutControlItem objectsListLayoutControlItem;

	private LayoutControlItem allObjectsLayoutControlItem;

	private LayoutControlItem noneObjectsLayoutControlItem;

	private EmptySpaceItem emptySpaceItem8;

	private HyperLinkEdit resetHyperLinkEdit;

	private LayoutControlItem resetLayoutControlItem;

	private HyperLinkEdit expandAllHyperLinkEdit;

	private HyperLinkEdit collapseAllHyperLinkEdit;

	private LayoutControlItem expandAllLayoutControlItem;

	private LayoutControlItem collapseAllLayoutControlItem;

	private BindingSource dDLChosenObjectBindingSource;

	private TreeListColumn sourceTreeListColumn;

	private ImageCollection treeMenuImageCollection;

	public BindingList<DDLObject> SelectedObjects { get; private set; }

	public DDLChooseObjectsUserControl()
	{
		InitializeComponent();
		SelectedObjects = new BindingList<DDLObject>();
	}

	public void LoadObjects(int databaseId, ScriptType.ScriptTypeEnum scriptType)
	{
		this.scriptType = scriptType;
		SelectedObjects = new BindingList<DDLObject>((from x in DB.Table.GetTablesAndColumnsForTree(databaseId)
			select new DDLObject
			{
				Id = x.Id,
				ParentId = x.ParentId,
				DisplayName = DBTreeMenu.SetNameOnlySchema(x.Schema, x.Name, DatabaseRow.GetShowSchema(x.ShowSchema, x.ShowSchemaOverride)),
				Name = x.Name,
				Schema = x.Schema,
				Source = UserTypeEnum.ObjectToType(x.Source),
				DatatypeLen = x.DatatypeLen,
				Nullable = (x.Nullable == true),
				DefaultValue = x.DefaultValue,
				Type = SharedObjectTypeEnum.StringToType(x.Type),
				Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.StringToType(x.Type), x.Subtype)
			}).ToList());
		objectsTreeList.DataSource = SelectedObjects;
		objectsTreeList.ExpandAll();
		objectsTreeList.TopVisibleNodeIndex = 0;
		SetSelectedObjects(scriptType);
	}

	private void SetSelectedObjects(ScriptType.ScriptTypeEnum scriptType)
	{
		objectsTreeList.UncheckAll();
		switch (scriptType)
		{
		case ScriptType.ScriptTypeEnum.New:
			objectsTreeList.CheckAll();
			break;
		case ScriptType.ScriptTypeEnum.Update:
			objectsTreeList.NodesIterator.DoOperation(delegate(TreeListNode node)
			{
				if (SelectedObjects[node.Id]?.Source == UserTypeEnum.UserType.USER)
				{
					node.Checked = true;
				}
			});
			break;
		}
	}

	private void allObjectsHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		objectsTreeList.CheckAll();
	}

	private void noneObjectsHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		objectsTreeList.UncheckAll();
	}

	private void resetHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		SetSelectedObjects(scriptType);
	}

	private void expandAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		objectsTreeList.ExpandAll();
	}

	private void collapseAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		objectsTreeList.CollapseAll();
	}

	private void objectsTreeList_CustomDrawNodeImages(object sender, CustomDrawNodeImagesEventArgs e)
	{
		DDLObject dDLObject = SelectedObjects[e.Node.Id];
		if (dDLObject != null)
		{
			Bitmap objectIcon = IconsSupport.GetObjectIcon(dDLObject.Type, dDLObject.Subtype, dDLObject.Source.Value, SynchronizeStateEnum.SynchronizeState.Synchronized);
			e.Cache.DrawImage(objectIcon, e.SelectImageLocation);
			e.Handled = true;
		}
	}

	private void objectsTreeList_AfterCheckNode(object sender, NodeEventArgs e)
	{
		if (e.Node.Checked)
		{
			e.Node.CheckAll();
		}
		else
		{
			e.Node.UncheckAll();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.DDLGenerating.DDLChooseObjectsUserControl));
		this.objectsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.resetHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.noneObjectsHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.allObjectsHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.objectsTreeList = new DevExpress.XtraTreeList.TreeList();
		this.nameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.sourceTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.dDLChosenObjectBindingSource = new System.Windows.Forms.BindingSource(this.components);
		this.treeMenuImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.expandAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.collapseAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.objectsListLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.allObjectsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.noneObjectsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.resetLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.expandAllLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.collapseAllLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.objectsLayoutControl).BeginInit();
		this.objectsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.resetHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noneObjectsHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allObjectsHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectsTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dDLChosenObjectBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectsListLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allObjectsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noneObjectsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.resetLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.objectsLayoutControl.AllowCustomization = false;
		this.objectsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.objectsLayoutControl.Controls.Add(this.resetHyperLinkEdit);
		this.objectsLayoutControl.Controls.Add(this.noneObjectsHyperLinkEdit);
		this.objectsLayoutControl.Controls.Add(this.allObjectsHyperLinkEdit);
		this.objectsLayoutControl.Controls.Add(this.objectsTreeList);
		this.objectsLayoutControl.Controls.Add(this.expandAllHyperLinkEdit);
		this.objectsLayoutControl.Controls.Add(this.collapseAllHyperLinkEdit);
		this.objectsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.objectsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.objectsLayoutControl.Name = "objectsLayoutControl";
		this.objectsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(839, 235, 697, 629);
		this.objectsLayoutControl.Root = this.layoutControlGroup;
		this.objectsLayoutControl.Size = new System.Drawing.Size(535, 441);
		this.objectsLayoutControl.TabIndex = 2;
		this.objectsLayoutControl.Text = "layoutControl3";
		this.resetHyperLinkEdit.EditValue = "Reset selection";
		this.resetHyperLinkEdit.Location = new System.Drawing.Point(72, 2);
		this.resetHyperLinkEdit.Name = "resetHyperLinkEdit";
		this.resetHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.resetHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.resetHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.resetHyperLinkEdit.Size = new System.Drawing.Size(86, 18);
		this.resetHyperLinkEdit.StyleController = this.objectsLayoutControl;
		this.resetHyperLinkEdit.TabIndex = 10;
		this.resetHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(resetHyperLinkEdit_OpenLink);
		this.noneObjectsHyperLinkEdit.EditValue = "None";
		this.noneObjectsHyperLinkEdit.Location = new System.Drawing.Point(27, 2);
		this.noneObjectsHyperLinkEdit.MaximumSize = new System.Drawing.Size(40, 0);
		this.noneObjectsHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.noneObjectsHyperLinkEdit.Name = "noneObjectsHyperLinkEdit";
		this.noneObjectsHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.noneObjectsHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.noneObjectsHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.noneObjectsHyperLinkEdit.Size = new System.Drawing.Size(40, 18);
		this.noneObjectsHyperLinkEdit.StyleController = this.objectsLayoutControl;
		this.noneObjectsHyperLinkEdit.TabIndex = 9;
		this.noneObjectsHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(noneObjectsHyperLinkEdit_OpenLink);
		this.allObjectsHyperLinkEdit.EditValue = "All";
		this.allObjectsHyperLinkEdit.Location = new System.Drawing.Point(2, 2);
		this.allObjectsHyperLinkEdit.MaximumSize = new System.Drawing.Size(25, 0);
		this.allObjectsHyperLinkEdit.MinimumSize = new System.Drawing.Size(25, 0);
		this.allObjectsHyperLinkEdit.Name = "allObjectsHyperLinkEdit";
		this.allObjectsHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.allObjectsHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.allObjectsHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.allObjectsHyperLinkEdit.Size = new System.Drawing.Size(25, 18);
		this.allObjectsHyperLinkEdit.StyleController = this.objectsLayoutControl;
		this.allObjectsHyperLinkEdit.TabIndex = 8;
		this.allObjectsHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(allObjectsHyperLinkEdit_OpenLink);
		this.objectsTreeList.CheckBoxFieldName = "Checked";
		this.objectsTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[2] { this.nameTreeListColumn, this.sourceTreeListColumn });
		this.objectsTreeList.DataSource = this.dDLChosenObjectBindingSource;
		this.objectsTreeList.ImageIndexFieldName = "";
		this.objectsTreeList.KeyFieldName = "Id";
		this.objectsTreeList.Location = new System.Drawing.Point(0, 24);
		this.objectsTreeList.Margin = new System.Windows.Forms.Padding(0);
		this.objectsTreeList.Name = "objectsTreeList";
		this.objectsTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.objectsTreeList.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.objectsTreeList.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
		this.objectsTreeList.ParentFieldName = "ParentId";
		this.objectsTreeList.SelectImageList = this.treeMenuImageCollection;
		this.objectsTreeList.Size = new System.Drawing.Size(535, 417);
		this.objectsTreeList.TabIndex = 4;
		this.objectsTreeList.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
		this.objectsTreeList.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(objectsTreeList_AfterCheckNode);
		this.objectsTreeList.CustomDrawNodeImages += new DevExpress.XtraTreeList.CustomDrawNodeImagesEventHandler(objectsTreeList_CustomDrawNodeImages);
		this.nameTreeListColumn.AppearanceCell.BackColor = System.Drawing.Color.Red;
		this.nameTreeListColumn.Caption = "Name";
		this.nameTreeListColumn.FieldName = "DisplayName";
		this.nameTreeListColumn.MinWidth = 32;
		this.nameTreeListColumn.Name = "nameTreeListColumn";
		this.nameTreeListColumn.OptionsColumn.AllowEdit = false;
		this.nameTreeListColumn.OptionsColumn.ReadOnly = true;
		this.nameTreeListColumn.Visible = true;
		this.nameTreeListColumn.VisibleIndex = 0;
		this.nameTreeListColumn.Width = 50;
		this.sourceTreeListColumn.Caption = "Source";
		this.sourceTreeListColumn.FieldName = "Source";
		this.sourceTreeListColumn.Name = "sourceTreeListColumn";
		this.dDLChosenObjectBindingSource.DataSource = typeof(Dataedo.App.Tools.DDLGenerating.DDLObject);
		this.treeMenuImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("treeMenuImageCollection.ImageStream");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_16, "table", typeof(Dataedo.App.Properties.Resources), 0, "table_16");
		this.treeMenuImageCollection.Images.SetKeyName(0, "table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_16, "column", typeof(Dataedo.App.Properties.Resources), 1, "column_16");
		this.treeMenuImageCollection.Images.SetKeyName(1, "column");
		this.expandAllHyperLinkEdit.EditValue = "Expand All";
		this.expandAllHyperLinkEdit.Location = new System.Drawing.Point(162, 2);
		this.expandAllHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.expandAllHyperLinkEdit.Name = "expandAllHyperLinkEdit";
		this.expandAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.expandAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.expandAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.expandAllHyperLinkEdit.Size = new System.Drawing.Size(65, 18);
		this.expandAllHyperLinkEdit.StyleController = this.objectsLayoutControl;
		this.expandAllHyperLinkEdit.TabIndex = 9;
		this.expandAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(expandAllHyperLinkEdit_OpenLink);
		this.collapseAllHyperLinkEdit.EditValue = "Collapse all";
		this.collapseAllHyperLinkEdit.Location = new System.Drawing.Point(231, 2);
		this.collapseAllHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.collapseAllHyperLinkEdit.Name = "collapseAllHyperLinkEdit";
		this.collapseAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.collapseAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.collapseAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.collapseAllHyperLinkEdit.Size = new System.Drawing.Size(67, 18);
		this.collapseAllHyperLinkEdit.StyleController = this.objectsLayoutControl;
		this.collapseAllHyperLinkEdit.TabIndex = 9;
		this.collapseAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(collapseAllHyperLinkEdit_OpenLink);
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.objectsListLayoutControlItem, this.allObjectsLayoutControlItem, this.noneObjectsLayoutControlItem, this.emptySpaceItem8, this.resetLayoutControlItem, this.expandAllLayoutControlItem, this.collapseAllLayoutControlItem });
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup.Size = new System.Drawing.Size(535, 441);
		this.layoutControlGroup.TextVisible = false;
		this.objectsListLayoutControlItem.Control = this.objectsTreeList;
		this.objectsListLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.objectsListLayoutControlItem.Name = "objectsListLayoutControlItem";
		this.objectsListLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.objectsListLayoutControlItem.Size = new System.Drawing.Size(535, 417);
		this.objectsListLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.objectsListLayoutControlItem.TextVisible = false;
		this.allObjectsLayoutControlItem.Control = this.allObjectsHyperLinkEdit;
		this.allObjectsLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.allObjectsLayoutControlItem.MaxSize = new System.Drawing.Size(25, 24);
		this.allObjectsLayoutControlItem.MinSize = new System.Drawing.Size(25, 24);
		this.allObjectsLayoutControlItem.Name = "allObjectsLayoutControlItem";
		this.allObjectsLayoutControlItem.Size = new System.Drawing.Size(25, 24);
		this.allObjectsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.allObjectsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.allObjectsLayoutControlItem.TextVisible = false;
		this.noneObjectsLayoutControlItem.Control = this.noneObjectsHyperLinkEdit;
		this.noneObjectsLayoutControlItem.Location = new System.Drawing.Point(25, 0);
		this.noneObjectsLayoutControlItem.MaxSize = new System.Drawing.Size(45, 24);
		this.noneObjectsLayoutControlItem.MinSize = new System.Drawing.Size(45, 24);
		this.noneObjectsLayoutControlItem.Name = "noneObjectsLayoutControlItem";
		this.noneObjectsLayoutControlItem.Size = new System.Drawing.Size(45, 24);
		this.noneObjectsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.noneObjectsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.noneObjectsLayoutControlItem.TextVisible = false;
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.Location = new System.Drawing.Point(300, 0);
		this.emptySpaceItem8.MaxSize = new System.Drawing.Size(0, 24);
		this.emptySpaceItem8.MinSize = new System.Drawing.Size(1, 24);
		this.emptySpaceItem8.Name = "emptySpaceItem8";
		this.emptySpaceItem8.Size = new System.Drawing.Size(235, 24);
		this.emptySpaceItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.resetLayoutControlItem.Control = this.resetHyperLinkEdit;
		this.resetLayoutControlItem.Location = new System.Drawing.Point(70, 0);
		this.resetLayoutControlItem.MaxSize = new System.Drawing.Size(90, 24);
		this.resetLayoutControlItem.MinSize = new System.Drawing.Size(90, 24);
		this.resetLayoutControlItem.Name = "resetLayoutControlItem";
		this.resetLayoutControlItem.Size = new System.Drawing.Size(90, 24);
		this.resetLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.resetLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.resetLayoutControlItem.TextVisible = false;
		this.expandAllLayoutControlItem.Control = this.expandAllHyperLinkEdit;
		this.expandAllLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.expandAllLayoutControlItem.CustomizationFormText = "noneObjectsLayoutControlItem";
		this.expandAllLayoutControlItem.Location = new System.Drawing.Point(160, 0);
		this.expandAllLayoutControlItem.MaxSize = new System.Drawing.Size(69, 24);
		this.expandAllLayoutControlItem.MinSize = new System.Drawing.Size(69, 24);
		this.expandAllLayoutControlItem.Name = "expandAllLayoutControlItem";
		this.expandAllLayoutControlItem.Size = new System.Drawing.Size(69, 24);
		this.expandAllLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.expandAllLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.expandAllLayoutControlItem.TextVisible = false;
		this.collapseAllLayoutControlItem.Control = this.collapseAllHyperLinkEdit;
		this.collapseAllLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.collapseAllLayoutControlItem.CustomizationFormText = "noneObjectsLayoutControlItem";
		this.collapseAllLayoutControlItem.Location = new System.Drawing.Point(229, 0);
		this.collapseAllLayoutControlItem.MaxSize = new System.Drawing.Size(71, 24);
		this.collapseAllLayoutControlItem.MinSize = new System.Drawing.Size(71, 24);
		this.collapseAllLayoutControlItem.Name = "collapseAllLayoutControlItem";
		this.collapseAllLayoutControlItem.Size = new System.Drawing.Size(71, 24);
		this.collapseAllLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.collapseAllLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.collapseAllLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.objectsLayoutControl);
		base.Name = "DDLChooseObjectsUserControl";
		base.Size = new System.Drawing.Size(535, 441);
		((System.ComponentModel.ISupportInitialize)this.objectsLayoutControl).EndInit();
		this.objectsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.resetHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noneObjectsHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allObjectsHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectsTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dDLChosenObjectBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectsListLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allObjectsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noneObjectsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.resetLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
