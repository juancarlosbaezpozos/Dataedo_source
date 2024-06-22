using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class CustomFieldControl : BaseUserControl
{
	public class CustomFieldValueModel
	{
		private string value;

		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;
					this.EditValueChanged?.Invoke(this, null);
				}
			}
		}

		public event EventHandler EditValueChanged;
	}

	public EventHandler ShowHistoryClick;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private GridControl valueGridControl;

	private GridView valueGridView;

	private LayoutControlItem layoutControlItem;

	private LabelControl fieldNameLabelControl;

	private LayoutControlItem fieldNameLayoutControlItem;

	private EmptySpaceItem emptySpaceItem;

	public CustomFieldRowExtended CustomField { get; private set; }

	public bool IsProgressPainterActive { get; set; }

	public string FieldName { get; set; }

	public CustomFieldValueModel FieldValue { get; set; }

	public string Title
	{
		get
		{
			return fieldNameLabelControl.Text;
		}
		set
		{
			fieldNameLabelControl.Text = value;
		}
	}

	public Color TitleForeColor
	{
		get
		{
			return layoutControlItem.AppearanceItemCaption.ForeColor;
		}
		set
		{
			layoutControlItem.AppearanceItemCaption.ForeColor = value;
		}
	}

	public string Value
	{
		get
		{
			return FieldValue.Value;
		}
		set
		{
			FieldValue.Value = value;
		}
	}

	public Color ValueForeColor
	{
		get
		{
			return valueGridView.Appearance.Row.ForeColor;
		}
		set
		{
			valueGridView.Appearance.Row.ForeColor = value;
		}
	}

	public Color ValueEditBackColor
	{
		get
		{
			return valueGridView.Appearance.Row.BackColor;
		}
		set
		{
			valueGridView.Appearance.Row.BackColor = value;
		}
	}

	[Browsable(true)]
	public event EventHandler EditValueChanged
	{
		add
		{
			FieldValue.EditValueChanged += value;
		}
		remove
		{
			FieldValue.EditValueChanged -= value;
		}
	}

	public event EventHandler EditValueChanging;

	public CustomFieldControl(CustomFieldDefinition customField)
		: this()
	{
		valueGridControl.UseDirectXPaint = DefaultBoolean.False;
		DoubleBuffered = true;
		SetCustomField(customField);
		valueGridView.CellValueChanging += delegate(object s, CellValueChangedEventArgs e)
		{
			if (CustomField.IsOpenDefinitionType || CustomField.Type == CustomFieldTypeEnum.CustomFieldType.Checkbox || CustomField.Type == CustomFieldTypeEnum.CustomFieldType.Integer)
			{
				this.EditValueChanging?.Invoke(s, e);
			}
		};
		valueGridView.CellValueChanged += delegate(object s, CellValueChangedEventArgs e)
		{
			FieldValue.Value = e.Value?.ToString();
			valueGridView.CloseEditor();
		};
		valueGridView.Appearance.Empty.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public CustomFieldControl()
	{
		InitializeComponent();
	}

	public void SetCustomField(CustomFieldDefinition customField)
	{
		CustomField = customField.CustomField;
		FieldName = customField.CustomField.FieldName;
		Title = customField.CustomField.Title;
		base.Name = customField.CustomField.FieldName + "CustomFieldControl";
		GridColumn gridColumn = new GridColumn
		{
			Caption = customField.CustomField.Title,
			FieldName = "Value",
			Visible = true,
			Tag = customField.CustomField
		};
		gridColumn.OptionsColumn.AllowEdit = true;
		gridColumn.OptionsColumn.ReadOnly = false;
		if (customField.CustomField.Type == CustomFieldTypeEnum.CustomFieldType.Date)
		{
			gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
			gridColumn.DisplayFormat.FormatString = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
		}
		else if (customField.CustomField.Type == CustomFieldTypeEnum.CustomFieldType.Checkbox)
		{
			valueGridView.BorderStyle = BorderStyles.NoBorder;
			ValueEditBackColor = SkinColors.ControlColorFromSystemColors;
		}
		else
		{
			valueGridView.BorderStyle = BorderStyles.Default;
		}
		valueGridView.Columns.Clear();
		valueGridView.Columns.Add(gridColumn);
		new CustomFieldsCellsTypesSupport(isForSummaryTable: false, isForGrid: false).SetCustomFieldColumn(valueGridView, customField.CustomField, gridColumn);
		SetValue(customField);
	}

	public void UpdateCustomField(CustomFieldDefinition customField)
	{
		GridColumn gridColumn = valueGridView?.Columns?[0];
		if (gridColumn == null)
		{
			SetCustomField(customField);
			return;
		}
		CustomField = customField.CustomField;
		FieldName = customField.CustomField.FieldName;
		Title = customField.CustomField.Title;
		base.Name = customField.CustomField.FieldName + "CustomFieldControl";
		if (customField.CustomField.Type == CustomFieldTypeEnum.CustomFieldType.Date)
		{
			gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
			gridColumn.DisplayFormat.FormatString = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
		}
		else if (customField.CustomField.Type == CustomFieldTypeEnum.CustomFieldType.Checkbox)
		{
			valueGridView.BorderStyle = BorderStyles.NoBorder;
			ValueEditBackColor = SkinColors.ControlColorFromSystemColors;
		}
		else
		{
			valueGridView.BorderStyle = BorderStyles.Default;
		}
		new CustomFieldsCellsTypesSupport(isForSummaryTable: false, isForGrid: false).SetCustomFieldColumn(valueGridView, customField.CustomField, gridColumn);
		SetValue(customField);
	}

	private void SetValue(CustomFieldDefinition customField)
	{
		if (!customField.CustomField.IsDefinitionType)
		{
			FieldValue = new CustomFieldValueModel
			{
				Value = customField.FieldValue
			};
		}
		else
		{
			FieldValue = new CustomFieldValueModel
			{
				Value = BaseCustomFieldsSupport.PrepareCustomFieldValue(customField.CustomField, customField.FieldValue)
			};
		}
		List<CustomFieldValueModel> dataSource = new List<CustomFieldValueModel> { FieldValue };
		valueGridControl.DataSource = dataSource;
	}

	public void CloseEditor()
	{
		valueGridView.CloseEditor();
	}

	public void UpdateDefinitionValues()
	{
		CustomField.UpdateDefinitionValues(FieldValue.Value);
	}

	private void CustomFieldControl_Enter(object sender, EventArgs e)
	{
		valueGridView.Focus();
		valueGridView.FocusedColumn = valueGridView.Columns[0];
		valueGridView.ShowEditor();
		valueGridView.ActiveEditor?.Focus();
	}

	public void SetProgressBackgroundColor()
	{
		if (IsProgressPainterActive)
		{
			if (!string.IsNullOrEmpty(Value))
			{
				SetBackColor(SkinColors.InputBackColor);
			}
			else
			{
				SetBackColor(ProgressPainter.Color);
			}
		}
	}

	public void ClearSearchHighlight()
	{
		if (!ValueEditBackColor.Equals(ProgressPainter.Color))
		{
			SetBackColor(SkinColors.InputBackColor);
		}
		ValueForeColor = SkinColors.ControlForeColorFromSystemColors;
		SetProgressBackgroundColor();
	}

	private void valueGridControl_Enter(object sender, EventArgs e)
	{
		SetBackColor(SkinColors.InputBackColor);
	}

	private void SetBackColor(Color color)
	{
		if (CustomField.Type != CustomFieldTypeEnum.CustomFieldType.Checkbox)
		{
			ValueEditBackColor = color;
		}
		else if (color == SkinColors.InputBackColor)
		{
			valueGridView.Columns[0].ColumnEdit.Appearance.BackColor = SkinColors.ControlColorFromSystemColors;
		}
	}

	private void valueGridControl_Leave(object sender, EventArgs e)
	{
		if (CustomField.Type == CustomFieldTypeEnum.CustomFieldType.MultiValueListClosed || CustomField.Type == CustomFieldTypeEnum.CustomFieldType.ListClosed)
		{
			Value = valueGridView?.FocusedValue?.ToString();
		}
		else
		{
			Value = valueGridView?.FocusedValue?.ToString();
		}
		CloseEditor();
		SetProgressBackgroundColor();
	}

	private void ValueGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		if (e.Menu == null || ShowHistoryClick == null)
		{
			e?.Menu?.Items?.Remove(e?.Menu?.Items?.Where((DXMenuItem x) => x.Caption == "View History").FirstOrDefault());
		}
		else
		{
			e.Menu.Items.Add(new DXMenuItem("View History", ShowHistoryItem_Click, Resources.search_16));
		}
	}

	private void ValueGridView_ShownEditor(object sender, EventArgs e)
	{
		if (sender is GridView gridView && gridView.ActiveEditor is TextEdit textEdit)
		{
			textEdit.Properties.BeforeShowMenu -= TextEdit_BeforeShowMenu;
			textEdit.Properties.BeforeShowMenu += TextEdit_BeforeShowMenu;
		}
	}

	private void TextEdit_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
	{
		if (e.Menu == null || ShowHistoryClick == null)
		{
			e?.Menu?.Items?.Remove(e?.Menu?.Items?.Where((DXMenuItem x) => x.Caption == "View History").FirstOrDefault());
		}
		else if (!e.Menu.Items.Any((DXMenuItem i) => i.Tag == "View History"))
		{
			DXMenuItem item = new DXMenuItem("View History", ShowHistoryItem_Click, Resources.search_16)
			{
				Tag = "View History"
			};
			e.Menu.Items.Add(item);
		}
	}

	private void ShowHistoryItem_Click(object sender, EventArgs e)
	{
		ShowHistoryClick?.Invoke(null, new TextEventArgs(FieldName));
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
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.fieldNameLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.valueGridControl = new DevExpress.XtraGrid.GridControl();
		this.valueGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.fieldNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.valueGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valueGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.Controls.Add(this.fieldNameLabelControl);
		this.layoutControl1.Controls.Add(this.valueGridControl);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1489, 285, 927, 727);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(500, 22);
		this.layoutControl1.TabIndex = 6;
		this.layoutControl1.Text = "layoutControl1";
		this.fieldNameLabelControl.Appearance.Options.UseTextOptions = true;
		this.fieldNameLabelControl.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
		this.fieldNameLabelControl.AutoEllipsis = true;
		this.fieldNameLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.fieldNameLabelControl.Location = new System.Drawing.Point(0, 0);
		this.fieldNameLabelControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
		this.fieldNameLabelControl.MaximumSize = new System.Drawing.Size(82, 0);
		this.fieldNameLabelControl.MinimumSize = new System.Drawing.Size(82, 0);
		this.fieldNameLabelControl.Name = "fieldNameLabelControl";
		this.fieldNameLabelControl.Size = new System.Drawing.Size(82, 22);
		this.fieldNameLabelControl.StyleController = this.layoutControl1;
		this.fieldNameLabelControl.TabIndex = 6;
		this.fieldNameLabelControl.Text = "Field";
		this.valueGridControl.Location = new System.Drawing.Point(85, 0);
		this.valueGridControl.MainView = this.valueGridView;
		this.valueGridControl.Margin = new System.Windows.Forms.Padding(0);
		this.valueGridControl.Name = "valueGridControl";
		this.valueGridControl.Size = new System.Drawing.Size(411, 22);
		this.valueGridControl.TabIndex = 5;
		this.valueGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.valueGridView });
		this.valueGridControl.Enter += new System.EventHandler(valueGridControl_Enter);
		this.valueGridControl.Leave += new System.EventHandler(valueGridControl_Leave);
		this.valueGridView.GridControl = this.valueGridControl;
		this.valueGridView.Name = "valueGridView";
		this.valueGridView.OptionsFilter.AllowFilterEditor = false;
		this.valueGridView.OptionsMenu.EnableColumnMenu = false;
		this.valueGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.valueGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.valueGridView.OptionsView.ShowColumnHeaders = false;
		this.valueGridView.OptionsView.ShowDetailButtons = false;
		this.valueGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.valueGridView.OptionsView.ShowGroupPanel = false;
		this.valueGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.valueGridView.OptionsView.ShowIndicator = false;
		this.valueGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
		this.valueGridView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
		this.valueGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(ValueGridView_PopupMenuShowing);
		this.valueGridView.ShownEditor += new System.EventHandler(ValueGridView_ShownEditor);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem, this.fieldNameLayoutControlItem, this.emptySpaceItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(500, 22);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem.AppearanceItemCaption.Options.UseTextOptions = true;
		this.layoutControlItem.AppearanceItemCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
		this.layoutControlItem.Control = this.valueGridControl;
		this.layoutControlItem.Location = new System.Drawing.Point(85, 0);
		this.layoutControlItem.MinSize = new System.Drawing.Size(1, 22);
		this.layoutControlItem.Name = "layoutControlItem";
		this.layoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 4, 0, 0);
		this.layoutControlItem.Size = new System.Drawing.Size(415, 22);
		this.layoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem.Text = "Field";
		this.layoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem.TextVisible = false;
		this.fieldNameLayoutControlItem.Control = this.fieldNameLabelControl;
		this.fieldNameLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.fieldNameLayoutControlItem.MaxSize = new System.Drawing.Size(82, 22);
		this.fieldNameLayoutControlItem.MinSize = new System.Drawing.Size(82, 22);
		this.fieldNameLayoutControlItem.Name = "fieldNameLayoutControlItem";
		this.fieldNameLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.fieldNameLayoutControlItem.Size = new System.Drawing.Size(82, 22);
		this.fieldNameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.fieldNameLayoutControlItem.Text = "layoutControlItem";
		this.fieldNameLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.fieldNameLayoutControlItem.TextVisible = false;
		this.emptySpaceItem.AllowHotTrack = false;
		this.emptySpaceItem.Location = new System.Drawing.Point(82, 0);
		this.emptySpaceItem.MaxSize = new System.Drawing.Size(3, 22);
		this.emptySpaceItem.MinSize = new System.Drawing.Size(3, 22);
		this.emptySpaceItem.Name = "emptySpaceItem";
		this.emptySpaceItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem.Size = new System.Drawing.Size(3, 22);
		this.emptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.layoutControl1);
		base.Margin = new System.Windows.Forms.Padding(0);
		this.MaximumSize = new System.Drawing.Size(500, 22);
		this.MinimumSize = new System.Drawing.Size(500, 22);
		base.Name = "CustomFieldControl";
		base.Size = new System.Drawing.Size(500, 22);
		base.Enter += new System.EventHandler(CustomFieldControl_Enter);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.valueGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valueGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
