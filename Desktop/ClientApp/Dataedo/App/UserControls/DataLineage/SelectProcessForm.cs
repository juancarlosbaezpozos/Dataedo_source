using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.CustomControls;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.DataLineage;

public class SelectProcessForm : BaseXtraForm
{
	private SharedObjectTypeEnum.ObjectType upperObjectType;

	private SharedObjectTypeEnum.ObjectType lowerObjectType;

	private ArrowDirection ArrowDirection = ArrowDirection.Down;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private SimpleButton okButton;

	private SimpleButton cancelButton;

	private LayoutControlGroup Root;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem2;

	private LabelControl upperLabelControl;

	private LayoutControlItem upperLayoutControlItem;

	private LabelControl lowerLabelControl;

	private LayoutControlItem lowerLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private LabelControl arrowImageLabelControl;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem6;

	private HyperlinkLabelControl lowerSwitchProcessorHyperlinkLabelControl;

	private HyperlinkLabelControl upperSwitchProcessorHyperlinkLabelControl;

	private LayoutControlItem upperSwitchLayoutControlItem;

	private LayoutControlItem lowerSwitchLayoutControlItem;

	private LabelControl flowDirectionLabelControl;

	private HyperlinkLabelControl inverFlowDirectionHyperlinkLabelControl;

	private LayoutControlItem invertFlowDirectionLayoutControlItem;

	private LayoutControlItem flowDirectionLayoutControlItem;

	private EmptySpaceItem emptySpaceItem7;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem emptySpaceItem9;

	private ToolTipController toolTipController;

	private GridLookUpEdit upperProcessesLookUpEdit;

	private GridView gridLookUpEdit1View;

	private GridColumn nameColumn;

	private LayoutControlItem upperProcessLayoutControlItem;

	private GridLookUpEdit lowerProcessesLookUpEdit;

	private GridView gridView1;

	private LayoutControlItem lowerProcessLayoutControlItem;

	private GridColumn nameColumnLowerLookup;

	public DataProcessRow SelectedProcessRow
	{
		get
		{
			if (IsUpperProcessor)
			{
				return upperProcessesLookUpEdit.EditValue as DataProcessRow;
			}
			return lowerProcessesLookUpEdit.EditValue as DataProcessRow;
		}
	}

	public bool IsInFlow
	{
		get
		{
			if (ArrowDirection != ArrowDirection.Down || !IsLowerProcessor)
			{
				if (ArrowDirection == ArrowDirection.Up)
				{
					return IsUpperProcessor;
				}
				return false;
			}
			return true;
		}
	}

	private bool IsOutFlow
	{
		get
		{
			if (ArrowDirection != ArrowDirection.Up || !IsLowerProcessor)
			{
				if (ArrowDirection == ArrowDirection.Down)
				{
					return IsUpperProcessor;
				}
				return false;
			}
			return true;
		}
	}

	private bool IsViewAsProcessor
	{
		get
		{
			if (upperObjectType != SharedObjectTypeEnum.ObjectType.View || !IsUpperProcessor)
			{
				if (lowerObjectType == SharedObjectTypeEnum.ObjectType.View)
				{
					return IsLowerProcessor;
				}
				return false;
			}
			return true;
		}
	}

	private bool IsUpperProcessor => upperProcessLayoutControlItem.Visibility == LayoutVisibility.Always;

	private bool IsLowerProcessor => lowerProcessLayoutControlItem.Visibility == LayoutVisibility.Always;

	public SelectProcessForm()
	{
		InitializeComponent();
		upperProcessesLookUpEdit.Properties.DisplayMember = "Name";
		lowerProcessesLookUpEdit.Properties.DisplayMember = "Name";
	}

