using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.ImportDescriptions.Processing.Saving;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.ImportDescriptions.UserControls;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.ImportDescriptions;

public class ChooseValuesToImportForm : BaseXtraForm
{
	private Form parentForm;

	private SharedObjectTypeEnum.ObjectType objectType;

	private CustomFieldsSupport customFieldsSupport;

	private List<ImportDataModel> modelsGeneral;

	private int databaseId;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup Root;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem layoutControlItem1;

	private SimpleButton saveSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem leftButtonsEmptySpaceItem;

	private EmptySpaceItem beetwenButtonsEmptySpaceItem;

	private EmptySpaceItem rightButtonsEmptySpaceItem;

	private EmptySpaceItem separatorEmptySpaceItem;

	private CheckingBeforeSavingUserControl checkingBeforeSavingUserControl;

	private LayoutControlItem checkingBeforeSavingUserControlLayoutControlItem;

	public ChooseValuesToImportForm(Form parentForm, int databaseId, SharedObjectTypeEnum.ObjectType objectType, List<FieldDefinition> fieldDefinitions, CustomFieldsSupport customFieldsSupport, List<ImportDataModel> modelsGeneral)
	{
		this.parentForm = parentForm;
		this.objectType = objectType;
		this.customFieldsSupport = customFieldsSupport;
		this.databaseId = databaseId;
		IEnumerable<ImportDataModel> enumerable = modelsGeneral.Where((ImportDataModel x) => !x.IsAnyChanged);
		int ignoredCounter = enumerable.Count();
		modelsGeneral = modelsGeneral.Except(enumerable).ToList();
		this.modelsGeneral = modelsGeneral;
		InitializeComponent();
		saveSimpleButton.Click += delegate
		{
			SaveData();
		};
		base.FormClosing += delegate(object sender, FormClosingEventArgs e)
		{
			BeforeClosing(e);
		};
		base.Shown += delegate
		{
			this.parentForm.Visible = false;
		};
		checkingBeforeSavingUserControl.Initialize(objectType, fieldDefinitions, modelsGeneral, ignoredCounter);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void BeforeClosing(FormClosingEventArgs e)
	{
		if (base.DialogResult != DialogResult.OK)
		{
			if (GeneralMessageBoxesHandling.Show("There were changes? Do you want to save them?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation).DialogResult == DialogResult.No)
			{
				parentForm.Visible = true;
				return;
			}
			base.DialogResult = DialogResult.Cancel;
			e.Cancel = true;
		}
	}

	private void SaveData()
	{
		ISaveProcessorBase saveProcessorBase;
		if (objectType == SharedObjectTypeEnum.ObjectType.Table)
		{
			saveProcessorBase = new SaveTablesProcessor(customFieldsSupport, modelsGeneral);
		}
		else
		{
			if (objectType != SharedObjectTypeEnum.ObjectType.Column)
			{
				throw new ArgumentException($"Provided type ({objectType}) is not valid argument.");
			}
			saveProcessorBase = new SaveColumnsProcessor(customFieldsSupport, modelsGeneral);
		}
		if (saveProcessorBase.ProcessSaving(databaseId, this))
		{
			base.DialogResult = DialogResult.OK;
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.checkingBeforeSavingUserControl = new Dataedo.App.ImportDescriptions.UserControls.CheckingBeforeSavingUserControl();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.leftButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.beetwenButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.rightButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.checkingBeforeSavingUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkingBeforeSavingUserControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.checkingBeforeSavingUserControl);
		this.mainLayoutControl.Controls.Add(this.saveSimpleButton);
		this.mainLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(167, 428, 727, 625);
		this.mainLayoutControl.Root = this.Root;
		this.mainLayoutControl.Size = new System.Drawing.Size(1514, 699);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.checkingBeforeSavingUserControl.BackColor = System.Drawing.Color.Transparent;
		this.checkingBeforeSavingUserControl.Location = new System.Drawing.Point(2, 2);
		this.checkingBeforeSavingUserControl.Margin = new System.Windows.Forms.Padding(5);
		this.checkingBeforeSavingUserControl.Name = "checkingBeforeSavingUserControl";
		this.checkingBeforeSavingUserControl.Size = new System.Drawing.Size(1510, 640);
		this.checkingBeforeSavingUserControl.TabIndex = 19;
		this.saveSimpleButton.Location = new System.Drawing.Point(1284, 658);
		this.saveSimpleButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.saveSimpleButton.MaximumSize = new System.Drawing.Size(99, 27);
		this.saveSimpleButton.MinimumSize = new System.Drawing.Size(99, 27);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(99, 27);
		this.saveSimpleButton.StyleController = this.mainLayoutControl;
		this.saveSimpleButton.TabIndex = 18;
		this.saveSimpleButton.Text = "Save";
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(1401, 658);
		this.cancelSimpleButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(99, 27);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(99, 27);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(99, 27);
		this.cancelSimpleButton.StyleController = this.mainLayoutControl;
		this.cancelSimpleButton.TabIndex = 17;
		this.cancelSimpleButton.Text = "Cancel";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.layoutControlItem1, this.layoutControlItem2, this.leftButtonsEmptySpaceItem, this.beetwenButtonsEmptySpaceItem, this.rightButtonsEmptySpaceItem, this.separatorEmptySpaceItem, this.checkingBeforeSavingUserControlLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 11);
		this.Root.Size = new System.Drawing.Size(1514, 699);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.cancelSimpleButton;
		this.layoutControlItem1.Location = new System.Drawing.Point(1399, 656);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(103, 32);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.saveSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(1282, 656);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(103, 32);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.leftButtonsEmptySpaceItem.AllowHotTrack = false;
		this.leftButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 656);
		this.leftButtonsEmptySpaceItem.Name = "leftButtonsEmptySpaceItem";
		this.leftButtonsEmptySpaceItem.Size = new System.Drawing.Size(1282, 32);
		this.leftButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.beetwenButtonsEmptySpaceItem.AllowHotTrack = false;
		this.beetwenButtonsEmptySpaceItem.Location = new System.Drawing.Point(1385, 656);
		this.beetwenButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(14, 32);
		this.beetwenButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(14, 32);
		this.beetwenButtonsEmptySpaceItem.Name = "beetwenButtonsEmptySpaceItem";
		this.beetwenButtonsEmptySpaceItem.Size = new System.Drawing.Size(14, 32);
		this.beetwenButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.beetwenButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.rightButtonsEmptySpaceItem.AllowHotTrack = false;
		this.rightButtonsEmptySpaceItem.Location = new System.Drawing.Point(1502, 656);
		this.rightButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(12, 32);
		this.rightButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(12, 32);
		this.rightButtonsEmptySpaceItem.Name = "rightButtonsEmptySpaceItem";
		this.rightButtonsEmptySpaceItem.Size = new System.Drawing.Size(12, 32);
		this.rightButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rightButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 644);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(12, 12);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(12, 12);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(1514, 12);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.checkingBeforeSavingUserControlLayoutControlItem.Control = this.checkingBeforeSavingUserControl;
		this.checkingBeforeSavingUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.checkingBeforeSavingUserControlLayoutControlItem.Name = "checkingBeforeSavingUserControlLayoutControlItem";
		this.checkingBeforeSavingUserControlLayoutControlItem.Size = new System.Drawing.Size(1514, 644);
		this.checkingBeforeSavingUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.checkingBeforeSavingUserControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1514, 699);
		base.Controls.Add(this.mainLayoutControl);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.MinimumSize = new System.Drawing.Size(300, 200);
		base.Name = "ChooseValuesToImportForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Values to import";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkingBeforeSavingUserControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
