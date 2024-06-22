using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.UI;

namespace Dataedo.App.UserControls;

public class RichEditUserControl : RichEditControl
{
	private string[] lastSearchWords;

	private BarManager barManager;

	public string OriginalHtmlText { get; set; }

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
				return $"({OccurrencesCount} occurrences) - use ctrl+F to find a specific appearance";
			}
			return string.Empty;
		}
	}

	public event EventHandler ContentChangedEvent;

	public void SetOriginalHtmlText()
	{
		OriginalHtmlText = base.HtmlText;
	}

	public RichEditUserControl()
	{
		InitializeComponent();
		InitializeBarManager();
		RemoveShortcutKey(Keys.Control, Keys.O);
	}

	public void RefreshSkin()
	{
		base.ActiveView.BackColor = SkinsManager.CurrentSkin.TextFieldBackColor;
		base.Document.DefaultCharacterProperties.ForeColor = SkinsManager.CurrentSkin.TextFieldForeColor;
	}

	public void SetCLRTriggerText()
	{
		Text = "The script is unavailable for assembly (CLR) triggers.";
	}

	public bool Highlight()
	{
		return Highlight(lastSearchWords);
	}

	public bool Highlight(string[] searchWords)
	{
		lastSearchWords = searchWords;
		List<DocumentRange> list = new List<DocumentRange>();
		if (searchWords != null)
		{
			foreach (string textToFind in searchWords)
			{
				DocumentRange[] array = base.Document.FindAll(textToFind, SearchOptions.None);
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
		OccurrencesCount = 0;
		return false;
	}

	public void ClearHighlights()
	{
		base.HtmlText = OriginalHtmlText;
		OccurrencesCount = 0;
		IsHighlighted = false;
	}

	private void InitializeBarManager()
	{
		barManager = new BarManager();
		BarDockControl barDockControl = new BarDockControl();
		barDockControl.CausesValidation = false;
		barDockControl.Dock = DockStyle.Top;
		barDockControl.Location = new Point(0, 0);
		barDockControl.Margin = new Padding(3, 4, 3, 4);
		barDockControl.Size = new Size(825, 31);
		FindItem item = new FindItem();
		EditingBar editingBar = new EditingBar();
		editingBar.Control = this;
		editingBar.DockCol = 3;
		editingBar.DockRow = 0;
		editingBar.DockStyle = BarDockStyle.Top;
		editingBar.LinksPersistInfo.AddRange(new LinkPersistInfo[1]
		{
			new LinkPersistInfo(item)
		});
		editingBar.OptionsBar.AllowQuickCustomization = false;
		editingBar.OptionsBar.DisableCustomization = true;
		editingBar.OptionsBar.DrawDragBorder = false;
		barManager.DockControls.Add(barDockControl);
		barManager.AllowCustomization = false;
		barManager.AllowQuickCustomization = false;
		barManager.Form = this;
		barManager.Bars.Add(editingBar);
		barManager.Items.Add(item);
		base.MenuManager = barManager;
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		base.Options.DocumentCapabilities.Comments = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		base.Options.Behavior.UseThemeFonts = false;
		base.ContentChanged += new System.EventHandler(RichEditUserControl_ContentChanged);
		base.ResumeLayout(false);
	}

	private void RichEditUserControl_ContentChanged(object sender, EventArgs e)
	{
		if (base.ContainsFocus)
		{
			this.ContentChangedEvent?.Invoke(this, e);
		}
	}
}
