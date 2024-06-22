using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.History;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Data.Procedures.Procedures;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.UI;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls;

public class HtmlUserControl : BaseUserControl
{
	private bool lockNextHtmlRichEditControlChange;

	private bool lastAvailableValue;

	private bool lastIsDescriptionProgressActive;

	private Action progressValueChanged;

	private Action isEditorFocused;

	private bool isChanged;

	private string[] lastSearchWords;

	private IContainer components;

	private BarManager htmlBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ClipboardBar clipboardBar;

	private PasteItem pasteItem;

	private CutItem cutItem;

	private CopyItem copyItem;

	private PasteSpecialItem pasteSpecialItem;

	private FontBar fontBar;

	private ChangeFontNameItem changeFontNameItem;

	private RepositoryItemFontEdit repositoryItemFontEdit;

	private ChangeFontSizeItem changeFontSizeItem;

	private RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit;

	private FontSizeIncreaseItem fontSizeIncreaseItem;

	private FontSizeDecreaseItem fontSizeDecreaseItem;

	private ToggleFontBoldItem toggleFontBoldItem;

	private ToggleFontItalicItem toggleFontItalicItem;

	private ToggleFontUnderlineItem toggleFontUnderlineItem;

	private ToggleFontDoubleUnderlineItem toggleFontDoubleUnderlineItem;

	private ToggleFontStrikeoutItem toggleFontStrikeoutItem;

	private ToggleFontDoubleStrikeoutItem toggleFontDoubleStrikeoutItem;

	private ToggleFontSuperscriptItem toggleFontSuperscriptItem;

	private ToggleFontSubscriptItem toggleFontSubscriptItem;

	private ChangeFontColorItem changeFontColorItem;

	private ChangeFontBackColorItem changeFontBackColorItem;

	private ChangeTextCaseItem changeTextCaseItem;

	private MakeTextUpperCaseItem makeTextUpperCaseItem;

	private MakeTextLowerCaseItem makeTextLowerCaseItem;

	private ToggleTextCaseItem toggleTextCaseItem;

	private ClearFormattingItem clearFormattingItem;

	private ShowFontFormItem showFontFormItem;

	private ParagraphBar paragraphBar;

	private ToggleBulletedListItem toggleBulletedListItem;

	private ToggleNumberingListItem toggleNumberingListItem;

	private ToggleMultiLevelListItem toggleMultiLevelListItem;

	private DecreaseIndentItem decreaseIndentItem;

	private IncreaseIndentItem increaseIndentItem;

	private ToggleParagraphAlignmentLeftItem toggleParagraphAlignmentLeftItem;

	private ToggleParagraphAlignmentCenterItem toggleParagraphAlignmentCenterItem;

	private ToggleParagraphAlignmentRightItem toggleParagraphAlignmentRightItem;

	private ToggleParagraphAlignmentJustifyItem toggleParagraphAlignmentJustifyItem;

	private ToggleShowWhitespaceItem toggleShowWhitespaceItem;

	private ChangeParagraphLineSpacingItem changeParagraphLineSpacingItem;

	private SetSingleParagraphSpacingItem setSingleParagraphSpacingItem;

	private SetSesquialteralParagraphSpacingItem setSesquialteralParagraphSpacingItem;

	private SetDoubleParagraphSpacingItem setDoubleParagraphSpacingItem;

	private ShowLineSpacingFormItem showLineSpacingFormItem;

	private AddSpacingBeforeParagraphItem addSpacingBeforeParagraphItem;

	private RemoveSpacingBeforeParagraphItem removeSpacingBeforeParagraphItem;

	private AddSpacingAfterParagraphItem addSpacingAfterParagraphItem;

	private RemoveSpacingAfterParagraphItem removeSpacingAfterParagraphItem;

	private ChangeParagraphBackColorItem changeParagraphBackColorItem;

	private ShowParagraphFormItem showParagraphFormItem;

	private ChangeStyleItem changeStyleItem;

	private RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit;

	private ShowEditStyleFormItem showEditStyleFormItem;

	private EditingBar editingBar;

	private FindItem findItem;

	private ReplaceItem replaceItem;

	private RichEditBarController htmlRichEditBarController;

	private RichEditControl htmlRichEditControl;

	private ChangeTableBorderLineStyleItem changeTableBorderLineStyleItem;

	private RepositoryItemBorderLineStyle repositoryItemBorderLineStyle;

	private ChangeTableBorderLineWeightItem changeTableBorderLineWeightItem;

	private RepositoryItemBorderLineWeight repositoryItemBorderLineWeight;

	private ChangeTableBorderColorItem changeTableBorderColorItem;

	private ChangeTableBordersItem changeTableBordersItem;

	private ToggleTableCellsBottomBorderItem toggleTableCellsBottomBorderItem;

	private ToggleTableCellsTopBorderItem toggleTableCellsTopBorderItem;

	private ToggleTableCellsLeftBorderItem toggleTableCellsLeftBorderItem;

	private ToggleTableCellsRightBorderItem toggleTableCellsRightBorderItem;

	private ResetTableCellsAllBordersItem resetTableCellsAllBordersItem;

	private ToggleTableCellsAllBordersItem toggleTableCellsAllBordersItem;

	private ToggleTableCellsOutsideBorderItem toggleTableCellsOutsideBorderItem;

	private ToggleTableCellsInsideBorderItem toggleTableCellsInsideBorderItem;

	private ToggleTableCellsInsideHorizontalBorderItem toggleTableCellsInsideHorizontalBorderItem;

	private ToggleTableCellsInsideVerticalBorderItem toggleTableCellsInsideVerticalBorderItem;

	private ToggleShowTableGridLinesItem toggleShowTableGridLinesItem;

	private ChangeTableCellsShadingItem changeTableCellsShadingItem;

	private SelectTableElementsItem selectTableElementsItem;

	private SelectTableCellItem selectTableCellItem;

	private SelectTableColumnItem selectTableColumnItem;

	private SelectTableRowItem selectTableRowItem;

	private SelectTableItem selectTableItem;

	private ShowTablePropertiesFormItem showTablePropertiesFormItem;

	private DeleteTableElementsItem deleteTableElementsItem;

	private ShowDeleteTableCellsFormItem showDeleteTableCellsFormItem;

	private DeleteTableColumnsItem deleteTableColumnsItem;

	private DeleteTableRowsItem deleteTableRowsItem;

	private DeleteTableItem deleteTableItem;

	private InsertTableRowAboveItem insertTableRowAboveItem;

	private InsertTableRowBelowItem insertTableRowBelowItem;

	private InsertTableColumnToLeftItem insertTableColumnToLeftItem;

	private InsertTableColumnToRightItem insertTableColumnToRightItem;

	private ShowInsertTableCellsFormItem showInsertTableCellsFormItem;

	private MergeTableCellsItem mergeTableCellsItem;

	private ShowSplitTableCellsForm showSplitTableCellsForm;

	private SplitTableItem splitTableItem;

	private ToggleTableAutoFitItem toggleTableAutoFitItem;

	private ToggleTableAutoFitContentsItem toggleTableAutoFitContentsItem;

	private ToggleTableAutoFitWindowItem toggleTableAutoFitWindowItem;

	private ToggleTableFixedColumnWidthItem toggleTableFixedColumnWidthItem;

	private ToggleTableCellsTopLeftAlignmentItem toggleTableCellsTopLeftAlignmentItem;

	private ToggleTableCellsMiddleLeftAlignmentItem toggleTableCellsMiddleLeftAlignmentItem;

	private ToggleTableCellsBottomLeftAlignmentItem toggleTableCellsBottomLeftAlignmentItem;

	private ToggleTableCellsTopCenterAlignmentItem toggleTableCellsTopCenterAlignmentItem;

	private ToggleTableCellsMiddleCenterAlignmentItem toggleTableCellsMiddleCenterAlignmentItem;

	private ToggleTableCellsBottomCenterAlignmentItem toggleTableCellsBottomCenterAlignmentItem;

	private ToggleTableCellsTopRightAlignmentItem toggleTableCellsTopRightAlignmentItem;

	private ToggleTableCellsMiddleRightAlignmentItem toggleTableCellsMiddleRightAlignmentItem;

	private ToggleTableCellsBottomRightAlignmentItem toggleTableCellsBottomRightAlignmentItem;

	private ShowTableOptionsFormItem showTableOptionsFormItem;

	private InsertPageBreakItem2 insertPageBreakItem2;

	private InsertPictureItem insertPictureItem;

	private InsertFloatingPictureItem insertFloatingPictureItem;

	private InsertBookmarkItem insertBookmarkItem;

	private InsertHyperlinkItem insertHyperlinkItem;

	private EditPageHeaderItem editPageHeaderItem;

	private EditPageFooterItem editPageFooterItem;

	private InsertPageNumberItem insertPageNumberItem;

	private InsertPageCountItem insertPageCountItem;

	private InsertTextBoxItem insertTextBoxItem;

	private InsertSymbolItem insertSymbolItem;

	private ToggleFirstRowItem toggleFirstRowItem;

	private ToggleLastRowItem toggleLastRowItem;

	private ToggleBandedRowsItem toggleBandedRowsItem;

	private ToggleFirstColumnItem toggleFirstColumnItem;

	private ToggleLastColumnItem toggleLastColumnItem;

	private ToggleBandedColumnsItem toggleBandedColumnsItem;

	private GalleryChangeTableStyleItem galleryChangeTableStyleItem;

	private TablesBar tablesBar;

	private InsertTableItem insertTableItem;

	private RepositoryItemFontEdit repositoryItemFontEdit1;

	private RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit1;

	private RepositoryItemBorderLineStyle repositoryItemBorderLineStyle1;

	private RepositoryItemBorderLineWeight repositoryItemBorderLineWeight1;

	private RepositoryItemRichEditStyleEdit repositoryItemRichEditStyleEdit1;

	private BarAndDockingController barAndDockingController;

	private BarCheckItem lightThemeBarCheckItem;

	private SpellChecker spellChecker;

	public bool IsDarkTheme => !SkinsManager.IsFormattedFieldsLightSkinEnabled;

	public bool CanListen { get; set; }

	public string OriginalHtmlText { get; set; }

