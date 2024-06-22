using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls;

public class HiererchyAndListsUserControl : BaseWizardPageControl
{
	private IContainer components;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private PanelControl dataPanelControl;

	private LayoutControlGroup Root;

	private EmptySpaceItem separator1EmptySpaceItem;

	private LayoutControlItem dataPanelControlLayoutControlItem;

	private ServerHierachyUserControl hierachyUserControl;

	private SplitterControl splitter1SplitterControl;

	private SplitterControl splitter2SplitterControl;

	private TablePanel objectsToBeImportedTablePanel;

	private SelectingPanelUserControl selectingPanelUserControl;

	private ObjectsToBeImportedUserControl objectsToBeImportedUserControl;

	private TablePanel availableObjectsTablePanel;

	private AvailableObjectsUserControl availableObjectsUserControl;

	public HiererchyAndListsUserControl()
	{
		InitializeComponent();
		Initialize();
	}

	private void Initialize()
	{
	}

	private void hierachyUserControl_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
	{
		Cursor current = Cursor.Current;
		Cursor.Current = Cursors.WaitCursor;
		availableObjectsUserControl.SetSource(e.Item.FullName);
		Cursor.Current = current;
	}

	private void selectingPanelUserControl_MoveAllRightClick(object sender, EventArgs e)
	{
	}

	private void selectingPanelUserControl_MoveSingleRightClick(object sender, EventArgs e)
	{
	}

	private void selectingPanelUserControl_MoveSingleLeftClick(object sender, EventArgs e)
	{
	}

