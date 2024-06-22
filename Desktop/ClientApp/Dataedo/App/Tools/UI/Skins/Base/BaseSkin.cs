using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Dataedo.App.Tools.Search;
using Dataedo.App.UserControls;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Tools.UI.Skins.Base;

internal abstract class BaseSkin
{
	public virtual Color DataedoColor => Color.FromArgb(70, 121, 198);

	public abstract string SkinName { get; }

	public abstract string Palette { get; }

	public abstract string ResourcesBaseName { get; }

	public abstract ResourceManager ResourceManager { get; }

	public abstract bool IsDarkTheme { get; }

	public abstract bool UseDefaultTitleBar { get; }

	public abstract Color ControlBackColor { get; }

	public abstract Color ControlForeColor { get; }

	public abstract Color ControlBorderColor { get; }

	public abstract Color LoginFormFocusedItemBackColor { get; }

	public abstract Color SuggestedDescriptionsMarkerColor { get; }

	public abstract Color DeletedObjectForeColor { get; }

	public abstract Color ToolTipSelectedColorForeColor { get; }

	public abstract Color ProgressPainterColor { get; }

	public abstract Color ProgressPainterTabColor { get; }

	public abstract Color ListSelectionBackColor { get; }

	public abstract Color ListSelectionForeColor { get; }

	public abstract Color LoginFormTitleForeColor { get; }

	public abstract Color LegacyTextExportBackColor { get; }

	public abstract Color BetaTextExportBackColor { get; }

	public abstract Color RecomendedExportBackColor { get; }

	public abstract Color LegacyTextExportForeColor { get; }

	public abstract Color BetaTextExportForeColor { get; }

	public abstract Color RecomendednExportForeColor { get; }

	public abstract Color DDLExampleBackColor { get; }

	public abstract Color DDLExampleForeColor { get; }

	public abstract Color OnboardingAddDatabaseButtonBackColor { get; }

	public abstract Color OnboardingAddDatabaseButtonForeColor { get; }

	public abstract Color DisabledForeColor { get; }

	public abstract Color YellowBackColor { get; }

	public abstract Color RedBackColor { get; }

	public abstract Color GreenBackColor { get; }

	public abstract Color OnboardingPanelBackColor { get; }

	public abstract Color OnboardingPanelForeColor { get; }

	public abstract Color OnboardingPanelCloseButtonHoverBackColor { get; }

	public abstract Color DisabledFunctionalitiesBackColor { get; }

	public abstract Color DisabledFunctionalitiesForeColor { get; }

	public abstract Color DeletedObjectPanelBackColor { get; }

	public abstract Color DeletedObjectPanelForeColor { get; }

	public abstract Color DisabledFunctionalityTextForeColor { get; }

	public abstract Color UpdatePanelForeColor { get; }

	public abstract Color UpdatePanelBackColor { get; }

	public abstract Color TreeListCustomFocusBackColor { get; }

	public abstract Color TreeListCustomFocusForeColor { get; }

	public abstract Color ProgressLoadingColumnBackColor { get; }

	public abstract Color ProgressObjectLoadingRepositoryItemProgressBarBackColor { get; }

	public abstract Color ProgressFolderLoadingRepositoryItemProgressBarBackColor { get; }

	public abstract Color ProgressModuleLoadingRepositoryItemProgressBarBackColor { get; }

	public abstract Color ProgressDatabaseLoadingRepositoryItemProgressBarBackColor { get; }

	public abstract Color ProgressDatabaseRepositoryItemProgressBarBackColor { get; }

	public abstract Color ProgressModuleRepositoryItemProgressBarBackColor { get; }

	public abstract Color ProgressFolderRepositoryItemProgressBarBackColor { get; }

	public abstract Color ProgressSimpleObjectRepositoryItemProgressBarBackColor { get; }

	public abstract string SearchHighlightHtmlColor { get; }

	public abstract string SearchHighlightHtmlForeColor { get; }

	public abstract Color SearchHighlightColor { get; }

	public abstract Color SearchHighlightForeColor { get; }

	public abstract Color SearchHighlightColorUnderSelectionColorNormal { get; }

	public abstract Color SearchHighlightColorUnderSelectionForeColorNormal { get; }

	public abstract Color SearchTabForeColorHighlighted { get; }

	public abstract Color GridGridRowBackColor { get; }

	public abstract Color GridGridRowForeColor { get; }

	public abstract Color GridNonEditableColumnsBackColor { get; }

	public abstract Color GridDisabledGridRowBackColor { get; }

	public abstract Color GridDisabledGridRowForeColor { get; }

	public abstract Color GridAccentGridRowBackColor { get; }

	public abstract Color GridAccentGridRowForeColor { get; }

	public abstract Color GridAccentRedGridRowForeColor { get; }

	public abstract Color GridHighlightRowBackColor { get; }

	public abstract Color GridSelectionBackColor { get; }

	public abstract Color GreyColor { get; }

	public abstract Color SchemaImportsAndChangesTreeAddedForeColor { get; }

	public abstract Color SchemaImportsAndChangesTreeUpdatedForeColor { get; }

	public abstract Color SchemaImportsAndChangesTreeDeletedForeColor { get; }

	public abstract Color SchemaImportsAndChangesPanelAddedForeColor { get; }

	public abstract Color SchemaImportsAndChangesPanelAddedBackColor { get; }

	public abstract Color SchemaImportsAndChangesPanelUpdatedForeColor { get; }

	public abstract Color SchemaImportsAndChangesPanelUpdatedBackColor { get; }

