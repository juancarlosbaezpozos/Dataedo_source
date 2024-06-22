using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls;

public class ChooseObjectTypesUserControl : BaseUserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl objectTypesLayoutControl;

	private HyperLinkEdit noneObjectTypesHyperLinkEdit;

	private HyperLinkEdit allObjectTypesHyperLinkEdit;

	private TreeList dbObjectsTreeList;

	private TreeListColumn treeListColumn1;

	private TreeListColumn treeListColumnUpgradeToPro;

	private RepositoryItemHyperLinkEdit repositoryItemHyperLinkEditUpgradeToPro;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControlItem layoutControlItem9;

	private LayoutControlItem layoutControlItem10;

	private LayoutControlItem layoutControlItem11;

	private EmptySpaceItem emptySpaceItem8;

	public bool IsExportingToDatabase { get; set; }

	public int SelectedObjectTypesCount => GetIncludedDbObjects().Count;

	public ChooseObjectTypesUserControl()
	{
		InitializeComponent();
	}

	public void LoadObjects(DocFormatEnum.DocFormat format, bool canExportBG = false)
	{
		SetDbObjects(DocDBObjectsManager.GetExportObjects(format, canExportBG));
		treeListColumn1.Width = 180;
	}

	public void LoadObjects(LoadObjectTypeEnum type)
	{
		SetDbObjects(DocDBObjectsManager.GetExportObjects(type));
		treeListColumn1.Width = 180;
	}

	public List<ObjectTypeHierarchy> GetIncludedDbObjects()
	{
		List<ObjectTypeHierarchy> list = new List<ObjectTypeHierarchy>();
		dbObjectsTreeList.Nodes.Where((TreeListNode x) => x.Checked).ToList();
		AddToIncludedObjects(list, dbObjectsTreeList.Nodes);
		return list;
	}

	private void AddToIncludedObjects(List<ObjectTypeHierarchy> includedObjects, IEnumerable<TreeListNode> nodes)
	{
		foreach (TreeListNode item in nodes.Where((TreeListNode x) => x.Checked))
		{
			DocDBObject docDBObject = dbObjectsTreeList.GetDataRecordByNode(item) as DocDBObject;
			if (docDBObject.ObjectType.ObjectType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
			{
				includedObjects.Add(docDBObject.ObjectType);
			}
			AddToIncludedObjects(includedObjects, item.Nodes);
		}
	}

	public List<ObjectTypeHierarchy> GetExcludedDbObjects()
	{
		List<ObjectTypeHierarchy> list = new List<ObjectTypeHierarchy>();
		foreach (TreeListNode node in dbObjectsTreeList.Nodes)
		{
			ExcludeDbObjects(list, node);
		}
		return list;
	}

	private void ExcludeDbObjects(List<ObjectTypeHierarchy> excludedObjects, TreeListNode node, bool excludeWithoutChecking = false)
	{
		if (!node.Checked || excludeWithoutChecking)
		{
			excludedObjects.Add((dbObjectsTreeList.GetDataRecordByNode(node) as DocDBObject).ObjectType);
			excludeWithoutChecking = true;
		}
		foreach (TreeListNode node2 in node.Nodes)
		{
			ExcludeDbObjects(excludedObjects, node2, excludeWithoutChecking);
		}
	}

	private void dbObjectsTreeList_BeforeCheckNode(object sender, CheckNodeEventArgs e)
	{
		TreeListNode treeListNode = e.Node;
		e.CanCheck = (dbObjectsTreeList.GetDataRecordByNode(treeListNode) as DocDBObject)?.IsEnabled ?? false;
		if (!e.CanCheck)
		{
			return;
		}
		if (IsExportingToDatabase)
		{
			treeListNode.Checked = true;
			return;
		}
		while (treeListNode != null)
		{
			DocDBObject obj = dbObjectsTreeList.GetDataRecordByNode(treeListNode) as DocDBObject;
			if (obj != null && obj.IsEnabled)
			{
				treeListNode.Checked = true;
				treeListNode = treeListNode.ParentNode;
				continue;
			}
			break;
		}
	}

	private void dbObjectsTreeList_AfterCheckNode(object sender, NodeEventArgs e)
	{
		if (e.Node.Checked)
		{
			TreeListNode node = e.Node;
			if (node != null)
			{
				TreeListNode parentNode = node.ParentNode;
				if (((parentNode != null) ? new bool?(!parentNode.Checked) : null) == true)
				{
					e.Node.ParentNode.Checked = true;
					return;
				}
			}
		}
		if (e.Node.HasChildren && e.Node.Checked)
		{
			e.Node.CheckAll();
		}
		if (dbObjectsTreeList.GetDataRecordByNode(e.Node) is DescriptionDocDBObject && e.Node.ParentNode != null)
		{
			e.Node.ParentNode.Checked = e.Node.ParentNode.Nodes.Any((TreeListNode x) => x.Checked);
		}
		if (e.Node.HasChildren && !e.Node.Checked)
		{
			e.Node.UncheckAll();
		}
	}

	private void dbObjectsTreeList_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
	{
		if (!(dbObjectsTreeList.GetDataRecordByNode(e.Node) as DocDBObject).IsEnabled)
		{
			e.ObjectArgs.State = ObjectState.Disabled;
		}
	}

	private void allObjectTypesHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		dbObjectsTreeList.NodesIterator.DoOperation(delegate(TreeListNode node)
		{
			if ((dbObjectsTreeList.GetDataRecordByNode(node) as DocDBObject).IsEnabled)
			{
				node.Checked = true;
			}
		});
	}

	private void noneObjectTypesHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		dbObjectsTreeList.NodesIterator.DoOperation(delegate(TreeListNode node)
		{
			if ((dbObjectsTreeList.GetDataRecordByNode(node) as DocDBObject).IsEnabled)
			{
				node.Checked = false;
			}
		});
	}

	private void SetDbObjects(BindingList<DocDBObject> dbObjects)
	{
		dbObjectsTreeList.DataSource = dbObjects;
		dbObjectsTreeList.ExpandAll();
		dbObjectsTreeList.CheckAll();
		dbObjectsTreeList.NodesIterator.DoOperation(delegate(TreeListNode x)
		{
			if (!(dbObjectsTreeList.GetDataRecordByNode(x) as DocDBObject).IsCheckedByDefault)
			{
				x.CheckState = CheckState.Unchecked;
			}
			else
			{
				x.CheckState = CheckState.Checked;
			}
		});
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
		this.objectTypesLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.noneObjectTypesHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.allObjectTypesHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.dbObjectsTreeList = new DevExpress.XtraTreeList.TreeList();
		this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.treeListColumnUpgradeToPro = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.repositoryItemHyperLinkEditUpgradeToPro = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.objectTypesLayoutControl).BeginInit();
		this.objectTypesLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.noneObjectTypesHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allObjectTypesHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbObjectsTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEditUpgradeToPro).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		base.SuspendLayout();
		this.objectTypesLayoutControl.AllowCustomization = false;
		this.objectTypesLayoutControl.Controls.Add(this.noneObjectTypesHyperLinkEdit);
		this.objectTypesLayoutControl.Controls.Add(this.allObjectTypesHyperLinkEdit);
		this.objectTypesLayoutControl.Controls.Add(this.dbObjectsTreeList);
		this.objectTypesLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.objectTypesLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.objectTypesLayoutControl.Name = "objectTypesLayoutControl";
		this.objectTypesLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(895, 321, 697, 629);
		this.objectTypesLayoutControl.Root = this.layoutControlGroup5;
		this.objectTypesLayoutControl.Size = new System.Drawing.Size(673, 489);
		this.objectTypesLayoutControl.TabIndex = 1;
		this.objectTypesLayoutControl.Text = "layoutControl3";
		this.noneObjectTypesHyperLinkEdit.EditValue = "None";
		this.noneObjectTypesHyperLinkEdit.Location = new System.Drawing.Point(27, 2);
		this.noneObjectTypesHyperLinkEdit.MaximumSize = new System.Drawing.Size(40, 0);
		this.noneObjectTypesHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.noneObjectTypesHyperLinkEdit.Name = "noneObjectTypesHyperLinkEdit";
		this.noneObjectTypesHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.noneObjectTypesHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.noneObjectTypesHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.noneObjectTypesHyperLinkEdit.Size = new System.Drawing.Size(40, 18);
		this.noneObjectTypesHyperLinkEdit.StyleController = this.objectTypesLayoutControl;
		this.noneObjectTypesHyperLinkEdit.TabIndex = 9;
		this.noneObjectTypesHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(noneObjectTypesHyperLinkEdit_OpenLink);
		this.allObjectTypesHyperLinkEdit.EditValue = "All";
		this.allObjectTypesHyperLinkEdit.Location = new System.Drawing.Point(2, 2);
		this.allObjectTypesHyperLinkEdit.MaximumSize = new System.Drawing.Size(25, 0);
		this.allObjectTypesHyperLinkEdit.MinimumSize = new System.Drawing.Size(25, 0);
		this.allObjectTypesHyperLinkEdit.Name = "allObjectTypesHyperLinkEdit";
		this.allObjectTypesHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.allObjectTypesHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.allObjectTypesHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.allObjectTypesHyperLinkEdit.Size = new System.Drawing.Size(25, 18);
		this.allObjectTypesHyperLinkEdit.StyleController = this.objectTypesLayoutControl;
		this.allObjectTypesHyperLinkEdit.TabIndex = 8;
		this.allObjectTypesHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(allObjectTypesHyperLinkEdit_OpenLink);
		this.dbObjectsTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[2] { this.treeListColumn1, this.treeListColumnUpgradeToPro });
		this.dbObjectsTreeList.KeyFieldName = "Id";
		this.dbObjectsTreeList.Location = new System.Drawing.Point(0, 24);
		this.dbObjectsTreeList.Margin = new System.Windows.Forms.Padding(0);
		this.dbObjectsTreeList.Name = "dbObjectsTreeList";
		this.dbObjectsTreeList.OptionsBehavior.AllowExpandOnDblClick = false;
		this.dbObjectsTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.dbObjectsTreeList.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.dbObjectsTreeList.OptionsView.AutoWidth = false;
		this.dbObjectsTreeList.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
		this.dbObjectsTreeList.OptionsView.ShowButtons = false;
		this.dbObjectsTreeList.OptionsView.ShowColumns = false;
		this.dbObjectsTreeList.OptionsView.ShowHorzLines = false;
		this.dbObjectsTreeList.OptionsView.ShowIndicator = false;
		this.dbObjectsTreeList.OptionsView.ShowVertLines = false;
		this.dbObjectsTreeList.ParentFieldName = "ParentId";
		this.dbObjectsTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit1, this.repositoryItemHyperLinkEditUpgradeToPro });
		this.dbObjectsTreeList.Size = new System.Drawing.Size(673, 465);
		this.dbObjectsTreeList.TabIndex = 4;
		this.dbObjectsTreeList.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(dbObjectsTreeList_BeforeCheckNode);
		this.dbObjectsTreeList.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(dbObjectsTreeList_AfterCheckNode);
		this.dbObjectsTreeList.CustomDrawNodeCheckBox += new DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventHandler(dbObjectsTreeList_CustomDrawNodeCheckBox);
		this.treeListColumn1.AppearanceCell.BackColor = System.Drawing.Color.Red;
		this.treeListColumn1.Caption = "Object class";
		this.treeListColumn1.FieldName = "Name";
		this.treeListColumn1.MinWidth = 32;
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.OptionsColumn.AllowEdit = false;
		this.treeListColumn1.OptionsColumn.ReadOnly = true;
		this.treeListColumn1.Visible = true;
		this.treeListColumn1.VisibleIndex = 0;
		this.treeListColumn1.Width = 260;
		this.treeListColumnUpgradeToPro.ColumnEdit = this.repositoryItemHyperLinkEditUpgradeToPro;
		this.treeListColumnUpgradeToPro.FieldName = "UpgradeToPro";
		this.treeListColumnUpgradeToPro.Name = "treeListColumnUpgradeToPro";
		this.treeListColumnUpgradeToPro.OptionsColumn.AllowEdit = false;
		this.treeListColumnUpgradeToPro.OptionsColumn.AllowFocus = false;
		this.treeListColumnUpgradeToPro.OptionsColumn.AllowMove = false;
		this.treeListColumnUpgradeToPro.OptionsColumn.AllowSize = false;
		this.treeListColumnUpgradeToPro.OptionsColumn.ReadOnly = true;
		this.treeListColumnUpgradeToPro.Visible = true;
		this.treeListColumnUpgradeToPro.VisibleIndex = 1;
		this.treeListColumnUpgradeToPro.Width = 140;
		this.repositoryItemHyperLinkEditUpgradeToPro.AllowFocused = false;
		this.repositoryItemHyperLinkEditUpgradeToPro.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.repositoryItemHyperLinkEditUpgradeToPro.Name = "repositoryItemHyperLinkEditUpgradeToPro";
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Caption = "Check";
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem9, this.layoutControlItem10, this.layoutControlItem11, this.emptySpaceItem8 });
		this.layoutControlGroup5.Name = "Root";
		this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup5.Size = new System.Drawing.Size(673, 489);
		this.layoutControlGroup5.TextVisible = false;
		this.layoutControlItem9.Control = this.dbObjectsTreeList;
		this.layoutControlItem9.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem9.Size = new System.Drawing.Size(673, 465);
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem10.Control = this.allObjectTypesHyperLinkEdit;
		this.layoutControlItem10.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem10.MaxSize = new System.Drawing.Size(25, 24);
		this.layoutControlItem10.MinSize = new System.Drawing.Size(25, 24);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.Size = new System.Drawing.Size(25, 24);
		this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.layoutControlItem11.Control = this.noneObjectTypesHyperLinkEdit;
		this.layoutControlItem11.Location = new System.Drawing.Point(25, 0);
		this.layoutControlItem11.MaxSize = new System.Drawing.Size(45, 24);
		this.layoutControlItem11.MinSize = new System.Drawing.Size(45, 24);
		this.layoutControlItem11.Name = "layoutControlItem11";
		this.layoutControlItem11.Size = new System.Drawing.Size(45, 24);
		this.layoutControlItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem11.TextVisible = false;
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.Location = new System.Drawing.Point(70, 0);
		this.emptySpaceItem8.Name = "emptySpaceItem8";
		this.emptySpaceItem8.Size = new System.Drawing.Size(603, 24);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.objectTypesLayoutControl);
		base.Name = "ChooseObjectTypesUserControl";
		base.Size = new System.Drawing.Size(673, 489);
		((System.ComponentModel.ISupportInitialize)this.objectTypesLayoutControl).EndInit();
		this.objectTypesLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.noneObjectTypesHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allObjectTypesHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbObjectsTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEditUpgradeToPro).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		base.ResumeLayout(false);
	}
}
