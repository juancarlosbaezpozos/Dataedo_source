using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraWaitForm;

namespace Dataedo.App.Forms;

public class AddLinkToBusinessGlossaryForm : BaseXtraForm
{
	private MetadataEditorUserControl mainControl;

	private SemaphoreSlim loadingDataMutex = new SemaphoreSlim(1, 1);

	private CancellationTokenSource loadingDataCancellationTokenSource = new CancellationTokenSource();

	private List<TermObjectWithLinkExtended> currentTermObjects;

	private Dictionary<int, bool> currentTermsModifications;

	private DXErrorProvider dXErrorProvider;

	private bool isModified;

	private IContainer components;

	private LayoutControlGroup rootLayoutControlGroup;

	private EmptySpaceItem leftButtonsEmptySpaceItem;

	private SimpleButton saveSimpleButton;

	private LayoutControlItem saveLayoutControlItem;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem cancelLayoutControlItem;

	private EmptySpaceItem beetwenButtonsEmptySpaceItem;

	private EmptySpaceItem bottomAboveButtonsEmptySpaceItem;

	private ToolTipController dataLinksToolTipController;

	private Panel progressPanelPanel;

	private PanelControl progressPanelControl;

	private LabelControl progressLabelControl;

	private ProgressPanel progressPanel;

	private LayoutControlGroup progressLayoutControlGroup;

	private LayoutControlItem progressPanelLayoutControlItem;

	private LayoutControlItem progressLabelLayoutControlItem;

	private NonCustomizableLayoutControl rootLayoutControl;

	private NonCustomizableLayoutControl progressLayoutControl;

	private GridLookUpEdit objectLookUpEdit;

	private CustomGridUserControl objectCustomGridUserControl;

	private GridColumn objectIconGridColumn;

	private GridColumn objectNameGridColumn;

	private LayoutControlItem objectLayoutControlItem;

	private GridPanelUserControl gridPanelUserControl;

	private LayoutControlItem gridPanelLayoutControlItem;

	private GridControl dataLinksGrid;

	private BulkCopyGridUserControl dataLinksGridView;

	private GridColumn isSelectedGridColumn;

	private RepositoryItemCheckEdit repositoryItemCheckEdit;

	private GridColumn iconGridColumn;

	private RepositoryItemPictureEdit repositoryItemPictureEdit;

	private GridColumn glossaryTitleGridColumn;

	private GridColumn titleGridColumn;

	private RepositoryItemCustomTextEdit repositoryItemCustomTextEdit;

	private GridColumn typeGridColumn;

	private GridColumn descriptionGridColumn;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private LayoutControlItem termsLayoutControlItem;

	private PictureBox iconPictureBox;

	private LayoutControlItem iconLayoutControlItem;

	private LabelControl linkDataElementLabelControl;

	private LayoutControlItem linkDataElementLayoutControlItem;

	private LabelControl withFollowingBusinessGlossaryTermsLabelControl;

	private LayoutControlItem withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private RepositoryItemCustomTextEdit linkableObjectFullNameFormattedRepositoryItemCustomTextEdit;

	public AddLinkToBusinessGlossaryForm()
	{
		InitializeComponent();
		dXErrorProvider = new DXErrorProvider();
		AddEvents();
		progressLabelLayoutControlItem.Visibility = LayoutVisibility.Never;
		currentTermObjects = new List<TermObjectWithLinkExtended>();
		currentTermsModifications = new Dictionary<int, bool>();
		dataLinksGridView.Copy = new BulkCopyWithBlockedIsSelectedCells<TermObjectWithLinkExtended>();
	}