	public void SetParameters(string upperObjectCaption, List<DataProcessRow> upperObjectProcesses, IFlowDraggable upperObject, string lowerObjectCaption, List<DataProcessRow> lowerObjectProcesses, IFlowDraggable lowerObject, bool isInFlow)
	{
		nonCustomizableLayoutControl1.BeginInit();
		upperLabelControl.Text = upperObjectCaption;
		upperObjectType = upperObject.ObjectType;
		upperLabelControl.ImageOptions.Image = IconsSupport.GetObjectIcon(upperObject.ObjectType, upperObject.Subtype, upperObject.Source);
		upperProcessesLookUpEdit.Properties.DataSource = upperObjectProcesses;
		if (upperObjectProcesses.Any())
		{
			upperProcessesLookUpEdit.EditValue = upperObjectProcesses[0];
			SetLookupHeight(upperProcessesLookUpEdit);
		}
		lowerLabelControl.Text = lowerObjectCaption;
		lowerObjectType = lowerObject.ObjectType;
		lowerLabelControl.ImageOptions.Image = IconsSupport.GetObjectIcon(lowerObject.ObjectType, lowerObject.Subtype, lowerObject.Source);
		lowerProcessesLookUpEdit.Properties.DataSource = lowerObjectProcesses;
		if (lowerObjectProcesses.Any())
		{
			lowerProcessesLookUpEdit.EditValue = lowerObjectProcesses[0];
			SetLookupHeight(lowerProcessesLookUpEdit);
		}
		nonCustomizableLayoutControl1.EndInit();
		if (isInFlow)
		{
			ChangeArrowDirection();
		}
		SetupForm();
	}

