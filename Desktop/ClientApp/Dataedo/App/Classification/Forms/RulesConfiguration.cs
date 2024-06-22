using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Enums;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Classification.Forms;

public class RulesConfiguration : BaseXtraForm
{
	private ClassificatorModelRow classificatorModelRow;

	private int? classificatorId;

	private List<ClassificationMaskRow> classificationsMasks;

	private List<ClassificatorModel> classificators;

	private IContainer components;

	private RibbonControl ribbonControl;

	private BarButtonItem addMaskBarButtonItem;

	private BarButtonItem removeMaskBarButtonItem;

	private RibbonPage ribbonPage;

	private RibbonPageGroup masksPageGroup;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private SimpleButton cancelButton;

	private LayoutControlItem cancelButtonLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private SimpleButton saveButton;

	private LayoutControlItem saveButtonLayoutControlItem;

	private SimpleSeparator bottomSeparator;

	private BarButtonItem editMaskButtonItem;

	private GridControl gridControl;

	private GridView gridView;

	private LayoutControlItem gridControlLayoutControlItem;

	private GridColumn maskNameGridColumn;

	private GridColumn patternsGridColumn;

	private SimpleButton saveAndCloseButton;

	private LayoutControlItem saveAndCloseButtonLayoutControlItem;

	private GridPanelUserControl gridPanelUserControl;

