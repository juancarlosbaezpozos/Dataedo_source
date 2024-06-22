using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using DevExpress.XtraBars;

namespace Dataedo.App.UserControls.PanelControls;

public class ClassificatorGridPanelUserControl : GridPanelUserControl
{
	private IContainer components;

	private BarButtonItem collapseBarButtonItem;

	private BarButtonItem expandBarButtonItem;

	public ClassificatorGridPanelUserControl()
	{
		InitializeComponent();
		gridPanelBar.LinksPersistInfo.Add(new LinkPersistInfo(expandBarButtonItem));
		gridPanelBar.LinksPersistInfo.Add(new LinkPersistInfo(collapseBarButtonItem));
		columnChooserBarButtonItem.Visibility = BarItemVisibility.Never;
	}

	private void expandBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		base.GridView.CloseEditor();
		base.GridView.ExpandAllGroups();
	}

	private void collapseBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		base.GridView.CloseEditor();
		base.GridView.CollapseAllGroups();
	}

	public new void HideButtons()
	{
		BarButtonItem barButtonItem = removeBarButtonItem;
		BarButtonItem barButtonItem2 = defaultViewBarButtonItem;
		BarItemVisibility barItemVisibility2 = (customFieldsBarButtonItem.Visibility = BarItemVisibility.Never);
		BarItemVisibility barItemVisibility5 = (barButtonItem.Visibility = (barButtonItem2.Visibility = barItemVisibility2));
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
		this.collapseBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.expandBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		base.SuspendLayout();
		this.collapseBarButtonItem.Glyph = Dataedo.App.Properties.Resources.collapse_all_16;
		this.collapseBarButtonItem.Hint = "Collapse all";
		this.collapseBarButtonItem.Id = 19;
		this.collapseBarButtonItem.Name = "collapseBarButtonItem";
		this.collapseBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(collapseBarButtonItem_ItemClick);
		this.expandBarButtonItem.Glyph = Dataedo.App.Properties.Resources.expand_all_16;
		this.expandBarButtonItem.Hint = "Expand all";
		this.expandBarButtonItem.Id = 20;
		this.expandBarButtonItem.Name = "expandBarButtonItem";
		this.expandBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(expandBarButtonItem_ItemClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Name = "ClassificatorGridPanelUserControl";
		base.ResumeLayout(false);
	}
}