	public void SetParameters(string objectType, int objectId, int? elementId, MetadataEditorUserControl mainControl, CustomFieldsSupport customFieldsSupport, bool contextShowSchema)
	{
		this.mainControl = mainControl;
		dataLinksGridView.SetRowCellValue(-2147483646, "iconGridColumn", Resources.blank_16);
		new CustomFieldsCellsTypesSupport(isForSummaryTable: true).SetCustomColumns(dataLinksGridView, customFieldsSupport, SharedObjectTypeEnum.ObjectType.Term, customFieldsAsArray: true);
		foreach (GridColumn item in dataLinksGridView.Columns.Where((GridColumn x) => x.FieldName != "IsSelected"))
		{
			item.OptionsColumn.AllowEdit = false;
		}
		foreach (GridColumn item2 in dataLinksGridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field") || x.FieldName.Contains("field")))
		{
			item2.Visible = false;
		}
		List<LinkableObject> linkableObjects = DB.BusinessGlossary.GetLinkableObjects<LinkableObject>(objectId);
		linkableObjects.ForEach(delegate(LinkableObject x)
		{
			x.ContextShowSchema = contextShowSchema;
		});
		objectLookUpEdit.Properties.DataSource = linkableObjects;
		objectLookUpEdit.EditValue = elementId ?? objectId;
		dataLinksGridView.SetRowCellValue(-2147483646, "iconGridColumn", Resources.blank_16);
		if (!base.IsHandleCreated)
		{
			CreateControl();
		}
		isModified = false;
		SetObjectLookUpEditSize();
		gridPanelUserControl.SetRemoveButtonVisibility(value: false);
		gridPanelUserControl.RemoveCustomFieldsButton();
		gridPanelUserControl.Initialize(Text);
	}

	private void SetFormName()
	{
		LinkableObject currentLinkableObject = GetCurrentLinkableObject();
		if (currentLinkableObject == null)
		{
			Text = "Links to Business Glossary terms";
		}
		else
		{
			Text = currentLinkableObject.DisplayName + " links to Business Glossary terms";
		}
	}

	private LinkableObject GetCurrentLinkableObject()
	{
		return objectLookUpEdit.Properties.GetRowByKeyValue(objectLookUpEdit.EditValue) as LinkableObject;
	}

	private void AddLinkToBusinessGlossaryForm_Shown(object sender, EventArgs e)
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
			Icons.SetIcon(e, iconGridColumn, SharedObjectTypeEnum.ObjectType.Term);
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

	private void TermsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
	}