	public abstract Color SchemaImportsAndChangesPanelDeletedForeColor { get; }

	public abstract Color SchemaImportsAndChangesPanelDeletedBackColor { get; }

	public abstract Color DoubledRowBackColor { get; }

	public abstract Color ImportDescriptionsNoChangeForeColor { get; }

	public abstract Color ImportDescriptionsNoChangeBackColor { get; }

	public abstract Color ImportDescriptionsNewForeColor { get; }

	public abstract Color ImportDescriptionsNewBackColor { get; }

	public abstract Color ImportDescriptionsEraseForeColor { get; }

	public abstract Color ImportDescriptionsEraseBackColor { get; }

	public abstract Color ImportDescriptionsUpdateForeColor { get; }

	public abstract Color ImportDescriptionsUpdateBackColor { get; }

	public abstract Color ErdBackColor { get; }

	public abstract Color ErdERDGridColor { get; }

	public abstract Color ErdNodeBackColor { get; }

	public abstract Color ErdNodeBorderColor { get; }

	public abstract Color ErdNodeForeColor { get; }

	public abstract Color ErdLinkColor { get; }

	public abstract Color ErdLinkTextForeColor { get; }

	public abstract Color ErdERDHiddenRelationColor { get; }

	public abstract Color ErdGrayLight { get; }

	public abstract Color ErdGrayDark { get; }

	public abstract Color ErdSelectedNodeBorderColor { get; }

	public abstract Color ErdSelectedNodeColor { get; }

	public abstract bool ErdUseNodeColorForHeaderBorder { get; }

	public abstract Color ErdNodeBlue { get; }

	public abstract Color ErdNodeGreen { get; }

	public abstract Color ErdNodeRed { get; }

	public abstract Color ErdNodeYellow { get; }

	public abstract Color ErdNodePurple { get; }

	public abstract Color ErdNodeOrange { get; }

	public abstract Color ErdNodeCyan { get; }

	public abstract Color ErdNodeLime { get; }

	public abstract Color ErdNodeDefault { get; }

	public abstract Color ErdFocusedColumnBackColor { get; }

	public abstract Color TextFieldBackColor { get; }

	public abstract Color TextFieldForeColor { get; }

	public abstract Color HyperlinkInRichTextForeColor { get; }

	public abstract bool ScriptShouldTranslateColors { get; }

	public abstract Color ScriptBackColor { get; }

	public abstract Color ScriptForeColor { get; }

	public virtual Color SearchTabForeColorNotHighlighted => SkinColors.ControlForeColorFromSystemColors;

	public abstract Color SynchronizationNotRequiredForeColor { get; }

	public abstract Color SynchronizationNotRequiredBackColor { get; }

	public abstract Color SynchronizeForeColor { get; }

	public abstract Color SynchronizeBackColor { get; }

	public abstract Color SynchronizationSuccessfulForeColor { get; }

	public abstract Color SynchronizationSuccessfulBackColor { get; }

	public abstract Color SynchronizationFailedForeColor { get; }

	public abstract Color SynchronizationFailedBackColor { get; }

	public abstract Color InfoTextForeColor { get; }

	public BaseSkin()
	{
	}

	public Bitmap GetBitmap(string resourceName)
	{
		return (Bitmap)ResourceManager.GetObject(resourceName);
	}

	public abstract void SetLightThemeBackground(RichEditControl richEditControl, bool setForeColor = false);

	public abstract void SetDarkThemeBackground(RichEditControl richEditControl, bool setForeColor = false);

	public abstract void TranslateColors(RichEditControl richEditControl);

	public abstract string GetHtmlTranslatedColors(RichEditControl richEditControl);

	public virtual Color GetNodeColor(string color)
	{
		return color switch
		{
			"Blue" => ErdNodeBlue, 
			"Green" => ErdNodeGreen, 
			"Red" => ErdNodeRed, 
			"Yellow" => ErdNodeYellow, 
			"Purple" => ErdNodePurple, 
			"Orange" => ErdNodeOrange, 
			"Cyan" => ErdNodeCyan, 
			"Lime" => ErdNodeLime, 
			_ => ErdNodeDefault, 
		};
	}

	public virtual Color GetNodeColorLight(string color)
	{
		Color nodeColor = GetNodeColor(color);
		return ControlPaint.Light(SkinsManager.CurrentSkin.GetNodeColor(NodeColors.GetNodeColorString(nodeColor)), 1.2f);
	}

	public abstract string ScriptTranslateHtmlColor(string htmlColor);

	public static void SetSearchHighlight(BaseControl control)
	{
		control.BackColor = SearchSupport.HighlightColor;
		control.ForeColor = SearchSupport.HighlightForeColor;
	}

	public static void SetSearchHighlight(CustomFieldControl control)
	{
		control.ValueEditBackColor = SearchSupport.HighlightColor;
		control.ValueForeColor = SearchSupport.HighlightForeColor;
	}

	public static void SetSearchHighlight(BaseStyleControl control)
	{
		control.Appearance.BackColor = SearchSupport.HighlightColor;
		control.Appearance.ForeColor = SearchSupport.HighlightForeColor;
	}

	public static void SetSearchHighlightOrDefault(BaseStyleControl control, bool condition)
	{
		if (condition)
		{
			control.Appearance.BackColor = SearchSupport.HighlightColor;
			control.Appearance.ForeColor = SearchSupport.HighlightForeColor;
		}
		else
		{
			control.Appearance.BackColor = SkinColors.ControlColorFromSystemColors;
			control.Appearance.ForeColor = SkinColors.ControlForeColorFromSystemColors;
		}
	}
}
