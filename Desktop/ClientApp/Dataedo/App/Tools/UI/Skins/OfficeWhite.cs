using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI.Skins.Base;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Tools.UI.Skins;

internal class OfficeWhite : BaseSkin
{
	public static string SkinNameValue = "The Bezier Common";

	public static string PaletteValue = "Office White";

	public override string SkinName => SkinNameValue;

	public override string Palette => PaletteValue;

	public override string ResourcesBaseName => typeof(Resources).FullName;

	public override ResourceManager ResourceManager
	{
		get
		{
			if (Resources.ResourceManager.BaseName == ResourcesBaseName)
			{
				return Resources.ResourceManager;
			}
			return new ResourceManager(ResourcesBaseName, typeof(Resources).Assembly);
		}
	}

	public override bool IsDarkTheme => false;

	public override bool UseDefaultTitleBar => true;

	public override Color ControlBackColor => Color.FromArgb(255, 255, 255);

	public override Color ControlForeColor => Color.FromArgb(38, 38, 38);

	public override Color ControlBorderColor => Color.FromArgb(160, 160, 160);

	public override Color LoginFormFocusedItemBackColor => Color.FromArgb(255, 215, 228, 242);

	public override Color SuggestedDescriptionsMarkerColor => Color.FromArgb(255, 70, 121, 198);

	public override Color DeletedObjectForeColor => Color.Gray;

	public override Color ProgressPainterColor => Color.FromArgb(230, 243, 255);

	public override Color ProgressPainterTabColor => Color.FromArgb(165, 200, 254);

	public override Color ToolTipSelectedColorForeColor => ColorTranslator.FromHtml("#3361A6");

	public override Color ListSelectionBackColor => Color.FromArgb(100, 70, 121, 198);

	public override Color ListSelectionForeColor => Color.FromArgb(128, 128, 128);

	public override Color LoginFormTitleForeColor => Color.Gray;

	public override Color LegacyTextExportBackColor => Color.FromArgb(229, 229, 229);

	public override Color BetaTextExportBackColor => Color.FromArgb(229, 229, 229);

	public override Color RecomendedExportBackColor => Color.FromArgb(224, 234, 248);

	public override Color LegacyTextExportForeColor => Color.FromArgb(124, 124, 124);

	public override Color BetaTextExportForeColor => Color.FromArgb(124, 124, 124);

	public override Color RecomendednExportForeColor => Color.FromArgb(32, 45, 48);

	public override Color DDLExampleBackColor => Color.FromArgb(229, 229, 229);

	public override Color DDLExampleForeColor => Color.FromArgb(57, 57, 57);

	public override Color OnboardingAddDatabaseButtonBackColor => Color.FromArgb(58, 91, 167);

	public override Color OnboardingAddDatabaseButtonForeColor => SkinColors.ControlColorFromSystemColors;

	public override Color DisabledForeColor => Color.Gray;

	public override Color YellowBackColor => Color.FromArgb(231, 230, 152);

	public override Color RedBackColor => Color.FromArgb(255, 214, 214);

	public override Color GreenBackColor => Color.FromArgb(217, 255, 217);

	public override Color OnboardingPanelBackColor => Color.FromArgb(183, 206, 240);

	public override Color OnboardingPanelForeColor => UpdatePanelForeColor;

	public override Color OnboardingPanelCloseButtonHoverBackColor => Color.FromArgb(30, Color.White);

	public override Color DisabledFunctionalitiesBackColor => Color.FromArgb(255, 224, 234, 248);

	public override Color DisabledFunctionalitiesForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color DeletedObjectPanelBackColor => Color.FromArgb(255, 255, 224);

	public override Color DeletedObjectPanelForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color DisabledFunctionalityTextForeColor => Color.FromArgb(70, 121, 198);

	public override Color UpdatePanelForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color UpdatePanelBackColor => Color.LemonChiffon;

	public override Color TreeListCustomFocusBackColor => Color.FromArgb(239, 247, 255);

	public override Color TreeListCustomFocusForeColor => SkinColors.AccentBrush;

	public override Color ProgressLoadingColumnBackColor => Color.FromArgb(239, 239, 239);

