using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.History;

public class HistoryForm : BaseXtraForm
{
	private string objectTable;

	private string objectColumn;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private GridControl historyRowsGridControl;

	private GridView historyRowsGridControlGridView;

	private RichEditUserControl historyTextRichEditControl;

	private LayoutControlGroup Root;

	private LayoutControlItem historyTextRichEditControlLayoutControlItem;

	private LayoutControlItem historyRowsGridControlLayoutControlItem;

	private SplitterItem splitterItem;

	private GridColumn valueColumn;

	private GridColumn dataColumn;

	private GridColumn userColumn;

	private GridColumn productColumn;

	private LabelControl historyTextLabelControl;

	private LayoutControlItem historyTextLabelControllLayoutControlItem;

	private SplashScreenManager splashScreenManager;

	public new string HtmlText
	{
		get
		{
			if (SkinsManager.CurrentSkin.IsDarkTheme)
			{
				return SkinsManager.DarkSkin.GetHtmlTranslatedColors(historyTextRichEditControl);
			}
			return historyTextRichEditControl.HtmlText;
		}
		set
		{
			historyTextRichEditControl.HtmlText = value;
			SetSkinStyle();
			if (SkinsManager.CurrentSkin.IsDarkTheme)
			{
				TranslateColorsIfHtmlTextForDarkSkin();
				SkinsManager.DarkSkin.SetDarkThemeBackground(historyTextRichEditControl, setForeColor: true);
			}
			else
			{
				TranslateColorsIfHtmlTextForWhiteSkin();
				SkinsManager.DefaultSkin.SetLightThemeBackground(historyTextRichEditControl, setForeColor: true);
			}
			historyTextRichEditControl.ClearUndo();
		}
	}

