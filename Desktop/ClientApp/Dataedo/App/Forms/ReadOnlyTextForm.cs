using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Forms;

public class ReadOnlyTextForm : BaseXtraForm
{
	private IContainer components;

	private RichEditUserControl richEditUserControl;

	public ReadOnlyTextForm()
	{
		InitializeComponent();
	}

	public ReadOnlyTextForm(string formTitle, string text)
	{
		InitializeComponent();
		richEditUserControl.RefreshSkin();
		richEditUserControl.Text = text;
		Text = formTitle;
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
		this.richEditUserControl = new Dataedo.App.UserControls.RichEditUserControl();
		base.SuspendLayout();
		this.richEditUserControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.richEditUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richEditUserControl.IsHighlighted = false;
		this.richEditUserControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.richEditUserControl.Location = new System.Drawing.Point(0, 0);
		this.richEditUserControl.Name = "richEditUserControl";
		this.richEditUserControl.OccurrencesCount = 0;
		this.richEditUserControl.Options.Behavior.UseThemeFonts = false;
		this.richEditUserControl.Options.DocumentCapabilities.Comments = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.richEditUserControl.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
		this.richEditUserControl.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
		this.richEditUserControl.OriginalHtmlText = null;
		this.richEditUserControl.ReadOnly = true;
		this.richEditUserControl.ShowCaretInReadOnly = false;
		this.richEditUserControl.Size = new System.Drawing.Size(763, 967);
		this.richEditUserControl.TabIndex = 0;
		this.richEditUserControl.Views.DraftView.Padding = new System.Windows.Forms.Padding(3, 4, 0, 0);
		this.richEditUserControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(3, 4, 4, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(763, 967);
		base.Controls.Add(this.richEditUserControl);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.Name = "ReadOnlyTextForm";
		base.ResumeLayout(false);
	}
}
