using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Executing;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraWaitForm;
using Snowflake.Data.Client;

namespace Dataedo.App.Forms;

public class AddDataLinksForm : BaseXtraForm
{
	private MetadataEditorUserControl mainControl;

	private SemaphoreSlim loadingDataMutex = new SemaphoreSlim(1, 1);

	private CancellationTokenSource loadingDataCancellationTokenSource = new CancellationTokenSource();

	private ICommandExtended currentlyRunningLoadingDataQuery;

	private TermObject term;

	private List<ObjectWithDataLinkExtended> currentFilteredObjectsWithDataLink;

	private List<ObjectWithDataLinkExtended> modifiedObjectsWithDataLink;

	private bool isModified;

	private IContainer components;

	private LayoutControlGroup rootLayoutControlGroup;

	private GridControl dataLinksGrid;

	private BulkCopyGridUserControl dataLinksGridView;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private GridColumn objectGridColumn;

	private RepositoryItemCustomTextEdit titleRepositoryItemCustomTextEdit;

	private GridColumn dataTypeTableGridColumn;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private LayoutControlItem termsLayoutControlItem;

	private EmptySpaceItem leftButtonsEmptySpaceItem;

	private SimpleButton saveSimpleButton;

	private LayoutControlItem saveLayoutControlItem;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem cancelLayoutControlItem;

	private EmptySpaceItem beetwenButtonsEmptySpaceItem;

	private EmptySpaceItem bottomAboveButtonsEmptySpaceItem;

	private ToolTipController dataLinksToolTipController;

	private GridColumn isSelectedGridColumn;

	private GridColumn iconGridColumn;

	private GridColumn descriptionGridColumn;

	private RepositoryItemCheckEdit isSelectedRepositoryItemCheckEdit;

	private GridColumn documentationGridColumn;

	private TextEdit nameTextEdit;

	private LayoutControlItem nameLayoutControlItem;

	private Panel progressPanelPanel;

	private PanelControl progressPanelControl;

	private LabelControl progressLabelControl;

	private ProgressPanel progressPanel;

	private LayoutControlGroup progressLayoutControlGroup;

	private LayoutControlItem progressPanelLayoutControlItem;

	private LayoutControlItem progressLabelLayoutControlItem;

	private EmptySpaceItem aboveGridEmptySpaceItem;

	private NonCustomizableLayoutControl rootLayoutControl;

	private NonCustomizableLayoutControl progressLayoutControl;

	private CheckEdit columnFilterCheckEdit;

	private CheckEdit tableViewFilterCheckEdit;

	private LayoutControlItem tableViewFilterLayoutControlItem;

	private LayoutControlItem columnFilterLayoutControlItem;

	private LayoutControlGroup Root;

	private LayoutControlItem errorInformationLayoutControlItem;

	private PictureBox errorIconPictureBox;

	private LayoutControlItem errorIconLayoutControlItem;

	private LabelControl errorTextLabelControl;

	private LayoutControlItem errorTextLayoutControlItem;

	private LabelControl headerLabelControl;

	private PictureBox headerIconPictureBox;

	private LayoutControlGroup headerLayoutControlGroup;

	private LayoutControlItem headerIconLayoutControlItem;

	private LayoutControlItem headerLayoutControlItem;

	private LayoutControlItem headerInformationLayoutControlItem;

	private NonCustomizableLayoutControl errorInformationLayoutControl;

	private NonCustomizableLayoutControl headerLayoutControl;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private SnowflakeDbCommand snowflakeDbCommand1;

	public AddDataLinksForm()
	{
		InitializeComponent();
		AddEvents();
		progressLabelLayoutControlItem.Visibility = LayoutVisibility.Never;
		currentFilteredObjectsWithDataLink = new List<ObjectWithDataLinkExtended>();
		modifiedObjectsWithDataLink = new List<ObjectWithDataLinkExtended>();
		errorInformationLayoutControlItem.Visibility = LayoutVisibility.Never;
		dataLinksGridView.Copy = new BulkCopyWithBlockedIsSelectedCells<ObjectWithDataLinkExtended>();
	}

	private void SetHeaderInformation(string header)
	{
		aboveGridEmptySpaceItem.Visibility = LayoutVisibility.Never;
		headerIconLayoutControlItem.Visibility = LayoutVisibility.Never;
		if (headerLabelControl.IsHandleCreated)
		{
			headerLabelControl.Invoke((Action)delegate
			{
				headerLabelControl.Text = header;
			});
		}
	}

	public void SetParameters(int termIdToAddRelationTo, MetadataEditorUserControl mainControl, CustomFieldsSupport customFieldsSupport)
	{
		this.mainControl = mainControl;
		term = DB.BusinessGlossary.GetTerm(termIdToAddRelationTo);
		if (term == null)
		{
			GeneralMessageBoxesHandling.Show("Term does not exist in repository.", "Term does not exist", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
			base.DialogResult = DialogResult.Abort;
			Close();
			return;
		}
		Text = "'" + term.Title + "' links to Data Dictionary elements";
		SetHeaderInformation("Start typing object name in the field above to search. Objects currently linked with " + term.Title + ":");
		dataLinksGridView.SetRowCellValue(-2147483646, "iconGridColumn", Resources.blank_16);
		if (!base.IsHandleCreated)
		{
			CreateControl();
		}
		isModified = false;
	}

	private void AddDataLinksForm_Shown(object sender, EventArgs e)
	{
		CallLoadingData(loadMappedOnly: true);
	}

	protected void AddEvents()
	{
		List<ToolTipData> toolTipDataList = new List<ToolTipData>
		{
			new ToolTipData(dataLinksGrid, SharedObjectTypeEnum.ObjectType.Term, iconGridColumn.VisibleIndex)
		};
		CommonFunctionsPanels.AddEventsForToolTips(dataLinksToolTipController, toolTipDataList);
		CommonFunctionsPanels.AddEventForAutoFilterRow(dataLinksGridView);
		dataLinksGridView.CustomUnboundColumnData += delegate(object sender, CustomColumnDataEventArgs e)
		{
			if (e.Column.Equals(iconGridColumn))
			{
				ObjectWithDataLink objectWithDataLink = e.Row as ObjectWithDataLink;
				e.Value = IconsSupport.GetObjectIcon(objectWithDataLink.TypeForIcon, objectWithDataLink.SubtypeForIcon, objectWithDataLink.SourceForIcon);
			}
		};
		dataLinksGridView.PopupMenuShowing += delegate(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
		{
			CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		};
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			base.DialogResult = DialogResult.OK;
			Close();
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void TermsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		ObjectWithDataLinkExtended obj = dataLinksGridView.GetRow((sender as GridView).FocusedRowHandle) as ObjectWithDataLinkExtended;
		if (obj != null && !obj.IsInitialized)
		{
			e.Cancel = true;
		}
	}

	private void TermsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.RowHandle != -2147483646)
		{
			ObjectWithDataLinkExtended objectWithDataLinkExtended = dataLinksGridView.GetRow(e.RowHandle) as ObjectWithDataLinkExtended;
			if (objectWithDataLinkExtended.IsInitialized)
			{
				Grids.SetYellowRowColor(sender, e, objectWithDataLinkExtended.IsOutsideFilter || objectWithDataLinkExtended.IsDataChanged);
			}
			else
			{
				Grids.DrawCenteredString(sender, e, objectWithDataLinkExtended.ObjectTitle);
			}
		}
	}

