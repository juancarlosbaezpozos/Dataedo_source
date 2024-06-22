using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Forms.Helpers;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.ImportDescriptions.UserControls;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.ImportDescriptions;

public class DataToImportForm : BaseXtraForm
{
	private Form parentForm;

	private int databaseId;

	private SharedObjectTypeEnum.ObjectType objectType;

	private List<FieldDefinition> fieldDefinitions;

	private CustomFieldsSupport customFieldsSupport;

	private IContainer components;

	private RibbonControl ribbonControl;

	private BarButtonItem barButtonItem;

	private BarButtonItem pasteBarButtonItem;

	private BarButtonItem removeBarButtonItem;

	private BarButtonItem moveUpBarButtonItem;

	private BarButtonItem defaultSortBarButtonItem;

	private BarButtonItem moveDownBarButtonItem;

	private BarButtonItem copyTemplateBarButtonItem;

	private RibbonPage ribbonPage;

	private RibbonPageGroup manipulateRibbonPageGroup;

	private InfoUserControl infoUserControl;

	private NonCustomizableLayoutControl mainLayoutControl;

	private DataToImportUserControl dataToImportUserControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LayoutControlItem dataToImportUserControlLayoutControlItem;

	private EmptySpaceItem separatorEmptySpaceItem;

	private SimpleButton nextSimpleButton;

	private LayoutControlItem layoutControlItem1;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem beetwenButtonsEmptySpaceItem;

	private EmptySpaceItem leftButtonsEmptySpaceItem;

	private EmptySpaceItem rightButtonsEmptySpaceItem;

	private BarButtonItem clearBarButtonItem;

