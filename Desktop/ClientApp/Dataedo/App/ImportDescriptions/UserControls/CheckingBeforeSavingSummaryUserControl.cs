using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraLayout;

namespace Dataedo.App.ImportDescriptions.UserControls;

public class CheckingBeforeSavingSummaryUserControl : BaseUserControl
{
	private IEnumerable<ImportDataModel> modelsGeneral;

	private GridControl gridControl;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private HyperlinkLabelControl eraseNoneHLC;

	private HyperlinkLabelControl updateNoneHLC;

	private HyperlinkLabelControl addNoneHLC;

	private HyperlinkLabelControl eraseAllHLC;

	private HyperlinkLabelControl updateAllHLC;

	private HyperlinkLabelControl addAllHLC;

	private LabelControl unchangedLabelControl;

	private LabelControl changedLabelControl;

	private LabelControl removedLabelControl;

	private LabelControl newLabelControl;

	private LayoutControlGroup Root;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem5;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlItem layoutControlItem7;

	private LayoutControlItem layoutControlItem8;

	private LayoutControlItem layoutControlItem9;

	private LayoutControlItem layoutControlItem10;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	public CheckingBeforeSavingSummaryUserControl()
	{
		InitializeComponent();
		addAllHLC.HyperlinkClick += delegate
		{
			addAllHyperLinkEdit_OpenLink();
		};
		addNoneHLC.HyperlinkClick += delegate
		{
			addNoneHyperLinkEdit_OpenLink();
		};
		eraseAllHLC.HyperlinkClick += delegate
		{
			eraseAllHyperLinkEdit_OpenLink();
		};
		eraseNoneHLC.HyperlinkClick += delegate
		{
			eraseNoneHyperLinkEdit_OpenLink();
		};
		updateAllHLC.HyperlinkClick += delegate
		{
			overwriteAllHyperLinkEdit_OpenLink();
		};
		updateNoneHLC.HyperlinkClick += delegate
		{
			overwriteNoneHyperLinkEdit_OpenLink();
		};
	}

	public void Initialize(GridControl _gridControl, List<FieldDefinition> _fieldDefinitions, IEnumerable<ImportDataModel> _modelsGeneral)
	{
		gridControl = _gridControl;
		modelsGeneral = _modelsGeneral;
		SetInfo();
	}

	private void eraseNoneHyperLinkEdit_OpenLink()
	{
		modelsGeneral.ForEach(delegate(ImportDataModel x)
		{
			x.UnselectAll(ChangeEnum.Change.Erase);
		});
		gridControl.RefreshDataSource();
		SetInfo();
	}

	private void eraseAllHyperLinkEdit_OpenLink()
	{
		modelsGeneral.ForEach(delegate(ImportDataModel x)
		{
			x.SelectAll(ChangeEnum.Change.Erase, ignoreEmptyOverwriteValues: false);
		});
		gridControl.RefreshDataSource();
		SetInfo();
	}

	private void overwriteNoneHyperLinkEdit_OpenLink()
	{
		modelsGeneral.ForEach(delegate(ImportDataModel x)
		{
			x.UnselectAll(ChangeEnum.Change.Update);
		});
		gridControl.RefreshDataSource();
		SetInfo();
	}

	private void overwriteAllHyperLinkEdit_OpenLink()
	{
		modelsGeneral.ForEach(delegate(ImportDataModel x)
		{
			x.SelectAll(ChangeEnum.Change.Update, ignoreEmptyOverwriteValues: true);
		});
		gridControl.RefreshDataSource();
		SetInfo();
	}

	private void addNoneHyperLinkEdit_OpenLink()
	{
		modelsGeneral.ForEach(delegate(ImportDataModel x)
		{
			x.UnselectAll(ChangeEnum.Change.New);
		});
		gridControl.RefreshDataSource();
		SetInfo();
	}

	private void addAllHyperLinkEdit_OpenLink()
	{
		modelsGeneral.ForEach(delegate(ImportDataModel x)
		{
			x.SelectAll(ChangeEnum.Change.New, ignoreEmptyOverwriteValues: true);
		});
		gridControl.RefreshDataSource();
		SetInfo();
	}

	public void SetInfo()
	{
		newLabelControl.Text = GetSingleInfo(ChangeEnum.Change.New, "new", "added");
		changedLabelControl.Text = GetSingleInfo(ChangeEnum.Change.Update, "changed", "overwritten");
		removedLabelControl.Text = GetSingleInfo(ChangeEnum.Change.Erase, "removed", "erased");
		Color textColor = ChangeEnum.GetTextColor(ChangeEnum.Change.NoChange, isSelected: true);
		unchangedLabelControl.Text = $"{modelsGeneral.Sum((ImportDataModel x) => x.CountAllValues(ChangeEnum.Change.NoChange))} " + $"<color={textColor.R}, {textColor.G}, {textColor.B}>unchaged</color>" + " fields will be ignored.";
	}