	public override Color ProgressObjectLoadingRepositoryItemProgressBarBackColor => Color.FromArgb(233, 176, 61);

	public override Color ProgressFolderLoadingRepositoryItemProgressBarBackColor => Color.FromArgb(221, 162, 40);

	public override Color ProgressModuleLoadingRepositoryItemProgressBarBackColor => Color.FromArgb(211, 150, 23);

	public override Color ProgressDatabaseLoadingRepositoryItemProgressBarBackColor => Color.FromArgb(198, 135, 4);

	public override Color ProgressDatabaseRepositoryItemProgressBarBackColor => Color.FromArgb(47, 92, 176);

	public override Color ProgressModuleRepositoryItemProgressBarBackColor => Color.FromArgb(88, 133, 202);

	public override Color ProgressFolderRepositoryItemProgressBarBackColor => Color.FromArgb(121, 162, 216);

	public override Color ProgressSimpleObjectRepositoryItemProgressBarBackColor => Color.FromArgb(151, 185, 227);

	public override string SearchHighlightHtmlColor => "#E8B655";

	public override string SearchHighlightHtmlForeColor => ColorTranslator.ToHtml(SearchHighlightForeColor);

	public override Color SearchHighlightColor => Color.FromArgb(255, 232, 182, 85);

	public override Color SearchHighlightForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SearchHighlightColorUnderSelectionColorNormal => Color.FromArgb(255, 218, 205, 165);

	public override Color SearchHighlightColorUnderSelectionForeColorNormal => SkinColors.ControlForeColorFromSystemColors;

	public override Color SearchTabForeColorHighlighted => Color.FromArgb(255, 185, 130, 26);

	public override Color GridGridRowBackColor => Color.Transparent;

	public override Color GridGridRowForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color GridNonEditableColumnsBackColor => Color.FromArgb(249, 249, 249);

	public override Color GridDisabledGridRowBackColor => Color.FromArgb(234, 234, 234);

	public override Color GridDisabledGridRowForeColor => Color.Gray;

	public override Color GridAccentGridRowBackColor => Color.FromArgb(253, 255, 226);

	public override Color GridAccentRedGridRowForeColor => Color.DarkRed;

	public override Color GridAccentGridRowForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color GridHighlightRowBackColor => Color.FromArgb(215, 228, 242);

	public override Color GridSelectionBackColor => Color.FromArgb(180, 196, 224);

	public override Color GreyColor => Color.FromArgb(128, 128, 128);

	public override Color SchemaImportsAndChangesTreeAddedForeColor => Color.FromArgb(0, 128, 0);

	public override Color SchemaImportsAndChangesTreeUpdatedForeColor => Color.FromArgb(71, 122, 198);

	public override Color SchemaImportsAndChangesTreeDeletedForeColor => Color.FromArgb(166, 43, 36);

	public override Color SchemaImportsAndChangesPanelAddedForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SchemaImportsAndChangesPanelAddedBackColor => Color.FromArgb(217, 255, 217);

	public override Color SchemaImportsAndChangesPanelUpdatedForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SchemaImportsAndChangesPanelUpdatedBackColor => Color.FromArgb(231, 230, 152);

	public override Color SchemaImportsAndChangesPanelDeletedForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SchemaImportsAndChangesPanelDeletedBackColor => Color.FromArgb(255, 214, 214);

	public override Color DoubledRowBackColor => Color.FromArgb(255, 243, 198);

	public override Color ImportDescriptionsNoChangeForeColor => Color.Gray;

	public override Color ImportDescriptionsNoChangeBackColor => Color.FromArgb(251, 251, 251);

	public override Color ImportDescriptionsNewForeColor => Color.FromArgb(0, 128, 0);

	public override Color ImportDescriptionsNewBackColor => Color.FromArgb(233, 253, 232);

	public override Color ImportDescriptionsEraseForeColor => Color.FromArgb(166, 43, 36);

	public override Color ImportDescriptionsEraseBackColor => Color.FromArgb(251, 231, 230);

	public override Color ImportDescriptionsUpdateForeColor => Color.FromArgb(191, 122, 70);

	public override Color ImportDescriptionsUpdateBackColor => Color.FromArgb(255, 243, 198);

	public override Color ErdBackColor => ControlBackColor;

