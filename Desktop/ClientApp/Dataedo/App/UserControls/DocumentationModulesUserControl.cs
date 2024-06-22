using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class DocumentationModulesUserControl : BaseUserControl
{
	private IdEventArgs idEventArgs;

	public EventHandler AddModule;

	private IContainer components;

	private PopupMenu popupMenu1;

	private NonCustomizableLayoutControl layoutControl2;

	private NonCustomizableLayoutControl layoutControl3;

	private ChooseModulesUserControl chooseModulesUserControl;

	private LayoutControlGroup Root;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlGroup layoutControlGroup2;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private PopupContainerEdit popupContainerEdit;

	private PopupContainerControl popupContainerControl1;

	private NonCustomizableLayoutControl layoutControl1;

	private SimpleButton addModuleSimpleButton;

	private LayoutControlItem layoutControlItem4;

	private EmptySpaceItem emptySpaceItem1;

	private SimpleButton okSimpleButton;

	private LayoutControlItem layoutControlItem5;

	public bool IsChanged { get; set; }

	public List<CommonFunctionsDatabase.ModuleItemModel> Modules { get; set; }

	private string PopupContainerEditText { get; set; }

	public (SharedObjectTypeEnum.ObjectType ObjectType, int ObjectId) LoadedFor { get; private set; }

	[Browsable(true)]
	public event EventHandler EditValueChanged;

	public DocumentationModulesUserControl()
	{
		InitializeComponent();
	}

	public void LoadData(IdEventArgs eventArgs)
	{
		idEventArgs = eventArgs;
		chooseModulesUserControl.TurnOffHyperlinksVisibility();
		Modules = new List<CommonFunctionsDatabase.ModuleItemModel>();
	}

	public void ClearData()
	{
		chooseModulesUserControl.ClearModules();
		Modules = new List<CommonFunctionsDatabase.ModuleItemModel>();
		LoadedFor = ((SharedObjectTypeEnum.ObjectType)0, 0);
	}

	public void SetPopupBarText(string text)
	{
		PopupContainerEditText = text;
		popupContainerEdit.Refresh();
	}

	public List<int> GetCurrentRowModulesId(SharedObjectTypeEnum.ObjectType objectType, int objectId)
	{
		List<int> list = new List<int>();
		chooseModulesUserControl.GetModules(idEventArgs.DatabaseId ?? (-1), null, useAllDocumentations: true, useOtherModule: false, includeBusinessGlossary: true, list);
		if (idEventArgs.DatabaseId.HasValue)
		{
			string text = string.Empty;
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				text = string.Join(",", DB.Procedure.GetProcedureModules(objectId));
				break;
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				text = string.Join(",", DB.Table.GetTableModules(objectId));
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(',');
				foreach (string s in array)
				{
					list.Add(int.Parse(s));
				}
				chooseModulesUserControl.GetModules(idEventArgs.DatabaseId ?? (-1), null, useAllDocumentations: true, useOtherModule: false, includeBusinessGlossary: true, list);
			}
			popupContainerControl1.Height = chooseModulesUserControl.GetVisibleRowsHeight() + addModuleSimpleButton.Height + popupContainerEdit.Height;
			popupContainerEdit.Refresh();
		}
		LoadedFor = (objectType, objectId);
		return list;
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		popupContainerEdit.ClosePopup();
		chooseModulesUserControl.Refresh();
	}

	private void addModuleSimpleButton_Click(object sender, EventArgs e)
	{
		popupContainerEdit.ClosePopup();
		AddModule?.Invoke(null, idEventArgs);
	}

	public DocumentationModules[] GetSelectedModules()
	{
		return chooseModulesUserControl.GetSelectedModules();
	}

	private void popupContainerEdit_QueryDisplayText(object sender, QueryDisplayTextEventArgs e)
	{
		DocumentationModulesContainer documentationsModulesData = chooseModulesUserControl.DocumentationsModulesData;
		if (documentationsModulesData != null && documentationsModulesData.Data?.Count == 0)
		{
			e.DisplayText = PopupContainerEditText;
			return;
		}
		List<string> list = new List<string>();
		DocumentationModules[] selectedModules = GetSelectedModules();
		if (selectedModules.Length != 0)
		{
			DocumentationModules[] array = selectedModules;
			foreach (DocumentationModules documentationModules in array)
			{
				foreach (ModuleRow module in documentationModules.Modules)
				{
					list.Add(module.Name + " (" + documentationModules.Documentation.Title + ")");
				}
			}
		}
		e.DisplayText = ((list.Count > 0) ? string.Join(", ", list) : string.Empty);
	}

	private void chooseModulesUserControl_ModulesCellValueChanged(object sender, EventArgs e)
	{
		IsChanged = true;
	}

	private void popupContainerEdit_Closed(object sender, ClosedEventArgs e)
	{
		this.EditValueChanged?.Invoke(this, e);
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
		this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
		this.layoutControl2 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.layoutControl3 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.addModuleSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.chooseModulesUserControl = new Dataedo.App.UserControls.ChooseModulesUserControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.popupContainerEdit = new DevExpress.XtraEditors.PopupContainerEdit();
		this.popupContainerControl1 = new DevExpress.XtraEditors.PopupContainerControl();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		((System.ComponentModel.ISupportInitialize)this.popupMenu1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		this.layoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).BeginInit();
		this.layoutControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupContainerEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupContainerControl1).BeginInit();
		this.popupContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		base.SuspendLayout();
		this.popupMenu1.Name = "popupMenu1";
		this.layoutControl2.AllowCustomization = false;
		this.layoutControl2.Controls.Add(this.layoutControl3);
		this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl2.Location = new System.Drawing.Point(0, 0);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.Root = this.layoutControlGroup2;
		this.layoutControl2.Size = new System.Drawing.Size(297, 252);
		this.layoutControl2.TabIndex = 1;
		this.layoutControl3.AllowCustomization = false;
		this.layoutControl3.Controls.Add(this.okSimpleButton);
		this.layoutControl3.Controls.Add(this.addModuleSimpleButton);
		this.layoutControl3.Controls.Add(this.chooseModulesUserControl);
		this.layoutControl3.Location = new System.Drawing.Point(2, 2);
		this.layoutControl3.Margin = new System.Windows.Forms.Padding(0);
		this.layoutControl3.Name = "layoutControl3";
		this.layoutControl3.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1181, 631, 650, 400);
		this.layoutControl3.Root = this.Root;
		this.layoutControl3.Size = new System.Drawing.Size(293, 248);
		this.layoutControl3.TabIndex = 4;
		this.layoutControl3.Text = "layoutControl3";
		this.okSimpleButton.Location = new System.Drawing.Point(213, 224);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(78, 22);
		this.okSimpleButton.StyleController = this.layoutControl3;
		this.okSimpleButton.TabIndex = 6;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.addModuleSimpleButton.Location = new System.Drawing.Point(2, 224);
		this.addModuleSimpleButton.Name = "addModuleSimpleButton";
		this.addModuleSimpleButton.Size = new System.Drawing.Size(96, 22);
		this.addModuleSimpleButton.StyleController = this.layoutControl3;
		this.addModuleSimpleButton.TabIndex = 5;
		this.addModuleSimpleButton.Text = "Add Subject Area";
		this.addModuleSimpleButton.Click += new System.EventHandler(addModuleSimpleButton_Click);
		this.chooseModulesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.chooseModulesUserControl.Location = new System.Drawing.Point(2, 2);
		this.chooseModulesUserControl.Name = "chooseModulesUserControl";
		this.chooseModulesUserControl.Size = new System.Drawing.Size(289, 218);
		this.chooseModulesUserControl.TabIndex = 4;
		this.chooseModulesUserControl.ModulesCellValueChanged += new System.EventHandler(chooseModulesUserControl_ModulesCellValueChanged);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem3, this.layoutControlItem4, this.emptySpaceItem1, this.layoutControlItem5 });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(293, 248);
		this.Root.TextVisible = false;
		this.layoutControlItem3.Control = this.chooseModulesUserControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(293, 222);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem4.Control = this.addModuleSimpleButton;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 222);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(100, 26);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(100, 26);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(100, 26);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(100, 222);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(111, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.Control = this.okSimpleButton;
		this.layoutControlItem5.Location = new System.Drawing.Point(211, 222);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(82, 26);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(82, 26);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(82, 26);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem2 });
		this.layoutControlGroup2.Name = "layoutControlGroup2";
		this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup2.Size = new System.Drawing.Size(297, 252);
		this.layoutControlGroup2.TextVisible = false;
		this.layoutControlItem2.Control = this.layoutControl3;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(297, 252);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(365, 323);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.popupContainerEdit;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem1.Size = new System.Drawing.Size(365, 323);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.popupContainerEdit.Location = new System.Drawing.Point(0, 0);
		this.popupContainerEdit.Margin = new System.Windows.Forms.Padding(0);
		this.popupContainerEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.popupContainerEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.popupContainerEdit.Name = "popupContainerEdit";
		this.popupContainerEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.popupContainerEdit.Properties.PopupControl = this.popupContainerControl1;
		this.popupContainerEdit.Properties.PopupSizeable = false;
		this.popupContainerEdit.Properties.ShowPopupCloseButton = false;
		this.popupContainerEdit.Size = new System.Drawing.Size(365, 22);
		this.popupContainerEdit.StyleController = this.layoutControl1;
		this.popupContainerEdit.TabIndex = 4;
		this.popupContainerEdit.QueryDisplayText += new DevExpress.XtraEditors.Controls.QueryDisplayTextEventHandler(popupContainerEdit_QueryDisplayText);
		this.popupContainerEdit.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(popupContainerEdit_Closed);
		this.popupContainerControl1.Controls.Add(this.layoutControl2);
		this.popupContainerControl1.Location = new System.Drawing.Point(12, 34);
		this.popupContainerControl1.Name = "popupContainerControl1";
		this.popupContainerControl1.Size = new System.Drawing.Size(297, 252);
		this.popupContainerControl1.TabIndex = 5;
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.Controls.Add(this.popupContainerControl1);
		this.layoutControl1.Controls.Add(this.popupContainerEdit);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(365, 323);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl1);
		base.Name = "DocumentationModulesUserControl";
		base.Size = new System.Drawing.Size(365, 323);
		((System.ComponentModel.ISupportInitialize)this.popupMenu1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		this.layoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).EndInit();
		this.layoutControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupContainerEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupContainerControl1).EndInit();
		this.popupContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