	private void TermsGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void TermsGridView_CustomRowCellEditForAutoFilterRow(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void TermsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		Grids.ShowEditiorOnClick(sender, e, "IsSelected");
	}

	private void TermsGridView_KeyDown(object sender, KeyEventArgs e)
	{
	}

	private void IsSelectedRepositoryItemCheckEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (dataLinksGridView.FocusedRowHandle != -2147483646)
		{
			dataLinksGridView.CloseEditor();
		}
	}

	private void RelationshipTypeRepositoryItemLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		dataLinksGridView.CloseEditor();
		dataLinksGridView.RefreshData();
	}

	private void TermsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (e.RowHandle != -2147483646)
		{
			(dataLinksGridView.GetFocusedRow() as ObjectWithDataLinkExtended).IsModified = true;
			isModified = true;
		}
	}

	private void RelationshipTypeRepositoryItemLookUpEdit_Closed(object sender, ClosedEventArgs e)
	{
		dataLinksGridView.CloseEditor();
	}

	private void SaveSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void CancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void AddDataLinksForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save(e);
		}
		else if (isModified && currentFilteredObjectsWithDataLink.Any((ObjectWithDataLinkExtended x) => x.IsDataChanged))
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Relationships have been changed, would you like to save these changes?", "Relationships have been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				base.DialogResult = DialogResult.OK;
				Save(e);
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void AddDataLinksForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		if (loadingDataCancellationTokenSource != null)
		{
			loadingDataCancellationTokenSource.Cancel();
		}
	}

	private void Save(FormClosingEventArgs e)
	{
		try
		{
			DB.BusinessGlossary.ProcessSavingDataLinks(currentFilteredObjectsWithDataLink.Where((ObjectWithDataLinkExtended x) => x.IsDataChanged));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating data links:", this);
		}
	}

	private void TermsGridView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
	}

	private GridCellInfo GetColumnInfo(GridView view, int rowHandle, GridColumn gridColumn)
	{
		return (view.GetViewInfo() as GridViewInfo)?.GetGridCellInfo(rowHandle, gridColumn);
	}

	private void TableViewFilterCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		CallLoadingData();
	}

	private void ColumnFilterCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		CallLoadingData();
	}

	private void NameTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(nameTextEdit.Text))
		{
			CallLoadingData();
		}
	}

	private void NameTextEdit_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			CallLoadingData();
		}
	}

	private void CallLoadingData(bool loadMappedOnly = false)
	{
		_ = dataLinksGrid.Handle;
		_ = nameTextEdit.Handle;
		loadingDataCancellationTokenSource?.Cancel();
		currentlyRunningLoadingDataQuery?.Cancel();
		CancellationTokenSource currentCancellationTokenSource = new CancellationTokenSource();
		loadingDataCancellationTokenSource = currentCancellationTokenSource;
		new TaskFactory(currentCancellationTokenSource.Token).StartNew(delegate
		{
			loadingDataMutex.Wait();
			if (dataLinksGrid.IsHandleCreated)
			{
				dataLinksGrid.Invoke((Action)delegate
				{
					progressPanelPanel.Visible = true;
					dataLinksGrid.Enabled = false;
					dataLinksGrid.SuspendLayout();
					dataLinksGridView.BeginDataUpdate();
					dataLinksGrid.BeginUpdate();
				});
			}
			modifiedObjectsWithDataLink = GetAllChangedObjects().ToList();
			int maxResultsCount = 1000;
			List<string> list = new List<string>();
			if (tableViewFilterCheckEdit.Checked)
			{
				list.Add("TABLE");
				list.Add("VIEW");
				list.Add("STRUCTURE");
			}
			if (columnFilterCheckEdit.Checked)
			{
				list.Add("TABLE COLUMN");
				list.Add("VIEW COLUMN");
				list.Add("STRUCTURE COLUMN");
			}
			DB.BusinessGlossary.GetObjectsWithDataLinks(term.TermId, nameTextEdit.Text, loadMappedOnly, list, loadMappedOnly ? null : new int?(maxResultsCount + 1), out currentlyRunningLoadingDataQuery).ContinueWith(delegate(Task<List<ObjectWithDataLinkExtended>> results)
			{
				if (results.Result != null)
				{
					currentFilteredObjectsWithDataLink = results.Result;
					currentlyRunningLoadingDataQuery = null;
					ProcessLoadingResults(loadMappedOnly, maxResultsCount, currentCancellationTokenSource);
				}
				if (dataLinksGrid.IsHandleCreated)
				{
					dataLinksGrid.Invoke((Action)delegate
					{
						dataLinksGridView.EndDataUpdate();
						dataLinksGrid.EndUpdate();
						dataLinksGrid.ResumeLayout();
					});
				}
				loadingDataMutex.Release();
			}).ContinueWith(delegate(Task x)
			{
				if (x.Exception != null)
				{
					HandleLoadingError(x.Exception);
					throw x.Exception;
				}
			}, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously)
				.ContinueWith(delegate
				{
					if (loadingDataCancellationTokenSource == currentCancellationTokenSource)
					{
						FinishInterfaceAfterLoading();
					}
				}, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously)
				.ContinueWith(delegate(Task x)
				{
					HandleLoadingError(x.Exception);
				}, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
		}).ContinueWith(delegate(Task x)
		{
			HandleLoadingError(x.Exception);
		}, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
	}

	private void ProcessLoadingResults(bool loadMappedOnly, int maxResultsCount, CancellationTokenSource currentCancellationTokenSource)
	{
		bool toManyResults = false;
		toManyResults = !loadMappedOnly && currentFilteredObjectsWithDataLink.Count >= maxResultsCount;
		if (toManyResults)
		{
			currentFilteredObjectsWithDataLink.Take(maxResultsCount).ToList();
		}
		if (dataLinksGrid.IsHandleCreated)
		{
			nameTextEdit.Invoke((Action)delegate
			{
				if (toManyResults)
				{
					errorTextLabelControl.Text = $"Only the first {maxResultsCount} results are shown. " + "Use more detailed criteria to get more specific results.";
					bottomAboveButtonsEmptySpaceItem.Visibility = LayoutVisibility.Never;
					errorInformationLayoutControlItem.Visibility = LayoutVisibility.Always;
				}
				else
				{
					errorInformationLayoutControlItem.Visibility = LayoutVisibility.Never;
					bottomAboveButtonsEmptySpaceItem.Visibility = LayoutVisibility.Always;
				}
			});
		}
		if (currentCancellationTokenSource.Token.IsCancellationRequested)
		{
			return;
		}
		bool flag = currentFilteredObjectsWithDataLink.Count == 0;
		int i;
		for (i = 0; i < currentFilteredObjectsWithDataLink.Count; i++)
		{
			ObjectWithDataLinkExtended objectWithDataLinkExtended = modifiedObjectsWithDataLink.FirstOrDefault((ObjectWithDataLinkExtended x) => x.IsSameObjectAndElement(currentFilteredObjectsWithDataLink[i]));
			if (objectWithDataLinkExtended != null)
			{
				objectWithDataLinkExtended.IsOutsideFilter = false;
				currentFilteredObjectsWithDataLink[i] = objectWithDataLinkExtended;
			}
		}
		if (currentCancellationTokenSource.Token.IsCancellationRequested)
		{
			return;
		}
		IEnumerable<ObjectWithDataLinkExtended> enumerable = modifiedObjectsWithDataLink.Where((ObjectWithDataLinkExtended x) => !currentFilteredObjectsWithDataLink.Any((ObjectWithDataLinkExtended y) => y.IsSameObjectAndElement(x)));
		foreach (ObjectWithDataLinkExtended item in enumerable)
		{
			item.IsOutsideFilter = true;
		}
		currentFilteredObjectsWithDataLink.AddRange(enumerable);
		currentFilteredObjectsWithDataLink.ForEach(delegate(ObjectWithDataLinkExtended x)
		{
			x.TermId = term.TermId;
			x.TermDocumentationShowSchema = term.DatabaseShowSchema;
			x.TermDocumentationShowSchemaOverride = term.DatabaseShowSchemaOverride;
		});
		if (loadMappedOnly)
		{
			SetHeaderInformation("Start typing object name in the field above to search. Objects currently linked with " + term.Title + ":");
		}
		else
		{
			bool flag2 = currentFilteredObjectsWithDataLink.Any((ObjectWithDataLinkExtended x) => x.IsOutsideFilter);
			string text = string.Empty;
			if (string.IsNullOrEmpty(nameTextEdit.Text))
			{
				if (tableViewFilterCheckEdit.Checked)
				{
					text = "table/view/structure";
				}
				if (columnFilterCheckEdit.Checked)
				{
					text = ((!(text != string.Empty)) ? "column/field" : (text + "/column/field"));
				}
				if (text != string.Empty)
				{
					SetHeaderInformation("Displaying " + text + (flag2 ? " and currently modified" : string.Empty) + " objects:");
				}
				else
				{
					SetHeaderInformation("Select an object type and start typing object name in the field above to search." + (flag2 ? " Displaying currently modified objects:" : string.Empty));
				}
			}
			else
			{
				if (tableViewFilterCheckEdit.Checked)
				{
					text = "table/view/structure";
				}
				if (columnFilterCheckEdit.Checked)
				{
					text = ((!(text != string.Empty)) ? "column/field" : (text + "/column/field"));
				}
				if (text != string.Empty)
				{
					SetHeaderInformation("Displaying objects containing \"" + nameTextEdit.Text + "\" in " + text + " name and title" + (flag2 ? " and currently modified objects" : string.Empty) + ":");
				}
				else
				{
					SetHeaderInformation("Select an object type and start typing object name in the field above to search." + (flag2 ? " Displaying currently modified objects:" : string.Empty));
				}
			}
		}
		List<ObjectWithDataLinkExtended> data = (from x in currentFilteredObjectsWithDataLink
			orderby x.ObjectNameWithSchemaAndTitle, x.ElementNameWithTitle
			select x).ToList();
		if (flag)
		{
			if (loadMappedOnly)
			{
				ObjectWithDataLinkExtended objectWithDataLinkExtended2 = new ObjectWithDataLinkExtended();
				objectWithDataLinkExtended2.ObjectTitle = "No objects linked to " + term.Title + " found";
				data.Add(objectWithDataLinkExtended2);
				ObjectWithDataLinkExtended objectWithDataLinkExtended3 = new ObjectWithDataLinkExtended();
				objectWithDataLinkExtended3.ObjectTitle = "Provide filter to add new links to " + term.Title;
				data.Add(objectWithDataLinkExtended3);
			}
			else
			{
				ObjectWithDataLinkExtended objectWithDataLinkExtended4 = new ObjectWithDataLinkExtended();
				objectWithDataLinkExtended4.ObjectTitle = "No objects found using provided filter";
				data.Add(objectWithDataLinkExtended4);
			}
		}
		if (currentCancellationTokenSource.Token.IsCancellationRequested || !dataLinksGrid.IsHandleCreated)
		{
			return;
		}
		dataLinksGrid.Invoke((Action)delegate
		{
			if (!currentCancellationTokenSource.Token.IsCancellationRequested)
			{
				dataLinksGrid.DataSource = data;
				SetBestFitForColumns();
			}
		});
	}

	private void CleanupLoadingDataTaskResources()
	{
		loadingDataCancellationTokenSource.Dispose();
		loadingDataCancellationTokenSource = null;
		if (loadingDataMutex.CurrentCount < 1)
		{
			loadingDataMutex.Release();
		}
	}

	private void FinishInterfaceAfterLoading()
	{
		if (!dataLinksGrid.IsHandleCreated)
		{
			return;
		}
		dataLinksGrid.Invoke((Action)delegate
		{
			dataLinksGrid.Enabled = true;
			progressPanel.Invoke((Action)delegate
			{
				progressPanelPanel.Visible = false;
			});
		});
	}

	private void HandleLoadingError(Exception exception)
	{
		CleanupLoadingDataTaskResources();
		FinishInterfaceAfterLoading();
		GeneralExceptionHandling.Handle(exception, "Error occurred while loading data.", this);
		dataLinksGrid.Invoke((Action)delegate
		{
			dataLinksGridView.EndDataUpdate();
			dataLinksGrid.EndUpdate();
			dataLinksGrid.ResumeLayout();
		});
	}

	private IEnumerable<ObjectWithDataLinkExtended> GetAllChangedObjects()
	{
		return currentFilteredObjectsWithDataLink.Where((ObjectWithDataLinkExtended x) => x.IsDataChanged).Concat(modifiedObjectsWithDataLink.Where((ObjectWithDataLinkExtended x) => !currentFilteredObjectsWithDataLink.Any((ObjectWithDataLinkExtended y) => y.MappingId == x.MappingId)));
	}

	private void SetBestFitForColumns()
	{
		CommonFunctionsPanels.SetBestFitForColumns(dataLinksGridView);
		if (descriptionGridColumn.Width > 400)
		{
			descriptionGridColumn.Width = 400;
		}
	}

	private void dataLinksGrid_Click(object sender, EventArgs e)
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
		this.components = new System.ComponentModel.Container();
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.AddDataLinksForm));
		this.rootLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.headerLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerIconPictureBox = new System.Windows.Forms.PictureBox();
		this.headerLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.headerIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.headerLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.errorInformationLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.errorTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.errorIconPictureBox = new System.Windows.Forms.PictureBox();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.errorIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.errorTextLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnFilterCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.tableViewFilterCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.nameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.dataLinksGrid = new DevExpress.XtraGrid.GridControl();
		this.dataLinksGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.isSelectedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.isSelectedRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.objectGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.dataTypeTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.documentationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataLinksToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.rootLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.termsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.leftButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.saveLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.beetwenButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bottomAboveButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.nameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableViewFilterLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnFilterLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.errorInformationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.headerInformationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.aboveGridEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.progressPanelPanel = new System.Windows.Forms.Panel();
		this.progressPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.progressLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.progressLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.progressLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.progressPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.snowflakeDbCommand1 = new Snowflake.Data.Client.SnowflakeDbCommand();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControl).BeginInit();
		this.rootLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.headerLayoutControl).BeginInit();
		this.headerLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.headerIconPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorInformationLayoutControl).BeginInit();
		this.errorInformationLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.errorIconPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnFilterCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableViewFilterCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectedRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableViewFilterLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnFilterLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorInformationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerInformationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.aboveGridEmptySpaceItem).BeginInit();
		this.progressPanelPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressPanelControl).BeginInit();
		this.progressPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControl).BeginInit();
		this.progressLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressPanelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressLabelLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.rootLayoutControl.AllowCustomization = false;
		this.rootLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.rootLayoutControl.Controls.Add(this.headerLayoutControl);
		this.rootLayoutControl.Controls.Add(this.errorInformationLayoutControl);
		this.rootLayoutControl.Controls.Add(this.columnFilterCheckEdit);
		this.rootLayoutControl.Controls.Add(this.tableViewFilterCheckEdit);
		this.rootLayoutControl.Controls.Add(this.nameTextEdit);
		this.rootLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.rootLayoutControl.Controls.Add(this.saveSimpleButton);
		this.rootLayoutControl.Controls.Add(this.dataLinksGrid);
		this.rootLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.rootLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.rootLayoutControl.Name = "rootLayoutControl";
		this.rootLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3613, 314, 827, 486);
		this.rootLayoutControl.Root = this.rootLayoutControlGroup;
		this.rootLayoutControl.Size = new System.Drawing.Size(950, 551);
		this.rootLayoutControl.TabIndex = 0;
		this.rootLayoutControl.Text = "layoutControl1";
		this.headerLayoutControl.AllowCustomization = false;
		this.headerLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.headerLayoutControl.Controls.Add(this.headerLabelControl);
		this.headerLayoutControl.Controls.Add(this.headerIconPictureBox);
		this.headerLayoutControl.Location = new System.Drawing.Point(13, 59);
		this.headerLayoutControl.Name = "headerLayoutControl";
		this.headerLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(446, 815, 327, 427);
		this.headerLayoutControl.Root = this.headerLayoutControlGroup;
		this.headerLayoutControl.Size = new System.Drawing.Size(924, 20);
		this.headerLayoutControl.TabIndex = 25;
		this.headerLayoutControl.Text = "layoutControl1";
		this.headerLabelControl.AutoEllipsis = true;
		this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.headerLabelControl.Location = new System.Drawing.Point(22, 2);
		this.headerLabelControl.MaximumSize = new System.Drawing.Size(0, 16);
		this.headerLabelControl.MinimumSize = new System.Drawing.Size(0, 16);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(900, 16);
		this.headerLabelControl.StyleController = this.headerLayoutControl;
		this.headerLabelControl.TabIndex = 23;
		this.headerLabelControl.Text = "Header information";
		this.headerLabelControl.UseMnemonic = false;
		this.headerIconPictureBox.Image = Dataedo.App.Properties.Resources.about_16;
		this.headerIconPictureBox.Location = new System.Drawing.Point(2, 2);
		this.headerIconPictureBox.Margin = new System.Windows.Forms.Padding(0);
		this.headerIconPictureBox.MaximumSize = new System.Drawing.Size(16, 16);
		this.headerIconPictureBox.MinimumSize = new System.Drawing.Size(16, 16);
		this.headerIconPictureBox.Name = "headerIconPictureBox";
		this.headerIconPictureBox.Size = new System.Drawing.Size(16, 16);
		this.headerIconPictureBox.TabIndex = 22;
		this.headerIconPictureBox.TabStop = false;
		this.headerLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.headerLayoutControlGroup.GroupBordersVisible = false;
		this.headerLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.headerIconLayoutControlItem, this.headerLayoutControlItem });
		this.headerLayoutControlGroup.Name = "headerLayoutControlGroup";
		this.headerLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.headerLayoutControlGroup.Size = new System.Drawing.Size(924, 20);
		this.headerLayoutControlGroup.TextVisible = false;
		this.headerIconLayoutControlItem.Control = this.headerIconPictureBox;
		this.headerIconLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.headerIconLayoutControlItem.MaxSize = new System.Drawing.Size(20, 20);
		this.headerIconLayoutControlItem.MinSize = new System.Drawing.Size(20, 20);
		this.headerIconLayoutControlItem.Name = "headerIconLayoutControlItem";
		this.headerIconLayoutControlItem.Size = new System.Drawing.Size(20, 20);
		this.headerIconLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerIconLayoutControlItem.TextVisible = false;
		this.headerLayoutControlItem.Control = this.headerLabelControl;
		this.headerLayoutControlItem.Location = new System.Drawing.Point(20, 0);
		this.headerLayoutControlItem.MinSize = new System.Drawing.Size(1, 20);
		this.headerLayoutControlItem.Name = "headerLayoutControlItem";
		this.headerLayoutControlItem.Size = new System.Drawing.Size(904, 20);
		this.headerLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLayoutControlItem.TextVisible = false;
		this.errorInformationLayoutControl.AllowCustomization = false;
		this.errorInformationLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.errorInformationLayoutControl.Controls.Add(this.errorTextLabelControl);
		this.errorInformationLayoutControl.Controls.Add(this.errorIconPictureBox);
		this.errorInformationLayoutControl.Location = new System.Drawing.Point(13, 482);
		this.errorInformationLayoutControl.Name = "errorInformationLayoutControl";
		this.errorInformationLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(446, 815, 327, 427);
		this.errorInformationLayoutControl.Root = this.Root;
		this.errorInformationLayoutControl.Size = new System.Drawing.Size(924, 20);
		this.errorInformationLayoutControl.TabIndex = 23;
		this.errorInformationLayoutControl.Text = "layoutControl1";
		this.errorTextLabelControl.AutoEllipsis = true;
		this.errorTextLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.errorTextLabelControl.Location = new System.Drawing.Point(22, 2);
		this.errorTextLabelControl.MaximumSize = new System.Drawing.Size(0, 16);
		this.errorTextLabelControl.MinimumSize = new System.Drawing.Size(0, 16);
		this.errorTextLabelControl.Name = "errorTextLabelControl";
		this.errorTextLabelControl.Size = new System.Drawing.Size(900, 16);
		this.errorTextLabelControl.StyleController = this.errorInformationLayoutControl;
		this.errorTextLabelControl.TabIndex = 23;
		this.errorTextLabelControl.Text = "Error text";
		this.errorIconPictureBox.Image = Dataedo.App.Properties.Resources.warning_16;
		this.errorIconPictureBox.Location = new System.Drawing.Point(2, 2);
		this.errorIconPictureBox.Margin = new System.Windows.Forms.Padding(0);
		this.errorIconPictureBox.MaximumSize = new System.Drawing.Size(16, 16);
		this.errorIconPictureBox.MinimumSize = new System.Drawing.Size(16, 16);
		this.errorIconPictureBox.Name = "errorIconPictureBox";
		this.errorIconPictureBox.Size = new System.Drawing.Size(16, 16);
		this.errorIconPictureBox.TabIndex = 22;
		this.errorIconPictureBox.TabStop = false;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.errorIconLayoutControlItem, this.errorTextLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(924, 20);
		this.Root.TextVisible = false;
		this.errorIconLayoutControlItem.Control = this.errorIconPictureBox;
		this.errorIconLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.errorIconLayoutControlItem.MaxSize = new System.Drawing.Size(20, 20);
		this.errorIconLayoutControlItem.MinSize = new System.Drawing.Size(20, 20);
		this.errorIconLayoutControlItem.Name = "errorIconLayoutControlItem";
		this.errorIconLayoutControlItem.Size = new System.Drawing.Size(20, 20);
		this.errorIconLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.errorIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.errorIconLayoutControlItem.TextVisible = false;
		this.errorTextLayoutControlItem.Control = this.errorTextLabelControl;
		this.errorTextLayoutControlItem.Location = new System.Drawing.Point(20, 0);
		this.errorTextLayoutControlItem.MinSize = new System.Drawing.Size(1, 20);
		this.errorTextLayoutControlItem.Name = "errorTextLayoutControlItem";
		this.errorTextLayoutControlItem.Size = new System.Drawing.Size(904, 20);
		this.errorTextLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.errorTextLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.errorTextLayoutControlItem.TextVisible = false;
		this.columnFilterCheckEdit.EditValue = true;
		this.columnFilterCheckEdit.Location = new System.Drawing.Point(13, 36);
		this.columnFilterCheckEdit.Name = "columnFilterCheckEdit";
		this.columnFilterCheckEdit.Properties.Caption = "Column/Field";
		this.columnFilterCheckEdit.Size = new System.Drawing.Size(84, 20);
		this.columnFilterCheckEdit.StyleController = this.rootLayoutControl;
		this.columnFilterCheckEdit.TabIndex = 20;
		this.columnFilterCheckEdit.CheckedChanged += new System.EventHandler(ColumnFilterCheckEdit_CheckedChanged);
		this.tableViewFilterCheckEdit.EditValue = true;
		this.tableViewFilterCheckEdit.Location = new System.Drawing.Point(13, 13);
		this.tableViewFilterCheckEdit.Name = "tableViewFilterCheckEdit";
		this.tableViewFilterCheckEdit.Properties.Caption = "Table/View/Structure";
		this.tableViewFilterCheckEdit.Size = new System.Drawing.Size(126, 20);
		this.tableViewFilterCheckEdit.StyleController = this.rootLayoutControl;
		this.tableViewFilterCheckEdit.TabIndex = 19;
		this.tableViewFilterCheckEdit.CheckedChanged += new System.EventHandler(TableViewFilterCheckEdit_CheckedChanged);
		this.nameTextEdit.Location = new System.Drawing.Point(178, 13);
		this.nameTextEdit.MenuManager = this.barManager;
		this.nameTextEdit.Name = "nameTextEdit";
		this.nameTextEdit.Properties.EditValueChangedDelay = 333;
		this.nameTextEdit.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
		this.nameTextEdit.Properties.MaxLength = 4000;
		this.nameTextEdit.Properties.NullValuePrompt = "Enter text to search...";
		this.nameTextEdit.Size = new System.Drawing.Size(759, 20);
		this.nameTextEdit.StyleController = this.rootLayoutControl;
		this.nameTextEdit.TabIndex = 18;
		this.nameTextEdit.EditValueChanged += new System.EventHandler(NameTextEdit_EditValueChanged);
		this.nameTextEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(NameTextEdit_KeyDown);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(950, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 551);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(950, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 551);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(950, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 551);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(857, 516);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.rootLayoutControl;
		this.cancelSimpleButton.TabIndex = 16;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(CancelSimpleButton_Click);
		this.saveSimpleButton.Location = new System.Drawing.Point(761, 516);
		this.saveSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.saveSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.saveSimpleButton.StyleController = this.rootLayoutControl;
		this.saveSimpleButton.TabIndex = 15;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(SaveSimpleButton_Click);
		this.dataLinksGrid.AllowDrop = true;
		this.dataLinksGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.dataLinksGrid.Location = new System.Drawing.Point(11, 91);
		this.dataLinksGrid.MainView = this.dataLinksGridView;
		this.dataLinksGrid.Margin = new System.Windows.Forms.Padding(0);
		this.dataLinksGrid.MenuManager = this.barManager;
		this.dataLinksGrid.Name = "dataLinksGrid";
		this.dataLinksGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.iconRepositoryItemPictureEdit, this.titleRepositoryItemCustomTextEdit, this.isSelectedRepositoryItemCheckEdit });
		this.dataLinksGrid.Size = new System.Drawing.Size(928, 389);
		this.dataLinksGrid.TabIndex = 13;
		this.dataLinksGrid.ToolTipController = this.dataLinksToolTipController;
		this.dataLinksGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dataLinksGridView });
		this.dataLinksGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.isSelectedGridColumn, this.iconGridColumn, this.objectGridColumn, this.dataTypeTableGridColumn, this.documentationGridColumn, this.descriptionGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn });
		defaultBulkCopy.IsCopying = false;
		this.dataLinksGridView.Copy = defaultBulkCopy;
		this.dataLinksGridView.GridControl = this.dataLinksGrid;
		this.dataLinksGridView.Name = "dataLinksGridView";
		this.dataLinksGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.dataLinksGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.dataLinksGridView.OptionsSelection.MultiSelect = true;
		this.dataLinksGridView.OptionsView.ColumnAutoWidth = false;
		this.dataLinksGridView.OptionsView.RowAutoHeight = true;
		this.dataLinksGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.dataLinksGridView.OptionsView.ShowGroupPanel = false;
		this.dataLinksGridView.OptionsView.ShowIndicator = false;
		this.dataLinksGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(TermsGridView_CustomDrawCell);
		this.dataLinksGridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(TermsGridView_CustomRowCellEdit);
		this.dataLinksGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(TermsGridView_ShowingEditor);
		this.dataLinksGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(TermsGridView_CellValueChanging);
		this.dataLinksGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(TermsGridView_KeyDown);
		this.dataLinksGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(TermsGridView_MouseDown);
		this.dataLinksGridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(TermsGridView_ValidatingEditor);
		this.isSelectedGridColumn.Caption = " ";
		this.isSelectedGridColumn.ColumnEdit = this.isSelectedRepositoryItemCheckEdit;
		this.isSelectedGridColumn.FieldName = "IsSelected";
		this.isSelectedGridColumn.MaxWidth = 29;
		this.isSelectedGridColumn.MinWidth = 29;
		this.isSelectedGridColumn.Name = "isSelectedGridColumn";
		this.isSelectedGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.isSelectedGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.isSelectedGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.isSelectedGridColumn.Visible = true;
		this.isSelectedGridColumn.VisibleIndex = 0;
		this.isSelectedGridColumn.Width = 29;
		this.isSelectedRepositoryItemCheckEdit.AutoHeight = false;
		this.isSelectedRepositoryItemCheckEdit.Name = "isSelectedRepositoryItemCheckEdit";
		this.isSelectedRepositoryItemCheckEdit.EditValueChanged += new System.EventHandler(IsSelectedRepositoryItemCheckEdit_EditValueChanged);
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.iconGridColumn.FieldName = "iconGridColumn";
		this.iconGridColumn.MaxWidth = 29;
		this.iconGridColumn.MinWidth = 29;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.AllowEdit = false;
		this.iconGridColumn.OptionsColumn.ReadOnly = true;
		this.iconGridColumn.OptionsFilter.AllowFilter = false;
		this.iconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 1;
		this.iconGridColumn.Width = 29;
		this.iconRepositoryItemPictureEdit.AllowFocused = false;
		this.iconRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.iconRepositoryItemPictureEdit.ShowMenu = false;
		this.objectGridColumn.Caption = "Table/Column";
		this.objectGridColumn.ColumnEdit = this.titleRepositoryItemCustomTextEdit;
		this.objectGridColumn.FieldName = "FullNameFormatted";
		this.objectGridColumn.Name = "objectGridColumn";
		this.objectGridColumn.OptionsColumn.AllowEdit = false;
		this.objectGridColumn.OptionsColumn.ReadOnly = true;
		this.objectGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.objectGridColumn.OptionsFilter.AllowFilter = false;
		this.objectGridColumn.Tag = "FIT_WIDTH";
		this.objectGridColumn.Visible = true;
		this.objectGridColumn.VisibleIndex = 2;
		this.objectGridColumn.Width = 140;
		this.titleRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.titleRepositoryItemCustomTextEdit.AutoHeight = false;
		this.titleRepositoryItemCustomTextEdit.Name = "titleRepositoryItemCustomTextEdit";
		this.dataTypeTableGridColumn.Caption = "Data type";
		this.dataTypeTableGridColumn.FieldName = "ElementFullDataType";
		this.dataTypeTableGridColumn.Name = "dataTypeTableGridColumn";
		this.dataTypeTableGridColumn.OptionsColumn.AllowEdit = false;
		this.dataTypeTableGridColumn.OptionsColumn.ReadOnly = true;
		this.dataTypeTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dataTypeTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dataTypeTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.dataTypeTableGridColumn.Tag = "FIT_WIDTH";
		this.dataTypeTableGridColumn.Visible = true;
		this.dataTypeTableGridColumn.VisibleIndex = 3;
		this.documentationGridColumn.Caption = "Database";
		this.documentationGridColumn.FieldName = "ObjectDocumentationTitle";
		this.documentationGridColumn.Name = "documentationGridColumn";
		this.documentationGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationGridColumn.OptionsColumn.ReadOnly = true;
		this.documentationGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.documentationGridColumn.Tag = "FIT_WIDTH";
		this.documentationGridColumn.Visible = true;
		this.documentationGridColumn.VisibleIndex = 4;
		this.documentationGridColumn.Width = 200;
		this.descriptionGridColumn.Caption = "Description";
		this.descriptionGridColumn.FieldName = "Description";
		this.descriptionGridColumn.Name = "descriptionGridColumn";
		this.descriptionGridColumn.OptionsColumn.AllowEdit = false;
		this.descriptionGridColumn.OptionsColumn.ReadOnly = true;
		this.descriptionGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionGridColumn.Tag = "FIT_WIDTH";
		this.descriptionGridColumn.Visible = true;
		this.descriptionGridColumn.VisibleIndex = 5;
		this.descriptionGridColumn.Width = 100;
		this.createdGridColumn.Caption = "Created";
		this.createdGridColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
		this.createdGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.createdGridColumn.FieldName = "CreationDate";
		this.createdGridColumn.Name = "createdGridColumn";
		this.createdGridColumn.OptionsColumn.AllowEdit = false;
		this.createdGridColumn.OptionsColumn.ReadOnly = true;
		this.createdGridColumn.Width = 120;
		this.createdByGridColumn.Caption = "Created by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowEdit = false;
		this.createdByGridColumn.OptionsColumn.ReadOnly = true;
		this.createdByGridColumn.Width = 150;
		this.lastUpdatedGridColumn.Caption = "Last updated";
		this.lastUpdatedGridColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
		this.lastUpdatedGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.lastUpdatedGridColumn.FieldName = "LastModificationDate";
		this.lastUpdatedGridColumn.Name = "lastUpdatedGridColumn";
		this.lastUpdatedGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedGridColumn.OptionsColumn.ReadOnly = true;
		this.lastUpdatedGridColumn.Width = 100;
		this.lastUpdatedByGridColumn.Caption = "Last updated by";
		this.lastUpdatedByGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByGridColumn.Name = "lastUpdatedByGridColumn";
		this.lastUpdatedByGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedByGridColumn.OptionsColumn.ReadOnly = true;
		this.lastUpdatedByGridColumn.Width = 100;
		this.dataLinksToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.rootLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.rootLayoutControlGroup.GroupBordersVisible = false;
		this.rootLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.termsLayoutControlItem, this.leftButtonsEmptySpaceItem, this.saveLayoutControlItem, this.cancelLayoutControlItem, this.beetwenButtonsEmptySpaceItem, this.bottomAboveButtonsEmptySpaceItem, this.nameLayoutControlItem, this.tableViewFilterLayoutControlItem, this.columnFilterLayoutControlItem, this.errorInformationLayoutControlItem,
			this.headerInformationLayoutControlItem, this.aboveGridEmptySpaceItem
		});
		this.rootLayoutControlGroup.Name = "Root";
		this.rootLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(11, 11, 11, 11);
		this.rootLayoutControlGroup.Size = new System.Drawing.Size(950, 551);
		this.rootLayoutControlGroup.TextVisible = false;
		this.termsLayoutControlItem.Control = this.dataLinksGrid;
		this.termsLayoutControlItem.Location = new System.Drawing.Point(0, 80);
		this.termsLayoutControlItem.MinSize = new System.Drawing.Size(104, 24);
		this.termsLayoutControlItem.Name = "termsLayoutControlItem";
		this.termsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.termsLayoutControlItem.Size = new System.Drawing.Size(928, 389);
		this.termsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.termsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.termsLayoutControlItem.TextVisible = false;
		this.leftButtonsEmptySpaceItem.AllowHotTrack = false;
		this.leftButtonsEmptySpaceItem.CustomizationFormText = "emptySpaceItem1";
		this.leftButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 503);
		this.leftButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.leftButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(1, 26);
		this.leftButtonsEmptySpaceItem.Name = "leftButtonsEmptySpaceItem";
		this.leftButtonsEmptySpaceItem.Size = new System.Drawing.Size(748, 26);
		this.leftButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.leftButtonsEmptySpaceItem.Text = "emptySpaceItem1";
		this.leftButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveLayoutControlItem.Control = this.saveSimpleButton;
		this.saveLayoutControlItem.Location = new System.Drawing.Point(748, 503);
		this.saveLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.Name = "saveLayoutControlItem";
		this.saveLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveLayoutControlItem.TextVisible = false;
		this.cancelLayoutControlItem.Control = this.cancelSimpleButton;
		this.cancelLayoutControlItem.Location = new System.Drawing.Point(844, 503);
		this.cancelLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.Name = "cancelLayoutControlItem2";
		this.cancelLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelLayoutControlItem.TextVisible = false;
		this.beetwenButtonsEmptySpaceItem.AllowHotTrack = false;
		this.beetwenButtonsEmptySpaceItem.Location = new System.Drawing.Point(832, 503);
		this.beetwenButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.Name = "beetwenButtonsEmptySpaceItem";
		this.beetwenButtonsEmptySpaceItem.Size = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.beetwenButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomAboveButtonsEmptySpaceItem.AllowHotTrack = false;
		this.bottomAboveButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 493);
		this.bottomAboveButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.bottomAboveButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(1, 10);
		this.bottomAboveButtonsEmptySpaceItem.Name = "bottomAboveButtonsEmptySpaceItem";
		this.bottomAboveButtonsEmptySpaceItem.Size = new System.Drawing.Size(928, 10);
		this.bottomAboveButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomAboveButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.nameLayoutControlItem.Control = this.nameTextEdit;
		this.nameLayoutControlItem.Location = new System.Drawing.Point(130, 0);
		this.nameLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.nameLayoutControlItem.MinSize = new System.Drawing.Size(84, 24);
		this.nameLayoutControlItem.Name = "nameLayoutControlItem";
		this.nameLayoutControlItem.Size = new System.Drawing.Size(798, 46);
		this.nameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nameLayoutControlItem.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 0, 0, 0);
		this.nameLayoutControlItem.Text = "Name";
		this.nameLayoutControlItem.TextSize = new System.Drawing.Size(27, 13);
		this.tableViewFilterLayoutControlItem.Control = this.tableViewFilterCheckEdit;
		this.tableViewFilterLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.tableViewFilterLayoutControlItem.MaxSize = new System.Drawing.Size(130, 23);
		this.tableViewFilterLayoutControlItem.MinSize = new System.Drawing.Size(130, 23);
		this.tableViewFilterLayoutControlItem.Name = "tableViewFilterLayoutControlItem";
		this.tableViewFilterLayoutControlItem.Size = new System.Drawing.Size(130, 23);
		this.tableViewFilterLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableViewFilterLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.tableViewFilterLayoutControlItem.TextVisible = false;
		this.columnFilterLayoutControlItem.Control = this.columnFilterCheckEdit;
		this.columnFilterLayoutControlItem.Location = new System.Drawing.Point(0, 23);
		this.columnFilterLayoutControlItem.MaxSize = new System.Drawing.Size(88, 23);
		this.columnFilterLayoutControlItem.MinSize = new System.Drawing.Size(88, 23);
		this.columnFilterLayoutControlItem.Name = "columnFilterLayoutControlItem";
		this.columnFilterLayoutControlItem.Size = new System.Drawing.Size(130, 23);
		this.columnFilterLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.columnFilterLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.columnFilterLayoutControlItem.TextVisible = false;
		this.errorInformationLayoutControlItem.Control = this.errorInformationLayoutControl;
		this.errorInformationLayoutControlItem.Location = new System.Drawing.Point(0, 469);
		this.errorInformationLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.errorInformationLayoutControlItem.MinSize = new System.Drawing.Size(111, 24);
		this.errorInformationLayoutControlItem.Name = "errorInformationLayoutControlItem";
		this.errorInformationLayoutControlItem.Size = new System.Drawing.Size(928, 24);
		this.errorInformationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.errorInformationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.errorInformationLayoutControlItem.TextVisible = false;
		this.headerInformationLayoutControlItem.Control = this.headerLayoutControl;
		this.headerInformationLayoutControlItem.Location = new System.Drawing.Point(0, 46);
		this.headerInformationLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.headerInformationLayoutControlItem.MinSize = new System.Drawing.Size(25, 24);
		this.headerInformationLayoutControlItem.Name = "headerInformationLayoutControlItem";
		this.headerInformationLayoutControlItem.Size = new System.Drawing.Size(928, 24);
		this.headerInformationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerInformationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerInformationLayoutControlItem.TextVisible = false;
		this.aboveGridEmptySpaceItem.AllowHotTrack = false;
		this.aboveGridEmptySpaceItem.Location = new System.Drawing.Point(0, 70);
		this.aboveGridEmptySpaceItem.Name = "aboveGridEmptySpaceItem";
		this.aboveGridEmptySpaceItem.Size = new System.Drawing.Size(928, 10);
		this.aboveGridEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressPanelPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.progressPanelPanel.BackColor = System.Drawing.Color.Transparent;
		this.progressPanelPanel.Controls.Add(this.progressPanelControl);
		this.progressPanelPanel.Location = new System.Drawing.Point(325, 235);
		this.progressPanelPanel.Name = "progressPanelPanel";
		this.progressPanelPanel.Size = new System.Drawing.Size(300, 81);
		this.progressPanelPanel.TabIndex = 3;
		this.progressPanelPanel.Visible = false;
		this.progressPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
		this.progressPanelControl.Controls.Add(this.progressLayoutControl);
		this.progressPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressPanelControl.Location = new System.Drawing.Point(0, 0);
		this.progressPanelControl.Name = "progressPanelControl";
		this.progressPanelControl.Size = new System.Drawing.Size(300, 81);
		this.progressPanelControl.TabIndex = 5;
		this.progressLayoutControl.AllowCustomization = false;
		this.progressLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.progressLayoutControl.Controls.Add(this.progressLabelControl);
		this.progressLayoutControl.Controls.Add(this.progressPanel);
		this.progressLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressLayoutControl.Location = new System.Drawing.Point(2, 2);
		this.progressLayoutControl.Name = "progressLayoutControl";
		this.progressLayoutControl.Root = this.progressLayoutControlGroup;
		this.progressLayoutControl.Size = new System.Drawing.Size(296, 77);
		this.progressLayoutControl.TabIndex = 6;
		this.progressLayoutControl.Text = "layoutControl1";
		this.progressLabelControl.AutoEllipsis = true;
		this.progressLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.progressLabelControl.Location = new System.Drawing.Point(12, 52);
		this.progressLabelControl.Name = "progressLabelControl";
		this.progressLabelControl.Padding = new System.Windows.Forms.Padding(41, 0, 0, 0);
		this.progressLabelControl.Size = new System.Drawing.Size(272, 13);
		this.progressLabelControl.StyleController = this.progressLayoutControl;
		this.progressLabelControl.TabIndex = 6;
		this.progressPanel.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f);
		this.progressPanel.AppearanceCaption.Options.UseFont = true;
		this.progressPanel.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressPanel.AppearanceDescription.Options.UseFont = true;
		this.progressPanel.Description = "Loading data...";
		this.progressPanel.Location = new System.Drawing.Point(12, 12);
		this.progressPanel.Name = "progressPanel";
		this.progressPanel.Size = new System.Drawing.Size(272, 36);
		this.progressPanel.StyleController = this.progressLayoutControl;
		this.progressPanel.TabIndex = 5;
		this.progressLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.progressLayoutControlGroup.GroupBordersVisible = false;
		this.progressLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.progressPanelLayoutControlItem, this.progressLabelLayoutControlItem });
		this.progressLayoutControlGroup.Name = "progressLayoutControlGroup";
		this.progressLayoutControlGroup.Size = new System.Drawing.Size(296, 77);
		this.progressLayoutControlGroup.TextVisible = false;
		this.progressPanelLayoutControlItem.Control = this.progressPanel;
		this.progressPanelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.progressPanelLayoutControlItem.MinSize = new System.Drawing.Size(1, 1);
		this.progressPanelLayoutControlItem.Name = "progressPanelLayoutControlItem";
		this.progressPanelLayoutControlItem.Size = new System.Drawing.Size(276, 40);
		this.progressPanelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.progressPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressPanelLayoutControlItem.TextVisible = false;
		this.progressLabelLayoutControlItem.Control = this.progressLabelControl;
		this.progressLabelLayoutControlItem.Location = new System.Drawing.Point(0, 40);
		this.progressLabelLayoutControlItem.Name = "progressLabelLayoutControlItem";
		this.progressLabelLayoutControlItem.Size = new System.Drawing.Size(276, 17);
		this.progressLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressLabelLayoutControlItem.TextVisible = false;
		this.snowflakeDbCommand1.CommandText = null;
		this.snowflakeDbCommand1.CommandTimeout = 0;
		this.snowflakeDbCommand1.UpdatedRowSource = System.Data.UpdateRowSource.None;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(950, 551);
		base.Controls.Add(this.progressPanelPanel);
		base.Controls.Add(this.rootLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("AddDataLinksForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "AddDataLinksForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Add links";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AddDataLinksForm_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(AddDataLinksForm_FormClosed);
		base.Shown += new System.EventHandler(AddDataLinksForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControl).EndInit();
		this.rootLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.headerLayoutControl).EndInit();
		this.headerLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.headerIconPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorInformationLayoutControl).EndInit();
		this.errorInformationLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.errorIconPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnFilterCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableViewFilterCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectedRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableViewFilterLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnFilterLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorInformationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerInformationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.aboveGridEmptySpaceItem).EndInit();
		this.progressPanelPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressPanelControl).EndInit();
		this.progressPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControl).EndInit();
		this.progressLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressPanelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressLabelLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