	public override Color ErdERDGridColor => ColorTranslator.FromHtml("#F3F3F3");

	public override Color ErdNodeBackColor => ControlBackColor;

	public override Color ErdNodeBorderColor => ErdNodeDefault;

	public override Color ErdNodeForeColor => ControlForeColor;

	public override Color ErdLinkColor => ErdGrayDark;

	public override Color ErdLinkTextForeColor => ErdLinkColor;

	public override Color ErdERDHiddenRelationColor => Color.FromArgb(220, 220, 220);

	public override Color ErdGrayLight => Color.FromArgb(200, 200, 200);

	public override Color ErdGrayDark => ColorTranslator.FromHtml("#757575");

	public override Color ErdSelectedNodeBorderColor => Color.FromArgb(83, 131, 202);

	public override Color ErdSelectedNodeColor => Color.FromArgb(160, 187, 227);

	public override bool ErdUseNodeColorForHeaderBorder => true;

	public override Color ErdNodeBlue => ColorTranslator.FromHtml("#487AC6");

	public override Color ErdNodeGreen => Color.FromArgb(76, 175, 80);

	public override Color ErdNodeRed => ColorTranslator.FromHtml("#C62D2D");

	public override Color ErdNodeYellow => Color.FromArgb(201, 176, 35);

	public override Color ErdNodePurple => ColorTranslator.FromHtml("#6D57B0");

	public override Color ErdNodeOrange => ColorTranslator.FromHtml("#ED7D17");

	public override Color ErdNodeCyan => ColorTranslator.FromHtml("#00A1D0");

	public override Color ErdNodeLime => ColorTranslator.FromHtml("#BACE1F");

	public override Color ErdNodeDefault => Color.FromArgb(179, 179, 179);

	public override Color ErdFocusedColumnBackColor => Color.FromArgb(230, 215, 229, 242);

	public override Color TextFieldBackColor => ControlBackColor;

	public override Color TextFieldForeColor => ControlForeColor;

	public override Color HyperlinkInRichTextForeColor => Color.FromArgb(0, 0, 255);

	public override bool ScriptShouldTranslateColors => false;

	public override Color ScriptBackColor => SkinColors.KeyPaint;

	public override Color ScriptForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationNotRequiredForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationNotRequiredBackColor => SkinColors.ControlColorFromSystemColors;

	public override Color SynchronizeForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizeBackColor => ColorTranslator.FromHtml("#F9EBD0");

	public override Color SynchronizationSuccessfulForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationSuccessfulBackColor => Color.FromArgb(221, 240, 221);

	public override Color SynchronizationFailedForeColor => SkinColors.ControlForeColorFromSystemColors;

	public override Color SynchronizationFailedBackColor => Color.FromArgb(244, 225, 225);

	public override Color InfoTextForeColor => ControlPaint.Light(Color.LightGray, 0.5f);

	public override void SetLightThemeBackground(RichEditControl richEditControl, bool setForeColor = false)
	{
		richEditControl.ActiveView.BackColor = TextFieldBackColor;
		if (setForeColor)
		{
			richEditControl.Appearance.Text.ForeColor = Color.Black;
		}
	}

	public override void SetDarkThemeBackground(RichEditControl richEditControl, bool setForeColor = false)
	{
		VsDark vsDark = new VsDark();
		richEditControl.ActiveView.BackColor = vsDark.TextFieldBackColor;
		if (setForeColor)
		{
			richEditControl.Appearance.Text.ForeColor = Color.White;
		}
	}

	public override void TranslateColors(RichEditControl richEditControl)
	{
		SetLightThemeBackground(richEditControl);
	}

	public override string GetHtmlTranslatedColors(RichEditControl richEditControl)
	{
		return richEditControl.HtmlText;
	}

	public override Color GetNodeColor(string color)
	{
		Color nodeColor = base.GetNodeColor(color);
		return base.GetNodeColor(NodeColors.GetNodeColorString(nodeColor));
	}

	public override Color GetNodeColorLight(string color)
	{
		return ControlPaint.Light(base.GetNodeColor(color), 1.8f);
	}

	public override string ScriptTranslateHtmlColor(string htmlColor)
	{
		return htmlColor;
	}
}