	private string GetSingleInfo(ChangeEnum.Change change, string text1, string text2)
	{
		int num = modelsGeneral.Sum((ImportDataModel x) => x.CountAllValues(change));
		int num2 = modelsGeneral.Sum((ImportDataModel x) => x.CountSelectedValues(change));
		Color textColor = ChangeEnum.GetTextColor(change, isSelected: true);
		return "<b>" + ((num2 == num) ? "All" : ((num2 == 0) ? "None" : num2.ToString())) + "</b>" + $" out of {num} " + $"<color={textColor.R}, {textColor.G}, {textColor.B}>{text1}</color>" + " values will be " + text2 + ".";
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
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.eraseNoneHLC = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.updateNoneHLC = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.addNoneHLC = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.eraseAllHLC = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.updateAllHLC = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.addAllHLC = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.unchangedLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.changedLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.removedLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.newLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.eraseNoneHLC);
		this.nonCustomizableLayoutControl1.Controls.Add(this.updateNoneHLC);
		this.nonCustomizableLayoutControl1.Controls.Add(this.addNoneHLC);
		this.nonCustomizableLayoutControl1.Controls.Add(this.eraseAllHLC);
		this.nonCustomizableLayoutControl1.Controls.Add(this.updateAllHLC);
		this.nonCustomizableLayoutControl1.Controls.Add(this.addAllHLC);
		this.nonCustomizableLayoutControl1.Controls.Add(this.unchangedLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.changedLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.removedLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.newLabelControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1346, 373, 812, 500);
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(1079, 126);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.eraseNoneHLC.Location = new System.Drawing.Point(621, 62);
		this.eraseNoneHLC.Name = "eraseNoneHLC";
		this.eraseNoneHLC.Size = new System.Drawing.Size(196, 21);
		this.eraseNoneHLC.StyleController = this.nonCustomizableLayoutControl1;
		this.eraseNoneHLC.TabIndex = 13;
		this.eraseNoneHLC.Text = "Erase none (default)";
		this.updateNoneHLC.Location = new System.Drawing.Point(621, 37);
		this.updateNoneHLC.Name = "updateNoneHLC";
		this.updateNoneHLC.Size = new System.Drawing.Size(196, 21);
		this.updateNoneHLC.StyleController = this.nonCustomizableLayoutControl1;
		this.updateNoneHLC.TabIndex = 12;
		this.updateNoneHLC.Text = "Update none (default)";
		this.addNoneHLC.Location = new System.Drawing.Point(621, 12);
		this.addNoneHLC.Name = "addNoneHLC";
		this.addNoneHLC.Size = new System.Drawing.Size(196, 21);
		this.addNoneHLC.StyleController = this.nonCustomizableLayoutControl1;
		this.addNoneHLC.TabIndex = 11;
		this.addNoneHLC.Text = "Add none";
		this.eraseAllHLC.Location = new System.Drawing.Point(441, 62);
		this.eraseAllHLC.Name = "eraseAllHLC";
		this.eraseAllHLC.Size = new System.Drawing.Size(156, 21);
		this.eraseAllHLC.StyleController = this.nonCustomizableLayoutControl1;
		this.eraseAllHLC.TabIndex = 10;
		this.eraseAllHLC.Text = "Erase all";
		this.updateAllHLC.Location = new System.Drawing.Point(441, 37);
		this.updateAllHLC.Name = "updateAllHLC";
		this.updateAllHLC.Size = new System.Drawing.Size(156, 21);
		this.updateAllHLC.StyleController = this.nonCustomizableLayoutControl1;
		this.updateAllHLC.TabIndex = 9;
		this.updateAllHLC.Text = "Update all";
		this.addAllHLC.Location = new System.Drawing.Point(441, 12);
		this.addAllHLC.Name = "addAllHLC";
		this.addAllHLC.Size = new System.Drawing.Size(176, 21);
		this.addAllHLC.StyleController = this.nonCustomizableLayoutControl1;
		this.addAllHLC.TabIndex = 8;
		this.addAllHLC.Text = "Add all (default)";
		this.unchangedLabelControl.AllowHtmlString = true;
		this.unchangedLabelControl.Appearance.Options.UseTextOptions = true;
		this.unchangedLabelControl.Location = new System.Drawing.Point(12, 87);
		this.unchangedLabelControl.Name = "unchangedLabelControl";
		this.unchangedLabelControl.Size = new System.Drawing.Size(133, 16);
		this.unchangedLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.unchangedLabelControl.TabIndex = 7;
		this.unchangedLabelControl.Text = "unchangedLabelControl";
		this.changedLabelControl.AllowHtmlString = true;
		this.changedLabelControl.Appearance.Options.UseTextOptions = true;
		this.changedLabelControl.Location = new System.Drawing.Point(12, 39);
		this.changedLabelControl.Name = "changedLabelControl";
		this.changedLabelControl.Size = new System.Drawing.Size(325, 16);
		this.changedLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.changedLabelControl.TabIndex = 6;
		this.changedLabelControl.Text = "None out of 1000000 changed values will be overwritten.";
		this.removedLabelControl.AllowHtmlString = true;
		this.removedLabelControl.Appearance.Options.UseTextOptions = true;
		this.removedLabelControl.Location = new System.Drawing.Point(12, 64);
		this.removedLabelControl.Name = "removedLabelControl";
		this.removedLabelControl.Size = new System.Drawing.Size(121, 16);
		this.removedLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.removedLabelControl.TabIndex = 5;
		this.removedLabelControl.Text = "removedLabelControl";
		this.newLabelControl.AllowHtmlString = true;
		this.newLabelControl.Appearance.Options.UseTextOptions = true;
		this.newLabelControl.Location = new System.Drawing.Point(12, 14);
		this.newLabelControl.Name = "newLabelControl";
		this.newLabelControl.Size = new System.Drawing.Size(95, 16);
		this.newLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.newLabelControl.TabIndex = 4;
		this.newLabelControl.Text = "newLabelControl";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[14]
		{
			this.layoutControlItem4, this.layoutControlItem5, this.layoutControlItem6, this.layoutControlItem3, this.layoutControlItem7, this.layoutControlItem8, this.layoutControlItem9, this.layoutControlItem10, this.layoutControlItem2, this.emptySpaceItem2,
			this.layoutControlItem1, this.emptySpaceItem1, this.emptySpaceItem3, this.emptySpaceItem4
		});
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(1079, 126);
		this.Root.TextVisible = false;
		this.layoutControlItem4.ContentVertAlignment = DevExpress.Utils.VertAlignment.Top;
		this.layoutControlItem4.Control = this.unchangedLabelControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 75);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(809, 31);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.addAllHLC;
		this.layoutControlItem5.Location = new System.Drawing.Point(429, 0);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(180, 25);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(180, 25);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(180, 25);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem6.Control = this.updateAllHLC;
		this.layoutControlItem6.Location = new System.Drawing.Point(429, 25);
		this.layoutControlItem6.MaxSize = new System.Drawing.Size(160, 25);
		this.layoutControlItem6.MinSize = new System.Drawing.Size(160, 25);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(180, 25);
		this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem3.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.layoutControlItem3.Control = this.changedLabelControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 25);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(329, 25);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem7.Control = this.eraseAllHLC;
		this.layoutControlItem7.Location = new System.Drawing.Point(429, 50);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(160, 25);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(160, 25);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(180, 25);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem8.Control = this.addNoneHLC;
		this.layoutControlItem8.Location = new System.Drawing.Point(609, 0);
		this.layoutControlItem8.MaxSize = new System.Drawing.Size(200, 25);
		this.layoutControlItem8.MinSize = new System.Drawing.Size(200, 25);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(200, 25);
		this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.layoutControlItem9.Control = this.updateNoneHLC;
		this.layoutControlItem9.Location = new System.Drawing.Point(609, 25);
		this.layoutControlItem9.MaxSize = new System.Drawing.Size(200, 25);
		this.layoutControlItem9.MinSize = new System.Drawing.Size(200, 25);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(200, 25);
		this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem10.Control = this.eraseNoneHLC;
		this.layoutControlItem10.Location = new System.Drawing.Point(609, 50);
		this.layoutControlItem10.MaxSize = new System.Drawing.Size(200, 25);
		this.layoutControlItem10.MinSize = new System.Drawing.Size(200, 25);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.Size = new System.Drawing.Size(200, 25);
		this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.layoutControlItem2.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.layoutControlItem2.Control = this.removedLabelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 50);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(329, 25);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(809, 0);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(250, 106);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.layoutControlItem1.Control = this.newLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(329, 25);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(329, 0);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(100, 25);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(329, 25);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(100, 25);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(329, 50);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(100, 0);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(100, 10);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(100, 25);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.Name = "CheckingBeforeSavingSummaryUserControl";
		base.Size = new System.Drawing.Size(1079, 126);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		base.ResumeLayout(false);
	}
}
