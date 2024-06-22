using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classification.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Classificator;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.Classification.UserControls;

public class ClassificationChooserUserControl : BaseUserControl
{
	public delegate void ClassificationChangedHandler();

	private List<ClassificatorModel> classificators;

	private CustomFieldsSupport customFieldsSupport;

	private bool isFirstLoad = true;

	private ClassificatorModel mouseRightClickSelectedClassificator;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private LabelControl chooseClassificationLabelControl;

	private TreeList classificatorChooserTreeList;

	private TreeListColumn titleColumn;

	private LayoutControlItem chooseClassificationLayoutControlItem;

	private LayoutControlItem layoutControlItem2;

	private SimpleButton addNewClassificationSimpleButton;

	private LayoutControlItem addNewClassificationLayoutControlItem;

	private EmptySpaceItem emptySpaceItem;

	private PopupMenu popupMenu;

	private BarButtonItem addBarButtonItem;

	private BarButtonItem configureBarButtonItem;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	public ClassificatorModel ChoosenClassificator => classificatorChooserTreeList.GetFocusedRow() as ClassificatorModel;

	[Browsable(true)]
	public event ClassificationChangedHandler ClassificationChanged;

	public ClassificationChooserUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(CustomFieldsSupport customFieldsSupport)
	{
		this.customFieldsSupport = customFieldsSupport;
	}

	public void InitDataSource(bool keepFocus = false)
	{
		TreeListNode focusedNode = classificatorChooserTreeList.FocusedNode;
		int? num = (classificatorChooserTreeList.GetDataRecordByNode(focusedNode) as ClassificatorModel)?.Id;
		classificators = (from x in DB.Classificator.GetClassificators()
			orderby x.Title
			select x).ToList();
		classificatorChooserTreeList.DataSource = classificators;
		if (keepFocus && num.HasValue)
		{
			SetNodeById(num.Value);
		}
	}

	private void SetNodeById(int classificatorId)
	{
		foreach (TreeListNode node in classificatorChooserTreeList.Nodes)
		{
			if ((classificatorChooserTreeList.GetDataRecordByNode(node) as ClassificatorModel).Id == classificatorId)
			{
				classificatorChooserTreeList.FocusedNode = node;
				break;
			}
		}
	}

	private void AddNewClassificationSimpleButton_Click(object sender, EventArgs e)
	{
		AddNewClassification();
	}

	private void ClassificatorChooserTreeList_AfterFocusNode(object sender, NodeEventArgs e)
	{
		this.ClassificationChanged?.Invoke();
	}

