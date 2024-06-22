using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
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

namespace Dataedo.App.UserControls;

public class DocumentationsUserControl : BaseUserControl
{
	private bool hasErrors;

	private DXErrorProvider errorProvider;

	private int sourceId;

	private IEnumerable<RepositoryDocumentationItem> sourceDocumentations;

	private IEnumerable<RepositoryDocumentationItem> destinationDocumentations;

	private IContainer components;

	private LayoutControlGroup layoutControlGroup;

	private CheckEdit userObjectsCheckEdit;

	private CheckEdit descriptionsCheckEdit;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlItem layoutControlItem7;

	private GridControl gridControl;

	private GridView gridView;

	private LayoutControlItem layoutControlItem1;

	private GridColumn destinationObjectGridColumn;

	private GridColumn isSelectedGridColumn;

	private GridColumn nameGridColumn;

	private RepositoryItemGridLookUpEdit repositoryItemGridLookUpEdit1;

	private GridView repositoryItemGridLookUpEdit1View;

	private GridColumn titleGridColumn;

	private NonCustomizableLayoutControl layoutControl;

	private CustomTextEdit copyObjectsCustomTextEdit;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem emptySpaceItem1;

	private ToolTipControllerUserControl toolTipController;

	private HelpIconUserControl helpIconUserControl;

	private LayoutControlItem helpIconLayoutControlItem;

	public Action<string, string> RedrawLabel { get; set; }

	public Action<bool> SetControls { get; set; }

	public bool IsCopyingInCurrentRepository => StaticData.Database.Equals(DestinationRepository);

	public bool HasFinished { get; private set; }

	public IEnumerable<int> ModifiedDatabases { get; set; }

	public BindingList<CopyDocumentationModel> Documentations => gridControl.DataSource as BindingList<CopyDocumentationModel>;

	public string DestinationRepository { get; set; }

	public DocumentationsUserControl()
	{
		InitializeComponent();
		errorProvider = new DXErrorProvider();
	}

	public void SetParameters(int sourceId, string destinationRepository)
	{
		this.sourceId = sourceId;
		DestinationRepository = destinationRepository;
		sourceDocumentations = (from x in RepositoriesDBHelper.GetRepositoryDocumentations(StaticData.Database)
			orderby x.Title
			select x).ToList();
		destinationDocumentations = (from x in RepositoriesDBHelper.GetRepositoryDocumentations(destinationRepository)
			orderby x.Title
			select x).ToList();
		SetGridControlDataSource();
		repositoryItemGridLookUpEdit1.DataSource = destinationDocumentations;
		repositoryItemGridLookUpEdit1.View.GridControl.BindingContext = new BindingContext();
		repositoryItemGridLookUpEdit1.View.GridControl.DataSource = destinationDocumentations;
		repositoryItemGridLookUpEdit1.View.GridControl.ForceInitialize();
		SetGridControlDefaultValues();
		destinationObjectGridColumn.Width = gridControl.Width - 20;
		int num3 = (nameGridColumn.MaxWidth = (destinationObjectGridColumn.MaxWidth = (gridControl.Width - isSelectedGridColumn.Width - 20) / 2));
		num3 = (nameGridColumn.MinWidth = (destinationObjectGridColumn.MinWidth = (gridControl.Width - isSelectedGridColumn.Width - 20) / 2));
		num3 = (nameGridColumn.Width = (destinationObjectGridColumn.Width = (gridControl.Width - isSelectedGridColumn.Width - 20) / 2));
		repositoryItemGridLookUpEdit1.PopupFormMinSize = new Size(destinationObjectGridColumn.MaxWidth - 3, 18 * destinationDocumentations.Count());
		repositoryItemGridLookUpEdit1.PopupFormSize = new Size(destinationObjectGridColumn.MaxWidth - 3, 18 * destinationDocumentations.Count());
		gridView.Columns.FirstOrDefault((GridColumn x) => x.FieldName.Equals("Title")).Caption = "Source: " + StaticData.Database;
		gridView.Columns.FirstOrDefault((GridColumn x) => x.FieldName.Equals("DestinationDocumentationId")).Caption = "Destination: " + DestinationRepository;
	}

