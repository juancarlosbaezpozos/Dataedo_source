using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Documentations;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;

namespace Dataedo.App.UserControls;

public class DDLDocumentationsUserControl : BaseUserControl
{
	private class DatabaseListItem
	{
		public int DatabaseId { get; set; }

		public string Title { get; set; }

		public string Type { get; set; }

		public string DBMSVersion { get; set; }

		public bool Enabled { get; set; }
	}

	public const string DisabledDatabaseText = "Export to DDL script is not supported for this database type.";

	private IContainer components;

	private LayoutControlGroup layoutControlGroup;

	private NonCustomizableLayoutControl layoutControl;

	private ToolTipController toolTipController;

	private ListBoxControl listBoxControl;

	private LayoutControlItem listBoxLayoutControlItem;

	public bool IsSelectedDatabaseEnabled => (listBoxControl.GetItem(listBoxControl.SelectedIndex) as DatabaseListItem)?.Enabled ?? false;

	public int DatabaseId { get; set; }

	public string DatabaseDBMSVersion { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; set; }

	public bool HasDatabaseSelected => listBoxControl.SelectedIndex >= 0;

	public DDLDocumentationsUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters()
	{
		listBoxControl.DataSource = from x in DB.Database.GetDataWithoutBusinessGlossaryForMenu()
			select new DatabaseListItem
			{
				DatabaseId = x.DatabaseId,
				Title = x.Title,
				Type = x.Type,
				DBMSVersion = x.DbmsVersion,
				Enabled = DatabaseSupportFactory.GetDatabaseSupport(DatabaseTypeEnum.StringToType(x.Type)).CanGenerateDDLScript
			};
		listBoxControl.ValueMember = "DatabaseId";
		listBoxControl.DisplayMember = "Title";
		listBoxControl.SelectedIndex = -1;
	}

	public void SelectDatabaseRow(int? id)
	{
		listBoxControl.SelectedValue = id;
	}

	public void SetDatabaseInfo()
	{
		int selectedIndex = listBoxControl.SelectedIndex;
		if (listBoxControl.GetItem(selectedIndex) is DatabaseListItem databaseListItem)
		{
			DatabaseId = databaseListItem.DatabaseId;
			DatabaseDBMSVersion = databaseListItem.DBMSVersion;
			DatabaseType = DatabaseTypeEnum.StringToType(databaseListItem.Type);
		}
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl != listBoxControl)
		{
			return;
		}
		ToolTipControlInfo info = null;
		try
		{
			int index = listBoxControl.IndexFromPoint(e.ControlMousePosition);
			if (listBoxControl.GetItem(index) is DatabaseListItem databaseListItem && !databaseListItem.Enabled)
			{
				info = new ToolTipControlInfo(databaseListItem, "Export to DDL script is not supported for this database type.");
			}
		}
		finally
		{
			e.Info = info;
		}
	}

	private void listBoxControl_DrawItem(object sender, ListBoxDrawItemEventArgs e)
	{
		if (listBoxControl.GetItem(e.Index) is DatabaseListItem databaseListItem && !databaseListItem.Enabled)
		{
			e.Appearance.ForeColor = Color.Gray;
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
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.listBoxControl = new DevExpress.XtraEditors.ListBoxControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.listBoxLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.listBoxControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.listBoxLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.listBoxControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(860, 293, 566, 534);
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(615, 329);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.listBoxControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.listBoxControl.Location = new System.Drawing.Point(12, 12);
		this.listBoxControl.Name = "listBoxControl";
		this.listBoxControl.Size = new System.Drawing.Size(591, 305);
		this.listBoxControl.StyleController = this.layoutControl;
		this.listBoxControl.TabIndex = 15;
		this.listBoxControl.ToolTipController = this.toolTipController;
		this.listBoxControl.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(listBoxControl_DrawItem);
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.listBoxLayoutControlItem });
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Size = new System.Drawing.Size(615, 329);
		this.layoutControlGroup.TextVisible = false;
		this.listBoxLayoutControlItem.Control = this.listBoxControl;
		this.listBoxLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.listBoxLayoutControlItem.Name = "listBoxLayoutControlItem";
		this.listBoxLayoutControlItem.Size = new System.Drawing.Size(595, 309);
		this.listBoxLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.listBoxLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Name = "DDLDocumentationsUserControl";
		base.Size = new System.Drawing.Size(615, 329);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.listBoxControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.listBoxLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
