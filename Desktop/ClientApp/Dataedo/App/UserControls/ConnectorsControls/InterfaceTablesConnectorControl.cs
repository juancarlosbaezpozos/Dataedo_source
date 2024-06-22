using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class InterfaceTablesConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup layoutControlGroup4;

	private EmptySpaceItem databaseNameEmptySpaceItem;

	private LookUpEdit databaseLookUpEdit;

	private LayoutControlItem databaseNameLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.InterfaceTables;

	public override string PreLearnMoreText
	{
		get
		{
			List<string> databaseNames = DatabaseNames;
			if (databaseNames == null || databaseNames.Count <= 0)
			{
				return "No data found in Interface Tables.";
			}
			return null;
		}
	}

	private List<string> DatabaseNames { get; set; }

	protected override string ValidationErrorMessage
	{
		get
		{
			List<string> databaseNames = DatabaseNames;
			if (databaseNames != null && databaseNames.Count > 0)
			{
				return base.ValidationErrorMessage;
			}
			return "No data found in Interface Tables.";
		}
	}

	public InterfaceTablesConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		DatabaseNames = DB.InterfaceTables.GetDatabaseNamesForImport();
		databaseLookUpEdit.Properties.DataSource = DatabaseNames;
		databaseLookUpEdit.EditValue = null;
		List<string> databaseNames = DatabaseNames;
		if (databaseNames != null && databaseNames.Count > 0)
		{
			databaseNameLayoutControlItem.Visibility = LayoutVisibility.Always;
			databaseNameEmptySpaceItem.Visibility = LayoutVisibility.Always;
			ConnectorControlBase.SetDropDownSize(databaseLookUpEdit, DatabaseNames.Count);
		}
		else
		{
			databaseNameLayoutControlItem.Visibility = LayoutVisibility.Never;
			databaseNameEmptySpaceItem.Visibility = LayoutVisibility.Never;
		}
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(StaticData.DataedoConnectionString);
		AuthenticationType.AuthenticationTypeEnum authenticationType = GetAuthenticationType(sqlConnectionStringBuilder);
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? sqlConnectionStringBuilder.InitialCatalog : base.DatabaseRow?.Name, documentationTitle, sqlConnectionStringBuilder.DataSource, sqlConnectionStringBuilder.UserID, sqlConnectionStringBuilder.Password, sqlConnectionStringBuilder.IntegratedSecurity, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, authenticationType);
		base.DatabaseRow.Param1 = $"{databaseLookUpEdit.EditValue}";
	}

	private AuthenticationType.AuthenticationTypeEnum GetAuthenticationType(SqlConnectionStringBuilder sqlConnectionStringBuilder)
	{
		if (sqlConnectionStringBuilder.IntegratedSecurity)
		{
			return AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication;
		}
		return sqlConnectionStringBuilder.Authentication switch
		{
			SqlAuthenticationMethod.ActiveDirectoryIntegrated => AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated, 
			SqlAuthenticationMethod.ActiveDirectoryPassword => AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword, 
			SqlAuthenticationMethod.ActiveDirectoryInteractive => AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryInteractive, 
			_ => AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, 
		};
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateDatabase();
	}

	private bool ValidateDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(databaseLookUpEdit, addDBErrorProvider, "Database name", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		if (!string.IsNullOrEmpty(base.DatabaseRow.Param1) && DatabaseNames.Contains(base.DatabaseRow.Param1))
		{
			databaseLookUpEdit.EditValue = base.DatabaseRow.Param1;
		}
	}

	protected override string GetPanelDocumentationTitle()
	{
		return $"{databaseLookUpEdit.EditValue}";
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.databaseLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.databaseNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.databaseNameEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.databaseLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseNameEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.databaseLookUpEdit);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.mainLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.mainLayoutControl.Root = this.layoutControlGroup4;
		this.mainLayoutControl.Size = new System.Drawing.Size(499, 322);
		this.mainLayoutControl.TabIndex = 3;
		this.mainLayoutControl.Text = "layoutControl1";
		this.databaseLookUpEdit.Location = new System.Drawing.Point(105, 0);
		this.databaseLookUpEdit.Name = "databaseLookUpEdit";
		this.databaseLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.databaseLookUpEdit.Properties.NullText = "";
		this.databaseLookUpEdit.Properties.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth;
		this.databaseLookUpEdit.Properties.ShowFooter = false;
		this.databaseLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.databaseLookUpEdit.StyleController = this.mainLayoutControl;
		this.databaseLookUpEdit.TabIndex = 4;
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.databaseNameLayoutControlItem, this.databaseNameEmptySpaceItem, this.emptySpaceItem1 });
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(499, 322);
		this.layoutControlGroup4.TextVisible = false;
		this.databaseNameLayoutControlItem.Control = this.databaseLookUpEdit;
		this.databaseNameLayoutControlItem.CustomizationFormText = "Database name:";
		this.databaseNameLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.databaseNameLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.databaseNameLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.databaseNameLayoutControlItem.Name = "databaseNameLayoutControlItem";
		this.databaseNameLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.databaseNameLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.databaseNameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.databaseNameLayoutControlItem.Text = "Database name:";
		this.databaseNameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.databaseNameLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.databaseNameLayoutControlItem.TextToControlDistance = 5;
		this.databaseNameEmptySpaceItem.AllowHotTrack = false;
		this.databaseNameEmptySpaceItem.CustomizationFormText = "emptySpaceItem6";
		this.databaseNameEmptySpaceItem.Location = new System.Drawing.Point(335, 0);
		this.databaseNameEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.databaseNameEmptySpaceItem.Name = "databaseNameEmptySpaceItem";
		this.databaseNameEmptySpaceItem.Size = new System.Drawing.Size(164, 24);
		this.databaseNameEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.databaseNameEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(499, 298);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "InterfaceTablesConnectorControl";
		base.Size = new System.Drawing.Size(499, 322);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.databaseLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseNameEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		base.ResumeLayout(false);
	}
}
