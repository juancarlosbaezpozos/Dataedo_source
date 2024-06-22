using System;
using System.Drawing;
using System.Text.RegularExpressions;
using ColorCode;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.PanelControls;
using DevExpress.XtraRichEdit.API.Native;

namespace Dataedo.App.Tools;

public static class RichEditUserControlHelper
{
	public static void ColorizeSyntax(RichEditUserControl destination, CodeColorizer codeColorizer, string sourceCode, string language)
	{
		sourceCode = sourceCode?.Trim();
		if (destination == null || codeColorizer == null || string.IsNullOrEmpty(sourceCode))
		{
			return;
		}
		if (language != "SQL")
		{
			destination.Text = sourceCode;
			return;
		}
		destination.HtmlText = codeColorizer.Colorize(sourceCode, Languages.Sql);
		if (SkinsManager.CurrentSkin.ScriptShouldTranslateColors)
		{
			int num;
			try
			{
				num = destination.HtmlText.IndexOf("</style>");
			}
			catch (StackOverflowException)
			{
				return;
			}
			if (num > 0)
			{
				string text = destination.HtmlText.Substring(0, num + 1);
				string text2 = destination.HtmlText.Substring(num - 1);
				string text3 = ColorTranslator.ToHtml(SkinsManager.CurrentSkin.ScriptBackColor);
				string text4 = ColorTranslator.ToHtml(SkinsManager.CurrentSkin.ScriptForeColor);
				text = text.Replace("color:#000000", "color:" + text4).Replace("background-color:#FFFFFF", "background-color:" + text3);
				text = Regex.Replace(text, "{color:(?!" + text4 + ")(#......)", (System.Text.RegularExpressions.Match x) => "{color:" + SkinsManager.CurrentSkin.ScriptTranslateHtmlColor(x.Groups[1].Value));
				destination.HtmlText = text + text2;
			}
		}
		Document document = destination.Document;
		document.BeginUpdate();
		foreach (Paragraph paragraph in document.Paragraphs)
		{
			if (!string.IsNullOrWhiteSpace(document.GetText(paragraph.Range)))
			{
				DocumentRange range = document.CreateRange(document.Range.Start, document.Paragraphs[paragraph.Index - 1].Range.End.ToInt());
				document.Delete(range);
				break;
			}
		}
		BasePanelControl.TrimEmptyParagraphs(document);
		destination.SetOriginalHtmlText();
		document.EndUpdate();
	}
}
