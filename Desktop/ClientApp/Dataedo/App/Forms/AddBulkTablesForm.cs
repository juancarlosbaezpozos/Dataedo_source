using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class AddBulkTablesForm : BaseXtraForm
{
	private int databaseId;

	private SharedObjectTypeEnum.ObjectType objectType;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl1;

	private InfoUserControl infoUserControl1;

	private SimpleButton addSimpleButton;

	private SimpleButton cancelSimpleButton;

	private LayoutControlGroup layoutControlGroup1;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem layoutControlItem7;

	private AddBulkTablesUserControl addBulkTablesUserControl;

	private LayoutControlItem layoutControlItem8;

	private RibbonControl ribbonControl;

	private RibbonPage ribbonPage1;

	private RibbonPageGroup ribbonPageGroup;

	private LayoutControlItem ribbonLayoutControlItem;

	private BarButtonItem pasteBarButtonItem;

	private BarButtonItem removeBarButtonItem;

	private BarButtonItem copyBarButtonItem;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	public BulkTableInserter Inserter { get; private set; }

	public List<BulkTableModel> Columns => addBulkTablesUserControl.ColumnsList;

	public AddBulkTablesForm()
	{
		InitializeComponent();
		ribbonControl.Manager.UseAltKeyForMenu = false;
	}

	public AddBulkTablesForm(int databaseId, SharedObjectTypeEnum.ObjectType objectType)
		: this()
	{
		this.databaseId = databaseId;
		this.objectType = objectType;
		addBulkTablesUserControl.Initialize(objectType);
		infoUserControl1.Description = "You can paste " + SharedObjectTypeEnum.TypeToStringForMenu(objectType) + " definition below from clipboard. Copy the template by clicking <b>Copy template</b> button above.";
		Text = "Bulk add " + SharedObjectTypeEnum.TypeToStringForMenu(objectType);
		Inserter = new BulkTableInserter(this.databaseId, Columns, objectType);
	}

	public void Insert(DBTreeNode node)
	{
		Inserter.AddTableModels();
		Inserter.InsertTables(this);
		Inserter.AddTablesToTree();
		Inserter.InsertIntoModules(node);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			base.DialogResult = DialogResult.OK;
			Close();
			break;
		case Keys.Escape:
			Close();
			break;
		case Keys.Delete:
			addBulkTablesUserControl.Remove();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void Save()
	{
		Columns.ForEach(delegate(BulkTableModel x)
		{
			x.CheckIfNameIsEmpty();
		});
		base.DialogResult = ((!addBulkTablesUserControl.IsNotValid()) ? DialogResult.OK : DialogResult.None);
	}

	private void copyBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		addBulkTablesUserControl.CopyTemplate();
	}

	private void pasteBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		addBulkTablesUserControl.Paste();
	}

	private void removeBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		addBulkTablesUserControl.Remove();
	}

	private void AddBulkTablesForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
		}
		else if (addBulkTablesUserControl.IsChanged && base.DialogResult != DialogResult.OK)
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Columns has been changed, would you like to save these changes?", "Data has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				Save();
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void addSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.AddBulkTablesForm));
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.pasteBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.copyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.ribbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.addBulkTablesUserControl = new Dataedo.App.UserControls.AddBulkTablesUserControl();
		this.infoUserControl1 = new Dataedo.App.UserControls.InfoUserControl();
		this.addSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.ribbonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.Controls.Add(this.ribbonControl);
		this.layoutControl1.Controls.Add(this.addBulkTablesUserControl);
		this.layoutControl1.Controls.Add(this.infoUserControl1);
		this.layoutControl1.Controls.Add(this.addSimpleButton);
		this.layoutControl1.Controls.Add(this.cancelSimpleButton);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(1089, 591);
		this.layoutControl1.TabIndex = 1;
		this.layoutControl1.Text = "layoutControl1";
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.Dock = System.Windows.Forms.DockStyle.None;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[5]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.pasteBarButtonItem,
			this.removeBarButtonItem,
			this.copyBarButtonItem
		});
		this.ribbonControl.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.ribbonControl.MaxItemId = 15;
		this.ribbonControl.Name = "ribbonControl";
		this.ribbonControl.OptionsPageCategories.ShowCaptions = false;
		this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage1 });
		this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.ribbonControl.ShowToolbarCustomizeItem = false;
		this.ribbonControl.Size = new System.Drawing.Size(1089, 102);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.pasteBarButtonItem.Caption = "Paste";
		this.pasteBarButtonItem.Id = 12;
		this.pasteBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.paste_16;
		this.pasteBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.paste_32;
		this.pasteBarButtonItem.Name = "pasteBarButtonItem";
		this.pasteBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(pasteBarButtonItem_ItemClick);
		this.removeBarButtonItem.Caption = "Remove";
		this.removeBarButtonItem.Id = 13;
		this.removeBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeBarButtonItem.Name = "removeBarButtonItem";
		this.removeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeBarButtonItem_ItemClick);
		this.copyBarButtonItem.Caption = "Copy template";
		this.copyBarButtonItem.Id = 14;
		this.copyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.copy_16_alt;
		this.copyBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.copy_32_alt;
		this.copyBarButtonItem.Name = "copyBarButtonItem";
		this.copyBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(copyBarButtonItem_ItemClick);
		this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.ribbonPageGroup });
		this.ribbonPage1.Name = "ribbonPage1";
		this.ribbonPage1.Text = "ribbonPage1";
		this.ribbonPageGroup.AllowTextClipping = false;
		this.ribbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonPageGroup.ItemLinks.Add(this.pasteBarButtonItem);
		this.ribbonPageGroup.ItemLinks.Add(this.removeBarButtonItem);
		this.ribbonPageGroup.ItemLinks.Add(this.copyBarButtonItem);
		this.ribbonPageGroup.Name = "ribbonPageGroup";
		this.ribbonPageGroup.Text = "Actions";
		this.addBulkTablesUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.addBulkTablesUserControl.IsChanged = false;
		this.addBulkTablesUserControl.Location = new System.Drawing.Point(0, 134);
		this.addBulkTablesUserControl.Name = "addBulkTablesUserControl";
		this.addBulkTablesUserControl.Size = new System.Drawing.Size(1089, 413);
		this.addBulkTablesUserControl.TabIndex = 11;
		this.infoUserControl1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl1.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl1.Description = "You can paste tables definition below from clipboard. Copy the template by clicking <b>Copy template</b> button above.";
		this.infoUserControl1.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl1.Image = Dataedo.App.Properties.Resources.about_16;
		this.infoUserControl1.Location = new System.Drawing.Point(0, 102);
		this.infoUserControl1.Margin = new System.Windows.Forms.Padding(0);
		this.infoUserControl1.MaximumSize = new System.Drawing.Size(0, 32);
		this.infoUserControl1.MinimumSize = new System.Drawing.Size(564, 32);
		this.infoUserControl1.Name = "infoUserControl1";
		this.infoUserControl1.Size = new System.Drawing.Size(1089, 32);
		this.infoUserControl1.TabIndex = 10;
		this.addSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.addSimpleButton.Location = new System.Drawing.Point(892, 558);
		this.addSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.addSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.addSimpleButton.Name = "addSimpleButton";
		this.addSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.addSimpleButton.StyleController = this.layoutControl1;
		this.addSimpleButton.TabIndex = 6;
		this.addSimpleButton.Text = "Add";
		this.addSimpleButton.Click += new System.EventHandler(addSimpleButton_Click);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(992, 558);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl1;
		this.cancelSimpleButton.TabIndex = 5;
		this.cancelSimpleButton.Text = "Cancel";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.emptySpaceItem1, this.layoutControlItem2, this.emptySpaceItem2, this.layoutControlItem7, this.layoutControlItem8, this.ribbonLayoutControlItem, this.layoutControlItem3, this.emptySpaceItem5, this.emptySpaceItem3, this.emptySpaceItem4 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1089, 591);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 556);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 26);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(890, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.cancelSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(990, 556);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(979, 556);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(11, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(11, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(11, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.Control = this.infoUserControl1;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 102);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem7.Size = new System.Drawing.Size(1089, 32);
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem8.Control = this.addBulkTablesUserControl;
		this.layoutControlItem8.Location = new System.Drawing.Point(0, 134);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem8.Size = new System.Drawing.Size(1089, 413);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.ribbonLayoutControlItem.Control = this.ribbonControl;
		this.ribbonLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.ribbonLayoutControlItem.MaxSize = new System.Drawing.Size(0, 102);
		this.ribbonLayoutControlItem.MinSize = new System.Drawing.Size(104, 102);
		this.ribbonLayoutControlItem.Name = "ribbonLayoutControlItem";
		this.ribbonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ribbonLayoutControlItem.Size = new System.Drawing.Size(1089, 102);
		this.ribbonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ribbonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.ribbonLayoutControlItem.TextVisible = false;
		this.layoutControlItem3.Control = this.addSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(890, 556);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 582);
		this.emptySpaceItem5.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem5.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(1089, 9);
		this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem5.Text = "emptySpaceItem1";
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 547);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(1089, 9);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.Text = "emptySpaceItem1";
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem4.Location = new System.Drawing.Point(1079, 556);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.Text = "emptySpaceItem2";
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1089, 591);
		base.Controls.Add(this.layoutControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("AddBulkTablesForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "AddBulkTablesForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Bulk add tables";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AddBulkTablesForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		this.layoutControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		base.ResumeLayout(false);
	}
}