	private void AddBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddNewClassification();
	}

	private void ConfigureBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ConfigureClassification(mouseRightClickSelectedClassificator);
		mouseRightClickSelectedClassificator = null;
	}

	public void AddNewClassification()
	{
		try
		{
			using EditClassification editClassification = new EditClassification();
			ClassificatorModel classificatorModel = new ClassificatorModel
			{
				Title = "New classification"
			};
			editClassification.SetParameters(classificatorModel, customFieldsSupport);
			editClassification.ShowDialog(this);
			InitDataSource();
			if (editClassification.EditedClassificatorId.HasValue)
			{
				SetNodeById(editClassification.EditedClassificatorId.Value);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding new classification", FindForm());
		}
	}

	public void ConfigureClassification(ClassificatorModel classification)
	{
		try
		{
			using EditClassification editClassification = new EditClassification();
			editClassification.SetParameters(classification, customFieldsSupport);
			editClassification.ShowDialog(this);
			InitDataSource();
			if (editClassification.EditedClassificatorId.HasValue)
			{
				SetNodeById(editClassification.EditedClassificatorId.Value);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while configuring new classification", FindForm());
		}
	}

	private void ClassificatorChooserTreeList_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			TreeList treeList = sender as TreeList;
			treeList.Focus();
			TreeListHitInfo treeListHitInfo = treeList.CalcHitInfo(new Point(e.X, e.Y));
			mouseRightClickSelectedClassificator = null;
			popupMenu.ItemLinks.Clear();
			popupMenu.ItemLinks.Add(addBarButtonItem);
			if (treeListHitInfo != null && treeListHitInfo.InRow && treeList.GetDataRecordByNode(treeListHitInfo.Node) is ClassificatorModel classificatorModel)
			{
				mouseRightClickSelectedClassificator = classificatorModel;
				popupMenu.ItemLinks.Add(configureBarButtonItem);
			}
			if (popupMenu.ItemLinks.Any())
			{
				popupMenu.ShowPopup(Control.MousePosition);
			}
		}
	}

	private void PopupMenu_CloseUp(object sender, EventArgs e)
	{
		mouseRightClickSelectedClassificator = null;
	}

	private void ClassificatorChooserTreeList_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		if (e.OldNode == null && isFirstLoad)
		{
			e.CanFocus = false;
			isFirstLoad = false;
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
		this.addNewClassificationSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.chooseClassificationLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.classificatorChooserTreeList = new DevExpress.XtraTreeList.TreeList();
		this.titleColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.chooseClassificationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.addNewClassificationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.configureBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.classificatorChooserTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chooseClassificationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.addNewClassificationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.addNewClassificationSimpleButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.chooseClassificationLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.classificatorChooserTreeList);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(826, 4, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(452, 296);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl";
		this.addNewClassificationSimpleButton.AllowFocus = false;
		this.addNewClassificationSimpleButton.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.addNewClassificationSimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.addNewClassificationSimpleButton.Location = new System.Drawing.Point(420, 10);
		this.addNewClassificationSimpleButton.Margin = new System.Windows.Forms.Padding(0);
		this.addNewClassificationSimpleButton.Name = "addNewClassificationSimpleButton";
		this.addNewClassificationSimpleButton.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
		this.addNewClassificationSimpleButton.Size = new System.Drawing.Size(22, 22);
		this.addNewClassificationSimpleButton.StyleController = this.nonCustomizableLayoutControl;
		this.addNewClassificationSimpleButton.TabIndex = 10;
		this.addNewClassificationSimpleButton.ToolTip = "Add new classification";
		this.addNewClassificationSimpleButton.Click += new System.EventHandler(AddNewClassificationSimpleButton_Click);
		this.chooseClassificationLabelControl.Location = new System.Drawing.Point(12, 17);
		this.chooseClassificationLabelControl.Name = "chooseClassificationLabelControl";
		this.chooseClassificationLabelControl.Size = new System.Drawing.Size(123, 13);
		this.chooseClassificationLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.chooseClassificationLabelControl.TabIndex = 9;
		this.chooseClassificationLabelControl.Text = "Choose data classification";
		this.classificatorChooserTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.titleColumn });
		this.classificatorChooserTreeList.CustomizationFormBounds = new System.Drawing.Rectangle(1286, 406, 250, 280);
		this.classificatorChooserTreeList.Location = new System.Drawing.Point(12, 34);
		this.classificatorChooserTreeList.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
		this.classificatorChooserTreeList.Name = "classificatorChooserTreeList";
		this.classificatorChooserTreeList.OptionsBehavior.AllowExpandOnDblClick = false;
		this.classificatorChooserTreeList.OptionsBehavior.AutoPopulateColumns = false;
		this.classificatorChooserTreeList.OptionsBehavior.Editable = false;
		this.classificatorChooserTreeList.OptionsBehavior.ReadOnly = true;
		this.classificatorChooserTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.classificatorChooserTreeList.OptionsCustomization.AllowColumnMoving = false;
		this.classificatorChooserTreeList.OptionsCustomization.AllowQuickHideColumns = false;
		this.classificatorChooserTreeList.OptionsMenu.ShowExpandCollapseItems = false;
		this.classificatorChooserTreeList.OptionsPrint.AutoRowHeight = false;
		this.classificatorChooserTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.classificatorChooserTreeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
		this.classificatorChooserTreeList.OptionsView.ShowBandsMode = DevExpress.Utils.DefaultBoolean.False;
		this.classificatorChooserTreeList.OptionsView.ShowButtons = false;
		this.classificatorChooserTreeList.OptionsView.ShowColumns = false;
		this.classificatorChooserTreeList.OptionsView.ShowHorzLines = false;
		this.classificatorChooserTreeList.OptionsView.ShowIndicator = false;
		this.classificatorChooserTreeList.OptionsView.ShowRoot = false;
		this.classificatorChooserTreeList.OptionsView.ShowVertLines = false;
		this.classificatorChooserTreeList.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
		this.classificatorChooserTreeList.RowHeight = 28;
		this.classificatorChooserTreeList.Size = new System.Drawing.Size(428, 250);
		this.classificatorChooserTreeList.TabIndex = 8;
		this.classificatorChooserTreeList.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(ClassificatorChooserTreeList_BeforeFocusNode);
		this.classificatorChooserTreeList.AfterFocusNode += new DevExpress.XtraTreeList.NodeEventHandler(ClassificatorChooserTreeList_AfterFocusNode);
		this.classificatorChooserTreeList.MouseDown += new System.Windows.Forms.MouseEventHandler(ClassificatorChooserTreeList_MouseDown);
		this.titleColumn.Caption = "titleColumn";
		this.titleColumn.FieldName = "Title";
		this.titleColumn.Name = "titleColumn";
		this.titleColumn.Visible = true;
		this.titleColumn.VisibleIndex = 0;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.chooseClassificationLayoutControlItem, this.layoutControlItem2, this.addNewClassificationLayoutControlItem, this.emptySpaceItem });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(452, 296);
		this.Root.TextVisible = false;
		this.chooseClassificationLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Bottom;
		this.chooseClassificationLayoutControlItem.Control = this.chooseClassificationLabelControl;
		this.chooseClassificationLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.chooseClassificationLayoutControlItem.Name = "chooseClassificationLayoutControlItem";
		this.chooseClassificationLayoutControlItem.Size = new System.Drawing.Size(127, 22);
		this.chooseClassificationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.chooseClassificationLayoutControlItem.TextVisible = false;
		this.layoutControlItem2.Control = this.classificatorChooserTreeList;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 22);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(432, 254);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.addNewClassificationLayoutControlItem.Control = this.addNewClassificationSimpleButton;
		this.addNewClassificationLayoutControlItem.Location = new System.Drawing.Point(410, 0);
		this.addNewClassificationLayoutControlItem.MaxSize = new System.Drawing.Size(22, 22);
		this.addNewClassificationLayoutControlItem.MinSize = new System.Drawing.Size(22, 22);
		this.addNewClassificationLayoutControlItem.Name = "addNewClassificationLayoutControlItem";
		this.addNewClassificationLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.addNewClassificationLayoutControlItem.Size = new System.Drawing.Size(22, 22);
		this.addNewClassificationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.addNewClassificationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.addNewClassificationLayoutControlItem.TextVisible = false;
		this.emptySpaceItem.AllowHotTrack = false;
		this.emptySpaceItem.Location = new System.Drawing.Point(127, 0);
		this.emptySpaceItem.Name = "emptySpaceItem";
		this.emptySpaceItem.Size = new System.Drawing.Size(283, 22);
		this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.popupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.configureBarButtonItem)
		});
		this.popupMenu.Manager = this.barManager1;
		this.popupMenu.Name = "popupMenu";
		this.popupMenu.CloseUp += new System.EventHandler(PopupMenu_CloseUp);
		this.addBarButtonItem.Caption = "Add Classification";
		this.addBarButtonItem.Id = 0;
		this.addBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.addBarButtonItem.Name = "addBarButtonItem";
		this.addBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddBarButtonItem_ItemClick);
		this.configureBarButtonItem.Caption = "Configure Classification";
		this.configureBarButtonItem.Id = 1;
		this.configureBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.configureBarButtonItem.Name = "configureBarButtonItem";
		this.configureBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ConfigureBarButtonItem_ItemClick);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[2] { this.addBarButtonItem, this.configureBarButtonItem });
		this.barManager1.MaxItemId = 2;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(452, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 296);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(452, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 296);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(452, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 296);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ClassificationChooserUserControl";
		base.Size = new System.Drawing.Size(452, 296);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.classificatorChooserTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chooseClassificationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.addNewClassificationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