	public DataToImportForm(Form parentForm, int databaseId, SharedObjectTypeEnum.ObjectType objectType, List<FieldDefinition> fieldDefinitions, CustomFieldsSupport customFieldsSupport)
	{
		this.parentForm = parentForm;
		this.databaseId = databaseId;
		this.objectType = objectType;
		this.fieldDefinitions = fieldDefinitions;
		this.customFieldsSupport = customFieldsSupport;
		InitializeComponent();
		dataToImportUserControl.ValidityChangedEvent += delegate(object sender, BoolEventArgs e)
		{
			nextSimpleButton.Enabled = e.Value;
		};
		nextSimpleButton.Click += delegate
		{
			ProceedNext();
		};
		base.FormClosing += delegate(object sender, FormClosingEventArgs e)
		{
			BeforeClosing(e);
		};
		base.Shown += delegate
		{
			this.parentForm.Visible = false;
		};
		pasteBarButtonItem.ItemClick += delegate
		{
			PasteData();
		};
		removeBarButtonItem.ItemClick += delegate
		{
			RemoveSelectedRow();
		};
		copyTemplateBarButtonItem.ItemClick += delegate
		{
			CopyTemplate();
		};
		clearBarButtonItem.ItemClick += delegate
		{
			RemoveAllRecords();
		};
		dataToImportUserControl.Initialize(databaseId, this.objectType, fieldDefinitions);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Escape:
			Close();
			break;
		case Keys.Delete:
			RemoveSelectedRow();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void PasteData()
	{
		bool flag = false;
		if (dataToImportUserControl.HasData)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Do you want to overwrite all existing data?", "Paste", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, null, 1, this);
			if (handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				flag = true;
			}
			else
			{
				if (handlingDialogResult.DialogResult != DialogResult.No)
				{
					return;
				}
				flag = false;
			}
		}
		string argument = string.Empty;
		try
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			if (dataObject == null)
			{
				argument = string.Empty;
			}
			else if (dataObject.GetDataPresent(DataFormats.UnicodeText))
			{
				argument = dataObject.GetData(DataFormats.UnicodeText)?.ToString();
			}
		}
		catch (Exception)
		{
		}
		if (flag)
		{
			dataToImportUserControl.ClearExisting();
		}
		BackgroundWorker backgroundWorker = new BackgroundWorker
		{
			WorkerSupportsCancellation = true
		};
		backgroundWorker.DoWork += BackgroundWorker_DoWork;
		OverlayWithProgress.Show("Processing, please wait", this, new OverlayImagePainter(Resources.cancel_normal, Resources.cancel_active, delegate
		{
			backgroundWorker.CancelAsync();
		}));
		backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
		{
			WorkerCompleted(e);
		};
		backgroundWorker.RunWorkerAsync(argument);
	}

	private void WorkerCompleted(RunWorkerCompletedEventArgs e)
	{
		dataToImportUserControl.FinishSettingData();
		if ((bool)e.Result)
		{
			dataToImportUserControl.AfterAddingRows();
		}
		OverlayWithProgress.Close();
		dataToImportUserControl.ShowWarnings();
	}

	private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker backgroundWorker = sender as BackgroundWorker;
		string data = (string)e.Argument;
		dataToImportUserControl.SetData(data, backgroundWorker, e);
	}

	private void RemoveSelectedRow()
	{
		dataToImportUserControl.RemoveSelectedRow();
	}

	private void RemoveAllRecords()
	{
		if (dataToImportUserControl.HasData)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Are you sure you want to remove all records?", "Clear all", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, 1, this);
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				dataToImportUserControl.RemoveAllRows();
			}
		}
	}

	private void CopyTemplate()
	{
		ClipboardSupport.SetDataObject(dataToImportUserControl.GetTemplate());
	}

	private void ProceedNext()
	{
		if (new ChooseValuesToImportForm(this, databaseId, objectType, fieldDefinitions, customFieldsSupport, dataToImportUserControl.ModelsGeneral).ShowDialog() == DialogResult.OK)
		{
			base.DialogResult = DialogResult.OK;
		}
	}

	private void BeforeClosing(FormClosingEventArgs e)
	{
		if (base.DialogResult != DialogResult.OK && dataToImportUserControl.IsChanged)
		{
			if (GeneralMessageBoxesHandling.Show("There were changes? Do you want to save them?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult == DialogResult.No)
			{
				parentForm.Close();
				return;
			}
			base.DialogResult = DialogResult.Cancel;
			e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.ImportDescriptions.DataToImportForm));
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.barButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.pasteBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveUpBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.defaultSortBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveDownBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.copyTemplateBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.manipulateRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.infoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.nextSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.dataToImportUserControl = new Dataedo.App.ImportDescriptions.UserControls.DataToImportUserControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.dataToImportUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.beetwenButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.leftButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.rightButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataToImportUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightButtonsEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[10]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.barButtonItem,
			this.pasteBarButtonItem,
			this.removeBarButtonItem,
			this.moveUpBarButtonItem,
			this.defaultSortBarButtonItem,
			this.moveDownBarButtonItem,
			this.copyTemplateBarButtonItem,
			this.clearBarButtonItem
		});
		this.ribbonControl.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.ribbonControl.MaxItemId = 13;
		this.ribbonControl.Name = "ribbonControl";
		this.ribbonControl.OptionsPageCategories.ShowCaptions = false;
		this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage });
		this.ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
		this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.ribbonControl.ShowToolbarCustomizeItem = false;
		this.ribbonControl.Size = new System.Drawing.Size(1164, 101);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.barButtonItem.Caption = "barButtonItem1";
		this.barButtonItem.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
		this.barButtonItem.Id = 1;
		this.barButtonItem.Name = "barButtonItem";
		this.pasteBarButtonItem.Caption = "Paste";
		this.pasteBarButtonItem.Hint = "Paste data from clipboard into the grid";
		this.pasteBarButtonItem.Id = 2;
		this.pasteBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.paste_16;
		this.pasteBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.paste_32;
		this.pasteBarButtonItem.Name = "pasteBarButtonItem";
		this.removeBarButtonItem.Caption = "Remove row";
		this.removeBarButtonItem.Hint = "Remove selected grid's row";
		this.removeBarButtonItem.Id = 3;
		this.removeBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeBarButtonItem.Name = "removeBarButtonItem";
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
		this.copyTemplateBarButtonItem.Caption = "Copy headers";
		this.copyTemplateBarButtonItem.Hint = "Copy the column headers to the clipboard";
		this.copyTemplateBarButtonItem.Id = 11;
		this.copyTemplateBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.copy_16_alt;
		this.copyTemplateBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.copy_32_alt;
		this.copyTemplateBarButtonItem.Name = "copyTemplateBarButtonItem";
		this.clearBarButtonItem.Caption = "Clear all";
		this.clearBarButtonItem.Hint = "Clear all pasted data";
		this.clearBarButtonItem.Id = 12;
		this.clearBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.clearBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.clearBarButtonItem.Name = "clearBarButtonItem";
		this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.manipulateRibbonPageGroup });
		this.ribbonPage.Name = "ribbonPage";
		this.ribbonPage.Text = "ribbonPage";
		this.manipulateRibbonPageGroup.AllowTextClipping = false;
		this.manipulateRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.pasteBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.removeBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.clearBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.copyTemplateBarButtonItem);
		this.manipulateRibbonPageGroup.Name = "manipulateRibbonPageGroup";
		this.manipulateRibbonPageGroup.Text = "Actions";
		this.infoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl.Description = "You can paste definition below from clipboard. Copy the headers by clicking <b><i>Copy headers</b></i> button above.";
		this.infoUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.infoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.infoUserControl.Location = new System.Drawing.Point(0, 101);
		this.infoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.infoUserControl.MaximumSize = new System.Drawing.Size(0, 39);
		this.infoUserControl.MinimumSize = new System.Drawing.Size(658, 39);
		this.infoUserControl.Name = "infoUserControl";
		this.infoUserControl.Size = new System.Drawing.Size(1164, 39);
		this.infoUserControl.TabIndex = 11;
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.mainLayoutControl.Controls.Add(this.nextSimpleButton);
		this.mainLayoutControl.Controls.Add(this.dataToImportUserControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 140);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3732, 441, 756, 681);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(1164, 559);
		this.mainLayoutControl.TabIndex = 15;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(1051, 518);
		this.cancelSimpleButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(99, 27);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(99, 27);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(99, 27);
		this.cancelSimpleButton.StyleController = this.mainLayoutControl;
		this.cancelSimpleButton.TabIndex = 16;
		this.cancelSimpleButton.Text = "Cancel";
		this.nextSimpleButton.Enabled = false;
		this.nextSimpleButton.Location = new System.Drawing.Point(934, 518);
		this.nextSimpleButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.nextSimpleButton.MaximumSize = new System.Drawing.Size(99, 27);
		this.nextSimpleButton.MinimumSize = new System.Drawing.Size(99, 27);
		this.nextSimpleButton.Name = "nextSimpleButton";
		this.nextSimpleButton.Size = new System.Drawing.Size(99, 27);
		this.nextSimpleButton.StyleController = this.mainLayoutControl;
		this.nextSimpleButton.TabIndex = 15;
		this.nextSimpleButton.Text = "Next";
		this.dataToImportUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dataToImportUserControl.IsChanged = false;
		this.dataToImportUserControl.IsDataValid = false;
		this.dataToImportUserControl.Location = new System.Drawing.Point(0, 0);
		this.dataToImportUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.dataToImportUserControl.Name = "dataToImportUserControl";
		this.dataToImportUserControl.Size = new System.Drawing.Size(1164, 505);
		this.dataToImportUserControl.TabIndex = 14;
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.dataToImportUserControlLayoutControlItem, this.separatorEmptySpaceItem, this.layoutControlItem1, this.layoutControlItem2, this.beetwenButtonsEmptySpaceItem, this.leftButtonsEmptySpaceItem, this.rightButtonsEmptySpaceItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 11);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(1164, 559);
		this.mainLayoutControlGroup.TextVisible = false;
		this.dataToImportUserControlLayoutControlItem.Control = this.dataToImportUserControl;
		this.dataToImportUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.dataToImportUserControlLayoutControlItem.Name = "dataToImportUserControlLayoutControlItem";
		this.dataToImportUserControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.dataToImportUserControlLayoutControlItem.Size = new System.Drawing.Size(1164, 505);
		this.dataToImportUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.dataToImportUserControlLayoutControlItem.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 505);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 11);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(1, 11);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(1164, 11);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.Control = this.nextSimpleButton;
		this.layoutControlItem1.Location = new System.Drawing.Point(932, 516);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(103, 32);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.cancelSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(1049, 516);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(103, 32);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.beetwenButtonsEmptySpaceItem.AllowHotTrack = false;
		this.beetwenButtonsEmptySpaceItem.Location = new System.Drawing.Point(1035, 516);
		this.beetwenButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(14, 32);
		this.beetwenButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(14, 32);
		this.beetwenButtonsEmptySpaceItem.Name = "beetwenButtonsEmptySpaceItem";
		this.beetwenButtonsEmptySpaceItem.Size = new System.Drawing.Size(14, 32);
		this.beetwenButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.beetwenButtonsEmptySpaceItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.beetwenButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.leftButtonsEmptySpaceItem.AllowHotTrack = false;
		this.leftButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 516);
		this.leftButtonsEmptySpaceItem.Name = "leftButtonsEmptySpaceItem";
		this.leftButtonsEmptySpaceItem.Size = new System.Drawing.Size(932, 32);
		this.leftButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.rightButtonsEmptySpaceItem.AllowHotTrack = false;
		this.rightButtonsEmptySpaceItem.Location = new System.Drawing.Point(1152, 516);
		this.rightButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(12, 32);
		this.rightButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(12, 32);
		this.rightButtonsEmptySpaceItem.Name = "rightButtonsEmptySpaceItem";
		this.rightButtonsEmptySpaceItem.Size = new System.Drawing.Size(12, 32);
		this.rightButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rightButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1164, 699);
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.infoUserControl);
		base.Controls.Add(this.ribbonControl);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("DataToImportForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.MinimumSize = new System.Drawing.Size(300, 200);
		base.Name = "DataToImportForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Data to import";
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataToImportUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightButtonsEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
