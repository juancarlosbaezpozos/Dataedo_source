using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.UserControls;

public class RepositoriesUserControl : BaseUserControl
{
	private DXErrorProvider errorProvider;

	private IContainer components;

	private LookUpEdit repositoriesLookUpEdit;

	private LayoutControlGroup layoutControlGroup;

	private LayoutControlItem layoutControlItem2;

	private RadioGroup radioGroup;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem3;

	private NonCustomizableLayoutControl layoutControl;

	private SplashScreenManager splashScreenManager1;

	public RepositoriesUserControl()
	{
		InitializeComponent();
		errorProvider = new DXErrorProvider();
	}

	public void SetParameters()
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: true);
		List<string> list = RepositoriesDBHelper.GetRepositories().ToList();
		list.Remove(StaticData.Database);
		repositoriesLookUpEdit.Properties.DataSource = list;
		SetDropDownSize(list.Count() - 1);
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: false);
	}

	public string GetSelectedRepositoryTitle()
	{
		if (IsCopyingInCurrentRepository())
		{
			return StaticData.Database;
		}
		return repositoriesLookUpEdit.GetSelectedDataRow() as string;
	}

	public bool IsReady()
	{
		bool result = true;
		if (radioGroup.SelectedIndex == radioGroup.Properties.Items.IndexOf(radioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.Equals("another"))) && repositoriesLookUpEdit.EditValue == null)
		{
			result = false;
			errorProvider.SetError(repositoriesLookUpEdit, "Destination repository can not be empty");
		}
		return result;
	}

	private bool IsCopyingInCurrentRepository()
	{
		return radioGroup.SelectedIndex == radioGroup.Properties.Items.IndexOf(radioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.Equals("current")));
	}

	private void SetDropDownSize(int repositoriesCount)
	{
		repositoriesLookUpEdit.Properties.PopupFormMinSize = new Size(repositoriesLookUpEdit.Width - 80, (repositoriesLookUpEdit.Height - 4) * repositoriesCount);
		repositoriesLookUpEdit.Properties.PopupFormSize = new Size(repositoriesLookUpEdit.Width - 80, (repositoriesLookUpEdit.Height - 4) * repositoriesCount);
	}

	private void radioGroup_SelectedIndexChanged(object sender, EventArgs e)
	{
		repositoriesLookUpEdit.Enabled = radioGroup.SelectedIndex == radioGroup.Properties.Items.IndexOf(radioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.Equals("another")));
		if (radioGroup.SelectedIndex != 0)
		{
			SetParameters();
		}
		errorProvider.ClearErrors();
	}

	private void repositoriesLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		errorProvider.ClearErrors();
	}

	private void RepositoriesUserControl_Load(object sender, EventArgs e)
	{
		radioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.Equals("another")).Enabled = AllowAnotherRepositories();
	}

	private bool AllowAnotherRepositories()
	{
		if (StaticData.IsProjectFile)
		{
			return false;
		}
		string commandText = "SELECT @@version;";
		bool result = true;
		try
		{
			if (string.IsNullOrWhiteSpace(StaticData.DataedoConnectionString))
			{
				return false;
			}
			using SqlConnection sqlConnection = new SqlConnection(StaticData.DataedoConnectionString);
			sqlConnection.Open();
			SqlDataReader sqlDataReader = CommandsWithTimeout.SqlServerForRepository(commandText, sqlConnection).ExecuteReader(CommandBehavior.CloseConnection);
			if (sqlDataReader.Read())
			{
				if ((sqlDataReader.GetValue(0)?.ToString()?.ToLower()).Contains("azure"))
				{
					return false;
				}
				return result;
			}
			return result;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
			return false;
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
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.radioGroup = new DevExpress.XtraEditors.RadioGroup();
		this.repositoriesLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.radioGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoriesLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.radioGroup);
		this.layoutControl.Controls.Add(this.repositoriesLookUpEdit);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(513, 99);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.radioGroup.EditValue = "current";
		this.radioGroup.Location = new System.Drawing.Point(12, 12);
		this.radioGroup.Name = "radioGroup";
		this.radioGroup.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroup.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("current", "Current repository", true, "current"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("another", "Another repository on the same server:", true, "another")
		});
		this.radioGroup.Size = new System.Drawing.Size(224, 55);
		this.radioGroup.StyleController = this.layoutControl;
		this.radioGroup.TabIndex = 6;
		this.radioGroup.SelectedIndexChanged += new System.EventHandler(radioGroup_SelectedIndexChanged);
		this.repositoriesLookUpEdit.Enabled = false;
		this.repositoriesLookUpEdit.Location = new System.Drawing.Point(33, 71);
		this.repositoriesLookUpEdit.Name = "repositoriesLookUpEdit";
		this.repositoriesLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoriesLookUpEdit.Properties.NullText = "";
		this.repositoriesLookUpEdit.Properties.ShowFooter = false;
		this.repositoriesLookUpEdit.Properties.ShowHeader = false;
		this.repositoriesLookUpEdit.Properties.ShowLines = false;
		this.repositoriesLookUpEdit.Size = new System.Drawing.Size(203, 20);
		this.repositoriesLookUpEdit.StyleController = this.layoutControl;
		this.repositoriesLookUpEdit.TabIndex = 5;
		this.repositoriesLookUpEdit.EditValueChanged += new System.EventHandler(repositoriesLookUpEdit_EditValueChanged);
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.layoutControlItem3, this.emptySpaceItem1, this.emptySpaceItem2, this.layoutControlItem2, this.emptySpaceItem3 });
		this.layoutControlGroup.Name = "layoutControlGroup";
		this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 5);
		this.layoutControlGroup.Size = new System.Drawing.Size(513, 99);
		this.layoutControlGroup.TextVisible = false;
		this.layoutControlItem3.Control = this.radioGroup;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(54, 42);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(228, 59);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(228, 0);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(265, 59);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(228, 59);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(265, 25);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(265, 25);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(265, 25);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.repositoriesLookUpEdit;
		this.layoutControlItem2.Location = new System.Drawing.Point(21, 59);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(54, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(207, 25);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 59);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(21, 25);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(21, 25);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(21, 25);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Name = "RepositoriesUserControl";
		base.Size = new System.Drawing.Size(513, 99);
		base.Load += new System.EventHandler(RepositoriesUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.radioGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoriesLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		base.ResumeLayout(false);
	}
}
