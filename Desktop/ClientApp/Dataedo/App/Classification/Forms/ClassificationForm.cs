using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classification.UserControls;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Helpers;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Helpers.Enums;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classificator;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Data.CustomFields;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Classification.Forms;

public class ClassificationForm : BaseXtraForm
{
	private MetadataEditorUserControl metadataEditorUserControl;

	private List<ClassificationStats> classificatorStats;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private DatabasesSelectorUserControl databasesSelectorUserControl;

	private SimpleButton configureClassificationButton;

	private ClassificatorPresenterUserControl classificatorPresenterUserControl;

	private ClassificationChooserUserControl classificationChooserUserControl;

	private LayoutControlItem classificationChooserLayoutControlItem;

	private LayoutControlItem classificatorPresenterLayoutControlItem;

	private LayoutControlItem configureClassificationButtonLayoutControlItem;

	private EmptySpaceItem emptySpaceItem;

	private LayoutControlItem databasesSelectorLayoutControlItem;

	private SimpleButton runClassificationButton;

	private LayoutControlItem runClassificationButtonLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private SimpleSeparator simpleSeparator1;

	private SimpleSeparator simpleSeparator2;

	private ToolTipController toolTipController;

	private SimpleButton exploreButton;

	private LayoutControlItem exploreButtonLayoutControlItem;

	private CustomFieldsSupport CustomFieldsSupport => metadataEditorUserControl.CustomFieldsSupport;

