using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classification.Forms;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Classification.UserControls;

public class ClassificationRulesUserControl : UserControl
{
	public delegate void ValuesFromMasksChangedHandler();

	private ClassificatorModelRow classificatorModelRow;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private GridControl gridControl;

	private GridView gridView;

	private LayoutControlGroup Root;

	private LayoutControlItem layoutControlItem;

	private SimpleButton manageRulesButton;

	private LayoutControlItem manageRulesButtonLlayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private GridColumn maskNameGridColumn;

	private GridColumn maskPatternsGridColumn;

	private GridColumn customField1GridColumn;

	private GridColumn customField2GridColumn;

	private GridColumn customField3GridColumn;

	private GridColumn customField4GridColumn;

	private GridColumn customField5GridColumn;

	private ToolTipController toolTipController;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private RepositoryItemComboBox repositoryItemComboBox2;

	private RepositoryItemComboBox repositoryItemComboBox3;

	private RepositoryItemComboBox repositoryItemComboBox4;

	private RepositoryItemComboBox repositoryItemComboBox5;

	[Browsable(true)]
	public event ValuesFromMasksChangedHandler ValuesFromMasksChanged;

	public ClassificationRulesUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(ClassificatorModelRow classificatorModelRow)
	{
		this.classificatorModelRow = classificatorModelRow;
		RefreshManageRulesButton();
		gridControl.DataSource = this.classificatorModelRow.Rules;
		maskNameGridColumn.SortOrder = ColumnSortOrder.Ascending;
		RefreshColumnsHeaders();
	}

	public void RefreshManageRulesButton()
	{
		if (classificatorModelRow.Id == 0)
		{
			manageRulesButton.Enabled = false;
			ButtonsHelpers.AddSuperTip(manageRulesButton, "Managing rules is possible only for saved Classifications.");
		}
		else
		{
			manageRulesButton.Enabled = true;
			manageRulesButton.SuperTip?.Items?.Clear();
		}
	}

	public void RefreshDataSource()
	{
		gridControl.RefreshDataSource();
	}

	public void RefreshColumnsHeaders()
	{
		int i;
		for (i = 1; i <= 5; i++)
		{
			ClassificatorCustomFieldRow classificatorCustomFieldRow = classificatorModelRow.UsedFields.Where((ClassificatorCustomField x) => x.Number == i).FirstOrDefault() as ClassificatorCustomFieldRow;
			SetColumn(GetColumnByFieldNumber(i), classificatorCustomFieldRow);
		}
	}

	private GridColumn GetColumnByFieldNumber(int number)
	{
		return number switch
		{
			1 => customField1GridColumn, 
			2 => customField2GridColumn, 
			3 => customField3GridColumn, 
			4 => customField4GridColumn, 
			5 => customField5GridColumn, 
			_ => null, 
		};
	}

	private void SetColumn(GridColumn gridColumn, ClassificatorCustomFieldRow classificatorCustomFieldRow)
	{
		gridColumn.Caption = classificatorCustomFieldRow?.Title ?? " ";
		gridColumn.Tag = classificatorCustomFieldRow;
		if (string.IsNullOrWhiteSpace(gridColumn.Caption))
		{
			gridColumn.OptionsColumn.AllowEdit = false;
			gridColumn.VisibleIndex = 10;
		}
		else
		{
			gridColumn.OptionsColumn.AllowEdit = true;
		}
	}

	private int? GetFieldNumberByColumn(GridColumn gridColumn)
	{
		if (gridColumn == customField1GridColumn)
		{
			return 1;
		}
		if (gridColumn == customField2GridColumn)
		{
			return 2;
		}
		if (gridColumn == customField3GridColumn)
		{
			return 3;
		}
		if (gridColumn == customField4GridColumn)
		{
			return 4;
		}
		if (gridColumn == customField5GridColumn)
		{
			return 5;
		}
		return null;
	}

	public void CloseEditor()
	{
		gridView.CloseEditor();
	}

	public bool IsEditorFocused()
	{
		return gridView.IsEditorFocused;
	}