	public void SetErrors()
	{
		errorProvider.SetError(copyObjectsCustomTextEdit, "Choose at least one option to continue.", ErrorType.Critical);
	}

	private void SetGridControlDataSource()
	{
		BindingList<CopyDocumentationModel> bindingList = new BindingList<CopyDocumentationModel>();
		if (IsCopyingInCurrentRepository)
		{
			SetCurrentRepositoryGridDataSource(bindingList);
		}
		else
		{
			SetAnotherRepositoryDataSource(bindingList);
		}
		gridControl.DataSource = bindingList;
	}

	private void SetAnotherRepositoryDataSource(BindingList<CopyDocumentationModel> dataSource)
	{
		foreach (RepositoryDocumentationItem documentation in sourceDocumentations)
		{
			int destinationDatabaseId = destinationDocumentations.FirstOrDefault((RepositoryDocumentationItem x) => x.Title.Equals(documentation.Title))?.DatabaseId ?? 0;
			dataSource.Add(new CopyDocumentationModel(documentation.Title, documentation.DatabaseId, destinationDatabaseId, documentation.Type, documentation.DBMSVersion));
		}
	}

	private void SetCurrentRepositoryGridDataSource(BindingList<CopyDocumentationModel> dataSource)
	{
		foreach (RepositoryDocumentationItem sourceDocumentation in sourceDocumentations)
		{
			dataSource.Add(new CopyDocumentationModel(sourceDocumentation.Title, sourceDocumentation.DatabaseId, 0, sourceDocumentation.Type, sourceDocumentation.DBMSVersion));
		}
	}

	private void SetGridControlDefaultValues()
	{
		int num = 0;
		foreach (RepositoryDocumentationItem documentation in sourceDocumentations)
		{
			if (IsCopyingInCurrentRepository)
			{
				gridView.SetRowCellValue(num, destinationObjectGridColumn, -1);
			}
			else if (destinationDocumentations.Any((RepositoryDocumentationItem x) => x.Title.Equals(documentation.Title)))
			{
				RepositoryDocumentationItem repositoryDocumentationItem = destinationDocumentations.FirstOrDefault((RepositoryDocumentationItem x) => x.Title.Equals(documentation.Title));
				gridView.SetRowCellValue(num, destinationObjectGridColumn, repositoryDocumentationItem.DatabaseId);
			}
			else
			{
				gridView.SetRowCellValue(num, destinationObjectGridColumn, -1);
			}
			num++;
		}
	}

	public bool IsDataValid()
	{
		ValidateData();
		return Documentations.Where((CopyDocumentationModel x) => x.IsChecked).All((CopyDocumentationModel x) => x.IsValid);
	}

	public bool HasAnyItemSelected()
	{
		return Documentations.Count((CopyDocumentationModel x) => x.IsChecked) > 0;
	}

	public bool HasSelectedAnyObject()
	{
		if (GetDescriptionsParameter() != 1)
		{
			return GetUserObjectsParameter() == 1;
		}
		return true;
	}

	public void ValidateData()
	{
		for (int i = 0; i < gridView.RowCount; i++)
		{
			CopyDocumentationModel copyDocumentationModel = gridView.GetRow(i) as CopyDocumentationModel;
			if (copyDocumentationModel.IsChecked)
			{
				copyDocumentationModel.IsValid = copyDocumentationModel.DestinationDocumentationId > 0;
			}
		}
		gridView.RefreshData();
	}