	public ClassificatorModel ChoosenClassificator => classificationChooserUserControl.ChoosenClassificator;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.ExStyle |= 33554432;
			return obj;
		}
	}

	public ClassificationForm()
	{
		InitializeComponent();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public void SetParameters(MetadataEditorUserControl metadataEditorUserControl)
	{
		this.metadataEditorUserControl = metadataEditorUserControl;
		int? databases = this.metadataEditorUserControl?.GetFocusedNode()?.DatabaseId;
		databasesSelectorUserControl.DatabasesSelectionChanged += DatabasesSelectionChanged;
		databasesSelectorUserControl.SetDatabases(databases);
		classificationChooserUserControl.SetParameters(CustomFieldsSupport);
		classificationChooserUserControl.ClassificationChanged += ClassificationChanged;
		classificationChooserUserControl.InitDataSource();
	}

	private void DatabasesSelectionChanged()
	{
		if (ChoosenClassificator != null && databasesSelectorUserControl.SelectedDatabasesIds.Any())
		{
			runClassificationButton.Enabled = true;
			runClassificationButton.SuperTip?.Items?.Clear();
		}
		else
		{
			runClassificationButton.Enabled = false;
			ButtonsHelpers.AddSuperTip(runClassificationButton, (ChoosenClassificator == null) ? "Please select data classification first." : "Please select some databases first.");
		}
	}

	private void ClassificationChanged()
	{
		if (ChoosenClassificator != null)
		{
			LayoutControlItem layoutControlItem = classificatorPresenterLayoutControlItem;
			LayoutControlItem layoutControlItem2 = configureClassificationButtonLayoutControlItem;
			LayoutControlItem layoutControlItem3 = exploreButtonLayoutControlItem;
			LayoutControlItem layoutControlItem4 = databasesSelectorLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (classificatorPresenterLayoutControlItem.Visibility = LayoutVisibility.Always);
			LayoutVisibility layoutVisibility4 = (layoutControlItem4.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility6 = (layoutControlItem3.Visibility = layoutVisibility4);
			LayoutVisibility layoutVisibility9 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility6));
			runClassificationButton?.SuperTip?.Items?.Clear();
			DatabasesSelectionChanged();
			string customFieldFieldName = ChoosenClassificator?.UsedFields?.FirstOrDefault()?.CustomFieldFieldName;
			classificatorStats = DB.Classificator.GetClassificationStats(customFieldFieldName);
			classificatorPresenterUserControl.SetParameters(ChoosenClassificator, classificatorStats);
			databasesSelectorUserControl.SetParameters(classificatorStats);
		}
	}

	private void ConfigureClassificationButton_Click(object sender, EventArgs e)
	{
		classificationChooserUserControl.ConfigureClassification(ChoosenClassificator);
	}

	private async void RunClassificationButton_Click(object sender, EventArgs e)
	{
		try
		{
			ProgressWaitFormSettings progressWaitFormSettings = new ProgressWaitFormSettings
			{
				FormTitle = "Classification in progress...",
				ProgressLabel = "Preparing custom fields...",
				Picture = Resources.classified
			};
			ProgressWaitFormInvoker.ShowMarqueeProgressWaitForm(this, progressWaitFormSettings);
			if (ChoosenClassificator == null)
			{
				ProgressWaitFormInvoker.CloseProgressWaitForm();
			}
			else if (VerifyCustomFieldsAvailability(ChoosenClassificator))
			{
				CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
				List<ClassificatorCustomFieldContainer> customFields = DB.Classificator.GetCustomFields(CustomFieldsSupport, ChoosenClassificator);
				if (customFields == null || !customFields.Any())
				{
					ProgressWaitFormInvoker.CloseProgressWaitForm();
					GeneralMessageBoxesHandling.Show("The selected classification <i>" + ChoosenClassificator.Title + "</i> doesn't have any Classification Fields. Please add some fields first to run it.", "No Classification Fields", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
					return;
				}
				ClassificationSummaryForm classificationSummaryForm = new ClassificationSummaryForm(CustomFieldsSupport, ChoosenClassificator.Id, customFields);
				classificationSummaryForm.SetParameters(databasesSelectorUserControl.SelectedDatabasesIds);
				base.Enabled = false;
				ClassificationTrackingHelper.TrackClassificationRun(databasesSelectorUserControl.SelectedDatabasesIds.Count(), databasesSelectorUserControl.NumberOfColumnsInSelectedDatabases);
				await classificationSummaryForm.LoadClassificationsAsync(this);
			}
		}
		catch (Exception exception)
		{
			base.Enabled = true;
			ProgressWaitFormInvoker.CloseProgressWaitForm();
			GeneralExceptionHandling.Handle(exception, "Error while running classification", FindForm());
		}
	}

	public void ClassificationLoaded(ClassificationSummaryForm classificationSummaryForm, ResultEnum result)
	{
		try
		{
			base.Enabled = true;
			switch (result)
			{
			case ResultEnum.Success:
				base.Visible = false;
				classificationSummaryForm.ShowDialog(this);
				base.Visible = true;
				if (classificationSummaryForm.DialogResult == DialogResult.OK)
				{
					metadataEditorUserControl.LoadCustomFields(loadDefinitionValues: true);
					metadataEditorUserControl.OpenPageControl(showControl: false);
					metadataEditorUserControl.RefreshSearchCustomFields();
				}
				classificationChooserUserControl.InitDataSource(keepFocus: true);
				break;
			case ResultEnum.Canceled:
				ClassificationTrackingHelper.TrackClassificationCanceled(databasesSelectorUserControl.SelectedDatabasesIds.Count(), databasesSelectorUserControl.NumberOfColumnsInSelectedDatabases);
				break;
			case ResultEnum.Error:
				ClassificationTrackingHelper.TrackClassificationFailed(databasesSelectorUserControl.SelectedDatabasesIds.Count(), databasesSelectorUserControl.NumberOfColumnsInSelectedDatabases);
				break;
			}
		}
		catch (Exception exception)
		{
			base.Visible = true;
			GeneralExceptionHandling.Handle(exception, "Error while running classification", this);
		}
		finally
		{
			classificationSummaryForm.Dispose();
		}
	}

	private bool VerifyCustomFieldsAvailability(ClassificatorModel model)
	{
		IEnumerable<int> first = from x in DB.CustomField.GetCustomFields(null, false)
			select x.CustomFieldId;
		IEnumerable<int> second = from x in model.Fields
			where x.Id.HasValue
			select x.Id.Value;
		int num = first.Except(second).Count();
		int num2 = model.Fields.Count((ClassificatorCustomField x) => !string.IsNullOrEmpty(x.Title));
		int num3 = num + num2;
		if (num3 > 100)
		{
			string arg = ((num3 - 100 == 1) ? "custom field" : "custom fields");
			ProgressWaitFormInvoker.CloseProgressWaitForm();
			GeneralMessageBoxesHandling.Show("Too many custom fields in use to run " + model.Title + "." + Environment.NewLine + $"Remove at least {num3 - 100} {arg} " + "to continue with classification.", "Not enough unused custom fields", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
			return false;
		}
		return true;
	}

	private void NonCustomizableLayoutControl_MouseMove(object sender, MouseEventArgs e)
	{
		Control childAtPoint = nonCustomizableLayoutControl.GetChildAtPoint(e.Location);
		if (!runClassificationButton.Enabled)
		{
			if (childAtPoint == runClassificationButton)
			{
				ToolTipControllerShowEventArgs eShow = new ToolTipControllerShowEventArgs
				{
					SuperTip = runClassificationButton.SuperTip,
					ToolTipType = ToolTipType.SuperTip
				};
				toolTipController.ShowHint(eShow, Control.MousePosition);
			}
			else
			{
				toolTipController.HideHint();
			}
		}
	}

	private void ExploreButton_Click(object sender, EventArgs e)
	{
		try
		{
			using (ClassificationExplorerForm classificationExplorerForm = new ClassificationExplorerForm())
			{
				classificationExplorerForm.SetParameters(metadataEditorUserControl.CustomFieldsSupport, ChoosenClassificator.Id);
				base.Visible = false;
				classificationExplorerForm.ShowDialog(this);
			}
			base.Visible = true;
		}
		catch (Exception exception)
		{
			base.Visible = true;
			GeneralExceptionHandling.Handle(exception, "Error while running classification explorer", this);
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.runClassificationButton = new DevExpress.XtraEditors.SimpleButton();
		this.databasesSelectorUserControl = new Dataedo.App.Classification.UserControls.DatabasesSelectorUserControl();
		this.configureClassificationButton = new DevExpress.XtraEditors.SimpleButton();
		this.classificatorPresenterUserControl = new Dataedo.App.Classification.UserControls.ClassificatorPresenterUserControl();
		this.classificationChooserUserControl = new Dataedo.App.Classification.UserControls.ClassificationChooserUserControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.classificationChooserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.classificatorPresenterLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.configureClassificationButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.databasesSelectorLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.runClassificationButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
		this.simpleSeparator2 = new DevExpress.XtraLayout.SimpleSeparator();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.exploreButton = new DevExpress.XtraEditors.SimpleButton();
		this.exploreButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.classificationChooserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.classificatorPresenterLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.configureClassificationButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databasesSelectorLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.runClassificationButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleSeparator1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.simpleSeparator2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.exploreButtonLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.runClassificationButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.databasesSelectorUserControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.configureClassificationButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.classificatorPresenterUserControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.classificationChooserUserControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.exploreButton);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1486, 181, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(715, 492);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.MouseMove += new System.Windows.Forms.MouseEventHandler(NonCustomizableLayoutControl_MouseMove);
		this.runClassificationButton.AllowFocus = false;
		this.runClassificationButton.Enabled = false;
		this.runClassificationButton.Location = new System.Drawing.Point(574, 464);
		this.runClassificationButton.Margin = new System.Windows.Forms.Padding(10);
		this.runClassificationButton.Name = "runClassificationButton";
		this.runClassificationButton.Size = new System.Drawing.Size(127, 22);
		this.runClassificationButton.StyleController = this.nonCustomizableLayoutControl;
		this.runClassificationButton.TabIndex = 8;
		this.runClassificationButton.Text = "Run Classification";
		this.runClassificationButton.Click += new System.EventHandler(RunClassificationButton_Click);
		this.databasesSelectorUserControl.BackColor = System.Drawing.Color.Transparent;
		this.databasesSelectorUserControl.Location = new System.Drawing.Point(362, 3);
		this.databasesSelectorUserControl.Name = "databasesSelectorUserControl";
		this.databasesSelectorUserControl.Size = new System.Drawing.Size(351, 452);
		this.databasesSelectorUserControl.TabIndex = 7;
		this.configureClassificationButton.AllowFocus = false;
		this.configureClassificationButton.Location = new System.Drawing.Point(205, 429);
		this.configureClassificationButton.Name = "configureClassificationButton";
		this.configureClassificationButton.Size = new System.Drawing.Size(141, 22);
		this.configureClassificationButton.StyleController = this.nonCustomizableLayoutControl;
		this.configureClassificationButton.TabIndex = 6;
		this.configureClassificationButton.Text = "Configure Classification";
		this.configureClassificationButton.Click += new System.EventHandler(ConfigureClassificationButton_Click);
		this.classificatorPresenterUserControl.BackColor = System.Drawing.Color.Transparent;
		this.classificatorPresenterUserControl.Location = new System.Drawing.Point(2, 226);
		this.classificatorPresenterUserControl.Name = "classificatorPresenterUserControl";
		this.classificatorPresenterUserControl.Size = new System.Drawing.Size(356, 199);
		this.classificatorPresenterUserControl.TabIndex = 5;
		this.classificationChooserUserControl.BackColor = System.Drawing.Color.Transparent;
		this.classificationChooserUserControl.Location = new System.Drawing.Point(2, 3);
		this.classificationChooserUserControl.Name = "classificationChooserUserControl";
		this.classificationChooserUserControl.Size = new System.Drawing.Size(356, 219);
		this.classificationChooserUserControl.TabIndex = 4;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.classificationChooserLayoutControlItem, this.classificatorPresenterLayoutControlItem, this.configureClassificationButtonLayoutControlItem, this.emptySpaceItem, this.databasesSelectorLayoutControlItem, this.runClassificationButtonLayoutControlItem, this.emptySpaceItem1, this.simpleSeparator1, this.simpleSeparator2, this.exploreButtonLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(715, 492);
		this.Root.TextVisible = false;
		this.classificationChooserLayoutControlItem.Control = this.classificationChooserUserControl;
		this.classificationChooserLayoutControlItem.Location = new System.Drawing.Point(0, 1);
		this.classificationChooserLayoutControlItem.MaxSize = new System.Drawing.Size(360, 223);
		this.classificationChooserLayoutControlItem.MinSize = new System.Drawing.Size(360, 223);
		this.classificationChooserLayoutControlItem.Name = "classificationChooserLayoutControlItem";
		this.classificationChooserLayoutControlItem.Size = new System.Drawing.Size(360, 223);
		this.classificationChooserLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.classificationChooserLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.classificationChooserLayoutControlItem.TextVisible = false;
		this.classificatorPresenterLayoutControlItem.Control = this.classificatorPresenterUserControl;
		this.classificatorPresenterLayoutControlItem.Location = new System.Drawing.Point(0, 224);
		this.classificatorPresenterLayoutControlItem.Name = "classificatorPresenterLayoutControlItem";
		this.classificatorPresenterLayoutControlItem.Size = new System.Drawing.Size(360, 203);
		this.classificatorPresenterLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.classificatorPresenterLayoutControlItem.TextVisible = false;
		this.classificatorPresenterLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.configureClassificationButtonLayoutControlItem.Control = this.configureClassificationButton;
		this.configureClassificationButtonLayoutControlItem.Location = new System.Drawing.Point(203, 427);
		this.configureClassificationButtonLayoutControlItem.MaxSize = new System.Drawing.Size(157, 30);
		this.configureClassificationButtonLayoutControlItem.MinSize = new System.Drawing.Size(157, 30);
		this.configureClassificationButtonLayoutControlItem.Name = "configureClassificationButtonLayoutControlItem";
		this.configureClassificationButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 14, 2, 6);
		this.configureClassificationButtonLayoutControlItem.Size = new System.Drawing.Size(157, 30);
		this.configureClassificationButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.configureClassificationButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.configureClassificationButtonLayoutControlItem.TextVisible = false;
		this.configureClassificationButtonLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem.AllowHotTrack = false;
		this.emptySpaceItem.Location = new System.Drawing.Point(0, 427);
		this.emptySpaceItem.Name = "emptySpaceItem";
		this.emptySpaceItem.Size = new System.Drawing.Size(123, 30);
		this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.databasesSelectorLayoutControlItem.Control = this.databasesSelectorUserControl;
		this.databasesSelectorLayoutControlItem.Location = new System.Drawing.Point(360, 1);
		this.databasesSelectorLayoutControlItem.Name = "databasesSelectorLayoutControlItem";
		this.databasesSelectorLayoutControlItem.Size = new System.Drawing.Size(355, 456);
		this.databasesSelectorLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.databasesSelectorLayoutControlItem.TextVisible = false;
		this.databasesSelectorLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.runClassificationButtonLayoutControlItem.Control = this.runClassificationButton;
		this.runClassificationButtonLayoutControlItem.Location = new System.Drawing.Point(560, 458);
		this.runClassificationButtonLayoutControlItem.MaxSize = new System.Drawing.Size(155, 34);
		this.runClassificationButtonLayoutControlItem.MinSize = new System.Drawing.Size(155, 34);
		this.runClassificationButtonLayoutControlItem.Name = "runClassificationButtonLayoutControlItem";
		this.runClassificationButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(14, 14, 6, 6);
		this.runClassificationButtonLayoutControlItem.Size = new System.Drawing.Size(155, 34);
		this.runClassificationButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.runClassificationButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.runClassificationButtonLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 458);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(560, 34);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.simpleSeparator1.AllowHotTrack = false;
		this.simpleSeparator1.Location = new System.Drawing.Point(0, 0);
		this.simpleSeparator1.Name = "simpleSeparator1";
		this.simpleSeparator1.Size = new System.Drawing.Size(715, 1);
		this.simpleSeparator2.AllowHotTrack = false;
		this.simpleSeparator2.Location = new System.Drawing.Point(0, 457);
		this.simpleSeparator2.Name = "simpleSeparator2";
		this.simpleSeparator2.Size = new System.Drawing.Size(715, 1);
		this.exploreButton.AllowFocus = false;
		this.exploreButton.Location = new System.Drawing.Point(125, 429);
		this.exploreButton.Name = "exploreButton";
		this.exploreButton.Size = new System.Drawing.Size(64, 22);
		this.exploreButton.StyleController = this.nonCustomizableLayoutControl;
		this.exploreButton.TabIndex = 6;
		this.exploreButton.Text = "Explore";
		this.exploreButton.Click += new System.EventHandler(ExploreButton_Click);
		this.exploreButtonLayoutControlItem.Control = this.exploreButton;
		this.exploreButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.exploreButtonLayoutControlItem.CustomizationFormText = "exploreButtonLayoutControlItem";
		this.exploreButtonLayoutControlItem.Location = new System.Drawing.Point(123, 427);
		this.exploreButtonLayoutControlItem.MaxSize = new System.Drawing.Size(80, 30);
		this.exploreButtonLayoutControlItem.MinSize = new System.Drawing.Size(80, 30);
		this.exploreButtonLayoutControlItem.Name = "exploreButtonLayoutControlItem";
		this.exploreButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.exploreButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.exploreButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.exploreButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 14, 2, 6);
		this.exploreButtonLayoutControlItem.Size = new System.Drawing.Size(80, 30);
		this.exploreButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.exploreButtonLayoutControlItem.Text = "exploreButtonLayoutControlItem";
		this.exploreButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.exploreButtonLayoutControlItem.TextVisible = false;
		this.exploreButtonLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(715, 492);
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.MaximizeBox = false;
		base.Name = "ClassificationForm";
		this.Text = "Classification";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.classificationChooserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.classificatorPresenterLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.configureClassificationButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databasesSelectorLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.runClassificationButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleSeparator1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.simpleSeparator2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.exploreButtonLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