	public string CustomFieldCaption { get; internal set; }

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; internal set; }

	public HistoryForm()
	{
		InitializeComponent();
	}

	private void TranslateColorsIfHtmlTextForWhiteSkin()
	{
		if (HistoryGeneralHelper.IsWithHtmlDescription(objectColumn, objectTable))
		{
			SkinsManager.DefaultSkin.TranslateColors(historyTextRichEditControl);
		}
	}

	private void TranslateColorsIfHtmlTextForDarkSkin()
	{
		if (HistoryGeneralHelper.IsWithHtmlDescription(objectColumn, objectTable))
		{
			SkinsManager.DarkSkin.TranslateColors(historyTextRichEditControl);
		}
	}

	private void SetSkinStyle()
	{
		CharacterStyle characterStyle = historyTextRichEditControl.Document.CharacterStyles["Hyperlink"];
		if (characterStyle != null)
		{
			characterStyle.ForeColor = (SkinsManager.CurrentSkin.IsDarkTheme ? SkinsManager.DarkSkin.HyperlinkInRichTextForeColor : SkinsManager.DefaultSkin.HyperlinkInRichTextForeColor);
		}
	}

	public void SetParameters(int? objectId, string objectColumn, string objectName, string objectSchema, bool? showSchema, bool? schemaOverride, string objectTitle, string objectTable, string objectType, string objectSubtype, string objectSource, Bitmap objectImage)
	{
		if (!objectId.HasValue || string.IsNullOrEmpty(objectColumn) || (objectColumn != null && objectColumn.Length < 2))
		{
			Close();
			return;
		}
		historyTextRichEditControl.RefreshSkin();
		SetTextLabels(objectName, objectSchema, showSchema, schemaOverride, objectTitle, objectTable, objectColumn, objectType, objectSubtype, objectSource, objectImage);
		List<HistoryModel> list = DB.History.SelectHistoryRows(objectId.Value, objectColumn, objectTable);
		ShowHistoryTracking(list);
		if (list.Count > 0)
		{
			SetHtmlDescriptionForObjects(list[0]);
			historyRowsGridControl.DataSource = list;
		}
	}

	private void SetHtmlDescriptionForObjects(HistoryModel historyModel)
	{
		if (historyModel != null && (objectTable == "tables" || objectTable == "glossary_terms") && objectColumn == "description")
		{
			historyModel.ValueInHTMLRichText = DB.History.SelectHtmlTextForHistoryRichEditControlByChangesHistoryId(historyModel?.ChangesHistoryId);
			historyModel.IsValueInHTMLRichTextDownloaded = true;
		}
	}

	private void SetTextLabels(string objectName, string objectSchema, bool? showSchema, bool? schemaOverride, string objectTitle, string objectTable, string objectColumn, string objectType, string objectSubtype, string objectSource, Bitmap objectImage = null)
	{
		this.objectColumn = objectColumn;
		this.objectTable = objectTable;
		string text = objectName;
		if (DatabaseRow.GetShowSchema(showSchema, schemaOverride))
		{
			text = (string.IsNullOrEmpty(objectSchema) ? objectName : (objectSchema + "." + objectName));
		}
		string text2;
		if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(objectTitle))
		{
			text2 = objectTitle;
			Text = "History - " + objectTitle;
		}
		else
		{
			text2 = (string.IsNullOrEmpty(objectTitle) ? text : (text + " (" + objectTitle + ")"));
			Text = "History - " + text;
		}
		historyTextLabelControl.Text = text2;
		string toolTip = (objectSubtype ?? objectType) + ": " + text2;
		historyTextLabelControl.ToolTip = toolTip;
		SetObjectImage(objectType, objectSubtype, objectSource, objectImage);
		if (!string.IsNullOrEmpty(CustomFieldCaption))
		{
			valueColumn.Caption = CustomFieldCaption;
		}
		else if (objectColumn != null && objectColumn.Length > 1)
		{
			valueColumn.Caption = objectColumn[0].ToString().ToUpper() + objectColumn.Substring(1);
		}
	}

	private void SetObjectImage(string objectType, string objectSubtype, string objectSource, Bitmap objectImage)
	{
		if (DatabaseType.HasValue)
		{
			historyTextLabelControl.ImageOptions.Image = IconsSupport.GetDatabaseIconByName16(DatabaseType, SharedObjectTypeEnum.ObjectType.Database);
		}
		else
		{
			historyTextLabelControl.ImageOptions.Image = objectImage ?? IconsSupport.GetObjectIcon(objectType, objectSubtype, objectSource);
		}
	}

	private void ShowHistoryTracking(List<HistoryModel> historyList)
	{
		string historyCount = historyList?.Count.ToString() ?? "0";
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificHistorySize(new TrackingUserParameters(), new TrackingDataedoParameters(), historyCount), TrackingEventEnum.HistoryShow);
		});
	}

	private void HistoryRowsGridControlGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (!(historyRowsGridControlGridView.GetFocusedRow() is HistoryModel historyModel))
		{
			HtmlText = string.Empty;
			return;
		}
		if (!historyModel.IsValueInHTMLRichTextDownloaded)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			try
			{
				historyModel.ValueInHTMLRichText = DB.History.SelectHtmlTextForHistoryRichEditControlByChangesHistoryId(historyModel.ChangesHistoryId);
				historyModel.IsValueInHTMLRichTextDownloaded = true;
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			catch (Exception exception)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				GeneralExceptionHandling.Handle(exception, FindForm());
			}
		}
		HtmlText = historyModel.ValueInHTMLRichText;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void HistoryRowsGridControlGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == dataColumn)
		{
			DateTime.TryParse(e?.Value1?.ToString(), out var result);
			DateTime.TryParse(e?.Value2?.ToString(), out var result2);
			e.Result = DateTime.Compare(result, result2);
			e.Handled = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.History.HistoryForm));
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.historyTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.historyRowsGridControl = new DevExpress.XtraGrid.GridControl();
		this.historyRowsGridControlGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.valueColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.userColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.productColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.historyTextRichEditControl = new Dataedo.App.UserControls.RichEditUserControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.historyTextRichEditControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.historyRowsGridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.splitterItem = new DevExpress.XtraLayout.SplitterItem();
		this.historyTextLabelControllLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.historyRowsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.historyRowsGridControlGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.historyTextRichEditControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.historyRowsGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.historyTextLabelControllLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.historyTextLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.historyRowsGridControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.historyTextRichEditControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.BackColor = System.Drawing.Color.LightGray;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25f);
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(1048, 675);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.historyTextLabelControl.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold);
		this.historyTextLabelControl.Appearance.Options.UseFont = true;
		this.historyTextLabelControl.AutoEllipsis = true;
		this.historyTextLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.historyTextLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.table_16;
		this.historyTextLabelControl.Location = new System.Drawing.Point(2, 2);
		this.historyTextLabelControl.Name = "historyTextLabelControl";
		this.historyTextLabelControl.Size = new System.Drawing.Size(1044, 50);
		this.historyTextLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.historyTextLabelControl.TabIndex = 7;
		this.historyTextLabelControl.Text = "dbo.TableName";
		this.historyRowsGridControl.Location = new System.Drawing.Point(2, 56);
		this.historyRowsGridControl.MainView = this.historyRowsGridControlGridView;
		this.historyRowsGridControl.Name = "historyRowsGridControl";
		this.historyRowsGridControl.Size = new System.Drawing.Size(567, 617);
		this.historyRowsGridControl.TabIndex = 6;
		this.historyRowsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.historyRowsGridControlGridView });
		this.historyRowsGridControlGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.valueColumn, this.dataColumn, this.userColumn, this.productColumn });
		this.historyRowsGridControlGridView.GridControl = this.historyRowsGridControl;
		this.historyRowsGridControlGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
		this.historyRowsGridControlGridView.Name = "historyRowsGridControlGridView";
		this.historyRowsGridControlGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.historyRowsGridControlGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.historyRowsGridControlGridView.OptionsFilter.UseNewCustomFilterDialog = true;
		this.historyRowsGridControlGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.historyRowsGridControlGridView.OptionsView.ShowGroupPanel = false;
		this.historyRowsGridControlGridView.OptionsView.ShowIndicator = false;
		this.historyRowsGridControlGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(HistoryRowsGridControlGridView_FocusedRowChanged);
		this.historyRowsGridControlGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(HistoryRowsGridControlGridView_CustomColumnSort);
		this.valueColumn.Caption = "Value";
		this.valueColumn.FieldName = "ValueField";
		this.valueColumn.MinWidth = 130;
		this.valueColumn.Name = "valueColumn";
		this.valueColumn.OptionsColumn.AllowEdit = false;
		this.valueColumn.OptionsColumn.ReadOnly = true;
		this.valueColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.valueColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.valueColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.valueColumn.Visible = true;
		this.valueColumn.VisibleIndex = 0;
		this.valueColumn.Width = 130;
		this.dataColumn.Caption = "Date";
		this.dataColumn.FieldName = "DataField";
		this.dataColumn.MinWidth = 155;
		this.dataColumn.Name = "dataColumn";
		this.dataColumn.OptionsColumn.AllowEdit = false;
		this.dataColumn.OptionsColumn.ReadOnly = true;
		this.dataColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dataColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dataColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.dataColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.dataColumn.Visible = true;
		this.dataColumn.VisibleIndex = 1;
		this.dataColumn.Width = 155;
		this.userColumn.Caption = "User";
		this.userColumn.FieldName = "UserField";
		this.userColumn.MinWidth = 180;
		this.userColumn.Name = "userColumn";
		this.userColumn.OptionsColumn.AllowEdit = false;
		this.userColumn.OptionsColumn.ReadOnly = true;
		this.userColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.userColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.userColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.userColumn.Visible = true;
		this.userColumn.VisibleIndex = 2;
		this.userColumn.Width = 180;
		this.productColumn.Caption = "Product";
		this.productColumn.FieldName = "ProductField";
		this.productColumn.MinWidth = 100;
		this.productColumn.Name = "productColumn";
		this.productColumn.OptionsColumn.AllowEdit = false;
		this.productColumn.OptionsColumn.ReadOnly = true;
		this.productColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.productColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.productColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.productColumn.Visible = true;
		this.productColumn.VisibleIndex = 3;
		this.productColumn.Width = 100;
		this.historyTextRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.historyTextRichEditControl.Appearance.Text.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.historyTextRichEditControl.Appearance.Text.Options.UseFont = true;
		this.historyTextRichEditControl.IsHighlighted = false;
		this.historyTextRichEditControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.historyTextRichEditControl.Location = new System.Drawing.Point(583, 56);
		this.historyTextRichEditControl.Name = "historyTextRichEditControl";
		this.historyTextRichEditControl.OccurrencesCount = 0;
		this.historyTextRichEditControl.Options.Behavior.UseThemeFonts = false;
		this.historyTextRichEditControl.Options.DocumentCapabilities.Comments = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.historyTextRichEditControl.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
		this.historyTextRichEditControl.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
		this.historyTextRichEditControl.OriginalHtmlText = null;
		this.historyTextRichEditControl.ReadOnly = true;
		this.historyTextRichEditControl.Size = new System.Drawing.Size(463, 617);
		this.historyTextRichEditControl.TabIndex = 5;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.historyTextRichEditControlLayoutControlItem, this.historyRowsGridControlLayoutControlItem, this.splitterItem, this.historyTextLabelControllLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(1048, 675);
		this.Root.TextVisible = false;
		this.historyTextRichEditControlLayoutControlItem.Control = this.historyTextRichEditControl;
		this.historyTextRichEditControlLayoutControlItem.Location = new System.Drawing.Point(581, 54);
		this.historyTextRichEditControlLayoutControlItem.Name = "historyTextRichEditControlLayoutControlItem";
		this.historyTextRichEditControlLayoutControlItem.Size = new System.Drawing.Size(467, 621);
		this.historyTextRichEditControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.historyTextRichEditControlLayoutControlItem.TextVisible = false;
		this.historyRowsGridControlLayoutControlItem.Control = this.historyRowsGridControl;
		this.historyRowsGridControlLayoutControlItem.Location = new System.Drawing.Point(0, 54);
		this.historyRowsGridControlLayoutControlItem.Name = "historyRowsGridControlLayoutControlItem";
		this.historyRowsGridControlLayoutControlItem.Size = new System.Drawing.Size(571, 621);
		this.historyRowsGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.historyRowsGridControlLayoutControlItem.TextVisible = false;
		this.splitterItem.AllowHotTrack = true;
		this.splitterItem.Location = new System.Drawing.Point(571, 54);
		this.splitterItem.Name = "splitterItem";
		this.splitterItem.Size = new System.Drawing.Size(10, 621);
		this.historyTextLabelControllLayoutControlItem.Control = this.historyTextLabelControl;
		this.historyTextLabelControllLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.historyTextLabelControllLayoutControlItem.MaxSize = new System.Drawing.Size(0, 54);
		this.historyTextLabelControllLayoutControlItem.MinSize = new System.Drawing.Size(101, 54);
		this.historyTextLabelControllLayoutControlItem.Name = "historyTextLabelControllLayoutControlItem";
		this.historyTextLabelControllLayoutControlItem.Size = new System.Drawing.Size(1048, 54);
		this.historyTextLabelControllLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.historyTextLabelControllLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.historyTextLabelControllLayoutControlItem.TextVisible = false;
		base.Appearance.Options.UseFont = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1048, 675);
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("HistoryForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		this.MinimumSize = new System.Drawing.Size(600, 450);
		base.Name = "HistoryForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "History";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.historyRowsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.historyRowsGridControlGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.historyTextRichEditControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.historyRowsGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.historyTextLabelControllLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