	private LayoutControlItem gridPanelLayoutControlItem;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.ExStyle |= 33554432;
			return obj;
		}
	}

	public RulesConfiguration()
	{
		InitializeComponent();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			Save(showInfo: true);
			break;
		case Keys.Escape:
			if (!gridView.IsEditorFocused)
			{
				Close();
			}
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public void SetParameters(int? classificatorId, ClassificatorModelRow classificatorModelRow = null)
	{
		this.classificatorId = classificatorId;
		this.classificatorModelRow = classificatorModelRow;
		PrepareClassifcatorsList();
		PrepareColumns();
		SetDataSource();
		maskNameGridColumn.SortOrder = ColumnSortOrder.Ascending;
		gridControl.DataSource = classificationsMasks;
		gridView.ActiveFilter.NonColumnFilter = new BinaryOperator("RowState", ManagingRowsEnum.ManagingRows.Deleted, BinaryOperatorType.NotEqual).ToString();
		SetRibbonButtons();
		gridPanelUserControl.Initialize(Text);
		gridPanelUserControl.HideButtons();
	}

	private void PrepareClassifcatorsList()
	{
		classificators = (from x in DB.Classificator.GetClassificators()
			orderby x.Title
			select x).ToList();
		if (classificatorId.HasValue)
		{
			ClassificatorModel item = classificators.Where((ClassificatorModel x) => x.Id == classificatorId).First();
			classificators.Remove(item);
			classificators.Insert(0, item);
		}
	}

	private void PrepareColumns()
	{
		foreach (ClassificatorModel classificator in classificators)
		{
			GridColumn gridColumn = new GridColumn
			{
				Caption = classificator.Title,
				FieldName = $"classificator_{classificator.Id}",
				Tag = classificator.Id,
				UnboundType = UnboundColumnType.Boolean,
				Visible = true
			};
			if (classificatorId.HasValue)
			{
				gridColumn.OptionsColumn.AllowEdit = classificator.Id == classificatorId;
			}
			gridView.Columns.Add(gridColumn);
		}
	}

	private void SetDataSource()
	{
		classificationsMasks = DB.Classificator.GetMasksFromRepositoryWithoutPresenceData();
		List<Dataedo.Model.Data.Classificator.ClassificationMaskPresence> classificationMasksPresence = DB.Classificator.GetClassificationMasksPresence();
		foreach (ClassificationMaskRow i in classificationsMasks)
		{
			i.SetClassificatorsInPresence(classificators, classificatorId);
			IEnumerable<Dataedo.Model.Data.Classificator.ClassificationMaskPresence> enumerable = classificationMasksPresence.Where((Dataedo.Model.Data.Classificator.ClassificationMaskPresence x) => x.MaskName == i.MaskName);
			if (!enumerable.Any())
			{
				continue;
			}
			foreach (Dataedo.Model.Data.Classificator.ClassificationMaskPresence p in enumerable)
			{
				i.PresenceInClassificators.Where((Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x) => x.Classificator.Id == p.ClassificatorId).ForEach(delegate(Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x)
				{
					x.IsPresent = true;
				});
			}
		}
	}

	private void AddMaskBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			using EditMask editMask = new EditMask();
			editMask.SetParameters("New mask", classificatorId, addingNewMask: true);
			editMask.ShowDialog(this);
			if (!string.IsNullOrEmpty(editMask.LastSavedMaskName))
			{
				ClassificationMaskRow maskFromRepository = DB.Classificator.GetMaskFromRepository(editMask.LastSavedMaskName, classificatorId);
				if (maskFromRepository != null)
				{
					classificationsMasks.Add(maskFromRepository);
					gridControl.RefreshDataSource();
					gridView.FocusedRowHandle = gridView.FindRow(maskFromRepository);
					SetRibbonButtons();
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding classification mask", FindForm());
		}
	}

	private void EditMaskBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			gridView.PostEditor();
			if (!(gridView.GetFocusedRow() is ClassificationMaskRow classificationMaskRow))
			{
				return;
			}
			using EditMask editMask = new EditMask();
			editMask.SetParameters(classificationMaskRow.MaskName, classificatorId, addingNewMask: false, classificatorModelRow);
			editMask.ShowDialog(this);
			classificationsMasks.Remove(classificationMaskRow);
			ClassificationMaskRow maskFromRepository = DB.Classificator.GetMaskFromRepository(editMask.LastSavedMaskName, classificatorId);
			if (maskFromRepository != null)
			{
				classificationsMasks.Add(maskFromRepository);
				gridControl.RefreshDataSource();
				gridView.FocusedRowHandle = gridView.FindRow(maskFromRepository);
				SetRibbonButtons();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while editing classification mask", FindForm());
		}
	}

	private void RemoveMaskBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (gridView.GetFocusedRow() is ClassificationMaskRow classificationMaskRow)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Are you sure you want to delete the Mask <b>" + classificationMaskRow.MaskName + "</b>?", "Delete Classification Mask", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, this);
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				classificationMaskRow.RowState = ManagingRowsEnum.ManagingRows.Deleted;
				gridControl.RefreshDataSource();
				SetRibbonButtons();
			}
		}
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		Save(showInfo: true);
	}

	private void SaveAndCloseButton_Click(object sender, EventArgs e)
	{
		Save();
		Close();
	}

	private void CancelButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void GridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		if (!int.TryParse(e.Column.Tag?.ToString(), out var classificatorId) || !(e.Row is ClassificationMaskRow classificationMaskRow))
		{
			return;
		}
		Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence classificationMaskPresence = classificationMaskRow.PresenceInClassificators.Where((Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x) => x.Classificator.Id == classificatorId).FirstOrDefault();
		if (classificationMaskPresence != null)
		{
			if (e.IsSetData)
			{
				classificationMaskPresence.IsPresent = (bool)e.Value;
				classificationMaskPresence.IsChanged = true;
				classificationMaskRow.SetUpdatedIfNotAdded();
			}
			else if (e.IsGetData)
			{
				e.Value = classificationMaskPresence.IsPresent;
			}
		}
	}

	private void GridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		GridViewHelpers.GrayOutNoneditableColumns(sender as GridView, e);
	}

	private void SetRibbonButtons()
	{
		BarButtonItem barButtonItem = removeMaskBarButtonItem;
		bool enabled = (editMaskButtonItem.Enabled = gridView.RowCount != 0);
		barButtonItem.Enabled = enabled;
	}

	private void Save(bool showInfo = false)
	{
		try
		{
			if (!gridView.PostEditor())
			{
				return;
			}
			if (!AnyChangesMade())
			{
				if (showInfo)
				{
					GeneralMessageBoxesHandling.Show("There are no changes to save.", "No changes made", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
				}
				return;
			}
			IEnumerable<ClassificationMaskRow> masksToDelete = classificationsMasks.Where((ClassificationMaskRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Deleted);
			IEnumerable<ClassificationMaskRow> enumerable = classificationsMasks.Where((ClassificationMaskRow x) => x.AnyChangesMade());
			DB.Classificator.SaveClassificationMasksPresence(masksToDelete, enumerable);
			classificationsMasks.RemoveAll((ClassificationMaskRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Deleted);
			enumerable.ForEach(delegate(ClassificationMaskRow x)
			{
				x.SetUnchanged();
			});
			if (showInfo)
			{
				GeneralMessageBoxesHandling.Show("Changes were successfully saved to the repository.", "Changes saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while saving rules configuration", FindForm());
		}
	}

	private bool AnyChangesMade()
	{
		return classificationsMasks.Where((ClassificationMaskRow x) => x.AnyChangesMade()).Any();
	}

	private void RulesConfiguration_FormClosing(object sender, FormClosingEventArgs e)
	{
		gridView.PostEditor();
		if (AnyChangesMade())
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("There were some changes made. Are you sure you want to cancel?", "Cancel changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, this);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
	}

	private void GridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		_ = e.Menu;
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
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.addMaskBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeMaskBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.editMaskButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.masksPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.maskNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.patternsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveAndCloseButton = new DevExpress.XtraEditors.SimpleButton();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.cancelButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.saveButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomSeparator = new DevExpress.XtraLayout.SimpleSeparator();
		this.gridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveAndCloseButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.gridPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAndCloseButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridPanelLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[5]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.addMaskBarButtonItem,
			this.removeMaskBarButtonItem,
			this.editMaskButtonItem
		});
		this.ribbonControl.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.ribbonControl.MaxItemId = 17;
		this.ribbonControl.Name = "ribbonControl";
		this.ribbonControl.OptionsPageCategories.ShowCaptions = false;
		this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage });
		this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.ribbonControl.ShowToolbarCustomizeItem = false;
		this.ribbonControl.Size = new System.Drawing.Size(977, 102);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.addMaskBarButtonItem.Caption = "Add";
		this.addMaskBarButtonItem.Id = 12;
		this.addMaskBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.addMaskBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.add_32;
		this.addMaskBarButtonItem.Name = "addMaskBarButtonItem";
		this.addMaskBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddMaskBarButtonItem_ItemClick);
		this.removeMaskBarButtonItem.Caption = "Remove";
		this.removeMaskBarButtonItem.Id = 13;
		this.removeMaskBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeMaskBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeMaskBarButtonItem.Name = "removeMaskBarButtonItem";
		this.removeMaskBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemoveMaskBarButtonItem_ItemClick);
		this.editMaskButtonItem.Caption = "Edit";
		this.editMaskButtonItem.Id = 16;
		this.editMaskButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.editMaskButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
		this.editMaskButtonItem.Name = "editMaskButtonItem";
		this.editMaskButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(EditMaskBarButtonItem_ItemClick);
		this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.masksPageGroup });
		this.ribbonPage.Name = "ribbonPage";
		this.ribbonPage.Text = "ribbonPage";
		this.masksPageGroup.AllowTextClipping = false;
		this.masksPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.masksPageGroup.ItemLinks.Add(this.addMaskBarButtonItem);
		this.masksPageGroup.ItemLinks.Add(this.editMaskButtonItem);
		this.masksPageGroup.ItemLinks.Add(this.removeMaskBarButtonItem);
		this.masksPageGroup.Name = "masksPageGroup";
		this.masksPageGroup.Text = "Masks";
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.gridPanelUserControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.gridControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.cancelButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.saveButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.saveAndCloseButton);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 102);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1150, 216, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(977, 350);
		this.nonCustomizableLayoutControl.TabIndex = 2;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.gridPanelUserControl.GridView = this.gridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(15, 2);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(950, 28);
		this.gridPanelUserControl.TabIndex = 10;
		this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.maskNameGridColumn, this.patternsGridColumn });
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
		this.gridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(GridView_CustomUnboundColumnData);
		this.maskNameGridColumn.Caption = "Mask";
		this.maskNameGridColumn.FieldName = "MaskName";
		this.maskNameGridColumn.Name = "maskNameGridColumn";
		this.maskNameGridColumn.OptionsColumn.AllowEdit = false;
		this.maskNameGridColumn.Visible = true;
		this.maskNameGridColumn.VisibleIndex = 0;
		this.patternsGridColumn.Caption = "Patterns";
		this.patternsGridColumn.FieldName = "AllPatterns";
		this.patternsGridColumn.Name = "patternsGridColumn";
		this.patternsGridColumn.OptionsColumn.AllowEdit = false;
		this.patternsGridColumn.Visible = true;
		this.patternsGridColumn.VisibleIndex = 1;
		this.gridControl.Location = new System.Drawing.Point(15, 32);
		this.gridControl.MainView = this.gridView;
		this.gridControl.MenuManager = this.ribbonControl;
		this.gridControl.Name = "gridControl";
		this.gridControl.Size = new System.Drawing.Size(947, 277);
		this.gridControl.TabIndex = 9;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView });
		this.cancelButton.AllowFocus = false;
		this.cancelButton.Location = new System.Drawing.Point(891, 322);
		this.cancelButton.Margin = new System.Windows.Forms.Padding(10);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(72, 22);
		this.cancelButton.StyleController = this.nonCustomizableLayoutControl;
		this.cancelButton.TabIndex = 8;
		this.cancelButton.Text = "Cancel";
		this.cancelButton.Click += new System.EventHandler(CancelButton_Click);
		this.saveButton.AllowFocus = false;
		this.saveButton.Location = new System.Drawing.Point(675, 322);
		this.saveButton.Margin = new System.Windows.Forms.Padding(10);
		this.saveButton.Name = "saveButton";
		this.saveButton.Size = new System.Drawing.Size(72, 22);
		this.saveButton.StyleController = this.nonCustomizableLayoutControl;
		this.saveButton.TabIndex = 8;
		this.saveButton.Text = "Save";
		this.saveButton.Click += new System.EventHandler(SaveButton_Click);
		this.saveAndCloseButton.AllowFocus = false;
		this.saveAndCloseButton.Location = new System.Drawing.Point(761, 322);
		this.saveAndCloseButton.Margin = new System.Windows.Forms.Padding(10);
		this.saveAndCloseButton.Name = "saveAndCloseButton";
		this.saveAndCloseButton.Size = new System.Drawing.Size(116, 22);
		this.saveAndCloseButton.StyleController = this.nonCustomizableLayoutControl;
		this.saveAndCloseButton.TabIndex = 8;
		this.saveAndCloseButton.Text = "Save and Close";
		this.saveAndCloseButton.Click += new System.EventHandler(SaveAndCloseButton_Click);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.cancelButtonLayoutControlItem, this.bottomEmptySpaceItem, this.saveButtonLayoutControlItem, this.bottomSeparator, this.gridControlLayoutControlItem, this.saveAndCloseButtonLayoutControlItem, this.gridPanelLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(977, 350);
		this.Root.TextVisible = false;
		this.cancelButtonLayoutControlItem.Control = this.cancelButton;
		this.cancelButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.cancelButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.cancelButtonLayoutControlItem.Location = new System.Drawing.Point(891, 316);
		this.cancelButtonLayoutControlItem.MaxSize = new System.Drawing.Size(86, 34);
		this.cancelButtonLayoutControlItem.MinSize = new System.Drawing.Size(86, 34);
		this.cancelButtonLayoutControlItem.Name = "cancelButtonLayoutControlItem";
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.cancelButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 14, 6, 6);
		this.cancelButtonLayoutControlItem.Size = new System.Drawing.Size(86, 34);
		this.cancelButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelButtonLayoutControlItem.Text = "runClassificationButtonLayoutControlItem";
		this.cancelButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelButtonLayoutControlItem.TextVisible = false;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 316);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(661, 34);
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveButtonLayoutControlItem.Control = this.saveButton;
		this.saveButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.saveButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.saveButtonLayoutControlItem.Location = new System.Drawing.Point(661, 316);
		this.saveButtonLayoutControlItem.MaxSize = new System.Drawing.Size(100, 34);
		this.saveButtonLayoutControlItem.MinSize = new System.Drawing.Size(100, 34);
		this.saveButtonLayoutControlItem.Name = "saveButtonLayoutControlItem";
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.saveButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(14, 14, 6, 6);
		this.saveButtonLayoutControlItem.Size = new System.Drawing.Size(100, 34);
		this.saveButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveButtonLayoutControlItem.Text = "runClassificationButtonLayoutControlItem";
		this.saveButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveButtonLayoutControlItem.TextVisible = false;
		this.bottomSeparator.AllowHotTrack = false;
		this.bottomSeparator.Location = new System.Drawing.Point(0, 315);
		this.bottomSeparator.Name = "bottomSeparator";
		this.bottomSeparator.Size = new System.Drawing.Size(977, 1);
		this.gridControlLayoutControlItem.Control = this.gridControl;
		this.gridControlLayoutControlItem.Location = new System.Drawing.Point(0, 32);
		this.gridControlLayoutControlItem.Name = "gridControlLayoutControlItem";
		this.gridControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 2, 6);
		this.gridControlLayoutControlItem.Size = new System.Drawing.Size(977, 283);
		this.gridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridControlLayoutControlItem.TextVisible = false;
		this.saveAndCloseButtonLayoutControlItem.Control = this.saveAndCloseButton;
		this.saveAndCloseButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.saveAndCloseButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.saveAndCloseButtonLayoutControlItem.Location = new System.Drawing.Point(761, 316);
		this.saveAndCloseButtonLayoutControlItem.MaxSize = new System.Drawing.Size(130, 34);
		this.saveAndCloseButtonLayoutControlItem.MinSize = new System.Drawing.Size(130, 34);
		this.saveAndCloseButtonLayoutControlItem.Name = "saveAndCloseButtonLayoutControlItem";
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.saveAndCloseButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 14, 6, 6);
		this.saveAndCloseButtonLayoutControlItem.Size = new System.Drawing.Size(130, 34);
		this.saveAndCloseButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveAndCloseButtonLayoutControlItem.Text = "runClassificationButtonLayoutControlItem";
		this.saveAndCloseButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveAndCloseButtonLayoutControlItem.TextVisible = false;
		this.gridPanelLayoutControlItem.Control = this.gridPanelUserControl;
		this.gridPanelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.gridPanelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 32);
		this.gridPanelLayoutControlItem.MinSize = new System.Drawing.Size(194, 32);
		this.gridPanelLayoutControlItem.Name = "gridPanelLayoutControlItem";
		this.gridPanelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 12, 2, 2);
		this.gridPanelLayoutControlItem.Size = new System.Drawing.Size(977, 32);
		this.gridPanelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.gridPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridPanelLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(977, 452);
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Controls.Add(this.ribbonControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.Name = "RulesConfiguration";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Rules Configuration";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(RulesConfiguration_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAndCloseButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridPanelLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
