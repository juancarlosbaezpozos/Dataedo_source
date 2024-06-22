using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Enums;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.PanelControls.Appearance;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Synchronization_Tools;

public class DocObjectsManagement
{
	private DatabaseRow database;

	private SynchStateAppearance synchStateAppearance;

	private GridView synchronizeGridView;

	private GridControl synchronizeGrid;

	private LabelControl selectedObjectCounterlabel;

	private LabelControl newObjectCounterlabel;

	private LabelControl updatedObjectCounterlabel;

	private LayoutControlItem checkAllLayoutControlItem;

	private HyperLinkEdit checkAllHyperLinkEdit;

	private LayoutControlItem checkNoneLayoutControlItem;

	private HyperLinkEdit checkNoneHyperLinkEdit;

	private EmptySpaceItem checkEmptySpaceItem;

	private RepositoryItemCheckEdit synchronizeTableRepositoryItemCheckEdit;

	private GridColumn synchronizeObjectGridColumn;

	private LayoutControlItem selectObjectsLayoutControlItem;

	private int _selectedObjectsCounter;

	private int _newObjectsCounter;

	private int _updatedObjectsCounter;

	private int SelectedObjectsCounter
	{
		get
		{
			return _selectedObjectsCounter;
		}
		set
		{
			_selectedObjectsCounter = value;
			database.SelectedObjectsCount = _selectedObjectsCounter;
			selectedObjectCounterlabel.Text = "Objects that will be added to repository: " + value;
			int num = RefreshToSynchCounterLabel();
			SetSynchButtonAvailability(_selectedObjectsCounter != 0 || num != 0 || _updatedObjectsCounter != 0);
		}
	}

	public int IgnoredNewObjectsCounter
	{
		get
		{
			return _newObjectsCounter;
		}
		set
		{
			_newObjectsCounter = value;
			newObjectCounterlabel.Text = "New objects that will be ignored: " + value;
		}
	}

	public int UpdatedObjectsCounter
	{
		get
		{
			return _updatedObjectsCounter;
		}
		set
		{
			_updatedObjectsCounter = value;
			updatedObjectCounterlabel.Text = "Objects that will be updated in repository: " + value;
			int num = RefreshToSynchCounterLabel();
			SetSynchButtonAvailability(_selectedObjectsCounter != 0 || num != 0 || _updatedObjectsCounter != 0);
		}
	}

	public bool IsFilterPopupMenuShown { get; set; }

	public event EventHandler UpdateSynchRequiredButtons;

	public DocObjectsManagement(InfoUserControl infoUserControl, GridView synchronizeGridView, LabelControl selectedObjectCounterlabel, LabelControl newObjectCounterlabel, LabelControl updatedObjectCounterlabel, LayoutControlItem checkAllLayoutControlItem, LayoutControlItem checkNoneLayoutControlItem, EmptySpaceItem checkEmptySpaceItem, RepositoryItemCheckEdit synchronizeTableRepositoryItemCheckEdit, HyperLinkEdit checkAllHyperLinkEdit, HyperLinkEdit checkNoneHyperLinkEdit, GridColumn synchronizeObjectGridColumn, LayoutControlItem selectObjectsLayoutControlItem)
	{
		synchStateAppearance = new SynchStateAppearance(infoUserControl);
		this.synchronizeGridView = synchronizeGridView;
		synchronizeGrid = this.synchronizeGridView.GridControl;
		this.synchronizeGridView.PopupMenuShowing += synchronizeGridView_PopupMenuShowing;
		this.synchronizeGridView.RowCellStyle += synchronizeGridView_RowCellStyle;
		this.synchronizeGridView.CustomDrawCell += synchronizeGridView_CustomDrawCell;
		this.synchronizeGridView.ShowingEditor += synchronizeGridView_ShowingEditor;
		this.synchronizeGridView.PopupMenuShowing += delegate(object s, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
		{
			GridView obj = s as GridView;
			if (obj.IsFilterRow(obj.FocusedRowHandle))
			{
				IsFilterPopupMenuShown = true;
			}
		};
		this.selectedObjectCounterlabel = selectedObjectCounterlabel;
		this.newObjectCounterlabel = newObjectCounterlabel;
		this.updatedObjectCounterlabel = updatedObjectCounterlabel;
		this.checkAllLayoutControlItem = checkAllLayoutControlItem;
		this.checkNoneLayoutControlItem = checkNoneLayoutControlItem;
		this.checkEmptySpaceItem = checkEmptySpaceItem;
		this.checkAllHyperLinkEdit = checkAllHyperLinkEdit;
		this.checkAllHyperLinkEdit.OpenLink += checkAllHyperLinkEdit_OpenLink;
		this.checkNoneHyperLinkEdit = checkNoneHyperLinkEdit;
		this.checkNoneHyperLinkEdit.OpenLink += checkNoneHyperLinkEdit_OpenLink;
		this.synchronizeTableRepositoryItemCheckEdit = synchronizeTableRepositoryItemCheckEdit;
		this.synchronizeTableRepositoryItemCheckEdit.EditValueChanged += synchronizeTableRepositoryItemCheckEdit_EditValueChanged;
		this.synchronizeObjectGridColumn = synchronizeObjectGridColumn;
		this.selectObjectsLayoutControlItem = selectObjectsLayoutControlItem;
	}

	private void synchronizeGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (synchronizeGridView.GetFocusedRow() is ObjectRow objectRow)
		{
			bool flag2 = (e.Cancel = objectRow.SynchronizeState != SynchronizeStateEnum.SynchronizeState.New && objectRow.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Ignored);
		}
	}

