using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Forms;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class HiveMetastoreMySQLConnectorControl : MySQLConnectorControl
{
	private ButtonEdit hiveDatabaseButtonEdit;

	private LayoutControlItem hiveDatabaseLayoutControlItem;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.HiveMetastoreMySQL;

	protected HiveDatabaseRow HiveDatabaseRow => base.DatabaseRow as HiveDatabaseRow;

	private HiveDatabaseInfo SelectedHiveDatabase => hiveDatabaseButtonEdit?.EditValue as HiveDatabaseInfo;

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new HiveDatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? base.providedMySQLDatabase : base.DatabaseRow?.Param2, SelectedHiveDatabase?.CatalogName, SelectedHiveDatabase?.DatabaseName, documentationTitle, base.providedMySQLHost, base.providedMySQLLogin, base.providedMySQLPassword, base.providedMySQLPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, null, base.DatabaseRow.SSLSettings);
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.databaseLayoutItem.Text = "Metastore database:";
		SetHiveDatabaseControlPosition();
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	protected override void ReadPanelValues()
	{
		base.ReadPanelValues();
		mySqlDatabaseButtonEdit.Text = base.DatabaseRow.Param2;
		HiveDatabaseInfo editValue = new HiveDatabaseInfo
		{
			CatalogName = base.DatabaseRow.Param3?.Trim(),
			DatabaseName = base.DatabaseRow.Param4?.Trim()
		};
		hiveDatabaseButtonEdit.EditValue = editValue;
	}

	private void SetHiveDatabaseControlPosition()
	{
		if (hiveDatabaseLayoutControlItem == null)
		{
			SetHiveDatabaseButtonEdit();
		}
		hiveDatabaseLayoutControlItem.Visibility = LayoutVisibility.Always;
		base.layoutControl.Root.Remove(hiveDatabaseLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			base.layoutControl.Root.AddItem(hiveDatabaseLayoutControlItem, base.databaseLayoutItem, InsertType.Bottom);
		}
	}

	private void SetHiveDatabaseButtonEdit()
	{
		hiveDatabaseButtonEdit = new ButtonEdit
		{
			Margin = new System.Windows.Forms.Padding(4),
			Name = "hiveDatabaseButtonEdit"
		};
		hiveDatabaseButtonEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
		hiveDatabaseButtonEdit.Size = new Size(309, 22);
		hiveDatabaseButtonEdit.ButtonClick += hiveDatabaseButtonEdit_ButtonClick;
		hiveDatabaseButtonEdit.EditValueChanged += hiveDatabaseButtonEdit_EditValueChanged;
		hiveDatabaseLayoutControlItem = new LayoutControlItem
		{
			Control = hiveDatabaseButtonEdit,
			CustomizationFormText = "Database:",
			MaxSize = new Size(335, 24),
			MinSize = new Size(335, 24),
			Name = "hiveDatabaseLayoutControlItem",
			Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0),
			Size = new Size(851, 30),
			SizeConstraintsType = SizeConstraintsType.Custom,
			Text = "Database:",
			TextAlignMode = TextAlignModeItem.CustomSize,
			TextSize = new Size(100, 13),
			TextToControlDistance = 5
		};
	}

	private void hiveDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ButtonEdit buttonEdit = sender as ButtonEdit;
		if (!ValidateRequiredFields(testForGettingDatabasesList: false, testForGettingWarehousesList: false, testForGettingPerspectiveList: true))
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		SetNewDBRowValues();
		if (!HiveDatabaseRow.ValidateHiveDatabase(addDBErrorProvider, mySqlDatabaseButtonEdit, base.SplashScreenManager, FindForm()))
		{
			buttonEdit.EditValue = null;
			HiveDatabaseRow.Param3 = null;
			HiveDatabaseRow.Param4 = null;
			return;
		}
		List<HiveDatabaseInfo> hiveDatabases = HiveDatabaseRow.GetHiveDatabases(base.SplashScreenManager, FindForm());
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (hiveDatabases == null)
		{
			return;
		}
		try
		{
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
			if (connectionResult.Exception != null)
			{
				throw connectionResult.Exception;
			}
			buttonEdit.EditValue = GridForm.ShowForValue(hiveDatabases, "Databases list", this);
		}
		catch (Exception ex)
		{
			DatabaseSupportFactory.GetDatabaseSupport(base.DatabaseRow.Type).ProcessException(ex, base.DatabaseRow.Name, base.DatabaseRow.ServiceName, FindForm());
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		finally
		{
			HiveDatabaseRow.Param3 = null;
			HiveDatabaseRow.Param4 = null;
		}
	}

	protected override string GetPanelDocumentationTitle()
	{
		return SelectedHiveDatabase?.CatalogName + "." + SelectedHiveDatabase?.DatabaseName + "@HiveMetastore";
	}

	private void hiveDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateHiveDatabase(acceptEmptyValue: true);
	}

	private bool ValidateHiveDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(hiveDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = base.ValidatePanelRequiredFields(testForGettingDatabasesList, testForGettingWarehousesList, testForGettingPerspectiveList);
		if (!testForGettingPerspectiveList && !testForGettingDatabasesList)
		{
			flag &= ValidateHiveDatabase();
		}
		return flag;
	}

	protected override void ClearDatabaseData()
	{
		base.DatabaseRow.Param2 = string.Empty;
	}
}
