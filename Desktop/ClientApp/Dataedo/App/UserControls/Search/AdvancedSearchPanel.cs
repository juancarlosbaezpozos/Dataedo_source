using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Search;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.Search;

public class AdvancedSearchPanel : BaseUserControl
{
	public static int BaseHeight = 28;

	private List<CustomFieldSearchControl> customFields;

	private List<CustomFieldRowExtended> customFieldsSupportFields;

	private IContainer components;

	private FlowLayoutPanel customFieldsFlowLayoutPanel;

	private LabelControl addCustomFieldLabelControl;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem addCustomFieldLayoutControlItem;

	private PopupMenu addFieldPopupMenu;

	private BarManager addFieldBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private EmptySpaceItem emptySpaceItem1;

	private SimpleButton searchSimpleButton;

	private LayoutControlItem searchButtonLayoutControlItem;

	private EmptySpaceItem emptySpaceItem2;

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public event PreviewKeyDownEventHandler FieldPreviewKeyDown;

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public event KeyPressEventHandler FieldKeyPress;

	public event Action CustomFieldAdded;

	public event Action<CustomFieldSearchControl> CustomFieldRemoved;

	public event Action Search;

	public AdvancedSearchPanel()
	{
		InitializeComponent();
		customFields = new List<CustomFieldSearchControl>();
		CustomFieldRemoved += AdvancedSearchPanel_CustomFieldRemoved;
	}

	public void Initialize(CustomFieldsSupport customFieldsSupport)
	{
		customFieldsSupportFields = new List<CustomFieldRowExtended>(customFieldsSupport.Fields);
	}

	public List<CustomFieldSearchItem> GetCustomFieldsSearchItems()
	{
		return customFields.Select((CustomFieldSearchControl x) => x.GetSearchItem()).ToList();
	}

	public int GetActualHeight()
	{
		return BaseHeight + customFields.Count * CustomFieldSearchControl.BaseHeight;
	}

	private void AdvancedSearchPanel_Load(object sender, EventArgs e)
	{
		RefreshCustomFieldsList();
	}

	public void RefreshDataSource()
	{
		RefreshCustomFieldsList();
		RepairCustomFieldsControls();
	}

	private void RefreshCustomFieldsList()
	{
		try
		{
			addFieldPopupMenu.ClearLinks();
			foreach (CustomFieldRowExtended item in customFieldsSupportFields.Where((CustomFieldRowExtended x) => !customFields.Any((CustomFieldSearchControl y) => y.CustomField.Code == x.Code)))
			{
				addFieldPopupMenu.AddItem(new BarButtonItem(addFieldBarManager, item.Title)
				{
					Hint = item.Title,
					Tag = item
				});
			}
			addCustomFieldLayoutControlItem.ContentVisible = addFieldPopupMenu.ItemLinks.Count > 0;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to refresh search custom fields list.", FindForm());
		}
	}