	public void CopyDocumentations()
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.ExportRun);
		});
		DateTime exportStartTime = DateTime.UtcNow;
		new TaskFactory().StartNew(delegate
		{
			try
			{
				foreach (CopyDocumentationModel item in Documentations.Where((CopyDocumentationModel x) => x.IsChecked))
				{
					StaticData.CrashedDatabaseType = SharedDatabaseTypeEnum.StringToType(item.Type);
					StaticData.CrashedDBMSVersion = item.DBMSVersion;
					RedrawLabel?.Invoke(item.Title, destinationDocumentations.FirstOrDefault((RepositoryDocumentationItem x) => x.DatabaseId == item.DestinationDocumentationId).Title);
					DB.Database.CopyDocumentation(item.DatabaseId, item.DestinationDocumentationId, StaticData.Database, DestinationRepository, GetDescriptionsParameter(), GetUserObjectsParameter());
				}
			}
			catch (Exception exception)
			{
				hasErrors = true;
				GeneralExceptionHandling.Handle(exception, "Error while copying documentation", FindForm());
			}
			finally
			{
				StaticData.ClearDatabaseInfoForCrashes();
			}
		}).ContinueWith(delegate
		{
			double duration = (DateTime.UtcNow - exportStartTime).TotalSeconds;
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderWithEventSpecificTime(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters(), duration.ToString()), (!hasErrors) ? TrackingEventEnum.ExportFinished : TrackingEventEnum.ExportFailed);
			});
			SetControls?.Invoke(hasErrors);
		});
		HasFinished = !hasErrors;
	}

	public int GetDescriptionsParameter()
	{
		if (!descriptionsCheckEdit.Checked)
		{
			return 0;
		}
		return 1;
	}

	public int GetUserObjectsParameter()
	{
		if (!userObjectsCheckEdit.Checked)
		{
			return 0;
		}
		return 1;
	}

	private void gridView_MouseDown(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.Location);
		if (gridHitInfo.InRow && gridHitInfo.Column.RealColumnEdit is RepositoryItemCheckEdit)
		{
			gridView.FocusedColumn = gridHitInfo.Column;
			gridView.FocusedRowHandle = gridHitInfo.RowHandle;
			gridView.ShowEditor();
			_ = gridView.ActiveEditor is CheckEdit;
		}
	}

	private void repositoryItemGridLookUpEdit1View_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (IsCopyingInCurrentRepository)
		{
			CopyDocumentationModel copyDocumentationModel = gridView.GetRow(gridView.FocusedRowHandle) as CopyDocumentationModel;
			if (e.CellValue.Equals(copyDocumentationModel.Title) && (repositoryItemGridLookUpEdit1View.GetRow(e.RowHandle) as RepositoryDocumentationItem).DatabaseId == copyDocumentationModel.DatabaseId)
			{
				e.Appearance.ForeColor = Color.Gray;
			}
		}
	}

	private void repositoryItemGridLookUpEdit1_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (IsCopyingInCurrentRepository)
		{
			CopyDocumentationModel copyDocumentationModel = gridView.GetRow(gridView.FocusedRowHandle) as CopyDocumentationModel;
			if ((int)e.NewValue == copyDocumentationModel.DatabaseId)
			{
				e.Cancel = true;
			}
		}
	}

	private void gridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		if (e.HitInfo.InColumnPanel)
		{
			for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
			{
				e.Menu.Items.RemoveAt(num);
			}
		}
	}

	private void checkEdit_CheckedChanged(object sender, EventArgs e)
	{
		errorProvider.ClearErrors();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.DocumentationsUserControl));
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.toolTipController = new Dataedo.App.UserControls.ToolTipControllerUserControl(this.components);
		this.copyObjectsCustomTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.isSelectedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.destinationObjectGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemGridLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.titleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.userObjectsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.descriptionsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.helpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.helpIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.copyObjectsCustomTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.userObjectsCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionsCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.helpIconUserControl);
		this.layoutControl.Controls.Add(this.copyObjectsCustomTextEdit);
		this.layoutControl.Controls.Add(this.gridControl);
		this.layoutControl.Controls.Add(this.userObjectsCheckEdit);
		this.layoutControl.Controls.Add(this.descriptionsCheckEdit);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2482, 184, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(615, 329);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.copyObjectsCustomTextEdit.EditValue = "Copy settings:";
		this.copyObjectsCustomTextEdit.Location = new System.Drawing.Point(12, 248);
		this.copyObjectsCustomTextEdit.Name = "copyObjectsCustomTextEdit";
		this.copyObjectsCustomTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.copyObjectsCustomTextEdit.Properties.ReadOnly = true;
		this.copyObjectsCustomTextEdit.Size = new System.Drawing.Size(591, 18);
		this.copyObjectsCustomTextEdit.StyleController = this.layoutControl;
		this.copyObjectsCustomTextEdit.TabIndex = 14;
		this.gridControl.Location = new System.Drawing.Point(12, 12);
		this.gridControl.MainView = this.gridView;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemGridLookUpEdit1 });
		this.gridControl.Size = new System.Drawing.Size(591, 232);
		this.gridControl.TabIndex = 13;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView });
		this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.isSelectedGridColumn, this.nameGridColumn, this.destinationObjectGridColumn });
		this.gridView.GridControl = this.gridControl;
		this.gridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
		this.gridView.Name = "gridView";
		this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.gridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.gridView.OptionsCustomization.AllowColumnMoving = false;
		this.gridView.OptionsCustomization.AllowFilter = false;
		this.gridView.OptionsCustomization.AllowGroup = false;
		this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.gridView.OptionsCustomization.AllowRowSizing = true;
		this.gridView.OptionsCustomization.AllowSort = false;
		this.gridView.OptionsDetail.EnableMasterViewMode = false;
		this.gridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.gridView.OptionsNavigation.AutoFocusNewRow = true;
		this.gridView.OptionsSelection.MultiSelect = true;
		this.gridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gridView.OptionsView.ColumnAutoWidth = false;
		this.gridView.OptionsView.RowAutoHeight = true;
		this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gridView.OptionsView.ShowGroupPanel = false;
		this.gridView.OptionsView.ShowIndicator = false;
		this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_PopupMenuShowing);
		this.gridView.MouseDown += new System.Windows.Forms.MouseEventHandler(gridView_MouseDown);
		this.isSelectedGridColumn.Caption = " ";
		this.isSelectedGridColumn.FieldName = "IsChecked";
		this.isSelectedGridColumn.Name = "isSelectedGridColumn";
		this.isSelectedGridColumn.Visible = true;
		this.isSelectedGridColumn.VisibleIndex = 0;
		this.isSelectedGridColumn.Width = 20;
		this.nameGridColumn.Caption = "Source";
		this.nameGridColumn.FieldName = "Title";
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsColumn.ReadOnly = true;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 1;
		this.nameGridColumn.Width = 200;
		this.destinationObjectGridColumn.Caption = "Destination";
		this.destinationObjectGridColumn.ColumnEdit = this.repositoryItemGridLookUpEdit1;
		this.destinationObjectGridColumn.FieldName = "DestinationDocumentationId";
		this.destinationObjectGridColumn.MaxWidth = 200;
		this.destinationObjectGridColumn.MinWidth = 200;
		this.destinationObjectGridColumn.Name = "destinationObjectGridColumn";
		this.destinationObjectGridColumn.Visible = true;
		this.destinationObjectGridColumn.VisibleIndex = 2;
		this.destinationObjectGridColumn.Width = 200;
		this.repositoryItemGridLookUpEdit1.AutoHeight = false;
		this.repositoryItemGridLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemGridLookUpEdit1.DisplayMember = "Title";
		this.repositoryItemGridLookUpEdit1.Name = "repositoryItemGridLookUpEdit1";
		this.repositoryItemGridLookUpEdit1.NullText = "";
		this.repositoryItemGridLookUpEdit1.PopupView = this.repositoryItemGridLookUpEdit1View;
		this.repositoryItemGridLookUpEdit1.ShowFooter = false;
		this.repositoryItemGridLookUpEdit1.ValueMember = "DatabaseId";
		this.repositoryItemGridLookUpEdit1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(repositoryItemGridLookUpEdit1_EditValueChanging);
		this.repositoryItemGridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.titleGridColumn });
		this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
		this.repositoryItemGridLookUpEdit1View.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemGridLookUpEdit1View.OptionsBehavior.Editable = false;
		this.repositoryItemGridLookUpEdit1View.OptionsBehavior.ReadOnly = true;
		this.repositoryItemGridLookUpEdit1View.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemGridLookUpEdit1View.OptionsCustomization.AllowColumnMoving = false;
		this.repositoryItemGridLookUpEdit1View.OptionsCustomization.AllowColumnResizing = false;
		this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowColumnHeaders = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowIndicator = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemGridLookUpEdit1View.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(repositoryItemGridLookUpEdit1View_RowCellStyle);
		this.titleGridColumn.FieldName = "Title";
		this.titleGridColumn.Name = "titleGridColumn";
		this.titleGridColumn.OptionsColumn.AllowEdit = false;
		this.titleGridColumn.OptionsColumn.ReadOnly = true;
		this.titleGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleGridColumn.Tag = "FIT_WIDTH";
		this.titleGridColumn.Visible = true;
		this.titleGridColumn.VisibleIndex = 0;
		this.titleGridColumn.Width = 200;
		this.userObjectsCheckEdit.Location = new System.Drawing.Point(13, 297);
		this.userObjectsCheckEdit.Name = "userObjectsCheckEdit";
		this.userObjectsCheckEdit.Properties.Caption = "Synchronize user-defined objects (will remove objects missing from source documentation)";
		this.userObjectsCheckEdit.Size = new System.Drawing.Size(455, 20);
		this.userObjectsCheckEdit.StyleController = this.layoutControl;
		this.userObjectsCheckEdit.TabIndex = 12;
		this.userObjectsCheckEdit.CheckedChanged += new System.EventHandler(checkEdit_CheckedChanged);
		this.descriptionsCheckEdit.Location = new System.Drawing.Point(13, 271);
		this.descriptionsCheckEdit.Name = "descriptionsCheckEdit";
		this.descriptionsCheckEdit.Properties.Caption = "Copy descriptions, custom field values and classifications for matching objects";
		this.descriptionsCheckEdit.Size = new System.Drawing.Size(589, 20);
		this.descriptionsCheckEdit.StyleController = this.layoutControl;
		this.descriptionsCheckEdit.TabIndex = 11;
		this.descriptionsCheckEdit.CheckedChanged += new System.EventHandler(checkEdit_CheckedChanged);
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.layoutControlItem6, this.layoutControlItem7, this.layoutControlItem1, this.layoutControlItem2, this.emptySpaceItem1, this.helpIconLayoutControlItem });
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Size = new System.Drawing.Size(615, 329);
		this.layoutControlGroup.TextVisible = false;
		this.layoutControlItem6.Control = this.descriptionsCheckEdit;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 258);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
		this.layoutControlItem6.Size = new System.Drawing.Size(595, 26);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem7.Control = this.userObjectsCheckEdit;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 284);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(461, 25);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(461, 25);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
		this.layoutControlItem7.Size = new System.Drawing.Size(461, 25);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem1.Control = this.gridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(104, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(595, 236);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.copyObjectsCustomTextEdit;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 236);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(595, 22);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(485, 284);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 25);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 25);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(110, 25);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconUserControl.BackColor = System.Drawing.Color.Transparent;
		this.helpIconUserControl.Location = new System.Drawing.Point(473, 296);
		this.helpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.Name = "helpIconUserControl";
		this.helpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.TabIndex = 31;
		this.helpIconUserControl.ToolTipHeader = null;
		this.helpIconUserControl.ToolTipText = resources.GetString("helpIconUserControl.ToolTipText");
		this.helpIconLayoutControlItem.Control = this.helpIconUserControl;
		this.helpIconLayoutControlItem.Location = new System.Drawing.Point(461, 284);
		this.helpIconLayoutControlItem.Name = "helpIconLayoutControlItem";
		this.helpIconLayoutControlItem.Size = new System.Drawing.Size(24, 25);
		this.helpIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Name = "DocumentationsUserControl";
		base.Size = new System.Drawing.Size(615, 329);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.copyObjectsCustomTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.userObjectsCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionsCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
