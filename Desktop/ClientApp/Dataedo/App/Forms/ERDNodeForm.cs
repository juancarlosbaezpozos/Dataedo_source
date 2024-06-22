using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.ERD;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;

namespace Dataedo.App.Forms;

public class ERDNodeForm : BaseXtraForm
{
	private Node node;

	private List<NodeColumnDB> columns;

	private bool nodeChanged;

	private bool valuesChanged;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private ImageComboBoxEdit colorImageComboBoxEdit;

	private LayoutControlItem layoutControlItem2;

	private ImageCollection colorImageCollection;

	private GridControl columnsGridControl;

	private LayoutControlItem layoutControlItem1;

	private GridColumn selectedGridColumn;

	private GridColumn pkGridColumn;

	private GridColumn nameGridColumn;

	private GridColumn dataTypeGridColumn;

	private RepositoryItemCheckEdit selectedRepositoryItemCheckEdit;

	private RepositoryItemPictureEdit pkRepositoryItemPictureEdit;

	private SimpleButton closeSimpleButton;

	private LayoutControlItem layoutControlItem4;

	private EmptySpaceItem emptySpaceItem2;

	private SimpleButton okSimpleButton;

	private LayoutControlItem layoutControlItem5;

	private EmptySpaceItem checkEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem1;

	private HyperLinkEdit showAllHyperLinkEdit;

	private LayoutControlItem layoutControlItem7;

	private HyperLinkEdit hideAllHyperLinkEdit;

	private LayoutControlItem layoutControlItem8;

	private HyperLinkEdit showKeysHyperLinkEdit;

	private LayoutControlItem layoutControlItem9;

	private CustomGridUserControl columnsGridView;

	private RepositoryItemCustomTextEdit columnFullNameFormattedRepositoryItemCustomTextEdit;

	public ERDNodeForm(Node node)
	{
		InitializeComponent();
		colorImageComboBoxEdit.Properties.Items.AddRange(new ImageComboBoxItem[9]
		{
			new ImageComboBoxItem("Default", "Default", 0),
			new ImageComboBoxItem("Blue", "Blue", 1),
			new ImageComboBoxItem("Green", "Green", 2),
			new ImageComboBoxItem("Red", "Red", 3),
			new ImageComboBoxItem("Yellow", "Yellow", 4),
			new ImageComboBoxItem("Purple", "Purple", 5),
			new ImageComboBoxItem("Orange", "Orange", 6),
			new ImageComboBoxItem("Cyan", "Cyan", 7),
			new ImageComboBoxItem("Lime", "Lime", 8)
		});
		colorImageComboBoxEdit.Properties.DropDownRows = colorImageComboBoxEdit.Properties.Items.Count;
		this.node = node;
		columns = new List<NodeColumnDB>();
		foreach (NodeColumnDB column in node.Columns)
		{
			columns.Add(new NodeColumnDB(column));
		}
		Text = node.Type.ToString() + ": " + node.DisplayName;
		string nodeColorString = NodeColors.GetNodeColorString(node.Color);
		colorImageComboBoxEdit.EditValue = nodeColorString;
	}