	public string HtmlText
	{
		get
		{
			bool canListen = CanListen;
			try
			{
				CanListen = false;
				if (IsDarkTheme)
				{
					return SkinsManager.DarkSkin.GetHtmlTranslatedColors(htmlRichEditControl);
				}
				return htmlRichEditControl.HtmlText;
			}
			finally
			{
				CanListen = canListen;
			}
		}
		set
		{
			htmlRichEditControl.HtmlText = value;
			SetSkinStyle();
			OriginalHtmlText = value;
			isChanged = false;
			if (IsDarkTheme)
			{
				SkinsManager.DarkSkin.TranslateColors(htmlRichEditControl);
			}
			if (IsDarkTheme)
			{
				SkinsManager.DarkSkin.SetDarkThemeBackground(htmlRichEditControl, setForeColor: true);
			}
			else
			{
				SkinsManager.DefaultSkin.SetLightThemeBackground(htmlRichEditControl, setForeColor: true);
			}
			htmlRichEditControl.ClearUndo();
		}
	}

	public string PlainText
	{
		get
		{
			return htmlRichEditControl.Text;
		}
		set
		{
			isChanged = false;
			htmlRichEditControl.Text = value;
		}
	}

	public string PlainTextForSearch => PlainText;

	public bool IsHighlighted { get; set; }

	public int OccurrencesCount { get; set; }

	public string Occurrences
	{
		get
		{
			if (OccurrencesCount != 0)
			{
				if (OccurrencesCount <= 1)
				{
					return $"({OccurrencesCount} occurrence)";
				}
				return $"({OccurrencesCount} occurrences)";
			}
			return string.Empty;
		}
	}

	public TableObject TableRow { get; set; }

	public TermObject TermObject { get; set; }

	public DatabaseRow DatabaseRow { get; set; }

	public ModuleObject ModuleRow { get; set; }

	public ProcedureObject ProcedureObject { get; set; }

	public SplashScreenManager SplashScreenManager { get; set; }

	public event EventHandler ContentChangedEvent;

	public event Action ProgressValueChanged
	{
		add
		{
			if (progressValueChanged == null || !progressValueChanged.GetInvocationList().Contains(value))
			{
				progressValueChanged = (Action)Delegate.Combine(progressValueChanged, value);
			}
		}
		remove
		{
			progressValueChanged = (Action)Delegate.Remove(progressValueChanged, value);
		}
	}

	public event Action IsEditorFocused
	{
		add
		{
			if (isEditorFocused == null || !isEditorFocused.GetInvocationList().Contains(value))
			{
				isEditorFocused = (Action)Delegate.Combine(isEditorFocused, value);
			}
		}
		remove
		{
			isEditorFocused = (Action)Delegate.Remove(isEditorFocused, value);
		}
	}

	public bool PerformFindActionIfFocused()
	{
		if (htmlRichEditControl.Focused)
		{
			findItem.PerformClick();
			return true;
		}
		return false;
	}

	public void SetNotChanged()
	{
		isChanged = false;
	}

	public void SetHtmlTextAsOriginal()
	{
		OriginalHtmlText = HtmlText;
	}

	public HtmlUserControl()
	{
		InitializeComponent();
		CustomCommandExecutionListenerService serviceInstance = new CustomCommandExecutionListenerService(htmlRichEditControl);
		htmlRichEditControl.RemoveService(typeof(ICommandExecutionListenerService));
		htmlRichEditControl.AddService(typeof(ICommandExecutionListenerService), serviceInstance);
		htmlRichEditControl.RemoveShortcutKey(Keys.Control, Keys.O);
		Initialize();
		SetRichEditControlThemeBackground();
	}

	public void Initialize()
	{
		lightThemeBarCheckItem.Checked = !IsDarkTheme;
		TableRow = null;
		SetSkinStyle();
		SetSpellChecker();
	}

	private void SetSpellChecker()
	{
		if (htmlRichEditControl != null && spellChecker != null)
		{
			if (spellChecker.Dictionaries.Count() == 0 && SpellCheckerHelper.englishDictionary != null)
			{
				spellChecker.Dictionaries.Add(SpellCheckerHelper.englishDictionary);
			}
			RichEditControl richEditControl = htmlRichEditControl;
			LoginInfo lOGIN_INFO = LastConnectionInfo.LOGIN_INFO;
			richEditControl.SpellChecker = (((lOGIN_INFO != null && lOGIN_INFO.TurnOffCheckSpelling) || SpellCheckerHelper.englishDictionary == null) ? null : spellChecker);
		}
	}

	public void HideHtmlBarManager()
	{
		foreach (Bar bar in htmlBarManager.Bars)
		{
			bar.Visible = false;
		}
	}

	public bool Highlight()
	{
		return Highlight(lastSearchWords);
	}

	public bool Highlight(string[] searchWords)
	{
		if (!isChanged)
		{
			lastSearchWords = searchWords;
			List<DocumentRange> list = new List<DocumentRange>();
			if (searchWords != null)
			{
				foreach (string textToFind in searchWords)
				{
					DocumentRange[] array = htmlRichEditControl.Document.FindAll(textToFind, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
					if (array.Length != 0)
					{
						list.AddRange(array);
						continue;
					}
					OccurrencesCount = 0;
					return false;
				}
			}
			if (list.Count > 0)
			{
				IsHighlighted = true;
				OccurrencesCount = list.Count;
				return true;
			}
		}
		OccurrencesCount = 0;
		return false;
	}

	private void htmlRichEditControl_Enter(object sender, EventArgs e)
	{
		if (IsHighlighted)
		{
			ClearHighlights();
		}
		isEditorFocused?.Invoke();
	}

	private void htmlRichEditControl_Leave(object sender, EventArgs e)
	{
		isEditorFocused?.Invoke();
	}

	public void ClearHighlights()
	{
		IsHighlighted = false;
	}

	public void SetAvailability(bool available)
	{
		lastAvailableValue = available;
		if (available)
		{
			SetRichEditControlThemeBackground();
		}
		else if (IsDarkTheme)
		{
			htmlRichEditControl.ActiveView.BackColor = SkinsManager.DarkSkin.DisabledFunctionalitiesBackColor;
		}
		else
		{
			htmlRichEditControl.ActiveView.BackColor = SkinsManager.DefaultSkin.DisabledFunctionalitiesBackColor;
		}
	}

	public void SetFocusedColor(bool isDescriptionProgressActive)
	{
		lastIsDescriptionProgressActive = isDescriptionProgressActive;
		if (string.IsNullOrEmpty(htmlRichEditControl.Text) && isDescriptionProgressActive)
		{
			if (htmlRichEditControl.Focused)
			{
				SetRichEditControlThemeBackground();
			}
			else if (IsDarkTheme)
			{
				htmlRichEditControl.ActiveView.BackColor = SkinsManager.DarkSkin.ProgressPainterColor;
			}
			else
			{
				htmlRichEditControl.ActiveView.BackColor = SkinsManager.DefaultSkin.ProgressPainterColor;
			}
		}
	}

	public void SetEmptyProgressBackgroundColor(bool isDescriptionProgressActive)
	{
		if (isDescriptionProgressActive && string.IsNullOrEmpty(htmlRichEditControl.Text) && !htmlRichEditControl.Focused)
		{
			if (IsDarkTheme)
			{
				htmlRichEditControl.ActiveView.BackColor = SkinsManager.DarkSkin.ProgressPainterColor;
			}
			else
			{
				htmlRichEditControl.ActiveView.BackColor = SkinsManager.DefaultSkin.ProgressPainterColor;
			}
		}
		else
		{
			SetRichEditControlThemeBackground();
		}
	}

	private void htmlRichEditControl_ContentChanged(object sender, EventArgs e)
	{
		bool flag = false;
		if (!CanListen)
		{
			RichEditControl richEditControl = (RichEditControl)sender;
			DocumentModel documentModel = (DocumentModel)richEditControl.GetType().GetProperty("DocumentModel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(richEditControl, null);
			flag = documentModel.History.Items.Count > 0 && documentModel.History.Items[0].DocumentModelPart is PieceTable;
		}
		if (!lockNextHtmlRichEditControlChange && (CanListen || flag) && this.ContentChangedEvent != null)
		{
			isChanged = true;
			this.ContentChangedEvent(this, e);
			CanListen = false;
			lockNextHtmlRichEditControlChange = false;
		}
		if (lockNextHtmlRichEditControlChange)
		{
			lockNextHtmlRichEditControlChange = false;
		}
		SetSpellChecker();
	}

	private void htmlRichEditControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		CanListen = true;
	}

	private void htmlBarManager_PressedLinkChanged(object sender, HighlightedLinkChangedEventArgs e)
	{
		CanListen = true;
	}

	private void themeBarCheckItem_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		if (IsDarkTheme != !lightThemeBarCheckItem.Checked)
		{
			SkinsManager.IsFormattedFieldsLightSkinEnabled = lightThemeBarCheckItem.Checked;
			LastConnectionInfo.Save();
		}
		SetRichEditControlThemeBackground();
		CanListen = false;
		try
		{
			lockNextHtmlRichEditControlChange = true;
			htmlRichEditControl.HtmlText = SkinsManager.DarkSkin.GetHtmlTranslatedColors(htmlRichEditControl);
			SetAvailability(lastAvailableValue);
			SetFocusedColor(lastIsDescriptionProgressActive);
			SetEmptyProgressBackgroundColor(lastIsDescriptionProgressActive);
		}
		finally
		{
			CanListen = true;
		}
		SetSkinStyle();
	}

	private void SetRichEditControlThemeBackground(bool setForeColor = false)
	{
		if (IsDarkTheme)
		{
			SkinsManager.DarkSkin.SetDarkThemeBackground(htmlRichEditControl);
		}
		else
		{
			SkinsManager.DefaultSkin.SetLightThemeBackground(htmlRichEditControl);
		}
		if (setForeColor)
		{
			SetRichEditControlThemeForeColor();
		}
	}

	private void SetRichEditControlThemeForeColor()
	{
		changeFontColorItem.SelectedColor = changeFontColorItem.SelectedColor.Invert();
	}

	private void SetSkinStyle()
	{
		DevExpress.XtraRichEdit.API.Native.CharacterStyle characterStyle = htmlRichEditControl.Document.CharacterStyles["Hyperlink"];
		if (characterStyle != null)
		{
			characterStyle.ForeColor = (IsDarkTheme ? SkinsManager.DarkSkin.HyperlinkInRichTextForeColor : SkinsManager.DefaultSkin.HyperlinkInRichTextForeColor);
		}
	}

	private void HtmlRichEditControl_PopupMenuShowing(object sender, DevExpress.XtraRichEdit.PopupMenuShowingEventArgs e)
	{
		if (TableRow != null || TermObject != null || DatabaseRow != null || ModuleRow != null || ProcedureObject != null)
		{
			RichEditMenuItem item = new RichEditMenuItem("View History", ViewHistoryClicked_RichEditMenuItem, Resources.search_16);
			e.Menu.Items.Add(item);
		}
		HideCustomDictionarySpellingOptions(e);
	}