	private void RepairCustomFieldsControls()
	{
		try
		{
			List<string> list = customFieldsSupportFields?.Select((CustomFieldRowExtended x) => x.Code).ToList();
			if (list == null)
			{
				return;
			}
			for (int num = customFields.Count - 1; num >= 0; num--)
			{
				CustomFieldSearchControl control = customFields[num];
				if (!list.Contains(control.CustomField.Code))
				{
					this.CustomFieldRemoved?.Invoke(control);
				}
				else
				{
					control.CustomField = customFieldsSupportFields.FirstOrDefault((CustomFieldRowExtended x) => x.Code == control.CustomField.Code);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to refresh search custom fields list.", FindForm());
		}
	}

	private void addCustomFieldLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		Point p = PointToScreen(new Point(addCustomFieldLabelControl.Left, addCustomFieldLabelControl.Bottom - addCustomFieldLayoutControlItem.Padding.Bottom));
		addFieldPopupMenu.ShowPopup(addFieldBarManager, p);
	}

	private void AdvancedSearchPanel_CustomFieldRemoved(CustomFieldSearchControl control)
	{
		if (control?.CustomField != null)
		{
			customFields.Remove(control);
			customFieldsFlowLayoutPanel.Controls.Remove(control);
			for (int i = 0; i < customFieldsFlowLayoutPanel.Controls.Count; i++)
			{
				customFieldsFlowLayoutPanel.Controls[i].TabIndex = i;
			}
			RefreshCustomFieldsList();
		}
	}

	private void addFieldBarManager_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.Item.Tag is CustomFieldRowExtended customField)
		{
			CustomFieldSearchControl customFieldSearchControl = new CustomFieldSearchControl(customField);
			customFieldSearchControl.Width = customFieldsFlowLayoutPanel.Width;
			customFieldSearchControl.CustomFieldRemoved += this.CustomFieldRemoved;
			customFieldSearchControl.FieldKeyPress += this.FieldKeyPress;
			customFieldSearchControl.FieldPreviewKeyDown += this.FieldPreviewKeyDown;
			customFields.Add(customFieldSearchControl);
			customFieldsFlowLayoutPanel.Controls.Add(customFieldSearchControl);
			customFieldsFlowLayoutPanel.Controls.SetChildIndex(customFieldSearchControl, customFieldsFlowLayoutPanel.Controls.Count);
			for (int i = 0; i < customFieldsFlowLayoutPanel.Controls.Count; i++)
			{
				customFieldsFlowLayoutPanel.Controls[i].TabIndex = i;
			}
			this.CustomFieldAdded?.Invoke();
			RefreshCustomFieldsList();
		}
	}

	private void searchSimpleButton_Click(object sender, EventArgs e)
	{
		this.Search?.Invoke();
	}

	public void ClearSearchFields()
	{
		customFields.Clear();
		customFieldsFlowLayoutPanel.Controls.Clear();
		RefreshCustomFieldsList();
		this.CustomFieldRemoved?.Invoke(null);
	}

	private void AdvancedSearchPanel_SizeChanged(object sender, EventArgs e)
	{
		foreach (Control control in customFieldsFlowLayoutPanel.Controls)
		{
			control.Width = customFieldsFlowLayoutPanel.Width;
		}
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
		this.customFieldsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.addCustomFieldLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.searchSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.addCustomFieldLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.searchButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.addFieldPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addFieldBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.addCustomFieldLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.addFieldPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.addFieldBarManager).BeginInit();
		base.SuspendLayout();
		this.customFieldsFlowLayoutPanel.AutoScroll = true;
		this.customFieldsFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
		this.customFieldsFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.customFieldsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.customFieldsFlowLayoutPanel.Name = "customFieldsFlowLayoutPanel";
		this.customFieldsFlowLayoutPanel.Size = new System.Drawing.Size(273, 2);
		this.customFieldsFlowLayoutPanel.TabIndex = 0;
		this.customFieldsFlowLayoutPanel.WrapContents = false;
		this.addCustomFieldLabelControl.AllowHtmlString = true;
		this.addCustomFieldLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.addCustomFieldLabelControl.Location = new System.Drawing.Point(125, 4);
		this.addCustomFieldLabelControl.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
		this.addCustomFieldLabelControl.Name = "addCustomFieldLabelControl";
		this.addCustomFieldLabelControl.Size = new System.Drawing.Size(42, 22);
		this.addCustomFieldLabelControl.StyleController = this.mainLayoutControl;
		this.addCustomFieldLabelControl.TabIndex = 0;
		this.addCustomFieldLabelControl.Text = "<href>Add field</href>\r\n";
		this.addCustomFieldLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(addCustomFieldLabelControl_HyperlinkClick);
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.searchSimpleButton);
		this.mainLayoutControl.Controls.Add(this.addCustomFieldLabelControl);
		this.mainLayoutControl.Controls.Add(this.customFieldsFlowLayoutPanel);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2652, 136, 250, 350);
		this.mainLayoutControl.Root = this.layoutControlGroup1;
		this.mainLayoutControl.Size = new System.Drawing.Size(273, 28);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "layoutControl1";
		this.searchSimpleButton.Location = new System.Drawing.Point(191, 4);
		this.searchSimpleButton.Name = "searchSimpleButton";
		this.searchSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.searchSimpleButton.StyleController = this.mainLayoutControl;
		this.searchSimpleButton.TabIndex = 1;
		this.searchSimpleButton.Text = "Search";
		this.searchSimpleButton.Click += new System.EventHandler(searchSimpleButton_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.addCustomFieldLayoutControlItem, this.emptySpaceItem1, this.layoutControlItem2, this.searchButtonLayoutControlItem, this.emptySpaceItem2 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(273, 28);
		this.layoutControlGroup1.TextVisible = false;
		this.addCustomFieldLayoutControlItem.Control = this.addCustomFieldLabelControl;
		this.addCustomFieldLayoutControlItem.Location = new System.Drawing.Point(123, 2);
		this.addCustomFieldLayoutControlItem.MaxSize = new System.Drawing.Size(46, 26);
		this.addCustomFieldLayoutControlItem.MinSize = new System.Drawing.Size(46, 26);
		this.addCustomFieldLayoutControlItem.Name = "addCustomFieldLayoutControlItem";
		this.addCustomFieldLayoutControlItem.Size = new System.Drawing.Size(46, 26);
		this.addCustomFieldLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.addCustomFieldLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.addCustomFieldLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 2);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(123, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.customFieldsFlowLayoutPanel;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(100, 1);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem2.Size = new System.Drawing.Size(273, 2);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Bottom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.searchButtonLayoutControlItem.Control = this.searchSimpleButton;
		this.searchButtonLayoutControlItem.Location = new System.Drawing.Point(189, 2);
		this.searchButtonLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.searchButtonLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.searchButtonLayoutControlItem.Name = "searchButtonLayoutControlItem";
		this.searchButtonLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.searchButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.searchButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.searchButtonLayoutControlItem.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(169, 2);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(20, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(20, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(20, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.addFieldPopupMenu.DrawMenuSideStrip = DevExpress.Utils.DefaultBoolean.False;
		this.addFieldPopupMenu.Manager = this.addFieldBarManager;
		this.addFieldPopupMenu.MultiColumn = DevExpress.Utils.DefaultBoolean.False;
		this.addFieldPopupMenu.Name = "addFieldPopupMenu";
		this.addFieldBarManager.DockControls.Add(this.barDockControlTop);
		this.addFieldBarManager.DockControls.Add(this.barDockControlBottom);
		this.addFieldBarManager.DockControls.Add(this.barDockControlLeft);
		this.addFieldBarManager.DockControls.Add(this.barDockControlRight);
		this.addFieldBarManager.Form = this;
		this.addFieldBarManager.MaxItemId = 1;
		this.addFieldBarManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addFieldBarManager_ItemClick);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.addFieldBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(273, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 28);
		this.barDockControlBottom.Manager = this.addFieldBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(273, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.addFieldBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 28);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(273, 0);
		this.barDockControlRight.Manager = this.addFieldBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 28);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "AdvancedSearchPanel";
		base.Size = new System.Drawing.Size(273, 28);
		base.Load += new System.EventHandler(AdvancedSearchPanel_Load);
		base.SizeChanged += new System.EventHandler(AdvancedSearchPanel_SizeChanged);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.addCustomFieldLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.addFieldPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.addFieldBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