	public void SetDatabase(DatabaseRow database)
	{
		this.database = database;
	}

	private IEnumerable<ObjectRow> GetFilteredGridDataSource(IEnumerable<ObjectRow> source)
	{
		GridView gridView = synchronizeGridView;
		if (gridView == null)
		{
			return new List<ObjectRow>();
		}
		if (gridView.ActiveFilter == null || !gridView.ActiveFilterEnabled || gridView.ActiveFilter.Expression == "")
		{
			return source;
		}
		CriteriaOperator op = StringsTolowerCloningHelper.Process(gridView.ActiveFilterCriteria);
		return source.AsQueryable().AppendWhere(new CriteriaToExpressionConverter(), op) as IEnumerable<ObjectRow>;
	}

	private void checkAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		IEnumerable<ObjectRow> source = synchronizeGrid.DataSource as IEnumerable<ObjectRow>;
		GetFilteredGridDataSource(source).ToList().ForEach(delegate(ObjectRow x)
		{
			x.ToSynchronize = true;
		});
		synchronizeGrid.RefreshDataSource();
		SelectedObjectsCounter = source.Where((ObjectRow x) => x.ToSynchronize && (x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored)).Count();
		IgnoredNewObjectsCounter = source.Where((ObjectRow x) => !x.ToSynchronize && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New).Count();
		UpdatedObjectsCounter = source.Where((ObjectRow x) => x.ToSynchronize && (x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)).Count();
	}

	private void checkNoneHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		IEnumerable<ObjectRow> source = synchronizeGrid.DataSource as IEnumerable<ObjectRow>;
		(from x in GetFilteredGridDataSource(source)
			where x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored
			select x).ToList().ForEach(delegate(ObjectRow x)
		{
			x.ToSynchronize = false;
		});
		synchronizeGrid.RefreshDataSource();
		SelectedObjectsCounter = source.Where((ObjectRow x) => x.ToSynchronize && (x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored)).Count();
		IgnoredNewObjectsCounter = source.Where((ObjectRow x) => !x.ToSynchronize && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New).Count();
		UpdatedObjectsCounter = source.Where((ObjectRow x) => x.ToSynchronize && (x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)).Count();
	}

	private void synchronizeGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if ((e.Column.FieldName.Contains("image") || e.Column.Equals(synchronizeObjectGridColumn)) && e.RowHandle == -2147483646)
		{
			e.Handled = true;
		}
	}

	private void synchronizeTableRepositoryItemCheckEdit_EditValueChanged(object sender, EventArgs e)
	{
		synchronizeGridView.CloseEditor();
		ObjectRow objectRow = synchronizeGridView.GetFocusedRow() as ObjectRow;
		int num = (objectRow.ToSynchronize ? 1 : (-1));
		if (objectRow.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || objectRow.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			UpdatedObjectsCounter += num;
			return;
		}
		if (objectRow.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New)
		{
			IgnoredNewObjectsCounter -= num;
		}
		SelectedObjectsCounter += num;
	}

	private void synchronizeGridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (!(synchronizeGridView.GetRow(e.RowHandle) is ObjectRow objectRow))
		{
			return;
		}
		if (!objectRow.ToSynchronize)
		{
			e.Appearance.ForeColor = Color.Gray;
			return;
		}
		switch (objectRow.SynchronizeState)
		{
		case SynchronizeStateEnum.SynchronizeState.New:
			e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			break;
		case SynchronizeStateEnum.SynchronizeState.Unsynchronized:
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			break;
		case SynchronizeStateEnum.SynchronizeState.Deleted:
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Strikeout);
			break;
		}
	}

	private void synchronizeGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private int AppendDescriptionLabel(StringBuilder synchCounterStringBuilder, SynchronizeStateEnum.SynchronizeState synchronizeState, string labelBefore, string labelBeforeObject, string labelAfter, bool includelabelIsAre = false)
	{
		int num = database.tableRows.Where((ObjectRow x) => x.SynchronizeState == synchronizeState).Count();
		if (num > 0)
		{
			if (!string.IsNullOrEmpty(synchCounterStringBuilder.ToString()))
			{
				synchCounterStringBuilder.Append("<br>");
			}
			string value = ((num == 1) ? " object " : " objects ");
			string value2 = null;
			if (includelabelIsAre)
			{
				value2 = ((num == 1) ? "is " : "are ");
			}
			synchCounterStringBuilder.Append(labelBefore).Append(value2).Append(num)
				.Append(labelBeforeObject)
				.Append(value)
				.Append(labelAfter);
		}
		return num;
	}

	private void SetSynchNotRequiredLabel(StringBuilder synchCounterStringBuilder)
	{
		string text = synchCounterStringBuilder.ToString();
		if (text.Length > 4 && text.Substring(text.Length - 4) != "<br>")
		{
			synchCounterStringBuilder.Append("<br>");
		}
		synchCounterStringBuilder.Append("<b>Import not required</b>");
	}

	private int RefreshToSynchCounterLabel()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		if (database.tableRows != null && database.tableRows.Any())
		{
			AppendDescriptionLabel(stringBuilder, SynchronizeStateEnum.SynchronizeState.New, "There ", (database.ConnectAndSynchronizeState == SynchConnectStateEnum.SynchConnectStateType.New) ? null : " new", "in the database. If you don't want to include some of them in the documentation just uncheck them.", includelabelIsAre: true);
			AppendDescriptionLabel(stringBuilder, SynchronizeStateEnum.SynchronizeState.Unsynchronized, null, null, "have been updated in the database");
			AppendDescriptionLabel(stringBuilder, SynchronizeStateEnum.SynchronizeState.Deleted, null, null, "have been removed from database.");
			num += database.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized && x.ToSynchronize).Count();
			num += database.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted && x.ToSynchronize).Count();
			AppendDescriptionLabel(stringBuilder, SynchronizeStateEnum.SynchronizeState.Ignored, "There ", null, "previously ignored from documentation. Check objects you would like to include again", includelabelIsAre: true);
			num += SelectedObjectsCounter;
			if (num == 0)
			{
				SetSynchNotRequiredLabel(stringBuilder);
			}
		}
		else
		{
			SetSynchNotRequiredLabel(stringBuilder);
		}
		if (database.ConnectAndSynchronizeState == SynchConnectStateEnum.SynchConnectStateType.Synchronize)
		{
			synchStateAppearance.SetAppearance(SynchStateAppearance.SynchState.Synchronized, (num == 0) ? null : stringBuilder.ToString());
		}
		else if (database.ConnectAndSynchronizeState == SynchConnectStateEnum.SynchConnectStateType.Error)
		{
			synchStateAppearance.SetAppearance(SynchStateAppearance.SynchState.Failed, (num == 0) ? null : stringBuilder.ToString());
		}
		else
		{
			synchStateAppearance.SetAppearance((num == 0) ? SynchStateAppearance.SynchState.SynchronizationNotRequired : SynchStateAppearance.SynchState.Synchronize, stringBuilder.ToString());
		}
		return num;
	}

	public void SetSynchronizedObjectWithCounter()
	{
		SetCheckButtonsVisibility(visible: true);
		if (database.tableRows != null && database.tableRows.Any())
		{
			IEnumerable<ObjectRow> enumerable = database.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted);
			synchronizeGrid.DataSource = enumerable;
			selectObjectsLayoutControlItem.Visibility = ((enumerable.Count() <= 0) ? LayoutVisibility.Never : LayoutVisibility.Always);
			SelectedObjectsCounter = enumerable.Where((ObjectRow x) => x.ToSynchronize && (x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored)).Count();
			IgnoredNewObjectsCounter = 0;
			UpdatedObjectsCounter = enumerable.Where((ObjectRow x) => x.ToSynchronize && (x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)).Count();
		}
		else
		{
			synchronizeGrid.DataSource = database.tableRows;
			SelectedObjectsCounter = 0;
			IgnoredNewObjectsCounter = 0;
			UpdatedObjectsCounter = 0;
		}
		RefreshToSynchCounterLabel();
		synchronizeGrid.Refresh();
		CommonFunctionsPanels.SetBestFitForColumns(synchronizeGridView);
	}

	private void SetCheckButtonsVisibility(bool visible)
	{
		if (visible)
		{
			LayoutControlItem layoutControlItem = checkAllLayoutControlItem;
			LayoutControlItem layoutControlItem2 = checkNoneLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (checkEmptySpaceItem.Visibility = LayoutVisibility.Always);
			LayoutVisibility layoutVisibility5 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility2));
		}
		else
		{
			LayoutControlItem layoutControlItem3 = checkAllLayoutControlItem;
			LayoutControlItem layoutControlItem4 = checkNoneLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (checkEmptySpaceItem.Visibility = LayoutVisibility.Never);
			LayoutVisibility layoutVisibility5 = (layoutControlItem3.Visibility = (layoutControlItem4.Visibility = layoutVisibility2));
		}
	}

	private void SetSynchButtonAvailability(bool isAvailable)
	{
		this.UpdateSynchRequiredButtons?.Invoke(null, new ObjectTypeEventArgs(isAvailable));
	}
}
