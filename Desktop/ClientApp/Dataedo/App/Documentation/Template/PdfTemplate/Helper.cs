using System.Drawing;
using Dataedo.App.Classes.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;
using DevExpress.XtraReports.UI;

namespace Dataedo.App.Documentation.Template.PdfTemplate;

public class Helper
{
	private static string generalFontFamily = null;

	private static bool generalOnly = true;

	public static void ApplyData(RootDoc data, PdfTemplateModel template)
	{
		if (template == null)
		{
			return;
		}
		if (template != null && template.Customization?.TitlePage?.Header?.Text != null)
		{
			data.TitlePage.CompanyName = template.Customization.TitlePage.Header?.Text;
		}
		if (template != null && template.Customization?.TitlePage?.Title?.Text != null)
		{
			data.TitlePage.DatabaseTitle = template.Customization.TitlePage.Title?.Text;
		}
		if (template != null && template.Customization?.TitlePage?.Subtitle?.Text != null)
		{
			data.TitlePage.Subtitle = template.Customization.TitlePage.Subtitle?.Text;
		}
		if (template != null && template.Customization?.TitlePage?.Header?.Link != null)
		{
			data.TitlePage.CompanyNameNavigationUrl = template.Customization.TitlePage.Header?.Link;
		}
		if (template != null && template.Customization?.TitlePage?.Title?.Link != null)
		{
			data.TitlePage.DatabaseTitleNavigationUrl = template.Customization.TitlePage.Title?.Link;
		}
		if (template != null && template.Customization?.TitlePage?.Subtitle?.Link != null)
		{
			data.TitlePage.SubtitleNavigationUrl = template.Customization.TitlePage.Subtitle?.Link;
		}
		data.Footer.Left = template?.Customization?.Footer?.Left;
		data.Footer.Center = template?.Customization?.Footer?.Center;
		data.Footer.Right = template?.Customization?.Footer?.Right;
		data.TitlePage.ShowLogo = template?.Customization?.TitlePage?.Image?.ImageValue != null;
		if (template?.Customization?.TitlePage?.Image?.ImageValue != null)
		{
			data.TitlePage.Logo = template.Customization.TitlePage.Image.ImageValue;
			data.PutDatabaseLogo = false;
			if (template != null && template.Customization?.TitlePage?.Image?.Link != null)
			{
				data.TitlePage.LogoNavigationUrl = template.Customization.TitlePage.Image?.Link;
			}
			else
			{
				data.TitlePage.LogoNavigationUrl = null;
			}
		}
		else
		{
			data.TitlePage.Logo = null;
			data.TitlePage.LogoNavigationUrl = null;
		}
		if (data.IsEducationEdition)
		{
			data.Footer.Center = null;
		}
		if (data.PutDataedoLogo)
		{
			data.Footer.Right = null;
		}
		if (template == null)
		{
			return;
		}
		PdfTemplateModel.CustomizationModel customization = template.Customization;
		if (customization == null)
		{
			return;
		}
		PdfTemplateModel.CustomizationModel.TitlePageModel titlePage = customization.TitlePage;
		if (titlePage != null)
		{
			SimpleFontModelWithShow date = titlePage.Date;
			if (date != null && date.Show.HasValue)
			{
				data.TitlePage.ShowDate = template.Customization.TitlePage.Date.Show.Value;
			}
		}
	}