	private void GridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void GridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		GridViewHelpers.GrayOutNoneditableColumns(sender as GridView, e);
		if (e.Column?.Tag != null)
		{
			GridColumn column = e.Column;
			if (column == null || column.OptionsColumn?.AllowEdit != false)
			{
				ClassificationRuleRow ruleRow = gridView.GetRow(e.RowHandle) as ClassificationRuleRow;
				if (ruleRow == null)
				{
					RestoreCellBackgroundColor(e);
				}
				else if (Array.TrueForAll(Enumerable.Range(1, 5).ToArray(), (int x) => string.IsNullOrEmpty(ruleRow.GetCustomFieldValue(x))))
				{
					SetCellHighlightedBackgroundColor(e);
				}
				else if (classificatorModelRow.Rules.TrueForAll((ClassificationRuleRow x) => string.IsNullOrEmpty(x.GetCustomFieldValue((e.Column.Tag as ClassificatorCustomFieldRow).Number))))
				{
					SetCellHighlightedBackgroundColor(e);
				}
				else
				{
					RestoreCellBackgroundColor(e);
				}
				return;
			}
		}
		RestoreCellBackgroundColor(e);
	}

	private void SetCellHighlightedBackgroundColor(RowCellCustomDrawEventArgs e)
	{
		if (e.Appearance.BackColor != SkinsManager.CurrentSkin.SearchHighlightColor)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.SearchHighlightColor;
		}
	}

	private void RestoreCellBackgroundColor(RowCellCustomDrawEventArgs e)
	{
		if (e.Appearance.BackColor == SkinsManager.CurrentSkin.SearchHighlightColor)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridGridRowBackColor;
		}
	}

	private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (GetFieldNumberByColumn(e.Column).HasValue)
		{
			this.ValuesFromMasksChanged?.Invoke();
		}
		(gridView.GetRow(e.RowHandle) as ClassificationRuleRow)?.SetUpdatedIfNotAdded();
		classificatorModelRow.SetUpdatedIfNotAdded();
	}

	private void ManageRulesButton_Click(object sender, EventArgs e)
	{
		try
		{
			using (RulesConfiguration rulesConfiguration = new RulesConfiguration())
			{
				rulesConfiguration.SetParameters(classificatorModelRow.Id, classificatorModelRow);
				rulesConfiguration.ShowDialog(FindForm());
			}
			classificatorModelRow.RefreshRules();
			gridControl.RefreshDataSource();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while managing rules", FindForm());
		}
	}

	private void GridView_ShownEditor(object sender, EventArgs e)
	{
		if (sender is GridView gridView && gridView.ActiveEditor is TextEdit textEdit)
		{
			GridColumn focusedColumn = gridView.FocusedColumn;
			if (focusedColumn == customField1GridColumn || focusedColumn == customField2GridColumn || focusedColumn == customField3GridColumn || focusedColumn == customField4GridColumn || focusedColumn == customField5GridColumn)
			{
				textEdit.Properties.MaxLength = 250;
			}
		}
	}

	private void NonCustomizableLayoutControl_MouseMove(object sender, MouseEventArgs e)
	{
		Control childAtPoint = nonCustomizableLayoutControl.GetChildAtPoint(e.Location);
		if (!manageRulesButton.Enabled)
		{
			if (childAtPoint == manageRulesButton)
			{
				ToolTipControllerShowEventArgs eShow = new ToolTipControllerShowEventArgs
				{
					SuperTip = manageRulesButton.SuperTip,
					ToolTipType = ToolTipType.SuperTip
				};
				toolTipController.ShowHint(eShow, Control.MousePosition);
			}
			else
			{
				toolTipController.HideHint();
			}
		}
	}

	private void RepositoryItemComboBox1_QueryPopUp(object sender, CancelEventArgs e)
	{
		SetColumnsEditItems(sender as ComboBoxEdit, 1);
	}

	private void RepositoryItemComboBox2_QueryPopUp(object sender, CancelEventArgs e)
	{
		SetColumnsEditItems(sender as ComboBoxEdit, 2);
	}

	private void RepositoryItemComboBox3_QueryPopUp(object sender, CancelEventArgs e)
	{
		SetColumnsEditItems(sender as ComboBoxEdit, 3);
	}

	private void RepositoryItemComboBox4_QueryPopUp(object sender, CancelEventArgs e)
	{
		SetColumnsEditItems(sender as ComboBoxEdit, 4);
	}

	private void RepositoryItemComboBox5_QueryPopUp(object sender, CancelEventArgs e)
	{
		SetColumnsEditItems(sender as ComboBoxEdit, 5);
	}

	private void SetColumnsEditItems(ComboBoxEdit comboBoxEdit, int i)
	{
		comboBoxEdit?.Properties?.Items?.Clear();
		if (comboBoxEdit == null)
		{
			return;
		}
		RepositoryItemComboBox properties = comboBoxEdit.Properties;
		if (properties != null)
		{
			ComboBoxItemCollection items = properties.Items;
			if (items != null)
			{
				object[] items2 = (GetColumnByFieldNumber(i)?.Tag as ClassificatorCustomFieldRow)?.AllLabels?.ToArray();
				items.AddRange(items2);
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.manageRulesButton = new DevExpress.XtraEditors.SimpleButton();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.maskNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.maskPatternsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.customField1GridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.customField2GridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemComboBox2 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.customField3GridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemComboBox3 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.customField4GridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemComboBox4 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.customField5GridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemComboBox5 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.manageRulesButtonLlayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.manageRulesButtonLlayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.manageRulesButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.gridControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(896, 11, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(547, 276);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl.MouseMove += new System.Windows.Forms.MouseEventHandler(NonCustomizableLayoutControl_MouseMove);
		this.manageRulesButton.AllowFocus = false;
		this.manageRulesButton.Location = new System.Drawing.Point(409, 248);
		this.manageRulesButton.Name = "manageRulesButton";
		this.manageRulesButton.Size = new System.Drawing.Size(126, 22);
		this.manageRulesButton.StyleController = this.nonCustomizableLayoutControl;
		this.manageRulesButton.TabIndex = 5;
		this.manageRulesButton.Text = "Manage Rules";
		this.manageRulesButton.Click += new System.EventHandler(ManageRulesButton_Click);
		this.gridControl.Location = new System.Drawing.Point(12, 28);
		this.gridControl.MainView = this.gridView;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemComboBox1, this.repositoryItemComboBox2, this.repositoryItemComboBox3, this.repositoryItemComboBox4, this.repositoryItemComboBox5 });
		this.gridControl.Size = new System.Drawing.Size(523, 212);
		this.gridControl.TabIndex = 4;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView });
		this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.maskNameGridColumn, this.maskPatternsGridColumn, this.customField1GridColumn, this.customField2GridColumn, this.customField3GridColumn, this.customField4GridColumn, this.customField5GridColumn });
		this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.gridView.GridControl = this.gridControl;
		this.gridView.Name = "gridView";
		this.gridView.OptionsCustomization.AllowColumnMoving = false;
		this.gridView.OptionsCustomization.AllowFilter = false;
		this.gridView.OptionsCustomization.AllowGroup = false;
		this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.gridView.OptionsFind.AllowFindPanel = false;
		this.gridView.OptionsMenu.EnableFooterMenu = false;
		this.gridView.OptionsMenu.EnableGroupPanelMenu = false;
		this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
		this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gridView.OptionsView.ShowGroupPanel = false;
		this.gridView.OptionsView.ShowIndicator = false;
		this.gridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
		this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(GridView_PopupMenuShowing);
		this.gridView.ShownEditor += new System.EventHandler(GridView_ShownEditor);
		this.gridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(GridView_CellValueChanged);
		this.maskNameGridColumn.Caption = "Mask";
		this.maskNameGridColumn.FieldName = "Mask.MaskName";
		this.maskNameGridColumn.Name = "maskNameGridColumn";
		this.maskNameGridColumn.OptionsColumn.AllowEdit = false;
		this.maskNameGridColumn.Visible = true;
		this.maskNameGridColumn.VisibleIndex = 0;
		this.maskPatternsGridColumn.Caption = "Patterns";
		this.maskPatternsGridColumn.FieldName = "MaskPatterns";
		this.maskPatternsGridColumn.Name = "maskPatternsGridColumn";
		this.maskPatternsGridColumn.OptionsColumn.AllowEdit = false;
		this.maskPatternsGridColumn.Visible = true;
		this.maskPatternsGridColumn.VisibleIndex = 1;
		this.customField1GridColumn.Caption = "gridColumn1";
		this.customField1GridColumn.ColumnEdit = this.repositoryItemComboBox1;
		this.customField1GridColumn.FieldName = "CustomField_1_Value";
		this.customField1GridColumn.Name = "customField1GridColumn";
		this.customField1GridColumn.Visible = true;
		this.customField1GridColumn.VisibleIndex = 2;
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.repositoryItemComboBox1.QueryPopUp += new System.ComponentModel.CancelEventHandler(RepositoryItemComboBox1_QueryPopUp);
		this.customField2GridColumn.Caption = "gridColumn2";
		this.customField2GridColumn.ColumnEdit = this.repositoryItemComboBox2;
		this.customField2GridColumn.FieldName = "CustomField_2_Value";
		this.customField2GridColumn.Name = "customField2GridColumn";
		this.customField2GridColumn.Visible = true;
		this.customField2GridColumn.VisibleIndex = 3;
		this.repositoryItemComboBox2.AutoHeight = false;
		this.repositoryItemComboBox2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox2.Name = "repositoryItemComboBox2";
		this.repositoryItemComboBox2.QueryPopUp += new System.ComponentModel.CancelEventHandler(RepositoryItemComboBox2_QueryPopUp);
		this.customField3GridColumn.Caption = "gridColumn3";
		this.customField3GridColumn.ColumnEdit = this.repositoryItemComboBox3;
		this.customField3GridColumn.FieldName = "CustomField_3_Value";
		this.customField3GridColumn.Name = "customField3GridColumn";
		this.customField3GridColumn.Visible = true;
		this.customField3GridColumn.VisibleIndex = 4;
		this.repositoryItemComboBox3.AutoHeight = false;
		this.repositoryItemComboBox3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox3.Name = "repositoryItemComboBox3";
		this.repositoryItemComboBox3.QueryPopUp += new System.ComponentModel.CancelEventHandler(RepositoryItemComboBox3_QueryPopUp);
		this.customField4GridColumn.Caption = "gridColumn4";
		this.customField4GridColumn.ColumnEdit = this.repositoryItemComboBox4;
		this.customField4GridColumn.FieldName = "CustomField_4_Value";
		this.customField4GridColumn.Name = "customField4GridColumn";
		this.customField4GridColumn.Visible = true;
		this.customField4GridColumn.VisibleIndex = 5;
		this.repositoryItemComboBox4.AutoHeight = false;
		this.repositoryItemComboBox4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox4.Name = "repositoryItemComboBox4";
		this.repositoryItemComboBox4.QueryPopUp += new System.ComponentModel.CancelEventHandler(RepositoryItemComboBox4_QueryPopUp);
		this.customField5GridColumn.Caption = "gridColumn5";
		this.customField5GridColumn.ColumnEdit = this.repositoryItemComboBox5;
		this.customField5GridColumn.FieldName = "CustomField_5_Value";
		this.customField5GridColumn.Name = "customField5GridColumn";
		this.customField5GridColumn.Visible = true;
		this.customField5GridColumn.VisibleIndex = 6;
		this.repositoryItemComboBox5.AutoHeight = false;
		this.repositoryItemComboBox5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox5.Name = "repositoryItemComboBox5";
		this.repositoryItemComboBox5.QueryPopUp += new System.ComponentModel.CancelEventHandler(RepositoryItemComboBox5_QueryPopUp);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem, this.manageRulesButtonLlayoutControlItem, this.emptySpaceItem1 });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 0);
		this.Root.Size = new System.Drawing.Size(547, 276);
		this.Root.TextVisible = false;
		this.layoutControlItem.Control = this.gridControl;
		this.layoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem.Name = "layoutControlItem";
		this.layoutControlItem.Size = new System.Drawing.Size(527, 232);
		this.layoutControlItem.Text = "Classification Rules:";
		this.layoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem.TextSize = new System.Drawing.Size(94, 13);
		this.manageRulesButtonLlayoutControlItem.Control = this.manageRulesButton;
		this.manageRulesButtonLlayoutControlItem.Location = new System.Drawing.Point(397, 232);
		this.manageRulesButtonLlayoutControlItem.MaxSize = new System.Drawing.Size(130, 34);
		this.manageRulesButtonLlayoutControlItem.MinSize = new System.Drawing.Size(130, 34);
		this.manageRulesButtonLlayoutControlItem.Name = "manageRulesButtonLlayoutControlItem";
		this.manageRulesButtonLlayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 6, 6);
		this.manageRulesButtonLlayoutControlItem.Size = new System.Drawing.Size(130, 34);
		this.manageRulesButtonLlayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.manageRulesButtonLlayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.manageRulesButtonLlayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 232);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(397, 34);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Name = "ClassificationRulesUserControl";
		base.Size = new System.Drawing.Size(547, 276);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.manageRulesButtonLlayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		base.ResumeLayout(false);
	}
}
