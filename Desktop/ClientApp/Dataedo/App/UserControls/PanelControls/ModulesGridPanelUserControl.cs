using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Helpers;
using Dataedo.App.UserControls.WindowControls;
using DevExpress.XtraBars;

namespace Dataedo.App.UserControls.PanelControls;

public class ModulesGridPanelUserControl : GridPanelUserControl
{
	private readonly int filterCheckItemIndex = 3;

	private MetadataEditorUserControl mainControl;

	private IContainer components;

	private BarButtonItem moveDownBarButtonItem;

	private BarButtonItem moveUpBarButtonItem;

	private BarButtonItem moveToTopBarButtonItem;

	private BarButtonItem moveToBottomBarButtonItem;

	private BarButtonItem sortAlphabeticallyBarButtonItem;

	public ModulesGridPanelUserControl()
	{
		InitializeComponent();
		gridPanelBar.LinksPersistInfo.Insert(filterCheckItemIndex, new LinkPersistInfo(moveUpBarButtonItem));
		gridPanelBar.LinksPersistInfo.Insert(filterCheckItemIndex + 1, new LinkPersistInfo(moveDownBarButtonItem));
		gridPanelBar.LinksPersistInfo.Insert(filterCheckItemIndex + 2, new LinkPersistInfo(moveToTopBarButtonItem));
		gridPanelBar.LinksPersistInfo.Insert(filterCheckItemIndex + 3, new LinkPersistInfo(moveToBottomBarButtonItem));
		gridPanelBar.LinksPersistInfo.Insert(filterCheckItemIndex + 4, new LinkPersistInfo(sortAlphabeticallyBarButtonItem));
	}

	public void SetMainControl(MetadataEditorUserControl meuc)
	{
		mainControl = meuc;
	}

	private void moveDownBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ModuleSummaryHelper.MoveModuleDownOnGridView(base.GridView, mainControl, base.ParentForm);
	}

	private void moveUpBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ModuleSummaryHelper.MoveModuleUpOnGridView(base.GridView, mainControl, base.ParentForm);
	}

	private void sortAlphabeticallyBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		mainControl?.SortModulesAlphabetically(fromCustomFocus: false);
	}

	private void moveToBottomBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ModuleSummaryHelper.MoveModuleToBottomOnGrid(base.GridView, mainControl, base.ParentForm);
	}

	private void moveToTopBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ModuleSummaryHelper.MoveModuleToTopOnGrid(base.GridView, mainControl, base.ParentForm);
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
		this.moveDownBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveUpBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveToTopBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveToBottomBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.sortAlphabeticallyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		base.SuspendLayout();
		this.moveDownBarButtonItem.Caption = "";
		this.moveDownBarButtonItem.Hint = "Move down";
		this.moveDownBarButtonItem.Id = 19;
		this.moveDownBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
		this.moveDownBarButtonItem.Name = "moveDownBarButtonItem";
		this.moveDownBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveDownBarButtonItem_ItemClick);
		this.moveUpBarButtonItem.Caption = "";
		this.moveUpBarButtonItem.Hint = "Move up";
		this.moveUpBarButtonItem.Id = 20;
		this.moveUpBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
		this.moveUpBarButtonItem.Name = "moveUpBarButtonItem";
		this.moveUpBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveUpBarButtonItem_ItemClick);
		this.moveToTopBarButtonItem.Caption = "";
		this.moveToTopBarButtonItem.Hint = "Move to top";
		this.moveToTopBarButtonItem.Id = 21;
		this.moveToTopBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_top_16;
		this.moveToTopBarButtonItem.Name = "moveToTopBarButtonItem";
		this.moveToTopBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveToTopBarButtonItem_ItemClick);
		this.moveToBottomBarButtonItem.Caption = "";
		this.moveToBottomBarButtonItem.Hint = "Move to bottom";
		this.moveToBottomBarButtonItem.Id = 22;
		this.moveToBottomBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_bottom_16;
		this.moveToBottomBarButtonItem.Name = "moveToBottomBarButtonItem";
		this.moveToBottomBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveToBottomBarButtonItem_ItemClick);
		this.sortAlphabeticallyBarButtonItem.Caption = "";
		this.sortAlphabeticallyBarButtonItem.Hint = "Sort alphabetically";
		this.sortAlphabeticallyBarButtonItem.Id = 23;
		this.sortAlphabeticallyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sort_asc_16;
		this.sortAlphabeticallyBarButtonItem.Name = "sortAlphabeticallyBarButtonItem";
		this.sortAlphabeticallyBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(sortAlphabeticallyBarButtonItem_ItemClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Name = "ModulesGridPanelUserControl";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
