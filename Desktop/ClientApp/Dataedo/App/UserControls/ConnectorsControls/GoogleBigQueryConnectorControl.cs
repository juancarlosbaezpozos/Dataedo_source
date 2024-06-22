using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class GoogleBigQueryConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl googleBigQueryLayoutControl;

	private ButtonEdit googleBigQueryHostButtonEdit;

	private ButtonEdit googleBigQueryFileButtonEdit;

	private LabelControl googleBigQueryDatabaseCountLabelControl;

	private ButtonEdit googleBigQueryDatasetButtonEdit;

	private LayoutControlGroup googleBigQueryRoot;

	private EmptySpaceItem bigQueryBottomEmptySpaceItem;

	private EmptySpaceItem bigQueryTimeoutEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem59;

	private LayoutControlItem googleBigQueryFileLayoutControl;

	private LayoutControlItem googleBigQueryHostLayoutControlItem;

	private LayoutControlItem layoutControlItem20;

	private EmptySpaceItem emptySpaceItem29;

	private EmptySpaceItem googleBigQueryDatabaseCountEmptySpaceItem;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.GoogleBigQuery;

	protected override TextEdit HostTextEdit => googleBigQueryHostButtonEdit;

	public GoogleBigQueryConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		List<string> source = googleBigQueryDatasetButtonEdit.EditValue?.ToString().Split(',').ToList() ?? new List<string>();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? googleBigQueryDatasetButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, googleBigQueryHostButtonEdit.Text, googleBigQueryFileButtonEdit.Text, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, null, source.Count() > 1, source);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		googleBigQueryLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			googleBigQueryLayoutControl.Root.AddItem(timeoutLayoutControlItem, bigQueryTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateGoogleBigQueryLogin();
		flag &= ValidateGoogleBigQueryHost();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateGoogleBigQuerySchema();
		}
		return flag;
	}

	private bool ValidateGoogleBigQueryHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(googleBigQueryHostButtonEdit, addDBErrorProvider, "project", acceptEmptyValue);
	}

	private bool ValidateGoogleBigQueryLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(googleBigQueryFileButtonEdit, addDBErrorProvider, "credentials path", acceptEmptyValue);
	}

	private bool ValidateGoogleBigQuerySchema(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(googleBigQueryDatasetButtonEdit, addDBErrorProvider, "dataset", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		googleBigQueryHostButtonEdit.Text = base.DatabaseRow.Host;
		googleBigQueryFileButtonEdit.Text = base.DatabaseRow.User;
		googleBigQueryDatasetButtonEdit.Text = base.DatabaseRow.Name;
		schemas = new List<string>(base.DatabaseRow.Schemas ?? new List<string>());
		SetSchemasButtonEdit(googleBigQueryDatasetButtonEdit);
		SetElementsLabelControl(googleBigQueryDatabaseCountLabelControl);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return googleBigQueryDatasetButtonEdit.Text + "@" + googleBigQueryHostButtonEdit.Text;
	}

	private void googleBigQueryDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		HandleSchemaButtonEdit(sender as ButtonEdit, testConnection: true, "Databases", null, "Dataset list");
	}

	private void googleBigQueryDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateGoogleBigQuerySchema();
	}

	private void googleBigQueryFileButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateGoogleBigQueryLogin();
	}

	private void googleBigQueryFileButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		string empty = string.Empty;
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			empty = openFileDialog.FileName;
			googleBigQueryFileButtonEdit.Text = empty;
		}
	}

	private void googleBigQueryHostButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateGoogleBigQueryHost();
	}

	private void googleBigQueryHostButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (!ValidateGoogleBigQueryLogin())
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			return;
		}
		SetNewDBRowValues(forGettingDatabasesList: true);
		GoogleBigQuerySupport googleBigQuerySupport = new GoogleBigQuerySupport();
		if (!googleBigQuerySupport.ValidateCredentialPath(base.DatabaseRow.ConnectionString, FindForm()))
		{
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		List<string> projectsId = googleBigQuerySupport.GetProjectsId(base.DatabaseRow.ConnectionString, base.SplashScreenManager, FindForm());
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (projectsId == null || projectsId.Count() == 0)
		{
			return;
		}
		string empty = string.Empty;
		if (projectsId == null)
		{
			return;
		}
		try
		{
			base.DatabaseRow.Name = string.Empty;
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
			if (connectionResult.Exception != null)
			{
				throw connectionResult.Exception;
			}
			string title = "Projects list";
			ListForm listForm = new ListForm(projectsId, title);
			if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
			{
				(sender as ButtonEdit).EditValue = listForm.SelectedValue;
			}
		}
		catch (Exception ex)
		{
			DatabaseSupportFactory.GetDatabaseSupport(base.DatabaseRow.Type).ProcessException(ex, base.DatabaseRow.Name, base.DatabaseRow.ServiceName, FindForm());
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		finally
		{
			base.DatabaseRow.Name = empty;
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
		this.googleBigQueryDatasetButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.googleBigQueryLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.googleBigQueryHostButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.googleBigQueryFileButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.googleBigQueryDatabaseCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.googleBigQueryRoot = new DevExpress.XtraLayout.LayoutControlGroup();
		this.bigQueryBottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bigQueryTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem59 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.googleBigQueryFileLayoutControl = new DevExpress.XtraLayout.LayoutControlItem();
		this.googleBigQueryHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem29 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.googleBigQueryDatabaseCountEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		DevExpress.XtraLayout.LayoutControlItem layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)layoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryDatasetButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryLayoutControl).BeginInit();
		this.googleBigQueryLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryHostButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryFileButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryRoot).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bigQueryBottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bigQueryTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem59).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryFileLayoutControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryDatabaseCountEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		layoutControlItem.Control = this.googleBigQueryDatasetButtonEdit;
		layoutControlItem.Location = new System.Drawing.Point(0, 48);
		layoutControlItem.MaxSize = new System.Drawing.Size(0, 16);
		layoutControlItem.MinSize = new System.Drawing.Size(176, 16);
		layoutControlItem.Name = "googleBigQueryDatasetLayoutControl";
		layoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		layoutControlItem.Size = new System.Drawing.Size(335, 24);
		layoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		layoutControlItem.Text = "Dataset:";
		layoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		layoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		layoutControlItem.TextToControlDistance = 5;
		this.googleBigQueryDatasetButtonEdit.Location = new System.Drawing.Point(105, 48);
		this.googleBigQueryDatasetButtonEdit.Name = "googleBigQueryDatasetButtonEdit";
		this.googleBigQueryDatasetButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.googleBigQueryDatasetButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.googleBigQueryDatasetButtonEdit.StyleController = this.googleBigQueryLayoutControl;
		this.googleBigQueryDatasetButtonEdit.TabIndex = 7;
		this.googleBigQueryDatasetButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(googleBigQueryDatabaseButtonEdit_ButtonClick);
		this.googleBigQueryDatasetButtonEdit.EditValueChanged += new System.EventHandler(googleBigQueryDatabaseButtonEdit_EditValueChanged);
		this.googleBigQueryLayoutControl.AllowCustomization = false;
		this.googleBigQueryLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.googleBigQueryLayoutControl.Controls.Add(this.googleBigQueryHostButtonEdit);
		this.googleBigQueryLayoutControl.Controls.Add(this.googleBigQueryFileButtonEdit);
		this.googleBigQueryLayoutControl.Controls.Add(this.googleBigQueryDatabaseCountLabelControl);
		this.googleBigQueryLayoutControl.Controls.Add(this.googleBigQueryDatasetButtonEdit);
		this.googleBigQueryLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.googleBigQueryLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.googleBigQueryLayoutControl.Name = "googleBigQueryLayoutControl";
		this.googleBigQueryLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3284, 270, 919, 626);
		this.googleBigQueryLayoutControl.Root = this.googleBigQueryRoot;
		this.googleBigQueryLayoutControl.Size = new System.Drawing.Size(532, 432);
		this.googleBigQueryLayoutControl.TabIndex = 1;
		this.googleBigQueryLayoutControl.Text = "layoutControl3";
		this.googleBigQueryHostButtonEdit.Location = new System.Drawing.Point(105, 24);
		this.googleBigQueryHostButtonEdit.Name = "googleBigQueryHostButtonEdit";
		this.googleBigQueryHostButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.googleBigQueryHostButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.googleBigQueryHostButtonEdit.StyleController = this.googleBigQueryLayoutControl;
		this.googleBigQueryHostButtonEdit.TabIndex = 10;
		this.googleBigQueryHostButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(googleBigQueryHostButtonEdit_ButtonClick);
		this.googleBigQueryHostButtonEdit.EditValueChanged += new System.EventHandler(googleBigQueryHostButtonEdit_EditValueChanged);
		this.googleBigQueryFileButtonEdit.Location = new System.Drawing.Point(105, 0);
		this.googleBigQueryFileButtonEdit.Name = "googleBigQueryFileButtonEdit";
		this.googleBigQueryFileButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.googleBigQueryFileButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.googleBigQueryFileButtonEdit.StyleController = this.googleBigQueryLayoutControl;
		this.googleBigQueryFileButtonEdit.TabIndex = 9;
		this.googleBigQueryFileButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(googleBigQueryFileButtonEdit_ButtonClick);
		this.googleBigQueryFileButtonEdit.EditValueChanged += new System.EventHandler(googleBigQueryFileButtonEdit_EditValueChanged);
		this.googleBigQueryDatabaseCountLabelControl.Location = new System.Drawing.Point(346, 50);
		this.googleBigQueryDatabaseCountLabelControl.Name = "googleBigQueryDatabaseCountLabelControl";
		this.googleBigQueryDatabaseCountLabelControl.Size = new System.Drawing.Size(105, 13);
		this.googleBigQueryDatabaseCountLabelControl.StyleController = this.googleBigQueryLayoutControl;
		this.googleBigQueryDatabaseCountLabelControl.TabIndex = 8;
		this.googleBigQueryRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.googleBigQueryRoot.GroupBordersVisible = false;
		this.googleBigQueryRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.bigQueryBottomEmptySpaceItem, this.bigQueryTimeoutEmptySpaceItem, layoutControlItem, this.emptySpaceItem59, this.googleBigQueryFileLayoutControl, this.googleBigQueryHostLayoutControlItem, this.layoutControlItem20, this.emptySpaceItem29, this.googleBigQueryDatabaseCountEmptySpaceItem });
		this.googleBigQueryRoot.Name = "Root";
		this.googleBigQueryRoot.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.googleBigQueryRoot.Size = new System.Drawing.Size(532, 432);
		this.googleBigQueryRoot.TextVisible = false;
		this.bigQueryBottomEmptySpaceItem.AllowHotTrack = false;
		this.bigQueryBottomEmptySpaceItem.Location = new System.Drawing.Point(0, 156);
		this.bigQueryBottomEmptySpaceItem.Name = "bigQueryBottomEmptySpaceItem";
		this.bigQueryBottomEmptySpaceItem.Size = new System.Drawing.Size(532, 276);
		this.bigQueryBottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bigQueryTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.bigQueryTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 72);
		this.bigQueryTimeoutEmptySpaceItem.Name = "bigQueryTimeoutEmptySpaceItem";
		this.bigQueryTimeoutEmptySpaceItem.Size = new System.Drawing.Size(532, 84);
		this.bigQueryTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem59.AllowHotTrack = false;
		this.emptySpaceItem59.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem59.Name = "emptySpaceItem59";
		this.emptySpaceItem59.Size = new System.Drawing.Size(197, 48);
		this.emptySpaceItem59.TextSize = new System.Drawing.Size(0, 0);
		this.googleBigQueryFileLayoutControl.Control = this.googleBigQueryFileButtonEdit;
		this.googleBigQueryFileLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.googleBigQueryFileLayoutControl.MaxSize = new System.Drawing.Size(335, 24);
		this.googleBigQueryFileLayoutControl.MinSize = new System.Drawing.Size(335, 24);
		this.googleBigQueryFileLayoutControl.Name = "googleBigQueryFileLayoutControl";
		this.googleBigQueryFileLayoutControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.googleBigQueryFileLayoutControl.Size = new System.Drawing.Size(335, 24);
		this.googleBigQueryFileLayoutControl.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.googleBigQueryFileLayoutControl.Text = "Service Access Key:";
		this.googleBigQueryFileLayoutControl.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.googleBigQueryFileLayoutControl.TextSize = new System.Drawing.Size(100, 13);
		this.googleBigQueryFileLayoutControl.TextToControlDistance = 5;
		this.googleBigQueryHostLayoutControlItem.Control = this.googleBigQueryHostButtonEdit;
		this.googleBigQueryHostLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.googleBigQueryHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.googleBigQueryHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.googleBigQueryHostLayoutControlItem.Name = "googleBigQueryHostLayoutControlItem";
		this.googleBigQueryHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.googleBigQueryHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.googleBigQueryHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.googleBigQueryHostLayoutControlItem.Text = "Project:";
		this.googleBigQueryHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.googleBigQueryHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.googleBigQueryHostLayoutControlItem.TextToControlDistance = 5;
		this.layoutControlItem20.Control = this.googleBigQueryDatabaseCountLabelControl;
		this.layoutControlItem20.Location = new System.Drawing.Point(345, 48);
		this.layoutControlItem20.Name = "layoutControlItem20";
		this.layoutControlItem20.Size = new System.Drawing.Size(107, 24);
		this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem20.TextVisible = false;
		this.emptySpaceItem29.AllowHotTrack = false;
		this.emptySpaceItem29.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem29.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem29.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem29.Name = "emptySpaceItem29";
		this.emptySpaceItem29.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem29.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem29.TextSize = new System.Drawing.Size(0, 0);
		this.googleBigQueryDatabaseCountEmptySpaceItem.AllowHotTrack = false;
		this.googleBigQueryDatabaseCountEmptySpaceItem.Location = new System.Drawing.Point(452, 48);
		this.googleBigQueryDatabaseCountEmptySpaceItem.Name = "googleBigQueryDatabaseCountEmptySpaceItem";
		this.googleBigQueryDatabaseCountEmptySpaceItem.Size = new System.Drawing.Size(80, 24);
		this.googleBigQueryDatabaseCountEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.googleBigQueryLayoutControl);
		base.Name = "GoogleBigQueryConnectorControl";
		base.Size = new System.Drawing.Size(532, 432);
		((System.ComponentModel.ISupportInitialize)layoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryDatasetButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryLayoutControl).EndInit();
		this.googleBigQueryLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryHostButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryFileButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryRoot).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bigQueryBottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bigQueryTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem59).EndInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryFileLayoutControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.googleBigQueryDatabaseCountEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
