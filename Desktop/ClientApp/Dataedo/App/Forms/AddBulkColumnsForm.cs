using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class AddBulkColumnsForm : BaseXtraForm
{
	private int tableId;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private EmptySpaceItem emptySpaceItem1;

	private SimpleButton addSimpleButton;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem2;

	private AddColumnsUserControl addColumnsUserControl;

	private RibbonControl ribbonControl;

	private BarButtonItem barButtonItem1;

	private BarButtonItem pasteBarButtonItem;

	private BarButtonItem removeBarButtonItem;

	private BarButtonItem moveUpBarButtonItem;

	private BarButtonItem defaultSortBarButtonItem;

	private BarButtonItem moveDownBarButtonItem;

	private RibbonPage ribbonPage;

	private RibbonPageGroup manipulateRibbonPageGroup;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem4;

	private BarButtonItem copyTemplateBarButtonItem;

	private EmptySpaceItem emptySpaceItem3;

	private InfoUserControl infoUserControl1;

	private LayoutControlItem infoUserControlLayoutControlItem;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem5;

	private SharedObjectTypeEnum.ObjectType objectType { get; set; }

	public List<ColumnRow> DataSourceColumns => addColumnsUserControl.DataSourceColumns;

	public AddBulkColumnsForm()
	{
		InitializeComponent();
	}

	public AddBulkColumnsForm(SharedObjectTypeEnum.ObjectType objectType, int tableId, int existingColumnsCount, CustomFieldsSupport customFieldsSupport)
	{
		InitializeComponent();
		this.objectType = objectType;
		this.tableId = tableId;
		addColumnsUserControl.TableId = this.tableId;
		addColumnsUserControl.ExistingColumnsCount = existingColumnsCount;
		ribbonControl.Manager.UseAltKeyForMenu = false;
		addColumnsUserControl.Initialize(objectType, customFieldsSupport);
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
			addColumnsUserControl.Remove();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void simpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void AddBulkColumnsForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (addColumnsUserControl.IsChanged && base.DialogResult != DialogResult.OK)
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Columns has been changed, would you like to save these changes?", "Columns has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				base.DialogResult = DialogResult.OK;
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void saveSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
	}

	private void pasteBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		addColumnsUserControl.Paste();
	}

	private void removeBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		addColumnsUserControl.Remove();
	}

	private void copyTemplateBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		addColumnsUserControl.CopyTemplate();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.AddBulkColumnsForm));
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.infoUserControl1 = new Dataedo.App.UserControls.InfoUserControl();
		this.addColumnsUserControl = new Dataedo.App.UserControls.WindowControls.AddColumnsUserControl();
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.pasteBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveUpBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.defaultSortBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveDownBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.copyTemplateBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.manipulateRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.addSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.infoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.Controls.Add(this.infoUserControl1);
		this.layoutControl1.Controls.Add(this.addColumnsUserControl);
		this.layoutControl1.Controls.Add(this.ribbonControl);
		this.layoutControl1.Controls.Add(this.addSimpleButton);
		this.layoutControl1.Controls.Add(this.cancelSimpleButton);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(909, 441);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.infoUserControl1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl1.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl1.Description = "You can paste table columns definition below from clipboard. Copy the template by clicking <b>Copy template</b> button above.";
		this.infoUserControl1.Image = Dataedo.App.Properties.Resources.about_16;
		this.infoUserControl1.Location = new System.Drawing.Point(0, 102);
		this.infoUserControl1.MaximumSize = new System.Drawing.Size(0, 32);
		this.infoUserControl1.MinimumSize = new System.Drawing.Size(564, 32);
		this.infoUserControl1.Name = "infoUserControl1";
		this.infoUserControl1.Size = new System.Drawing.Size(909, 32);
		this.infoUserControl1.TabIndex = 10;
		this.addColumnsUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.addColumnsUserControl.ExistingColumnsCount = 0;
		this.addColumnsUserControl.Location = new System.Drawing.Point(0, 134);
		this.addColumnsUserControl.Name = "addColumnsUserControl";
		this.addColumnsUserControl.Size = new System.Drawing.Size(909, 263);
		this.addColumnsUserControl.TabIndex = 4;
		this.addColumnsUserControl.TableId = 0;
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.Dock = System.Windows.Forms.DockStyle.None;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[9]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.barButtonItem1,
			this.pasteBarButtonItem,
			this.removeBarButtonItem,
			this.moveUpBarButtonItem,
			this.defaultSortBarButtonItem,
			this.moveDownBarButtonItem,
			this.copyTemplateBarButtonItem
		});
		this.ribbonControl.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.ribbonControl.MaxItemId = 12;
		this.ribbonControl.Name = "ribbonControl";
		this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage });
		this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowCategoryInCaption = false;
		this.ribbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.ribbonControl.ShowToolbarCustomizeItem = false;
		this.ribbonControl.Size = new System.Drawing.Size(909, 102);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.barButtonItem1.Caption = "barButtonItem1";
		this.barButtonItem1.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
		this.barButtonItem1.Id = 1;
		this.barButtonItem1.Name = "barButtonItem1";
		this.pasteBarButtonItem.Caption = "Paste";
		this.pasteBarButtonItem.Id = 2;
		this.pasteBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.paste_16;
		this.pasteBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.paste_32;
		this.pasteBarButtonItem.Name = "pasteBarButtonItem";
		this.pasteBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(pasteBarButtonItem_ItemClick);
		this.removeBarButtonItem.Caption = "Remove";
		this.removeBarButtonItem.Id = 3;
		this.removeBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeBarButtonItem.Name = "removeBarButtonItem";
		this.removeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeBarButtonItem_ItemClick);
		this.moveUpBarButtonItem.Caption = "Move up";
		this.moveUpBarButtonItem.Id = 4;
		this.moveUpBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
		this.moveUpBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_up_32;
		this.moveUpBarButtonItem.Name = "moveUpBarButtonItem";
		this.defaultSortBarButtonItem.Caption = "DefaultSort";
		this.defaultSortBarButtonItem.Id = 7;
		this.defaultSortBarButtonItem.Name = "defaultSortBarButtonItem";
		this.moveDownBarButtonItem.Caption = "Move down";
		this.moveDownBarButtonItem.Id = 9;
		this.moveDownBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
		this.moveDownBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_down_32;
		this.moveDownBarButtonItem.Name = "moveDownBarButtonItem";
		this.copyTemplateBarButtonItem.Caption = "Copy template";
		this.copyTemplateBarButtonItem.Id = 11;
		this.copyTemplateBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.copy_16_alt;
		this.copyTemplateBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.copy_32_alt;
		this.copyTemplateBarButtonItem.Name = "copyTemplateBarButtonItem";
		this.copyTemplateBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(copyTemplateBarButtonItem_ItemClick);
		this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.manipulateRibbonPageGroup });
		this.ribbonPage.Name = "ribbonPage";
		this.ribbonPage.Text = "ribbonPage";
		this.manipulateRibbonPageGroup.AllowTextClipping = false;
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.pasteBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.removeBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.copyTemplateBarButtonItem);
		this.manipulateRibbonPageGroup.Name = "manipulateRibbonPageGroup";
		this.manipulateRibbonPageGroup.ShowCaptionButton = false;
		this.manipulateRibbonPageGroup.Text = "Actions";
		this.addSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.addSimpleButton.Location = new System.Drawing.Point(712, 408);
		this.addSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.addSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.addSimpleButton.Name = "addSimpleButton";
		this.addSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.addSimpleButton.StyleController = this.layoutControl1;
		this.addSimpleButton.TabIndex = 6;
		this.addSimpleButton.Text = "Add";
		this.addSimpleButton.Click += new System.EventHandler(saveSimpleButton_Click);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(812, 408);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl1;
		this.cancelSimpleButton.TabIndex = 5;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(simpleButton_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.emptySpaceItem1, this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem2, this.layoutControlItem1, this.layoutControlItem4, this.infoUserControlLayoutControlItem, this.emptySpaceItem3, this.emptySpaceItem4, this.emptySpaceItem5 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(909, 441);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 406);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 26);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(710, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.cancelSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(810, 406);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.addSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(710, 406);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(799, 406);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(11, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(11, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(11, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.Control = this.ribbonControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 102);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(104, 102);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem1.Size = new System.Drawing.Size(909, 102);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem4.Control = this.addColumnsUserControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 134);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem4.Size = new System.Drawing.Size(909, 263);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.infoUserControlLayoutControlItem.Control = this.infoUserControl1;
		this.infoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 102);
		this.infoUserControlLayoutControlItem.Name = "infoUserControlLayoutControlItem";
		this.infoUserControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.infoUserControlLayoutControlItem.Size = new System.Drawing.Size(909, 32);
		this.infoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.infoUserControlLayoutControlItem.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(899, 406);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 432);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(0, 9);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 9);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(909, 9);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 397);
		this.emptySpaceItem5.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem5.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(909, 9);
		this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem5.Text = "emptySpaceItem1";
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(909, 441);
		base.Controls.Add(this.layoutControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("AddBulkColumnsForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "AddBulkColumnsForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add bulk columns";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AddBulkColumnsForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		this.layoutControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		base.ResumeLayout(false);
	}
}