	private void SetupForm()
	{
		nonCustomizableLayoutControl1.BeginInit();
		if (upperObjectType == SharedObjectTypeEnum.ObjectType.Table || upperObjectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			lowerSwitchLayoutControlItem.Visibility = LayoutVisibility.Never;
			SetUpperProcessVisibility(visible: false);
		}
		if (lowerObjectType == SharedObjectTypeEnum.ObjectType.Table || lowerObjectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			upperSwitchLayoutControlItem.Visibility = LayoutVisibility.Never;
			SetLowerProcessVisibility(visible: false);
		}
		if (upperObjectType == SharedObjectTypeEnum.ObjectType.View && lowerObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			upperSwitchLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
		if (IsViewAsProcessor)
		{
			inverFlowDirectionHyperlinkLabelControl.Enabled = false;
			inverFlowDirectionHyperlinkLabelControl.ToolTip = "View can only serve itself";
		}
		else
		{
			inverFlowDirectionHyperlinkLabelControl.Enabled = true;
			inverFlowDirectionHyperlinkLabelControl.ToolTip = null;
		}
		nonCustomizableLayoutControl1.EndInit();
	}

	private void CancelButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void OkButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void invertFlowDirectionHyperlinkLabelControl_Click(object sender, EventArgs e)
	{
		ChangeArrowDirection();
	}

	private void switchProcessorHyperlinkLabelControl_Click(object sender, EventArgs e)
	{
		nonCustomizableLayoutControl1.BeginInit();
		if (upperProcessLayoutControlItem.Visibility == LayoutVisibility.Always)
		{
			SetUpperProcessVisibility(visible: false);
			SetLowerProcessVisibility(visible: true);
		}
		else
		{
			SetUpperProcessVisibility(visible: true);
			SetLowerProcessVisibility(visible: false);
		}
		if (IsViewAsProcessor && IsOutFlow)
		{
			ChangeArrowDirection();
		}
		nonCustomizableLayoutControl1.EndInit();
		SetupForm();
	}

	private void SetUpperProcessVisibility(bool visible)
	{
		if (visible)
		{
			upperProcessLayoutControlItem.Visibility = LayoutVisibility.Always;
			upperSwitchLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else
		{
			upperProcessLayoutControlItem.Visibility = LayoutVisibility.Never;
			upperSwitchLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	private void SetLowerProcessVisibility(bool visible)
	{
		if (visible)
		{
			lowerProcessLayoutControlItem.Visibility = LayoutVisibility.Always;
			lowerSwitchLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else
		{
			lowerProcessLayoutControlItem.Visibility = LayoutVisibility.Never;
			lowerSwitchLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	private void ChangeArrowDirection()
	{
		nonCustomizableLayoutControl1.BeginInit();
		arrowImageLabelControl.ImageOptions.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
		if (ArrowDirection == ArrowDirection.Down)
		{
			ArrowDirection = ArrowDirection.Up;
		}
		else
		{
			ArrowDirection = ArrowDirection.Down;
		}
		nonCustomizableLayoutControl1.EndInit();
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (inverFlowDirectionHyperlinkLabelControl.Bounds.Contains(e.ControlMousePosition) && !string.IsNullOrWhiteSpace(inverFlowDirectionHyperlinkLabelControl.ToolTip))
		{
			e.Info = new ToolTipControlInfo(inverFlowDirectionHyperlinkLabelControl, inverFlowDirectionHyperlinkLabelControl.ToolTip, immediateToolTip: true, ToolTipIconType.Information);
		}
		else if (e.SelectedControl == arrowImageLabelControl)
		{
			e.Info = new ToolTipControlInfo(arrowImageLabelControl, IsInFlow ? "Inflow" : "Outflow");
		}
	}

	private void SetLookupHeight(GridLookUpEdit processGridLookUpEdit)
	{
		if (processGridLookUpEdit != null && processGridLookUpEdit.Properties.DataSource != null)
		{
			int count = (processGridLookUpEdit.Properties.DataSource as List<DataProcessRow>).Count;
			int num = ((count > 15) ? 15 : count);
			processGridLookUpEdit.Properties.PopupFormMinSize = new Size(processGridLookUpEdit.Width, (processGridLookUpEdit.Height + 2) * num);
			processGridLookUpEdit.Properties.PopupFormSize = new Size(processGridLookUpEdit.Width, (processGridLookUpEdit.Height + 2) * num);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.DataLineage.SelectProcessForm));
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.lowerProcessesLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.nameColumnLowerLookup = new DevExpress.XtraGrid.Columns.GridColumn();
		this.upperProcessesLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.nameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.flowDirectionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.inverFlowDirectionHyperlinkLabelControl = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.lowerSwitchProcessorHyperlinkLabelControl = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.arrowImageLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.upperSwitchProcessorHyperlinkLabelControl = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.lowerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.upperLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.okButton = new DevExpress.XtraEditors.SimpleButton();
		this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.upperLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.lowerLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.upperSwitchLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.lowerSwitchLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.invertFlowDirectionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.flowDirectionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.upperProcessLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.lowerProcessLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lowerProcessesLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.upperProcessesLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.upperLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lowerLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.upperSwitchLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lowerSwitchLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.invertFlowDirectionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.flowDirectionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.upperProcessLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lowerProcessLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.lowerProcessesLookUpEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.upperProcessesLookUpEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.flowDirectionLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.inverFlowDirectionHyperlinkLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.lowerSwitchProcessorHyperlinkLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.arrowImageLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.upperSwitchProcessorHyperlinkLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.lowerLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.upperLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.okButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.cancelButton);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(558, 256, 812, 500);
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(436, 223);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.ToolTipController = this.toolTipController;
		this.lowerProcessesLookUpEdit.Location = new System.Drawing.Point(106, 158);
		this.lowerProcessesLookUpEdit.Name = "lowerProcessesLookUpEdit";
		this.lowerProcessesLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lowerProcessesLookUpEdit.Properties.PopupView = this.gridView1;
		this.lowerProcessesLookUpEdit.Properties.ShowFooter = false;
		this.lowerProcessesLookUpEdit.Size = new System.Drawing.Size(194, 20);
		this.lowerProcessesLookUpEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.lowerProcessesLookUpEdit.TabIndex = 18;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.nameColumnLowerLookup });
		this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsBehavior.AutoPopulateColumns = false;
		this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView1.OptionsView.ShowColumnHeaders = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gridView1.OptionsView.ShowIndicator = false;
		this.gridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.nameColumnLowerLookup.Caption = "Name";
		this.nameColumnLowerLookup.FieldName = "Name";
		this.nameColumnLowerLookup.Name = "nameColumnLowerLookup";
		this.nameColumnLowerLookup.Visible = true;
		this.nameColumnLowerLookup.VisibleIndex = 0;
		this.upperProcessesLookUpEdit.EditValue = "";
		this.upperProcessesLookUpEdit.Location = new System.Drawing.Point(106, 36);
		this.upperProcessesLookUpEdit.Name = "upperProcessesLookUpEdit";
		this.upperProcessesLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.upperProcessesLookUpEdit.Properties.PopupView = this.gridLookUpEdit1View;
		this.upperProcessesLookUpEdit.Properties.ShowFooter = false;
		this.upperProcessesLookUpEdit.Size = new System.Drawing.Size(194, 20);
		this.upperProcessesLookUpEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.upperProcessesLookUpEdit.TabIndex = 17;
		this.gridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.nameColumn });
		this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
		this.gridLookUpEdit1View.OptionsBehavior.AutoPopulateColumns = false;
		this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridLookUpEdit1View.OptionsView.ShowColumnHeaders = false;
		this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.gridLookUpEdit1View.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gridLookUpEdit1View.OptionsView.ShowIndicator = false;
		this.gridLookUpEdit1View.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.nameColumn.Caption = "Name";
		this.nameColumn.FieldName = "Name";
		this.nameColumn.Name = "nameColumn";
		this.nameColumn.Visible = true;
		this.nameColumn.VisibleIndex = 0;
		this.flowDirectionLabelControl.Appearance.ForeColor = System.Drawing.Color.LightGray;
		this.flowDirectionLabelControl.Appearance.Options.UseForeColor = true;
		this.flowDirectionLabelControl.Location = new System.Drawing.Point(38, 77);
		this.flowDirectionLabelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.flowDirectionLabelControl.Name = "flowDirectionLabelControl";
		this.flowDirectionLabelControl.Size = new System.Drawing.Size(65, 13);
		this.flowDirectionLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.flowDirectionLabelControl.TabIndex = 16;
		this.flowDirectionLabelControl.Text = "Flow direction";
		this.inverFlowDirectionHyperlinkLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.inverFlowDirectionHyperlinkLabelControl.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("inverFlowDirectionHyperlinkLabelControl.ImageOptions.Image");
		this.inverFlowDirectionHyperlinkLabelControl.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
		this.inverFlowDirectionHyperlinkLabelControl.Location = new System.Drawing.Point(38, 94);
		this.inverFlowDirectionHyperlinkLabelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.inverFlowDirectionHyperlinkLabelControl.Name = "inverFlowDirectionHyperlinkLabelControl";
		this.inverFlowDirectionHyperlinkLabelControl.Size = new System.Drawing.Size(84, 20);
		this.inverFlowDirectionHyperlinkLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.inverFlowDirectionHyperlinkLabelControl.TabIndex = 15;
		this.inverFlowDirectionHyperlinkLabelControl.Text = "click to invert";
		this.inverFlowDirectionHyperlinkLabelControl.ToolTipController = this.toolTipController;
		this.inverFlowDirectionHyperlinkLabelControl.Click += new System.EventHandler(invertFlowDirectionHyperlinkLabelControl_Click);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		this.lowerSwitchProcessorHyperlinkLabelControl.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
		this.lowerSwitchProcessorHyperlinkLabelControl.Location = new System.Drawing.Point(304, 158);
		this.lowerSwitchProcessorHyperlinkLabelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lowerSwitchProcessorHyperlinkLabelControl.Name = "lowerSwitchProcessorHyperlinkLabelControl";
		this.lowerSwitchProcessorHyperlinkLabelControl.Size = new System.Drawing.Size(79, 13);
		this.lowerSwitchProcessorHyperlinkLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		toolTipItem.Text = "Use process from the other object";
		superToolTip.Items.Add(toolTipItem);
		this.lowerSwitchProcessorHyperlinkLabelControl.SuperTip = superToolTip;
		this.lowerSwitchProcessorHyperlinkLabelControl.TabIndex = 14;
		this.lowerSwitchProcessorHyperlinkLabelControl.Text = "switch processor";
		this.lowerSwitchProcessorHyperlinkLabelControl.Click += new System.EventHandler(switchProcessorHyperlinkLabelControl_Click);
		this.arrowImageLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_process_direction;
		this.arrowImageLabelControl.Location = new System.Drawing.Point(14, 70);
		this.arrowImageLabelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.arrowImageLabelControl.Name = "arrowImageLabelControl";
		this.arrowImageLabelControl.Size = new System.Drawing.Size(18, 50);
		this.arrowImageLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.arrowImageLabelControl.TabIndex = 13;
		this.arrowImageLabelControl.ToolTipController = this.toolTipController;
		this.upperSwitchProcessorHyperlinkLabelControl.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
		this.upperSwitchProcessorHyperlinkLabelControl.Location = new System.Drawing.Point(304, 36);
		this.upperSwitchProcessorHyperlinkLabelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.upperSwitchProcessorHyperlinkLabelControl.Name = "upperSwitchProcessorHyperlinkLabelControl";
		this.upperSwitchProcessorHyperlinkLabelControl.Size = new System.Drawing.Size(79, 13);
		this.upperSwitchProcessorHyperlinkLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		toolTipItem2.Text = "Use process from the other object";
		superToolTip2.Items.Add(toolTipItem2);
		this.upperSwitchProcessorHyperlinkLabelControl.SuperTip = superToolTip2;
		this.upperSwitchProcessorHyperlinkLabelControl.TabIndex = 11;
		this.upperSwitchProcessorHyperlinkLabelControl.Text = "switch processor";
		this.upperSwitchProcessorHyperlinkLabelControl.Click += new System.EventHandler(switchProcessorHyperlinkLabelControl_Click);
		this.lowerLabelControl.AutoEllipsis = true;
		this.lowerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.lowerLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.lowerLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.table_16;
		this.lowerLabelControl.Location = new System.Drawing.Point(12, 134);
		this.lowerLabelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lowerLabelControl.MaximumSize = new System.Drawing.Size(343, 0);
		this.lowerLabelControl.Name = "lowerLabelControl";
		this.lowerLabelControl.Size = new System.Drawing.Size(343, 20);
		this.lowerLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.lowerLabelControl.TabIndex = 10;
		this.lowerLabelControl.Text = "lowerLabelControl";
		this.upperLabelControl.AutoEllipsis = true;
		this.upperLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.upperLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.upperLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.function_16;
		this.upperLabelControl.Location = new System.Drawing.Point(12, 12);
		this.upperLabelControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.upperLabelControl.MaximumSize = new System.Drawing.Size(343, 0);
		this.upperLabelControl.Name = "upperLabelControl";
		this.upperLabelControl.Size = new System.Drawing.Size(343, 20);
		this.upperLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.upperLabelControl.TabIndex = 8;
		this.upperLabelControl.Text = "upperLabelControl";
		this.okButton.Location = new System.Drawing.Point(351, 194);
		this.okButton.Name = "okButton";
		this.okButton.Size = new System.Drawing.Size(56, 22);
		this.okButton.StyleController = this.nonCustomizableLayoutControl1;
		this.okButton.TabIndex = 7;
		this.okButton.Text = "Add";
		this.okButton.Click += new System.EventHandler(OkButton_Click);
		this.cancelButton.Location = new System.Drawing.Point(273, 194);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(65, 22);
		this.cancelButton.StyleController = this.nonCustomizableLayoutControl1;
		this.cancelButton.TabIndex = 6;
		this.cancelButton.Text = "Cancel";
		this.cancelButton.Click += new System.EventHandler(CancelButton_Click);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[20]
		{
			this.emptySpaceItem1, this.layoutControlItem4, this.layoutControlItem3, this.emptySpaceItem2, this.upperLayoutControlItem, this.lowerLayoutControlItem, this.emptySpaceItem3, this.layoutControlItem1, this.emptySpaceItem4, this.emptySpaceItem5,
			this.emptySpaceItem6, this.upperSwitchLayoutControlItem, this.lowerSwitchLayoutControlItem, this.invertFlowDirectionLayoutControlItem, this.flowDirectionLayoutControlItem, this.emptySpaceItem7, this.emptySpaceItem8, this.emptySpaceItem9, this.upperProcessLayoutControlItem, this.lowerProcessLayoutControlItem
		});
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(419, 228);
		this.Root.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 182);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(261, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.okButton;
		this.layoutControlItem4.Location = new System.Drawing.Point(339, 182);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(60, 26);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(60, 26);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(60, 26);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem3.Control = this.cancelButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(261, 182);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(69, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(69, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(69, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(26, 48);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(373, 17);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.upperLayoutControlItem.Control = this.upperLabelControl;
		this.upperLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.upperLayoutControlItem.Name = "upperLayoutControlItem";
		this.upperLayoutControlItem.Size = new System.Drawing.Size(399, 24);
		this.upperLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.upperLayoutControlItem.TextVisible = false;
		this.lowerLayoutControlItem.Control = this.lowerLabelControl;
		this.lowerLayoutControlItem.Location = new System.Drawing.Point(0, 122);
		this.lowerLayoutControlItem.Name = "lowerLayoutControlItem";
		this.lowerLayoutControlItem.Size = new System.Drawing.Size(399, 24);
		this.lowerLayoutControlItem.Text = "Target:";
		this.lowerLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.lowerLayoutControlItem.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 170);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(89, 12);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(399, 12);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.layoutControlItem1.Control = this.arrowImageLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 12, 12);
		this.layoutControlItem1.Size = new System.Drawing.Size(26, 74);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(330, 182);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(9, 0);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(9, 8);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(9, 26);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem5.MaxSize = new System.Drawing.Size(29, 0);
		this.emptySpaceItem5.MinSize = new System.Drawing.Size(29, 8);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(29, 24);
		this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 146);
		this.emptySpaceItem6.MaxSize = new System.Drawing.Size(29, 0);
		this.emptySpaceItem6.MinSize = new System.Drawing.Size(29, 8);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(29, 24);
		this.emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.upperSwitchLayoutControlItem.Control = this.upperSwitchProcessorHyperlinkLabelControl;
		this.upperSwitchLayoutControlItem.Location = new System.Drawing.Point(292, 24);
		this.upperSwitchLayoutControlItem.Name = "upperSwitchLayoutControlItem";
		this.upperSwitchLayoutControlItem.Size = new System.Drawing.Size(83, 24);
		this.upperSwitchLayoutControlItem.Text = "Process:";
		this.upperSwitchLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.upperSwitchLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.upperSwitchLayoutControlItem.TextToControlDistance = 0;
		this.upperSwitchLayoutControlItem.TextVisible = false;
		this.lowerSwitchLayoutControlItem.Control = this.lowerSwitchProcessorHyperlinkLabelControl;
		this.lowerSwitchLayoutControlItem.Location = new System.Drawing.Point(292, 146);
		this.lowerSwitchLayoutControlItem.Name = "lowerSwitchLayoutControlItem";
		this.lowerSwitchLayoutControlItem.Size = new System.Drawing.Size(83, 24);
		this.lowerSwitchLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.lowerSwitchLayoutControlItem.TextVisible = false;
		this.lowerSwitchLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.invertFlowDirectionLayoutControlItem.Control = this.inverFlowDirectionHyperlinkLabelControl;
		this.invertFlowDirectionLayoutControlItem.Location = new System.Drawing.Point(26, 82);
		this.invertFlowDirectionLayoutControlItem.Name = "invertFlowDirectionLayoutControlItem";
		this.invertFlowDirectionLayoutControlItem.Size = new System.Drawing.Size(373, 24);
		this.invertFlowDirectionLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.invertFlowDirectionLayoutControlItem.TextVisible = false;
		this.flowDirectionLayoutControlItem.Control = this.flowDirectionLabelControl;
		this.flowDirectionLayoutControlItem.Location = new System.Drawing.Point(26, 65);
		this.flowDirectionLayoutControlItem.Name = "flowDirectionLayoutControlItem";
		this.flowDirectionLayoutControlItem.Size = new System.Drawing.Size(373, 17);
		this.flowDirectionLayoutControlItem.Text = "Flow direction";
		this.flowDirectionLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.flowDirectionLayoutControlItem.TextVisible = false;
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.Location = new System.Drawing.Point(26, 106);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(373, 16);
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.Location = new System.Drawing.Point(375, 24);
		this.emptySpaceItem8.Name = "emptySpaceItem8";
		this.emptySpaceItem8.Size = new System.Drawing.Size(24, 24);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.Location = new System.Drawing.Point(375, 146);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(24, 24);
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.upperProcessLayoutControlItem.Control = this.upperProcessesLookUpEdit;
		this.upperProcessLayoutControlItem.ImageOptions.Image = Dataedo.App.Properties.Resources.process_16;
		this.upperProcessLayoutControlItem.Location = new System.Drawing.Point(29, 24);
		this.upperProcessLayoutControlItem.MaxSize = new System.Drawing.Size(263, 24);
		this.upperProcessLayoutControlItem.MinSize = new System.Drawing.Size(263, 24);
		this.upperProcessLayoutControlItem.Name = "upperProcessLayoutControlItem";
		this.upperProcessLayoutControlItem.Size = new System.Drawing.Size(263, 24);
		this.upperProcessLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.upperProcessLayoutControlItem.Text = "Process:";
		this.upperProcessLayoutControlItem.TextSize = new System.Drawing.Size(62, 16);
		this.lowerProcessLayoutControlItem.Control = this.lowerProcessesLookUpEdit;
		this.lowerProcessLayoutControlItem.ImageOptions.Image = Dataedo.App.Properties.Resources.process_16;
		this.lowerProcessLayoutControlItem.Location = new System.Drawing.Point(29, 146);
		this.lowerProcessLayoutControlItem.MaxSize = new System.Drawing.Size(263, 24);
		this.lowerProcessLayoutControlItem.MinSize = new System.Drawing.Size(263, 24);
		this.lowerProcessLayoutControlItem.Name = "lowerProcessLayoutControlItem";
		this.lowerProcessLayoutControlItem.Size = new System.Drawing.Size(263, 24);
		this.lowerProcessLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.lowerProcessLayoutControlItem.Text = "Process:";
		this.lowerProcessLayoutControlItem.TextSize = new System.Drawing.Size(62, 16);
		this.lowerProcessLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(436, 223);
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.MaximizeBox = false;
		base.Name = "SelectProcessForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Flow";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lowerProcessesLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.upperProcessesLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.upperLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lowerLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.upperSwitchLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lowerSwitchLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.invertFlowDirectionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.flowDirectionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.upperProcessLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lowerProcessLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
