using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI.Skins.Base;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

namespace Dataedo.App.Tools.UI.Skins;

internal class VsDark : BaseSkin
{
	internal class ColorTranslatorVisitor : DocumentVisitorBase
	{
		private readonly Document document;

		public ColorTranslatorVisitor(Document document)
		{
			this.document = document;
		}

		public override void Visit(DocumentText text)
		{
			base.Visit(text);
			CharacterProperties properties = document.BeginUpdateCharacters(text.Position, text.Length);
			properties.InvertColors();
			document.EndUpdateCharacters(properties);
		}

		public override void Visit(DocumentTableCellBorder cellBorder)
		{
			base.Visit(cellBorder);
		}

		public override void Visit(DocumentParagraphEnd paragraphEnd)
		{
			base.Visit(paragraphEnd);
		}
	}

	private static RichEditControl TranslationRichEditControl;

	public static string SkinNameValue = "The Bezier Common";

	public static string PaletteValue = "VS Dark";

	private float progressDarkFactor = 0.3f;

	private float erdDarkFactor = 0.7f;

	private float erdLightFactor = 0.5f;

	public override string SkinName => SkinNameValue;

	public override string Palette => PaletteValue;

	public override string ResourcesBaseName => typeof(ResourcesVsDark).FullName;

	public override ResourceManager ResourceManager
	{
		get
		{
			if (ResourcesVsDark.ResourceManager.BaseName == ResourcesBaseName)
			{
				return ResourcesVsDark.ResourceManager;
			}
			return new ResourceManager(ResourcesBaseName, typeof(ResourcesVsDark).Assembly);
		}
	}

	public override bool IsDarkTheme => true;

	public override bool UseDefaultTitleBar => false;

	public override Color ControlBackColor => Color.FromArgb(37, 37, 38);

	public override Color ControlForeColor => Color.FromArgb(241, 241, 241);

	public override Color ControlBorderColor => Color.FromArgb(67, 67, 70);

	public override Color LoginFormFocusedItemBackColor => Color.FromArgb(255, 70, 121, 198);

	public override Color SuggestedDescriptionsMarkerColor => Color.FromArgb(255, 70, 121, 198);

	public override Color DeletedObjectForeColor => ControlPaint.Light(SkinsManager.DefaultSkin.DeletedObjectForeColor, 0.2f);

	public override Color ProgressPainterColor => Color.FromArgb(70, 121, 198);

	public override Color ProgressPainterTabColor => Color.FromArgb(165, 200, 254);

	public override Color ToolTipSelectedColorForeColor => ColorTranslator.FromHtml("#3361A6");

	public override Color ListSelectionBackColor => Color.FromArgb(70, 121, 198);

	public override Color ListSelectionForeColor => Color.FromArgb(128, 128, 128);

	public override Color LoginFormTitleForeColor => Color.LightGray;

	public override Color YellowBackColor => Color.FromArgb(142, 140, 34);

	public override Color RedBackColor => Color.FromArgb(160, 53, 24);

	public override Color GreenBackColor => Color.FromArgb(30, 109, 30);

	public override Color DisabledForeColor => Color.Gray;

	public override Color LegacyTextExportBackColor => Color.FromArgb(68, 68, 68);

	public override Color BetaTextExportBackColor => Color.FromArgb(68, 68, 68);

	public override Color RecomendedExportBackColor => Color.FromArgb(52, 99, 175);

	public override Color LegacyTextExportForeColor => Color.FromArgb(151, 151, 151);

	public override Color BetaTextExportForeColor => Color.FromArgb(151, 151, 151);

	public override Color RecomendednExportForeColor => Color.FromArgb(252, 252, 254);

	public override Color DDLExampleBackColor => Color.FromArgb(68, 68, 68);

	public override Color DDLExampleForeColor => Color.FromArgb(153, 153, 153);

	public override Color OnboardingAddDatabaseButtonBackColor => Color.FromArgb(58, 91, 167);

	public override Color OnboardingAddDatabaseButtonForeColor => DisabledFunctionalitiesForeColor;

	public override Color OnboardingPanelBackColor => Color.FromArgb(183, 206, 240);

	public override Color OnboardingPanelForeColor => UpdatePanelForeColor;

