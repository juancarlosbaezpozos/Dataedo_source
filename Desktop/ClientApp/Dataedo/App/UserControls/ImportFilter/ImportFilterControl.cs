using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ImportFilter;

public class ImportFilterControl : BaseUserControl
{
	private bool enabled;

	private bool fullReimportVisible;

	private IContainer components;

	private FlowLayoutPanel rulesFlowLayoutPanel;

	private LabelControl includeLabelControl;

	private LabelControl includeAddRuleLabelControl;

	private LabelControl excludeLabelControl;

	private LabelControl excludeAddRuleLabelControl;

	private LabelControl clearLabelControl;

	private NonCustomizableLayoutControl layoutControl2;

	private LayoutControlGroup Root;

	private LayoutControlItem filterLayoutControlItem;

	private LayoutControlItem clearFilterLayoutControlItem;

	private LabelControl includeFilterStatusLabelControl;

	private CheckEdit fullReimportCheckEdit;

	private LayoutControlItem reimportCheckEditLayoutControlItem;

	private LabelControl importModeLabelControl;

	private LayoutControlItem optionsLayoutControlItem;

	private EmptySpaceItem reimportEmptySpaceItem;

	private NonCustomizableLayoutControl layoutControl1;

	private LabelControl labelControl1;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem5;

	private LayoutControlItem layoutControlItem7;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private NonCustomizableLayoutControl includeLabelLayoutControl;

	private LayoutControlGroup layoutControlGroup2;

	private LayoutControlItem layoutControlItem8;

	private ToolTipControllerUserControl toolTipController;

	private HelpIconUserControl fullReimportHelpIconUserControl;

	private LayoutControlItem fullReimportHelpIconLayoutControlItem;

	private HelpIconUserControl rulesHelpIconUserControl;