	private void TermsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		TermObjectWithLinkExtended obj = dataLinksGridView.GetRow((sender as GridView).FocusedRowHandle) as TermObjectWithLinkExtended;
		if (obj != null && !obj.IsInitialized)
		{
			e.Cancel = true;
		}
	}

	private void TermsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.RowHandle != -2147483646)
		{
			TermObjectWithLinkExtended termObjectWithLinkExtended = dataLinksGridView.GetRow(e.RowHandle) as TermObjectWithLinkExtended;
			if (termObjectWithLinkExtended.IsInitialized)
			{
				Grids.SetYellowRowColor(sender, e, termObjectWithLinkExtended.IsDataChanged);
			}
			else
			{
				Grids.DrawCenteredString(sender, e, termObjectWithLinkExtended.Title);
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

	private void TermsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (e.RowHandle != -2147483646)
		{
			(dataLinksGridView.GetFocusedRow() as TermObjectWithLinkExtended).IsModified = true;
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

	private void AddLinkToBusinessGlossaryForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save(e);
		}
		else if (isModified && currentTermObjects.Any((TermObjectWithLinkExtended x) => x.IsDataChanged))
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

	private void AddLinkToBusinessGlossaryForm_FormClosed(object sender, FormClosedEventArgs e)
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
			DB.BusinessGlossary.ProcessSavingDataLinks(currentTermObjects.Where((TermObjectWithLinkExtended x) => x.IsDataChanged));
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
		if (loadingDataCancellationTokenSource != null)
		{
			loadingDataCancellationTokenSource.Cancel();
		}
		CancellationTokenSource currentCancellationTokenSource = new CancellationTokenSource();
		loadingDataCancellationTokenSource = currentCancellationTokenSource;
		foreach (TermObjectWithLinkExtended item in currentTermObjects.Where((TermObjectWithLinkExtended x) => x.TermId.HasValue && (x.IsDataChanged || x.IsModified)))
		{
			if (currentTermsModifications.ContainsKey(item.TermId.Value))
			{
				currentTermsModifications[item.TermId.Value] = item.IsSelected;
			}
			else
			{
				currentTermsModifications.Add(item.TermId.Value, item.IsSelected);
			}
		}
		LinkableObject selectedItem;
		List<TermObjectWithLinkExtended> data;
		new TaskFactory(currentCancellationTokenSource.Token).StartNew(delegate
		{
			try
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
				if (!currentCancellationTokenSource.Token.IsCancellationRequested)
				{
					selectedItem = GetCurrentLinkableObject();
					if (selectedItem == null)
					{
						GeneralMessageBoxesHandling.Show("Object does not exist in repository.", "Object does not exist", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
					}
					else
					{
						if (dataLinksGrid.IsHandleCreated)
						{
							dataLinksGrid.Invoke((Action)delegate
							{
								if (!currentCancellationTokenSource.Token.IsCancellationRequested)
								{
									currentTermObjects = DB.BusinessGlossary.GetTermsAndTermLinks<TermObjectWithLinkExtended>(null, selectedItem.ObjectId, selectedItem.ElementId, selectedItem.CurrentType);
								}
							});
						}
						if (!currentCancellationTokenSource.Token.IsCancellationRequested)
						{
							bool flag = currentTermObjects.Count == 0;
							currentTermObjects.ForEach(delegate(TermObjectWithLinkExtended x)
							{
								x.SetObjectInformation(selectedItem.CurrentType, selectedItem.ObjectId, selectedItem.ElementId);
							});
							data = currentTermObjects.OrderBy((TermObjectWithLinkExtended x) => x.Title).ToList();
							foreach (TermObjectWithLinkExtended item2 in data.Where((TermObjectWithLinkExtended x) => x.TermId.HasValue && currentTermsModifications.ContainsKey(x.TermId.Value)))
							{
								item2.IsSelected = currentTermsModifications[item2.TermId.Value];
								item2.IsModified = item2.IsDataChanged;
							}
							if (flag)
							{
								GetCurrentLinkableObject();
								TermObjectWithLinkExtended termObjectWithLinkExtended = new TermObjectWithLinkExtended();
								termObjectWithLinkExtended.Initialize();
								termObjectWithLinkExtended.Title = "No terms found";
								data.Insert(0, termObjectWithLinkExtended);
								TermObjectWithLinkExtended termObjectWithLinkExtended2 = new TermObjectWithLinkExtended();
								termObjectWithLinkExtended2.Initialize();
								termObjectWithLinkExtended2.Title = "Add Business Glossary terms to add new links";
								data.Insert(1, termObjectWithLinkExtended2);
							}
							if (!currentCancellationTokenSource.Token.IsCancellationRequested && dataLinksGrid.IsHandleCreated)
							{
								dataLinksGrid.Invoke((Action)delegate
								{
									if (!currentCancellationTokenSource.Token.IsCancellationRequested)
									{
										dataLinksGrid.DataSource = data;
										SetBestFitForColumns();
									}
								});
							}
						}
					}
				}
			}
			finally
			{
				if (dataLinksGrid.IsHandleCreated)
				{
					dataLinksGrid.Invoke((Action)delegate
					{
						dataLinksGridView.EndDataUpdate();
						dataLinksGrid.EndUpdate();
						dataLinksGrid.ResumeLayout();
					});
				}
			}
		}).ContinueWith(delegate(Task x)
		{
			if (x.Exception != null)
			{
				HandleLoadingError(x.Exception);
				throw x.Exception;
			}
			loadingDataMutex.Release();
		}, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously).ContinueWith(delegate
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
	}

	private void CleanupLoadingDataTaskResources()
	{
		loadingDataCancellationTokenSource.Dispose();
		loadingDataCancellationTokenSource = null;
		loadingDataMutex.Release();
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
	}

	private IEnumerable<TermObjectWithLinkExtended> GetAllChangedObjects()
	{
		return currentTermObjects.Where((TermObjectWithLinkExtended x) => x.IsDataChanged);
	}

	private void SetBestFitForColumns()
	{
		CommonFunctionsPanels.SetBestFitForColumns(dataLinksGridView);
		if (descriptionGridColumn.Width > 400)
		{
			descriptionGridColumn.Width = 400;
		}
	}

	private void ObjectLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (objectLookUpEdit.Properties.GetRowByKeyValue(objectLookUpEdit.EditValue) is LinkableObject linkableObject)
		{
			Bitmap objectIcon = IconsSupport.GetObjectIcon(linkableObject.CurrentType, linkableObject.CurrentSubtype, linkableObject.CurrentSource);
			iconPictureBox.Image = objectIcon;
			SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(linkableObject.CurrentType);
			string nodeDescription = ToolTips.GetNodeDescription(objectType ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectSubtypeEnum.StringToType(objectType, linkableObject.CurrentSubtype), SynchronizeStateEnum.DBStringToState(linkableObject.CurrentStatus), UserTypeEnum.ObjectToType(linkableObject.CurrentSource));
			new ToolTip().SetToolTip(iconPictureBox, nodeDescription);
			SetFormName();
			CallLoadingData();
		}
	}

	private void ObjectCustomGridUserControl_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column.Equals(objectIconGridColumn))
		{
			LinkableObject linkableObject = (sender as CustomGridUserControl).GetRow(e.RowHandle) as LinkableObject;
			Bitmap objectIcon = IconsSupport.GetObjectIcon(linkableObject.CurrentType, linkableObject.CurrentSubtype, linkableObject.CurrentSource);
			Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, 16, 16);
			e.Cache.DrawImage(objectIcon, rect);
			e.Handled = true;
		}
	}

	private void ObjectLookUpEdit_Resize(object sender, EventArgs e)
	{
		SetObjectLookUpEditSize();
	}

	private void SetObjectLookUpEditSize()
	{
		if (objectLookUpEdit.Properties.DataSource is List<LinkableObject> list)
		{
			int num = 25;
			int num2 = ((list.Count > 25) ? num : list.Count);
			objectLookUpEdit.Properties.PopupFormMinSize = new Size(objectLookUpEdit.Width, objectLookUpEdit.Height);
			objectLookUpEdit.Properties.PopupFormSize = new Size(objectLookUpEdit.Width, objectLookUpEdit.Height * num2);
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
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.AddLinkToBusinessGlossaryForm));
		this.rootLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.linkDataElementLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.iconPictureBox = new System.Windows.Forms.PictureBox();
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.dataLinksGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.isSelectedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.titleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.typeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.glossaryTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataLinksGrid = new DevExpress.XtraGrid.GridControl();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.dataLinksToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.objectLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.objectCustomGridUserControl = new Dataedo.App.UserControls.CustomGridUserControl();
		this.objectIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.objectNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.withFollowingBusinessGlossaryTermsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rootLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.leftButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.saveLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.beetwenButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bottomAboveButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.objectLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.gridPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.termsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.iconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.linkDataElementLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressPanelPanel = new System.Windows.Forms.Panel();
		this.progressPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.progressLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.progressLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.progressLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.progressPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControl).BeginInit();
		this.rootLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.iconPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectCustomGridUserControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridPanelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkDataElementLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem).BeginInit();
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
		this.rootLayoutControl.Controls.Add(this.linkDataElementLabelControl);
		this.rootLayoutControl.Controls.Add(this.iconPictureBox);
		this.rootLayoutControl.Controls.Add(this.gridPanelUserControl);
		this.rootLayoutControl.Controls.Add(this.objectLookUpEdit);
		this.rootLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.rootLayoutControl.Controls.Add(this.saveSimpleButton);
		this.rootLayoutControl.Controls.Add(this.dataLinksGrid);
		this.rootLayoutControl.Controls.Add(this.withFollowingBusinessGlossaryTermsLabelControl);
		this.rootLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.rootLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.rootLayoutControl.MenuManager = this.barManager;
		this.rootLayoutControl.Name = "rootLayoutControl";
		this.rootLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1409, 312, 518, 486);
		this.rootLayoutControl.Root = this.rootLayoutControlGroup;
		this.rootLayoutControl.Size = new System.Drawing.Size(927, 551);
		this.rootLayoutControl.TabIndex = 0;
		this.rootLayoutControl.Text = "layoutControl1";
		this.linkDataElementLabelControl.Location = new System.Drawing.Point(13, 13);
		this.linkDataElementLabelControl.Name = "linkDataElementLabelControl";
		this.linkDataElementLabelControl.Size = new System.Drawing.Size(901, 13);
		this.linkDataElementLabelControl.StyleController = this.rootLayoutControl;
		this.linkDataElementLabelControl.TabIndex = 22;
		this.linkDataElementLabelControl.Text = "Link data element";
		this.iconPictureBox.Location = new System.Drawing.Point(15, 32);
		this.iconPictureBox.Margin = new System.Windows.Forms.Padding(0);
		this.iconPictureBox.MaximumSize = new System.Drawing.Size(16, 16);
		this.iconPictureBox.MinimumSize = new System.Drawing.Size(16, 16);
		this.iconPictureBox.Name = "iconPictureBox";
		this.iconPictureBox.Size = new System.Drawing.Size(16, 16);
		this.iconPictureBox.TabIndex = 21;
		this.iconPictureBox.TabStop = false;
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.gridPanelUserControl.GridView = this.dataLinksGridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(13, 71);
		this.gridPanelUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(901, 26);
		this.gridPanelUserControl.TabIndex = 20;
		this.dataLinksGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.isSelectedGridColumn, this.iconGridColumn, this.titleGridColumn, this.typeGridColumn, this.glossaryTitleGridColumn, this.descriptionGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn });
		defaultBulkCopy.IsCopying = false;
		this.dataLinksGridView.Copy = defaultBulkCopy;
		this.dataLinksGridView.GridControl = this.dataLinksGrid;
		this.dataLinksGridView.Name = "dataLinksGridView";
		this.dataLinksGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.dataLinksGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.dataLinksGridView.OptionsNavigation.AutoMoveRowFocus = false;
		this.dataLinksGridView.OptionsSelection.MultiSelect = true;
		this.dataLinksGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.dataLinksGridView.OptionsView.ColumnAutoWidth = false;
		this.dataLinksGridView.OptionsView.RowAutoHeight = true;
		this.dataLinksGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.dataLinksGridView.OptionsView.ShowGroupPanel = false;
		this.dataLinksGridView.OptionsView.ShowIndicator = false;
		this.dataLinksGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(TermsGridView_CustomDrawCell);
		this.dataLinksGridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(TermsGridView_CustomRowCellEdit);
		this.dataLinksGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(TermsGridView_SelectionChanged);
		this.dataLinksGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(TermsGridView_ShowingEditor);
		this.dataLinksGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(TermsGridView_CellValueChanging);
		this.dataLinksGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(TermsGridView_KeyDown);
		this.dataLinksGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(TermsGridView_MouseDown);
		this.dataLinksGridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(TermsGridView_ValidatingEditor);
		this.isSelectedGridColumn.Caption = " ";
		this.isSelectedGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
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
		this.repositoryItemCheckEdit.AutoHeight = false;
		this.repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
		this.repositoryItemCheckEdit.EditValueChanged += new System.EventHandler(IsSelectedRepositoryItemCheckEdit_EditValueChanged);
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.ColumnEdit = this.repositoryItemPictureEdit;
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
		this.repositoryItemPictureEdit.AllowFocused = false;
		this.repositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemPictureEdit.Name = "repositoryItemPictureEdit";
		this.repositoryItemPictureEdit.ShowMenu = false;
		this.titleGridColumn.Caption = "Term";
		this.titleGridColumn.ColumnEdit = this.repositoryItemCustomTextEdit;
		this.titleGridColumn.FieldName = "Title";
		this.titleGridColumn.Name = "titleGridColumn";
		this.titleGridColumn.OptionsColumn.AllowEdit = false;
		this.titleGridColumn.OptionsColumn.ReadOnly = true;
		this.titleGridColumn.OptionsFilter.AllowFilter = false;
		this.titleGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleGridColumn.Tag = "FIT_WIDTH";
		this.titleGridColumn.Visible = true;
		this.titleGridColumn.VisibleIndex = 2;
		this.titleGridColumn.Width = 140;
		this.repositoryItemCustomTextEdit.AutoHeight = false;
		this.repositoryItemCustomTextEdit.Name = "repositoryItemCustomTextEdit";
		this.typeGridColumn.Caption = "Type";
		this.typeGridColumn.FieldName = "TypeTitle";
		this.typeGridColumn.Name = "typeGridColumn";
		this.typeGridColumn.OptionsColumn.AllowEdit = false;
		this.typeGridColumn.OptionsColumn.ReadOnly = true;
		this.typeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.typeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.typeGridColumn.Tag = "FIT_WIDTH";
		this.typeGridColumn.Visible = true;
		this.typeGridColumn.VisibleIndex = 3;
		this.glossaryTitleGridColumn.Caption = "Glossary";
		this.glossaryTitleGridColumn.FieldName = "DatabaseTitle";
		this.glossaryTitleGridColumn.Name = "glossaryTitleGridColumn";
		this.glossaryTitleGridColumn.OptionsColumn.AllowEdit = false;
		this.glossaryTitleGridColumn.OptionsColumn.ReadOnly = true;
		this.glossaryTitleGridColumn.Tag = "FIT_WIDTH";
		this.glossaryTitleGridColumn.Visible = true;
		this.glossaryTitleGridColumn.VisibleIndex = 4;
		this.glossaryTitleGridColumn.Width = 200;
		this.descriptionGridColumn.Caption = "Term description";
		this.descriptionGridColumn.FieldName = "DescriptionPlain";
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
		this.dataLinksGrid.AllowDrop = true;
		this.dataLinksGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.dataLinksGrid.Location = new System.Drawing.Point(11, 99);
		this.dataLinksGrid.MainView = this.dataLinksGridView;
		this.dataLinksGrid.Margin = new System.Windows.Forms.Padding(0);
		this.dataLinksGrid.MenuManager = this.barManager;
		this.dataLinksGrid.Name = "dataLinksGrid";
		this.dataLinksGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemPictureEdit, this.repositoryItemCustomTextEdit, this.repositoryItemCheckEdit });
		this.dataLinksGrid.Size = new System.Drawing.Size(905, 405);
		this.dataLinksGrid.TabIndex = 13;
		this.dataLinksGrid.ToolTipController = this.dataLinksToolTipController;
		this.dataLinksGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dataLinksGridView });
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(927, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 551);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(927, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 551);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(927, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 551);
		this.dataLinksToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.objectLookUpEdit.Location = new System.Drawing.Point(37, 30);
		this.objectLookUpEdit.Name = "objectLookUpEdit";
		this.objectLookUpEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.objectLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.objectLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.objectLookUpEdit.Properties.DisplayMember = "DisplayNameFormatted";
		this.objectLookUpEdit.Properties.ImmediatePopup = true;
		this.objectLookUpEdit.Properties.NullText = "";
		this.objectLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.objectLookUpEdit.Properties.PopupView = this.objectCustomGridUserControl;
		this.objectLookUpEdit.Properties.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit });
		this.objectLookUpEdit.Properties.ShowFooter = false;
		this.objectLookUpEdit.Properties.ValueMember = "Id";
		this.objectLookUpEdit.Size = new System.Drawing.Size(877, 20);
		this.objectLookUpEdit.StyleController = this.rootLayoutControl;
		this.objectLookUpEdit.TabIndex = 19;
		this.objectLookUpEdit.TabStop = false;
		this.objectLookUpEdit.EditValueChanged += new System.EventHandler(ObjectLookUpEdit_EditValueChanged);
		this.objectLookUpEdit.Resize += new System.EventHandler(ObjectLookUpEdit_Resize);
		this.objectCustomGridUserControl.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.objectIconGridColumn, this.objectNameGridColumn });
		this.objectCustomGridUserControl.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.objectCustomGridUserControl.Name = "objectCustomGridUserControl";
		this.objectCustomGridUserControl.OptionsBehavior.AutoPopulateColumns = false;
		this.objectCustomGridUserControl.OptionsCustomization.AllowFilter = false;
		this.objectCustomGridUserControl.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.objectCustomGridUserControl.OptionsView.ShowColumnHeaders = false;
		this.objectCustomGridUserControl.OptionsView.ShowGroupPanel = false;
		this.objectCustomGridUserControl.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.objectCustomGridUserControl.OptionsView.ShowIndicator = false;
		this.objectCustomGridUserControl.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.objectCustomGridUserControl.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(ObjectCustomGridUserControl_CustomDrawCell);
		this.objectIconGridColumn.FieldName = "Image";
		this.objectIconGridColumn.MaxWidth = 19;
		this.objectIconGridColumn.MinWidth = 19;
		this.objectIconGridColumn.Name = "objectIconGridColumn";
		this.objectIconGridColumn.Visible = true;
		this.objectIconGridColumn.VisibleIndex = 0;
		this.objectIconGridColumn.Width = 19;
		this.objectNameGridColumn.Caption = "DisplayName";
		this.objectNameGridColumn.ColumnEdit = this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit;
		this.objectNameGridColumn.FieldName = "DisplayNameFormatted";
		this.objectNameGridColumn.Name = "objectNameGridColumn";
		this.objectNameGridColumn.Visible = true;
		this.objectNameGridColumn.VisibleIndex = 1;
		this.objectNameGridColumn.Width = 365;
		this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit.AutoHeight = false;
		this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit.Name = "linkableObjectFullNameFormattedRepositoryItemCustomTextEdit";
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(834, 516);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.rootLayoutControl;
		this.cancelSimpleButton.TabIndex = 16;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(CancelSimpleButton_Click);
		this.saveSimpleButton.Location = new System.Drawing.Point(738, 516);
		this.saveSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.saveSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.saveSimpleButton.StyleController = this.rootLayoutControl;
		this.saveSimpleButton.TabIndex = 15;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(SaveSimpleButton_Click);
		this.withFollowingBusinessGlossaryTermsLabelControl.AutoEllipsis = true;
		this.withFollowingBusinessGlossaryTermsLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.withFollowingBusinessGlossaryTermsLabelControl.Location = new System.Drawing.Point(13, 54);
		this.withFollowingBusinessGlossaryTermsLabelControl.Name = "withFollowingBusinessGlossaryTermsLabelControl";
		this.withFollowingBusinessGlossaryTermsLabelControl.Size = new System.Drawing.Size(901, 13);
		this.withFollowingBusinessGlossaryTermsLabelControl.StyleController = this.rootLayoutControl;
		this.withFollowingBusinessGlossaryTermsLabelControl.TabIndex = 22;
		this.withFollowingBusinessGlossaryTermsLabelControl.Text = "With following Business Glossary terms";
		this.rootLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.rootLayoutControlGroup.GroupBordersVisible = false;
		this.rootLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
		{
			this.leftButtonsEmptySpaceItem, this.saveLayoutControlItem, this.cancelLayoutControlItem, this.beetwenButtonsEmptySpaceItem, this.bottomAboveButtonsEmptySpaceItem, this.objectLayoutControlItem, this.gridPanelLayoutControlItem, this.termsLayoutControlItem, this.iconLayoutControlItem, this.linkDataElementLayoutControlItem,
			this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem
		});
		this.rootLayoutControlGroup.Name = "Root";
		this.rootLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(11, 11, 11, 11);
		this.rootLayoutControlGroup.Size = new System.Drawing.Size(927, 551);
		this.rootLayoutControlGroup.TextVisible = false;
		this.leftButtonsEmptySpaceItem.AllowHotTrack = false;
		this.leftButtonsEmptySpaceItem.CustomizationFormText = "emptySpaceItem1";
		this.leftButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 503);
		this.leftButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.leftButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(1, 26);
		this.leftButtonsEmptySpaceItem.Name = "leftButtonsEmptySpaceItem";
		this.leftButtonsEmptySpaceItem.Size = new System.Drawing.Size(725, 26);
		this.leftButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.leftButtonsEmptySpaceItem.Text = "emptySpaceItem1";
		this.leftButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveLayoutControlItem.Control = this.saveSimpleButton;
		this.saveLayoutControlItem.Location = new System.Drawing.Point(725, 503);
		this.saveLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.Name = "saveLayoutControlItem";
		this.saveLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveLayoutControlItem.TextVisible = false;
		this.cancelLayoutControlItem.Control = this.cancelSimpleButton;
		this.cancelLayoutControlItem.Location = new System.Drawing.Point(821, 503);
		this.cancelLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.Name = "cancelLayoutControlItem2";
		this.cancelLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelLayoutControlItem.TextVisible = false;
		this.beetwenButtonsEmptySpaceItem.AllowHotTrack = false;
		this.beetwenButtonsEmptySpaceItem.Location = new System.Drawing.Point(809, 503);
		this.beetwenButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.Name = "beetwenButtonsEmptySpaceItem";
		this.beetwenButtonsEmptySpaceItem.Size = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.beetwenButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomAboveButtonsEmptySpaceItem.AllowHotTrack = false;
		this.bottomAboveButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 493);
		this.bottomAboveButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.bottomAboveButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.bottomAboveButtonsEmptySpaceItem.Name = "bottomAboveButtonsEmptySpaceItem";
		this.bottomAboveButtonsEmptySpaceItem.Size = new System.Drawing.Size(905, 10);
		this.bottomAboveButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomAboveButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.objectLayoutControlItem.Control = this.objectLookUpEdit;
		this.objectLayoutControlItem.Location = new System.Drawing.Point(24, 17);
		this.objectLayoutControlItem.Name = "objectLayoutControlItem";
		this.objectLayoutControlItem.Size = new System.Drawing.Size(881, 24);
		this.objectLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.objectLayoutControlItem.TextVisible = false;
		this.gridPanelLayoutControlItem.Control = this.gridPanelUserControl;
		this.gridPanelLayoutControlItem.Location = new System.Drawing.Point(0, 58);
		this.gridPanelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 30);
		this.gridPanelLayoutControlItem.MinSize = new System.Drawing.Size(104, 30);
		this.gridPanelLayoutControlItem.Name = "gridPanelLayoutControlItem";
		this.gridPanelLayoutControlItem.Size = new System.Drawing.Size(905, 30);
		this.gridPanelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.gridPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridPanelLayoutControlItem.TextVisible = false;
		this.termsLayoutControlItem.Control = this.dataLinksGrid;
		this.termsLayoutControlItem.CustomizationFormText = "termsLayoutControlItem";
		this.termsLayoutControlItem.Location = new System.Drawing.Point(0, 88);
		this.termsLayoutControlItem.MinSize = new System.Drawing.Size(104, 24);
		this.termsLayoutControlItem.Name = "termsLayoutControlItem";
		this.termsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.termsLayoutControlItem.Size = new System.Drawing.Size(905, 405);
		this.termsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.termsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.termsLayoutControlItem.TextVisible = false;
		this.iconLayoutControlItem.Control = this.iconPictureBox;
		this.iconLayoutControlItem.ImageOptions.ImageToTextDistance = 0;
		this.iconLayoutControlItem.Location = new System.Drawing.Point(0, 17);
		this.iconLayoutControlItem.MaxSize = new System.Drawing.Size(24, 24);
		this.iconLayoutControlItem.MinSize = new System.Drawing.Size(24, 24);
		this.iconLayoutControlItem.Name = "iconLayoutControlItem";
		this.iconLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 4, 4);
		this.iconLayoutControlItem.Size = new System.Drawing.Size(24, 24);
		this.iconLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.iconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.iconLayoutControlItem.TextVisible = false;
		this.linkDataElementLayoutControlItem.Control = this.linkDataElementLabelControl;
		this.linkDataElementLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.linkDataElementLayoutControlItem.MaxSize = new System.Drawing.Size(0, 17);
		this.linkDataElementLayoutControlItem.MinSize = new System.Drawing.Size(88, 17);
		this.linkDataElementLayoutControlItem.Name = "linkDataElementLayoutControlItem";
		this.linkDataElementLayoutControlItem.Size = new System.Drawing.Size(905, 17);
		this.linkDataElementLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.linkDataElementLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.linkDataElementLayoutControlItem.TextVisible = false;
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.Control = this.withFollowingBusinessGlossaryTermsLabelControl;
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.CustomizationFormText = "linkDataElementLayoutControlItem";
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.Location = new System.Drawing.Point(0, 41);
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.MaxSize = new System.Drawing.Size(0, 17);
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.MinSize = new System.Drawing.Size(14, 17);
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.Name = "withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem";
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.Size = new System.Drawing.Size(905, 17);
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.Text = "linkDataElementLayoutControlItem";
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem.TextVisible = false;
		this.progressPanelPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.progressPanelPanel.BackColor = System.Drawing.Color.Transparent;
		this.progressPanelPanel.Controls.Add(this.progressPanelControl);
		this.progressPanelPanel.Location = new System.Drawing.Point(313, 235);
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
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(927, 551);
		base.Controls.Add(this.progressPanelPanel);
		base.Controls.Add(this.rootLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("AddLinkToBusinessGlossaryForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "AddLinkToBusinessGlossaryForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Business Glossary links";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AddLinkToBusinessGlossaryForm_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(AddLinkToBusinessGlossaryForm_FormClosed);
		base.Shown += new System.EventHandler(AddLinkToBusinessGlossaryForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControl).EndInit();
		this.rootLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.iconPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectCustomGridUserControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkableObjectFullNameFormattedRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridPanelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkDataElementLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.withFollowingBusinessGlossaryTermsLinkDataElementLayoutControlItem).EndInit();
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
