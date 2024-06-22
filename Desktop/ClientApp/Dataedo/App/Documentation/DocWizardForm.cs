using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Documentation.Template;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.ExportCommand;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.DDLGenerating;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.Export.PDF;
using Dataedo.App.Tools.Export.Universal;
using Dataedo.App.Tools.Export.Universal.Customizers;
using Dataedo.App.Tools.Export.Universal.ProgressTrackers;
using Dataedo.App.Tools.Export.Universal.Storage.Providers;
using Dataedo.App.Tools.Export.Universal.Templates;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.TemplateEditor;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ConnectorsControls;
using Dataedo.App.UserControls.DDLGenerating;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout.Events;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraWizard;

namespace Dataedo.App.Documentation;

public class DocWizardForm : BaseXtraForm
{
	private int? currentDatabaseId;

	private DocGenerating docGenerating;

	private int numberOfSteps = 4;

	private bool canCloseWindow;

	private bool exportToDB;

	private bool DDLGenerating;

	private DocFormatEnum.DocFormat docFormat;

	private MetadataEditorUserControl metadataEditorUserControl;

	private SharedObjectTypeEnum.ObjectType? objectType;

	private SharedDatabaseTypeEnum.DatabaseType? currentDatabaseType;

	private bool canExportBG;

	private BackgroundProcessingWorker exportWorker;

	private DateTime exportStartTime;

	private IContainer components;

	private CompletionWizardPage docCompletionWizardPage;

	private DevExpress.XtraWizard.WizardPage docTemplateWizardPage;

	private GridControl templatesGrid;

	private RepositoryItemPictureEdit imageRepositoryItemPictureEdit;

	private WizardControl docWizardControl;

	private DevExpress.XtraWizard.WizardPage exportDataWizardPage;

	private NonCustomizableLayoutControl layoutControl1;

	private TextEdit filenameTextEdit;

	private ButtonEdit folderButtonEdit;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem directoryPathLayoutControlItem;

	private LayoutControlItem fileNameLayoutControlItem;

	private RepositoryItemMemoEdit descriptionRepositoryItemMemoEdit;

	private GridColumn layoutViewColumn2;

	private GridColumn layoutViewColumn4;

	private ProgressBarControl progressBarControl1;

	private DevExpress.XtraWizard.WizardPage formatWizardPage;

	private RadioGroup docFormatRadioGroup;

	private LabelControl radioGroupBuyProLabelControl2;

	private FolderBrowserDialog folderBrowserDialog1;

	private SaveFileDialog excelSaveFileDialog;

	private LabelControl saveExportCommandLabelControl;

	private LayoutControlItem saveExportCommandLayoutControlItem;

	private NonCustomizableLayoutControl layoutControl2;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem layoutControlItem8;

	private SimpleButton customizeSimpleButton;

	private LayoutControlItem customizeLayoutControlItem;

	private EmptySpaceItem customizeEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem6;

	private DevExpress.XtraWizard.WizardPage dbConnectWizardPage;

	private DevExpress.XtraWizard.WizardPage chooseModulesWizardPage;

	private ChooseModulesUserControl chooseModulesUserControl;

	private DevExpress.XtraWizard.WizardPage chooseObjectsWizardPage;

	private ChooseObjectTypesUserControl chooseObjectTypesUserControl;

	private LabelControl radioGroupBuyProLabelControl3;

	private DevExpress.XtraWizard.WizardPage chooseCustomFieldsWizardPage;

	private ChooseCustomFieldsUserControl chooseCustomFieldsUserControl;

	private TextEdit titleTextEdit;

	private LayoutControlItem titleLayoutControlItem;

	private CustomGridUserControl templatesGridView;

	private DevExpress.XtraWizard.WizardPage chooseRepositoryDocumentationsWizardPage;

	private DevExpress.XtraWizard.WizardPage chooseDocumentationsWizardPage;

	private RepositoriesUserControl repositoriesUserControl;

	private DocumentationsUserControl documentationsUserControl;

	private LabelControl radioGroupBuyProLabelControl4;

	private InfoUserControl betaFeatureInfoUserControl;

	private CheckEdit scriptsOnlyCheckEdit;

	private DbConnectUserControlNew dbConnectUserControl;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem4;

	private NonCustomizableLayoutControl generatingDocLayoutControl;

	private RichEditUserControl scriptRichEditUserControl;

	private MarqueeProgressBarControl marqueeProgressBarControl;

	private ProgressBarControl creatingPagesProgressBarControl;

	private ProgressBarControl generatingDocProgressBar;

	private LabelControl exportingLabelObject;

	private LabelControl exportingLabel;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControlItem layoutControlItem5;

	private LayoutControlItem layoutControlItem4;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem exportingDetailsLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem marqueeBarLayoutControlItem;

	private LayoutControlItem scriptLayoutControlItem;

	private LayoutControlGroup layoutControlGroup2;

	private LayoutControlItem layoutControlItem6;

	private HyperLinkEdit copyScriptHyperLinkEdit;

	private LayoutControlItem layoutControlItem9;

	private NonCustomizableLayoutControl layoutControl3;

	private NonCustomizableLayoutControl layoutControl4;

	private PopupMenu templatesPopupMenu;

	private BarButtonItem customizeBarButtonItem;

	private BarButtonItem openFolderBarButtonItem;

	private BarManager tamplatesBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private InfoUserControl infoUserControl;

	private LabelControl legacyTextLabelControl;

	private HyperlinkLabelControl copyDescriptionsHyperlinkLabelControl;

	private HyperlinkLabelControl ExtendedPropHyperlinkLabelControl;

	private LabelControl betaLabelControl;

	private DevExpress.XtraWizard.WizardPage DDLChooseDocumentationWizardPage;

	private DevExpress.XtraWizard.WizardPage DDLScriptTypeWizardPage;

	private DevExpress.XtraWizard.WizardPage DDLExportSettingsWizardPage;

	private DevExpress.XtraWizard.WizardPage DDLChooseObjectsWizardPage;

	private DDLDocumentationsUserControl ddlDocumentationsUserControl;

	private SaveFileDialog sqlSaveFileDialog;

	private DDLExportSettingsUserControl ddlExportSettingsUserControl;

	private NonCustomizableLayoutControl ddlChooseObjectsLayoutControl;

	private LayoutControlGroup Root;

	private PanelControl ddlScriptTypePanelControl;

	private LayoutControlItem ddlScriptTypeLayoutControlItem;

	private EmptySpaceItem emptySpaceItem7;

	private RadioGroup ddlScriptTypeRadioGroup;

	private LabelControl andLabelControl;

	private GrayLabelControl alterGrayLabelControl;

	private GrayLabelControl create2GrayLabelControl;

	private GrayLabelControl createGrayLabelControl;

	private DDLChooseObjectsUserControl ddlChooseObjectsUserControl;

	private InfoUserControl webCatalogInfoUserControl;

	private ToolTipController toolTipController;

	private int currentDatabaseIdValue => currentDatabaseId.GetValueOrDefault();

