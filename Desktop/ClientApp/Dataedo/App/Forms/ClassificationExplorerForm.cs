using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools.ClassificationExplorer;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Classificator;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraWaitForm;

namespace Dataedo.App.Forms;

public class ClassificationExplorerForm : BaseXtraForm
{
	private bool isClosingEnabled = true;

	private int focusedClassificatorId;

	private List<ClassificationExplorerCustomFieldsItem> dataSource;

	private IContainer components;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private ClassificationExplorerUserControl classificationExplorerUserControl;

	private LayoutControlItem layoutControlItem;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem1;

	private Panel progressPanelPanel;

	private PanelControl progressPanelControl;

	private NonCustomizableLayoutControl progressLayoutControl;

	private ProgressPanel progressPanel;

	private LayoutControlGroup progressLayoutControlGroup;

	private LayoutControlItem progressLayoutControlItem;

	private LookUpEdit classificationsLookUpEdit;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem3;

	private InfoUserControl infoUserControl;

	private LayoutControlItem infoUserControlLayoutControlItem;

	private NonCustomizableLayoutControl layoutControl;

	private BindingSource classificationExplorerCustomFieldsItemBindingSource;

	public ClassificationExplorerForm()
	{
		InitializeComponent();
		dataSource = new List<ClassificationExplorerCustomFieldsItem>();
		infoUserControlLayoutControlItem.Visibility = (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification) ? LayoutVisibility.Never : LayoutVisibility.Always);
	}

	public void SetParameters(CustomFieldsSupport support, int focusedClassificatorId)
	{
		this.focusedClassificatorId = focusedClassificatorId;
		classificationExplorerUserControl.SetParameters(support, Text);
		SetLookUpEdit();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void SetLookUpEdit()
	{
		List<ClassificationExplorerCustomFieldsItem> source = (dataSource = (from c in DB.Classificator.GetClassificators()
			select new ClassificationExplorerCustomFieldsItem(c.Id, c.Title, from x in c.Fields
				where x.Id > 0
				select x.Id) into x
			orderby x.DisplayName
			select x).ToList());
		classificationsLookUpEdit.Properties.DataSource = dataSource;
		LookUpEditSizeHelper.SetDropDownSize(classificationsLookUpEdit, source.Count());
	}

	private void ClassificationsLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (classificationsLookUpEdit.EditValue == null)
		{
			return;
		}
		progressPanelPanel.Visible = true;
		cancelSimpleButton.Enabled = false;
		classificationExplorerUserControl.Enabled = false;
		ClassificationExplorerCustomFieldsItem item = dataSource.FirstOrDefault((ClassificationExplorerCustomFieldsItem x) => x.DisplayName.Equals(classificationsLookUpEdit.EditValue.ToString()));
		if (!base.IsHandleCreated)
		{
			CreateControl();
		}
		isClosingEnabled = false;
		new TaskFactory().StartNew(delegate
		{
			classificationExplorerUserControl.SetDataSource(item);
		}).ContinueWith(delegate(Task x)
		{
			progressPanel.Invoke((Action)delegate
			{
				progressPanelPanel.Visible = false;
			});
			cancelSimpleButton.Invoke((Action)delegate
			{
				cancelSimpleButton.Enabled = true;
			});
			classificationExplorerUserControl.Invoke((Action)delegate
			{
				classificationExplorerUserControl.Enabled = true;
			});
			classificationExplorerUserControl.Invoke((Action)delegate
			{
				classificationExplorerUserControl.SetColumns(item);
			});
			classificationExplorerUserControl.GridPanel.Invoke((Action)delegate
			{
				classificationExplorerUserControl.GridPanel.Initialize(Text);
			});
			if (x.Exception != null)
			{
				throw x.Exception;
			}
			isClosingEnabled = true;
		}, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously);
	}

	private void ClassificationExplorerForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (!isClosingEnabled)
		{
			e.Cancel = true;
		}
	}

	private void CancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void ClassificationExplorerForm_Shown(object sender, EventArgs e)
	{
		await Task.Delay(100);
		classificationsLookUpEdit.EditValue = dataSource.FirstOrDefault((ClassificationExplorerCustomFieldsItem x) => x.ClassificatorId == focusedClassificatorId).DisplayName;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ClassificationExplorerForm));
		this.progressPanelPanel = new System.Windows.Forms.Panel();
		this.progressPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.progressLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.progressLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.progressLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.infoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.classificationsLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.classificationExplorerCustomFieldsItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.classificationExplorerUserControl = new Dataedo.App.UserControls.ClassificationExplorerUserControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.infoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressPanelPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressPanelControl).BeginInit();
		this.progressPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControl).BeginInit();
		this.progressLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.classificationsLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.classificationExplorerCustomFieldsItemBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.progressPanelPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.progressPanelPanel.BackColor = System.Drawing.Color.Transparent;
		this.progressPanelPanel.Controls.Add(this.progressPanelControl);
		this.progressPanelPanel.Location = new System.Drawing.Point(350, 282);
		this.progressPanelPanel.Name = "progressPanelPanel";
		this.progressPanelPanel.Size = new System.Drawing.Size(300, 81);
		this.progressPanelPanel.TabIndex = 4;
		this.progressPanelPanel.Visible = false;
		this.progressPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
		this.progressPanelControl.Controls.Add(this.progressLayoutControl);
		this.progressPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressPanelControl.Location = new System.Drawing.Point(0, 0);
		this.progressPanelControl.Name = "progressPanelControl";
		this.progressPanelControl.Size = new System.Drawing.Size(300, 81);
		this.progressPanelControl.TabIndex = 5;
		this.progressLayoutControl.AllowCustomization = false;
		this.progressLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.progressLayoutControl.Controls.Add(this.progressPanel);
		this.progressLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressLayoutControl.Location = new System.Drawing.Point(2, 2);
		this.progressLayoutControl.Name = "progressLayoutControl";
		this.progressLayoutControl.Root = this.progressLayoutControlGroup;
		this.progressLayoutControl.Size = new System.Drawing.Size(296, 77);
		this.progressLayoutControl.TabIndex = 6;
		this.progressLayoutControl.Text = "layoutControl1";
		this.progressPanel.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f);
		this.progressPanel.AppearanceCaption.Options.UseFont = true;
		this.progressPanel.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressPanel.AppearanceDescription.Options.UseFont = true;
		this.progressPanel.Description = "Getting columns...";
		this.progressPanel.Location = new System.Drawing.Point(12, 12);
		this.progressPanel.Name = "progressPanel";
		this.progressPanel.Size = new System.Drawing.Size(272, 53);
		this.progressPanel.StyleController = this.progressLayoutControl;
		this.progressPanel.TabIndex = 5;
		this.progressLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.progressLayoutControlGroup.GroupBordersVisible = false;
		this.progressLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.progressLayoutControlItem });
		this.progressLayoutControlGroup.Name = "progressLayoutControlGroup";
		this.progressLayoutControlGroup.Size = new System.Drawing.Size(296, 77);
		this.progressLayoutControlGroup.TextVisible = false;
		this.progressLayoutControlItem.Control = this.progressPanel;
		this.progressLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.progressLayoutControlItem.MinSize = new System.Drawing.Size(1, 1);
		this.progressLayoutControlItem.Name = "progressLayoutControlItem";
		this.progressLayoutControlItem.Size = new System.Drawing.Size(276, 57);
		this.progressLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.progressLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressLayoutControlItem.TextVisible = false;
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.infoUserControl);
		this.layoutControl.Controls.Add(this.classificationsLookUpEdit);
		this.layoutControl.Controls.Add(this.cancelSimpleButton);
		this.layoutControl.Controls.Add(this.classificationExplorerUserControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2429, 299, 250, 339);
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(1000, 644);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.infoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl.Description = " Upgrade to Enterprise Edition to unlock full Data Classification functionality";
		this.infoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.infoUserControl.Location = new System.Drawing.Point(12, 12);
		this.infoUserControl.MaximumSize = new System.Drawing.Size(0, 32);
		this.infoUserControl.MinimumSize = new System.Drawing.Size(564, 32);
		this.infoUserControl.Name = "infoUserControl";
		this.infoUserControl.Size = new System.Drawing.Size(976, 32);
		this.infoUserControl.TabIndex = 32;
		this.classificationsLookUpEdit.Location = new System.Drawing.Point(12, 48);
		this.classificationsLookUpEdit.Name = "classificationsLookUpEdit";
		this.classificationsLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.classificationsLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "Display Name", 5, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.classificationsLookUpEdit.Properties.DataSource = this.classificationExplorerCustomFieldsItemBindingSource;
		this.classificationsLookUpEdit.Properties.DisplayMember = "DisplayName";
		this.classificationsLookUpEdit.Properties.NullText = "";
		this.classificationsLookUpEdit.Properties.NullValuePrompt = "Choose data classificator";
		this.classificationsLookUpEdit.Properties.ShowFooter = false;
		this.classificationsLookUpEdit.Properties.ShowHeader = false;
		this.classificationsLookUpEdit.Properties.ValueMember = "DisplayName";
		this.classificationsLookUpEdit.Size = new System.Drawing.Size(301, 20);
		this.classificationsLookUpEdit.StyleController = this.layoutControl;
		this.classificationsLookUpEdit.TabIndex = 6;
		this.classificationsLookUpEdit.EditValueChanged += new System.EventHandler(ClassificationsLookUpEdit_EditValueChanged);
		this.classificationExplorerCustomFieldsItemBindingSource.DataSource = typeof(Dataedo.App.Tools.ClassificationExplorer.ClassificationExplorerCustomFieldsItem);
		this.cancelSimpleButton.Location = new System.Drawing.Point(903, 610);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl;
		this.cancelSimpleButton.TabIndex = 5;
		this.cancelSimpleButton.Text = "Close";
		this.cancelSimpleButton.Click += new System.EventHandler(CancelSimpleButton_Click);
		this.classificationExplorerUserControl.BackColor = System.Drawing.Color.Transparent;
		this.classificationExplorerUserControl.Location = new System.Drawing.Point(12, 72);
		this.classificationExplorerUserControl.Name = "classificationExplorerUserControl";
		this.classificationExplorerUserControl.Size = new System.Drawing.Size(976, 534);
		this.classificationExplorerUserControl.TabIndex = 4;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.layoutControlItem, this.layoutControlItem1, this.emptySpaceItem1, this.layoutControlItem3, this.emptySpaceItem3, this.infoUserControlLayoutControlItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(1000, 644);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem.Control = this.classificationExplorerUserControl;
		this.layoutControlItem.Location = new System.Drawing.Point(0, 60);
		this.layoutControlItem.Name = "layoutControlItem";
		this.layoutControlItem.Size = new System.Drawing.Size(980, 538);
		this.layoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem.TextVisible = false;
		this.layoutControlItem1.Control = this.cancelSimpleButton;
		this.layoutControlItem1.Location = new System.Drawing.Point(891, 598);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(89, 26);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(89, 26);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 598);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(891, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.Control = this.classificationsLookUpEdit;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 36);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(305, 24);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(305, 24);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(305, 24);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem3.Location = new System.Drawing.Point(305, 36);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(675, 24);
		this.emptySpaceItem3.Text = "emptySpaceItem1";
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.infoUserControlLayoutControlItem.Control = this.infoUserControl;
		this.infoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.infoUserControlLayoutControlItem.Name = "infoUserControlLayoutControlItem";
		this.infoUserControlLayoutControlItem.Size = new System.Drawing.Size(980, 36);
		this.infoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.infoUserControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1000, 644);
		base.Controls.Add(this.progressPanelPanel);
		base.Controls.Add(this.layoutControl);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ClassificationExplorerForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "ClassificationExplorerForm";
		this.Text = "Data Classification Explorer";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ClassificationExplorerForm_FormClosing);
		base.Shown += new System.EventHandler(ClassificationExplorerForm_Shown);
		this.progressPanelPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressPanelControl).EndInit();
		this.progressPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControl).EndInit();
		this.progressLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.classificationsLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.classificationExplorerCustomFieldsItemBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
