using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Drivers.ODBC;
using Dataedo.App.Drivers.ODBC.Repositories;
using Dataedo.App.Drivers.ODBC.ValueObjects;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Data.Odbc;
using Dataedo.Data.Odbc.Exceptions;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class ODBCConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl odbcLayoutControl;

	private LabelControl odbcLearnMoreLabelControl;

	private SimpleButton odbcConnectionSimpleButton;

	private MemoEdit odbcConnectionMemoEdit;

	private InfoUserControl infoUserControl1;

	private ButtonEdit odbcDriverButtonEdit;

	private LayoutControlGroup layoutControlGroup9;

	private EmptySpaceItem odbcEmptySpaceItem;

	private EmptySpaceItem odbcTimeoutEmptySpaceItem;

	private LayoutControlItem odbcDriverLayoutControlItem;

	private LayoutControlItem layoutControlItem21;

	private EmptySpaceItem emptySpaceItem67;

	private LayoutControlItem odbcLayoutControlItem;

	private LayoutControlItem odbcLayoutControlItemSimpleButton;

	private LayoutControlItem odbcLearnMoreLabelControlLayoutControlItem;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Odbc;

	public ODBCConnectorControl()
	{
		InitializeComponent();
	}

	public override bool TestConnection(bool testForGettingDatabasesList)
	{
		try
		{
			if (!ValidateRequiredFields(testForGettingDatabasesList))
			{
				GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
				return false;
			}
			if (base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.Odbc)
			{
				try
				{
					new OdbcConnectionStringBuilder(odbcConnectionMemoEdit.Text);
				}
				catch (ArgumentException)
				{
					GeneralMessageBoxesHandling.Show("Invalid format of the connection string.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
					return false;
				}
			}
			SetNewDBRowValues();
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection();
			if (connectionResult.IsSuccess)
			{
				base.DatabaseRow.Connection = connectionResult.Connection;
				if (!string.IsNullOrEmpty(connectionResult.NewConnectionString) && base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.Odbc)
				{
					base.DatabaseRow.Password = connectionResult.NewConnectionString;
					odbcConnectionMemoEdit.Text = base.DatabaseRow.Password;
				}
				SetDBMSVersion();
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				SharedDatabaseTypeEnum.DatabaseType? expectedDatabaseType = GetExpectedDatabaseType();
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				if (!expectedDatabaseType.HasValue)
				{
					return false;
				}
				if (expectedDatabaseType != base.DatabaseRow.Type)
				{
					base.DatabaseRow.Type = expectedDatabaseType;
					base.SelectedDatabaseType = expectedDatabaseType;
				}
				return true;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(connectionResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, connectionResult.Details, 1, messageSimple: connectionResult.MessageSimple, owner: FindForm());
			return false;
		}
		catch
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		OdbcConnectionStringBuilder odbcConnectionStringBuilder = new OdbcConnectionStringBuilder(odbcConnectionMemoEdit.Text);
		string text = odbcConnectionStringBuilder.Dsn;
		if (odbcConnectionStringBuilder.ContainsKey("database"))
		{
			text = odbcConnectionStringBuilder["database"] as string;
		}
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? text : base.DatabaseRow?.Name, documentationTitle, null, null, odbcConnectionMemoEdit.Text, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, odbcDriverButtonEdit.Text, null, null, null, null);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		odbcLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			odbcLayoutControl.Root.AddItem(timeoutLayoutControlItem, odbcTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateOdbcConnection() & ValidateOdbcDriver();
	}

	private bool ValidateOdbcConnection(bool acceptEmptyValue = false)
	{
		if (!ValidateFields.ValidateEdit(odbcConnectionMemoEdit, addDBErrorProvider, "connection", acceptEmptyValue))
		{
			return false;
		}
		try
		{
			new OdbcConnectionStringBuilder(odbcConnectionMemoEdit.Text);
		}
		catch (ArgumentException)
		{
			addDBErrorProvider.SetError(odbcConnectionMemoEdit, "Invalid format of the connection string.");
		}
		return true;
	}

	private bool ValidateOdbcDriver(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(odbcDriverButtonEdit, addDBErrorProvider, "driver", acceptEmptyValue);
	}

	protected override bool GetSavedPassword()
	{
		if (!GetSavePasswordCheckEditState())
		{
			if (base.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Odbc)
			{
				return !string.IsNullOrEmpty(base.DatabaseRow.PasswordEncrypted);
			}
			return false;
		}
		return true;
	}

	protected override void ReadPanelValues()
	{
		odbcConnectionMemoEdit.EditValue = base.DatabaseRow.Password;
		odbcDriverButtonEdit.EditValue = base.DatabaseRow.ServiceName;
	}

	public override void HideOtherTypeFields()
	{
		PanelTypeEnum.PanelType panelType = PanelTypeEnum.PanelType.None;
		if (DatabaseSupportFactoryShared.CheckIfTypeIsSupported(base.SelectedDatabaseType))
		{
			panelType = DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType).PanelType;
		}
		if (panelType == PanelType)
		{
			odbcDriverLayoutControlItem.Visibility = ((!odbcHasAnyLocalDrier()) ? LayoutVisibility.Never : LayoutVisibility.Always);
		}
	}

	private bool odbcHasAnyLocalDrier()
	{
		return Factory.GetLocalRepository().List().Count() > 0;
	}

	protected override string GetPanelDocumentationTitle()
	{
		try
		{
			OdbcConnectionStringBuilder odbcConnectionStringBuilder = new OdbcConnectionStringBuilder(odbcConnectionMemoEdit.Text);
			if (odbcConnectionStringBuilder.TryGetValue("database", out var value) && !string.IsNullOrWhiteSpace((string)value))
			{
				return (string)value;
			}
			if (odbcConnectionStringBuilder.TryGetValue("dbq", out var value2) && !string.IsNullOrWhiteSpace((string)value2))
			{
				return Path.GetFileName((string)value2);
			}
			if (!string.IsNullOrWhiteSpace(odbcConnectionStringBuilder.Dsn))
			{
				return odbcConnectionStringBuilder.Dsn;
			}
			if (!string.IsNullOrWhiteSpace(odbcConnectionStringBuilder.Driver))
			{
				return odbcConnectionStringBuilder.Driver;
			}
		}
		catch
		{
		}
		return odbcDriverButtonEdit.Text;
	}

	private void odbcDriverButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		SetNewDBRowValues(forGettingDatabasesList: true);
		IEnumerable<DriverMetaFile> enumerable = null;
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		enumerable = Factory.GetLocalRepository().List();
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (enumerable == null || enumerable.Count() == 0)
		{
			GeneralMessageBoxesHandling.Show("No drivers found for your application version.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
			return;
		}
		string title = "Drivers list";
		ListForm listForm = new ListForm(enumerable, title);
		if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
		{
			DriverMetaFile driverMetaFile = (DriverMetaFile)listForm.SelectedObject;
			IRepository localRepository = Factory.GetLocalRepository();
			if (!localRepository.Has(driverMetaFile.UID))
			{
				Driver driver = Factory.GetRemoteRepository().Load(driverMetaFile);
				localRepository.Store(driver);
			}
			(sender as ButtonEdit).EditValue = driverMetaFile.UID;
		}
	}

	private void odbcConnectionMemoEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateOdbcConnection(acceptEmptyValue: true);
	}

	private void odbcConnectionSimpleButton_ButtonClick(object sender, EventArgs e)
	{
		base.Enabled = false;
		try
		{
			odbcConnectionMemoEdit.Text = Dataedo.Data.Odbc.DataSources.ConnectionStringWizard();
		}
		catch (ArchitectureMismatchException ex)
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "ODBC Data Source", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		catch (NetworkException ex2)
		{
			GeneralMessageBoxesHandling.Show(ex2.Message, "ODBC Data Source", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		catch (DataSourceWizardException ex3)
		{
			GeneralMessageBoxesHandling.Show(ex3.Message, "ODBC Data Source", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		catch (UnknownConnectionException ex4)
		{
			GeneralMessageBoxesHandling.Show(ex4.Message, "ODBC Data Source", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		catch (IncorrectCredentialsException ex5)
		{
			if (GeneralMessageBoxesHandling.Show(ex5.Message, "ODBC Data Source", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand, null, 1, FindForm()).DialogResult == DialogResult.Retry)
			{
				odbcConnectionSimpleButton_ButtonClick(sender, e);
			}
		}
		catch (EmptyConnectionStringException)
		{
		}
		finally
		{
			base.Enabled = true;
		}
	}

	private void odbcDriverButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateOdbcDriver(acceptEmptyValue: true);
	}

	private void odbcLearnMoreLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		Links.OpenLink(Links.ODBCConnectionToDatabase, FindForm());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.ConnectorsControls.ODBCConnectorControl));
		this.odbcLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.odbcLearnMoreLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.odbcConnectionSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.odbcConnectionMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.infoUserControl1 = new Dataedo.App.UserControls.InfoUserControl();
		this.odbcDriverButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.layoutControlGroup9 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.odbcEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.odbcTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.odbcDriverLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem67 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.odbcLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.odbcLayoutControlItemSimpleButton = new DevExpress.XtraLayout.LayoutControlItem();
		this.odbcLearnMoreLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.odbcLayoutControl).BeginInit();
		this.odbcLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.odbcConnectionMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.odbcDriverButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.odbcEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.odbcTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.odbcDriverLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem67).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.odbcLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.odbcLayoutControlItemSimpleButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.odbcLearnMoreLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.odbcLayoutControl.AllowCustomization = false;
		this.odbcLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.odbcLayoutControl.Controls.Add(this.odbcLearnMoreLabelControl);
		this.odbcLayoutControl.Controls.Add(this.odbcConnectionSimpleButton);
		this.odbcLayoutControl.Controls.Add(this.odbcConnectionMemoEdit);
		this.odbcLayoutControl.Controls.Add(this.infoUserControl1);
		this.odbcLayoutControl.Controls.Add(this.odbcDriverButtonEdit);
		this.odbcLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.odbcLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.odbcLayoutControl.Name = "odbcLayoutControl";
		this.odbcLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1451, 470, 360, 525);
		this.odbcLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.odbcLayoutControl.Root = this.layoutControlGroup9;
		this.odbcLayoutControl.Size = new System.Drawing.Size(612, 393);
		this.odbcLayoutControl.TabIndex = 5;
		this.odbcLayoutControl.Text = "layoutControl1";
		this.odbcLearnMoreLabelControl.AllowHtmlString = true;
		this.odbcLearnMoreLabelControl.Location = new System.Drawing.Point(359, 2);
		this.odbcLearnMoreLabelControl.Name = "odbcLearnMoreLabelControl";
		this.odbcLearnMoreLabelControl.Size = new System.Drawing.Size(252, 20);
		this.odbcLearnMoreLabelControl.StyleController = this.odbcLayoutControl;
		this.odbcLearnMoreLabelControl.TabIndex = 8;
		this.odbcLearnMoreLabelControl.Text = "<href>Learn more</href>";
		this.odbcLearnMoreLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(odbcLearnMoreLabelControl_HyperlinkClick);
		this.odbcConnectionSimpleButton.Appearance.Options.UseTextOptions = true;
		this.odbcConnectionSimpleButton.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.odbcConnectionSimpleButton.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.odbcConnectionSimpleButton.Location = new System.Drawing.Point(336, 3);
		this.odbcConnectionSimpleButton.MaximumSize = new System.Drawing.Size(20, 22);
		this.odbcConnectionSimpleButton.MinimumSize = new System.Drawing.Size(20, 22);
		this.odbcConnectionSimpleButton.Name = "odbcConnectionSimpleButton";
		this.odbcConnectionSimpleButton.Size = new System.Drawing.Size(20, 22);
		this.odbcConnectionSimpleButton.StyleController = this.odbcLayoutControl;
		this.odbcConnectionSimpleButton.TabIndex = 7;
		this.odbcConnectionSimpleButton.Text = "...";
		this.odbcConnectionSimpleButton.Click += new System.EventHandler(odbcConnectionSimpleButton_ButtonClick);
		this.odbcConnectionMemoEdit.EditValue = "";
		this.odbcConnectionMemoEdit.Location = new System.Drawing.Point(105, 3);
		this.odbcConnectionMemoEdit.Name = "odbcConnectionMemoEdit";
		this.odbcConnectionMemoEdit.Size = new System.Drawing.Size(229, 82);
		this.odbcConnectionMemoEdit.StyleController = this.odbcLayoutControl;
		this.odbcConnectionMemoEdit.TabIndex = 6;
		this.odbcConnectionMemoEdit.EditValueChanged += new System.EventHandler(odbcConnectionMemoEdit_EditValueChanged);
		this.infoUserControl1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl1.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl1.Description = "ODBC connection imports only metadata provided by the chosen driver. Try choosing a different driver if you're missing table, view or column metadata after import.";
		this.infoUserControl1.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl1.Image = (System.Drawing.Image)resources.GetObject("infoUserControl1.Image");
		this.infoUserControl1.Location = new System.Drawing.Point(1, 90);
		this.infoUserControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
		this.infoUserControl1.Name = "infoUserControl1";
		this.infoUserControl1.Size = new System.Drawing.Size(610, 40);
		this.infoUserControl1.TabIndex = 5;
		this.odbcDriverButtonEdit.EditValue = "basic";
		this.odbcDriverButtonEdit.Location = new System.Drawing.Point(105, 132);
		this.odbcDriverButtonEdit.Name = "odbcDriverButtonEdit";
		this.odbcDriverButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.odbcDriverButtonEdit.Properties.ReadOnly = true;
		this.odbcDriverButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.odbcDriverButtonEdit.StyleController = this.odbcLayoutControl;
		this.odbcDriverButtonEdit.TabIndex = 4;
		this.odbcDriverButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(odbcDriverButtonEdit_ButtonClick);
		this.odbcDriverButtonEdit.EditValueChanged += new System.EventHandler(odbcDriverButtonEdit_EditValueChanged);
		this.layoutControlGroup9.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup9.GroupBordersVisible = false;
		this.layoutControlGroup9.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.odbcEmptySpaceItem, this.odbcTimeoutEmptySpaceItem, this.odbcDriverLayoutControlItem, this.layoutControlItem21, this.emptySpaceItem67, this.odbcLayoutControlItem, this.odbcLayoutControlItemSimpleButton, this.odbcLearnMoreLabelControlLayoutControlItem });
		this.layoutControlGroup9.Name = "Root";
		this.layoutControlGroup9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup9.Size = new System.Drawing.Size(612, 393);
		this.layoutControlGroup9.TextVisible = false;
		this.odbcEmptySpaceItem.AllowHotTrack = false;
		this.odbcEmptySpaceItem.Location = new System.Drawing.Point(0, 180);
		this.odbcEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.odbcEmptySpaceItem.Name = "odbcEmptySpaceItem";
		this.odbcEmptySpaceItem.Size = new System.Drawing.Size(612, 213);
		this.odbcEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.odbcEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.odbcTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.odbcTimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.odbcTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 156);
		this.odbcTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(405, 24);
		this.odbcTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(405, 24);
		this.odbcTimeoutEmptySpaceItem.Name = "odbcTimeoutEmptySpaceItem";
		this.odbcTimeoutEmptySpaceItem.Size = new System.Drawing.Size(612, 24);
		this.odbcTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.odbcTimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.odbcTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.odbcDriverLayoutControlItem.Control = this.odbcDriverButtonEdit;
		this.odbcDriverLayoutControlItem.CustomizationFormText = "Driver:";
		this.odbcDriverLayoutControlItem.Location = new System.Drawing.Point(0, 132);
		this.odbcDriverLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.odbcDriverLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.odbcDriverLayoutControlItem.Name = "odbcDriverLayoutControlItem";
		this.odbcDriverLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.odbcDriverLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.odbcDriverLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.odbcDriverLayoutControlItem.Text = "Driver:";
		this.odbcDriverLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.odbcDriverLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.odbcDriverLayoutControlItem.TextToControlDistance = 5;
		this.odbcDriverLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem21.Control = this.infoUserControl1;
		this.layoutControlItem21.Location = new System.Drawing.Point(0, 88);
		this.layoutControlItem21.MaxSize = new System.Drawing.Size(0, 44);
		this.layoutControlItem21.MinSize = new System.Drawing.Size(104, 44);
		this.layoutControlItem21.Name = "layoutControlItem21";
		this.layoutControlItem21.Size = new System.Drawing.Size(612, 44);
		this.layoutControlItem21.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem21.Text = " ";
		this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem21.TextVisible = false;
		this.emptySpaceItem67.AllowHotTrack = false;
		this.emptySpaceItem67.Location = new System.Drawing.Point(335, 132);
		this.emptySpaceItem67.Name = "emptySpaceItem67";
		this.emptySpaceItem67.Size = new System.Drawing.Size(277, 24);
		this.emptySpaceItem67.TextSize = new System.Drawing.Size(0, 0);
		this.odbcLayoutControlItem.Control = this.odbcConnectionMemoEdit;
		this.odbcLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.odbcLayoutControlItem.MaxSize = new System.Drawing.Size(334, 88);
		this.odbcLayoutControlItem.MinSize = new System.Drawing.Size(334, 88);
		this.odbcLayoutControlItem.Name = "odbcLayoutControlItem";
		this.odbcLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 3, 3);
		this.odbcLayoutControlItem.Size = new System.Drawing.Size(334, 88);
		this.odbcLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.odbcLayoutControlItem.Text = "Connection:";
		this.odbcLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.odbcLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.odbcLayoutControlItem.TextToControlDistance = 5;
		this.odbcLayoutControlItemSimpleButton.Control = this.odbcConnectionSimpleButton;
		this.odbcLayoutControlItemSimpleButton.Location = new System.Drawing.Point(334, 0);
		this.odbcLayoutControlItemSimpleButton.MaxSize = new System.Drawing.Size(24, 88);
		this.odbcLayoutControlItemSimpleButton.MinSize = new System.Drawing.Size(24, 88);
		this.odbcLayoutControlItemSimpleButton.Name = "odbcLayoutControlItemSimpleButton";
		this.odbcLayoutControlItemSimpleButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 3, 0);
		this.odbcLayoutControlItemSimpleButton.Size = new System.Drawing.Size(24, 88);
		this.odbcLayoutControlItemSimpleButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.odbcLayoutControlItemSimpleButton.TextSize = new System.Drawing.Size(0, 0);
		this.odbcLayoutControlItemSimpleButton.TextVisible = false;
		this.odbcLayoutControlItemSimpleButton.Click += new System.EventHandler(odbcConnectionSimpleButton_ButtonClick);
		this.odbcLearnMoreLabelControlLayoutControlItem.Control = this.odbcLearnMoreLabelControl;
		this.odbcLearnMoreLabelControlLayoutControlItem.Location = new System.Drawing.Point(358, 0);
		this.odbcLearnMoreLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.odbcLearnMoreLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 24);
		this.odbcLearnMoreLabelControlLayoutControlItem.Name = "odbcLearnMoreLabelControlLayoutControlItem";
		this.odbcLearnMoreLabelControlLayoutControlItem.Size = new System.Drawing.Size(254, 88);
		this.odbcLearnMoreLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.odbcLearnMoreLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.odbcLearnMoreLabelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.odbcLayoutControl);
		base.Name = "ODBCConnectorControl";
		base.Size = new System.Drawing.Size(612, 393);
		((System.ComponentModel.ISupportInitialize)this.odbcLayoutControl).EndInit();
		this.odbcLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.odbcConnectionMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.odbcDriverButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.odbcEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.odbcTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.odbcDriverLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem67).EndInit();
		((System.ComponentModel.ISupportInitialize)this.odbcLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.odbcLayoutControlItemSimpleButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.odbcLearnMoreLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