	private void HideCustomDictionarySpellingOptions(DevExpress.XtraRichEdit.PopupMenuShowingEventArgs e)
	{
		if (e == null || e.Menu == null || e.Menu.Items == null)
		{
			return;
		}
		foreach (DXMenuItem item in e.Menu.Items.Where((DXMenuItem x) => x.Caption == "Spelling" || x.Caption == "Add to Dictionary" || x.Caption == "Ignore" || x.Caption == "Ignore All").ToList())
		{
			e.Menu.Items.Remove(item);
		}
	}

	public void ViewHistoryClicked_RichEditMenuItem(object sender, EventArgs e)
	{
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: true);
			if (TableRow != null)
			{
				historyForm.SetParameters(TableRow.Id, "description", TableRow.Name, TableRow.Schema, TableRow.DatabaseDatabaseShowSchema, TableRow.DatabaseShowSchemaOverride, TableRow.Title, "tables", TableRow.ObjectType, TableRow.Subtype, TableRow.Status, null);
				CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
				historyForm.ShowDialog(FindForm());
			}
			else if (TermObject != null)
			{
				historyForm.SetParameters(TermObject.TermId, "description", TermObject.Title, null, null, null, null, "glossary_terms", TermObject.TypeTitle, null, null, BusinessGlossarySupport.GetTermIcon(TermObject.TypeIconId));
				CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
				historyForm.ShowDialog(FindForm());
			}
			else if (DatabaseRow != null)
			{
				historyForm.DatabaseType = DatabaseRow.Type;
				historyForm.SetParameters(DatabaseRow?.Id, "description", null, null, DatabaseRow.ShowSchema, DatabaseRow.ShowSchemaOverride, DatabaseRow.Title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(DatabaseRow.ObjectTypeValue), SharedObjectTypeEnum.TypeToString(DatabaseRow.ObjectTypeValue), null, null, null);
				CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
				historyForm.ShowDialog();
			}
			else if (ModuleRow != null)
			{
				historyForm.SetParameters(ModuleRow?.Id, "description", null, null, null, null, ModuleRow?.Title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Module), SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Module), null, null, null);
				CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
				historyForm.ShowDialog();
			}
			else if (ProcedureObject != null)
			{
				historyForm.SetParameters(ProcedureObject?.Id, "description", ProcedureObject.Name, ProcedureObject.Schema, ProcedureObject.DatabaseDatabaseShowSchema, ProcedureObject.DatabaseShowSchemaOverride, ProcedureObject?.Title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.StringToType(ProcedureObject.ObjectType)), ProcedureObject.ObjectType, null, null, null);
				CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
				historyForm.ShowDialog();
			}
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	internal void ClearHistoryObjects()
	{
		TableRow = null;
		TermObject = null;
		DatabaseRow = null;
		ModuleRow = null;
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
		DevExpress.XtraSpellChecker.OptionsSpelling optionsSpelling = new DevExpress.XtraSpellChecker.OptionsSpelling();
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem3 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip4 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem4 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem4 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip5 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem5 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem5 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip6 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem6 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem6 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip7 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem7 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem7 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip8 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem8 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem8 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip9 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem9 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem9 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip10 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem10 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem10 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip11 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem11 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem11 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip12 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem12 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem12 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip13 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem13 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem13 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip14 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem14 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem14 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip15 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem15 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem15 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip16 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem16 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem16 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip17 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem17 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem17 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip18 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem18 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem18 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip19 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem19 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem19 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip20 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem20 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem20 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip21 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem21 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem21 = new DevExpress.Utils.ToolTipItem();
		DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
		this.htmlBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.clipboardBar = new DevExpress.XtraRichEdit.UI.ClipboardBar();
		this.htmlRichEditControl = new DevExpress.XtraRichEdit.RichEditControl();
		this.spellChecker = new DevExpress.XtraSpellChecker.SpellChecker(this.components);
		this.pasteItem = new DevExpress.XtraRichEdit.UI.PasteItem();
		this.cutItem = new DevExpress.XtraRichEdit.UI.CutItem();
		this.copyItem = new DevExpress.XtraRichEdit.UI.CopyItem();
		this.pasteSpecialItem = new DevExpress.XtraRichEdit.UI.PasteSpecialItem();
		this.fontBar = new DevExpress.XtraRichEdit.UI.FontBar();
		this.changeFontNameItem = new DevExpress.XtraRichEdit.UI.ChangeFontNameItem();
		this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
		this.changeFontSizeItem = new DevExpress.XtraRichEdit.UI.ChangeFontSizeItem();
		this.repositoryItemRichEditFontSizeEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
		this.fontSizeIncreaseItem = new DevExpress.XtraRichEdit.UI.FontSizeIncreaseItem();
		this.fontSizeDecreaseItem = new DevExpress.XtraRichEdit.UI.FontSizeDecreaseItem();
		this.toggleFontBoldItem = new DevExpress.XtraRichEdit.UI.ToggleFontBoldItem();
		this.toggleFontItalicItem = new DevExpress.XtraRichEdit.UI.ToggleFontItalicItem();
		this.toggleFontUnderlineItem = new DevExpress.XtraRichEdit.UI.ToggleFontUnderlineItem();
		this.toggleFontDoubleUnderlineItem = new DevExpress.XtraRichEdit.UI.ToggleFontDoubleUnderlineItem();
		this.toggleFontStrikeoutItem = new DevExpress.XtraRichEdit.UI.ToggleFontStrikeoutItem();
		this.toggleFontDoubleStrikeoutItem = new DevExpress.XtraRichEdit.UI.ToggleFontDoubleStrikeoutItem();
		this.toggleFontSuperscriptItem = new DevExpress.XtraRichEdit.UI.ToggleFontSuperscriptItem();
		this.toggleFontSubscriptItem = new DevExpress.XtraRichEdit.UI.ToggleFontSubscriptItem();
		this.changeFontColorItem = new DevExpress.XtraRichEdit.UI.ChangeFontColorItem();
		this.changeFontBackColorItem = new DevExpress.XtraRichEdit.UI.ChangeFontBackColorItem();
		this.changeTextCaseItem = new DevExpress.XtraRichEdit.UI.ChangeTextCaseItem();
		this.makeTextUpperCaseItem = new DevExpress.XtraRichEdit.UI.MakeTextUpperCaseItem();
		this.makeTextLowerCaseItem = new DevExpress.XtraRichEdit.UI.MakeTextLowerCaseItem();
		this.toggleTextCaseItem = new DevExpress.XtraRichEdit.UI.ToggleTextCaseItem();
		this.clearFormattingItem = new DevExpress.XtraRichEdit.UI.ClearFormattingItem();
		this.showFontFormItem = new DevExpress.XtraRichEdit.UI.ShowFontFormItem();
		this.paragraphBar = new DevExpress.XtraRichEdit.UI.ParagraphBar();
		this.toggleBulletedListItem = new DevExpress.XtraRichEdit.UI.ToggleBulletedListItem();
		this.toggleNumberingListItem = new DevExpress.XtraRichEdit.UI.ToggleNumberingListItem();
		this.toggleMultiLevelListItem = new DevExpress.XtraRichEdit.UI.ToggleMultiLevelListItem();
		this.decreaseIndentItem = new DevExpress.XtraRichEdit.UI.DecreaseIndentItem();
		this.increaseIndentItem = new DevExpress.XtraRichEdit.UI.IncreaseIndentItem();
		this.toggleParagraphAlignmentLeftItem = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentLeftItem();
		this.toggleParagraphAlignmentCenterItem = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentCenterItem();
		this.toggleParagraphAlignmentRightItem = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentRightItem();
		this.toggleParagraphAlignmentJustifyItem = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentJustifyItem();
		this.toggleShowWhitespaceItem = new DevExpress.XtraRichEdit.UI.ToggleShowWhitespaceItem();
		this.changeParagraphLineSpacingItem = new DevExpress.XtraRichEdit.UI.ChangeParagraphLineSpacingItem();
		this.setSingleParagraphSpacingItem = new DevExpress.XtraRichEdit.UI.SetSingleParagraphSpacingItem();
		this.setSesquialteralParagraphSpacingItem = new DevExpress.XtraRichEdit.UI.SetSesquialteralParagraphSpacingItem();
		this.setDoubleParagraphSpacingItem = new DevExpress.XtraRichEdit.UI.SetDoubleParagraphSpacingItem();
		this.showLineSpacingFormItem = new DevExpress.XtraRichEdit.UI.ShowLineSpacingFormItem();
		this.addSpacingBeforeParagraphItem = new DevExpress.XtraRichEdit.UI.AddSpacingBeforeParagraphItem();
		this.removeSpacingBeforeParagraphItem = new DevExpress.XtraRichEdit.UI.RemoveSpacingBeforeParagraphItem();
		this.addSpacingAfterParagraphItem = new DevExpress.XtraRichEdit.UI.AddSpacingAfterParagraphItem();
		this.removeSpacingAfterParagraphItem = new DevExpress.XtraRichEdit.UI.RemoveSpacingAfterParagraphItem();
		this.changeParagraphBackColorItem = new DevExpress.XtraRichEdit.UI.ChangeParagraphBackColorItem();
		this.showParagraphFormItem = new DevExpress.XtraRichEdit.UI.ShowParagraphFormItem();
		this.editingBar = new DevExpress.XtraRichEdit.UI.EditingBar();
		this.findItem = new DevExpress.XtraRichEdit.UI.FindItem();
		this.replaceItem = new DevExpress.XtraRichEdit.UI.ReplaceItem();
		this.tablesBar = new DevExpress.XtraRichEdit.UI.TablesBar();
		this.insertTableItem = new DevExpress.XtraRichEdit.UI.InsertTableItem();
		this.lightThemeBarCheckItem = new DevExpress.XtraBars.BarCheckItem();
		this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.changeStyleItem = new DevExpress.XtraRichEdit.UI.ChangeStyleItem();
		this.repositoryItemRichEditStyleEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditStyleEdit();
		this.showEditStyleFormItem = new DevExpress.XtraRichEdit.UI.ShowEditStyleFormItem();
		this.insertPageBreakItem2 = new DevExpress.XtraRichEdit.UI.InsertPageBreakItem2();
		this.insertPictureItem = new DevExpress.XtraRichEdit.UI.InsertPictureItem();
		this.insertFloatingPictureItem = new DevExpress.XtraRichEdit.UI.InsertFloatingPictureItem();
		this.insertBookmarkItem = new DevExpress.XtraRichEdit.UI.InsertBookmarkItem();
		this.insertHyperlinkItem = new DevExpress.XtraRichEdit.UI.InsertHyperlinkItem();
		this.editPageHeaderItem = new DevExpress.XtraRichEdit.UI.EditPageHeaderItem();
		this.editPageFooterItem = new DevExpress.XtraRichEdit.UI.EditPageFooterItem();
		this.insertPageNumberItem = new DevExpress.XtraRichEdit.UI.InsertPageNumberItem();
		this.insertPageCountItem = new DevExpress.XtraRichEdit.UI.InsertPageCountItem();
		this.insertTextBoxItem = new DevExpress.XtraRichEdit.UI.InsertTextBoxItem();
		this.insertSymbolItem = new DevExpress.XtraRichEdit.UI.InsertSymbolItem();
		this.toggleFirstRowItem = new DevExpress.XtraRichEdit.UI.ToggleFirstRowItem();
		this.toggleLastRowItem = new DevExpress.XtraRichEdit.UI.ToggleLastRowItem();
		this.toggleBandedRowsItem = new DevExpress.XtraRichEdit.UI.ToggleBandedRowsItem();
		this.toggleFirstColumnItem = new DevExpress.XtraRichEdit.UI.ToggleFirstColumnItem();
		this.toggleLastColumnItem = new DevExpress.XtraRichEdit.UI.ToggleLastColumnItem();
		this.toggleBandedColumnsItem = new DevExpress.XtraRichEdit.UI.ToggleBandedColumnsItem();
		this.galleryChangeTableStyleItem = new DevExpress.XtraRichEdit.UI.GalleryChangeTableStyleItem();
		this.changeTableBorderLineStyleItem = new DevExpress.XtraRichEdit.UI.ChangeTableBorderLineStyleItem();
		this.repositoryItemBorderLineStyle1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle();
		this.changeTableBorderLineWeightItem = new DevExpress.XtraRichEdit.UI.ChangeTableBorderLineWeightItem();
		this.repositoryItemBorderLineWeight1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineWeight();
		this.changeTableBorderColorItem = new DevExpress.XtraRichEdit.UI.ChangeTableBorderColorItem();
		this.changeTableBordersItem = new DevExpress.XtraRichEdit.UI.ChangeTableBordersItem();
		this.toggleTableCellsBottomBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomBorderItem();
		this.toggleTableCellsTopBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopBorderItem();
		this.toggleTableCellsLeftBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsLeftBorderItem();
		this.toggleTableCellsRightBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsRightBorderItem();
		this.resetTableCellsAllBordersItem = new DevExpress.XtraRichEdit.UI.ResetTableCellsAllBordersItem();
		this.toggleTableCellsAllBordersItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsAllBordersItem();
		this.toggleTableCellsOutsideBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsOutsideBorderItem();
		this.toggleTableCellsInsideBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsInsideBorderItem();
		this.toggleTableCellsInsideHorizontalBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsInsideHorizontalBorderItem();
		this.toggleTableCellsInsideVerticalBorderItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsInsideVerticalBorderItem();
		this.toggleShowTableGridLinesItem = new DevExpress.XtraRichEdit.UI.ToggleShowTableGridLinesItem();
		this.changeTableCellsShadingItem = new DevExpress.XtraRichEdit.UI.ChangeTableCellsShadingItem();
		this.selectTableElementsItem = new DevExpress.XtraRichEdit.UI.SelectTableElementsItem();
		this.selectTableCellItem = new DevExpress.XtraRichEdit.UI.SelectTableCellItem();
		this.selectTableColumnItem = new DevExpress.XtraRichEdit.UI.SelectTableColumnItem();
		this.selectTableRowItem = new DevExpress.XtraRichEdit.UI.SelectTableRowItem();
		this.selectTableItem = new DevExpress.XtraRichEdit.UI.SelectTableItem();
		this.showTablePropertiesFormItem = new DevExpress.XtraRichEdit.UI.ShowTablePropertiesFormItem();
		this.deleteTableElementsItem = new DevExpress.XtraRichEdit.UI.DeleteTableElementsItem();
		this.showDeleteTableCellsFormItem = new DevExpress.XtraRichEdit.UI.ShowDeleteTableCellsFormItem();
		this.deleteTableColumnsItem = new DevExpress.XtraRichEdit.UI.DeleteTableColumnsItem();
		this.deleteTableRowsItem = new DevExpress.XtraRichEdit.UI.DeleteTableRowsItem();
		this.deleteTableItem = new DevExpress.XtraRichEdit.UI.DeleteTableItem();
		this.insertTableRowAboveItem = new DevExpress.XtraRichEdit.UI.InsertTableRowAboveItem();
		this.insertTableRowBelowItem = new DevExpress.XtraRichEdit.UI.InsertTableRowBelowItem();
		this.insertTableColumnToLeftItem = new DevExpress.XtraRichEdit.UI.InsertTableColumnToLeftItem();
		this.insertTableColumnToRightItem = new DevExpress.XtraRichEdit.UI.InsertTableColumnToRightItem();
		this.showInsertTableCellsFormItem = new DevExpress.XtraRichEdit.UI.ShowInsertTableCellsFormItem();
		this.mergeTableCellsItem = new DevExpress.XtraRichEdit.UI.MergeTableCellsItem();
		this.showSplitTableCellsForm = new DevExpress.XtraRichEdit.UI.ShowSplitTableCellsForm();
		this.splitTableItem = new DevExpress.XtraRichEdit.UI.SplitTableItem();
		this.toggleTableAutoFitItem = new DevExpress.XtraRichEdit.UI.ToggleTableAutoFitItem();
		this.toggleTableAutoFitContentsItem = new DevExpress.XtraRichEdit.UI.ToggleTableAutoFitContentsItem();
		this.toggleTableAutoFitWindowItem = new DevExpress.XtraRichEdit.UI.ToggleTableAutoFitWindowItem();
		this.toggleTableFixedColumnWidthItem = new DevExpress.XtraRichEdit.UI.ToggleTableFixedColumnWidthItem();
		this.toggleTableCellsTopLeftAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopLeftAlignmentItem();
		this.toggleTableCellsMiddleLeftAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsMiddleLeftAlignmentItem();
		this.toggleTableCellsBottomLeftAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomLeftAlignmentItem();
		this.toggleTableCellsTopCenterAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopCenterAlignmentItem();
		this.toggleTableCellsMiddleCenterAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsMiddleCenterAlignmentItem();
		this.toggleTableCellsBottomCenterAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomCenterAlignmentItem();
		this.toggleTableCellsTopRightAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopRightAlignmentItem();
		this.toggleTableCellsMiddleRightAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsMiddleRightAlignmentItem();
		this.toggleTableCellsBottomRightAlignmentItem = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomRightAlignmentItem();
		this.showTableOptionsFormItem = new DevExpress.XtraRichEdit.UI.ShowTableOptionsFormItem();
		this.htmlRichEditBarController = new DevExpress.XtraRichEdit.UI.RichEditBarController(this.components);
		((System.ComponentModel.ISupportInitialize)this.htmlBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemRichEditFontSizeEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barAndDockingController).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemRichEditStyleEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineStyle1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineWeight1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.htmlRichEditBarController).BeginInit();
		base.SuspendLayout();
		this.htmlBarManager.AllowCustomization = false;
		this.htmlBarManager.AllowQuickCustomization = false;
		this.htmlBarManager.Bars.AddRange(new DevExpress.XtraBars.Bar[5] { this.clipboardBar, this.fontBar, this.paragraphBar, this.editingBar, this.tablesBar });
		this.htmlBarManager.Controller = this.barAndDockingController;
		this.htmlBarManager.DockControls.Add(this.barDockControlTop);
		this.htmlBarManager.DockControls.Add(this.barDockControlBottom);
		this.htmlBarManager.DockControls.Add(this.barDockControlLeft);
		this.htmlBarManager.DockControls.Add(this.barDockControlRight);
		this.htmlBarManager.Form = this;
		this.htmlBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[118]
		{
			this.pasteItem, this.cutItem, this.copyItem, this.pasteSpecialItem, this.changeFontNameItem, this.changeFontSizeItem, this.fontSizeIncreaseItem, this.fontSizeDecreaseItem, this.toggleFontBoldItem, this.toggleFontItalicItem,
			this.toggleFontUnderlineItem, this.toggleFontDoubleUnderlineItem, this.toggleFontStrikeoutItem, this.toggleFontDoubleStrikeoutItem, this.toggleFontSuperscriptItem, this.toggleFontSubscriptItem, this.changeFontColorItem, this.changeFontBackColorItem, this.changeTextCaseItem, this.makeTextUpperCaseItem,
			this.makeTextLowerCaseItem, this.toggleTextCaseItem, this.clearFormattingItem, this.showFontFormItem, this.toggleBulletedListItem, this.toggleNumberingListItem, this.toggleMultiLevelListItem, this.decreaseIndentItem, this.increaseIndentItem, this.toggleParagraphAlignmentLeftItem,
			this.toggleParagraphAlignmentCenterItem, this.toggleParagraphAlignmentRightItem, this.toggleParagraphAlignmentJustifyItem, this.toggleShowWhitespaceItem, this.changeParagraphLineSpacingItem, this.setSingleParagraphSpacingItem, this.setSesquialteralParagraphSpacingItem, this.setDoubleParagraphSpacingItem, this.showLineSpacingFormItem, this.addSpacingBeforeParagraphItem,
			this.removeSpacingBeforeParagraphItem, this.addSpacingAfterParagraphItem, this.removeSpacingAfterParagraphItem, this.changeParagraphBackColorItem, this.showParagraphFormItem, this.changeStyleItem, this.showEditStyleFormItem, this.findItem, this.replaceItem, this.insertPageBreakItem2,
			this.insertPictureItem, this.insertFloatingPictureItem, this.insertBookmarkItem, this.insertHyperlinkItem, this.editPageHeaderItem, this.editPageFooterItem, this.insertPageNumberItem, this.insertPageCountItem, this.insertTextBoxItem, this.insertSymbolItem,
			this.toggleFirstRowItem, this.toggleLastRowItem, this.toggleBandedRowsItem, this.toggleFirstColumnItem, this.toggleLastColumnItem, this.toggleBandedColumnsItem, this.galleryChangeTableStyleItem, this.changeTableBorderLineStyleItem, this.changeTableBorderLineWeightItem, this.changeTableBorderColorItem,
			this.changeTableBordersItem, this.toggleTableCellsBottomBorderItem, this.toggleTableCellsTopBorderItem, this.toggleTableCellsLeftBorderItem, this.toggleTableCellsRightBorderItem, this.resetTableCellsAllBordersItem, this.toggleTableCellsAllBordersItem, this.toggleTableCellsOutsideBorderItem, this.toggleTableCellsInsideBorderItem, this.toggleTableCellsInsideHorizontalBorderItem,
			this.toggleTableCellsInsideVerticalBorderItem, this.toggleShowTableGridLinesItem, this.changeTableCellsShadingItem, this.selectTableElementsItem, this.selectTableCellItem, this.selectTableColumnItem, this.selectTableRowItem, this.selectTableItem, this.showTablePropertiesFormItem, this.deleteTableElementsItem,
			this.showDeleteTableCellsFormItem, this.deleteTableColumnsItem, this.deleteTableRowsItem, this.deleteTableItem, this.insertTableRowAboveItem, this.insertTableRowBelowItem, this.insertTableColumnToLeftItem, this.insertTableColumnToRightItem, this.showInsertTableCellsFormItem, this.mergeTableCellsItem,
			this.showSplitTableCellsForm, this.splitTableItem, this.toggleTableAutoFitItem, this.toggleTableAutoFitContentsItem, this.toggleTableAutoFitWindowItem, this.toggleTableFixedColumnWidthItem, this.toggleTableCellsTopLeftAlignmentItem, this.toggleTableCellsMiddleLeftAlignmentItem, this.toggleTableCellsBottomLeftAlignmentItem, this.toggleTableCellsTopCenterAlignmentItem,
			this.toggleTableCellsMiddleCenterAlignmentItem, this.toggleTableCellsBottomCenterAlignmentItem, this.toggleTableCellsTopRightAlignmentItem, this.toggleTableCellsMiddleRightAlignmentItem, this.toggleTableCellsBottomRightAlignmentItem, this.showTableOptionsFormItem, this.insertTableItem, this.lightThemeBarCheckItem
		});
		this.htmlBarManager.MaxItemId = 120;
		this.htmlBarManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemFontEdit1, this.repositoryItemRichEditFontSizeEdit1, this.repositoryItemRichEditStyleEdit1, this.repositoryItemBorderLineStyle1, this.repositoryItemBorderLineWeight1 });
		this.htmlBarManager.PressedLinkChanged += new DevExpress.XtraBars.HighlightedLinkChangedEventHandler(htmlBarManager_PressedLinkChanged);
		this.clipboardBar.Control = this.htmlRichEditControl;
		this.clipboardBar.DockCol = 0;
		this.clipboardBar.DockRow = 0;
		this.clipboardBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.clipboardBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.pasteItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.cutItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.copyItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.pasteSpecialItem)
		});
		this.clipboardBar.OptionsBar.AllowQuickCustomization = false;
		this.clipboardBar.OptionsBar.DisableCustomization = true;
		this.clipboardBar.OptionsBar.DrawDragBorder = false;
		this.htmlRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.htmlRichEditControl.Appearance.Text.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.htmlRichEditControl.Appearance.Text.Options.UseBackColor = true;
		this.htmlRichEditControl.Appearance.Text.Options.UseFont = true;
		this.htmlRichEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.htmlRichEditControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.htmlRichEditControl.Location = new System.Drawing.Point(0, 28);
		this.htmlRichEditControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.htmlRichEditControl.MenuManager = this.htmlBarManager;
		this.htmlRichEditControl.Name = "htmlRichEditControl";
		this.htmlRichEditControl.Options.Behavior.Save = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.htmlRichEditControl.Options.Behavior.SaveAs = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.htmlRichEditControl.Options.Behavior.UseThemeFonts = false;
		this.htmlRichEditControl.Options.DocumentCapabilities.Comments = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.htmlRichEditControl.Options.Export.Html.HtmlNumberingListExportFormat = DevExpress.XtraRichEdit.Export.Html.HtmlNumberingListExportFormat.PlainTextFormat;
		this.htmlRichEditControl.Options.SpellChecker.AutoDetectDocumentCulture = false;
		this.htmlRichEditControl.Size = new System.Drawing.Size(1801, 738);
		this.htmlRichEditControl.SpellChecker = this.spellChecker;
		optionsSpelling.IgnoreUpperCaseWords = DevExpress.Utils.DefaultBoolean.False;
		this.spellChecker.SetSpellCheckerOptions(this.htmlRichEditControl, optionsSpelling);
		this.htmlRichEditControl.TabIndex = 4;
		this.htmlRichEditControl.PopupMenuShowing += new DevExpress.XtraRichEdit.PopupMenuShowingEventHandler(HtmlRichEditControl_PopupMenuShowing);
		this.htmlRichEditControl.ContentChanged += new System.EventHandler(htmlRichEditControl_ContentChanged);
		this.htmlRichEditControl.Enter += new System.EventHandler(htmlRichEditControl_Enter);
		this.htmlRichEditControl.Leave += new System.EventHandler(htmlRichEditControl_Leave);
		this.htmlRichEditControl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(htmlRichEditControl_PreviewKeyDown);
		this.spellChecker.Culture = new System.Globalization.CultureInfo("en-US");
		this.spellChecker.ParentContainer = null;
		this.spellChecker.SpellCheckMode = DevExpress.XtraSpellChecker.SpellCheckMode.AsYouType;
		this.pasteItem.Id = 0;
		this.pasteItem.Name = "pasteItem";
		toolTipTitleItem.Text = "Paste (ctrl+V)";
		toolTipItem.LeftIndent = 6;
		toolTipItem.Text = "Paste the contents of the Clipboard.";
		superToolTip.Items.Add(toolTipTitleItem);
		superToolTip.Items.Add(toolTipItem);
		this.pasteItem.SuperTip = superToolTip;
		this.cutItem.Id = 1;
		this.cutItem.Name = "cutItem";
		toolTipTitleItem2.Text = "Cut (ctrl+X)";
		toolTipItem2.LeftIndent = 6;
		toolTipItem2.Text = "Cut the selection from the document and put it on the Clipboard.";
		superToolTip2.Items.Add(toolTipTitleItem2);
		superToolTip2.Items.Add(toolTipItem2);
		this.cutItem.SuperTip = superToolTip2;
		this.copyItem.Id = 2;
		this.copyItem.Name = "copyItem";
		toolTipTitleItem3.Text = "Copy (ctrl+C)";
		toolTipItem3.LeftIndent = 6;
		toolTipItem3.Text = "Copy the selection and put it on the Clipboard.";
		superToolTip3.Items.Add(toolTipTitleItem3);
		superToolTip3.Items.Add(toolTipItem3);
		this.copyItem.SuperTip = superToolTip3;
		this.pasteSpecialItem.Id = 3;
		this.pasteSpecialItem.Name = "pasteSpecialItem";
		toolTipTitleItem4.Text = "Paste Special (ctrl+Alt+V)";
		toolTipItem4.LeftIndent = 6;
		toolTipItem4.Text = "Paste Special";
		superToolTip4.Items.Add(toolTipTitleItem4);
		superToolTip4.Items.Add(toolTipItem4);
		this.pasteSpecialItem.SuperTip = superToolTip4;
		this.fontBar.Control = this.htmlRichEditControl;
		this.fontBar.DockCol = 1;
		this.fontBar.DockRow = 0;
		this.fontBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.fontBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[17]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontNameItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontSizeItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.fontSizeIncreaseItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.fontSizeDecreaseItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontBoldItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontItalicItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontUnderlineItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontDoubleUnderlineItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontStrikeoutItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontDoubleStrikeoutItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontSuperscriptItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleFontSubscriptItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontColorItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeFontBackColorItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeTextCaseItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.clearFormattingItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.showFontFormItem)
		});
		this.fontBar.OptionsBar.AllowQuickCustomization = false;
		this.fontBar.OptionsBar.DisableCustomization = true;
		this.fontBar.OptionsBar.DrawDragBorder = false;
		this.changeFontNameItem.Edit = this.repositoryItemFontEdit1;
		this.changeFontNameItem.Id = 4;
		this.changeFontNameItem.Name = "changeFontNameItem";
		this.repositoryItemFontEdit1.AutoHeight = false;
		this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
		this.changeFontSizeItem.Edit = this.repositoryItemRichEditFontSizeEdit1;
		this.changeFontSizeItem.Id = 5;
		this.changeFontSizeItem.Name = "changeFontSizeItem";
		this.repositoryItemRichEditFontSizeEdit1.AutoHeight = false;
		this.repositoryItemRichEditFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemRichEditFontSizeEdit1.Control = this.htmlRichEditControl;
		this.repositoryItemRichEditFontSizeEdit1.Name = "repositoryItemRichEditFontSizeEdit1";
		this.fontSizeIncreaseItem.Id = 6;
		this.fontSizeIncreaseItem.Name = "fontSizeIncreaseItem";
		toolTipTitleItem5.Text = "Grow Font (ctrl+Shift+.)";
		toolTipItem5.LeftIndent = 6;
		toolTipItem5.Text = "Increase the font size.";
		superToolTip5.Items.Add(toolTipTitleItem5);
		superToolTip5.Items.Add(toolTipItem5);
		this.fontSizeIncreaseItem.SuperTip = superToolTip5;
		this.fontSizeDecreaseItem.Id = 7;
		this.fontSizeDecreaseItem.Name = "fontSizeDecreaseItem";
		toolTipTitleItem6.Text = "Shrink Font (ctrl+Shift+,)";
		toolTipItem6.LeftIndent = 6;
		toolTipItem6.Text = "Decrease the font size.";
		superToolTip6.Items.Add(toolTipTitleItem6);
		superToolTip6.Items.Add(toolTipItem6);
		this.fontSizeDecreaseItem.SuperTip = superToolTip6;
		this.toggleFontBoldItem.Id = 8;
		this.toggleFontBoldItem.Name = "toggleFontBoldItem";
		toolTipTitleItem7.Text = "Bold (ctrl+B)";
		toolTipItem7.LeftIndent = 6;
		toolTipItem7.Text = "Make the selected text bold.";
		superToolTip7.Items.Add(toolTipTitleItem7);
		superToolTip7.Items.Add(toolTipItem7);
		this.toggleFontBoldItem.SuperTip = superToolTip7;
		this.toggleFontItalicItem.Id = 9;
		this.toggleFontItalicItem.Name = "toggleFontItalicItem";
		toolTipTitleItem8.Text = "Italic (ctrl+I)";
		toolTipItem8.LeftIndent = 6;
		toolTipItem8.Text = "Italicize the selected text.";
		superToolTip8.Items.Add(toolTipTitleItem8);
		superToolTip8.Items.Add(toolTipItem8);
		this.toggleFontItalicItem.SuperTip = superToolTip8;
		this.toggleFontUnderlineItem.Id = 10;
		this.toggleFontUnderlineItem.Name = "toggleFontUnderlineItem";
		toolTipTitleItem9.Text = "Underline (ctrl+U)";
		toolTipItem9.LeftIndent = 6;
		toolTipItem9.Text = "Underline the selected text.";
		superToolTip9.Items.Add(toolTipTitleItem9);
		superToolTip9.Items.Add(toolTipItem9);
		this.toggleFontUnderlineItem.SuperTip = superToolTip9;
		this.toggleFontDoubleUnderlineItem.Id = 1;
		this.toggleFontDoubleUnderlineItem.Name = "toggleFontDoubleUnderlineItem";
		toolTipTitleItem10.Text = "Double Underline (ctrl+Shift+D)";
		toolTipItem10.LeftIndent = 6;
		toolTipItem10.Text = "Double underline";
		superToolTip10.Items.Add(toolTipTitleItem10);
		superToolTip10.Items.Add(toolTipItem10);
		this.toggleFontDoubleUnderlineItem.SuperTip = superToolTip10;
		this.toggleFontStrikeoutItem.Id = 12;
		this.toggleFontStrikeoutItem.Name = "toggleFontStrikeoutItem";
		this.toggleFontDoubleStrikeoutItem.Id = 13;
		this.toggleFontDoubleStrikeoutItem.Name = "toggleFontDoubleStrikeoutItem";
		this.toggleFontSuperscriptItem.Id = 14;
		this.toggleFontSuperscriptItem.Name = "toggleFontSuperscriptItem";
		toolTipTitleItem11.Text = "Superscript (ctrl+Shift++)";
		toolTipItem11.LeftIndent = 6;
		toolTipItem11.Text = "Create small letters above the line of text.";
		superToolTip11.Items.Add(toolTipTitleItem11);
		superToolTip11.Items.Add(toolTipItem11);
		this.toggleFontSuperscriptItem.SuperTip = superToolTip11;
		this.toggleFontSubscriptItem.Id = 15;
		this.toggleFontSubscriptItem.Name = "toggleFontSubscriptItem";
		toolTipTitleItem12.Text = "Subscript (ctrl++)";
		toolTipItem12.LeftIndent = 6;
		toolTipItem12.Text = "Create small letters below the text baseline.";
		superToolTip12.Items.Add(toolTipTitleItem12);
		superToolTip12.Items.Add(toolTipItem12);
		this.toggleFontSubscriptItem.SuperTip = superToolTip12;
		this.changeFontColorItem.Id = 16;
		this.changeFontColorItem.Name = "changeFontColorItem";
		this.changeFontBackColorItem.Id = 17;
		this.changeFontBackColorItem.Name = "changeFontBackColorItem";
		this.changeTextCaseItem.Id = 18;
		this.changeTextCaseItem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.makeTextUpperCaseItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.makeTextLowerCaseItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTextCaseItem)
		});
		this.changeTextCaseItem.Name = "changeTextCaseItem";
		this.makeTextUpperCaseItem.Id = 19;
		this.makeTextUpperCaseItem.Name = "makeTextUpperCaseItem";
		this.makeTextLowerCaseItem.Id = 20;
		this.makeTextLowerCaseItem.Name = "makeTextLowerCaseItem";
		this.toggleTextCaseItem.Id = 2;
		this.toggleTextCaseItem.Name = "toggleTextCaseItem";
		this.clearFormattingItem.Id = 22;
		this.clearFormattingItem.Name = "clearFormattingItem";
		toolTipTitleItem13.Text = "Clear Formatting (ctrl+Space)";
		toolTipItem13.LeftIndent = 6;
		toolTipItem13.Text = "Clear all the formatting from the selection, leaving only plain text.";
		superToolTip13.Items.Add(toolTipTitleItem13);
		superToolTip13.Items.Add(toolTipItem13);
		this.clearFormattingItem.SuperTip = superToolTip13;
		this.showFontFormItem.Id = 23;
		this.showFontFormItem.Name = "showFontFormItem";
		toolTipTitleItem14.Text = "Font... (ctrl+D)";
		toolTipItem14.LeftIndent = 6;
		toolTipItem14.Text = "Show the Font dialog box.";
		superToolTip14.Items.Add(toolTipTitleItem14);
		superToolTip14.Items.Add(toolTipItem14);
		this.showFontFormItem.SuperTip = superToolTip14;
		this.paragraphBar.Control = this.htmlRichEditControl;
		this.paragraphBar.DockCol = 2;
		this.paragraphBar.DockRow = 0;
		this.paragraphBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.paragraphBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[13]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleBulletedListItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleNumberingListItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleMultiLevelListItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.decreaseIndentItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.increaseIndentItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentLeftItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentCenterItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentRightItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphAlignmentJustifyItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleShowWhitespaceItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphLineSpacingItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeParagraphBackColorItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.showParagraphFormItem)
		});
		this.paragraphBar.OptionsBar.AllowQuickCustomization = false;
		this.paragraphBar.OptionsBar.DisableCustomization = true;
		this.paragraphBar.OptionsBar.DrawDragBorder = false;
		this.toggleBulletedListItem.Id = 24;
		this.toggleBulletedListItem.Name = "toggleBulletedListItem";
		this.toggleNumberingListItem.Id = 25;
		this.toggleNumberingListItem.Name = "toggleNumberingListItem";
		this.toggleMultiLevelListItem.Id = 26;
		this.toggleMultiLevelListItem.Name = "toggleMultiLevelListItem";
		this.decreaseIndentItem.Id = 27;
		this.decreaseIndentItem.Name = "decreaseIndentItem";
		this.increaseIndentItem.Id = 28;
		this.increaseIndentItem.Name = "increaseIndentItem";
		this.toggleParagraphAlignmentLeftItem.Id = 29;
		this.toggleParagraphAlignmentLeftItem.Name = "toggleParagraphAlignmentLeftItem";
		toolTipTitleItem15.Text = "Align Text Left (ctrl+L)";
		toolTipItem15.LeftIndent = 6;
		toolTipItem15.Text = "Align your content with the left margin.";
		superToolTip15.Items.Add(toolTipTitleItem15);
		superToolTip15.Items.Add(toolTipItem15);
		this.toggleParagraphAlignmentLeftItem.SuperTip = superToolTip15;
		this.toggleParagraphAlignmentCenterItem.Id = 30;
		this.toggleParagraphAlignmentCenterItem.Name = "toggleParagraphAlignmentCenterItem";
		toolTipTitleItem16.Text = "Center (ctrl+E)";
		toolTipItem16.LeftIndent = 6;
		toolTipItem16.Text = "Center your content on the page.";
		superToolTip16.Items.Add(toolTipTitleItem16);
		superToolTip16.Items.Add(toolTipItem16);
		this.toggleParagraphAlignmentCenterItem.SuperTip = superToolTip16;
		this.toggleParagraphAlignmentRightItem.Id = 3;
		this.toggleParagraphAlignmentRightItem.Name = "toggleParagraphAlignmentRightItem";
		toolTipTitleItem17.Text = "Align Text Right (ctrl+R)";
		toolTipItem17.LeftIndent = 6;
		toolTipItem17.Text = "Align your content with the right margin.";
		superToolTip17.Items.Add(toolTipTitleItem17);
		superToolTip17.Items.Add(toolTipItem17);
		this.toggleParagraphAlignmentRightItem.SuperTip = superToolTip17;
		this.toggleParagraphAlignmentJustifyItem.Id = 32;
		this.toggleParagraphAlignmentJustifyItem.Name = "toggleParagraphAlignmentJustifyItem";
		toolTipTitleItem18.Text = "Justify (ctrl+J)";
		toolTipItem18.LeftIndent = 6;
		toolTipItem18.Text = "Distribute your text evenly between left and right margins. Justified text gives you document clean edges so it looks more polished.";
		superToolTip18.Items.Add(toolTipTitleItem18);
		superToolTip18.Items.Add(toolTipItem18);
		this.toggleParagraphAlignmentJustifyItem.SuperTip = superToolTip18;
		this.toggleShowWhitespaceItem.Id = 33;
		this.toggleShowWhitespaceItem.Name = "toggleShowWhitespaceItem";
		toolTipTitleItem19.Text = "Show/Hide ¶ (ctrl+Shift+8)";
		toolTipItem19.LeftIndent = 6;
		toolTipItem19.Text = "Show paragraph marks and other hidden formatting symbols.";
		superToolTip19.Items.Add(toolTipTitleItem19);
		superToolTip19.Items.Add(toolTipItem19);
		this.toggleShowWhitespaceItem.SuperTip = superToolTip19;
		this.changeParagraphLineSpacingItem.Id = 34;
		this.changeParagraphLineSpacingItem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setSingleParagraphSpacingItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSesquialteralParagraphSpacingItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.setDoubleParagraphSpacingItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.showLineSpacingFormItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.addSpacingBeforeParagraphItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeSpacingBeforeParagraphItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.addSpacingAfterParagraphItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeSpacingAfterParagraphItem)
		});
		this.changeParagraphLineSpacingItem.Name = "changeParagraphLineSpacingItem";
		this.setSingleParagraphSpacingItem.Id = 35;
		this.setSingleParagraphSpacingItem.Name = "setSingleParagraphSpacingItem";
		this.setSesquialteralParagraphSpacingItem.Id = 36;
		this.setSesquialteralParagraphSpacingItem.Name = "setSesquialteralParagraphSpacingItem";
		this.setDoubleParagraphSpacingItem.Id = 37;
		this.setDoubleParagraphSpacingItem.Name = "setDoubleParagraphSpacingItem";
		this.showLineSpacingFormItem.Id = 38;
		this.showLineSpacingFormItem.Name = "showLineSpacingFormItem";
		this.addSpacingBeforeParagraphItem.Id = 39;
		this.addSpacingBeforeParagraphItem.Name = "addSpacingBeforeParagraphItem";
		this.removeSpacingBeforeParagraphItem.Id = 40;
		this.removeSpacingBeforeParagraphItem.Name = "removeSpacingBeforeParagraphItem";
		this.addSpacingAfterParagraphItem.Id = 4;
		this.addSpacingAfterParagraphItem.Name = "addSpacingAfterParagraphItem";
		this.removeSpacingAfterParagraphItem.Id = 42;
		this.removeSpacingAfterParagraphItem.Name = "removeSpacingAfterParagraphItem";
		this.changeParagraphBackColorItem.Id = 43;
		this.changeParagraphBackColorItem.Name = "changeParagraphBackColorItem";
		this.showParagraphFormItem.Id = 44;
		this.showParagraphFormItem.Name = "showParagraphFormItem";
		this.editingBar.Control = this.htmlRichEditControl;
		this.editingBar.DockCol = 3;
		this.editingBar.DockRow = 0;
		this.editingBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.editingBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.findItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.replaceItem)
		});
		this.editingBar.OptionsBar.AllowQuickCustomization = false;
		this.editingBar.OptionsBar.DisableCustomization = true;
		this.editingBar.OptionsBar.DrawDragBorder = false;
		this.findItem.Id = 47;
		this.findItem.Name = "findItem";
		toolTipTitleItem20.Text = "Find (ctrl+F)";
		toolTipItem20.LeftIndent = 6;
		toolTipItem20.Text = "Find text in the document.";
		superToolTip20.Items.Add(toolTipTitleItem20);
		superToolTip20.Items.Add(toolTipItem20);
		this.findItem.SuperTip = superToolTip20;
		this.replaceItem.Id = 48;
		this.replaceItem.Name = "replaceItem";
		toolTipTitleItem21.Text = "Replace (ctrl+H)";
		toolTipItem21.LeftIndent = 6;
		toolTipItem21.Text = "Replace text in the document.";
		superToolTip21.Items.Add(toolTipTitleItem21);
		superToolTip21.Items.Add(toolTipItem21);
		this.replaceItem.SuperTip = superToolTip21;
		this.tablesBar.Control = this.htmlRichEditControl;
		this.tablesBar.DockCol = 4;
		this.tablesBar.DockRow = 0;
		this.tablesBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.tablesBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.insertTableItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.lightThemeBarCheckItem)
		});
		this.tablesBar.OptionsBar.AllowQuickCustomization = false;
		this.tablesBar.OptionsBar.DisableCustomization = true;
		this.tablesBar.OptionsBar.DrawDragBorder = false;
		this.insertTableItem.Id = 117;
		this.insertTableItem.Name = "insertTableItem";
		this.lightThemeBarCheckItem.Caption = "Light theme";
		this.lightThemeBarCheckItem.Id = 119;
		this.lightThemeBarCheckItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sun_16;
		this.lightThemeBarCheckItem.Name = "lightThemeBarCheckItem";
		this.lightThemeBarCheckItem.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(themeBarCheckItem_CheckedChanged);
		this.barAndDockingController.PropertiesBar.AllowLinkLighting = false;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.htmlBarManager;
		this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.barDockControlTop.Size = new System.Drawing.Size(1801, 28);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 766);
		this.barDockControlBottom.Manager = this.htmlBarManager;
		this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.barDockControlBottom.Size = new System.Drawing.Size(1801, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 28);
		this.barDockControlLeft.Manager = this.htmlBarManager;
		this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 738);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1801, 28);
		this.barDockControlRight.Manager = this.htmlBarManager;
		this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 738);
		this.changeStyleItem.Edit = this.repositoryItemRichEditStyleEdit1;
		this.changeStyleItem.Id = 45;
		this.changeStyleItem.Name = "changeStyleItem";
		this.repositoryItemRichEditStyleEdit1.AutoHeight = false;
		this.repositoryItemRichEditStyleEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemRichEditStyleEdit1.Control = this.htmlRichEditControl;
		this.repositoryItemRichEditStyleEdit1.Name = "repositoryItemRichEditStyleEdit1";
		this.showEditStyleFormItem.Id = 46;
		this.showEditStyleFormItem.Name = "showEditStyleFormItem";
		this.insertPageBreakItem2.Id = 99;
		this.insertPageBreakItem2.Name = "insertPageBreakItem2";
		this.insertPictureItem.Id = 100;
		this.insertPictureItem.Name = "insertPictureItem";
		this.insertFloatingPictureItem.Id = 10;
		this.insertFloatingPictureItem.Name = "insertFloatingPictureItem";
		this.insertBookmarkItem.Id = 102;
		this.insertBookmarkItem.Name = "insertBookmarkItem";
		this.insertHyperlinkItem.Id = 103;
		this.insertHyperlinkItem.Name = "insertHyperlinkItem";
		this.editPageHeaderItem.Id = 104;
		this.editPageHeaderItem.Name = "editPageHeaderItem";
		this.editPageFooterItem.Id = 105;
		this.editPageFooterItem.Name = "editPageFooterItem";
		this.insertPageNumberItem.Id = 106;
		this.insertPageNumberItem.Name = "insertPageNumberItem";
		this.insertPageCountItem.Id = 107;
		this.insertPageCountItem.Name = "insertPageCountItem";
		this.insertTextBoxItem.Id = 108;
		this.insertTextBoxItem.Name = "insertTextBoxItem";
		this.insertSymbolItem.Id = 109;
		this.insertSymbolItem.Name = "insertSymbolItem";
		this.toggleFirstRowItem.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleFirstRowItem.Id = 110;
		this.toggleFirstRowItem.Name = "toggleFirstRowItem";
		this.toggleLastRowItem.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleLastRowItem.Id = 11;
		this.toggleLastRowItem.Name = "toggleLastRowItem";
		this.toggleBandedRowsItem.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleBandedRowsItem.Id = 112;
		this.toggleBandedRowsItem.Name = "toggleBandedRowsItem";
		this.toggleFirstColumnItem.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleFirstColumnItem.Id = 113;
		this.toggleFirstColumnItem.Name = "toggleFirstColumnItem";
		this.toggleLastColumnItem.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleLastColumnItem.Id = 114;
		this.toggleLastColumnItem.Name = "toggleLastColumnItem";
		this.toggleBandedColumnsItem.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleBandedColumnsItem.Id = 115;
		this.toggleBandedColumnsItem.Name = "toggleBandedColumnsItem";
		this.galleryChangeTableStyleItem.CurrentItem = null;
		this.galleryChangeTableStyleItem.DeleteItemLink = null;
		this.galleryChangeTableStyleItem.Gallery.ColumnCount = 3;
		this.galleryChangeTableStyleItem.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { galleryItemGroup });
		this.galleryChangeTableStyleItem.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryChangeTableStyleItem.Id = 116;
		this.galleryChangeTableStyleItem.ModifyItemLink = null;
		this.galleryChangeTableStyleItem.Name = "galleryChangeTableStyleItem";
		this.galleryChangeTableStyleItem.NewItemLink = null;
		this.galleryChangeTableStyleItem.PopupGallery = null;
		this.changeTableBorderLineStyleItem.Edit = this.repositoryItemBorderLineStyle1;
		this.changeTableBorderLineStyleItem.EditWidth = 130;
		this.changeTableBorderLineStyleItem.Id = 50;
		this.changeTableBorderLineStyleItem.Name = "changeTableBorderLineStyleItem";
		this.repositoryItemBorderLineStyle1.AutoHeight = false;
		this.repositoryItemBorderLineStyle1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemBorderLineStyle1.Control = this.htmlRichEditControl;
		this.repositoryItemBorderLineStyle1.Name = "repositoryItemBorderLineStyle1";
		this.changeTableBorderLineWeightItem.Edit = this.repositoryItemBorderLineWeight1;
		this.changeTableBorderLineWeightItem.EditValue = 20;
		this.changeTableBorderLineWeightItem.EditWidth = 130;
		this.changeTableBorderLineWeightItem.Id = 5;
		this.changeTableBorderLineWeightItem.Name = "changeTableBorderLineWeightItem";
		this.repositoryItemBorderLineWeight1.AutoHeight = false;
		this.repositoryItemBorderLineWeight1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemBorderLineWeight1.Control = this.htmlRichEditControl;
		this.repositoryItemBorderLineWeight1.Name = "repositoryItemBorderLineWeight1";
		this.changeTableBorderColorItem.Id = 52;
		this.changeTableBorderColorItem.Name = "changeTableBorderColorItem";
		this.changeTableBordersItem.Id = 53;
		this.changeTableBordersItem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[11]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsBottomBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsTopBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsLeftBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsRightBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.resetTableCellsAllBordersItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsAllBordersItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsOutsideBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideHorizontalBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideVerticalBorderItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleShowTableGridLinesItem)
		});
		this.changeTableBordersItem.Name = "changeTableBordersItem";
		this.toggleTableCellsBottomBorderItem.Id = 54;
		this.toggleTableCellsBottomBorderItem.Name = "toggleTableCellsBottomBorderItem";
		this.toggleTableCellsTopBorderItem.Id = 55;
		this.toggleTableCellsTopBorderItem.Name = "toggleTableCellsTopBorderItem";
		this.toggleTableCellsLeftBorderItem.Id = 56;
		this.toggleTableCellsLeftBorderItem.Name = "toggleTableCellsLeftBorderItem";
		this.toggleTableCellsRightBorderItem.Id = 57;
		this.toggleTableCellsRightBorderItem.Name = "toggleTableCellsRightBorderItem";
		this.resetTableCellsAllBordersItem.Id = 58;
		this.resetTableCellsAllBordersItem.Name = "resetTableCellsAllBordersItem";
		this.toggleTableCellsAllBordersItem.Id = 59;
		this.toggleTableCellsAllBordersItem.Name = "toggleTableCellsAllBordersItem";
		this.toggleTableCellsOutsideBorderItem.Id = 60;
		this.toggleTableCellsOutsideBorderItem.Name = "toggleTableCellsOutsideBorderItem";
		this.toggleTableCellsInsideBorderItem.Id = 6;
		this.toggleTableCellsInsideBorderItem.Name = "toggleTableCellsInsideBorderItem";
		this.toggleTableCellsInsideHorizontalBorderItem.Id = 62;
		this.toggleTableCellsInsideHorizontalBorderItem.Name = "toggleTableCellsInsideHorizontalBorderItem";
		this.toggleTableCellsInsideVerticalBorderItem.Id = 63;
		this.toggleTableCellsInsideVerticalBorderItem.Name = "toggleTableCellsInsideVerticalBorderItem";
		this.toggleShowTableGridLinesItem.Id = 64;
		this.toggleShowTableGridLinesItem.Name = "toggleShowTableGridLinesItem";
		this.changeTableCellsShadingItem.Id = 65;
		this.changeTableCellsShadingItem.Name = "changeTableCellsShadingItem";
		this.selectTableElementsItem.Id = 66;
		this.selectTableElementsItem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableCellItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableColumnItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableRowItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableItem)
		});
		this.selectTableElementsItem.Name = "selectTableElementsItem";
		this.selectTableCellItem.Id = 67;
		this.selectTableCellItem.Name = "selectTableCellItem";
		this.selectTableColumnItem.Id = 68;
		this.selectTableColumnItem.Name = "selectTableColumnItem";
		this.selectTableRowItem.Id = 69;
		this.selectTableRowItem.Name = "selectTableRowItem";
		this.selectTableItem.Id = 70;
		this.selectTableItem.Name = "selectTableItem";
		this.showTablePropertiesFormItem.Id = 7;
		this.showTablePropertiesFormItem.Name = "showTablePropertiesFormItem";
		this.deleteTableElementsItem.Id = 72;
		this.deleteTableElementsItem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.showDeleteTableCellsFormItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteTableColumnsItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteTableRowsItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteTableItem)
		});
		this.deleteTableElementsItem.Name = "deleteTableElementsItem";
		this.showDeleteTableCellsFormItem.Id = 73;
		this.showDeleteTableCellsFormItem.Name = "showDeleteTableCellsFormItem";
		this.deleteTableColumnsItem.Id = 74;
		this.deleteTableColumnsItem.Name = "deleteTableColumnsItem";
		this.deleteTableRowsItem.Id = 75;
		this.deleteTableRowsItem.Name = "deleteTableRowsItem";
		this.deleteTableItem.Id = 76;
		this.deleteTableItem.Name = "deleteTableItem";
		this.insertTableRowAboveItem.Id = 77;
		this.insertTableRowAboveItem.Name = "insertTableRowAboveItem";
		this.insertTableRowBelowItem.Id = 78;
		this.insertTableRowBelowItem.Name = "insertTableRowBelowItem";
		this.insertTableColumnToLeftItem.Id = 79;
		this.insertTableColumnToLeftItem.Name = "insertTableColumnToLeftItem";
		this.insertTableColumnToRightItem.Id = 80;
		this.insertTableColumnToRightItem.Name = "insertTableColumnToRightItem";
		this.showInsertTableCellsFormItem.Id = 8;
		this.showInsertTableCellsFormItem.Name = "showInsertTableCellsFormItem";
		this.mergeTableCellsItem.Id = 82;
		this.mergeTableCellsItem.Name = "mergeTableCellsItem";
		this.showSplitTableCellsForm.Id = 83;
		this.showSplitTableCellsForm.Name = "showSplitTableCellsForm";
		this.splitTableItem.Id = 84;
		this.splitTableItem.Name = "splitTableItem";
		this.toggleTableAutoFitItem.Id = 85;
		this.toggleTableAutoFitItem.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableAutoFitContentsItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableAutoFitWindowItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableFixedColumnWidthItem)
		});
		this.toggleTableAutoFitItem.Name = "toggleTableAutoFitItem";
		this.toggleTableAutoFitContentsItem.Id = 86;
		this.toggleTableAutoFitContentsItem.Name = "toggleTableAutoFitContentsItem";
		this.toggleTableAutoFitWindowItem.Id = 87;
		this.toggleTableAutoFitWindowItem.Name = "toggleTableAutoFitWindowItem";
		this.toggleTableFixedColumnWidthItem.Id = 88;
		this.toggleTableFixedColumnWidthItem.Name = "toggleTableFixedColumnWidthItem";
		this.toggleTableCellsTopLeftAlignmentItem.Id = 89;
		this.toggleTableCellsTopLeftAlignmentItem.Name = "toggleTableCellsTopLeftAlignmentItem";
		this.toggleTableCellsMiddleLeftAlignmentItem.Id = 90;
		this.toggleTableCellsMiddleLeftAlignmentItem.Name = "toggleTableCellsMiddleLeftAlignmentItem";
		this.toggleTableCellsBottomLeftAlignmentItem.Id = 9;
		this.toggleTableCellsBottomLeftAlignmentItem.Name = "toggleTableCellsBottomLeftAlignmentItem";
		this.toggleTableCellsTopCenterAlignmentItem.Id = 92;
		this.toggleTableCellsTopCenterAlignmentItem.Name = "toggleTableCellsTopCenterAlignmentItem";
		this.toggleTableCellsMiddleCenterAlignmentItem.Id = 93;
		this.toggleTableCellsMiddleCenterAlignmentItem.Name = "toggleTableCellsMiddleCenterAlignmentItem";
		this.toggleTableCellsBottomCenterAlignmentItem.Id = 94;
		this.toggleTableCellsBottomCenterAlignmentItem.Name = "toggleTableCellsBottomCenterAlignmentItem";
		this.toggleTableCellsTopRightAlignmentItem.Id = 95;
		this.toggleTableCellsTopRightAlignmentItem.Name = "toggleTableCellsTopRightAlignmentItem";
		this.toggleTableCellsMiddleRightAlignmentItem.Id = 96;
		this.toggleTableCellsMiddleRightAlignmentItem.Name = "toggleTableCellsMiddleRightAlignmentItem";
		this.toggleTableCellsBottomRightAlignmentItem.Id = 97;
		this.toggleTableCellsBottomRightAlignmentItem.Name = "toggleTableCellsBottomRightAlignmentItem";
		this.showTableOptionsFormItem.Id = 98;
		this.showTableOptionsFormItem.Name = "showTableOptionsFormItem";
		this.htmlRichEditBarController.BarItems.Add(this.pasteItem);
		this.htmlRichEditBarController.BarItems.Add(this.cutItem);
		this.htmlRichEditBarController.BarItems.Add(this.copyItem);
		this.htmlRichEditBarController.BarItems.Add(this.pasteSpecialItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeFontNameItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeFontSizeItem);
		this.htmlRichEditBarController.BarItems.Add(this.fontSizeIncreaseItem);
		this.htmlRichEditBarController.BarItems.Add(this.fontSizeDecreaseItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontBoldItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontItalicItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontUnderlineItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontDoubleUnderlineItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontStrikeoutItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontDoubleStrikeoutItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontSuperscriptItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFontSubscriptItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeFontColorItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeFontBackColorItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeTextCaseItem);
		this.htmlRichEditBarController.BarItems.Add(this.makeTextUpperCaseItem);
		this.htmlRichEditBarController.BarItems.Add(this.makeTextLowerCaseItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTextCaseItem);
		this.htmlRichEditBarController.BarItems.Add(this.clearFormattingItem);
		this.htmlRichEditBarController.BarItems.Add(this.showFontFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleBulletedListItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleNumberingListItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleMultiLevelListItem);
		this.htmlRichEditBarController.BarItems.Add(this.decreaseIndentItem);
		this.htmlRichEditBarController.BarItems.Add(this.increaseIndentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleParagraphAlignmentLeftItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleParagraphAlignmentCenterItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleParagraphAlignmentRightItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleParagraphAlignmentJustifyItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleShowWhitespaceItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeParagraphLineSpacingItem);
		this.htmlRichEditBarController.BarItems.Add(this.setSingleParagraphSpacingItem);
		this.htmlRichEditBarController.BarItems.Add(this.setSesquialteralParagraphSpacingItem);
		this.htmlRichEditBarController.BarItems.Add(this.setDoubleParagraphSpacingItem);
		this.htmlRichEditBarController.BarItems.Add(this.showLineSpacingFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.addSpacingBeforeParagraphItem);
		this.htmlRichEditBarController.BarItems.Add(this.removeSpacingBeforeParagraphItem);
		this.htmlRichEditBarController.BarItems.Add(this.addSpacingAfterParagraphItem);
		this.htmlRichEditBarController.BarItems.Add(this.removeSpacingAfterParagraphItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeParagraphBackColorItem);
		this.htmlRichEditBarController.BarItems.Add(this.showParagraphFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeStyleItem);
		this.htmlRichEditBarController.BarItems.Add(this.showEditStyleFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.findItem);
		this.htmlRichEditBarController.BarItems.Add(this.replaceItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertPageBreakItem2);
		this.htmlRichEditBarController.BarItems.Add(this.insertPictureItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertFloatingPictureItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertBookmarkItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertHyperlinkItem);
		this.htmlRichEditBarController.BarItems.Add(this.editPageHeaderItem);
		this.htmlRichEditBarController.BarItems.Add(this.editPageFooterItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertPageNumberItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertPageCountItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertTextBoxItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertSymbolItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFirstRowItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleLastRowItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleBandedRowsItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleFirstColumnItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleLastColumnItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleBandedColumnsItem);
		this.htmlRichEditBarController.BarItems.Add(this.galleryChangeTableStyleItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeTableBorderLineStyleItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeTableBorderLineWeightItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeTableBorderColorItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeTableBordersItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsBottomBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsTopBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsLeftBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsRightBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.resetTableCellsAllBordersItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsAllBordersItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsOutsideBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsInsideBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsInsideHorizontalBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsInsideVerticalBorderItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleShowTableGridLinesItem);
		this.htmlRichEditBarController.BarItems.Add(this.changeTableCellsShadingItem);
		this.htmlRichEditBarController.BarItems.Add(this.selectTableElementsItem);
		this.htmlRichEditBarController.BarItems.Add(this.selectTableCellItem);
		this.htmlRichEditBarController.BarItems.Add(this.selectTableColumnItem);
		this.htmlRichEditBarController.BarItems.Add(this.selectTableRowItem);
		this.htmlRichEditBarController.BarItems.Add(this.selectTableItem);
		this.htmlRichEditBarController.BarItems.Add(this.showTablePropertiesFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.deleteTableElementsItem);
		this.htmlRichEditBarController.BarItems.Add(this.showDeleteTableCellsFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.deleteTableColumnsItem);
		this.htmlRichEditBarController.BarItems.Add(this.deleteTableRowsItem);
		this.htmlRichEditBarController.BarItems.Add(this.deleteTableItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertTableRowAboveItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertTableRowBelowItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertTableColumnToLeftItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertTableColumnToRightItem);
		this.htmlRichEditBarController.BarItems.Add(this.showInsertTableCellsFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.mergeTableCellsItem);
		this.htmlRichEditBarController.BarItems.Add(this.showSplitTableCellsForm);
		this.htmlRichEditBarController.BarItems.Add(this.splitTableItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableAutoFitItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableAutoFitContentsItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableAutoFitWindowItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableFixedColumnWidthItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsTopLeftAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsMiddleLeftAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsBottomLeftAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsTopCenterAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsMiddleCenterAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsBottomCenterAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsTopRightAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsMiddleRightAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.toggleTableCellsBottomRightAlignmentItem);
		this.htmlRichEditBarController.BarItems.Add(this.showTableOptionsFormItem);
		this.htmlRichEditBarController.BarItems.Add(this.insertTableItem);
		this.htmlRichEditBarController.Control = this.htmlRichEditControl;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 17f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.htmlRichEditControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "HtmlUserControl";
		base.Size = new System.Drawing.Size(1801, 766);
		((System.ComponentModel.ISupportInitialize)this.htmlBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemRichEditFontSizeEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barAndDockingController).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemRichEditStyleEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineStyle1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineWeight1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.htmlRichEditBarController).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
