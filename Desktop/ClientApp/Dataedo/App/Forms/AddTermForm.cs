using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Properties;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Model.Data.BusinessGlossary;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;

namespace Dataedo.App.Forms;

public class AddTermForm : BaseXtraForm
{
	private DXErrorProvider errorProvider;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup Root;

	private LookUpEdit typeLookUpEdit;

	private LayoutControlItem typeLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private TextEdit termTitleTextEdit;

	private LayoutControlItem titleLayoutControlItem;

	private SimpleButton cancelsimpleButton;

	private SimpleButton saveSimpleButton;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem3;

	private LayoutControlItem layoutControlItem2;

	public bool CanSaveTermTitle { get; private set; }

	public bool CanSaveTermType { get; private set; }

	public AddTermForm()
	{
		InitializeComponent();
		errorProvider = new DXErrorProvider();
		List<TermTypeObject> termTypes = DB.BusinessGlossary.GetTermTypes();
		typeLookUpEdit.Properties.DataSource = termTypes;
		typeLookUpEdit.Properties.DropDownRows = ((termTypes.Count > 15) ? 15 : termTypes.Count);
		SetDefaultTypeLookUpEditValue(termTypes);
	}

	private void SetDefaultTypeLookUpEditValue(List<TermTypeObject> termTypes)
	{
		if (termTypes != null && termTypes.Count != 0)
		{
			typeLookUpEdit.EditValue = termTypes.FirstOrDefault();
		}
	}

	private void saveSimpleButton_Click(object sender, EventArgs e)
	{
		ValidateTermData(checkIfTermExists: true);
		if (CanSaveTermType && CanSaveTermTitle)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
		else if (CanSaveTermType)
		{
			GeneralMessageBoxesHandling.Show("Title of the term can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
		}
		else if (CanSaveTermTitle)
		{
			GeneralMessageBoxesHandling.Show("Type of the term can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
		}
		else
		{
			GeneralMessageBoxesHandling.Show("Type and title of the term can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
		}
	}

	private void cancelsimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void termTitleTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateTermData();
	}

	public TermTypeObject GetTermType()
	{
		return typeLookUpEdit.EditValue as TermTypeObject;
	}

	public string GetTermTitle()
	{
		return termTitleTextEdit.Text;
	}

	private void ValidateTermData(bool checkIfTermExists = false)
	{
		CanSaveTermType = true;
		CanSaveTermTitle = true;
		errorProvider.ClearErrors();
		SetError(typeLookUpEdit.EditValue == null, "Type can not be empty", typeLookUpEdit);
		SetError(!ValidateFields.IsFieldNotEmpty(termTitleTextEdit.Text), "Title can not be empty", termTitleTextEdit);
	}

	private void SetError(bool condition, string message, Control control)
	{
		if (condition)
		{
			errorProvider.SetError(control, message, ErrorType.Critical);
			if (control == termTitleTextEdit)
			{
				CanSaveTermTitle = false;
			}
			else if (control == typeLookUpEdit)
			{
				CanSaveTermType = false;
			}
			base.DialogResult = DialogResult.None;
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
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.cancelsimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.termTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.typeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.typeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.titleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.termTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.cancelsimpleButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.saveSimpleButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.termTitleTextEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.typeLookUpEdit);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(419, 145);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.cancelsimpleButton.Location = new System.Drawing.Point(323, 111);
		this.cancelsimpleButton.Name = "cancelsimpleButton";
		this.cancelsimpleButton.Size = new System.Drawing.Size(84, 22);
		this.cancelsimpleButton.StyleController = this.nonCustomizableLayoutControl1;
		this.cancelsimpleButton.TabIndex = 7;
		this.cancelsimpleButton.Text = "Cancel";
		this.cancelsimpleButton.Click += new System.EventHandler(cancelsimpleButton_Click);
		this.saveSimpleButton.Location = new System.Drawing.Point(225, 111);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(84, 22);
		this.saveSimpleButton.StyleController = this.nonCustomizableLayoutControl1;
		this.saveSimpleButton.TabIndex = 6;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(saveSimpleButton_Click);
		this.termTitleTextEdit.Location = new System.Drawing.Point(39, 36);
		this.termTitleTextEdit.Name = "termTitleTextEdit";
		this.termTitleTextEdit.Size = new System.Drawing.Size(368, 20);
		this.termTitleTextEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.termTitleTextEdit.TabIndex = 5;
		this.termTitleTextEdit.EditValueChanged += new System.EventHandler(termTitleTextEdit_EditValueChanged);
		this.typeLookUpEdit.Location = new System.Drawing.Point(39, 12);
		this.typeLookUpEdit.Name = "typeLookUpEdit";
		this.typeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Title", "Title")
		});
		this.typeLookUpEdit.Properties.DisplayMember = "Title";
		this.typeLookUpEdit.Properties.DropDownRows = 4;
		this.typeLookUpEdit.Properties.NullText = "";
		this.typeLookUpEdit.Properties.ShowFooter = false;
		this.typeLookUpEdit.Properties.ShowHeader = false;
		this.typeLookUpEdit.Properties.ShowLines = false;
		this.typeLookUpEdit.Size = new System.Drawing.Size(368, 20);
		this.typeLookUpEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.typeLookUpEdit.TabIndex = 4;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.typeLayoutControlItem, this.emptySpaceItem1, this.titleLayoutControlItem, this.emptySpaceItem2, this.layoutControlItem1, this.emptySpaceItem3, this.layoutControlItem2 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(419, 145);
		this.Root.TextVisible = false;
		this.typeLayoutControlItem.Control = this.typeLookUpEdit;
		this.typeLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.typeLayoutControlItem.Name = "typeLayoutControlItem";
		this.typeLayoutControlItem.Size = new System.Drawing.Size(399, 24);
		this.typeLayoutControlItem.Text = "Type";
		this.typeLayoutControlItem.TextSize = new System.Drawing.Size(24, 13);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 99);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(213, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.titleLayoutControlItem.Control = this.termTitleTextEdit;
		this.titleLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.titleLayoutControlItem.Name = "titleLayoutControlItem";
		this.titleLayoutControlItem.Size = new System.Drawing.Size(399, 24);
		this.titleLayoutControlItem.Text = "Title";
		this.titleLayoutControlItem.TextSize = new System.Drawing.Size(24, 13);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 48);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(399, 51);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.Control = this.saveSimpleButton;
		this.layoutControlItem1.Location = new System.Drawing.Point(213, 99);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(88, 26);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(88, 26);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(88, 26);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(301, 99);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.cancelsimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(311, 99);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(88, 26);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(88, 26);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(88, 26);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(419, 145);
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.Name = "AddTermForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Term";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.termTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		base.ResumeLayout(false);
	}
}