	public DocWizardForm(int? databaseId, SharedObjectTypeEnum.ObjectType? objectType, SharedDatabaseTypeEnum.DatabaseType? databaseType, MetadataEditorUserControl metadataEditor)
	{
		Paths.CheckAndSaveDefaultDocumentationPath();
		InitializeComponent();
		dbConnectUserControl.IsExporting = true;
		currentDatabaseId = databaseId;
		this.objectType = objectType;
		currentDatabaseType = databaseType;
		metadataEditorUserControl = metadataEditor;
		docGenerating = new DocGenerating(currentDatabaseId, chooseModulesUserControl.DocumentationsModulesData, numberOfSteps, this);
		docGenerating.UpdateProgressEvent += docGenerating_UpdateProgressEvent;
		docGenerating.FinishedEvent += docGenerating_FinishedEvent;
		SetPathData();
		canCloseWindow = true;
		exportToDB = false;
		DDLGenerating = false;
		SetFunctionality();
		scriptRichEditUserControl.RefreshSkin();
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.ExportWelcomePage);
		});
	}

	public void SetCopyDescriptionsPage()
	{
		docWizardControl.SelectedPage = chooseRepositoryDocumentationsWizardPage;
		chooseRepositoryDocumentationsWizardPage.AllowBack = false;
	}

	private void SetFunctionality()
	{
		docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "ep").Description = GetExportCommentsButtonDescription();
		RadioGroupItem radioGroupItem = docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString().Equals("copy"));
		if (StaticData.IsProjectFile)
		{
			radioGroupItem.Description = "<color=192, 192, 192>Copy descriptions to other documentations (requires server repository)</color>";
			int num = 135;
			betaLabelControl.Location = new Point(betaLabelControl.Location.X + num, betaLabelControl.Location.Y);
			copyDescriptionsHyperlinkLabelControl.Location = new Point(copyDescriptionsHyperlinkLabelControl.Location.X + num, copyDescriptionsHyperlinkLabelControl.Location.Y);
			radioGroupBuyProLabelControl3.Location = new Point(radioGroupBuyProLabelControl3.Location.X + num, radioGroupBuyProLabelControl3.Location.Y);
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			radioGroupItem.Description = "<color=192, 192, 192>" + radioGroupItem.Description + " (not available for Business Glossary)</color>";
			int num2 = 175;
			betaLabelControl.Location = new Point(betaLabelControl.Location.X + num2, betaLabelControl.Location.Y);
			copyDescriptionsHyperlinkLabelControl.Location = new Point(copyDescriptionsHyperlinkLabelControl.Location.X + num2, copyDescriptionsHyperlinkLabelControl.Location.Y);
			radioGroupBuyProLabelControl3.Location = new Point(radioGroupBuyProLabelControl3.Location.X + num2, radioGroupBuyProLabelControl3.Location.Y);
		}
		else
		{
			radioGroupItem.Description = "Copy descriptions to other documentations";
		}
		RadioGroupItem radioGroupItem2 = docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "html");
		RadioGroupItem radioGroupItem3 = docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "excel");
		RadioGroupItem radioGroupItem4 = docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "ep");
		bool flag2 = (docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "copy").Enabled = true);
		bool flag4 = (radioGroupItem4.Enabled = flag2);
		bool enabled = (radioGroupItem3.Enabled = flag4);
		radioGroupItem2.Enabled = enabled;
		LabelControl labelControl = radioGroupBuyProLabelControl2;
		LabelControl labelControl2 = radioGroupBuyProLabelControl3;
		flag4 = (radioGroupBuyProLabelControl4.Visible = false);
		enabled = (labelControl2.Visible = flag4);
		labelControl.Visible = enabled;
		if (objectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			docFormatRadioGroup.SelectedIndex = docFormatRadioGroup.Properties.Items.IndexOf(docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "html"));
		}
		SetBusinessGlossaryExportFunctionality();
		if (!DatabaseSupportFactoryShared.CheckIfTypeIsSupported(currentDatabaseType) || !DatabaseSupportFactory.GetDatabaseSupport(currentDatabaseType).CanExportExtendedPropertiesOrComments)
		{
			docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "ep").Enabled = false;
		}
		saveExportCommandLabelControl.Text = "<href>Save export command</href>";
		if (StaticData.IsProjectFile || objectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			radioGroupItem.Enabled = false;
		}
		infoUserControl.Description = "<href=" + Links.DocumentationSharing + ">Learn more</href> about the options";
		SetWebCatalogRecommendation();
		SetLabelsColors();
	}

	private void SetWebCatalogRecommendation()
	{
		webCatalogInfoUserControl.Description = "<href=" + Links.WebCatalog + ">Web Catalog</href> is our recommended way of sharing documentation.";
		bool flag = false;
		try
		{
			flag = RepositoriesDBHelper.GetIsWebPortalConnected();
		}
		catch (Exception ex)
		{
			if (!ex.InnerException.Message.Contains("guid") && !ex.InnerException.Message.Contains("is_web_portal_connected"))
			{
				throw;
			}
		}
		if (flag)
		{
			webCatalogInfoUserControl.Visible = false;
		}
		else
		{
			webCatalogInfoUserControl.Visible = true;
		}
	}

	private void SetLabelsColors()
	{
		legacyTextLabelControl.BackColor = SkinsManager.CurrentSkin.LegacyTextExportBackColor;
		legacyTextLabelControl.ForeColor = SkinsManager.CurrentSkin.LegacyTextExportForeColor;
		betaLabelControl.BackColor = SkinsManager.CurrentSkin.BetaTextExportBackColor;
		betaLabelControl.ForeColor = SkinsManager.CurrentSkin.BetaTextExportForeColor;
		GrayLabelControl grayLabelControl = createGrayLabelControl;
		GrayLabelControl grayLabelControl2 = create2GrayLabelControl;
		Color color = (alterGrayLabelControl.BackColor = SkinsManager.CurrentSkin.DDLExampleBackColor);
		Color color4 = (grayLabelControl.BackColor = (grayLabelControl2.BackColor = color));
		GrayLabelControl grayLabelControl3 = createGrayLabelControl;
		GrayLabelControl grayLabelControl4 = create2GrayLabelControl;
		color = (alterGrayLabelControl.ForeColor = SkinsManager.CurrentSkin.DDLExampleForeColor);
		color4 = (grayLabelControl3.ForeColor = (grayLabelControl4.ForeColor = color));
	}

	private string GetExportCommentsButtonDescription()
	{
		if (DatabaseSupportFactoryShared.CheckIfTypeIsSupported(currentDatabaseType) && DatabaseSupportFactory.GetDatabaseSupport(currentDatabaseType).CanExportExtendedPropertiesOrComments)
		{
			int num = 170;
			string exportToDatabaseButtonDescription = DatabaseSupportFactory.GetDatabaseSupport(currentDatabaseType).ExportToDatabaseButtonDescription;
			if (exportToDatabaseButtonDescription == "Export comments to database")
			{
				num = -10;
			}
			ExtendedPropHyperlinkLabelControl.Location = new Point(ExtendedPropHyperlinkLabelControl.Location.X + num, ExtendedPropHyperlinkLabelControl.Location.Y);
			return exportToDatabaseButtonDescription;
		}
		int num2 = 170;
		ExtendedPropHyperlinkLabelControl.Location = new Point(ExtendedPropHyperlinkLabelControl.Location.X + num2, ExtendedPropHyperlinkLabelControl.Location.Y);
		return "<color=192, 192, 192>Export comments to database (not available for this database type)</color>";
	}

	private void SetBusinessGlossaryExportFunctionality()
	{
		Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary);
		RadioGroupItem radioGroupItem = docFormatRadioGroup.Properties.Items.FirstOrDefault((RadioGroupItem x) => x.Tag.ToString() == "html");
		if (radioGroupItem != null)
		{
			radioGroupItem.Description = "HTML";
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Escape:
			Close();
			break;
		case Keys.Return:
			if (dbConnectUserControl.ContainsFocus)
			{
				dbConnectUserControl.SetPortFromHost();
			}
			base.ProcessCmdKey(ref msg, Keys.Return);
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ActionAfterFinish()
	{
		Invoke((Action)delegate
		{
			docCompletionWizardPage.AllowFinish = true;
			docCompletionWizardPage.AllowCancel = false;
			canCloseWindow = true;
		});
	}

	private void docGenerating_FinishedEvent(object sender, EventArgs e)
	{
		ActionAfterFinish();
		if (!docCompletionWizardPage.AllowCancel)
		{
			Close();
		}
	}

	private void docGenerating_UpdateProgressEvent(object sender, EventArgs e)
	{
		BackgroundWorkerProgressEventArgs backgroundWorkerProgressEventArgs = e as BackgroundWorkerProgressEventArgs;
		generatingDocProgressBar.Position = backgroundWorkerProgressEventArgs.Progress;
		exportingLabel.Text = ((backgroundWorkerProgressEventArgs.Messages != null) ? backgroundWorkerProgressEventArgs.Messages[0] : null);
		if (backgroundWorkerProgressEventArgs.Messages.Count > 1)
		{
			exportingDetailsLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
	}

	private void ForceClose()
	{
		canCloseWindow = true;
		Close();
	}

	private void FormatPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		exportToDB = false;
		DDLGenerating = false;
		canExportBG = false;
		if (objectType != SharedObjectTypeEnum.ObjectType.Repository)
		{
			dbConnectUserControl.InitializeDatabaseRow(currentDatabaseId);
		}
		if (docFormatRadioGroup.SelectedIndex != docFormatRadioGroup.Properties.Items.IndexOf(docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "ep")))
		{
			dbConnectUserControl.SetParameters(currentDatabaseId, false, null, false);
		}
		docCompletionWizardPage.Text = "Generating";
		if (docFormatRadioGroup.SelectedIndex == docFormatRadioGroup.Properties.Items.IndexOf(docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "ep")))
		{
			DatabaseTypes.CreateDatabaseTypesDataSource();
			DBMSGridModel dBMSGridModel = DatabaseTypes.databaseTypesDataSource.FirstOrDefault((DBMSGridModel x) => x.Type.Equals(DatabaseTypeEnum.TypeToString(dbConnectUserControl.DatabaseRow.Type)));
			DbConnectUserControlNew dbConnectUserControlNew = dbConnectUserControl;
			int? databaseId = currentDatabaseId;
			SharedDatabaseTypeEnum.DatabaseType? type = dbConnectUserControl.DatabaseRow.Type;
			DBMSGridModel dbmsGridModel = dBMSGridModel;
			dbConnectUserControlNew.SetParameters(databaseId, false, type, false, dbmsGridModel);
			dbConnectUserControl.SetSwitchDatabaseAvailability(isAvailable: false);
			docWizardControl.SelectedPage = dbConnectWizardPage;
			exportToDB = true;
			e.Handled = true;
		}
		else if (docFormatRadioGroup.SelectedIndex == docFormatRadioGroup.Properties.Items.IndexOf(docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "copy")))
		{
			GeneralMessageBoxesHandling.Show("Copying documentations is irreversible." + Environment.NewLine + "We strongly recommend to <b>backup your repository</b> before continuing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			docWizardControl.SelectedPage = chooseRepositoryDocumentationsWizardPage;
			e.Handled = true;
		}
		else if (docFormatRadioGroup.SelectedIndex == docFormatRadioGroup.Properties.Items.IndexOf(docFormatRadioGroup.Properties.Items.Single((RadioGroupItem x) => x.Tag.ToString() == "ddl")))
		{
			docFormat = DocFormatEnum.DocFormat.DDL;
			DDLGenerating = true;
			ddlDocumentationsUserControl.SetParameters();
			ddlDocumentationsUserControl.SelectDatabaseRow(currentDatabaseId);
			marqueeProgressBarControl.Properties.Stopped = false;
			filenameTextEdit.Text = DB.Database.GetTitle(currentDatabaseIdValue);
			docWizardControl.SelectedPage = DDLChooseDocumentationWizardPage;
			e.Handled = true;
		}
		else
		{
			LoadTemplatesList(e);
			SetCustomizationVisibility();
		}
		if (!DDLGenerating)
		{
			chooseObjectTypesUserControl.IsExportingToDatabase = exportToDB;
			if (exportToDB)
			{
				IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type);
				if (databaseSupport.CanExportExtendedPropertiesOrComments)
				{
					chooseObjectTypesUserControl.LoadObjects(databaseSupport.TypeOfExportToDatabase);
					chooseCustomFieldsUserControl.IsExtendedPropertyExport = databaseSupport.HasExtendedPropertiesExport;
				}
			}
			else
			{
				chooseObjectTypesUserControl.LoadObjects(docFormat, canExportBG);
			}
			chooseObjectsWizardPage.DescriptionText = GetChooseObjectPageDescriptionText();
			chooseCustomFieldsUserControl.Initialize(metadataEditorUserControl.CustomFieldsSupport);
			chooseCustomFieldsUserControl.LoadCustomFields(docFormat);
			chooseCustomFieldsUserControl.LoadDocumentationCustomFields(currentDatabaseId);
			chooseCustomFieldsUserControl.SetView(docFormat);
			chooseModulesUserControl.GetModules(currentDatabaseId, metadataEditorUserControl, !exportToDB, useOtherModule: true, canExportBG, new List<int>());
		}
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderWithEventSpecificFormat(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters(), GetSelectedFormat()), TrackingEventEnum.ExportChooseFormat);
		});
	}

	private string GetSelectedFormat()
	{
		return docFormat switch
		{
			DocFormatEnum.DocFormat.Excel => "EXCEL", 
			DocFormatEnum.DocFormat.HTML => "HTML", 
			DocFormatEnum.DocFormat.PDF => "PDF", 
			DocFormatEnum.DocFormat.DDL => "DDL", 
			_ => "DATABASE", 
		};
	}

	private void RepositoryNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (repositoriesUserControl.IsReady())
		{
			if (!RepositoriesDBHelper.CheckRepository(repositoriesUserControl.GetSelectedRepositoryTitle(), this))
			{
				docWizardControl.SelectedPage = chooseRepositoryDocumentationsWizardPage;
				e.Handled = true;
				return;
			}
			documentationsUserControl.SetParameters(currentDatabaseIdValue, repositoriesUserControl.GetSelectedRepositoryTitle());
			DocumentationsUserControl obj = documentationsUserControl;
			obj.RedrawLabel = (Action<string, string>)Delegate.Combine(obj.RedrawLabel, new Action<string, string>(RedrawLabel));
			DocumentationsUserControl obj2 = documentationsUserControl;
			obj2.SetControls = (Action<bool>)Delegate.Combine(obj2.SetControls, new Action<bool>(RedrawControlsAfterCopying));
			docWizardControl.SelectedPage = chooseDocumentationsWizardPage;
			marqueeProgressBarControl.Properties.Stopped = false;
			e.Handled = true;
		}
		else
		{
			docWizardControl.SelectedPage = chooseRepositoryDocumentationsWizardPage;
			e.Handled = true;
		}
	}

	public void InvokeClose()
	{
		Invoke((Action)delegate
		{
			Close();
		});
	}

	private void DocumentationsNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (!documentationsUserControl.HasAnyItemSelected())
		{
			docWizardControl.SelectedPage = chooseDocumentationsWizardPage;
			GeneralMessageBoxesHandling.Show("Select at least one documentation to copy", "Choose documentations to copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			e.Handled = true;
		}
		else if (!documentationsUserControl.HasSelectedAnyObject())
		{
			documentationsUserControl.SetErrors();
			docWizardControl.SelectedPage = chooseDocumentationsWizardPage;
			GeneralMessageBoxesHandling.Show("Choose at least one option to continue.", "Choose copy settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			e.Handled = true;
		}
		else if (documentationsUserControl.IsDataValid())
		{
			bool flag = documentationsUserControl.GetDescriptionsParameter() == 1;
			bool flag2 = documentationsUserControl.GetUserObjectsParameter() == 1;
			string text = "descriptions and user defined objects";
			if (flag && !flag2)
			{
				text = "descriptions";
			}
			else if (!flag && flag2)
			{
				text = "user defined objects";
			}
			string message;
			if (documentationsUserControl.IsCopyingInCurrentRepository)
			{
				message = "Are you sure you want to overwrite " + text + " for selected documentations?";
			}
			else
			{
				string database = StaticData.Database;
				string destinationRepository = documentationsUserControl.DestinationRepository;
				message = "Are you sure you want to overwrite " + text + " in <b>" + destinationRepository + "</b> with descriptions from <b>" + database + "</b>?";
			}
			if (GeneralMessageBoxesHandling.Show(message, "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, null, 1, this).DialogResult != DialogResult.Yes)
			{
				e.Handled = true;
				return;
			}
			marqueeBarLayoutControlItem.Visibility = LayoutVisibility.Always;
			exportingLabel.Text = "Copying documentation descriptions";
			LayoutControlItem layoutControlItem = exportingDetailsLayoutControlItem;
			LayoutControlItem layoutControlItem2 = layoutControlItem4;
			EmptySpaceItem emptySpaceItem = emptySpaceItem1;
			LayoutVisibility layoutVisibility2 = (emptySpaceItem2.Visibility = LayoutVisibility.Never);
			LayoutVisibility layoutVisibility4 = (emptySpaceItem.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility7 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility4));
			docCompletionWizardPage.AllowBack = false;
			docCompletionWizardPage.AllowCancel = false;
			docCompletionWizardPage.AllowFinish = false;
			docWizardControl.SelectedPage = docCompletionWizardPage;
			documentationsUserControl.CopyDocumentations();
			e.Handled = true;
		}
		else
		{
			docWizardControl.SelectedPage = chooseDocumentationsWizardPage;
			e.Handled = true;
		}
	}

	public bool HasCopiedInCurrentRepository()
	{
		if (documentationsUserControl.IsCopyingInCurrentRepository)
		{
			return documentationsUserControl.HasFinished;
		}
		return false;
	}

	private void RedrawLabel(string source, string destination)
	{
		exportingLabel.Invoke((Action)delegate
		{
			string text = "Copying " + source + " descriptions to " + destination;
			if (text.Length > 70)
			{
				exportingLabel.Text = text.Substring(0, 70) + "...";
				exportingLabel.ToolTip = text;
			}
			else
			{
				exportingLabel.Text = "Copying " + source + " descriptions to " + destination;
			}
		});
	}

	private void RedrawControlsAfterCopying(bool hasErrors)
	{
		exportingLabel.Invoke((Action)delegate
		{
			exportingLabel.MinimumSize = new Size(413, 16);
			if (!hasErrors)
			{
				exportingLabel.Text = "Copying finished";
				exportingLabel.Appearance.Image = Resources.ok_16;
			}
			else
			{
				exportingLabel.Text = "Copying failed";
				exportingLabel.Appearance.Image = Resources.blank_16;
			}
		});
		marqueeProgressBarControl.Invoke((Action)delegate
		{
			marqueeBarLayoutControlItem.Visibility = LayoutVisibility.Never;
		});
		Invoke((Action)delegate
		{
			docCompletionWizardPage.AllowFinish = true;
		});
	}

	private string GetChooseObjectPageDescriptionText()
	{
		_ = dbConnectUserControl.DatabaseRow.Type;
		if (!exportToDB)
		{
			return "Please select database object types you want to include in documentation.";
		}
		return DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type).ChooseObjectPageDescriptionText;
	}

	private void CustomFieldsPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (!chooseCustomFieldsUserControl.ValidateExtendedPropertyColumnValue())
		{
			chooseCustomFieldsUserControl.ValidateColumns();
			GeneralMessageBoxesHandling.Show("Please provide extended property name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			docWizardControl.SelectedPage = chooseCustomFieldsWizardPage;
			e.Handled = true;
		}
		else if (!chooseCustomFieldsUserControl.ValidateExtendedPropertySelectedItems())
		{
			GeneralMessageBoxesHandling.Show("Check at least one object.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			docWizardControl.SelectedPage = chooseCustomFieldsWizardPage;
			e.Handled = true;
		}
		else if (scriptsOnlyCheckEdit.Checked)
		{
			UpdateExtendedPropertiesDefinition();
			GenerateScript();
		}
		else
		{
			UpdateExtendedPropertiesDefinition();
			ProcessExportToDatabase();
		}
	}

	private void UpdateExtendedPropertiesDefinition()
	{
		chooseCustomFieldsUserControl.UpdateCustomFieldsExtendedProperties(chooseCustomFieldsUserControl.CustomFieldsSupport.DocumentationCustomFields.Where((DocumentationCustomFieldRow x) => x.IsSelected).ToList(), currentDatabaseIdValue);
	}

	private void docWizardControl_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		if (e.Page.Equals(formatWizardPage))
		{
			FormatPageNextClick(e);
		}
		else if (e.Page.Equals(chooseRepositoryDocumentationsWizardPage))
		{
			docCompletionWizardPage.Text = "Copying";
			RepositoryNextClick(e);
		}
		else if (e.Page.Equals(chooseDocumentationsWizardPage))
		{
			DocumentationsNextClick(e);
		}
		else if (e.Page.Equals(DDLChooseDocumentationWizardPage))
		{
			DDLChooseDocumentationNextClick(e);
		}
		else if (e.Page.Equals(DDLExportSettingsWizardPage))
		{
			DDLExportSettingsNextClick(e);
		}
		else if (e.Page.Equals(DDLScriptTypeWizardPage))
		{
			ScriptType.ScriptTypeEnum scriptType = ScriptType.StringToType(ddlScriptTypeRadioGroup.EditValue?.ToString());
			ddlChooseObjectsUserControl.LoadObjects(ddlDocumentationsUserControl.DatabaseId, scriptType);
		}
		else if (e.Page.Equals(docTemplateWizardPage))
		{
			chooseModulesWizardPage.DescriptionText = "Please choose Subject Areas you want to include in documentation.";
			docWizardControl.SelectedPage = chooseModulesWizardPage;
			e.Handled = true;
		}
		else if (e.Page.Equals(dbConnectWizardPage))
		{
			if (!dbConnectUserControl.TestConnection(testForGettingDatabasesList: false))
			{
				e.Handled = true;
				return;
			}
			dbConnectUserControl.Save();
			if (chooseModulesUserControl.DocumentationsModulesData.Data.Select((DocumentationModules x) => x.Modules).Single().Count == 0 && exportToDB)
			{
				docWizardControl.SelectedPage = chooseObjectsWizardPage;
				e.Handled = true;
			}
			else if (exportToDB)
			{
				docWizardControl.SelectedPage = chooseModulesWizardPage;
				e.Handled = true;
			}
			chooseModulesWizardPage.DescriptionText = "Please choose Subject Areas you want to export.";
		}
		else if (e.Page.Equals(chooseModulesWizardPage))
		{
			chooseObjectsWizardPage.DescriptionText = GetChooseObjectPageDescriptionText();
			if (chooseModulesUserControl.DocumentationsModulesData.GetSelectedDocumentations().Count() == 0)
			{
				GeneralMessageBoxesHandling.Show("Choose at least one documentation to export to continue.", "Choose documentations", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
				e.Handled = true;
				return;
			}
			bool areManyDocumentationsSelected = chooseModulesUserControl.AreManyDocumentationsSelected;
			if (!areManyDocumentationsSelected)
			{
				docGenerating.ExportFile = new ExportFile(chooseModulesUserControl.DocumentationId);
			}
			filenameTextEdit.Text = (areManyDocumentationsSelected ? "Dataedo Data Dictionary" : docGenerating.ExportFile.FileName);
			titleTextEdit.Text = (areManyDocumentationsSelected ? "Data Dictionary" : docGenerating.ExportFile.Title);
		}
		else if (e.Page.Equals(chooseObjectsWizardPage))
		{
			chooseCustomFieldsWizardPage.Text = ((docFormat == DocFormatEnum.DocFormat.HTML) ? "Choose fields to export" : "Choose custom fields");
			chooseCustomFieldsWizardPage.DescriptionText = (exportToDB ? "Please choose custom fields you want to export." : ((docFormat == DocFormatEnum.DocFormat.HTML) ? "Please select fields you want to include in documentation." : "Please select custom fields you want to include in documentation."));
			if (chooseObjectTypesUserControl.SelectedObjectTypesCount == 0)
			{
				GeneralMessageBoxesHandling.Show("Choose at least one object type to export to continue.", "Choose object types", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
				e.Handled = true;
				return;
			}
			if (!exportToDB && chooseCustomFieldsUserControl.CustomFieldsSupport.Fields.Count != 0)
			{
				docWizardControl.SelectedPage = chooseCustomFieldsWizardPage;
			}
			else if (!exportToDB && chooseCustomFieldsUserControl.CustomFieldsSupport.Fields.Count == 0)
			{
				docWizardControl.SelectedPage = exportDataWizardPage;
			}
			else if (exportToDB)
			{
				IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type);
				if (databaseSupport.HasExtendedPropertiesExport && chooseCustomFieldsUserControl.CustomFieldsSupport.Fields.Count != 0)
				{
					docWizardControl.SelectedPage = chooseCustomFieldsWizardPage;
				}
				else if (scriptsOnlyCheckEdit.Checked && databaseSupport.CanExportExtendedPropertiesOrComments)
				{
					GenerateScript();
				}
				else
				{
					ProcessExportToDatabase();
				}
			}
			e.Handled = true;
		}
		else if (e.Page.Equals(chooseCustomFieldsWizardPage))
		{
			if (exportToDB)
			{
				CustomFieldsPageNextClick(e);
				return;
			}
			if (docFormat == DocFormatEnum.DocFormat.HTML)
			{
				exportDataWizardPage.Text = "Choose folder";
				exportDataWizardPage.DescriptionText = "Please choose a folder.";
			}
			else
			{
				exportDataWizardPage.Text = "Choose file";
				exportDataWizardPage.DescriptionText = "Please choose folder and file name.";
			}
			switch (docFormat)
			{
			case DocFormatEnum.DocFormat.PDF:
				fileNameLayoutControlItem.Text = "Filename:";
				break;
			case DocFormatEnum.DocFormat.HTML:
				directoryPathLayoutControlItem.Text = "Path:";
				fileNameLayoutControlItem.Text = "Folder:";
				filenameTextEdit.Text = Regex.Replace(filenameTextEdit.Text.ToLower(), "[\\s]+", "-");
				break;
			case DocFormatEnum.DocFormat.Excel:
				fileNameLayoutControlItem.Text = "Filename:";
				break;
			}
		}
		else if (e.Page.Equals(exportDataWizardPage))
		{
			if (DDLGenerating)
			{
				ProcessGenerateDDL(e);
			}
			else
			{
				ProcessExportToDocument(e);
			}
		}
	}

	private void DDLChooseDocumentationNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (!ddlDocumentationsUserControl.HasDatabaseSelected)
		{
			GeneralMessageBoxesHandling.Show("Select documentation to continue", "Select documentation to continue", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			docWizardControl.SelectedPage = DDLChooseDocumentationWizardPage;
		}
		else if (!ddlDocumentationsUserControl.IsSelectedDatabaseEnabled)
		{
			GeneralMessageBoxesHandling.Show("Export to DDL script is not supported for this database type.", "Select documentation to continue", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			docWizardControl.SelectedPage = DDLChooseDocumentationWizardPage;
		}
		else
		{
			ddlDocumentationsUserControl.SetDatabaseInfo();
			docWizardControl.SelectedPage = DDLScriptTypeWizardPage;
		}
		e.Handled = true;
	}

	private void DDLExportSettingsNextClick(WizardCommandButtonClickEventArgs e)
	{
		docGenerating.ExportFile = new ExportFile(ddlDocumentationsUserControl.DatabaseId, DocFormatEnum.DocFormat.DDL);
		filenameTextEdit.Text = docGenerating.ExportFile.FileName;
		docWizardControl.SelectedPage = exportDataWizardPage;
		e.Handled = true;
	}

	private void docWizardControl_PrevClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		if (e.Page.Equals(dbConnectWizardPage))
		{
			docWizardControl.SelectedPage = formatWizardPage;
			chooseCustomFieldsUserControl.IsExtendedPropertyExport = false;
			e.Handled = true;
		}
		if (e.Page.Equals(chooseModulesWizardPage))
		{
			if (exportToDB)
			{
				docWizardControl.SelectedPage = dbConnectWizardPage;
			}
			else
			{
				docWizardControl.SelectedPage = docTemplateWizardPage;
			}
			e.Handled = true;
		}
		if (e.Page.Equals(DDLChooseDocumentationWizardPage))
		{
			docWizardControl.SelectedPage = formatWizardPage;
			e.Handled = true;
		}
		if (e.Page.Equals(DDLExportSettingsWizardPage))
		{
			docWizardControl.SelectedPage = DDLChooseObjectsWizardPage;
			e.Handled = true;
		}
		DocumentationModulesContainer documentationsModulesData = chooseModulesUserControl.DocumentationsModulesData;
		if (documentationsModulesData != null && documentationsModulesData.Data.Select((DocumentationModules x) => x.Modules).First().Count == 0 && e.Page.Equals(chooseObjectsWizardPage) && exportToDB)
		{
			docWizardControl.SelectedPage = dbConnectWizardPage;
			e.Handled = true;
		}
		if (e.Page.Equals(exportDataWizardPage))
		{
			if (DDLGenerating)
			{
				docWizardControl.SelectedPage = DDLExportSettingsWizardPage;
				e.Handled = true;
			}
			else if (chooseCustomFieldsUserControl.CustomFieldsSupport.Fields.Count == 0)
			{
				docWizardControl.SelectedPage = chooseObjectsWizardPage;
				e.Handled = true;
			}
		}
		if (e.Page.Equals(chooseDocumentationsWizardPage))
		{
			docWizardControl.SelectedPage = chooseRepositoryDocumentationsWizardPage;
			e.Handled = true;
		}
		if (e.Page.Equals(chooseRepositoryDocumentationsWizardPage))
		{
			docWizardControl.SelectedPage = formatWizardPage;
			e.Handled = true;
		}
	}

	private void ProcessExportToDocument(WizardCommandButtonClickEventArgs e)
	{
		docCompletionWizardPage.AllowFinish = false;
		docCompletionWizardPage.AllowBack = false;
		if (!Paths.IsFilePathCorrect(folderButtonEdit.Text, filenameTextEdit.Text, this))
		{
			e.Handled = true;
			return;
		}
		docGenerating.ExportFile.Directory = folderButtonEdit.Text;
		docGenerating.ExportFile.FileName = filenameTextEdit.Text;
		switch (docFormat)
		{
		case DocFormatEnum.DocFormat.PDF:
			if (docGenerating.SetExportData(docGenerating.ExportFile))
			{
				DocGeneratingOptions docGeneratingOptions2 = new DocGeneratingOptions(metadataEditorUserControl.CustomFieldsSupport)
				{
					DocumentationTitle = titleTextEdit.Text,
					TemplateFilePath = (templatesGridView.GetFocusedRow() as DocTemplateFile).FileName,
					CoreTemplateFilePath = (templatesGridView.GetFocusedRow() as DocTemplateFile).CoreTemplateFilePath,
					Template = (templatesGridView.GetFocusedRow() as DocTemplateFile).Template,
					DocumentationsModulesData = chooseModulesUserControl.DocumentationsModulesData,
					ExcludedObjects = chooseObjectTypesUserControl.GetExcludedDbObjects(),
					ProgressBar = creatingPagesProgressBarControl
				};
				docGeneratingOptions2.SetLoadingErd();
				docGeneratingOptions2.CustomFields = chooseCustomFieldsUserControl.CustomFieldsSupport;
				Paths.UpdateDocumentationSaveDirectoryPath(docGenerating.ExportFile.Directory);
				exportingDetailsLayoutControlItem.Visibility = LayoutVisibility.Never;
				try
				{
					canCloseWindow = false;
					docGenerating.GenerateDocument(docGeneratingOptions2);
					break;
				}
				catch (Exception ex2)
				{
					GeneralExceptionHandling.Handle(ex2, ex2.Message, this);
					ForceClose();
					break;
				}
				finally
				{
					StaticData.ClearDatabaseInfoForCrashes();
				}
			}
			e.Handled = true;
			break;
		case DocFormatEnum.DocFormat.HTML:
		{
			string fullPath = Path.Combine(folderButtonEdit.Text, filenameTextEdit.Text);
			if (!Paths.IsValidName(string.Empty, fullPath, showMessage: true, this))
			{
				GeneralMessageBoxesHandling.Show("Check file location or file name and try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
				e.Handled = true;
				break;
			}
			if (Directory.Exists(fullPath) && Directory.EnumerateFileSystemEntries(fullPath).Count() > 0)
			{
				GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("The folder " + fullPath + " is not empty and files may be overwritten. Continue?", "Folder is not empty", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, 1, this);
				if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
				{
					e.Handled = true;
					break;
				}
			}
			try
			{
				Directory.CreateDirectory(fullPath);
			}
			catch (Exception exception)
			{
				GeneralFileExceptionHandling.Handle(exception, "An error occurred while exporting documentation.");
				e.Handled = true;
				break;
			}
			Paths.UpdateDocumentationSaveDirectoryPath(docGenerating.ExportFile.Directory);
			exportingDetailsLayoutControlItem.Visibility = LayoutVisibility.Never;
			exportStartTime = DateTime.UtcNow;
			try
			{
				WizardOptions options = new WizardOptions
				{
					Title = titleTextEdit.Text,
					Scope = chooseModulesUserControl.DocumentationsModulesData,
					ExcludedObject = chooseObjectTypesUserControl.GetExcludedDbObjects(),
					CustomFields = chooseCustomFieldsUserControl.CustomFieldsSupport,
					OtherFields = chooseCustomFieldsUserControl.OtherFieldsSupport
				};
				if (!canExportBG)
				{
					options.ExcludedObject.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.BusinessGlossary));
				}
				exportWorker = new BackgroundProcessingWorker(this);
				exportWorker.SetEventsHandling(delegate
				{
					ITemplate template = (templatesGridView.GetFocusedRow() as DocTemplateFile).CustomObject as ITemplate;
					LocalStorage destination = new LocalStorage(fullPath);
					BackgroundWorkerTracker progress = new BackgroundWorkerTracker(exportWorker);
					TemplatesManager templatesManager = TemplatesFactory.MakeHtmlTemplatesManager(this);
					try
					{
						templatesManager.Export(template, destination, progress, options);
					}
					catch (OperationCanceledException)
					{
						exportWorker.HasResult = false;
					}
					templatesManager = null;
					GC.Collect();
				}, delegate
				{
					generatingDocProgressBar.ShowProgressInTaskBar = true;
					generatingDocProgressBar.Text = ((int)Math.Round(exportWorker.TotalProgress)).ToString();
					creatingPagesProgressBarControl.Text = ((int)Math.Round(exportWorker.CurrentProgress)).ToString();
					exportingLabel.Text = exportWorker.CurrentStage;
					exportingLabelObject.Text = exportWorker.CurrentObject;
				}, delegate
				{
					ActionAfterFinish();
					if (exportWorker.IsCancelled)
					{
						exportingLabel.Text = "Cancelled!";
					}
					else if (exportWorker.HasResult)
					{
						if (!exportWorker.HasError)
						{
							generatingDocProgressBar.Text = "100";
							creatingPagesProgressBarControl.Text = "100";
						}
						Paths.OpenSavedFile(Path.Combine(fullPath, "index.html"), this);
					}
					Close();
				});
				canCloseWindow = false;
				exportWorker.Start();
				break;
			}
			catch (Exception ex3)
			{
				GeneralExceptionHandling.Handle(ex3, ex3.Message, this);
				ForceClose();
				break;
			}
		}
		case DocFormatEnum.DocFormat.Excel:
			if (docGenerating.ExportFile.Extension == ".pdf")
			{
				docGenerating.ExportFile.Extension = "." + excelSaveFileDialog.DefaultExt;
			}
			if (docGenerating.SetExportData(docGenerating.ExportFile))
			{
				DocGeneratingOptions docGeneratingOptions = new DocGeneratingOptions(metadataEditorUserControl.CustomFieldsSupport)
				{
					TemplateFilePath = (templatesGridView.GetFocusedRow() as DocTemplateFile).FileName,
					DocumentationsModulesData = chooseModulesUserControl.DocumentationsModulesData,
					ExcludedObjects = chooseObjectTypesUserControl.GetExcludedDbObjects(),
					ProgressBar = creatingPagesProgressBarControl
				};
				docGeneratingOptions.SetLoadingErd();
				docGeneratingOptions.CustomFields = chooseCustomFieldsUserControl.CustomFieldsSupport;
				Paths.UpdateDocumentationSaveDirectoryPath(docGenerating.ExportFile.Directory);
				exportingDetailsLayoutControlItem.Visibility = LayoutVisibility.Always;
				try
				{
					string path = docGenerating.ExportFile.FullPath;
					exportWorker = new BackgroundProcessingWorker(this);
					exportWorker.SetEventsHandling(delegate
					{
						ExportMethods.ExportToExcel(new SpreadsheetControl(), exportWorker, path, chooseModulesUserControl.DocumentationsModulesData, chooseObjectTypesUserControl.GetExcludedDbObjects(), chooseCustomFieldsUserControl.CustomFieldsSupport, Path.GetFileNameWithoutExtension(docGeneratingOptions.TemplateFilePath));
					}, delegate
					{
						generatingDocProgressBar.Text = ((int)Math.Round(exportWorker.TotalProgress)).ToString();
						creatingPagesProgressBarControl.Text = ((int)Math.Round(exportWorker.CurrentProgress)).ToString();
						exportingLabel.Text = exportWorker.CurrentStage;
						exportingLabelObject.Text = exportWorker.CurrentObject;
					}, delegate
					{
						ActionAfterFinish();
						if (exportWorker.IsCancelled)
						{
							exportingLabel.Text = "Cancelled!";
						}
						else if (exportWorker.HasResult)
						{
							if (!exportWorker.HasError)
							{
								generatingDocProgressBar.Text = "100";
								creatingPagesProgressBarControl.Text = "100";
							}
							Paths.OpenSavedFile(path, this);
						}
						StaticData.ClearDatabaseInfoForCrashes();
						Close();
					});
					canCloseWindow = false;
					exportWorker.Start();
					break;
				}
				catch (Exception ex)
				{
					GeneralExceptionHandling.Handle(ex, ex.Message, this);
					ForceClose();
					break;
				}
			}
			e.Handled = true;
			break;
		}
	}

	private string GetScript()
	{
		try
		{
			if (chooseModulesUserControl.GetChosenModules().Count != 0)
			{
				IEnumerable<string> extendedPropertiesOrCommentsScriptLines = DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type).GetExtendedPropertiesOrCommentsScriptLines(exportWorker, dbConnectUserControl.DatabaseRow.GetConnectionString(withUnicode: true), currentDatabaseIdValue, chooseModulesUserControl.GetChosenModules(), chooseCustomFieldsUserControl, chooseObjectTypesUserControl);
				return GenerateText(extendedPropertiesOrCommentsScriptLines);
			}
			return null;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
			return null;
		}
	}

	private string GenerateText(IEnumerable<string> objects)
	{
		string text = string.Empty;
		foreach (string @object in objects)
		{
			text = text + @object + ";";
			text += Environment.NewLine;
		}
		text.Trim();
		return text;
	}

	private void GenerateScript()
	{
		LayoutControlItem layoutControlItem = layoutControlItem1;
		EmptySpaceItem emptySpaceItem = emptySpaceItem1;
		LayoutControlItem layoutControlItem2 = layoutControlItem4;
		EmptySpaceItem emptySpaceItem2 = this.emptySpaceItem2;
		EmptySpaceItem emptySpaceItem3 = this.emptySpaceItem3;
		LayoutControlItem layoutControlItem3 = marqueeBarLayoutControlItem;
		LayoutVisibility layoutVisibility2 = (exportingDetailsLayoutControlItem.Visibility = LayoutVisibility.Never);
		LayoutVisibility layoutVisibility4 = (layoutControlItem3.Visibility = layoutVisibility2);
		LayoutVisibility layoutVisibility6 = (emptySpaceItem3.Visibility = layoutVisibility4);
		LayoutVisibility layoutVisibility8 = (emptySpaceItem2.Visibility = layoutVisibility6);
		LayoutVisibility layoutVisibility10 = (layoutControlItem2.Visibility = layoutVisibility8);
		LayoutVisibility layoutVisibility13 = (layoutControlItem.Visibility = (emptySpaceItem.Visibility = layoutVisibility10));
		docWizardControl.SelectedPage = docCompletionWizardPage;
		marqueeProgressBarControl.Properties.Stopped = false;
		docCompletionWizardPage.AllowFinish = false;
		docCompletionWizardPage.AllowBack = false;
		docCompletionWizardPage.AllowCancel = false;
		canCloseWindow = false;
		bool hasErrors = false;
		marqueeProgressBarControl.Invoke((Action)delegate
		{
			marqueeBarLayoutControlItem.Visibility = LayoutVisibility.Always;
		});
		new TaskFactory().StartNew(delegate
		{
			try
			{
				StaticData.CrashedDatabaseType = dbConnectUserControl?.DatabaseRow?.Type;
				StaticData.CrashedDBMSVersion = dbConnectUserControl?.DatabaseRow?.DbmsVersion;
				scriptRichEditUserControl.HtmlText = GetScript();
			}
			catch (Exception exception)
			{
				hasErrors = true;
				GeneralExceptionHandling.Handle(exception, "Error while copying documentation", this);
			}
			finally
			{
				StaticData.ClearDatabaseInfoForCrashes();
			}
		}).ContinueWith(delegate
		{
			SetControlsAfterScriptGeneration(hasErrors);
		});
	}

	private string GetExportedScriptLabelText()
	{
		string empty = string.Empty;
		empty = ((currentDatabaseType != SharedDatabaseTypeEnum.DatabaseType.SqlServer && currentDatabaseType != SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase && currentDatabaseType != SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse) ? "comments" : "extended properties");
		return "Run the script below in your database to add " + empty + "." + Environment.NewLine + "If you don't have permissions to edit " + empty + " on some objects," + Environment.NewLine + "you may need to remove relevant lines.";
	}

	private void SetControlsAfterScriptGeneration(bool hasErrors)
	{
		canCloseWindow = false;
		if (!hasErrors)
		{
			exportingLabel.Invoke((Action)delegate
			{
				exportingLabel.MinimumSize = new Size(413, 16);
				exportingLabel.Text = GetExportedScriptLabelText();
				exportingLabel.Appearance.Image = Resources.ok_16;
			});
			copyScriptHyperLinkEdit.Invoke((Action)delegate
			{
				layoutControlItem9.Visibility = LayoutVisibility.Always;
			});
			ActionAfterFinish();
		}
		else
		{
			exportingLabel.Invoke((Action)delegate
			{
				exportingLabel.MinimumSize = new Size(413, 16);
				exportingLabel.Text = "HOLDER";
				exportingLabel.Appearance.Image = Resources.error_16;
				layoutControlItem4.Visibility = LayoutVisibility.Never;
			});
		}
		marqueeProgressBarControl.Invoke((Action)delegate
		{
			marqueeBarLayoutControlItem.Visibility = LayoutVisibility.Never;
		});
		scriptRichEditUserControl.Invoke((Action)delegate
		{
			scriptLayoutControlItem.Visibility = LayoutVisibility.Always;
		});
	}

	private void ProcessGenerateDDL(WizardCommandButtonClickEventArgs e)
	{
		if (!Paths.IsFilePathCorrect(folderButtonEdit.Text, filenameTextEdit.Text, this))
		{
			e.Handled = true;
			return;
		}
		docCompletionWizardPage.AllowBack = false;
		docCompletionWizardPage.AllowFinish = false;
		if (!docGenerating.SetExportData(docGenerating.ExportFile))
		{
			e.Handled = true;
			return;
		}
		docWizardControl.SelectedPage = docCompletionWizardPage;
		exportWorker = new BackgroundProcessingWorker(this);
		try
		{
			exportWorker.SetEventsHandling(delegate
			{
				StaticData.CrashedDatabaseType = ddlDocumentationsUserControl.DatabaseType;
				StaticData.CrashedDBMSVersion = ddlDocumentationsUserControl.DatabaseDBMSVersion;
				new DDLGenerator(ddlExportSettingsUserControl.GetSettings()).ProcessDatabase(ddlDocumentationsUserControl.DatabaseId, ddlChooseObjectsUserControl.SelectedObjects, exportWorker, docGenerating.saveFileDialog.FileName, this);
			}, delegate
			{
				generatingDocProgressBar.Text = ((int)Math.Round(exportWorker.TotalProgress)).ToString();
				creatingPagesProgressBarControl.Text = ((int)Math.Round(exportWorker.CurrentProgress)).ToString();
				exportingLabel.Text = exportWorker.CurrentStage;
				exportingLabelObject.Text = exportWorker.CurrentObject;
			}, delegate
			{
				ActionAfterFinish();
				if (exportWorker.IsCancelled)
				{
					exportingLabel.Text = "Cancelled!";
				}
				else if (exportWorker.HasResult)
				{
					if (!exportWorker.HasError)
					{
						generatingDocProgressBar.Text = "100";
						exportingLabel.MinimumSize = new Size(413, 16);
						exportingLabel.Text = "Generating succeeded";
						exportingLabel.Appearance.Image = Resources.ok_16;
						Paths.OpenSavedFile(docGenerating.saveFileDialog.FileName, this);
					}
					else
					{
						SetFailedExportLabels();
					}
					generatingDocProgressBar.Visible = false;
					creatingPagesProgressBarControl.Visible = false;
				}
				StaticData.ClearDatabaseInfoForCrashes();
				Close();
			});
			canCloseWindow = false;
			exportWorker.Start();
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, this);
			ForceClose();
		}
	}

	private void ProcessExportToDatabase()
	{
		docWizardControl.SelectedPage = docCompletionWizardPage;
		docCompletionWizardPage.AllowFinish = false;
		docCompletionWizardPage.AllowBack = false;
		exportWorker = new BackgroundProcessingWorker(this);
		try
		{
			exportWorker.SetEventsHandling(delegate
			{
				if (chooseModulesUserControl.GetChosenModules().Count != 0)
				{
					StaticData.CrashedDatabaseType = dbConnectUserControl?.DatabaseRow?.Type;
					StaticData.CrashedDBMSVersion = dbConnectUserControl?.DatabaseRow?.DbmsVersion;
					DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type).ExportExtendedPropertiesOrComments(exportWorker, dbConnectUserControl.DatabaseRow.GetConnectionString(withUnicode: true), currentDatabaseIdValue, chooseModulesUserControl.GetChosenModules(), chooseCustomFieldsUserControl, chooseObjectTypesUserControl);
				}
			}, delegate
			{
				generatingDocProgressBar.Text = ((int)Math.Round(exportWorker.TotalProgress)).ToString();
				creatingPagesProgressBarControl.Text = ((int)Math.Round(exportWorker.CurrentProgress)).ToString();
				exportingLabel.Text = exportWorker.CurrentStage;
				exportingLabelObject.Text = exportWorker.CurrentObject;
			}, delegate
			{
				ActionAfterFinish();
				if (exportWorker.IsCancelled)
				{
					exportingLabel.Text = "Cancelled!";
				}
				else if (exportWorker.HasResult)
				{
					if (!exportWorker.HasError)
					{
						generatingDocProgressBar.Text = "100";
						exportingLabel.MinimumSize = new Size(413, 16);
						exportingLabel.Text = "Export succeeded";
						exportingLabel.Appearance.Image = Resources.ok_16;
					}
					else
					{
						SetFailedExportLabels();
					}
					generatingDocProgressBar.Visible = false;
					creatingPagesProgressBarControl.Visible = false;
				}
				StaticData.ClearDatabaseInfoForCrashes();
			});
			canCloseWindow = false;
			exportWorker.Start();
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, this);
			ForceClose();
		}
	}

	private void SetFailedExportLabels()
	{
		generatingDocProgressBar.Text = "0";
		exportingLabel.MinimumSize = new Size(413, 16);
		exportingLabel.Text = "Export failed!";
		exportingLabel.Appearance.Image = Resources.error_16;
		layoutControlItem4.Visibility = LayoutVisibility.Never;
	}

	private void LoadTemplatesList(WizardCommandButtonClickEventArgs e = null)
	{
		switch (docFormatRadioGroup.SelectedIndex)
		{
		case 0:
			canExportBG = Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary);
			LoadTemplates(DocFormatEnum.DocFormat.HTML);
			docFormat = DocFormatEnum.DocFormat.HTML;
			break;
		case 2:
			canExportBG = true;
			LoadTemplates(DocFormatEnum.DocFormat.PDF);
			docFormat = DocFormatEnum.DocFormat.PDF;
			break;
		case 1:
			LoadTemplates(DocFormatEnum.DocFormat.Excel);
			docFormat = DocFormatEnum.DocFormat.Excel;
			break;
		}
	}

	private void LoadTemplates(DocFormatEnum.DocFormat format)
	{
		BindingList<DocTemplateFile> bindingList = new BindingList<DocTemplateFile>();
		try
		{
			List<DocTemplateFile> list = new List<DocTemplateFile>();
			switch (format)
			{
			case DocFormatEnum.DocFormat.PDF:
				try
				{
					IEnumerable<DocTemplateFile> collection = LoadTemplates(Paths.GetUserTemplatesPath(format), format);
					list.AddRange(collection);
				}
				catch (DirectoryNotFoundException ex)
				{
					GeneralMessageBoxesHandling.Show("Unable to load user templates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, this);
				}
				catch (UnauthorizedAccessException ex2)
				{
					GeneralMessageBoxesHandling.Show("Unable to load user templates." + Environment.NewLine + "Access is denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Message, 1, this);
				}
				catch (Exception)
				{
					throw;
				}
				break;
			case DocFormatEnum.DocFormat.HTML:
			{
				TemplatesManager htmlTemplatesManager = TemplatesFactory.MakeHtmlTemplatesManager(this);
				IEnumerable<DocTemplateFile> userAndProgramTemplates = htmlTemplatesManager.ListTemplates().Select(delegate(ITemplate template)
				{
					DocTemplateFile docTemplateFile = new DocTemplateFile();
					docTemplateFile.Description = "<b>" + template.Name + "</b><br><br><br>" + template.Description;
					docTemplateFile.Image = template.Thumbnail;
					docTemplateFile.Type = TemplateTypeEnum.TemplateType.HTML;
					docTemplateFile.CustomObject = template;
					docTemplateFile.Template = new BaseTemplateModel
					{
						Name = template.Name,
						Description = docTemplateFile.Description,
						Type = TemplateTypeEnum.TemplateType.HTML,
						IsCustomizable = htmlTemplatesManager.IsCustomizable(template, typeof(LocalStorage))
					};
					return docTemplateFile;
				});
				SortTemplatesAndIncludedInvalid(list, userAndProgramTemplates);
				break;
			}
			}
			if (format != DocFormatEnum.DocFormat.HTML)
			{
				List<DocTemplateFile> collection2 = LoadTemplates(DocTemplateFile.GetTemplatesPath(format), format, userTemplate: false).ToList();
				list.AddRange(collection2);
			}
			if (format == DocFormatEnum.DocFormat.PDF)
			{
				list.Sort(new PDFTemplatesByIsValidThenByIsUserTemplateThenByName());
			}
			foreach (DocTemplateFile item in list)
			{
				bindingList.Add(item);
			}
			List<DocTemplateFile> list2 = (from t in bindingList
				where (t.Image == null || t.Type == TemplateTypeEnum.TemplateType.Undefined) && t?.Template?.ExceptionValue == null
				select (t)).ToList();
			for (int i = 0; i < list2.Count; i++)
			{
				bindingList.Remove(list2[i]);
			}
			if (format == DocFormatEnum.DocFormat.HTML && bindingList.Count == 0)
			{
				GeneralMessageBoxesHandling.Show("No HTML export template found. Please reinstall Dataedo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
				SetFailedExportLabels();
				generatingDocProgressBar.Visible = false;
				creatingPagesProgressBarControl.Visible = false;
				docCompletionWizardPage.AllowBack = false;
				docCompletionWizardPage.AllowCancel = false;
				docWizardControl.SelectedPage = docCompletionWizardPage;
			}
			templatesGrid.BeginUpdate();
			templatesGrid.DataSource = bindingList;
			templatesGrid.EndUpdate();
			if (templatesGridView.RowCount > 0)
			{
				templatesGridView.BeginUpdate();
				templatesGridView.FocusedRowHandle = 0;
				templatesGridView.EndUpdate();
				docTemplateWizardPage.AllowNext = true;
				SetCustomizationAvailability();
			}
			else
			{
				docTemplateWizardPage.AllowNext = false;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while searching for templates.", this);
		}
	}

	private void SortTemplatesAndIncludedInvalid(List<DocTemplateFile> templatesTemporary, IEnumerable<DocTemplateFile> userAndProgramTemplates)
	{
		List<DocTemplateFile> list = new List<DocTemplateFile>();
		List<DocTemplateFile> list2 = new List<DocTemplateFile>();
		List<DocTemplateFile> list3 = new List<DocTemplateFile>();
		foreach (DocTemplateFile userAndProgramTemplate in userAndProgramTemplates)
		{
			LocalTemplate localTemplate = userAndProgramTemplate.CustomObject as LocalTemplate;
			if (string.IsNullOrEmpty(localTemplate.TransformerName))
			{
				localTemplate.IsValid = false;
				localTemplate.ExceptionWhenTemplateIsNotValid = localTemplate.ExceptionWhenTemplateIsNotValid ?? new Exception("Transformer name is empty or none");
			}
			if (canExportBG && (userAndProgramTemplate.CustomObject as ITemplate)?.Version == null)
			{
				localTemplate.IsValid = false;
				localTemplate.ExceptionWhenTemplateIsNotValid = localTemplate.ExceptionWhenTemplateIsNotValid ?? new Exception("Cant export BG, template version is none");
			}
			if (!localTemplate.IsValid)
			{
				userAndProgramTemplate.Description = "[Directory] : \r\n\r\n<b><color=darkred>Invalid template: " + Path.GetFileName(localTemplate.Storage.Dir) + "</color></b><br><br><b>Double click for more information</b>";
				userAndProgramTemplate.Template.ExceptionValue = localTemplate.ExceptionWhenTemplateIsNotValid;
			}
			if (localTemplate.IsInUserTemplateDirectory && localTemplate.IsValid)
			{
				list.Add(userAndProgramTemplate);
			}
			else if (!localTemplate.IsInUserTemplateDirectory && localTemplate.IsValid)
			{
				list2.Add(userAndProgramTemplate);
			}
			else if (!localTemplate.IsValid)
			{
				list3.Add(userAndProgramTemplate);
			}
			else
			{
				list.Add(userAndProgramTemplate);
			}
		}
		list = list.OrderBy((DocTemplateFile x) => x?.Template?.Name).ToList();
		list2 = list2.OrderBy((DocTemplateFile x) => x?.Template?.Name).ToList();
		list3 = list3.OrderBy(delegate(DocTemplateFile x)
		{
			object obj = x?.Template?.Name;
			if (obj == null)
			{
				if (x == null)
				{
					return null;
				}
				obj = x.FileName;
			}
			return (string)obj;
		}).ToList();
		foreach (DocTemplateFile item in list)
		{
			templatesTemporary.Add(item);
		}
		foreach (DocTemplateFile item2 in list2)
		{
			templatesTemporary.Add(item2);
		}
		foreach (DocTemplateFile item3 in list3)
		{
			templatesTemporary.Add(item3);
		}
	}

	private IEnumerable<DocTemplateFile> LoadTemplates(string path, DocFormatEnum.DocFormat format, bool userTemplate = true)
	{
		if (string.IsNullOrEmpty(path))
		{
			yield break;
		}
		DirectoryInfo dir = new DirectoryInfo(path);
		if (format != DocFormatEnum.DocFormat.HTML)
		{
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				yield return new DocTemplateFile(format, fileInfo.FullName, userTemplate, this);
			}
			DirectoryInfo[] directories = dir.GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				yield return new DocTemplateFile(format, directoryInfo.FullName, userTemplate, this);
			}
		}
		else
		{
			DirectoryInfo[] directories = dir.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				yield return new DocTemplateFile(format, directoryInfo2.FullName, userTemplate, this);
			}
		}
	}

	private void templatesGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		SetCustomizationAvailability();
	}

	private void SetCustomizationAvailability()
	{
		if (templatesGridView.GetFocusedRow() is DocTemplateFile docTemplateFile)
		{
			docTemplateWizardPage.AllowNext = docTemplateFile.Template?.ExceptionValue == null;
			customizeSimpleButton.Enabled = docTemplateFile != null && docTemplateFile.Template.IsCustomizable == true;
		}
		else
		{
			SimpleButton simpleButton = customizeSimpleButton;
			bool enabled = (docTemplateWizardPage.AllowNext = false);
			simpleButton.Enabled = enabled;
		}
	}

	private void SetCustomizationVisibility()
	{
		bool flag = IsCustomizationEnabled();
		customizeLayoutControlItem.Visibility = ((!flag) ? LayoutVisibility.Never : LayoutVisibility.Always);
	}

	private void docWizardControl_CancelClick(object sender, CancelEventArgs e)
	{
		CancelClick();
	}

	private void CancelClick()
	{
		if (docWizardControl.SelectedPage.Equals(docCompletionWizardPage))
		{
			switch (docFormat)
			{
			case DocFormatEnum.DocFormat.PDF:
				docGenerating.Cancel();
				docCompletionWizardPage.AllowCancel = false;
				break;
			case DocFormatEnum.DocFormat.HTML:
			case DocFormatEnum.DocFormat.Excel:
				exportWorker?.Cancel();
				break;
			}
			if (exportToDB || DDLGenerating)
			{
				exportWorker?.Cancel();
			}
		}
	}

	private void docWizardControl_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
		if (e.Page.Equals(docTemplateWizardPage))
		{
			e.NextButton.Button.Select();
		}
		else if (e.Page.Equals(exportDataWizardPage))
		{
			e.NextButton.Text = "Export";
			e.NextButton.Image = Resources.icon_16;
		}
		else if (exportToDB && e.Page.Equals(chooseObjectsWizardPage) && (dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.SqlServer || dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase || dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse) && chooseCustomFieldsUserControl.CustomFieldsSupport.Fields.Count != 0)
		{
			e.NextButton.Text = "Next >";
			e.NextButton.Image = null;
		}
		else if (exportToDB && e.Page.Equals(chooseObjectsWizardPage))
		{
			e.NextButton.Text = "Export";
			e.NextButton.Image = Resources.icon_16;
		}
		else if (exportToDB && e.Page.Equals(chooseCustomFieldsWizardPage))
		{
			e.NextButton.Text = "Export";
			e.NextButton.Image = Resources.icon_16;
		}
		else if (exportToDB && e.Page.Equals(chooseModulesWizardPage))
		{
			chooseModulesUserControl.Button = e.NextButton.Button;
			e.NextButton.Text = "Next >";
			e.NextButton.Image = null;
			e.NextButton.Button.Enabled = chooseModulesUserControl.HasSelectedAnySubItem.GetValueOrDefault();
		}
		else
		{
			e.NextButton.Text = "Next >";
			e.NextButton.Image = null;
		}
	}

	private void folderButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		switch (docFormat)
		{
		case DocFormatEnum.DocFormat.PDF:
		{
			ExportFile exportFile2 = docGenerating.ChooseSavePathData(string.IsNullOrEmpty(filenameTextEdit.Text) ? DB.Database.GetTitle(currentDatabaseIdValue) : filenameTextEdit.Text);
			if (exportFile2 != null)
			{
				folderButtonEdit.Text = exportFile2.Directory;
				filenameTextEdit.Text = exportFile2.FileName;
			}
			else
			{
				docGenerating.ExportFile = new ExportFile(folderButtonEdit.Text, filenameTextEdit.Text, ".pdf");
			}
			break;
		}
		case DocFormatEnum.DocFormat.HTML:
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				folderButtonEdit.Text = folderBrowserDialog1.SelectedPath;
			}
			break;
		case DocFormatEnum.DocFormat.Excel:
		{
			ExportFile exportFile3 = docGenerating.ChooseSavePathData(string.IsNullOrEmpty(filenameTextEdit.Text) ? DB.Database.GetTitle(currentDatabaseIdValue) : filenameTextEdit.Text, excelSaveFileDialog);
			if (exportFile3 != null)
			{
				folderButtonEdit.Text = exportFile3.Directory;
				filenameTextEdit.Text = exportFile3.FileName;
			}
			else
			{
				docGenerating.ExportFile = new ExportFile(folderButtonEdit.Text, filenameTextEdit.Text, ".xlsx");
			}
			break;
		}
		case DocFormatEnum.DocFormat.DDL:
		{
			ExportFile exportFile = docGenerating.ChooseSavePathData(string.IsNullOrEmpty(filenameTextEdit.Text) ? DB.Database.GetTitle(currentDatabaseIdValue) : filenameTextEdit.Text, sqlSaveFileDialog);
			if (exportFile != null)
			{
				folderButtonEdit.Text = exportFile.Directory;
				filenameTextEdit.Text = exportFile.FileName;
			}
			else
			{
				docGenerating.ExportFile = new ExportFile(folderButtonEdit.Text, filenameTextEdit.Text, ".sql");
			}
			break;
		}
		}
	}

	public void SetPathData()
	{
		folderButtonEdit.Text = docGenerating.ExportFile.Directory;
		filenameTextEdit.Text = docGenerating.ExportFile.FileName;
	}

	private void folderButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		docGenerating.ExportFile.Directory = folderButtonEdit.Text;
	}

	private void filenameTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (docGenerating.ExportFile != null)
		{
			docGenerating.ExportFile.FileName = filenameTextEdit.Text;
		}
	}

	private void filenameTextEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (docGenerating.ExportFile != null)
		{
			docGenerating.ExportFile.Correct = false;
		}
	}

	private void formatLookUpEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (docGenerating.ExportFile != null)
		{
			docGenerating.ExportFile.Correct = false;
		}
	}

	private void templatesLayoutView_CustomDrawCardCaption(object sender, LayoutViewCustomDrawCardCaptionEventArgs e)
	{
		e.CardCaption = null;
	}

	private void DocWizardForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = !canCloseWindow;
	}

	private void saveExportCommandLabelControl_Click(object sender, EventArgs e)
	{
		if (!Paths.IsFilePathCorrect(docGenerating.ExportFile.Directory, docGenerating.ExportFile.FileName, this))
		{
			return;
		}
		string empty = string.Empty;
		string empty2 = string.Empty;
		string extension = string.Empty;
		if (docFormat == DocFormatEnum.DocFormat.PDF)
		{
			empty2 = Path.Combine(docGenerating.ExportFile.Directory, docGenerating.ExportFile.FileName);
			extension = docGenerating.ExportFile.Extension;
			empty = "PDF";
		}
		else if (docFormat == DocFormatEnum.DocFormat.HTML)
		{
			empty2 = Path.Combine(folderButtonEdit.Text, filenameTextEdit.Text);
			empty = "HTML";
		}
		else
		{
			if (docFormat != DocFormatEnum.DocFormat.Excel)
			{
				return;
			}
			if (docGenerating.ExportFile.Extension == ".pdf")
			{
				docGenerating.ExportFile.Extension = "." + excelSaveFileDialog.DefaultExt;
			}
			empty2 = Path.Combine(docGenerating.ExportFile.Directory, docGenerating.ExportFile.FileName);
			extension = docGenerating.ExportFile.Extension;
			empty = "Excel";
		}
		DataedoCommandsVersion2 dataedoCommandsVersion = new DataedoCommandsVersion2();
		DatabaseRow databaseRow = new DatabaseRow(DB.Database.GetDataById(currentDatabaseIdValue));
		LocalTemplate localTemplate = ((templatesGridView.GetFocusedRow() as DocTemplateFile).CustomObject as ITemplate) as LocalTemplate;
		string text = (templatesGridView.GetFocusedRow() as DocTemplateFile).FileName;
		if (localTemplate != null)
		{
			text = localTemplate.Storage.Dir;
		}
		string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		string text2 = null;
		ExportVersion2 exportVersion = ExportCommandBuilder.CreateExportCommandObject(templatePath: (!text.StartsWith(directoryName)) ? text : Path.GetFileName(text), repositoryType: StaticData.RepositoryType, loginInfo: LastConnectionInfo.LOGIN_INFO, format: docFormat, documentTitle: titleTextEdit.Text, documentationsModulesData: chooseModulesUserControl.DocumentationsModulesData, objects: chooseObjectTypesUserControl.GetIncludedDbObjects(), otherFieldsSupport: chooseCustomFieldsUserControl.OtherFieldsSupport, customFieldsSupport: chooseCustomFieldsUserControl.CustomFieldsSupport, outputPathBase: empty2, extension: extension);
		dataedoCommandsVersion.Commands.Add(exportVersion);
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Dataedo commands file (*." + CommandBuilderBase.CommandsFileExtension + ")|*." + CommandBuilderBase.CommandsFileExtension;
		dataedoCommandsVersion.Settings.LogFile.Path = "Dataedo " + Paths.RemoveInvalidFilePathCharacters("Export " + databaseRow.Title + " documentation from " + exportVersion.RepositoryConnection.GetDatabaseFull() + " ", "_", removeInvalidFileNameCharsOnly: true) + "{DateTime:yyyy-MM-dd}.log";
		dataedoCommandsVersion.Settings.LogFile.PathAlternative = new XmlCommentObject("<Path>{MyDocuments}\\\\Dataedo\\\\Logs\\\\" + dataedoCommandsVersion.Settings.LogFile.Path + "</Path>");
		saveFileDialog.FileName += Paths.RemoveInvalidFilePathCharacters("Export " + databaseRow.Title + " documentation to " + empty + "." + CommandBuilderBase.CommandsFileExtension, "_", removeInvalidFileNameCharsOnly: true);
		if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
		{
			dataedoCommandsVersion.SaveCommandsXml(saveFileDialog.FileName, FileMode.Create);
		}
	}

	private void templatesGrid_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		int[] selectedRows = templatesGridView.GetSelectedRows();
		if (selectedRows.Length != 1)
		{
			return;
		}
		Exception ex = (templatesGridView.GetRow(selectedRows[0]) as DocTemplateFile)?.Template?.ExceptionValue;
		if (ex != null)
		{
			string text = (string.IsNullOrEmpty(ex?.InnerException?.Message) ? (ex.Message ?? "") : (ex.Message + Environment.NewLine + ex?.InnerException?.Message));
			if (ex != null)
			{
				GeneralMessageBoxesHandling.Show(text + Environment.NewLine + Environment.NewLine + "Visit <href=" + Links.InvalidPdfTemplateSupportUrl + ">" + Links.DataedoBaseUrl + "</href> for more information.", "Invalid template", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.ToString(), 1, this);
			}
		}
	}

	private void radioGroupBuyProLabelControl_HyperlinkClick(object sender, DevExpress.Utils.HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			Links.OpenCTALink(e.Link, this, afterLogin: true, null, this);
			SetFunctionality();
		}
	}

	private bool IsCustomizationEnabled()
	{
		if (docFormat != DocFormatEnum.DocFormat.PDF)
		{
			return docFormat == DocFormatEnum.DocFormat.HTML;
		}
		return true;
	}

	private void customizeSimpleButton_Click(object sender, EventArgs e)
	{
		CustomizeSelectedTemplate();
	}

	private void CustomizeSelectedTemplate()
	{
		try
		{
			if (!(templatesGridView.GetFocusedRow() is DocTemplateFile docTemplateFile))
			{
				GeneralMessageBoxesHandling.Show("Unable to customize this template", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
				return;
			}
			BaseTemplateCustomizer baseTemplateCustomizer = null;
			if (docTemplateFile.Type == TemplateTypeEnum.TemplateType.PDF)
			{
				baseTemplateCustomizer = new PdfTemplateCustomizer(docTemplateFile, this);
				string text = null;
				try
				{
					text = baseTemplateCustomizer?.Customize(this);
					if (string.IsNullOrEmpty(text))
					{
						GeneralMessageBoxesHandling.Show("Unable to customize this template", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
						return;
					}
					LoadTemplatesList();
					FocusLastNameTemplate(text);
					SetFunctionality();
					return;
				}
				catch (UnauthorizedAccessException ex)
				{
					GeneralMessageBoxesHandling.Show("Unable to customize template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, this);
					return;
				}
				catch (Exception)
				{
					throw;
				}
			}
			if (docTemplateFile.Type == TemplateTypeEnum.TemplateType.HTML)
			{
				string userTemplatesPath = Paths.GetUserTemplatesPath(DocFormatEnum.DocFormat.HTML);
				string nextName = Paths.GetNextName(userTemplatesPath, "CustomTemplate", null, forFile: false);
				LocalStorage destination = new LocalStorage(Path.Combine(userTemplatesPath, nextName));
				ITemplate template = docTemplateFile.CustomObject as ITemplate;
				string newTemplateName = TemplatesFactory.MakeHtmlTemplatesManager(this).Customize(template, destination, new VueCustomData
				{
					Name = nextName
				}, this);
				LoadTemplatesList();
				FocusLastNameTemplate(newTemplateName);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, this);
		}
	}

	private void FocusLastNameTemplate(string newTemplateName)
	{
		if (string.IsNullOrEmpty(newTemplateName))
		{
			return;
		}
		BindingList<DocTemplateFile> bindingList = templatesGrid.DataSource as BindingList<DocTemplateFile>;
		for (int i = 0; i < bindingList.Count; i++)
		{
			if (bindingList[i]?.Template.Name == newTemplateName)
			{
				templatesGridView.BeginUpdate();
				templatesGridView.FocusedRowHandle = i;
				templatesGridView.EndUpdate();
			}
		}
	}

	private void radioGroupLearnMoreLabelControl_HyperlinkClick(object sender, DevExpress.Utils.HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			new BusinessGlossaryExportForm().ShowDialog();
			SetBusinessGlossaryExportFunctionality();
		}
	}

	private void copyScriptHyperLinkEdit_Click(object sender, EventArgs e)
	{
		ClipboardSupport.SetText(scriptRichEditUserControl.Text, this);
	}

	private void customizeBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		CustomizeSelectedTemplate();
	}

	private void openFolderBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			Paths.OpenExplorerAndSelectFolder((templatesGridView.GetFocusedRow() as DocTemplateFile).FileName);
		}
		catch (Exception)
		{
			GeneralMessageBoxesHandling.Show("Unable to open template folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
		}
	}

	private void templatesPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (!IsCustomizationEnabled())
		{
			e.Cancel = true;
			return;
		}
		DocTemplateFile docTemplateFile = templatesGridView.GetFocusedRow() as DocTemplateFile;
		openFolderBarButtonItem.Visibility = ((docTemplateFile == null || !docTemplateFile.IsUserTemplate) ? BarItemVisibility.Never : BarItemVisibility.Always);
		openFolderBarButtonItem.Visibility = ((docTemplateFile.Type != TemplateTypeEnum.TemplateType.PDF || docTemplateFile == null || !docTemplateFile.IsUserTemplate) ? BarItemVisibility.Never : BarItemVisibility.Always);
		if (!docTemplateFile.IsCustomizable)
		{
			e.Cancel = true;
		}
	}

	private void templatesGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		templatesPopupMenu.ShowPopupMenu(sender, e);
	}

	private void ExtendedPropHyperlinkLabelControl_HyperlinkClick(object sender, DevExpress.Utils.HyperlinkClickEventArgs e)
	{
		Links.OpenLink(Links.ExportDescriptions);
	}

	private void copyDescriptionsHyperlinkLabelControl_HyperlinkClick(object sender, DevExpress.Utils.HyperlinkClickEventArgs e)
	{
		Links.OpenLink(Links.CopyDescriptions);
	}

	private void docWizardControl_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
	{
		if (e.Page.Equals(exportDataWizardPage))
		{
			LayoutVisibility layoutVisibility3 = (titleLayoutControlItem.Visibility = (saveExportCommandLayoutControlItem.Visibility = (DDLGenerating ? LayoutVisibility.Never : LayoutVisibility.Always)));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Documentation.DocWizardForm));
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		this.docCompletionWizardPage = new DevExpress.XtraWizard.CompletionWizardPage();
		this.layoutControl4 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.generatingDocLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.copyScriptHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.scriptRichEditUserControl = new Dataedo.App.UserControls.RichEditUserControl();
		this.marqueeProgressBarControl = new DevExpress.XtraEditors.MarqueeProgressBarControl();
		this.creatingPagesProgressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.generatingDocProgressBar = new DevExpress.XtraEditors.ProgressBarControl();
		this.exportingLabelObject = new DevExpress.XtraEditors.LabelControl();
		this.exportingLabel = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.exportingDetailsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.marqueeBarLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.scriptLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.docTemplateWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.layoutControl2 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.customizeSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.templatesGrid = new DevExpress.XtraGrid.GridControl();
		this.templatesGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.layoutViewColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.imageRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.layoutViewColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionRepositoryItemMemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.customizeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.customizeEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.tamplatesBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.customizeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.openFolderBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.docWizardControl = new DevExpress.XtraWizard.WizardControl();
		this.exportDataWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.saveExportCommandLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.filenameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.folderButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.titleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.directoryPathLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.fileNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveExportCommandLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.titleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.formatWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.webCatalogInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.betaLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.copyDescriptionsHyperlinkLabelControl = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.ExtendedPropHyperlinkLabelControl = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.legacyTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.infoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.radioGroupBuyProLabelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.radioGroupBuyProLabelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.radioGroupBuyProLabelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.docFormatRadioGroup = new DevExpress.XtraEditors.RadioGroup();
		this.dbConnectWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.layoutControl3 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.scriptsOnlyCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.dbConnectUserControl = new Dataedo.App.UserControls.ConnectorsControls.DbConnectUserControlNew();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.chooseModulesWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.chooseModulesUserControl = new Dataedo.App.UserControls.ChooseModulesUserControl();
		this.chooseObjectsWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.chooseObjectTypesUserControl = new Dataedo.App.UserControls.ChooseObjectTypesUserControl();
		this.chooseCustomFieldsWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.chooseCustomFieldsUserControl = new Dataedo.App.UserControls.ChooseCustomFieldsUserControl();
		this.chooseDocumentationsWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.documentationsUserControl = new Dataedo.App.UserControls.DocumentationsUserControl();
		this.chooseRepositoryDocumentationsWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.repositoriesUserControl = new Dataedo.App.UserControls.RepositoriesUserControl();
		this.betaFeatureInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.DDLChooseDocumentationWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.ddlDocumentationsUserControl = new Dataedo.App.UserControls.DDLDocumentationsUserControl();
		this.DDLScriptTypeWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.ddlChooseObjectsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.ddlScriptTypePanelControl = new DevExpress.XtraEditors.PanelControl();
		this.andLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.alterGrayLabelControl = new Dataedo.App.UserControls.GrayLabelControl();
		this.create2GrayLabelControl = new Dataedo.App.UserControls.GrayLabelControl();
		this.createGrayLabelControl = new Dataedo.App.UserControls.GrayLabelControl();
		this.ddlScriptTypeRadioGroup = new DevExpress.XtraEditors.RadioGroup();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.ddlScriptTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.DDLExportSettingsWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.ddlExportSettingsUserControl = new Dataedo.App.UserControls.DDLExportSettingsUserControl();
		this.DDLChooseObjectsWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.ddlChooseObjectsUserControl = new Dataedo.App.UserControls.DDLGenerating.DDLChooseObjectsUserControl();
		this.progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
		this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
		this.excelSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.templatesPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.sqlSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.docCompletionWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).BeginInit();
		this.layoutControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.generatingDocLayoutControl).BeginInit();
		this.generatingDocLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.copyScriptHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.creatingPagesProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.generatingDocProgressBar.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.exportingDetailsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeBarLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		this.docTemplateWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		this.layoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.templatesGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.templatesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionRepositoryItemMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customizeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customizeEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tamplatesBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docWizardControl).BeginInit();
		this.docWizardControl.SuspendLayout();
		this.exportDataWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.filenameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.folderButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.directoryPathLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveExportCommandLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).BeginInit();
		this.formatWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.docFormatRadioGroup.Properties).BeginInit();
		this.dbConnectWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).BeginInit();
		this.layoutControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.scriptsOnlyCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		this.chooseModulesWizardPage.SuspendLayout();
		this.chooseObjectsWizardPage.SuspendLayout();
		this.chooseCustomFieldsWizardPage.SuspendLayout();
		this.chooseDocumentationsWizardPage.SuspendLayout();
		this.chooseRepositoryDocumentationsWizardPage.SuspendLayout();
		this.DDLChooseDocumentationWizardPage.SuspendLayout();
		this.DDLScriptTypeWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ddlChooseObjectsLayoutControl).BeginInit();
		this.ddlChooseObjectsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ddlScriptTypePanelControl).BeginInit();
		this.ddlScriptTypePanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ddlScriptTypeRadioGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ddlScriptTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		this.DDLExportSettingsWizardPage.SuspendLayout();
		this.DDLChooseObjectsWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressBarControl1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.templatesPopupMenu).BeginInit();
		base.SuspendLayout();
		this.docCompletionWizardPage.Controls.Add(this.layoutControl4);
		this.docCompletionWizardPage.FinishText = "";
		this.docCompletionWizardPage.Name = "docCompletionWizardPage";
		this.docCompletionWizardPage.ProceedText = "";
		this.docCompletionWizardPage.Size = new System.Drawing.Size(616, 472);
		this.docCompletionWizardPage.Text = "Generating";
		this.layoutControl4.AllowCustomization = false;
		this.layoutControl4.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl4.Controls.Add(this.generatingDocLayoutControl);
		this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl4.Location = new System.Drawing.Point(0, 0);
		this.layoutControl4.Name = "layoutControl4";
		this.layoutControl4.Root = this.layoutControlGroup2;
		this.layoutControl4.Size = new System.Drawing.Size(616, 472);
		this.layoutControl4.TabIndex = 0;
		this.layoutControl4.Text = "layoutControl4";
		this.generatingDocLayoutControl.AllowCustomization = false;
		this.generatingDocLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.generatingDocLayoutControl.Controls.Add(this.copyScriptHyperLinkEdit);
		this.generatingDocLayoutControl.Controls.Add(this.scriptRichEditUserControl);
		this.generatingDocLayoutControl.Controls.Add(this.marqueeProgressBarControl);
		this.generatingDocLayoutControl.Controls.Add(this.creatingPagesProgressBarControl);
		this.generatingDocLayoutControl.Controls.Add(this.generatingDocProgressBar);
		this.generatingDocLayoutControl.Controls.Add(this.exportingLabelObject);
		this.generatingDocLayoutControl.Controls.Add(this.exportingLabel);
		this.generatingDocLayoutControl.Location = new System.Drawing.Point(12, 12);
		this.generatingDocLayoutControl.Name = "generatingDocLayoutControl";
		this.generatingDocLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1490, 340, 572, 558);
		this.generatingDocLayoutControl.OptionsView.ShareLookAndFeelWithChildren = false;
		this.generatingDocLayoutControl.Root = this.layoutControlGroup5;
		this.generatingDocLayoutControl.Size = new System.Drawing.Size(592, 448);
		this.generatingDocLayoutControl.TabIndex = 15;
		this.generatingDocLayoutControl.Text = "layoutControl1";
		this.copyScriptHyperLinkEdit.EditValue = "Copy all";
		this.copyScriptHyperLinkEdit.Location = new System.Drawing.Point(356, 51);
		this.copyScriptHyperLinkEdit.Name = "copyScriptHyperLinkEdit";
		this.copyScriptHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.copyScriptHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.copyScriptHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.copyScriptHyperLinkEdit.Size = new System.Drawing.Size(50, 18);
		this.copyScriptHyperLinkEdit.TabIndex = 13;
		this.copyScriptHyperLinkEdit.Click += new System.EventHandler(copyScriptHyperLinkEdit_Click);
		this.scriptRichEditUserControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.scriptRichEditUserControl.Appearance.Text.Font = new System.Drawing.Font("Courier New", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.scriptRichEditUserControl.Appearance.Text.Options.UseFont = true;
		this.scriptRichEditUserControl.IsHighlighted = false;
		this.scriptRichEditUserControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.scriptRichEditUserControl.Location = new System.Drawing.Point(2, 174);
		this.scriptRichEditUserControl.Name = "scriptRichEditUserControl";
		this.scriptRichEditUserControl.OccurrencesCount = 0;
		this.scriptRichEditUserControl.Options.Behavior.UseThemeFonts = false;
		this.scriptRichEditUserControl.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
		this.scriptRichEditUserControl.OriginalHtmlText = null;
		this.scriptRichEditUserControl.ReadOnly = true;
		this.scriptRichEditUserControl.Size = new System.Drawing.Size(588, 272);
		this.scriptRichEditUserControl.TabIndex = 12;
		this.scriptRichEditUserControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
		this.marqueeProgressBarControl.EditValue = 0;
		this.marqueeProgressBarControl.Location = new System.Drawing.Point(2, 154);
		this.marqueeProgressBarControl.Name = "marqueeProgressBarControl";
		this.marqueeProgressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.marqueeProgressBarControl.Properties.MarqueeAnimationSpeed = 120;
		this.marqueeProgressBarControl.Properties.Stopped = true;
		this.marqueeProgressBarControl.Size = new System.Drawing.Size(588, 16);
		this.marqueeProgressBarControl.TabIndex = 11;
		this.creatingPagesProgressBarControl.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.creatingPagesProgressBarControl.Location = new System.Drawing.Point(2, 122);
		this.creatingPagesProgressBarControl.Name = "creatingPagesProgressBarControl";
		this.creatingPagesProgressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.creatingPagesProgressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.creatingPagesProgressBarControl.Properties.ShowTitle = true;
		this.creatingPagesProgressBarControl.Properties.Step = 1;
		this.creatingPagesProgressBarControl.Size = new System.Drawing.Size(588, 18);
		this.creatingPagesProgressBarControl.TabIndex = 10;
		this.generatingDocProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.generatingDocProgressBar.EditValue = 50;
		this.generatingDocProgressBar.Location = new System.Drawing.Point(2, 90);
		this.generatingDocProgressBar.Name = "generatingDocProgressBar";
		this.generatingDocProgressBar.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.generatingDocProgressBar.Properties.ShowTitle = true;
		this.generatingDocProgressBar.Properties.Step = 1;
		this.generatingDocProgressBar.Size = new System.Drawing.Size(588, 18);
		this.generatingDocProgressBar.TabIndex = 9;
		this.exportingLabelObject.AllowHtmlString = true;
		this.exportingLabelObject.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.exportingLabelObject.Location = new System.Drawing.Point(2, 51);
		this.exportingLabelObject.Name = "exportingLabelObject";
		this.exportingLabelObject.Size = new System.Drawing.Size(350, 20);
		this.exportingLabelObject.TabIndex = 5;
		this.exportingLabel.AllowHtmlString = true;
		this.exportingLabel.Appearance.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.exportingLabel.Appearance.Options.UseImageAlign = true;
		this.exportingLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.exportingLabel.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.exportingLabel.Location = new System.Drawing.Point(2, 2);
		this.exportingLabel.Name = "exportingLabel";
		this.exportingLabel.Size = new System.Drawing.Size(588, 45);
		this.exportingLabel.TabIndex = 5;
		this.layoutControlGroup5.CustomizationFormText = "Root";
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.layoutControlItem5, this.layoutControlItem4, this.emptySpaceItem1, this.emptySpaceItem2, this.exportingDetailsLayoutControlItem, this.emptySpaceItem3, this.layoutControlItem1, this.marqueeBarLayoutControlItem, this.scriptLayoutControlItem, this.layoutControlItem9 });
		this.layoutControlGroup5.Name = "Root";
		this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup5.Size = new System.Drawing.Size(592, 448);
		this.layoutControlGroup5.TextVisible = false;
		this.layoutControlItem5.Control = this.exportingLabel;
		this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 49);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(14, 49);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(592, 49);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem4.Control = this.generatingDocProgressBar;
		this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 88);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 22);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(54, 22);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(592, 22);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 73);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 15);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 15);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(592, 15);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 110);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(0, 10);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 10);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(592, 10);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.exportingDetailsLayoutControlItem.Control = this.creatingPagesProgressBarControl;
		this.exportingDetailsLayoutControlItem.CustomizationFormText = "exportingDetailsLayoutControlItem";
		this.exportingDetailsLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.exportingDetailsLayoutControlItem.MaxSize = new System.Drawing.Size(0, 22);
		this.exportingDetailsLayoutControlItem.MinSize = new System.Drawing.Size(54, 22);
		this.exportingDetailsLayoutControlItem.Name = "exportingDetailsLayoutControlItem";
		this.exportingDetailsLayoutControlItem.Size = new System.Drawing.Size(592, 22);
		this.exportingDetailsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.exportingDetailsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.exportingDetailsLayoutControlItem.TextVisible = false;
		this.exportingDetailsLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 142);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(0, 10);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 10);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(592, 10);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.Control = this.exportingLabelObject;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 49);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(354, 24);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(354, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(354, 24);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.marqueeBarLayoutControlItem.Control = this.marqueeProgressBarControl;
		this.marqueeBarLayoutControlItem.Location = new System.Drawing.Point(0, 152);
		this.marqueeBarLayoutControlItem.Name = "marqueeBarLayoutControlItem";
		this.marqueeBarLayoutControlItem.Size = new System.Drawing.Size(592, 20);
		this.marqueeBarLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.marqueeBarLayoutControlItem.TextVisible = false;
		this.marqueeBarLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.scriptLayoutControlItem.Control = this.scriptRichEditUserControl;
		this.scriptLayoutControlItem.Location = new System.Drawing.Point(0, 172);
		this.scriptLayoutControlItem.Name = "layoutControlItem9";
		this.scriptLayoutControlItem.Size = new System.Drawing.Size(592, 276);
		this.scriptLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.scriptLayoutControlItem.TextVisible = false;
		this.scriptLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem9.Control = this.copyScriptHyperLinkEdit;
		this.layoutControlItem9.Location = new System.Drawing.Point(354, 49);
		this.layoutControlItem9.MaxSize = new System.Drawing.Size(54, 24);
		this.layoutControlItem9.MinSize = new System.Drawing.Size(54, 24);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(238, 24);
		this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem6 });
		this.layoutControlGroup2.Name = "layoutControlGroup2";
		this.layoutControlGroup2.Size = new System.Drawing.Size(616, 472);
		this.layoutControlGroup2.TextVisible = false;
		this.layoutControlItem6.Control = this.generatingDocLayoutControl;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(596, 452);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.docTemplateWizardPage.Controls.Add(this.layoutControl2);
		this.docTemplateWizardPage.DescriptionText = "Please choose documentation template.";
		this.docTemplateWizardPage.Name = "docTemplateWizardPage";
		this.docTemplateWizardPage.Size = new System.Drawing.Size(617, 461);
		this.docTemplateWizardPage.Text = "Choose template";
		this.layoutControl2.AllowCustomization = false;
		this.layoutControl2.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl2.Controls.Add(this.customizeSimpleButton);
		this.layoutControl2.Controls.Add(this.templatesGrid);
		this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl2.Location = new System.Drawing.Point(0, 0);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2424, 281, 250, 350);
		this.layoutControl2.Root = this.layoutControlGroup4;
		this.layoutControl2.Size = new System.Drawing.Size(617, 461);
		this.layoutControl2.TabIndex = 1;
		this.layoutControl2.Text = "layoutControl2";
		this.customizeSimpleButton.Location = new System.Drawing.Point(510, 437);
		this.customizeSimpleButton.Name = "customizeSimpleButton";
		this.customizeSimpleButton.Size = new System.Drawing.Size(105, 22);
		this.customizeSimpleButton.StyleController = this.layoutControl2;
		this.customizeSimpleButton.TabIndex = 4;
		this.customizeSimpleButton.Text = "Customize template";
		this.customizeSimpleButton.Click += new System.EventHandler(customizeSimpleButton_Click);
		this.templatesGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.templatesGrid.Location = new System.Drawing.Point(2, 2);
		this.templatesGrid.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.templatesGrid.MainView = this.templatesGridView;
		this.templatesGrid.Name = "templatesGrid";
		this.templatesGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.imageRepositoryItemPictureEdit, this.descriptionRepositoryItemMemoEdit });
		this.templatesGrid.Size = new System.Drawing.Size(613, 431);
		this.templatesGrid.TabIndex = 0;
		this.templatesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.templatesGridView });
		this.templatesGrid.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(templatesGrid_MouseDoubleClick);
		this.templatesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.layoutViewColumn2, this.layoutViewColumn4 });
		this.templatesGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.templatesGridView.GridControl = this.templatesGrid;
		this.templatesGridView.Name = "templatesGridView";
		this.templatesGridView.OptionsBehavior.Editable = false;
		this.templatesGridView.OptionsFilter.AllowColumnMRUFilterList = false;
		this.templatesGridView.OptionsFilter.AllowFilterEditor = false;
		this.templatesGridView.OptionsFind.AllowFindPanel = false;
		this.templatesGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.templatesGridView.OptionsView.RowAutoHeight = true;
		this.templatesGridView.OptionsView.ShowColumnHeaders = false;
		this.templatesGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.templatesGridView.OptionsView.ShowGroupPanel = false;
		this.templatesGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.templatesGridView.OptionsView.ShowIndicator = false;
		this.templatesGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.templatesGridView.RowHighlightingIsEnabled = true;
		this.templatesGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(templatesGridView_PopupMenuShowing);
		this.templatesGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(templatesGridView_FocusedRowChanged);
		this.layoutViewColumn2.ColumnEdit = this.imageRepositoryItemPictureEdit;
		this.layoutViewColumn2.FieldName = "Image";
		this.layoutViewColumn2.Name = "layoutViewColumn2";
		this.layoutViewColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.layoutViewColumn2.OptionsColumn.FixedWidth = true;
		this.layoutViewColumn2.OptionsFilter.AllowAutoFilter = false;
		this.layoutViewColumn2.OptionsFilter.AllowFilter = false;
		this.layoutViewColumn2.Visible = true;
		this.layoutViewColumn2.VisibleIndex = 0;
		this.layoutViewColumn2.Width = 110;
		this.imageRepositoryItemPictureEdit.AllowFocused = false;
		this.imageRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.imageRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.imageRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.imageRepositoryItemPictureEdit.Name = "imageRepositoryItemPictureEdit";
		this.imageRepositoryItemPictureEdit.ShowMenu = false;
		this.imageRepositoryItemPictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
		this.layoutViewColumn4.Caption = "Description";
		this.layoutViewColumn4.ColumnEdit = this.descriptionRepositoryItemMemoEdit;
		this.layoutViewColumn4.FieldName = "Description";
		this.layoutViewColumn4.Name = "layoutViewColumn4";
		this.layoutViewColumn4.Visible = true;
		this.layoutViewColumn4.VisibleIndex = 1;
		this.layoutViewColumn4.Width = 320;
		this.descriptionRepositoryItemMemoEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionRepositoryItemMemoEdit.Name = "descriptionRepositoryItemMemoEdit";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem8, this.customizeLayoutControlItem, this.customizeEmptySpaceItem });
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(617, 461);
		this.layoutControlGroup4.TextVisible = false;
		this.layoutControlItem8.Control = this.templatesGrid;
		this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(617, 435);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.customizeLayoutControlItem.Control = this.customizeSimpleButton;
		this.customizeLayoutControlItem.Location = new System.Drawing.Point(508, 435);
		this.customizeLayoutControlItem.MaxSize = new System.Drawing.Size(109, 26);
		this.customizeLayoutControlItem.MinSize = new System.Drawing.Size(109, 26);
		this.customizeLayoutControlItem.Name = "customizeLayoutControlItem";
		this.customizeLayoutControlItem.Size = new System.Drawing.Size(109, 26);
		this.customizeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.customizeLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.customizeLayoutControlItem.TextVisible = false;
		this.customizeEmptySpaceItem.AllowHotTrack = false;
		this.customizeEmptySpaceItem.Location = new System.Drawing.Point(0, 435);
		this.customizeEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.customizeEmptySpaceItem.MinSize = new System.Drawing.Size(10, 26);
		this.customizeEmptySpaceItem.Name = "customizeEmptySpaceItem";
		this.customizeEmptySpaceItem.Size = new System.Drawing.Size(508, 26);
		this.customizeEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.customizeEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.tamplatesBarManager.DockControls.Add(this.barDockControlTop);
		this.tamplatesBarManager.DockControls.Add(this.barDockControlBottom);
		this.tamplatesBarManager.DockControls.Add(this.barDockControlLeft);
		this.tamplatesBarManager.DockControls.Add(this.barDockControlRight);
		this.tamplatesBarManager.Form = this;
		this.tamplatesBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[2] { this.customizeBarButtonItem, this.openFolderBarButtonItem });
		this.tamplatesBarManager.MaxItemId = 2;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.tamplatesBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(649, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 604);
		this.barDockControlBottom.Manager = this.tamplatesBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(649, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.tamplatesBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 604);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(649, 0);
		this.barDockControlRight.Manager = this.tamplatesBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 604);
		this.customizeBarButtonItem.Caption = "Customize";
		this.customizeBarButtonItem.Id = 0;
		this.customizeBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.customizeBarButtonItem.Name = "customizeBarButtonItem";
		this.customizeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(customizeBarButtonItem_ItemClick);
		this.openFolderBarButtonItem.Caption = "Open folder";
		this.openFolderBarButtonItem.Id = 1;
		this.openFolderBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.folder_open_16;
		this.openFolderBarButtonItem.Name = "openFolderBarButtonItem";
		this.openFolderBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(openFolderBarButtonItem_ItemClick);
		this.docWizardControl.Controls.Add(this.docTemplateWizardPage);
		this.docWizardControl.Controls.Add(this.docCompletionWizardPage);
		this.docWizardControl.Controls.Add(this.exportDataWizardPage);
		this.docWizardControl.Controls.Add(this.formatWizardPage);
		this.docWizardControl.Controls.Add(this.dbConnectWizardPage);
		this.docWizardControl.Controls.Add(this.chooseModulesWizardPage);
		this.docWizardControl.Controls.Add(this.chooseObjectsWizardPage);
		this.docWizardControl.Controls.Add(this.chooseCustomFieldsWizardPage);
		this.docWizardControl.Controls.Add(this.chooseDocumentationsWizardPage);
		this.docWizardControl.Controls.Add(this.chooseRepositoryDocumentationsWizardPage);
		this.docWizardControl.Controls.Add(this.DDLChooseDocumentationWizardPage);
		this.docWizardControl.Controls.Add(this.DDLScriptTypeWizardPage);
		this.docWizardControl.Controls.Add(this.DDLExportSettingsWizardPage);
		this.docWizardControl.Controls.Add(this.DDLChooseObjectsWizardPage);
		this.docWizardControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.docWizardControl.Image = Dataedo.App.Properties.Resources.empty_image;
		this.docWizardControl.ImageLayout = System.Windows.Forms.ImageLayout.None;
		this.docWizardControl.ImageWidth = 1;
		this.docWizardControl.Name = "docWizardControl";
		this.docWizardControl.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[14]
		{
			this.formatWizardPage, this.docTemplateWizardPage, this.dbConnectWizardPage, this.chooseRepositoryDocumentationsWizardPage, this.chooseDocumentationsWizardPage, this.DDLChooseDocumentationWizardPage, this.DDLScriptTypeWizardPage, this.DDLChooseObjectsWizardPage, this.DDLExportSettingsWizardPage, this.chooseModulesWizardPage,
			this.chooseObjectsWizardPage, this.chooseCustomFieldsWizardPage, this.exportDataWizardPage, this.docCompletionWizardPage
		});
		this.docWizardControl.Size = new System.Drawing.Size(649, 604);
		this.docWizardControl.Text = "Generating documentation";
		this.docWizardControl.SelectedPageChanging += new DevExpress.XtraWizard.WizardPageChangingEventHandler(docWizardControl_SelectedPageChanging);
		this.docWizardControl.CancelClick += new System.ComponentModel.CancelEventHandler(docWizardControl_CancelClick);
		this.docWizardControl.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(docWizardControl_NextClick);
		this.docWizardControl.PrevClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(docWizardControl_PrevClick);
		this.docWizardControl.CustomizeCommandButtons += new DevExpress.XtraWizard.WizardCustomizeCommandButtonsEventHandler(docWizardControl_CustomizeCommandButtons);
		this.exportDataWizardPage.Controls.Add(this.layoutControl1);
		this.exportDataWizardPage.DescriptionText = "Please choose folder and file name.";
		this.exportDataWizardPage.Name = "exportDataWizardPage";
		this.exportDataWizardPage.Size = new System.Drawing.Size(617, 461);
		this.exportDataWizardPage.Text = "Choose file";
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.saveExportCommandLabelControl);
		this.layoutControl1.Controls.Add(this.filenameTextEdit);
		this.layoutControl1.Controls.Add(this.folderButtonEdit);
		this.layoutControl1.Controls.Add(this.titleTextEdit);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(744, 202, 670, 560);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(617, 461);
		this.layoutControl1.TabIndex = 1;
		this.layoutControl1.Text = "layoutControl1";
		this.saveExportCommandLabelControl.AllowHtmlString = true;
		this.saveExportCommandLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.saveExportCommandLabelControl.Location = new System.Drawing.Point(499, 436);
		this.saveExportCommandLabelControl.Name = "saveExportCommandLabelControl";
		this.saveExportCommandLabelControl.Size = new System.Drawing.Size(106, 13);
		this.saveExportCommandLabelControl.StyleController = this.layoutControl1;
		this.saveExportCommandLabelControl.TabIndex = 13;
		this.saveExportCommandLabelControl.Text = "<href>Save export command</href>";
		this.saveExportCommandLabelControl.Click += new System.EventHandler(saveExportCommandLabelControl_Click);
		this.filenameTextEdit.Location = new System.Drawing.Point(77, 36);
		this.filenameTextEdit.Name = "filenameTextEdit";
		this.filenameTextEdit.Size = new System.Drawing.Size(528, 20);
		this.filenameTextEdit.StyleController = this.layoutControl1;
		this.filenameTextEdit.TabIndex = 4;
		this.filenameTextEdit.EditValueChanged += new System.EventHandler(filenameTextEdit_EditValueChanged);
		this.filenameTextEdit.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(filenameTextEdit_PreviewKeyDown);
		this.folderButtonEdit.Location = new System.Drawing.Point(77, 12);
		this.folderButtonEdit.Name = "folderButtonEdit";
		this.folderButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.folderButtonEdit.Size = new System.Drawing.Size(528, 20);
		this.folderButtonEdit.StyleController = this.layoutControl1;
		this.folderButtonEdit.TabIndex = 0;
		this.folderButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(folderButtonEdit_ButtonClick);
		this.folderButtonEdit.EditValueChanged += new System.EventHandler(folderButtonEdit_EditValueChanged);
		this.titleTextEdit.Location = new System.Drawing.Point(77, 60);
		this.titleTextEdit.Name = "titleTextEdit";
		this.titleTextEdit.Size = new System.Drawing.Size(528, 20);
		this.titleTextEdit.StyleController = this.layoutControl1;
		this.titleTextEdit.TabIndex = 4;
		this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.directoryPathLayoutControlItem, this.fileNameLayoutControlItem, this.saveExportCommandLayoutControlItem, this.emptySpaceItem5, this.emptySpaceItem6, this.titleLayoutControlItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(617, 461);
		this.layoutControlGroup1.TextVisible = false;
		this.directoryPathLayoutControlItem.Control = this.folderButtonEdit;
		this.directoryPathLayoutControlItem.CustomizationFormText = "Folder:";
		this.directoryPathLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.directoryPathLayoutControlItem.Name = "directoryPathLayoutControlItem";
		this.directoryPathLayoutControlItem.Size = new System.Drawing.Size(597, 24);
		this.directoryPathLayoutControlItem.Text = "Folder:";
		this.directoryPathLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.directoryPathLayoutControlItem.TextSize = new System.Drawing.Size(60, 13);
		this.directoryPathLayoutControlItem.TextToControlDistance = 5;
		this.fileNameLayoutControlItem.Control = this.filenameTextEdit;
		this.fileNameLayoutControlItem.CustomizationFormText = "File name";
		this.fileNameLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.fileNameLayoutControlItem.Name = "fileNameLayoutControlItem";
		this.fileNameLayoutControlItem.Size = new System.Drawing.Size(597, 24);
		this.fileNameLayoutControlItem.Text = "Filename:";
		this.fileNameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.fileNameLayoutControlItem.TextSize = new System.Drawing.Size(60, 13);
		this.fileNameLayoutControlItem.TextToControlDistance = 5;
		this.saveExportCommandLayoutControlItem.Control = this.saveExportCommandLabelControl;
		this.saveExportCommandLayoutControlItem.Location = new System.Drawing.Point(487, 424);
		this.saveExportCommandLayoutControlItem.Name = "saveExportCommandLayoutControlItem";
		this.saveExportCommandLayoutControlItem.Size = new System.Drawing.Size(110, 17);
		this.saveExportCommandLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveExportCommandLayoutControlItem.TextVisible = false;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 72);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(597, 352);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 424);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(487, 17);
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.titleLayoutControlItem.Control = this.titleTextEdit;
		this.titleLayoutControlItem.CustomizationFormText = "File name";
		this.titleLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.titleLayoutControlItem.Name = "titleLayoutControlItem";
		this.titleLayoutControlItem.Size = new System.Drawing.Size(597, 24);
		this.titleLayoutControlItem.Text = "Title:";
		this.titleLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.titleLayoutControlItem.TextSize = new System.Drawing.Size(60, 13);
		this.titleLayoutControlItem.TextToControlDistance = 5;
		this.formatWizardPage.Controls.Add(this.webCatalogInfoUserControl);
		this.formatWizardPage.Controls.Add(this.betaLabelControl);
		this.formatWizardPage.Controls.Add(this.copyDescriptionsHyperlinkLabelControl);
		this.formatWizardPage.Controls.Add(this.ExtendedPropHyperlinkLabelControl);
		this.formatWizardPage.Controls.Add(this.legacyTextLabelControl);
		this.formatWizardPage.Controls.Add(this.infoUserControl);
		this.formatWizardPage.Controls.Add(this.radioGroupBuyProLabelControl4);
		this.formatWizardPage.Controls.Add(this.radioGroupBuyProLabelControl3);
		this.formatWizardPage.Controls.Add(this.radioGroupBuyProLabelControl2);
		this.formatWizardPage.Controls.Add(this.docFormatRadioGroup);
		this.formatWizardPage.DescriptionText = "Please choose documentation export format.";
		this.formatWizardPage.Name = "formatWizardPage";
		this.formatWizardPage.Size = new System.Drawing.Size(617, 461);
		this.formatWizardPage.Text = "Choose format";
		this.webCatalogInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.webCatalogInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.webCatalogInfoUserControl.Description = "Web Catalog is recommended";
		this.webCatalogInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.webCatalogInfoUserControl.Image = (System.Drawing.Image)resources.GetObject("webCatalogInfoUserControl.Image");
		this.webCatalogInfoUserControl.Location = new System.Drawing.Point(0, 429);
		this.webCatalogInfoUserControl.Name = "webCatalogInfoUserControl";
		this.webCatalogInfoUserControl.Size = new System.Drawing.Size(755, 32);
		this.webCatalogInfoUserControl.TabIndex = 11;
		this.betaLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(229, 229, 229);
		this.betaLabelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(124, 124, 124);
		this.betaLabelControl.Appearance.Options.UseBackColor = true;
		this.betaLabelControl.Appearance.Options.UseForeColor = true;
		this.betaLabelControl.Appearance.Options.UseTextOptions = true;
		this.betaLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.betaLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.betaLabelControl.Location = new System.Drawing.Point(238, 198);
		this.betaLabelControl.MinimumSize = new System.Drawing.Size(40, 20);
		this.betaLabelControl.Name = "betaLabelControl";
		this.betaLabelControl.Size = new System.Drawing.Size(40, 20);
		this.betaLabelControl.TabIndex = 10;
		this.betaLabelControl.Text = "Beta";
		this.copyDescriptionsHyperlinkLabelControl.Location = new System.Drawing.Point(284, 202);
		this.copyDescriptionsHyperlinkLabelControl.Name = "copyDescriptionsHyperlinkLabelControl";
		this.copyDescriptionsHyperlinkLabelControl.Size = new System.Drawing.Size(49, 13);
		this.copyDescriptionsHyperlinkLabelControl.TabIndex = 8;
		this.copyDescriptionsHyperlinkLabelControl.Text = "learn more";
		this.copyDescriptionsHyperlinkLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(copyDescriptionsHyperlinkLabelControl_HyperlinkClick);
		this.ExtendedPropHyperlinkLabelControl.Location = new System.Drawing.Point(187, 171);
		this.ExtendedPropHyperlinkLabelControl.Name = "ExtendedPropHyperlinkLabelControl";
		this.ExtendedPropHyperlinkLabelControl.Size = new System.Drawing.Size(49, 13);
		this.ExtendedPropHyperlinkLabelControl.TabIndex = 7;
		this.ExtendedPropHyperlinkLabelControl.Text = "learn more";
		this.ExtendedPropHyperlinkLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(ExtendedPropHyperlinkLabelControl_HyperlinkClick);
		this.legacyTextLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(229, 229, 229);
		this.legacyTextLabelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(124, 124, 124);
		this.legacyTextLabelControl.Appearance.Options.UseBackColor = true;
		this.legacyTextLabelControl.Appearance.Options.UseForeColor = true;
		this.legacyTextLabelControl.Appearance.Options.UseTextOptions = true;
		this.legacyTextLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.legacyTextLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.legacyTextLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.RightCenter;
		this.legacyTextLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.question_16;
		this.legacyTextLabelControl.Location = new System.Drawing.Point(51, 109);
		this.legacyTextLabelControl.MaximumSize = new System.Drawing.Size(65, 0);
		this.legacyTextLabelControl.MinimumSize = new System.Drawing.Size(65, 20);
		this.legacyTextLabelControl.Name = "legacyTextLabelControl";
		this.legacyTextLabelControl.Size = new System.Drawing.Size(65, 20);
		toolTipItem.Text = "We are no longer maintaining PDF export, so it can be missing newer features.";
		superToolTip.Items.Add(toolTipItem);
		this.legacyTextLabelControl.SuperTip = superToolTip;
		this.legacyTextLabelControl.TabIndex = 6;
		this.legacyTextLabelControl.Text = "Legacy";
		this.legacyTextLabelControl.ToolTipController = this.toolTipController;
		this.infoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl.Description = null;
		this.infoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl.Image = (System.Drawing.Image)resources.GetObject("infoUserControl.Image");
		this.infoUserControl.Location = new System.Drawing.Point(0, 3);
		this.infoUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.infoUserControl.Name = "infoUserControl";
		this.infoUserControl.Size = new System.Drawing.Size(755, 32);
		this.infoUserControl.TabIndex = 5;
		this.radioGroupBuyProLabelControl4.AllowHtmlString = true;
		this.radioGroupBuyProLabelControl4.Location = new System.Drawing.Point(341, 202);
		this.radioGroupBuyProLabelControl4.Name = "radioGroupBuyProLabelControl4";
		this.radioGroupBuyProLabelControl4.Size = new System.Drawing.Size(41, 13);
		this.radioGroupBuyProLabelControl4.TabIndex = 4;
		this.radioGroupBuyProLabelControl4.Text = "Upgrade";
		this.radioGroupBuyProLabelControl4.Visible = false;
		this.radioGroupBuyProLabelControl4.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(radioGroupBuyProLabelControl_HyperlinkClick);
		this.radioGroupBuyProLabelControl3.AllowHtmlString = true;
		this.radioGroupBuyProLabelControl3.Location = new System.Drawing.Point(244, 171);
		this.radioGroupBuyProLabelControl3.Name = "radioGroupBuyProLabelControl3";
		this.radioGroupBuyProLabelControl3.Size = new System.Drawing.Size(41, 13);
		this.radioGroupBuyProLabelControl3.TabIndex = 3;
		this.radioGroupBuyProLabelControl3.Text = "Upgrade";
		this.radioGroupBuyProLabelControl3.Visible = false;
		this.radioGroupBuyProLabelControl3.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(radioGroupBuyProLabelControl_HyperlinkClick);
		this.radioGroupBuyProLabelControl2.AllowHtmlString = true;
		this.radioGroupBuyProLabelControl2.Location = new System.Drawing.Point(57, 83);
		this.radioGroupBuyProLabelControl2.Name = "radioGroupBuyProLabelControl2";
		this.radioGroupBuyProLabelControl2.Size = new System.Drawing.Size(41, 13);
		this.radioGroupBuyProLabelControl2.TabIndex = 2;
		this.radioGroupBuyProLabelControl2.Text = "Upgrade";
		this.radioGroupBuyProLabelControl2.Visible = false;
		this.radioGroupBuyProLabelControl2.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(radioGroupBuyProLabelControl_HyperlinkClick);
		this.docFormatRadioGroup.Location = new System.Drawing.Point(0, 41);
		this.docFormatRadioGroup.Name = "docFormatRadioGroup";
		this.docFormatRadioGroup.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.docFormatRadioGroup.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.docFormatRadioGroup.Properties.Appearance.Options.UseBackColor = true;
		this.docFormatRadioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.docFormatRadioGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[6]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "HTML", true, "html"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Excel ", true, "excel"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "PDF", true, "pdf"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "DDL Script", true, "ddl"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Extended properties / comments", true, "ep"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Copy descriptions to other documentations", true, "copy")
		});
		this.docFormatRadioGroup.Size = new System.Drawing.Size(614, 193);
		this.docFormatRadioGroup.TabIndex = 0;
		this.dbConnectWizardPage.Controls.Add(this.layoutControl3);
		this.dbConnectWizardPage.DescriptionText = "Please provide connection details to database you want to export descriptions.";
		this.dbConnectWizardPage.Name = "dbConnectWizardPage";
		this.dbConnectWizardPage.Size = new System.Drawing.Size(617, 461);
		this.dbConnectWizardPage.Text = "Connection";
		this.layoutControl3.AllowCustomization = false;
		this.layoutControl3.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl3.Controls.Add(this.scriptsOnlyCheckEdit);
		this.layoutControl3.Controls.Add(this.dbConnectUserControl);
		this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl3.Location = new System.Drawing.Point(0, 0);
		this.layoutControl3.Name = "layoutControl3";
		this.layoutControl3.Root = this.layoutControlGroup3;
		this.layoutControl3.Size = new System.Drawing.Size(617, 461);
		this.layoutControl3.TabIndex = 1;
		this.layoutControl3.Text = "layoutControl3";
		this.scriptsOnlyCheckEdit.Location = new System.Drawing.Point(17, 439);
		this.scriptsOnlyCheckEdit.Name = "scriptsOnlyCheckEdit";
		this.scriptsOnlyCheckEdit.Properties.Caption = "Only generate scripts (do not run)";
		this.scriptsOnlyCheckEdit.Size = new System.Drawing.Size(598, 20);
		this.scriptsOnlyCheckEdit.StyleController = this.layoutControl3;
		this.scriptsOnlyCheckEdit.TabIndex = 5;
		this.dbConnectUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dbConnectUserControl.DatabaseRow = null;
		this.dbConnectUserControl.IsDBAdded = false;
		this.dbConnectUserControl.IsExporting = false;
		this.dbConnectUserControl.Location = new System.Drawing.Point(2, 2);
		this.dbConnectUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.dbConnectUserControl.Name = "dbConnectUserControl";
		this.dbConnectUserControl.SelectedDatabaseType = null;
		this.dbConnectUserControl.Size = new System.Drawing.Size(613, 433);
		this.dbConnectUserControl.TabIndex = 4;
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem4 });
		this.layoutControlGroup3.Name = "layoutControlGroup3";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(617, 461);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControlItem2.Control = this.dbConnectUserControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(617, 437);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.scriptsOnlyCheckEdit;
		this.layoutControlItem3.Location = new System.Drawing.Point(15, 437);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(602, 24);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 437);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(15, 23);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(15, 23);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(15, 24);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.chooseModulesWizardPage.Controls.Add(this.chooseModulesUserControl);
		this.chooseModulesWizardPage.DescriptionText = "Please choose Subject Areas you want to include in documentation.";
		this.chooseModulesWizardPage.Name = "chooseModulesWizardPage";
		this.chooseModulesWizardPage.Size = new System.Drawing.Size(617, 461);
		this.chooseModulesWizardPage.Text = "Choose Subject Areas";
		this.chooseModulesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.chooseModulesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chooseModulesUserControl.Location = new System.Drawing.Point(0, 0);
		this.chooseModulesUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.chooseModulesUserControl.Name = "chooseModulesUserControl";
		this.chooseModulesUserControl.Size = new System.Drawing.Size(617, 461);
		this.chooseModulesUserControl.TabIndex = 0;
		this.chooseObjectsWizardPage.Controls.Add(this.chooseObjectTypesUserControl);
		this.chooseObjectsWizardPage.DescriptionText = "Please select database object types you want to include in documentation.";
		this.chooseObjectsWizardPage.Name = "chooseObjectsWizardPage";
		this.chooseObjectsWizardPage.Size = new System.Drawing.Size(617, 461);
		this.chooseObjectsWizardPage.Text = "Choose objects";
		this.chooseObjectTypesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.chooseObjectTypesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chooseObjectTypesUserControl.IsExportingToDatabase = false;
		this.chooseObjectTypesUserControl.Location = new System.Drawing.Point(0, 0);
		this.chooseObjectTypesUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.chooseObjectTypesUserControl.Name = "chooseObjectTypesUserControl";
		this.chooseObjectTypesUserControl.Size = new System.Drawing.Size(617, 461);
		this.chooseObjectTypesUserControl.TabIndex = 0;
		this.chooseCustomFieldsWizardPage.Controls.Add(this.chooseCustomFieldsUserControl);
		this.chooseCustomFieldsWizardPage.DescriptionText = "Please select custom fields you want to include in documentation.";
		this.chooseCustomFieldsWizardPage.Name = "chooseCustomFieldsWizardPage";
		this.chooseCustomFieldsWizardPage.Size = new System.Drawing.Size(617, 461);
		this.chooseCustomFieldsWizardPage.Text = "Choose custom fields";
		this.chooseCustomFieldsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.chooseCustomFieldsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chooseCustomFieldsUserControl.IsExtendedPropertyExport = false;
		this.chooseCustomFieldsUserControl.Location = new System.Drawing.Point(0, 0);
		this.chooseCustomFieldsUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.chooseCustomFieldsUserControl.Name = "chooseCustomFieldsUserControl";
		this.chooseCustomFieldsUserControl.OtherFieldsSupport = null;
		this.chooseCustomFieldsUserControl.Size = new System.Drawing.Size(617, 461);
		this.chooseCustomFieldsUserControl.TabIndex = 0;
		this.chooseDocumentationsWizardPage.Controls.Add(this.documentationsUserControl);
		this.chooseDocumentationsWizardPage.DescriptionText = "Please choose documentations to copy from and to.";
		this.chooseDocumentationsWizardPage.Name = "chooseDocumentationsWizardPage";
		this.chooseDocumentationsWizardPage.Size = new System.Drawing.Size(617, 461);
		this.chooseDocumentationsWizardPage.Text = "Choose documentations";
		this.documentationsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.documentationsUserControl.DestinationRepository = null;
		this.documentationsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.documentationsUserControl.Location = new System.Drawing.Point(0, 0);
		this.documentationsUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.documentationsUserControl.ModifiedDatabases = null;
		this.documentationsUserControl.Name = "documentationsUserControl";
		this.documentationsUserControl.RedrawLabel = null;
		this.documentationsUserControl.SetControls = null;
		this.documentationsUserControl.Size = new System.Drawing.Size(617, 461);
		this.documentationsUserControl.TabIndex = 0;
		this.chooseRepositoryDocumentationsWizardPage.Controls.Add(this.repositoriesUserControl);
		this.chooseRepositoryDocumentationsWizardPage.Controls.Add(this.betaFeatureInfoUserControl);
		this.chooseRepositoryDocumentationsWizardPage.DescriptionText = "Please select repository to copy descritpions to.";
		this.chooseRepositoryDocumentationsWizardPage.Name = "chooseRepositoryDocumentationsWizardPage";
		this.chooseRepositoryDocumentationsWizardPage.Size = new System.Drawing.Size(617, 461);
		this.chooseRepositoryDocumentationsWizardPage.Text = "Choose repository";
		this.repositoriesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.repositoriesUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.repositoriesUserControl.Location = new System.Drawing.Point(0, 32);
		this.repositoriesUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.repositoriesUserControl.Name = "repositoriesUserControl";
		this.repositoriesUserControl.Size = new System.Drawing.Size(617, 102);
		this.repositoriesUserControl.TabIndex = 0;
		this.betaFeatureInfoUserControl.BackColor = System.Drawing.Color.White;
		this.betaFeatureInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.betaFeatureInfoUserControl.Description = "This is a beta feature";
		this.betaFeatureInfoUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.betaFeatureInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.betaFeatureInfoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.betaFeatureInfoUserControl.Location = new System.Drawing.Point(0, 0);
		this.betaFeatureInfoUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.betaFeatureInfoUserControl.Name = "betaFeatureInfoUserControl";
		this.betaFeatureInfoUserControl.Size = new System.Drawing.Size(617, 32);
		this.betaFeatureInfoUserControl.TabIndex = 1;
		this.DDLChooseDocumentationWizardPage.Controls.Add(this.ddlDocumentationsUserControl);
		this.DDLChooseDocumentationWizardPage.DescriptionText = "Please choose the database that you would like to export as a DDL script.";
		this.DDLChooseDocumentationWizardPage.Name = "DDLChooseDocumentationWizardPage";
		this.DDLChooseDocumentationWizardPage.Size = new System.Drawing.Size(617, 461);
		this.DDLChooseDocumentationWizardPage.Text = "Choose database";
		this.ddlDocumentationsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.ddlDocumentationsUserControl.DatabaseDBMSVersion = null;
		this.ddlDocumentationsUserControl.DatabaseId = 0;
		this.ddlDocumentationsUserControl.DatabaseType = null;
		this.ddlDocumentationsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ddlDocumentationsUserControl.Location = new System.Drawing.Point(0, 0);
		this.ddlDocumentationsUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.ddlDocumentationsUserControl.Name = "ddlDocumentationsUserControl";
		this.ddlDocumentationsUserControl.Size = new System.Drawing.Size(617, 461);
		this.ddlDocumentationsUserControl.TabIndex = 0;
		this.DDLScriptTypeWizardPage.Controls.Add(this.ddlChooseObjectsLayoutControl);
		this.DDLScriptTypeWizardPage.DescriptionText = "Please choose whether you would like to create a script for a new database or update an existing one.";
		this.DDLScriptTypeWizardPage.Name = "DDLScriptTypeWizardPage";
		this.DDLScriptTypeWizardPage.Size = new System.Drawing.Size(617, 461);
		this.DDLScriptTypeWizardPage.Text = "Settings";
		this.ddlChooseObjectsLayoutControl.AllowCustomization = false;
		this.ddlChooseObjectsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.ddlChooseObjectsLayoutControl.Controls.Add(this.ddlScriptTypePanelControl);
		this.ddlChooseObjectsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ddlChooseObjectsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.ddlChooseObjectsLayoutControl.Name = "ddlChooseObjectsLayoutControl";
		this.ddlChooseObjectsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(761, 308, 650, 400);
		this.ddlChooseObjectsLayoutControl.Root = this.Root;
		this.ddlChooseObjectsLayoutControl.Size = new System.Drawing.Size(617, 461);
		this.ddlChooseObjectsLayoutControl.TabIndex = 0;
		this.ddlChooseObjectsLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.ddlScriptTypePanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.ddlScriptTypePanelControl.Controls.Add(this.andLabelControl);
		this.ddlScriptTypePanelControl.Controls.Add(this.alterGrayLabelControl);
		this.ddlScriptTypePanelControl.Controls.Add(this.create2GrayLabelControl);
		this.ddlScriptTypePanelControl.Controls.Add(this.createGrayLabelControl);
		this.ddlScriptTypePanelControl.Controls.Add(this.ddlScriptTypeRadioGroup);
		this.ddlScriptTypePanelControl.Location = new System.Drawing.Point(12, 28);
		this.ddlScriptTypePanelControl.Name = "ddlScriptTypePanelControl";
		this.ddlScriptTypePanelControl.Size = new System.Drawing.Size(593, 85);
		this.ddlScriptTypePanelControl.TabIndex = 4;
		this.andLabelControl.Location = new System.Drawing.Point(211, 54);
		this.andLabelControl.Name = "andLabelControl";
		this.andLabelControl.Size = new System.Drawing.Size(18, 13);
		this.andLabelControl.TabIndex = 4;
		this.andLabelControl.Text = "and";
		this.alterGrayLabelControl.AllowHtmlString = true;
		this.alterGrayLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(229, 229, 229);
		this.alterGrayLabelControl.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.alterGrayLabelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(57, 57, 57);
		this.alterGrayLabelControl.Appearance.Options.UseBackColor = true;
		this.alterGrayLabelControl.Appearance.Options.UseFont = true;
		this.alterGrayLabelControl.Appearance.Options.UseForeColor = true;
		this.alterGrayLabelControl.Location = new System.Drawing.Point(233, 49);
		this.alterGrayLabelControl.MaximumSize = new System.Drawing.Size(0, 24);
		this.alterGrayLabelControl.MinimumSize = new System.Drawing.Size(0, 24);
		this.alterGrayLabelControl.Name = "alterGrayLabelControl";
		this.alterGrayLabelControl.Padding = new System.Windows.Forms.Padding(4);
		this.alterGrayLabelControl.Size = new System.Drawing.Size(43, 24);
		this.alterGrayLabelControl.TabIndex = 3;
		this.alterGrayLabelControl.Text = "ALTER";
		this.create2GrayLabelControl.AllowHtmlString = true;
		this.create2GrayLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(229, 229, 229);
		this.create2GrayLabelControl.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.create2GrayLabelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(57, 57, 57);
		this.create2GrayLabelControl.Appearance.Options.UseBackColor = true;
		this.create2GrayLabelControl.Appearance.Options.UseFont = true;
		this.create2GrayLabelControl.Appearance.Options.UseForeColor = true;
		this.create2GrayLabelControl.Location = new System.Drawing.Point(156, 49);
		this.create2GrayLabelControl.MaximumSize = new System.Drawing.Size(0, 24);
		this.create2GrayLabelControl.MinimumSize = new System.Drawing.Size(0, 24);
		this.create2GrayLabelControl.Name = "create2GrayLabelControl";
		this.create2GrayLabelControl.Padding = new System.Windows.Forms.Padding(4);
		this.create2GrayLabelControl.Size = new System.Drawing.Size(50, 24);
		this.create2GrayLabelControl.TabIndex = 2;
		this.create2GrayLabelControl.Text = "CREATE";
		this.createGrayLabelControl.AllowHtmlString = true;
		this.createGrayLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(229, 229, 229);
		this.createGrayLabelControl.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.createGrayLabelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(57, 57, 57);
		this.createGrayLabelControl.Appearance.Options.UseBackColor = true;
		this.createGrayLabelControl.Appearance.Options.UseFont = true;
		this.createGrayLabelControl.Appearance.Options.UseForeColor = true;
		this.createGrayLabelControl.Location = new System.Drawing.Point(139, 11);
		this.createGrayLabelControl.MaximumSize = new System.Drawing.Size(0, 24);
		this.createGrayLabelControl.MinimumSize = new System.Drawing.Size(0, 24);
		this.createGrayLabelControl.Name = "createGrayLabelControl";
		this.createGrayLabelControl.Padding = new System.Windows.Forms.Padding(4);
		this.createGrayLabelControl.Size = new System.Drawing.Size(50, 24);
		this.createGrayLabelControl.TabIndex = 1;
		this.createGrayLabelControl.Text = "CREATE";
		this.ddlScriptTypeRadioGroup.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ddlScriptTypeRadioGroup.EditValue = "new";
		this.ddlScriptTypeRadioGroup.Location = new System.Drawing.Point(0, 0);
		this.ddlScriptTypeRadioGroup.MenuManager = this.tamplatesBarManager;
		this.ddlScriptTypeRadioGroup.Name = "ddlScriptTypeRadioGroup";
		this.ddlScriptTypeRadioGroup.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.ddlScriptTypeRadioGroup.Properties.Appearance.Options.UseBackColor = true;
		this.ddlScriptTypeRadioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.ddlScriptTypeRadioGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("new", "New database script"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("update", "Update existing database")
		});
		this.ddlScriptTypeRadioGroup.Size = new System.Drawing.Size(593, 85);
		this.ddlScriptTypeRadioGroup.TabIndex = 0;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.ddlScriptTypeLayoutControlItem, this.emptySpaceItem7 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(617, 461);
		this.Root.TextVisible = false;
		this.ddlScriptTypeLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.ddlScriptTypeLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.ddlScriptTypeLayoutControlItem.Control = this.ddlScriptTypePanelControl;
		this.ddlScriptTypeLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.ddlScriptTypeLayoutControlItem.Name = "ddlScriptTypeLayoutControlItem";
		this.ddlScriptTypeLayoutControlItem.Size = new System.Drawing.Size(597, 105);
		this.ddlScriptTypeLayoutControlItem.Text = "Script type";
		this.ddlScriptTypeLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.ddlScriptTypeLayoutControlItem.TextSize = new System.Drawing.Size(62, 13);
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.Location = new System.Drawing.Point(0, 105);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(597, 336);
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.DDLExportSettingsWizardPage.Controls.Add(this.ddlExportSettingsUserControl);
		this.DDLExportSettingsWizardPage.DescriptionText = "Please set options for the export script.";
		this.DDLExportSettingsWizardPage.Name = "DDLExportSettingsWizardPage";
		this.DDLExportSettingsWizardPage.Size = new System.Drawing.Size(617, 461);
		this.DDLExportSettingsWizardPage.Text = "Settings";
		this.ddlExportSettingsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.ddlExportSettingsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ddlExportSettingsUserControl.Location = new System.Drawing.Point(0, 0);
		this.ddlExportSettingsUserControl.Margin = new System.Windows.Forms.Padding(2);
		this.ddlExportSettingsUserControl.Name = "ddlExportSettingsUserControl";
		this.ddlExportSettingsUserControl.Size = new System.Drawing.Size(617, 461);
		this.ddlExportSettingsUserControl.TabIndex = 0;
		this.DDLChooseObjectsWizardPage.Controls.Add(this.ddlChooseObjectsUserControl);
		this.DDLChooseObjectsWizardPage.DescriptionText = "Please choose which objects you would like to include in the DDL script.";
		this.DDLChooseObjectsWizardPage.Name = "DDLChooseObjectsWizardPage";
		this.DDLChooseObjectsWizardPage.Size = new System.Drawing.Size(617, 461);
		this.DDLChooseObjectsWizardPage.Text = "Settings";
		this.ddlChooseObjectsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.ddlChooseObjectsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ddlChooseObjectsUserControl.Location = new System.Drawing.Point(0, 0);
		this.ddlChooseObjectsUserControl.Name = "ddlChooseObjectsUserControl";
		this.ddlChooseObjectsUserControl.Size = new System.Drawing.Size(617, 461);
		this.ddlChooseObjectsUserControl.TabIndex = 0;
		this.progressBarControl1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.progressBarControl1.Location = new System.Drawing.Point(2, 85);
		this.progressBarControl1.Name = "progressBarControl1";
		this.progressBarControl1.Properties.EndColor = System.Drawing.Color.FromArgb(70, 121, 198);
		this.progressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.progressBarControl1.Properties.ShowTitle = true;
		this.progressBarControl1.Properties.StartColor = System.Drawing.Color.FromArgb(70, 121, 198);
		this.progressBarControl1.Properties.Step = 1;
		this.progressBarControl1.Size = new System.Drawing.Size(313, 15);
		this.progressBarControl1.TabIndex = 10;
		this.excelSaveFileDialog.DefaultExt = "xlsx";
		this.excelSaveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx|Excel 97-2003 Workbook (*.xls)|*.xls";
		this.excelSaveFileDialog.Title = "Saving as";
		this.templatesPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.customizeBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.openFolderBarButtonItem)
		});
		this.templatesPopupMenu.Manager = this.tamplatesBarManager;
		this.templatesPopupMenu.Name = "templatesPopupMenu";
		this.templatesPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(templatesPopupMenu_BeforePopup);
		this.sqlSaveFileDialog.DefaultExt = "sql";
		this.sqlSaveFileDialog.Filter = "SQL Files (*.sql)|*.sql";
		this.sqlSaveFileDialog.Title = "Saving as";
		this.toolTipController.InitialDelay = 100;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(649, 604);
		base.Controls.Add(this.docWizardControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("DocWizardForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.MaximizeBox = false;
		base.Name = "DocWizardForm";
		this.Text = " Export";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DocWizardForm_FormClosing);
		this.docCompletionWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).EndInit();
		this.layoutControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.generatingDocLayoutControl).EndInit();
		this.generatingDocLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.copyScriptHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.creatingPagesProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.generatingDocProgressBar.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.exportingDetailsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeBarLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		this.docTemplateWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		this.layoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.templatesGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.templatesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionRepositoryItemMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customizeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customizeEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tamplatesBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docWizardControl).EndInit();
		this.docWizardControl.ResumeLayout(false);
		this.exportDataWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.filenameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.folderButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.directoryPathLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveExportCommandLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).EndInit();
		this.formatWizardPage.ResumeLayout(false);
		this.formatWizardPage.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.docFormatRadioGroup.Properties).EndInit();
		this.dbConnectWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).EndInit();
		this.layoutControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.scriptsOnlyCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		this.chooseModulesWizardPage.ResumeLayout(false);
		this.chooseObjectsWizardPage.ResumeLayout(false);
		this.chooseCustomFieldsWizardPage.ResumeLayout(false);
		this.chooseDocumentationsWizardPage.ResumeLayout(false);
		this.chooseRepositoryDocumentationsWizardPage.ResumeLayout(false);
		this.DDLChooseDocumentationWizardPage.ResumeLayout(false);
		this.DDLScriptTypeWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ddlChooseObjectsLayoutControl).EndInit();
		this.ddlChooseObjectsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ddlScriptTypePanelControl).EndInit();
		this.ddlScriptTypePanelControl.ResumeLayout(false);
		this.ddlScriptTypePanelControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.ddlScriptTypeRadioGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ddlScriptTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		this.DDLExportSettingsWizardPage.ResumeLayout(false);
		this.DDLChooseObjectsWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressBarControl1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.templatesPopupMenu).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