	public static void ApplyStyles(XtraReport report, PdfTemplateModel template, bool generalOnly)
	{
		Helper.generalOnly = generalOnly;
		generalFontFamily = ((template != null && template.Customization?.General?.FontFamily != null) ? template.Customization.General.FontFamily : null);
		if (report.FindControl("xrTableOfContents", ignoreCase: false) is XRTableOfContents xRTableOfContents)
		{
			if (template != null && template.Customization?.Localization?.Headings?.TableOfContents != null)
			{
				xRTableOfContents.LevelTitle.Text = template.Customization.Localization.Headings.TableOfContents;
			}
			xRTableOfContents.LevelTitle.ForeColor = SetColor(xRTableOfContents.LevelTitle.ForeColor, template?.Customization?.TableOfContents?.Title?.Font?.Color);
			xRTableOfContents.LevelTitle.Font = SetFont(xRTableOfContents.LevelTitle.Font, template?.Customization?.TableOfContents?.Title?.Font);
			xRTableOfContents.Levels[0].ForeColor = SetColor(xRTableOfContents.Levels[0].ForeColor, template?.Customization?.TableOfContents?.Level1?.Font?.Color);
			xRTableOfContents.Levels[0].Font = SetFont(xRTableOfContents.Levels[0].Font, template?.Customization?.TableOfContents?.Level1?.Font);
			xRTableOfContents.Levels[1].ForeColor = SetColor(xRTableOfContents.Levels[1].ForeColor, template?.Customization?.TableOfContents?.Level2?.Font?.Color);
			xRTableOfContents.Levels[1].Font = SetFont(xRTableOfContents.Levels[1].Font, template?.Customization?.TableOfContents?.Level2?.Font);
			xRTableOfContents.Levels[2].ForeColor = SetColor(xRTableOfContents.Levels[2].ForeColor, template?.Customization?.TableOfContents?.Level3?.Font?.Color);
			xRTableOfContents.Levels[2].Font = SetFont(xRTableOfContents.Levels[2].Font, template?.Customization?.TableOfContents?.Level3?.Font);
		}
		foreach (object item in report.StyleSheet)
		{
			if (item is XRControlStyle xRControlStyle)
			{
				if (xRControlStyle.Name == "titlePageHeaderStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.TitlePage?.Header?.Font);
				}
				else if (xRControlStyle.Name == "titlePageTitleStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.TitlePage?.Title?.Font);
				}
				else if (xRControlStyle.Name == "titlePageSubtitleStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.TitlePage?.Subtitle?.Font);
				}
				else if (xRControlStyle.Name == "titlePageTodayDateStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.TitlePage?.Date?.Font);
				}
				else if (xRControlStyle.Name == "cellDescriptionStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Legend?.Font);
				}
				else if (xRControlStyle.Name == "footerLeftLabelStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Footer?.Left?.Text?.Font);
				}
				else if (xRControlStyle.Name == "footerCenterLabelStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Footer?.Center?.Text?.Font);
				}
				else if (xRControlStyle.Name == "footerRightLabelStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Footer?.Right?.Text?.Font);
				}
				else if (xRControlStyle.Name == "footerLeftPageInfoStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Footer?.Left?.PageNumber?.Font);
				}
				else if (xRControlStyle.Name == "footerCenterPageInfoStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Footer?.Center?.PageNumber?.Font);
				}
				else if (xRControlStyle.Name == "footerRightPageInfoStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Footer?.Right?.PageNumber?.Font);
				}
				else if (xRControlStyle.Name == "cellHeaderMiddleXrControlStyle" || xRControlStyle.Name == "cellHeaderRightXrControlStyle")
				{
					xRControlStyle.BackColor = SetColor(xRControlStyle.BackColor, template?.Customization?.Tables?.Header?.BackgroundColor);
					xRControlStyle.BorderColor = SetColor(xRControlStyle.BorderColor, template?.Customization?.Tables?.Header?.BorderColor);
					SetFont(xRControlStyle, template?.Customization?.Tables?.Header?.Font);
				}
				else if (xRControlStyle.Name == "cellContentMiddleXrControlStyle" || xRControlStyle.Name == "cellContentRightXrControlStyle" || xRControlStyle.Name == "dependencyRootNameControlStyle" || xRControlStyle.Name == "dependencyNameControlStyle")
				{
					xRControlStyle.BackColor = SetColor(xRControlStyle.BackColor, template?.Customization?.Tables?.Body?.BackgroundColor);
					xRControlStyle.BorderColor = SetColor(xRControlStyle.BorderColor, template?.Customization?.Tables?.Body?.BorderColor);
					SetFont(xRControlStyle, template?.Customization?.Tables?.Body?.Font);
				}
				else if (xRControlStyle.Name == "cellContentTriggerScriptXrControlStyle")
				{
					xRControlStyle.BorderColor = SetColor(xRControlStyle.BorderColor, template?.Customization?.Tables?.Body?.BorderColor);
					SetFont(xRControlStyle, template?.Customization?.Tables?.Body?.Font);
				}
				else if (xRControlStyle.Name == "heading1XrControlStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Headings?.Heading1?.Font);
				}
				else if (xRControlStyle.Name == "heading2XrControlStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Headings?.Heading2?.Font);
				}
				else if (xRControlStyle.Name == "heading3XrControlStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Headings?.Heading3?.Font);
				}
				else if (xRControlStyle.Name == "heading4XrControlStyle")
				{
					SetFont(xRControlStyle, template?.Customization?.Headings?.Heading4?.Font);
				}
			}
		}
	}

	private static void SetFont(XRControlStyle styleTyped, FontModel fontModel)
	{
		styleTyped.ForeColor = SetColor(styleTyped.ForeColor, fontModel?.Color);
		styleTyped.Font = SetFont(styleTyped.Font, fontModel);
	}

	private static Font SetFont(Font font, FontModel fontModel)
	{
		string familyName = font.FontFamily.Name;
		float emSize = font.Size;
		bool flag = font.Bold;
		bool flag2 = font.Italic;
		bool flag3 = font.Underline;
		bool flag4 = font.Strikeout;
		if (!generalOnly && !string.IsNullOrEmpty(fontModel?.FontFamily))
		{
			familyName = fontModel.FontFamily;
		}
		else if (generalFontFamily != null)
		{
			familyName = generalFontFamily;
		}
		if (!generalOnly)
		{
			if (fontModel != null && fontModel.Size.HasValue)
			{
				emSize = (float)fontModel.Size.Value;
			}
			if (fontModel != null && fontModel.Bold.HasValue)
			{
				flag = fontModel.Bold.Value;
			}
			if (fontModel != null && fontModel.Italic.HasValue)
			{
				flag2 = fontModel.Italic.Value;
			}
			if (fontModel != null && fontModel.Underline.HasValue)
			{
				flag3 = fontModel.Underline.Value;
			}
			if (fontModel != null && fontModel.Strikethrough.HasValue)
			{
				flag4 = fontModel.Strikethrough.Value;
			}
		}
		FontStyle fontStyle = FontStyle.Regular;
		if (flag)
		{
			fontStyle |= FontStyle.Bold;
		}
		if (flag2)
		{
			fontStyle |= FontStyle.Italic;
		}
		if (flag3)
		{
			fontStyle |= FontStyle.Underline;
		}
		if (flag4)
		{
			fontStyle |= FontStyle.Strikeout;
		}
		return new Font(familyName, emSize, fontStyle);
	}

	private static Color SetColor(Color color, ArgbColorModel colorModel)
	{
		Color result = color;
		if (!generalOnly && colorModel != null && colorModel.Color.HasValue)
		{
			result = colorModel.Color.Value;
		}
		return result;
	}
}