	private LayoutControlItem rulesLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	public new bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			foreach (Control control in base.Controls)
			{
				if (control is FilterRowControl)
				{
					control.Enabled = value;
				}
			}
			enabled = value;
		}
	}

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; set; }

	public bool? IsFilterControlTransparent => GetRulesCollection()?.IsTransparent;

	public bool IsFullReimport => fullReimportCheckEdit.Checked;

	public bool FullReimportVisible
	{
		get
		{
			return fullReimportVisible;
		}
		set
		{
			bool flag2 = (optionsLayoutControlItem.ContentVisible = value);
			fullReimportVisible = flag2;
			LayoutControlItem layoutControlItem = reimportCheckEditLayoutControlItem;
			LayoutControlItem layoutControlItem2 = fullReimportHelpIconLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (reimportEmptySpaceItem.Visibility = ((!value) ? LayoutVisibility.Never : LayoutVisibility.Always));
			LayoutVisibility layoutVisibility5 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility2));
		}
	}

	public bool SaveChangesVisible { get; set; }

	private bool excludeRulesControlsVisible => GetRulesCollection().HasExcludedRules;

	private IEnumerable<FilterRowControl> ruleControls
	{
		get
		{
			foreach (object control in rulesFlowLayoutPanel.Controls)
			{
				if (control is FilterRowControl filterRowControl)
				{
					yield return filterRowControl;
				}
			}
		}
	}

	private int nextIncludeControlIndex => rulesFlowLayoutPanel.Controls.IndexOf(layoutControl1);

	private int nextExcludeControlIndex => rulesFlowLayoutPanel.Controls.IndexOf(excludeAddRuleLabelControl);

	[Browsable(true)]
	public event Action ClearEvent;

	public event Action FilterChangedByUserEvent;

	public FilterRulesCollection GetRulesCollection()
	{
		FilterRulesCollection filterRulesCollection = new FilterRulesCollection(ruleControls.Select((FilterRowControl x) => new FilterRule(x)));
		if (filterRulesCollection.Rules.Count((FilterRule x) => x.RuleType == FilterRuleType.Include) == 0)
		{
			filterRulesCollection.Rules.Add(new FilterRule(FilterRuleType.Include, FilterObjectTypeEnum.FilterObjectType.Any, string.Empty, string.Empty));
		}
		return filterRulesCollection;
	}

	public ImportFilterControl()
	{
		InitializeComponent();
		ClearEvent += ImportFilterControl_ClearEvent;
		FilterChangedByUserEvent += ImportFilterControl_FilterChangedByUserEvent;
		RefreshExcludeRulesControlsVisible();
	}

	private void ImportFilterControl_FilterChangedByUserEvent()
	{
		SaveChangesVisible = true;
		includeFilterStatusLabelControl.Visible = IsFilterControlTransparent == true;
		RefreshExcludeRulesControlsVisible();
	}

	public void SetFullReimportSwitchable(bool forceFullReimport, string dbName)
	{
		string text = "Dataedo imports changes (updated, new and deleted objects)." + Environment.NewLine + "Use this option if you need a full reimport of all schema objects or import extended properties." + Environment.NewLine + "Your existing descriptions will be preserved.";
		if (forceFullReimport)
		{
			fullReimportCheckEdit.Checked = true;
			fullReimportCheckEdit.Enabled = false;
			fullReimportHelpIconUserControl.ToolTipText = text + Environment.NewLine + "<b>Note for " + dbName + ":</b> There are some issues with " + dbName + " tracking objects’ last update timestamps and this is why Dataedo always imports all objects to make sure all changes are included.";
		}
		else
		{
			fullReimportCheckEdit.Checked = false;
			fullReimportCheckEdit.Enabled = true;
			fullReimportHelpIconUserControl.ToolTipText = text;
		}
	}

	private void ImportFilterControl_ClearEvent()
	{
		ClearFilter();
	}

	private void includeAddRuleLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			AddRule(FilterRuleType.Include);
			this.FilterChangedByUserEvent?.Invoke();
			includeAddRuleLabelControl.Focus();
		}
	}

	private void excludeAddRuleLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			AddExceludeRuleControl();
		}
	}

	private void AddExceludeRuleControl()
	{
		AddRule(FilterRuleType.Exclude);
		this.FilterChangedByUserEvent?.Invoke();
		excludeAddRuleLabelControl.Focus();
	}

	private void AddRule(FilterRuleType type)
	{
		AddControl(new FilterRowControl(type, DatabaseType));
	}

	private void AddRule(FilterRule rule)
	{
		AddControl(new FilterRowControl(rule, DatabaseType));
	}

	private void AddControl(FilterRowControl control)
	{
		rulesFlowLayoutPanel.Controls.Add(control);
		switch (control.RuleType)
		{
		case FilterRuleType.Include:
			rulesFlowLayoutPanel.Controls.SetChildIndex(control, nextIncludeControlIndex);
			rulesFlowLayoutPanel.ScrollControlIntoView(layoutControl1);
			break;
		case FilterRuleType.Exclude:
			rulesFlowLayoutPanel.Controls.SetChildIndex(control, nextExcludeControlIndex);
			rulesFlowLayoutPanel.ScrollControlIntoView(excludeAddRuleLabelControl);
			break;
		}
		control.RemoveClick += delegate(object s, HyperlinkClickEventArgs e)
		{
			RemoveClick(control, e);
		};
		control.FilterChangedByUserEvent += delegate
		{
			this.FilterChangedByUserEvent?.Invoke();
		};
		for (int i = 0; i < rulesFlowLayoutPanel.Controls.Count; i++)
		{
			rulesFlowLayoutPanel.Controls[i].TabIndex = i;
		}
		control.Focus();
	}

	private void RemoveClick(FilterRowControl control, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			rulesFlowLayoutPanel.Controls.Remove(control);
			this.FilterChangedByUserEvent?.Invoke();
		}
	}

	private void clearLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			InvokeClearFilter();
			this.FilterChangedByUserEvent?.Invoke();
		}
	}

	public void InvokeClearFilter()
	{
		this.ClearEvent?.Invoke();
	}

	private void ClearFilter()
	{
		rulesFlowLayoutPanel.Controls.Clear();
		rulesFlowLayoutPanel.Controls.Add(includeLabelLayoutControl);
		rulesFlowLayoutPanel.Controls.Add(includeFilterStatusLabelControl);
		rulesFlowLayoutPanel.Controls.Add(layoutControl1);
		rulesFlowLayoutPanel.Controls.Add(excludeLabelControl);
		rulesFlowLayoutPanel.Controls.Add(excludeAddRuleLabelControl);
		includeLabelControl.Focus();
	}

	internal void SetRulesCollection(FilterRulesCollection filter)
	{
		ClearFilter();
		foreach (FilterRule rule in filter.Rules)
		{
			AddRule(rule);
		}
		SaveChangesVisible = false;
		includeFilterStatusLabelControl.Visible = IsFilterControlTransparent == true;
		RefreshExcludeRulesControlsVisible();
	}

	private void labelControl1_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			AddExceludeRuleControl();
			RefreshExcludeRulesControlsVisible();
		}
	}

	public void RefreshExcludeRulesControlsVisible()
	{
		layoutControlItem7.ContentVisible = !excludeRulesControlsVisible;
		LabelControl labelControl = excludeLabelControl;
		bool visible = (excludeAddRuleLabelControl.Visible = excludeRulesControlsVisible);
		labelControl.Visible = visible;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.ImportFilter.ImportFilterControl));
		this.layoutControl2 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.fullReimportHelpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.importModeLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.fullReimportCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.rulesFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.includeLabelLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.includeLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rulesHelpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.rulesLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.includeFilterStatusLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.includeAddRuleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.excludeLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.excludeAddRuleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.clearLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.filterLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.reimportCheckEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.reimportEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.optionsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.clearFilterLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.fullReimportHelpIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new Dataedo.App.UserControls.ToolTipControllerUserControl(this.components);
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		this.layoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.fullReimportCheckEdit.Properties).BeginInit();
		this.rulesFlowLayoutPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.includeLabelLayoutControl).BeginInit();
		this.includeLabelLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rulesLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reimportCheckEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reimportEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.optionsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.clearFilterLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fullReimportHelpIconLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl2.AllowCustomization = false;
		this.layoutControl2.Controls.Add(this.fullReimportHelpIconUserControl);
		this.layoutControl2.Controls.Add(this.importModeLabelControl);
		this.layoutControl2.Controls.Add(this.fullReimportCheckEdit);
		this.layoutControl2.Controls.Add(this.rulesFlowLayoutPanel);
		this.layoutControl2.Controls.Add(this.clearLabelControl);
		this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl2.Location = new System.Drawing.Point(0, 0);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1670, 197, 580, 524);
		this.layoutControl2.Root = this.Root;
		this.layoutControl2.Size = new System.Drawing.Size(557, 461);
		this.layoutControl2.TabIndex = 5;
		this.layoutControl2.Text = "layoutControl2";
		this.fullReimportHelpIconUserControl.AutoPopDelay = 5000;
		this.fullReimportHelpIconUserControl.BackColor = System.Drawing.Color.Transparent;
		this.fullReimportHelpIconUserControl.KeepWhileHovered = false;
		this.fullReimportHelpIconUserControl.Location = new System.Drawing.Point(162, 439);
		this.fullReimportHelpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.fullReimportHelpIconUserControl.MaxToolTipWidth = 500;
		this.fullReimportHelpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.fullReimportHelpIconUserControl.Name = "fullReimportHelpIconUserControl";
		this.fullReimportHelpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.fullReimportHelpIconUserControl.TabIndex = 43;
		this.fullReimportHelpIconUserControl.ToolTipHeader = null;
		this.fullReimportHelpIconUserControl.ToolTipText = null;
		this.importModeLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.importModeLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.importModeLabelControl.Appearance.Options.UseFont = true;
		this.importModeLabelControl.Appearance.Options.UseForeColor = true;
		this.importModeLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.importModeLabelControl.Location = new System.Drawing.Point(2, 416);
		this.importModeLabelControl.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.importModeLabelControl.Name = "importModeLabelControl";
		this.importModeLabelControl.Size = new System.Drawing.Size(499, 19);
		this.importModeLabelControl.StyleController = this.layoutControl2;
		this.importModeLabelControl.TabIndex = 41;
		this.importModeLabelControl.Text = "Options:";
		this.fullReimportCheckEdit.Location = new System.Drawing.Point(2, 439);
		this.fullReimportCheckEdit.Name = "fullReimportCheckEdit";
		this.fullReimportCheckEdit.Properties.Caption = "Reimport all included objects";
		this.fullReimportCheckEdit.Size = new System.Drawing.Size(158, 20);
		this.fullReimportCheckEdit.StyleController = this.layoutControl2;
		this.fullReimportCheckEdit.TabIndex = 40;
		this.rulesFlowLayoutPanel.AutoScroll = true;
		this.rulesFlowLayoutPanel.Controls.Add(this.includeLabelLayoutControl);
		this.rulesFlowLayoutPanel.Controls.Add(this.includeFilterStatusLabelControl);
		this.rulesFlowLayoutPanel.Controls.Add(this.layoutControl1);
		this.rulesFlowLayoutPanel.Controls.Add(this.excludeLabelControl);
		this.rulesFlowLayoutPanel.Controls.Add(this.excludeAddRuleLabelControl);
		this.rulesFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
		this.rulesFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.rulesFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.rulesFlowLayoutPanel.Name = "rulesFlowLayoutPanel";
		this.rulesFlowLayoutPanel.Size = new System.Drawing.Size(557, 414);
		this.rulesFlowLayoutPanel.TabIndex = 2;
		this.rulesFlowLayoutPanel.WrapContents = false;
		this.includeLabelLayoutControl.AllowCustomization = false;
		this.includeLabelLayoutControl.Controls.Add(this.includeLabelControl);
		this.includeLabelLayoutControl.Controls.Add(this.rulesHelpIconUserControl);
		this.includeLabelLayoutControl.Location = new System.Drawing.Point(0, 3);
		this.includeLabelLayoutControl.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.includeLabelLayoutControl.Name = "includeLabelLayoutControl";
		this.includeLabelLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1661, 388, 626, 606);
		this.includeLabelLayoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.includeLabelLayoutControl.Root = this.layoutControlGroup2;
		this.includeLabelLayoutControl.Size = new System.Drawing.Size(180, 20);
		this.includeLabelLayoutControl.TabIndex = 41;
		this.includeLabelLayoutControl.Text = "layoutControl3";
		this.includeLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.includeLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.includeLabelControl.Appearance.Options.UseFont = true;
		this.includeLabelControl.Appearance.Options.UseForeColor = true;
		this.includeLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.includeLabelControl.Location = new System.Drawing.Point(0, 0);
		this.includeLabelControl.Name = "includeLabelControl";
		this.includeLabelControl.Size = new System.Drawing.Size(98, 20);
		this.includeLabelControl.StyleController = this.includeLabelLayoutControl;
		this.includeLabelControl.TabIndex = 2;
		this.includeLabelControl.Text = "Include objects:";
		this.rulesHelpIconUserControl.AutoPopDelay = 5000;
		this.rulesHelpIconUserControl.BackColor = System.Drawing.Color.Transparent;
		this.rulesHelpIconUserControl.KeepWhileHovered = false;
		this.rulesHelpIconUserControl.Location = new System.Drawing.Point(98, 0);
		this.rulesHelpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.rulesHelpIconUserControl.MaxToolTipWidth = 500;
		this.rulesHelpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.rulesHelpIconUserControl.Name = "rulesHelpIconUserControl";
		this.rulesHelpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.rulesHelpIconUserControl.TabIndex = 44;
		this.rulesHelpIconUserControl.ToolTipHeader = null;
		this.rulesHelpIconUserControl.ToolTipText = resources.GetString("rulesHelpIconUserControl.ToolTipText");
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem8, this.rulesLayoutControlItem, this.emptySpaceItem1 });
		this.layoutControlGroup2.Name = "Root";
		this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup2.Size = new System.Drawing.Size(180, 20);
		this.layoutControlGroup2.TextVisible = false;
		this.layoutControlItem8.Control = this.includeLabelControl;
		this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem8.MaxSize = new System.Drawing.Size(98, 20);
		this.layoutControlItem8.MinSize = new System.Drawing.Size(98, 20);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem8.Size = new System.Drawing.Size(98, 20);
		this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.rulesLayoutControlItem.Control = this.rulesHelpIconUserControl;
		this.rulesLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.rulesLayoutControlItem.CustomizationFormText = "rulesLayoutControlItem";
		this.rulesLayoutControlItem.Location = new System.Drawing.Point(98, 0);
		this.rulesLayoutControlItem.MaxSize = new System.Drawing.Size(20, 20);
		this.rulesLayoutControlItem.MinSize = new System.Drawing.Size(20, 20);
		this.rulesLayoutControlItem.Name = "rulesLayoutControlItem";
		this.rulesLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.rulesLayoutControlItem.Size = new System.Drawing.Size(20, 20);
		this.rulesLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rulesLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rulesLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(118, 0);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(62, 20);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.includeFilterStatusLabelControl.Location = new System.Drawing.Point(3, 29);
		this.includeFilterStatusLabelControl.Name = "includeFilterStatusLabelControl";
		this.includeFilterStatusLabelControl.Size = new System.Drawing.Size(11, 13);
		this.includeFilterStatusLabelControl.TabIndex = 38;
		this.includeFilterStatusLabelControl.Text = "All";
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.Controls.Add(this.labelControl1);
		this.layoutControl1.Controls.Add(this.includeAddRuleLabelControl);
		this.layoutControl1.Location = new System.Drawing.Point(3, 48);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(826, 164, 250, 350);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(536, 19);
		this.layoutControl1.TabIndex = 40;
		this.layoutControl1.Text = "layoutControl1";
		this.labelControl1.AllowHtmlString = true;
		this.labelControl1.Cursor = System.Windows.Forms.Cursors.Hand;
		this.labelControl1.Location = new System.Drawing.Point(106, 2);
		this.labelControl1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(98, 15);
		this.labelControl1.StyleController = this.layoutControl1;
		this.labelControl1.TabIndex = 6;
		this.labelControl1.Text = "<href>Add exclude pattern</href>";
		this.labelControl1.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(labelControl1_HyperlinkClick);
		this.includeAddRuleLabelControl.AllowHtmlString = true;
		this.includeAddRuleLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.includeAddRuleLabelControl.Location = new System.Drawing.Point(0, 0);
		this.includeAddRuleLabelControl.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.includeAddRuleLabelControl.Name = "includeAddRuleLabelControl";
		this.includeAddRuleLabelControl.Size = new System.Drawing.Size(94, 19);
		this.includeAddRuleLabelControl.StyleController = this.layoutControl1;
		this.includeAddRuleLabelControl.TabIndex = 3;
		this.includeAddRuleLabelControl.Text = "<href>Add include pattern</href>";
		this.includeAddRuleLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(includeAddRuleLabelControl_HyperlinkClick);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem5, this.layoutControlItem7, this.emptySpaceItem3, this.emptySpaceItem4 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(536, 19);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem5.Control = this.includeAddRuleLabelControl;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(94, 19);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(94, 19);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem5.Size = new System.Drawing.Size(94, 19);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem7.Control = this.labelControl1;
		this.layoutControlItem7.Location = new System.Drawing.Point(104, 0);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(102, 19);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(102, 19);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(102, 19);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(206, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem3.Size = new System.Drawing.Size(330, 19);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(94, 0);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(10, 19);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 19);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(10, 19);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.excludeLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.excludeLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.excludeLabelControl.Appearance.Options.UseFont = true;
		this.excludeLabelControl.Appearance.Options.UseForeColor = true;
		this.excludeLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.excludeLabelControl.Location = new System.Drawing.Point(0, 73);
		this.excludeLabelControl.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.excludeLabelControl.Name = "excludeLabelControl";
		this.excludeLabelControl.Size = new System.Drawing.Size(523, 13);
		this.excludeLabelControl.TabIndex = 4;
		this.excludeLabelControl.Text = "Exclude objects:";
		this.excludeAddRuleLabelControl.AllowHtmlString = true;
		this.excludeAddRuleLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.excludeAddRuleLabelControl.Location = new System.Drawing.Point(0, 92);
		this.excludeAddRuleLabelControl.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.excludeAddRuleLabelControl.Name = "excludeAddRuleLabelControl";
		this.excludeAddRuleLabelControl.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
		this.excludeAddRuleLabelControl.Size = new System.Drawing.Size(101, 13);
		this.excludeAddRuleLabelControl.TabIndex = 5;
		this.excludeAddRuleLabelControl.Text = "<href>Add exclude pattern</href>";
		this.excludeAddRuleLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(excludeAddRuleLabelControl_HyperlinkClick);
		this.clearLabelControl.AllowHtmlString = true;
		this.clearLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.clearLabelControl.Location = new System.Drawing.Point(505, 416);
		this.clearLabelControl.Name = "clearLabelControl";
		this.clearLabelControl.Size = new System.Drawing.Size(50, 19);
		this.clearLabelControl.StyleController = this.layoutControl2;
		this.clearLabelControl.TabIndex = 4;
		this.clearLabelControl.Text = "<href>Clear filter</href>";
		this.clearLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(clearLabelControl_HyperlinkClick);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.filterLayoutControlItem, this.reimportCheckEditLayoutControlItem, this.reimportEmptySpaceItem, this.optionsLayoutControlItem, this.clearFilterLayoutControlItem, this.fullReimportHelpIconLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(557, 461);
		this.Root.TextVisible = false;
		this.filterLayoutControlItem.Control = this.rulesFlowLayoutPanel;
		this.filterLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.filterLayoutControlItem.Name = "filterLayoutControlItem";
		this.filterLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.filterLayoutControlItem.Size = new System.Drawing.Size(557, 414);
		this.filterLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.filterLayoutControlItem.TextVisible = false;
		this.reimportCheckEditLayoutControlItem.Control = this.fullReimportCheckEdit;
		this.reimportCheckEditLayoutControlItem.Location = new System.Drawing.Point(0, 437);
		this.reimportCheckEditLayoutControlItem.MaxSize = new System.Drawing.Size(160, 24);
		this.reimportCheckEditLayoutControlItem.MinSize = new System.Drawing.Size(160, 24);
		this.reimportCheckEditLayoutControlItem.Name = "reimportCheckEditLayoutControlItem";
		this.reimportCheckEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
		this.reimportCheckEditLayoutControlItem.Size = new System.Drawing.Size(160, 24);
		this.reimportCheckEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.reimportCheckEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.reimportCheckEditLayoutControlItem.TextVisible = false;
		this.reimportEmptySpaceItem.AllowHotTrack = false;
		this.reimportEmptySpaceItem.Location = new System.Drawing.Point(184, 437);
		this.reimportEmptySpaceItem.Name = "reimportEmptySpaceItem";
		this.reimportEmptySpaceItem.Size = new System.Drawing.Size(373, 24);
		this.reimportEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.optionsLayoutControlItem.Control = this.importModeLabelControl;
		this.optionsLayoutControlItem.Location = new System.Drawing.Point(0, 414);
		this.optionsLayoutControlItem.MaxSize = new System.Drawing.Size(503, 23);
		this.optionsLayoutControlItem.MinSize = new System.Drawing.Size(503, 23);
		this.optionsLayoutControlItem.Name = "optionsLayoutControlItem";
		this.optionsLayoutControlItem.Size = new System.Drawing.Size(503, 23);
		this.optionsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.optionsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.optionsLayoutControlItem.TextVisible = false;
		this.clearFilterLayoutControlItem.Control = this.clearLabelControl;
		this.clearFilterLayoutControlItem.Location = new System.Drawing.Point(503, 414);
		this.clearFilterLayoutControlItem.MaxSize = new System.Drawing.Size(54, 23);
		this.clearFilterLayoutControlItem.MinSize = new System.Drawing.Size(54, 23);
		this.clearFilterLayoutControlItem.Name = "clearFilterLayoutControlItem";
		this.clearFilterLayoutControlItem.Size = new System.Drawing.Size(54, 23);
		this.clearFilterLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.clearFilterLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.clearFilterLayoutControlItem.TextVisible = false;
		this.fullReimportHelpIconLayoutControlItem.Control = this.fullReimportHelpIconUserControl;
		this.fullReimportHelpIconLayoutControlItem.Location = new System.Drawing.Point(160, 437);
		this.fullReimportHelpIconLayoutControlItem.Name = "fullReimportHelpIconLayoutControlItem";
		this.fullReimportHelpIconLayoutControlItem.Size = new System.Drawing.Size(24, 24);
		this.fullReimportHelpIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.fullReimportHelpIconLayoutControlItem.TextVisible = false;
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.AutoPopDelay = int.MaxValue;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		base.Controls.Add(this.layoutControl2);
		base.Name = "ImportFilterControl";
		base.Size = new System.Drawing.Size(557, 461);
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		this.layoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.fullReimportCheckEdit.Properties).EndInit();
		this.rulesFlowLayoutPanel.ResumeLayout(false);
		this.rulesFlowLayoutPanel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.includeLabelLayoutControl).EndInit();
		this.includeLabelLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rulesLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reimportCheckEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reimportEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.optionsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.clearFilterLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fullReimportHelpIconLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