	public override Color OnboardingPanelCloseButtonHoverBackColor => Color.FromArgb(30, Color.Black);

	public override Color DisabledFunctionalitiesBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.DisabledFunctionalitiesBackColor, 0.85f);

	public override Color DisabledFunctionalitiesForeColor => ControlPaint.Light(SkinsManager.DefaultSkin.DisabledFunctionalitiesForeColor, 1.3f);

	public override Color DeletedObjectPanelBackColor => ControlPaint.Light(ControlBackColor, 0.5f);

	public override Color DeletedObjectPanelForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color DisabledFunctionalityTextForeColor => Color.FromArgb(70, 121, 198);

	public override Color UpdatePanelForeColor => ControlBackColor;

	public override Color UpdatePanelBackColor => Color.FromArgb(219, 198, 96);

	public override Color TreeListCustomFocusBackColor => ControlPaint.Dark(SkinColors.AccentPaint, 0.2f);

	public override Color TreeListCustomFocusForeColor => SkinColors.AccentBrush;

	public override Color ProgressLoadingColumnBackColor => Color.FromArgb(52, 52, 52);

	public override Color ProgressObjectLoadingRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressObjectLoadingRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override Color ProgressFolderLoadingRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressFolderLoadingRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override Color ProgressModuleLoadingRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressModuleLoadingRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override Color ProgressDatabaseLoadingRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressDatabaseLoadingRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override Color ProgressDatabaseRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressDatabaseRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override Color ProgressModuleRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressModuleRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override Color ProgressFolderRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressFolderRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override Color ProgressSimpleObjectRepositoryItemProgressBarBackColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ProgressSimpleObjectRepositoryItemProgressBarBackColor, progressDarkFactor);

	public override string SearchHighlightHtmlColor => "#E8B655";

	public override string SearchHighlightHtmlForeColor => ColorTranslator.ToHtml(SearchHighlightForeColor);

	public override Color SearchHighlightColor => Color.FromArgb(255, 232, 182, 85);

	public override Color SearchHighlightForeColor => SkinColors.ControlColorFromSystemColors;

	public override Color SearchHighlightColorUnderSelectionColorNormal => Color.FromArgb(255, 218, 205, 165);

	public override Color SearchHighlightColorUnderSelectionForeColorNormal => SkinColors.ControlColorFromSystemColors;

	public override Color SearchTabForeColorHighlighted => Color.FromArgb(255, 185, 130, 26);

	public override Color GridGridRowBackColor => Color.Transparent;

	public override Color GridGridRowForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color GridNonEditableColumnsBackColor => Color.FromArgb(49, 49, 51);

	public override Color GridDisabledGridRowBackColor => Color.FromArgb(49, 49, 51);

	public override Color GridDisabledGridRowForeColor => Color.Gray;

	public override Color GridAccentGridRowBackColor => Color.FromArgb(76, 74, 72);

	public override Color GridAccentGridRowForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color GridAccentRedGridRowForeColor => ControlPaint.Light(Color.DarkRed, 0.5f);

	public override Color GridHighlightRowBackColor => Color.FromArgb(128, 70, 121, 198);

	public override Color GridSelectionBackColor => Color.FromArgb(70, 121, 198);

	public override Color GreyColor => Color.FromArgb(128, 128, 128);

	public override Color SchemaImportsAndChangesTreeAddedForeColor => Color.FromArgb(0, 128, 0);

	public override Color SchemaImportsAndChangesTreeUpdatedForeColor => Color.FromArgb(71, 122, 198);

	public override Color SchemaImportsAndChangesTreeDeletedForeColor => Color.FromArgb(166, 43, 36);

	public override Color SchemaImportsAndChangesPanelAddedForeColor => SkinColors.Brush;

	public override Color SchemaImportsAndChangesPanelAddedBackColor => Color.FromArgb(30, 109, 30);

	public override Color SchemaImportsAndChangesPanelUpdatedForeColor => SkinColors.Brush;

	public override Color SchemaImportsAndChangesPanelUpdatedBackColor => Color.FromArgb(142, 140, 34);

	public override Color SchemaImportsAndChangesPanelDeletedForeColor => SkinColors.Brush;

	public override Color SchemaImportsAndChangesPanelDeletedBackColor => Color.FromArgb(160, 53, 24);

	public override Color DoubledRowBackColor => ControlPaint.Dark(Color.FromArgb(255, 243, 198), 0.8f);

	public override Color ImportDescriptionsNoChangeForeColor => Color.Gray;

	public override Color ImportDescriptionsNoChangeBackColor => ControlPaint.Dark(Color.FromArgb(251, 251, 251), 0.7f);

	public override Color ImportDescriptionsNewForeColor => Color.FromArgb(0, 128, 0);

	public override Color ImportDescriptionsNewBackColor => ControlPaint.Dark(Color.FromArgb(233, 253, 232), 0.8f);

	public override Color ImportDescriptionsEraseForeColor => Color.FromArgb(166, 43, 36);

	public override Color ImportDescriptionsEraseBackColor => ControlPaint.Dark(Color.FromArgb(251, 231, 230), 0.8f);

	public override Color ImportDescriptionsUpdateForeColor => Color.FromArgb(191, 122, 70);

	public override Color ImportDescriptionsUpdateBackColor => ControlPaint.Dark(Color.FromArgb(255, 243, 198), 0.8f);

	public override Color ErdBackColor => ControlBackColor;

	public override Color ErdERDGridColor => ControlPaint.Light(ControlBackColor, 0.06f);

	public override Color ErdNodeBackColor => ControlPaint.Light(ControlBackColor, 0.1f);

	public override Color ErdNodeBorderColor => ControlPaint.Light(ErdGrayDark, 0.7f);

	public override Color ErdNodeForeColor => ControlForeColor;

	public override Color ErdLinkColor => ControlPaint.Light(ErdGrayDark, 1f);

	public override Color ErdLinkTextForeColor => ControlPaint.Light(ErdLinkColor, 1.2f);

	public override Color ErdERDHiddenRelationColor => ControlPaint.Dark(ErdLinkColor, 0.4f);

	public override Color ErdGrayLight => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdGrayLight, erdDarkFactor);

	public override Color ErdGrayDark => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdGrayDark, erdLightFactor);

	public override Color ErdSelectedNodeBorderColor => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdSelectedNodeBorderColor, 0.1f);

	public override Color ErdSelectedNodeColor => ControlPaint.Light(ErdSelectedNodeBorderColor, 0.4f);

	public override bool ErdUseNodeColorForHeaderBorder => false;

	public override Color ErdNodeBlue => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeBlue, erdDarkFactor);

	public override Color ErdNodeGreen => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeGreen, erdDarkFactor);

	public override Color ErdNodeRed => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeRed, erdDarkFactor);

	public override Color ErdNodeYellow => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeYellow, erdDarkFactor);

	public override Color ErdNodePurple => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodePurple, erdDarkFactor);

	public override Color ErdNodeOrange => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeOrange, erdDarkFactor);

	public override Color ErdNodeCyan => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeCyan, erdDarkFactor);

	public override Color ErdNodeLime => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeLime, erdDarkFactor);

	public override Color ErdNodeDefault => ControlPaint.Dark(SkinsManager.DefaultSkin.ErdNodeDefault, erdDarkFactor);

	public override Color ErdFocusedColumnBackColor => ErdSelectedNodeBorderColor;

	public override Color TextFieldBackColor => ControlBackColor;

	public override Color TextFieldForeColor => ControlForeColor;

	public override Color HyperlinkInRichTextForeColor => Color.FromArgb(255, 255, 0);

	public override bool ScriptShouldTranslateColors => true;

	public override Color ScriptBackColor => SkinColors.KeyPaint;

	public override Color ScriptForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationNotRequiredForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationNotRequiredBackColor => SkinColors.ControlColorFromSystemColors;

	public override Color SynchronizeForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizeBackColor => Color.FromArgb(37, 37, 38);

	public override Color SynchronizationSuccessfulForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationSuccessfulBackColor => Color.FromArgb(19, 35, 19);

	public override Color SynchronizationFailedForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationFailedBackColor => Color.FromArgb(35, 19, 19);

	public override Color InfoTextForeColor => ControlPaint.Dark(Color.DarkGray, 0.5f);

	public override void SetLightThemeBackground(RichEditControl richEditControl, bool setForeColor = false)
	{
		BaseSkin defaultSkin = SkinsManager.DefaultSkin;
		richEditControl.ActiveView.BackColor = defaultSkin.TextFieldBackColor;
		if (setForeColor)
		{
			richEditControl.Appearance.Text.ForeColor = Color.Black;
		}
	}

	public override void SetDarkThemeBackground(RichEditControl richEditControl, bool setForeColor = false)
	{
		richEditControl.ActiveView.BackColor = TextFieldBackColor;
		if (setForeColor)
		{
			richEditControl.Appearance.Text.ForeColor = Color.White;
		}
	}

	public override void TranslateColors(RichEditControl richEditControl)
	{
		richEditControl.BeginUpdate();
		richEditControl.Document.BeginUpdate();
		ColorTranslatorVisitor visitor = new ColorTranslatorVisitor(richEditControl.Document);
		DocumentIterator documentIterator = new DocumentIterator(richEditControl.Document, visibleTextOnly: true);
		while (documentIterator.MoveNext())
		{
			documentIterator.Current.Accept(visitor);
		}
		richEditControl.Document.EndUpdate();
		richEditControl.EndUpdate();
	}

	public override string GetHtmlTranslatedColors(RichEditControl richEditControl)
	{
		if (richEditControl.HtmlText.Length == 0)
		{
			return string.Empty;
		}
		if (TranslationRichEditControl == null)
		{
			TranslationRichEditControl = new RichEditControl();
		}
		TranslationRichEditControl.HtmlText = richEditControl.HtmlText;
		TranslateColors(TranslationRichEditControl);
		string htmlText = TranslationRichEditControl.HtmlText;
		TranslationRichEditControl.HtmlText = null;
		return htmlText;
	}

	private void TranslateTablesColors(RichEditControl richEditControl)
	{
		foreach (Table table in richEditControl.Document.Tables)
		{
			table.BeginUpdate();
			foreach (TableRow row in table.Rows)
			{
				foreach (TableCell cell in row.Cells)
				{
					TranslateCellColors(cell);
				}
			}
			TranslateTableColors(table);
			table.EndUpdate();
		}
	}

	private void TranslateCellColors(TableCell cell)
	{
		cell.Borders.Left.LineColor = cell.Borders.Left.LineColor.Invert();
		cell.Borders.Top.LineColor = cell.Borders.Top.LineColor.Invert();
		cell.Borders.Right.LineColor = cell.Borders.Right.LineColor.Invert();
		cell.Borders.Bottom.LineColor = cell.Borders.Bottom.LineColor.Invert();
		cell.BackgroundColor = cell.BackgroundColor.Invert();
	}

	private void TranslateTableColors(Table table)
	{
		table.Borders.Left.LineColor = table.Borders.Left.LineColor.Invert();
		table.Borders.Top.LineColor = table.Borders.Top.LineColor.Invert();
		table.Borders.Right.LineColor = table.Borders.Right.LineColor.Invert();
		table.Borders.Bottom.LineColor = table.Borders.Bottom.LineColor.Invert();
		table.Borders.InsideHorizontalBorder.LineColor = table.Borders.InsideHorizontalBorder.LineColor.Invert();
		table.Borders.InsideVerticalBorder.LineColor = table.Borders.InsideVerticalBorder.LineColor.Invert();
		table.TableBackgroundColor = table.TableBackgroundColor.Invert();
	}

	private void TranslateListsColors(RichEditControl richEditControl)
	{
		foreach (NumberingList numberingList in richEditControl.Document.NumberingLists)
		{
			foreach (OverrideListLevel level in numberingList.Levels)
			{
				level.CharacterProperties.InvertColors();
			}
		}
	}

	public override Color GetNodeColor(string color)
	{
		return base.GetNodeColor(color);
	}

	public override Color GetNodeColorLight(string color)
	{
		return ControlPaint.Light(base.GetNodeColor(color), erdLightFactor);
	}

	public override string ScriptTranslateHtmlColor(string htmlColor)
	{
		return ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(htmlColor), 0.9f));
	}
}
