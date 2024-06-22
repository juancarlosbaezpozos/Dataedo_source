using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.FormsSupport;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Results.Base;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.CustomFields;
using DevExpress.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class CustomFieldsPanelControl : BaseUserControl
{
	private bool forceRefreshCustomFieldsControlsOnce;

	private const int LeftPadding = 2;

	private const int RightPadding = 30;

	private const int BottomPadding = 0;

	private LayoutControlItem panelControlItem;

	private LayoutControlItem[] layoutControlItems;

	private Size defaultItemSize = new Size(532, 26);

	private readonly int maxControlHeight;

	private readonly int additionalHeight = 4;

	private IContainer components;

	private LayoutControl scrollLayoutControl;

	private LayoutControlGroup scrollLayoutControlGroup;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup Root;

	private LayoutControlGroup layoutControlGroup;

	private LayoutControlItem contentLayoutControlItem;

	public List<CustomFieldControl> FieldControls { get; private set; }

	public event EventHandler EditValueChanging;

	public event EventHandler ShowHistoryClick;

	public CustomFieldsPanelControl()
	{
		InitializeComponent();
		FieldControls = new List<CustomFieldControl>();
		layoutControlItems = new LayoutControlItem[0];
		layoutControl.HorizontalScroll.Enabled = false;
		layoutControl.Padding = new System.Windows.Forms.Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
		maxControlHeight = additionalHeight + defaultItemSize.Height * 10;
	}

	public void ForceRefreshCustomFieldsControlsOnNextLoading()
	{
		forceRefreshCustomFieldsControlsOnce = true;
	}

	public void LoadFields(IEnumerable<CustomFieldDefinition> customFieldRows, EventHandler fieldValueChangedHandler, LayoutControlItem panelControlItem = null)
	{
		this.panelControlItem = panelControlItem;
		if (this.panelControlItem != null)
		{
			this.panelControlItem.SizeConstraintsType = SizeConstraintsType.Custom;
		}
		if (!forceRefreshCustomFieldsControlsOnce && customFieldRows.Count() == FieldControls.Count && customFieldRows.All((CustomFieldDefinition x) => FieldControls.Any((CustomFieldControl y) => y.FieldName == x.CustomField.FieldName && y.CustomField.OrdinalPosition == x.OrdinalPosition)))
		{
			foreach (CustomFieldControl customFieldControl in FieldControls)
			{
				customFieldControl.UpdateCustomField(customFieldRows.Single((CustomFieldDefinition x) => x.CustomField.FieldName == customFieldControl.FieldName));
				customFieldControl.EditValueChanging += this.EditValueChanging;
				customFieldControl.EditValueChanged += fieldValueChangedHandler;
				CustomFieldControl customFieldControl2 = customFieldControl;
				customFieldControl2.ShowHistoryClick = (EventHandler)Delegate.Remove(customFieldControl2.ShowHistoryClick, this.ShowHistoryClick);
				CustomFieldControl customFieldControl3 = customFieldControl;
				customFieldControl3.ShowHistoryClick = (EventHandler)Delegate.Combine(customFieldControl3.ShowHistoryClick, this.ShowHistoryClick);
				customFieldControl.TabStop = true;
			}
			return;
		}
		forceRefreshCustomFieldsControlsOnce = false;
		base.Visible = false;
		DrawingHelpers.SuspendDrawing(this);
		layoutControl.SuspendLayout();
		Root.BeginUpdate();
		layoutControlGroup.BeginUpdate();
		foreach (CustomFieldControl fieldControl in FieldControls)
		{
			fieldControl.Dispose();
		}
		LayoutControlItem[] array = layoutControlItems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Dispose();
		}
		FieldControls = new List<CustomFieldControl>(customFieldRows.OrderBy((CustomFieldDefinition x) => x.OrdinalPosition).Select(delegate(CustomFieldDefinition x)
		{
			_ = x.CustomField.FieldName;
			CustomFieldControl customFieldControl4 = new CustomFieldControl(x);
			customFieldControl4.EditValueChanging += this.EditValueChanging;
			customFieldControl4.EditValueChanged += fieldValueChangedHandler;
			customFieldControl4.ShowHistoryClick = (EventHandler)Delegate.Remove(customFieldControl4.ShowHistoryClick, this.ShowHistoryClick);
			customFieldControl4.ShowHistoryClick = (EventHandler)Delegate.Combine(customFieldControl4.ShowHistoryClick, this.ShowHistoryClick);
			customFieldControl4.TabStop = true;
			return customFieldControl4;
		}));
		layoutControlItems = FieldControls.Select(delegate(CustomFieldControl x)
		{
			LayoutControlItem obj2 = new LayoutControlItem
			{
				Control = x,
				SizeConstraintsType = SizeConstraintsType.Custom
			};
			Size size2 = (obj2.Size = defaultItemSize);
			Size size5 = (obj2.MaxSize = (obj2.MinSize = size2));
			obj2.Text = "layoutControlItem";
			obj2.TextSize = new Size(0, 0);
			obj2.TextVisible = false;
			obj2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 0, 30);
			return obj2;
		}).ToArray();
		layoutControlGroup.Clear();
		LayoutControlGroup obj = layoutControlGroup;
		BaseLayoutItem[] items = layoutControlItems;
		obj.AddRange(items);
		layoutControlGroup.EndUpdate();
		Root.EndUpdate();
		layoutControl.ResumeLayout();
		if (customFieldRows.Count() == 0)
		{
			panelControlItem.Visibility = LayoutVisibility.Never;
			return;
		}
		panelControlItem.Visibility = LayoutVisibility.Always;
		DrawingHelpers.ResumeDrawing(this);
		SetPanelControlHeight();
		base.Visible = true;
	}

	public void CloseEditors()
	{
		foreach (CustomFieldControl fieldControl in FieldControls)
		{
			fieldControl?.CloseEditor();
		}
	}

	public void SetCustomFieldsValuesInRow(BasicRow row)
	{
		foreach (CustomFieldControl fieldControl in FieldControls)
		{
			row.CustomFields.SetCustomFieldValue(fieldControl.FieldName, fieldControl.Value);
		}
	}

	public void SetCustomFieldsValues(Dataedo.Data.Base.Commands.Results.Base.CustomFieldsData data)
	{
		foreach (CustomFieldControl fieldControl in FieldControls)
		{
			data.SetField(fieldControl.FieldName, fieldControl.Value, isActive: true);
		}
	}

	public void SetCustomFieldsValues(Dataedo.Model.Data.Common.CustomFields.CustomFieldsData data)
	{
		foreach (CustomFieldControl fieldControl in FieldControls)
		{
			data.SetField(fieldControl.FieldName, fieldControl.Value, isActive: true);
		}
	}

	public void SetCustomFieldsValues(CustomFieldContainer data)
	{
		foreach (CustomFieldControl fieldControl in FieldControls)
		{
			data.SetCustomFieldValue(fieldControl.FieldName, fieldControl.Value);
		}
	}

	public void UpdateDefinitionValues()
	{
		foreach (CustomFieldControl item in FieldControls.Where((CustomFieldControl x) => x.CustomField.IsOpenDefinitionType))
		{
			item.UpdateDefinitionValues();
		}
	}

	public void FocusPrevious(CustomFieldControl customFieldControl)
	{
		int num = FieldControls.IndexOf(customFieldControl);
		if (num >= 1)
		{
			FieldControls[num - 1].Focus();
			return;
		}
		Control[] array = (from Control x in customFieldControl.Parent.Parent.Parent.Parent.Controls
			where x.TabStop || x.Name.Contains("Title")
			orderby x.TabIndex
			select x).ToArray();
		for (int num2 = Array.IndexOf(array, customFieldControl.Parent.Parent.Parent) - 1; num2 >= 0; num2--)
		{
			if (array[num2].CanFocus)
			{
				array[num2].Focus();
				break;
			}
		}
	}

	public void FocusNext(CustomFieldControl customFieldControl)
	{
		int num = FieldControls.IndexOf(customFieldControl);
		if (num >= 0 && FieldControls.Count > num + 1)
		{
			FieldControls[num + 1].Focus();
			return;
		}
		Control[] array = (from Control x in customFieldControl.Parent.Parent.Parent.Parent.Controls
			where x.TabStop
			orderby x.TabIndex
			select x).ToArray();
		for (int i = Array.IndexOf(array, customFieldControl.Parent.Parent.Parent) + 1; i < array.Length; i++)
		{
			if (array[i].CanFocus)
			{
				array[i].Focus();
				break;
			}
		}
	}

	public void SetPanelControlHeight()
	{
		if (panelControlItem == null)
		{
			return;
		}
		layoutControlGroup.Invalidate();
		int num = base.Width / defaultItemSize.Width;
		num = ((num == 0) ? 1 : num);
		int num2 = layoutControlItems.Length / num;
		int num3 = layoutControlItems.Length % num;
		int num4 = additionalHeight + (num2 + ((num3 != 0) ? 1 : 0)) * defaultItemSize.Height;
		if (num4 > maxControlHeight)
		{
			num4 = maxControlHeight;
		}
		else if (num4 == maxControlHeight)
		{
			num4 += layoutControlGroup.Padding.Bottom;
		}
		if (panelControlItem == null || panelControlItem.Height != num4)
		{
			base.Size = new Size(base.Width, num4);
			if (panelControlItem != null)
			{
				LayoutControlItem layoutControlItem = panelControlItem;
				LayoutControlItem layoutControlItem2 = panelControlItem;
				Size size2 = (panelControlItem.Size = new Size(0, num4));
				Size size5 = (layoutControlItem.MaxSize = (layoutControlItem2.MinSize = size2));
			}
			layoutControl.Height = num4;
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
		this.scrollLayoutControl = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.scrollLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.contentLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.scrollLayoutControl).BeginInit();
		this.scrollLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.scrollLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.contentLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.scrollLayoutControl.Controls.Add(this.layoutControl);
		this.scrollLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.scrollLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.scrollLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.scrollLayoutControl.Name = "scrollLayoutControl";
		this.scrollLayoutControl.Root = this.scrollLayoutControlGroup;
		this.scrollLayoutControl.Size = new System.Drawing.Size(550, 30);
		this.scrollLayoutControl.TabIndex = 1;
		this.scrollLayoutControl.Text = "layoutControl";
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.AutoScroll = false;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(513, 303, 650, 400);
		this.layoutControl.Root = this.Root;
		this.layoutControl.Size = new System.Drawing.Size(550, 30);
		this.layoutControl.TabIndex = 4;
		this.layoutControl.Text = "nonCustomizableLayoutControl1";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlGroup });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(550, 30);
		this.Root.TextVisible = false;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Flow;
		this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup.Name = "layoutControlGroup";
		this.layoutControlGroup.Size = new System.Drawing.Size(550, 30);
		this.layoutControlGroup.TextVisible = false;
		this.scrollLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.scrollLayoutControlGroup.GroupBordersVisible = false;
		this.scrollLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.contentLayoutControlItem });
		this.scrollLayoutControlGroup.Name = "scrollLayoutControlGroup";
		this.scrollLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.scrollLayoutControlGroup.Size = new System.Drawing.Size(550, 30);
		this.scrollLayoutControlGroup.TextVisible = false;
		this.contentLayoutControlItem.Control = this.layoutControl;
		this.contentLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.contentLayoutControlItem.Name = "contentLayoutControlItem";
		this.contentLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.contentLayoutControlItem.Size = new System.Drawing.Size(550, 30);
		this.contentLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.contentLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.scrollLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "CustomFieldsPanelControl";
		base.Size = new System.Drawing.Size(550, 30);
		((System.ComponentModel.ISupportInitialize)this.scrollLayoutControl).EndInit();
		this.scrollLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.scrollLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.contentLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
