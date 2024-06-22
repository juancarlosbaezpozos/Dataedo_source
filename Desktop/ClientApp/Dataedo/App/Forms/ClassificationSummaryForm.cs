using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classification.Forms;
using Dataedo.App.Helpers.Enums;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools.ClassificationSummary;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classificator;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Forms;

public class ClassificationSummaryForm : BaseXtraForm
{
	private bool isClosingEnabled = true;

	private readonly bool isEnabled;

	private IContainer components;

	private LayoutControlGroup layoutControlGroup;

	private SimpleButton cancelSimpleButton;

	private SimpleButton okSimpleButton;

	private ClassificationSummaryUserControl classificationSummaryUserControl;

	private LayoutControlItem mainControlLayoutControlItem;

	private LayoutControlItem saveBtnLayoutControlItem;

	private LayoutControlItem cancelBtnLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private NonCustomizableLayoutControl layoutControl;

	private SplashScreenManager splashScreenManager;

	private SimpleSeparator bottomSeparator;

	private SimpleSeparator topSeparator;

	public bool HasDisplayedChildWindow { get; private set; }

	public ClassificationSummaryForm()
	{
		InitializeComponent();
		isEnabled = Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification);
	}

	public ClassificationSummaryForm(CustomFieldsSupport support, int classificatorId, IEnumerable<ClassificatorCustomFieldContainer> selectedCustomFields)
		: this()
	{
		classificationSummaryUserControl.ColumnOperations = new Classificator(support, classificationSummaryUserControl)
		{
			ClassificatorId = classificatorId,
			SelectedCustomFields = selectedCustomFields
		};
		if (!isEnabled)
		{
			okSimpleButton.Enabled = false;
			okSimpleButton.Text = "Save (unavailable)";
			saveBtnLayoutControlItem.MaxSize = (saveBtnLayoutControlItem.MinSize = (saveBtnLayoutControlItem.Size = new Size(180, 22)));
			okSimpleButton.MaximumSize = (okSimpleButton.MinimumSize = (okSimpleButton.Size = new Size(170, 22)));
		}
	}

	public void SetParameters(IEnumerable<int> databasesIds)
	{
		classificationSummaryUserControl.SetParameters(databasesIds);
		classificationSummaryUserControl.SetBandedGridView();
		classificationSummaryUserControl.SetInfoBarVisibility();
		classificationSummaryUserControl.SetGridPanel();
	}

	private void OkSimpleButton_Click(object sender, EventArgs e)
	{
		Save();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Escape:
			if (!classificationSummaryUserControl.IsFilterPopupMenuShown)
			{
				Close();
			}
			else
			{
				classificationSummaryUserControl.IsFilterPopupMenuShown = false;
			}
			break;
		case Keys.S | Keys.Control:
			if (base.Enabled)
			{
				Save();
			}
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void Save()
	{
		isClosingEnabled = false;
		base.Enabled = false;
		classificationSummaryUserControl.Save();
	}

	public void AfterSaving()
	{
		isClosingEnabled = true;
		base.Enabled = true;
	}

	private void CancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void ClassificationSummaryForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (!isClosingEnabled || HasDisplayedChildWindow)
		{
			e.Cancel = true;
		}
		else if (!isEnabled)
		{
			base.DialogResult = DialogResult.No;
		}
		else if (base.DialogResult != DialogResult.OK && classificationSummaryUserControl.HasChanges)
		{
			HasDisplayedChildWindow = true;
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Data has been changed, would you like to save these changes?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				Save();
			}
			else if (dialogResult != DialogResult.No)
			{
				HasDisplayedChildWindow = false;
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	public async Task LoadClassificationsAsync(Form owner)
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		try
		{
			ProgressWaitFormInvoker.SetProgressWaitFormCancellationToken(cancellationTokenSource);
			ProgressWaitFormInvoker.SwitchWaitFormToProgress(13, "Classification in progress...");
			await classificationSummaryUserControl.GetClassificationsAsync(cancellationTokenSource.Token);
			AfterClassificationLoaded(ResultEnum.Success, owner);
		}
		catch (Exception ex)
		{
			if (ex is OperationCanceledException || ex?.InnerException is OperationCanceledException || ex?.InnerException?.InnerException is OperationCanceledException || cancellationTokenSource.IsCancellationRequested)
			{
				ProgressWaitFormInvoker.SetAfterError("The classification was canceled by user.", delegate
				{
					AfterClassificationLoaded(ResultEnum.Canceled, owner);
				});
			}
			else if (CheckIfTimeoutException(ex) || CheckIfTimeoutException(ex?.InnerException) || CheckIfTimeoutException(ex?.InnerException?.InnerException))
			{
				ProgressWaitFormInvoker.SetAfterError("Query time outed. Please increase timeouts and try again.", delegate
				{
					AfterClassificationLoaded(ResultEnum.Error, owner);
				});
			}
			else
			{
				ProgressWaitFormInvoker.CloseProgressWaitForm();
				GeneralExceptionHandling.Handle(ex, owner);
				AfterClassificationLoaded(ResultEnum.Error, owner);
			}
		}
	}

	public void AfterClassificationLoaded(ResultEnum result, Form owner)
	{
		ProgressWaitFormInvoker.CloseProgressWaitForm();
		owner.Invoke((Action)delegate
		{
			(owner as ClassificationForm)?.ClassificationLoaded(this, result);
		});
	}

	private static bool CheckIfTimeoutException(Exception ex)
	{
		if (ex is SqlException ex2 && ex2.Number == -2)
		{
			return true;
		}
		return false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ClassificationSummaryForm));
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.classificationSummaryUserControl = new Dataedo.App.UserControls.ClassificationSummaryUserControl();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.mainControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveBtnLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelBtnLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bottomSeparator = new DevExpress.XtraLayout.SimpleSeparator();
		this.topSeparator = new DevExpress.XtraLayout.SimpleSeparator();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveBtnLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelBtnLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparator).BeginInit();
		base.SuspendLayout();
		this.splashScreenManager.ClosingDelay = 500;
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.cancelSimpleButton);
		this.layoutControl.Controls.Add(this.okSimpleButton);
		this.layoutControl.Controls.Add(this.classificationSummaryUserControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(886, 271, 650, 400);
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(1200, 770);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.cancelSimpleButton.AllowFocus = false;
		this.cancelSimpleButton.Location = new System.Drawing.Point(1100, 742);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl;
		this.cancelSimpleButton.TabIndex = 6;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(CancelSimpleButton_Click);
		this.okSimpleButton.AllowFocus = false;
		this.okSimpleButton.Location = new System.Drawing.Point(1000, 742);
		this.okSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.okSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.okSimpleButton.StyleController = this.layoutControl;
		this.okSimpleButton.TabIndex = 5;
		this.okSimpleButton.Text = "Save";
		this.okSimpleButton.Click += new System.EventHandler(OkSimpleButton_Click);
		this.classificationSummaryUserControl.BackColor = System.Drawing.Color.Transparent;
		this.classificationSummaryUserControl.ColumnOperations = null;
		this.classificationSummaryUserControl.IsFilterPopupMenuShown = false;
		this.classificationSummaryUserControl.Location = new System.Drawing.Point(2, 3);
		this.classificationSummaryUserControl.Name = "classificationSummaryUserControl";
		this.classificationSummaryUserControl.Padding = new System.Windows.Forms.Padding(10);
		this.classificationSummaryUserControl.Size = new System.Drawing.Size(1196, 730);
		this.classificationSummaryUserControl.TabIndex = 4;
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.mainControlLayoutControlItem, this.saveBtnLayoutControlItem, this.cancelBtnLayoutControlItem, this.bottomEmptySpaceItem, this.bottomSeparator, this.topSeparator });
		this.layoutControlGroup.Name = "layoutControlGroup";
		this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup.Size = new System.Drawing.Size(1200, 770);
		this.layoutControlGroup.TextVisible = false;
		this.mainControlLayoutControlItem.Control = this.classificationSummaryUserControl;
		this.mainControlLayoutControlItem.Location = new System.Drawing.Point(0, 1);
		this.mainControlLayoutControlItem.Name = "mainControlLayoutControlItem";
		this.mainControlLayoutControlItem.Size = new System.Drawing.Size(1200, 734);
		this.mainControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.mainControlLayoutControlItem.TextVisible = false;
		this.saveBtnLayoutControlItem.Control = this.okSimpleButton;
		this.saveBtnLayoutControlItem.Location = new System.Drawing.Point(1000, 736);
		this.saveBtnLayoutControlItem.MaxSize = new System.Drawing.Size(100, 34);
		this.saveBtnLayoutControlItem.MinSize = new System.Drawing.Size(100, 34);
		this.saveBtnLayoutControlItem.Name = "saveBtnLayoutControlItem";
		this.saveBtnLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 14, 6, 6);
		this.saveBtnLayoutControlItem.Size = new System.Drawing.Size(100, 34);
		this.saveBtnLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveBtnLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveBtnLayoutControlItem.TextVisible = false;
		this.cancelBtnLayoutControlItem.Control = this.cancelSimpleButton;
		this.cancelBtnLayoutControlItem.Location = new System.Drawing.Point(1100, 736);
		this.cancelBtnLayoutControlItem.MaxSize = new System.Drawing.Size(100, 34);
		this.cancelBtnLayoutControlItem.MinSize = new System.Drawing.Size(100, 34);
		this.cancelBtnLayoutControlItem.Name = "cancelBtnLayoutControlItem";
		this.cancelBtnLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 14, 6, 6);
		this.cancelBtnLayoutControlItem.Size = new System.Drawing.Size(100, 34);
		this.cancelBtnLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelBtnLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelBtnLayoutControlItem.TextVisible = false;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 736);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(1000, 34);
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomSeparator.AllowHotTrack = false;
		this.bottomSeparator.Location = new System.Drawing.Point(0, 735);
		this.bottomSeparator.Name = "bottomSeparator";
		this.bottomSeparator.Size = new System.Drawing.Size(1200, 1);
		this.topSeparator.AllowHotTrack = false;
		this.topSeparator.Location = new System.Drawing.Point(0, 0);
		this.topSeparator.Name = "topSeparator";
		this.topSeparator.Size = new System.Drawing.Size(1200, 1);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1200, 770);
		base.Controls.Add(this.layoutControl);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ClassificationSummaryForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "ClassificationSummaryForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Data Classification";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ClassificationSummaryForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveBtnLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelBtnLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparator).EndInit();
		base.ResumeLayout(false);
	}
}