	private void ERDNodeForm_Load(object sender, EventArgs e)
	{
		columnsGridControl.DataSource = columns;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			Save();
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void Save()
	{
		string text = colorImageComboBoxEdit.SelectedItem.ToString();
		if (!string.IsNullOrEmpty(text))
		{
			Color nodeColor = NodeColors.GetNodeColor(text);
			node.Color = nodeColor;
			node.Columns = new List<NodeColumnDB>(columns);
			node.RefreshColumnsListString();
			nodeChanged = true;
			valuesChanged = true;
			base.DialogResult = DialogResult.OK;
		}
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		if (nodeChanged)
		{
			node.ImportantChange?.Invoke(null, null);
		}
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void colorImageComboBoxEdit_SelectedIndexChanged(object sender, EventArgs e)
	{
		nodeChanged = true;
	}

	private void nodeDescriptionHtmlUserControl_ContentChangedEvent(object sender, EventArgs e)
	{
		nodeChanged = true;
		valuesChanged = true;
	}

	private void closeSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void RefreshGrid()
	{
		columnsGridView.RefreshRow(columnsGridView.FocusedRowHandle);
	}

	private void columnsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void columnsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left || !(sender is GridView gridView))
		{
			return;
		}
		GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.Location);
		if (gridHitInfo.InRowCell && gridHitInfo.Column.RealColumnEdit is RepositoryItemCheckEdit)
		{
			gridView.FocusedColumn = gridHitInfo.Column;
			gridView.FocusedRowHandle = gridHitInfo.RowHandle;
			gridView.ShowEditor();
			if (gridView.ActiveEditor is CheckEdit checkEdit)
			{
				checkEdit.Toggle();
				NodeColumnDB selectedColumn = gridView.GetFocusedRow() as NodeColumnDB;
				gridView.CloseEditor();
				ColumnsSelector.SetColumnsSelection(selectedColumn, columns);
				columnsGridView.RefreshData();
				DXMouseEventArgs.GetMouseArgs(e).Handled = true;
			}
		}
	}

	private void showAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		valuesChanged = true;
		if (!(columnsGridView.GridControl.DataSource is IEnumerable<NodeColumnDB> enumerable))
		{
			return;
		}
		foreach (NodeColumnDB item in enumerable)
		{
			item.Selected = true;
		}
		columnsGridControl.RefreshDataSource();
	}

	private void hideAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		valuesChanged = true;
		foreach (NodeColumnDB item in columnsGridView.DataSource as IEnumerable<NodeColumnDB>)
		{
			item.Selected = false;
		}
		columnsGridControl.RefreshDataSource();
	}

	private void showKeyHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		valuesChanged = true;
		IEnumerable<NodeColumnDB> enumerable = (columnsGridView.DataSource as IEnumerable<NodeColumnDB>).Where((NodeColumnDB x) => x.UniqueConstraintsDataContainer.IsKey);
		columns.ForEach(delegate(NodeColumnDB x)
		{
			x.Selected = false;
		});
		foreach (NodeColumnDB item in enumerable)
		{
			item.Selected = true;
			ColumnsSelector.SelectParentColumns(item, columns);
		}
		columnsGridView.RefreshData();
		columnsGridControl.RefreshDataSource();
	}

	private void columnsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		valuesChanged = true;
	}

	private void ERDNodeForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
		}
		else if (valuesChanged)
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Node has been changed, would you like to save these changes?", "Node has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				Save();
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ERDNodeForm));
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.showAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.closeSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.columnsGridControl = new DevExpress.XtraGrid.GridControl();
		this.columnsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.selectedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.selectedRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.pkGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.pkRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colorImageComboBoxEdit = new DevExpress.XtraEditors.ImageComboBoxEdit();
		this.colorImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.hideAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.showKeysHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.checkEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnFullNameFormattedRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.showAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectedRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pkRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.colorImageComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.colorImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hideAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.showKeysHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameFormattedRepositoryItemCustomTextEdit).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.Controls.Add(this.showAllHyperLinkEdit);
		this.layoutControl1.Controls.Add(this.okSimpleButton);
		this.layoutControl1.Controls.Add(this.closeSimpleButton);
		this.layoutControl1.Controls.Add(this.columnsGridControl);
		this.layoutControl1.Controls.Add(this.colorImageComboBoxEdit);
		this.layoutControl1.Controls.Add(this.hideAllHyperLinkEdit);
		this.layoutControl1.Controls.Add(this.showKeysHyperLinkEdit);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(875, 438, 771, 480);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(697, 389);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl";
		this.showAllHyperLinkEdit.EditValue = "Show all columns";
		this.showAllHyperLinkEdit.Location = new System.Drawing.Point(12, 12);
		this.showAllHyperLinkEdit.Name = "showAllHyperLinkEdit";
		this.showAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.showAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.showAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.showAllHyperLinkEdit.Size = new System.Drawing.Size(92, 18);
		this.showAllHyperLinkEdit.StyleController = this.layoutControl1;
		this.showAllHyperLinkEdit.TabIndex = 8;
		this.showAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(showAllHyperLinkEdit_OpenLink);
		this.okSimpleButton.Location = new System.Drawing.Point(509, 355);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.layoutControl1;
		this.okSimpleButton.TabIndex = 4;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.closeSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.closeSimpleButton.Location = new System.Drawing.Point(605, 355);
		this.closeSimpleButton.Name = "closeSimpleButton";
		this.closeSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.closeSimpleButton.StyleController = this.layoutControl1;
		this.closeSimpleButton.TabIndex = 5;
		this.closeSimpleButton.Text = "Cancel";
		this.closeSimpleButton.Click += new System.EventHandler(closeSimpleButton_Click);
		this.columnsGridControl.Location = new System.Drawing.Point(12, 50);
		this.columnsGridControl.MainView = this.columnsGridView;
		this.columnsGridControl.Name = "columnsGridControl";
		this.columnsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.selectedRepositoryItemCheckEdit, this.pkRepositoryItemPictureEdit, this.columnFullNameFormattedRepositoryItemCustomTextEdit });
		this.columnsGridControl.Size = new System.Drawing.Size(673, 276);
		this.columnsGridControl.TabIndex = 1;
		this.columnsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.columnsGridView });
		this.columnsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.selectedGridColumn, this.pkGridColumn, this.nameGridColumn, this.dataTypeGridColumn });
		this.columnsGridView.GridControl = this.columnsGridControl;
		this.columnsGridView.Name = "columnsGridView";
		this.columnsGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.columnsGridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
		this.columnsGridView.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
		this.columnsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.columnsGridView.OptionsBehavior.FocusLeaveOnTab = true;
		this.columnsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.columnsGridView.OptionsCustomization.AllowFilter = false;
		this.columnsGridView.OptionsCustomization.AllowSort = false;
		this.columnsGridView.OptionsDetail.EnableMasterViewMode = false;
		this.columnsGridView.OptionsView.ShowGroupExpandCollapseButtons = false;
		this.columnsGridView.OptionsView.ShowGroupPanel = false;
		this.columnsGridView.OptionsView.ShowIndicator = false;
		this.columnsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(columnsGridView_PopupMenuShowing);
		this.columnsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(columnsGridView_CellValueChanged);
		this.columnsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(columnsGridView_MouseDown);
		this.selectedGridColumn.ColumnEdit = this.selectedRepositoryItemCheckEdit;
		this.selectedGridColumn.FieldName = "Selected";
		this.selectedGridColumn.MaxWidth = 26;
		this.selectedGridColumn.Name = "selectedGridColumn";
		this.selectedGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.selectedGridColumn.OptionsColumn.ShowCaption = false;
		this.selectedGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.selectedGridColumn.OptionsFilter.AllowFilter = false;
		this.selectedGridColumn.Visible = true;
		this.selectedGridColumn.VisibleIndex = 0;
		this.selectedGridColumn.Width = 26;
		this.selectedRepositoryItemCheckEdit.AutoHeight = false;
		this.selectedRepositoryItemCheckEdit.Name = "selectedRepositoryItemCheckEdit";
		this.pkGridColumn.Caption = "Key";
		this.pkGridColumn.ColumnEdit = this.pkRepositoryItemPictureEdit;
		this.pkGridColumn.FieldName = "UniqueConstraintIcon";
		this.pkGridColumn.MaxWidth = 30;
		this.pkGridColumn.MinWidth = 30;
		this.pkGridColumn.Name = "pkGridColumn";
		this.pkGridColumn.OptionsColumn.AllowEdit = false;
		this.pkGridColumn.OptionsColumn.ReadOnly = true;
		this.pkGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.pkGridColumn.Visible = true;
		this.pkGridColumn.VisibleIndex = 1;
		this.pkGridColumn.Width = 30;
		this.pkRepositoryItemPictureEdit.AllowFocused = false;
		this.pkRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.pkRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pkRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pkRepositoryItemPictureEdit.Name = "pkRepositoryItemPictureEdit";
		this.pkRepositoryItemPictureEdit.ShowMenu = false;
		this.nameGridColumn.Caption = "Name";
		this.nameGridColumn.ColumnEdit = this.columnFullNameFormattedRepositoryItemCustomTextEdit;
		this.nameGridColumn.FieldName = "FullNameFormatted";
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsColumn.AllowEdit = false;
		this.nameGridColumn.OptionsColumn.ReadOnly = true;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 2;
		this.nameGridColumn.Width = 297;
		this.dataTypeGridColumn.Caption = "Data type";
		this.dataTypeGridColumn.FieldName = "DataType";
		this.dataTypeGridColumn.Name = "dataTypeGridColumn";
		this.dataTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.dataTypeGridColumn.OptionsColumn.ReadOnly = true;
		this.dataTypeGridColumn.Visible = true;
		this.dataTypeGridColumn.VisibleIndex = 3;
		this.dataTypeGridColumn.Width = 318;
		this.colorImageComboBoxEdit.Location = new System.Drawing.Point(52, 330);
		this.colorImageComboBoxEdit.Name = "colorImageComboBoxEdit";
		this.colorImageComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.colorImageComboBoxEdit.Properties.SmallImages = this.colorImageCollection;
		this.colorImageComboBoxEdit.Size = new System.Drawing.Size(201, 20);
		this.colorImageComboBoxEdit.StyleController = this.layoutControl1;
		this.colorImageComboBoxEdit.TabIndex = 3;
		this.colorImageComboBoxEdit.SelectedIndexChanged += new System.EventHandler(colorImageComboBoxEdit_SelectedIndexChanged);
		this.colorImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("colorImageCollection.ImageStream");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_gray, "color_gray", typeof(Dataedo.App.Properties.Resources), 0);
		this.colorImageCollection.Images.SetKeyName(0, "color_gray");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_blue, "color_blue", typeof(Dataedo.App.Properties.Resources), 1);
		this.colorImageCollection.Images.SetKeyName(1, "color_blue");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_green, "color_green", typeof(Dataedo.App.Properties.Resources), 2);
		this.colorImageCollection.Images.SetKeyName(2, "color_green");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_red, "color_red", typeof(Dataedo.App.Properties.Resources), 3);
		this.colorImageCollection.Images.SetKeyName(3, "color_red");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_yellow, "color_yellow", typeof(Dataedo.App.Properties.Resources), 4);
		this.colorImageCollection.Images.SetKeyName(4, "color_yellow");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_purple, "color_purple", typeof(Dataedo.App.Properties.Resources), 5);
		this.colorImageCollection.Images.SetKeyName(5, "color_purple");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_orange, "color_orange", typeof(Dataedo.App.Properties.Resources), 6);
		this.colorImageCollection.Images.SetKeyName(6, "color_orange");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_cyan, "color_cyan", typeof(Dataedo.App.Properties.Resources), 7);
		this.colorImageCollection.Images.SetKeyName(7, "color_cyan");
		this.colorImageCollection.InsertImage(Dataedo.App.Properties.Resources.color_lime, "color_lime", typeof(Dataedo.App.Properties.Resources), 8);
		this.colorImageCollection.Images.SetKeyName(8, "color_lime");
		this.hideAllHyperLinkEdit.EditValue = "Hide all columns";
		this.hideAllHyperLinkEdit.Location = new System.Drawing.Point(235, 12);
		this.hideAllHyperLinkEdit.Name = "hideAllHyperLinkEdit";
		this.hideAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.hideAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.hideAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.hideAllHyperLinkEdit.Size = new System.Drawing.Size(86, 18);
		this.hideAllHyperLinkEdit.StyleController = this.layoutControl1;
		this.hideAllHyperLinkEdit.TabIndex = 8;
		this.hideAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(hideAllHyperLinkEdit_OpenLink);
		this.showKeysHyperLinkEdit.EditValue = "Show key columns only";
		this.showKeysHyperLinkEdit.Location = new System.Drawing.Point(108, 12);
		this.showKeysHyperLinkEdit.Name = "showKeysHyperLinkEdit";
		this.showKeysHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.showKeysHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.showKeysHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.showKeysHyperLinkEdit.Size = new System.Drawing.Size(123, 18);
		this.showKeysHyperLinkEdit.StyleController = this.layoutControl1;
		this.showKeysHyperLinkEdit.TabIndex = 9;
		this.showKeysHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(showKeyHyperLinkEdit_OpenLink);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem4, this.emptySpaceItem2, this.layoutControlItem5, this.emptySpaceItem1, this.checkEmptySpaceItem, this.layoutControlItem7, this.layoutControlItem9, this.layoutControlItem8 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(697, 389);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.columnsGridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 22);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(677, 296);
		this.layoutControlItem1.Text = "Columns:";
		this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(44, 13);
		this.layoutControlItem2.Control = this.colorImageComboBoxEdit;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 318);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(245, 25);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(245, 25);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(677, 25);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.Text = "Color:";
		this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(40, 13);
		this.layoutControlItem2.TextToControlDistance = 0;
		this.layoutControlItem4.Control = this.closeSimpleButton;
		this.layoutControlItem4.Location = new System.Drawing.Point(593, 343);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(581, 343);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.Control = this.okSimpleButton;
		this.layoutControlItem5.Location = new System.Drawing.Point(497, 343);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 343);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(497, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.checkEmptySpaceItem.AllowHotTrack = false;
		this.checkEmptySpaceItem.CustomizationFormText = "checkEmptySpaceItem";
		this.checkEmptySpaceItem.Location = new System.Drawing.Point(313, 0);
		this.checkEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 20);
		this.checkEmptySpaceItem.MinSize = new System.Drawing.Size(104, 20);
		this.checkEmptySpaceItem.Name = "checkEmptySpaceItem";
		this.checkEmptySpaceItem.Size = new System.Drawing.Size(364, 22);
		this.checkEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.checkEmptySpaceItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.checkEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.Control = this.showAllHyperLinkEdit;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(96, 22);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(96, 22);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(96, 22);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem9.Control = this.showKeysHyperLinkEdit;
		this.layoutControlItem9.CustomizationFormText = "layoutControlItem9";
		this.layoutControlItem9.Location = new System.Drawing.Point(96, 0);
		this.layoutControlItem9.MaxSize = new System.Drawing.Size(127, 22);
		this.layoutControlItem9.MinSize = new System.Drawing.Size(127, 22);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(127, 22);
		this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem8.Control = this.hideAllHyperLinkEdit;
		this.layoutControlItem8.CustomizationFormText = "layoutControlItem7";
		this.layoutControlItem8.FillControlToClientArea = false;
		this.layoutControlItem8.Location = new System.Drawing.Point(223, 0);
		this.layoutControlItem8.MaxSize = new System.Drawing.Size(90, 22);
		this.layoutControlItem8.MinSize = new System.Drawing.Size(90, 22);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(90, 22);
		this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem8.Text = "layoutControlItem7";
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.columnFullNameFormattedRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.columnFullNameFormattedRepositoryItemCustomTextEdit.AutoHeight = false;
		this.columnFullNameFormattedRepositoryItemCustomTextEdit.Name = "columnFullNameFormattedRepositoryItemCustomTextEdit";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(697, 389);
		base.Controls.Add(this.layoutControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ERDNodeForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "ERDNodeForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Table";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ERDNodeForm_FormClosing);
		base.Load += new System.EventHandler(ERDNodeForm_Load);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.showAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectedRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pkRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.colorImageComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.colorImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hideAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.showKeysHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameFormattedRepositoryItemCustomTextEdit).EndInit();
		base.ResumeLayout(false);
	}
}