	private void selectingPanelUserControl_MoveAllLeftClick(object sender, EventArgs e)
	{
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
		this.mainNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.dataPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.availableObjectsTablePanel = new DevExpress.Utils.Layout.TablePanel();
		this.availableObjectsUserControl = new Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.AvailableObjectsUserControl();
		this.splitter2SplitterControl = new DevExpress.XtraEditors.SplitterControl();
		this.objectsToBeImportedTablePanel = new DevExpress.Utils.Layout.TablePanel();
		this.selectingPanelUserControl = new Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.SelectingPanelUserControl();
		this.objectsToBeImportedUserControl = new Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.ObjectsToBeImportedUserControl();
		this.splitter1SplitterControl = new DevExpress.XtraEditors.SplitterControl();
		this.hierachyUserControl = new Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.ServerHierachyUserControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.dataPanelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separator1EmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataPanelControl).BeginInit();
		this.dataPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.availableObjectsTablePanel).BeginInit();
		this.availableObjectsTablePanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.objectsToBeImportedTablePanel).BeginInit();
		this.objectsToBeImportedTablePanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataPanelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separator1EmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.dataPanelControl);
		this.mainNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainNonCustomizableLayoutControl.Name = "mainNonCustomizableLayoutControl";
		this.mainNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2703, 538, 650, 594);
		this.mainNonCustomizableLayoutControl.Root = this.Root;
		this.mainNonCustomizableLayoutControl.Size = new System.Drawing.Size(1018, 614);
		this.mainNonCustomizableLayoutControl.TabIndex = 15;
		this.mainNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.dataPanelControl.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.dataPanelControl.Appearance.Options.UseBackColor = true;
		this.dataPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.dataPanelControl.Controls.Add(this.availableObjectsTablePanel);
		this.dataPanelControl.Controls.Add(this.splitter2SplitterControl);
		this.dataPanelControl.Controls.Add(this.objectsToBeImportedTablePanel);
		this.dataPanelControl.Controls.Add(this.splitter1SplitterControl);
		this.dataPanelControl.Controls.Add(this.hierachyUserControl);
		this.dataPanelControl.Location = new System.Drawing.Point(12, 22);
		this.dataPanelControl.Margin = new System.Windows.Forms.Padding(0);
		this.dataPanelControl.Name = "dataPanelControl";
		this.dataPanelControl.Size = new System.Drawing.Size(994, 580);
		this.dataPanelControl.TabIndex = 16;
		this.availableObjectsTablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 5f));
		this.availableObjectsTablePanel.Controls.Add(this.availableObjectsUserControl);
		this.availableObjectsTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.availableObjectsTablePanel.Location = new System.Drawing.Point(250, 0);
		this.availableObjectsTablePanel.Name = "availableObjectsTablePanel";
		this.availableObjectsTablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 100f));
		this.availableObjectsTablePanel.Size = new System.Drawing.Size(454, 580);
		this.availableObjectsTablePanel.TabIndex = 37;
		this.availableObjectsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.availableObjectsTablePanel.SetColumn(this.availableObjectsUserControl, 0);
		this.availableObjectsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.availableObjectsUserControl.Location = new System.Drawing.Point(0, 0);
		this.availableObjectsUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.availableObjectsUserControl.Name = "availableObjectsUserControl";
		this.availableObjectsTablePanel.SetRow(this.availableObjectsUserControl, 0);
		this.availableObjectsUserControl.Size = new System.Drawing.Size(454, 580);
		this.availableObjectsUserControl.TabIndex = 33;
		this.splitter2SplitterControl.Dock = System.Windows.Forms.DockStyle.Right;
		this.splitter2SplitterControl.Location = new System.Drawing.Point(704, 0);
		this.splitter2SplitterControl.Name = "splitter2SplitterControl";
		this.splitter2SplitterControl.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
		this.splitter2SplitterControl.Size = new System.Drawing.Size(10, 580);
		this.splitter2SplitterControl.TabIndex = 36;
		this.splitter2SplitterControl.TabStop = false;
		this.objectsToBeImportedTablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 40f), new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 100f));
		this.objectsToBeImportedTablePanel.Controls.Add(this.selectingPanelUserControl);
		this.objectsToBeImportedTablePanel.Controls.Add(this.objectsToBeImportedUserControl);
		this.objectsToBeImportedTablePanel.Dock = System.Windows.Forms.DockStyle.Right;
		this.objectsToBeImportedTablePanel.Location = new System.Drawing.Point(714, 0);
		this.objectsToBeImportedTablePanel.Margin = new System.Windows.Forms.Padding(0);
		this.objectsToBeImportedTablePanel.Name = "objectsToBeImportedTablePanel";
		this.objectsToBeImportedTablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26f));
		this.objectsToBeImportedTablePanel.Size = new System.Drawing.Size(280, 580);
		this.objectsToBeImportedTablePanel.TabIndex = 34;
		this.selectingPanelUserControl.BackColor = System.Drawing.Color.Transparent;
		this.objectsToBeImportedTablePanel.SetColumn(this.selectingPanelUserControl, 0);
		this.selectingPanelUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.selectingPanelUserControl.Location = new System.Drawing.Point(3, 3);
		this.selectingPanelUserControl.Name = "selectingPanelUserControl";
		this.objectsToBeImportedTablePanel.SetRow(this.selectingPanelUserControl, 0);
		this.selectingPanelUserControl.Size = new System.Drawing.Size(34, 574);
		this.selectingPanelUserControl.TabIndex = 35;
		this.selectingPanelUserControl.MoveAllRightClick += new System.EventHandler(selectingPanelUserControl_MoveAllRightClick);
		this.selectingPanelUserControl.MoveSingleRightClick += new System.EventHandler(selectingPanelUserControl_MoveSingleRightClick);
		this.selectingPanelUserControl.MoveAllLeftClick += new System.EventHandler(selectingPanelUserControl_MoveAllLeftClick);
		this.selectingPanelUserControl.MoveSingleLeftClick += new System.EventHandler(selectingPanelUserControl_MoveSingleLeftClick);
		this.objectsToBeImportedUserControl.BackColor = System.Drawing.Color.Transparent;
		this.objectsToBeImportedTablePanel.SetColumn(this.objectsToBeImportedUserControl, 1);
		this.objectsToBeImportedUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.objectsToBeImportedUserControl.Location = new System.Drawing.Point(40, 0);
		this.objectsToBeImportedUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.objectsToBeImportedUserControl.Name = "objectsToBeImportedUserControl";
		this.objectsToBeImportedTablePanel.SetRow(this.objectsToBeImportedUserControl, 0);
		this.objectsToBeImportedUserControl.Size = new System.Drawing.Size(240, 580);
		this.objectsToBeImportedUserControl.TabIndex = 33;
		this.splitter1SplitterControl.Location = new System.Drawing.Point(240, 0);
		this.splitter1SplitterControl.Name = "splitter1SplitterControl";
		this.splitter1SplitterControl.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
		this.splitter1SplitterControl.Size = new System.Drawing.Size(10, 580);
		this.splitter1SplitterControl.TabIndex = 27;
		this.splitter1SplitterControl.TabStop = false;
		this.hierachyUserControl.BackColor = System.Drawing.Color.Transparent;
		this.hierachyUserControl.Dock = System.Windows.Forms.DockStyle.Left;
		this.hierachyUserControl.Location = new System.Drawing.Point(0, 0);
		this.hierachyUserControl.Name = "hierachyUserControl";
		this.hierachyUserControl.Size = new System.Drawing.Size(240, 580);
		this.hierachyUserControl.TabIndex = 19;
		this.hierachyUserControl.SelectedItemChanged += new System.EventHandler<Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs.SelectedItemChangedEventArgs>(hierachyUserControl_SelectedItemChanged);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.dataPanelControlLayoutControlItem, this.separator1EmptySpaceItem });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(1018, 614);
		this.Root.TextVisible = false;
		this.dataPanelControlLayoutControlItem.Control = this.dataPanelControl;
		this.dataPanelControlLayoutControlItem.Location = new System.Drawing.Point(0, 10);
		this.dataPanelControlLayoutControlItem.Name = "dataPanelControlLayoutControlItem";
		this.dataPanelControlLayoutControlItem.Size = new System.Drawing.Size(998, 584);
		this.dataPanelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.dataPanelControlLayoutControlItem.TextVisible = false;
		this.separator1EmptySpaceItem.AllowHotTrack = false;
		this.separator1EmptySpaceItem.Location = new System.Drawing.Point(0, 0);
		this.separator1EmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.separator1EmptySpaceItem.MinSize = new System.Drawing.Size(1, 10);
		this.separator1EmptySpaceItem.Name = "separator1EmptySpaceItem";
		this.separator1EmptySpaceItem.Size = new System.Drawing.Size(998, 10);
		this.separator1EmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separator1EmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		base.Controls.Add(this.mainNonCustomizableLayoutControl);
		base.Name = "HiererchyAndListsUserControl";
		base.Size = new System.Drawing.Size(1018, 614);
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataPanelControl).EndInit();
		this.dataPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.availableObjectsTablePanel).EndInit();
		this.availableObjectsTablePanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.objectsToBeImportedTablePanel).EndInit();
		this.objectsToBeImportedTablePanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataPanelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separator1EmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
