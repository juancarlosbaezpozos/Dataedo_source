using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classification.Forms;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.Forms;
using Dataedo.App.Licences;
using Dataedo.App.LoginFormTools;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.MenuTree;
using Dataedo.App.Onboarding;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlHelpers;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.CustomControls;
using Dataedo.CustomMessageBox;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;
using RecentProjectsLibrary;

namespace Dataedo.App.Forms;

public class MainForm : BaseRibbonForm
{
    private MetadataEditorUserControl metadataEditorUserControl;

    private LoginFormNew loginForm;

    private bool isSearchActive;

    private bool onboardingAfterSavingDiagramPending;

    public static MainForm Instance;

    private IContainer components;

    private RibbonControl mainRibbonControl;

    private BarButtonItem refreshMenuButton;

    private RibbonPage mainRibbonPage;

    private PanelControl mainPanel;

    private SaveFileDialog xmlSaveFileDialog;

    private BarButtonItem addDatabaseButton;

    private RibbonPageGroup debugToolsRibbonPageGroup;

    private ImageCollection exportTypesImageCollection;

    private BarButtonItem saveButton;

    private RibbonPageGroup designRibbonPageGroup;

    private BarButtonItem DocCreatorBarButtonItem;

    private BarButtonItem aboutBarButtonItem;

    private BarButtonItem synchronizeBarButtonItem;

    private BarButtonItem addModuleBarButtonItem;

    private BarButtonItem exporttoExcelBarButtonItem;

    private BarButtonItem documentationXtraReportBarButtonItem;

    private BarButtonItem searchButtonItem;

    private RibbonPageGroup erdRibbonPageGroup;

    private BarButtonItem showProgressBarButtonItem;

    private BarButtonItem newOpenBarButtonItem;

    private BarButtonItem moveDownDependencyBarButtonItem;

    private BarButtonItem moveUpDependencyBarButtonItem;

    private BarButtonItem addRelationBarButtonItem;

    private BarButtonItem supportBarButtonItem;

    private BarButtonItem editRelationBarButtonItem;

    private BarButtonItem editConstraintBarButtonItem;

    private BarButtonItem customFieldsBarButtonItem;

    private PanelControl majorUpdatePanelControl;

    private TableLayoutPanel minorUpdateTableLayoutPanel;

    private Label minorUpdateLabel;

    private PictureEdit updateInfoPictureEdit;

    private SimpleButton downloadMinorUpdateButton;

    private SimpleButton closeMinorUpdatePanelButton;

    private PanelControl minorUpdatePanelControl;

    private Label updateInfolabel;

    private BarButtonItem addUserTableBarButtonItem;

    private PopupMenu databasePopUpMenu;

    private BarButtonItem editTableBarButtonItem;

    private ImageCollection databasesImageCollection;

    private BarButtonItem addNewTableBarButtonItem;

    private BarButtonItem designTableBarButtonItem;

    private BarButtonItem editDisplayOptionsBarButtonItem;

    private BarButtonItem editRelationERDBarButtonItem;

    private BarButtonItem showHideAvailableEntitiesBarButtonItem;

    private PopupMenu progressPopupMenu;

    private BarButtonItem barButtonItem2;

    private BarButtonItem suggestedDescriptionBarButtonItem;

    private BarButtonItem chatBarButtonItem;

    private BarButtonItem addTermBarButtonItem;

    private BarButtonItem addRelatedTermBarButtonItem;

    private BarButtonItem addDataLinkBarButtonItem;

    private PopupMenu addTermPopupMenu;

    private BarButtonItem classificationBarButtonItem;

    private ToolTipController toolTipController;

    private BarAndDockingController barAndDockingController;

    private SplashScreenManager splashScreenManager;

    private SkinRibbonGalleryBarItem skinRibbonGalleryBarItem2;

    private SkinRibbonGalleryBarItem skinRibbonGalleryBarItem1;

    private SkinPaletteRibbonGalleryBarItem skinPaletteRibbonGalleryBarItem1;

    private TableLayoutPanel majorUpdateTableLayoutPanel;

    private Label majorUpdateLabel;

    private Label newVersionInfoLabel;

    private PictureEdit newVersionInfoPictureEdit;

    private SimpleButton updateNowUpdatePanelControlButton;

    private SimpleButton closeMajorUpdatePanelButton;

    private SkinPaletteDropDownButtonItem skinPaletteDropDownButtonItem1;

    private PopupMenu recentPopupMenu;

    private RibbonPageGroup addRibbonPageGroup;

    private RibbonPageGroup toolsRibbonPageGroup;

    private RibbonPage helpRibbonPage;

    private RibbonPage debugToolsRibbonPage;

    private PopupMenu savePopupMenu;

    private BarButtonItem turnAutosaveBarButtonItem;

    private ToolTipController onboardingToolTipController;

    private BarButtonItem profileTablebarButtonItem;

    private BarButtonItem profileColumnbarButtonItem;

    private PopupMenu addObjectPopupMenu;

    private RibbonPageGroup importExportRibbonPageGroup;

    private RibbonPageGroup informationRibbonPageGroup;

    private RibbonPageGroup supportRibbonPageGroup;

    private RibbonPageGroup communityRibbonPageGroup;

    private BarButtonItem communityBarButtonItem;

    private BarButtonItem shareProblemBarButtonItem;

    private BarButtonItem shareQuestionBarButtonItem;

    private BarButtonItem shareIdeaBarButtonItem;

    private BarButtonItem documentationBarButtonItem;

    private BarButtonItem tutorialsBarButtonItem;

    private BarButtonItem createSupportTicketBarButtonItem;

    private BarButtonItem talkToSalesBarButtonItem;

    private BarButtonItem userBarButtonItem;

    private PopupControlContainer userPopupControlContainer;

    private UserPanelUserControl userPanelUserControl;

    private BarButtonItem checkPriceBuyBarButtonItem;

    private BarButtonItem addPrimaryKeyBarButtonItem;

    private BarButtonItem addUniqueKeyBarButtonItem;

    private BarButtonItem repositoryDocumentationBarButtonItem;

    private RibbonPageGroup configRibbonPageGroup;

    private BarButtonItem settingsBarButtonItem;

    private RibbonPageGroup viewRibbonPageGroup;

    private BarButtonItem previewSampleDataBarButtonItem;

    private BarButtonItem sortAlphabeticallyButtonItem;

    private BarButtonItem moveUpBarButtonItem;

    private BarButtonItem moveDownBarButtonItem;

    private BarButtonItem moveToTopBarButtonItem;

    private BarButtonItem moveToBottomBarButtonItem;

    private BarButtonItem dataProfilingBarButtonItem;

    private BarButtonItem addNewViewBarButtonItem;

    private BarButtonItem addNewStructureBarButtonItem;

    private BarButtonItem addNewPostItBarButtonItem;

    private BarButtonItem showStaticDataBarButtonItem;

    private BarButtonItem showFlowsColumnsButtonItem;

    private BarButtonItem zoomInLineageDiagramButtonItem;

    private BarButtonItem zoomOutLineageDiagramButtonItem;

    public MainForm(LoginFormNew loginForm, bool loadData = false)
    {
        Instance = this;
        CustomMessageBoxForm.DefaultOwner = this;
        InitializeComponent();
        this.loginForm = loginForm;
        metadataEditorUserControl = new MetadataEditorUserControl(loadData);
        metadataEditorUserControl.SelectedObjectChanged += delegate (object sender, EventArgs e)
        {
            ChangeDeleteButtonIcon(e as ObjectTypeEventArgs);
        };
        metadataEditorUserControl.SelectedObjectChanged += delegate (object sender, EventArgs e)
        {
            SetERDGroupVisibility(e as ObjectTypeEventArgs);
        };
        metadataEditorUserControl.SelectedObjectChanged += delegate (object sender, EventArgs e)
        {
            ShowHideButtonsForDocumentation(e as ObjectTypeEventArgs);
        };
        metadataEditorUserControl.SelectedObjectChanged += delegate (object sender, EventArgs e)
        {
            ShowHideTableButtons(e as ObjectTypeEventArgs);
        };
        metadataEditorUserControl.SelectedObjectChanged += delegate (object sender, EventArgs e)
        {
            SetManualItemButtonVisibility(e as ObjectTypeEventArgs);
        };
        metadataEditorUserControl.SelectedObjectChanged += delegate (object sender, EventArgs e)
        {
            SetModulesButtonsVisibility(e as ObjectTypeEventArgs);
        };
        metadataEditorUserControl.SelectedObjectChanged += delegate (object sender, EventArgs e)
        {
            SetDataLineageButtonsVisibility(e as ObjectTypeEventArgs);
        };
        metadataEditorUserControl.ModuleERDTabSelectedEvent += delegate (object sender, EventArgs e)
        {
            SetERDGroupVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.ModuleDescriptionTabSelectedEvent += delegate (object sender, EventArgs e)
        {
            SetModulesPositionButtonsVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.ModuleERDNodeSelectedEvent += delegate (object sender, EventArgs e)
        {
            SetERDButtonsVisibility(e as ErdButtonArgs);
        };
        metadataEditorUserControl.ErdSavedEvent += delegate
        {
            ShowOnboardingAfterSavingDiagram();
        };
        metadataEditorUserControl.GoingToNodeEnded += delegate
        {
            ShowOnboardings();
        };
        metadataEditorUserControl.AddRelationVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetAddRelationButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.ProfileTableVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetTableProfilingButtonsVisibility(e as ProfilingVisibilityEventArgs);
        };
        metadataEditorUserControl.ProfileColumnVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetProfileColumnButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.EditRelationVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetEditRelationButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.AddPrimaryKeyVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetAddPrimaryKeyButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.AddUniqueKeyVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetAddUniqueKeyButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.EditConstraintVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetEditConstraintButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.UserObjectsButtonsEnabledEvent += delegate
        {
            SetUserObjectsButtonsEnabled();
        };
        metadataEditorUserControl.RefreshBarButtonText += delegate
        {
            SetBarButtonAfterSynchronize();
        };
        metadataEditorUserControl.AddRelatedTermVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetAddRelatedTermButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.AddDataLinkVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetAddDataLinksButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.SearchTabVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetButtonsInSearchModeVisibility(e as RibbonEventArgs);
        };
        metadataEditorUserControl.SaveButtonChangeEvent += delegate
        {
            SetTurnAutosaveButtonCaption();
        };
        metadataEditorUserControl.RefreshViewEvent += delegate
        {
            RefreshView();
        };
        metadataEditorUserControl.ShowLoginFormEvent += delegate (object sender, LoginFormPageEnum.LoginFormPage e)
        {
            ShowLoginForm(e);
        };
        MetadataEditorUserControl obj = metadataEditorUserControl;
        obj.OpenRecentEvent = (EventHandler<RecentProject>)Delegate.Combine(obj.OpenRecentEvent, (EventHandler<RecentProject>)delegate (object sender, RecentProject e)
        {
            RunRecentProject(e);
        });
        metadataEditorUserControl.DataLineageTabVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetShowFlowsColumnsButtonVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.DataLineageDiagramVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetZoomLineageDiagramButtonsVisibility((e as BoolEventArgs)?.Value);
        };
        metadataEditorUserControl.DataLineageColumnsVisibilityEvent += delegate (object sender, EventArgs e)
        {
            SetShowFlowsColumnsButtonCaption((e as BoolEventArgs)?.Value);
        };
        ScreenHelper.CenterInCurrentScreen(this);
        userPanelUserControl.OnSignWithDifferentAccount += UserPanelUserControl_OnSignWithDifferentAccount;
        userPanelUserControl.OnChangeLicense += UserPanelUserControl_OnChangeLicense;
        userPanelUserControl.OnLicenseDetails += UserPanelUserControl_OnLicenseDetails;
        userPanelUserControl.OnSignOut += UserPanelUserControl_OnSignOut;
        Ribbon.Manager.ShowScreenTipsInMenus = true;
        Ribbon.ToolTipController = toolTipController;
        ToolTipController.DefaultController.HyperlinkClick += toolTipController_HyperlinkClick;
    }

    private void UserPanelUserControl_OnSignWithDifferentAccount(object sender, EventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            userPopupControlContainer.HidePopup();
            loginForm.StartPosition = FormStartPosition.CenterParent;
            loginForm.SignInWithDifferentAccount();
            loginForm.TopMost = true;
            loginForm.ShowDialog(this);
        }
    }

    private void UserPanelUserControl_OnChangeLicense(object sender, EventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            userPopupControlContainer.HidePopup();
            loginForm.StartPosition = FormStartPosition.CenterParent;
            loginForm.ChangeLicense();
            loginForm.TopMost = true;
            loginForm.ShowDialog(this);
        }
    }

    private void UserPanelUserControl_OnLicenseDetails(object sender, EventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            userPopupControlContainer.HidePopup();
            LicenseDetailsForm.ShowInParentForm(this);
        }
    }

    private void UserPanelUserControl_OnSignOut(object sender, EventArgs e)
    {
        SignOut();
    }

    public void SignOut()
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            userPopupControlContainer.HidePopup();
            DB.DataProfiling.DataProfiling?.Close();
            if (DB.DataProfiling.DataProfiling != null)
            {
                GeneralMessageBoxesHandling.Show("Please close the Data Profiling form before signing out.", "Data Profiling form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
                return;
            }
            StaticData.SignInAfterAfterSignOutInProgress = true;
            SignOutHelper.SignOut();
            Close();
        }
    }

    public void CloseAfterError()
    {
        DB.DataProfiling.DataProfiling?.Close();
        StaticData.ClosingMainFormAfterErrorInProgress = true;
        Close();
    }

    private void SetModulesButtonsVisibility(ObjectTypeEventArgs e)
    {
        sortAlphabeticallyButtonItem.Visibility = (((e == null || e.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database) && (e == null || e.ObjectType != SharedObjectTypeEnum.ObjectType.Module)) ? BarItemVisibility.Never : BarItemVisibility.Always);
        BarButtonItem barButtonItem = moveUpBarButtonItem;
        BarButtonItem barButtonItem2 = moveDownBarButtonItem;
        BarButtonItem barButtonItem3 = moveToTopBarButtonItem;
        BarItemVisibility barItemVisibility2 = (moveToBottomBarButtonItem.Visibility = ((e == null || e.ObjectType != SharedObjectTypeEnum.ObjectType.Module) ? BarItemVisibility.Never : BarItemVisibility.Always));
        BarItemVisibility barItemVisibility4 = (barButtonItem3.Visibility = barItemVisibility2);
        BarItemVisibility barItemVisibility7 = (barButtonItem.Visibility = (barButtonItem2.Visibility = barItemVisibility4));
        SetDesignGroupVisibility();
    }

    private void SetModulesPositionButtonsVisibility(bool? value)
    {
        BarItemVisibility barItemVisibility = ((value != true) ? BarItemVisibility.Never : BarItemVisibility.Always);
        BarButtonItem barButtonItem = moveUpBarButtonItem;
        BarButtonItem barButtonItem2 = moveDownBarButtonItem;
        BarButtonItem barButtonItem3 = moveToTopBarButtonItem;
        BarButtonItem barButtonItem4 = moveToBottomBarButtonItem;
        BarItemVisibility barItemVisibility3 = (sortAlphabeticallyButtonItem.Visibility = barItemVisibility);
        BarItemVisibility barItemVisibility5 = (barButtonItem4.Visibility = barItemVisibility3);
        BarItemVisibility barItemVisibility7 = (barButtonItem3.Visibility = barItemVisibility5);
        BarItemVisibility barItemVisibility10 = (barButtonItem.Visibility = (barButtonItem2.Visibility = barItemVisibility7));
        SetDesignGroupVisibility();
    }

    private void SetAddPrimaryKeyButtonVisibility(bool? value)
    {
        if (value.HasValue)
        {
            addPrimaryKeyBarButtonItem.Visibility = ((!value.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    private void SetAddUniqueKeyButtonVisibility(bool? value)
    {
        if (value.HasValue)
        {
            addUniqueKeyBarButtonItem.Visibility = ((!value.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    private void MainForm_Activated(object sender, EventArgs e)
    {
        CustomMessageBoxForm.DefaultOwner = this;
        base.TopMost = false;
    }

    private void SetERDButtonsVisibility(ErdButtonArgs e)
    {
        if (e.DatabaseClass == SharedDatabaseClassEnum.DatabaseClass.Database)
        {
            addNewTableBarButtonItem.Visibility = BarItemVisibility.Always;
            addNewViewBarButtonItem.Visibility = BarItemVisibility.Always;
            addNewStructureBarButtonItem.Visibility = BarItemVisibility.Always;
        }
        else
        {
            e.Visible = false;
            addNewTableBarButtonItem.Visibility = BarItemVisibility.Never;
            addNewViewBarButtonItem.Visibility = BarItemVisibility.Never;
            addNewStructureBarButtonItem.Visibility = BarItemVisibility.Never;
        }
        if (e.IsRelation)
        {
            editRelationERDBarButtonItem.Hint = ((e.IsUserRelation == true) ? "Edit relationship in the repository" : "Edit relationship display options");
            editRelationERDBarButtonItem.Visibility = ((!e.Visible) ? BarItemVisibility.Never : BarItemVisibility.Always);
            return;
        }
        string text = SharedObjectTypeEnum.TypeToStringForSingle(e.ObjectType);
        designTableBarButtonItem.Caption = "Design " + text;
        designTableBarButtonItem.Hint = "Add or edit " + text + "'s column definition";
        designTableBarButtonItem.LargeGlyph = IconsForButtonsFinder.ReturnImageForDesignButtonItem32(e.ObjectType);
        designTableBarButtonItem.Glyph = IconsForButtonsFinder.ReturnImageForDesignButtonItem16(e.ObjectType);
        editTableBarButtonItem.Caption = "Design " + text;
        editTableBarButtonItem.Hint = "Add or edit " + text + "'s column definition";
        designTableBarButtonItem.Visibility = ((e.ObjectType != SharedObjectTypeEnum.ObjectType.Table && e.ObjectType != SharedObjectTypeEnum.ObjectType.Structure && e.ObjectType != SharedObjectTypeEnum.ObjectType.View) ? BarItemVisibility.Never : BarItemVisibility.Always);
        designTableBarButtonItem.Hint = "Add or edit " + text + "'s column definition";
        BarItemVisibility barItemVisibility3 = (editDisplayOptionsBarButtonItem.Visibility = (editRelationERDBarButtonItem.Visibility = BarItemVisibility.Never));
    }

    private void SetManualItemButtonVisibility(ObjectTypeEventArgs e)
    {
        string text = SharedObjectTypeEnum.TypeToStringForSingle(e?.Node.ContainedObjectsObjectType);
        bool num = e != null && e.Node.ObjectType == SharedObjectTypeEnum.ObjectType.Database && SharedDatabaseClassEnum.MayContainAnyObject(e?.Node.DatabaseClass);
        bool flag = TreeListHelpers.IsObjectOrFolder(e?.Node, SharedObjectTypeEnum.ObjectType.Table);
        bool flag2 = TreeListHelpers.IsObjectOrFolder(e?.Node, SharedObjectTypeEnum.ObjectType.Structure);
        bool flag3 = TreeListHelpers.IsObjectOrFolder(e?.Node, SharedObjectTypeEnum.ObjectType.View);
        bool flag4 = TreeListHelpers.IsObjectOrFolder(e?.Node, SharedObjectTypeEnum.ObjectType.Procedure);
        bool flag5 = TreeListHelpers.IsObjectOrFolder(e?.Node, SharedObjectTypeEnum.ObjectType.Function);
        bool value = num || flag || flag2 || flag3 || flag4 || flag5;
        bool value2 = (e == null || e.Node.Id != 0) && ((e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Table) || (e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) || (e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.View) || (e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure) || (e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Function));
        bool flag6 = (e == null || e.Node.Id != 0) && e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Table;
        addObjectPopupMenu.ClearLinks();
        if (num)
        {
            addUserTableBarButtonItem.Caption = "Add Object";
            addUserTableBarButtonItem.LargeGlyph = Resources.table_add_32;
            addUserTableBarButtonItem.Glyph = Resources.table_add_16;
            addUserTableBarButtonItem.ButtonStyle = BarButtonStyle.DropDown;
            addUserTableBarButtonItem.ActAsDropDown = true;
            if (SharedDatabaseClassEnum.MayContainTableField(e?.Node.DatabaseClass))
            {
                BarButtonItem barButtonItem = new BarButtonItem(addUserTableBarButtonItem.Manager, "Table/Entity");
                barButtonItem.Glyph = Resources.table_add_32;
                barButtonItem.ItemClick += delegate
                {
                    metadataEditorUserControl.AddTableEntity();
                };
                addObjectPopupMenu.AddItem(barButtonItem);
            }
            if (SharedDatabaseClassEnum.MayContainStructure(e?.Node.DatabaseClass))
            {
                BarButtonItem barButtonItem2 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Structure/File");
                barButtonItem2.Glyph = Resources.structure_add_32;
                barButtonItem2.ItemClick += delegate
                {
                    metadataEditorUserControl.AddImportStructure();
                };
                addObjectPopupMenu.AddItem(barButtonItem2);
            }
            if (SharedDatabaseClassEnum.MayContainView(e?.Node.DatabaseClass))
            {
                BarButtonItem barButtonItem3 = new BarButtonItem(addUserTableBarButtonItem.Manager, "View");
                barButtonItem3.Glyph = Resources.view_new_32;
                barButtonItem3.ItemClick += delegate
                {
                    metadataEditorUserControl.AddView();
                };
                addObjectPopupMenu.AddItem(barButtonItem3);
            }
            if (SharedDatabaseClassEnum.MayContainProcedure(e?.Node.DatabaseClass))
            {
                BarButtonItem barButtonItem4 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Procedure");
                barButtonItem4.Glyph = Resources.procedure_new_32;
                barButtonItem4.ItemClick += delegate
                {
                    metadataEditorUserControl.AddProcedure();
                };
                addObjectPopupMenu.AddItem(barButtonItem4);
            }
            if (SharedDatabaseClassEnum.MayContainFunction(e?.Node.DatabaseClass))
            {
                BarButtonItem barButtonItem5 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Function");
                barButtonItem5.Glyph = Resources.function_new_32;
                barButtonItem5.ItemClick += delegate
                {
                    metadataEditorUserControl.AddFunction();
                };
                addObjectPopupMenu.AddItem(barButtonItem5);
            }
        }
        else if (flag)
        {
            addUserTableBarButtonItem.Caption = "Add Table";
            addUserTableBarButtonItem.LargeGlyph = Resources.table_add_32;
            addUserTableBarButtonItem.Glyph = Resources.table_add_16;
            addUserTableBarButtonItem.ButtonStyle = BarButtonStyle.DropDown;
            addUserTableBarButtonItem.ActAsDropDown = false;
            BarButtonItem barButtonItem6 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Design Manually");
            barButtonItem6.Glyph = Resources.design_table_32;
            barButtonItem6.ItemClick += delegate
            {
                metadataEditorUserControl.AddUserTable(null, SharedObjectTypeEnum.ObjectType.Table);
            };
            BarButtonItem barButtonItem7 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Paste JSON Document (NoSQL Collection)");
            barButtonItem7.Glyph = Resources.paste_document_32;
            barButtonItem7.ItemClick += delegate
            {
                metadataEditorUserControl.PasteDocument(null, SharedObjectTypeEnum.ObjectType.Table);
            };
            BarButtonItem barButtonItem8 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Import JSON File (NoSQL Collection)");
            barButtonItem8.Glyph = Resources.import_from_file_32;
            barButtonItem8.ItemClick += delegate
            {
                metadataEditorUserControl.ImportFromFile(null, SharedObjectTypeEnum.ObjectType.Table);
            };
            BarButtonItem barButtonItem9 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Bulk Add Tables");
            barButtonItem9.Glyph = Resources.bulk_add_tables_32;
            barButtonItem9.ItemClick += delegate
            {
                metadataEditorUserControl.BulkAddTables(SharedObjectTypeEnum.ObjectType.Table);
            };
            addObjectPopupMenu.AddItem(barButtonItem6);
            addObjectPopupMenu.AddItem(barButtonItem7);
            addObjectPopupMenu.AddItem(barButtonItem8);
            addObjectPopupMenu.AddItem(barButtonItem9);
            editTableBarButtonItem.LargeGlyph = Resources.design_table_32;
            editTableBarButtonItem.Glyph = Resources.design_table_16;
        }
        else if (flag2)
        {
            addUserTableBarButtonItem.Caption = "Add Structure/File";
            addUserTableBarButtonItem.LargeGlyph = Resources.structure_add_32;
            addUserTableBarButtonItem.Glyph = Resources.structure_add_16;
            addUserTableBarButtonItem.ButtonStyle = BarButtonStyle.DropDown;
            addUserTableBarButtonItem.ActAsDropDown = false;
            BarButtonItem barButtonItem10 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Paste document");
            barButtonItem10.Glyph = Resources.paste_document_32;
            barButtonItem10.ItemClick += delegate
            {
                metadataEditorUserControl.PasteDocument(null, SharedObjectTypeEnum.ObjectType.Structure);
            };
            BarButtonItem barButtonItem11 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Import from File");
            barButtonItem11.Glyph = Resources.import_from_file_32;
            barButtonItem11.ItemClick += delegate
            {
                metadataEditorUserControl.ImportFromFile(null, SharedObjectTypeEnum.ObjectType.Structure);
            };
            BarButtonItem barButtonItem12 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Design Manually");
            barButtonItem12.Glyph = Resources.design_structure_32;
            barButtonItem12.ItemClick += delegate
            {
                metadataEditorUserControl.AddUserTable(null, SharedObjectTypeEnum.ObjectType.Structure);
            };
            BarButtonItem barButtonItem13 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Bulk Add Structures");
            barButtonItem13.Glyph = Resources.bulk_add_structures_32;
            barButtonItem13.ItemClick += delegate
            {
                metadataEditorUserControl.BulkAddTables(SharedObjectTypeEnum.ObjectType.Structure);
            };
            addObjectPopupMenu.AddItem(barButtonItem12);
            addObjectPopupMenu.AddItem(barButtonItem10);
            addObjectPopupMenu.AddItem(barButtonItem11);
            addObjectPopupMenu.AddItem(barButtonItem13);
            editTableBarButtonItem.LargeGlyph = Resources.design_structure_32;
            editTableBarButtonItem.Glyph = Resources.design_structure_16;
        }
        else if (flag3)
        {
            addUserTableBarButtonItem.Caption = "Add View";
            addUserTableBarButtonItem.LargeGlyph = Resources.view_new_32;
            addUserTableBarButtonItem.Glyph = Resources.view_new_16;
            addUserTableBarButtonItem.ButtonStyle = BarButtonStyle.DropDown;
            addUserTableBarButtonItem.ActAsDropDown = false;
            BarButtonItem barButtonItem14 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Design Manually");
            barButtonItem14.Glyph = Resources.view_new_32;
            barButtonItem14.ItemClick += delegate
            {
                metadataEditorUserControl.AddView();
            };
            BarButtonItem barButtonItem15 = new BarButtonItem(addUserTableBarButtonItem.Manager, "Bulk Add Views");
            barButtonItem15.Glyph = Resources.bulk_add_views_32;
            barButtonItem15.ItemClick += delegate
            {
                metadataEditorUserControl.BulkAddTables(SharedObjectTypeEnum.ObjectType.View);
            };
            addObjectPopupMenu.AddItem(barButtonItem14);
            addObjectPopupMenu.AddItem(barButtonItem15);
            editTableBarButtonItem.LargeGlyph = Resources.design_view_32;
            editTableBarButtonItem.Glyph = Resources.design_view_16;
        }
        else if (flag4)
        {
            addUserTableBarButtonItem.Caption = "Add Procedure";
            addUserTableBarButtonItem.LargeGlyph = Resources.procedure_new_32;
            addUserTableBarButtonItem.Glyph = Resources.procedure_new_16;
            addUserTableBarButtonItem.ButtonStyle = BarButtonStyle.Default;
            addUserTableBarButtonItem.ActAsDropDown = false;
            editTableBarButtonItem.LargeGlyph = Resources.design_procedure_32;
            editTableBarButtonItem.Glyph = Resources.design_procedure_16;
        }
        else if (flag5)
        {
            addUserTableBarButtonItem.Caption = "Add Function";
            addUserTableBarButtonItem.LargeGlyph = Resources.function_new_32;
            addUserTableBarButtonItem.Glyph = Resources.function_new_16;
            addUserTableBarButtonItem.ButtonStyle = BarButtonStyle.Default;
            addUserTableBarButtonItem.ActAsDropDown = false;
            editTableBarButtonItem.LargeGlyph = Resources.design_function_32;
            editTableBarButtonItem.Glyph = Resources.design_function_16;
        }
        SetAddManualTableVisibility(value);
        SetDesignTableButtonVisibility((e == null || e.Node.Id != 0) && e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Table);
        SetDesignTableButtonVisibility(value2);
        if (!DB.DataProfiling.IsDataProfilingDisabled() && e != null && (e == null || e.Node.Id != 0) && DataProfilingUtils.ObjectCanBeProfilled(e?.ObjectType) && DataProfilingUtils.NodeCanBeProfilled(e?.Node))
        {
            SetTableProfilingButtonsVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: true, e?.ObjectType, dataProfilingButtonVisible: false));
        }
        else if (!DB.DataProfiling.IsDataProfilingDisabled() && e != null && (e == null || e.Node.Id != 0) && e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Database && (e == null || e.DatabaseType != SharedDatabaseTypeEnum.DatabaseType.Manual))
        {
            SetTableProfilingButtonsVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: false, null, dataProfilingButtonVisible: true));
        }
        else if (!DB.DataProfiling.IsDataProfilingDisabled() && e != null && (e == null || e.Node.Id != 0) && DataProfilingUtils.NodeFolderCanBeProfilled(e?.Node))
        {
            SetTableProfilingButtonsVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: false, null, dataProfilingButtonVisible: true));
        }
        else
        {
            SetTableProfilingButtonsVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: false, null, dataProfilingButtonVisible: false));
        }
        profileColumnbarButtonItem.Visibility = BarItemVisibility.Never;
        DB.Table.HasMultipleLevelColumns(e?.Node.Id ?? 0);
        editTableBarButtonItem.Caption = "Design " + text;
        editTableBarButtonItem.Hint = "Add or edit " + text + "'s column definition";
        if (!flag6)
        {
            addUniqueKeyBarButtonItem.Visibility = BarItemVisibility.Never;
            addPrimaryKeyBarButtonItem.Visibility = BarItemVisibility.Never;
        }
        SetDesignGroupVisibility();
    }

    private void addUserTableBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (!addUserTableBarButtonItem.ActAsDropDown)
        {
            metadataEditorUserControl.AddObject();
        }
    }

    private void SetProfileColumnButtonVisibility(bool? showButton)
    {
        profileColumnbarButtonItem.Visibility = ((!showButton.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
    }

    private void SetTableProfilingButtonsVisibility(ProfilingVisibilityEventArgs args)
    {
        dataProfilingBarButtonItem.Visibility = ((!args.DataProfilingButtonVisible) ? BarItemVisibility.Never : BarItemVisibility.Always);
        profileTablebarButtonItem.Caption = DataProfilingUtils.GetButtonNameByObjectType(args.SingleObjectType);
        profileTablebarButtonItem.Visibility = ((!args.SingleTableButtonsVisible) ? BarItemVisibility.Never : BarItemVisibility.Always);
        previewSampleDataBarButtonItem.Visibility = ((!args.SingleTableButtonsVisible) ? BarItemVisibility.Never : BarItemVisibility.Always);
    }

    public void LoadUpdateBars(bool hideWaitForm)
    {
        ResizeMainPanel();
        HideUpdateBars();
        ShowHideTableLayoutUpdatePanel();
        if (hideWaitForm)
        {
            CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
        }
    }

    private void SetDropDownMenu()
    {
        databasePopUpMenu.ClearLinks();
        BarButtonItem barButtonItem = new BarButtonItem(databasePopUpMenu.Manager, "New connection", 0);
        barButtonItem.ItemClick += addDatabaseButton_ItemClick;
        BarButtonItem barButtonItem2 = new BarButtonItem(databasePopUpMenu.Manager, "Manual database", 1);
        barButtonItem2.ItemClick += manualDatabaseDropDownMenu_ItemClick;
        BarButtonItem barButtonItem3 = new BarButtonItem(databasePopUpMenu.Manager, "Business Glossary", 2);
        barButtonItem3.ItemClick += BusinessGlossary_ItemClick;
        BarItem[] items = new BarItem[3] { barButtonItem, barButtonItem2, barButtonItem3 };
        databasePopUpMenu.Manager.Images = databasesImageCollection;
        databasePopUpMenu.AddItems(items);
    }

    public void ResizeMainPanel()
    {
        mainPanel.Location = new Point(0, mainRibbonControl.Height + 1);
        if (majorUpdatePanelControl.Visible && minorUpdatePanelControl.Visible)
        {
            mainPanel.MaximumSize = new Size(mainPanel.Width, mainPanel.Height + majorUpdatePanelControl.Height + minorUpdatePanelControl.Height);
            mainPanel.Size = mainPanel.MaximumSize;
        }
        else if (majorUpdatePanelControl.Visible)
        {
            mainPanel.MaximumSize = new Size(mainPanel.Width, mainPanel.Height + majorUpdatePanelControl.Height);
            mainPanel.Size = mainPanel.MaximumSize;
        }
        else if (minorUpdatePanelControl.Visible)
        {
            mainPanel.MaximumSize = new Size(mainPanel.Width, mainPanel.Height + minorUpdatePanelControl.Height);
            mainPanel.Size = mainPanel.MaximumSize;
        }
    }

    public void HideUpdateBars()
    {
        majorUpdatePanelControl.Visible = false;
        minorUpdatePanelControl.Visible = false;
    }

    public async void ShowHideTableLayoutUpdatePanel()
    {
        mainPanel.Location = new Point(0, mainRibbonControl.Height);
        UpdateBarReader readUpdateBarAvailability = new UpdateBarReader();
        await readUpdateBarAvailability.SetVersions();
        bool flag = readUpdateBarAvailability.CheckMajorUpdateAvailability();
        bool flag2 = readUpdateBarAvailability.CheckMinorUpdateAvailability();
        bool flag3 = readUpdateBarAvailability.CheckIfRepositoryUpgradeIsNeeded();
        try
        {
            string text = string.Empty;
            if (flag3)
            {
                text = "The version requires repository upgrade.";
            }
            majorUpdateLabel.Text = "New major version is available for download: " + readUpdateBarAvailability.GetMajorVersionString() + ". " + text;
            minorUpdateLabel.Text = readUpdateBarAvailability.GetMinorVersionString() + " update is available. " + text;
            if (flag2)
            {
                minorUpdatePanelControl.Appearance.BackColor = SkinsManager.CurrentSkin.UpdatePanelBackColor;
                minorUpdatePanelControl.Appearance.ForeColor = SkinsManager.CurrentSkin.UpdatePanelForeColor;
                closeMajorUpdatePanelButton.Appearance.BackColor = SkinsManager.CurrentSkin.UpdatePanelBackColor;
            }
            if (flag)
            {
                majorUpdatePanelControl.Appearance.BackColor = SkinsManager.CurrentSkin.UpdatePanelBackColor;
                majorUpdatePanelControl.Appearance.ForeColor = SkinsManager.CurrentSkin.UpdatePanelForeColor;
                closeMinorUpdatePanelButton.Appearance.BackColor = SkinsManager.CurrentSkin.UpdatePanelBackColor;
            }
            if (flag && flag2)
            {
                minorUpdatePanelControl.Visible = true;
                majorUpdatePanelControl.Visible = true;
            }
            else if (flag)
            {
                majorUpdatePanelControl.Location = new Point(0, mainRibbonControl.Size.Height);
                majorUpdatePanelControl.Visible = true;
            }
            else if (flag2)
            {
                minorUpdatePanelControl.Location = new Point(0, mainRibbonControl.Size.Height);
                minorUpdatePanelControl.Visible = true;
            }
        }
        catch
        {
            HideUpdateBars();
        }
    }

    private void SetERDGroupVisibility(ObjectTypeEventArgs e)
    {
        erdRibbonPageGroup.Visible = false;
    }

    private void SetERDGroupVisibility(bool? visible)
    {
        erdRibbonPageGroup.Visible = visible.Value;
    }

    private void SetAddRelationButtonVisibility(bool? value)
    {
        if (value.HasValue)
        {
            addRelationBarButtonItem.Visibility = ((!value.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    public void SetUserObjectsButtonsEnabled()
    {
        customFieldsBarButtonItem.Enabled = Functionalities.HasFunctionality(FunctionalityEnum.Functionality.CustomFields) || (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification) && DB.Classificator.GetClassificators().Any());
    }

    private void SetEditRelationButtonVisibility(bool? value)
    {
        if (value.HasValue)
        {
            editRelationBarButtonItem.Visibility = ((!value.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    private void SetEditConstraintButtonVisibility(bool? value)
    {
        if (value.HasValue)
        {
            editConstraintBarButtonItem.Visibility = ((!value.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    private void SetBarButtonAfterSynchronize()
    {
        if (metadataEditorUserControl.GetFocusedNode() == null)
        {
            synchronizeBarButtonItem.Caption = string.Empty;
        }
        else
        {
            synchronizeBarButtonItem.Caption = "Import changes";
        }
    }

    private void SetAddRelatedTermButtonVisibility(bool? value)
    {
        if (value.HasValue)
        {
            addRelatedTermBarButtonItem.Visibility = ((!value.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    private void SetAddDataLinksButtonVisibility(bool? value)
    {
        if (value.HasValue)
        {
            addDataLinkBarButtonItem.Visibility = ((!value.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    private void SetButtonsInSearchModeVisibility(RibbonEventArgs e)
    {
        if (e.Value.HasValue)
        {
            isSearchActive = e.Value.Value;
            SetDesignGroupVisibility();
        }
    }

    private void SetAddManualTableVisibility(bool? showButton)
    {
        if (showButton.HasValue)
        {
            addUserTableBarButtonItem.Visibility = ((!showButton.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
        }
    }

    private void SetDesignTableButtonVisibility(bool? showButton)
    {
        editTableBarButtonItem.Visibility = ((!showButton.Value) ? BarItemVisibility.Never : BarItemVisibility.Always);
    }

    private void CreateProgressMenu()
    {
        progressPopupMenu.Manager.ShowScreenTipsInMenus = true;
        progressPopupMenu.ClearLinks();
        BarButtonItem barButtonItem = new BarButtonItem(progressPopupMenu.Manager, "All descriptions");
        barButtonItem.Glyph = Resources.progress_32;
        barButtonItem.ItemClick += allDocsProgressType_Click;
        barButtonItem.Hint = "% of descriptions entered";
        BarButtonItem barButtonItem2 = new BarButtonItem(progressPopupMenu.Manager, "Tables && columns");
        barButtonItem2.Glyph = Resources.progress_32;
        barButtonItem2.ItemClick += tablesAndColumnsProgressType_Click;
        barButtonItem2.Hint = "% of tables and columns descriptions entered";
        progressPopupMenu.AddItem(barButtonItem);
        progressPopupMenu.AddItem(barButtonItem2);
        IOrderedEnumerable<CustomFieldRowExtended> orderedEnumerable = from x in (from x in DB.CustomField.GetCustomFields(Licence.GetCustomFieldsLimit(), false)
                                                                                  select new CustomFieldRowExtended(x)).ToList()
                                                                       orderby x.Title
                                                                       select x;
        foreach (CustomFieldRowExtended item in orderedEnumerable)
        {
            BarButtonItem barButtonItem3 = new BarButtonItem(progressPopupMenu.Manager, Escaping.EscapeTextForUI(item.Title));
            barButtonItem3.Tag = item;
            barButtonItem3.Glyph = Resources.progress_32;
            barButtonItem3.ItemClick += customFieldProgressType_Click;
            barButtonItem3.Hint = "% of " + Escaping.EscapeTextForUI(item.Title) + " entered";
            progressPopupMenu.AddItem(barButtonItem3).BeginGroup = item.Equals(orderedEnumerable.FirstOrDefault());
        }
    }

    public void SetFunctionality()
    {
        Text = ProgramVersion.DisplayNameWithFullVersion;
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "build-revision");
        if (File.Exists(path))
        {
            Text = Text + " - " + File.ReadAllText(path).Trim();
        }
        addNewTableBarButtonItem.Hint = "Add new table to repository and diagram";
        showHideAvailableEntitiesBarButtonItem.Hint = string.Empty;
        if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.CustomFields))
        {
            customFieldsBarButtonItem.Enabled = true;
            customFieldsBarButtonItem.SuperTip = null;
        }
        else
        {
            customFieldsBarButtonItem.Enabled = false;
            customFieldsBarButtonItem.SuperTip = Functionalities.GetUnavailableActionToolTip(FunctionalityEnum.Functionality.CustomFields);
        }
        SetClassificatorVisibility();
        SetDropDownMenu();
        SetAddTermDropDownMenu(loadTermTypes: true);
        SetRecentDropDownMenu();
        AdjustWindowSize();
        SetTurnAutosaveButtonCaption();
        userPanelUserControl.SetParameters();
        userBarButtonItem.Caption = UserDataService.GetUsernameText();
        DB.DataProfiling.SetSelectKillerSwitchMode();
        DB.History.SetSelectKillerSwitchMode();
    }

    private void turnAutosaveBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        LastConnectionInfo.SetAutosave(!LastConnectionInfo.LOGIN_INFO.Autosave);
        SetTurnAutosaveButtonCaption();
    }

    private void SetTurnAutosaveButtonCaption()
    {
        turnAutosaveBarButtonItem.Caption = (LastConnectionInfo.LOGIN_INFO.Autosave ? "Turn autosave off" : "Turn autosave on");
        SetSaveCaption();
    }

    private void SetSaveCaption()
    {
        saveButton.Caption = (LastConnectionInfo.LOGIN_INFO.Autosave ? ("Save" + Environment.NewLine + "(autosave on)") : ("Save" + Environment.NewLine + "(autosave off)"));
    }

    private void SetClassificatorVisibility()
    {
        bool enabled;
        string hint;
        if (StaticData.IsProjectFile)
        {
            enabled = false;
            hint = "Find and classify columns containing sensitive data" + Environment.NewLine + "Requires a server repository to work. <href=" + Links.CreatingServerRepository + ">Learn more</href>";
        }
        else
        {
            enabled = true;
            hint = "Find and classify columns containing sensitive data";
        }
        classificationBarButtonItem.Enabled = enabled;
        classificationBarButtonItem.Hint = hint;
    }

    public void RefreshControls()
    {
        metadataEditorUserControl.LoadUserControls();
        ResizeMainPanel();
        majorUpdatePanelControl.Visible = false;
        minorUpdatePanelControl.Visible = false;
        ShowHideTableLayoutUpdatePanel();
    }

    public void RefreshDataSource()
    {
        ShowProgress(show: false);
        metadataEditorUserControl.CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
        metadataEditorUserControl.BusinessGlossarySupport.LoadTermRelationshipTypes();
        metadataEditorUserControl.RefreshTree(showWaitForm: false, refreshProgress: false, rememberSelectedNode: false);
        metadataEditorUserControl.SetBannerFunctionality();
        metadataEditorUserControl.RebuildHomePage(forceReload: true);
        SetRecentDropDownMenu();
    }

    private void ChangeDeleteButtonIcon(ObjectTypeEventArgs e)
    {
        if (e != null)
        {
            if (e.ObjectType == SharedObjectTypeEnum.ObjectType.Database || e.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary || e.ObjectType == SharedObjectTypeEnum.ObjectType.Term || e.ObjectType == SharedObjectTypeEnum.ObjectType.Module || e.ObjectType == SharedObjectTypeEnum.ObjectType.Function || e.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || e.ObjectType == SharedObjectTypeEnum.ObjectType.Table || e.ObjectType == SharedObjectTypeEnum.ObjectType.View || e.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
            {
                SharedObjectTypeEnum.TypeToStringForSingleLower(e.ObjectType);
                SetSaveCaption();
                saveButton.Hint = "Save " + SharedObjectTypeEnum.TypeToStringForSaveButton(e.ObjectType) + " (ctrl+S)";
                saveButton.Visibility = BarItemVisibility.Always;
            }
            else
            {
                saveButton.Visibility = BarItemVisibility.Never;
            }
            SetDesignGroupVisibility();
            bool flag = e.ObjectType == SharedObjectTypeEnum.ObjectType.Database || e.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database || e.ObjectType == SharedObjectTypeEnum.ObjectType.Module;
            addModuleBarButtonItem.Visibility = ((!flag) ? BarItemVisibility.Never : BarItemVisibility.Always);
            bool flag2 = (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && e.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary) || (e.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && e != null && e.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary) || e.ObjectType == SharedObjectTypeEnum.ObjectType.Term;
            addTermBarButtonItem.Visibility = ((!flag2) ? BarItemVisibility.Never : BarItemVisibility.Always);
            bool flag3 = e.ObjectType == SharedObjectTypeEnum.ObjectType.Term;
            addRelatedTermBarButtonItem.Visibility = ((!flag3) ? BarItemVisibility.Never : BarItemVisibility.Always);
            bool flag4 = e.ObjectType == SharedObjectTypeEnum.ObjectType.Term;
            addDataLinkBarButtonItem.Visibility = ((!flag4) ? BarItemVisibility.Never : BarItemVisibility.Always);
            if (e.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
            {
                addDataLinkBarButtonItem.Caption = "Edit data links";
                addDataLinkBarButtonItem.Hint = "Edit links to Data Dictionary elements";
            }
            else
            {
                addDataLinkBarButtonItem.Caption = "Edit links to Glossary";
                addDataLinkBarButtonItem.Hint = "Edit links to Business Glossary terms";
            }
        }
    }

    private void SetDesignGroupVisibility()
    {
        designRibbonPageGroup.Visible = designRibbonPageGroup.ItemLinks.Any((BarItemLink x) => x.Item.Visibility == BarItemVisibility.Always);
    }

    private void SetImportExportGroupVisibility()
    {
        importExportRibbonPageGroup.Visible = importExportRibbonPageGroup.ItemLinks.Any((BarItemLink x) => x.Item.Visibility == BarItemVisibility.Always);
    }

    private void ShowHideTableButtons(ObjectTypeEventArgs e)
    {
        if ((e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.Table) || (e != null && e.ObjectType == SharedObjectTypeEnum.ObjectType.View))
        {
            metadataEditorUserControl.ShowRelationAndConstraintButtons();
            metadataEditorUserControl.ShowDataLinksButtons();
        }
        else
        {
            addRelationBarButtonItem.Visibility = BarItemVisibility.Never;
        }
    }

    private void ShowHideButtonsForDocumentation(ObjectTypeEventArgs e)
    {
        bool flag = e?.Available ?? false;
        bool flag2 = e != null && e.DatabaseId == 0;
        bool flag3 = e == null;
        synchronizeBarButtonItem.Caption = SharedDatabaseTypeEnum.SynchronizeText(e?.Node?.DatabaseType);
        synchronizeBarButtonItem.Visibility = ((!flag || flag3 || flag2 || e.IsBusinessGlossary || string.IsNullOrEmpty(synchronizeBarButtonItem.Caption)) ? BarItemVisibility.Never : BarItemVisibility.Always);
        DocCreatorBarButtonItem.Visibility = ((!flag || flag3 || (flag2 && e.ObjectType != SharedObjectTypeEnum.ObjectType.Repository)) ? BarItemVisibility.Never : BarItemVisibility.Always);
        SetImportExportGroupVisibility();
        BarItemVisibility barItemVisibility3 = (searchButtonItem.Visibility = (showProgressBarButtonItem.Visibility = (flag3 ? BarItemVisibility.Never : BarItemVisibility.Always)));
        SetDesignGroupVisibility();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        mainRibbonControl.Manager.UseAltKeyForMenu = false;
        CommonFunctionsPanels.ShowUserControlInPanel(metadataEditorUserControl, mainPanel);
        majorUpdatePanelControl.Visible = false;
        minorUpdatePanelControl.Visible = false;
        ShowHideTableLayoutUpdatePanel();
        Paths.CheckAndSaveDefaultDocumentationPath();
        SetUserObjectsButtonsEnabled();
        SetSuggestedDescriptionsByLicense();
        SetDropDownMenu();
    }

    private void manualDatabaseDropDownMenu_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            metadataEditorUserControl.AddManualDatabaseBarButtonItem_ItemClick(sender, e);
        }
    }

    private void BusinessGlossary_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            metadataEditorUserControl.AddBusinessGlossaryBarButtonItem_ItemClick(sender, e);
        }
    }

    private void refreshMenuButton_ItemClick(object sender, ItemClickEventArgs e)
    {
        RefreshView();
    }

    private void RefreshView()
    {
        RefreshTree(showWaitForm: true, metadataEditorUserControl.ShowProgress);
        SetUserObjectsButtonsEnabled();
        SetSuggestedDescriptionsByLicense();
        metadataEditorUserControl.SetBannerFunctionality();
        metadataEditorUserControl.ForceRefreshCustomFieldsControlsOnNextLoading();
        metadataEditorUserControl.RebuildHomePage(forceReload: false);
        SetAddTermDropDownMenu(loadTermTypes: true);
    }

    private void ShowLoginForm(LoginFormPageEnum.LoginFormPage loginFormPage)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            loginForm.StartPosition = FormStartPosition.CenterParent;
            switch (loginFormPage)
            {
                case LoginFormPageEnum.LoginFormPage.ConnectToRepository:
                    loginForm.OpenConnectToRepositoryPage();
                    break;
                case LoginFormPageEnum.LoginFormPage.CreateNewRepository:
                    loginForm.OpenCreateRepositoryPage();
                    break;
            }
            loginForm.TopMost = true;
            loginForm.ShowDialog(this);
        }
    }

    private Point CenterForm()
    {
        CenterToScreen();
        return base.Location;
    }

    private void AdjustWindowSize()
    {
        try
        {
            if (!base.Visible)
            {
                base.WindowState = FormWindowState.Normal;
                Rectangle workingArea = Screen.GetWorkingArea(this);
                Point? windowLocation = ConfigurationFileHelper.GetWindowLocation();
                Size windowSize = ConfigurationFileHelper.GetWindowSize();
                bool flag = workingArea.Width < windowSize.Width || workingArea.Height < windowSize.Height;
                base.Size = (flag ? new Size(workingArea.Width, workingArea.Height) : windowSize);
                if (!windowLocation.HasValue)
                {
                    base.Location = CenterForm();
                }
                else
                {
                    base.Location = (flag ? base.RestoreBounds.Location : windowLocation.Value);
                }
                base.WindowState = FormWindowState.Maximized;
            }
        }
        catch (Exception)
        {
        }
    }

    private bool RefreshTree(bool showWaitForm, bool refreshProgress)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            metadataEditorUserControl.RefreshTree(showWaitForm, refreshProgress);
            return true;
        }
        return false;
    }

    private void synchronizeBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            if (metadataEditorUserControl.ContinueAfterPossibleChanges())
            {
                metadataEditorUserControl.Synchronize();
            }
        }
        catch (Exception exception)
        {
            GeneralExceptionHandling.Handle(exception, null, this);
        }
    }

    private void DocCreatorBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            metadataEditorUserControl.ShowDocCreator();
        }
    }

    private void addDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddDatabaseConnectionBarButtonItem_ItemClick(sender, e);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        try
        {
            switch (keyData)
            {
                case Keys.N | Keys.Control:
                    metadataEditorUserControl.AddDatabase();
                    break;
                case Keys.F5:
                    RefreshTree(showWaitForm: true, metadataEditorUserControl.ShowProgress);
                    break;
            }
        }
        catch (Exception exception)
        {
            GeneralExceptionHandling.Handle(exception, null, this);
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void saveButton_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.Save();
    }

    private void aboutBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            new AboutForm().ShowDialog(this);
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.ApplicationExitCall)
        {
            e.Cancel = !metadataEditorUserControl.ContinueAfterPossibleChanges();
        }
        _ = base.WindowState;
        if (base.WindowState == FormWindowState.Normal)
        {
            LastConnectionInfo.SetWindowSize(base.Size);
            LastConnectionInfo.SetWindowLocation(base.Location);
        }
        else
        {
            LastConnectionInfo.SetWindowSize(base.RestoreBounds.Size);
            LastConnectionInfo.SetWindowLocation(base.RestoreBounds.Location);
        }
        metadataEditorUserControl.SaveUserColumns();
    }

    private void addModuleBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddModule();
    }

    private void mainRibbonControl_ShowCustomizationMenu(object sender, RibbonCustomizationMenuEventArgs e)
    {
        foreach (BarItemLink itemLink in e.CustomizationMenu.ItemLinks)
        {
            if (itemLink.Caption.Equals(BarLocalizer.Active.GetLocalizedString(BarString.RibbonToolbarMinimizeRibbon)))
            {
                itemLink.Visible = false;
                break;
            }
        }
    }

    private void documentationXtraReportBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ShowDocXtraReportForm();
    }

    private void searchButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ShowSearchPanel();
    }

    private void showProgressBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        bool show = showProgressBarButtonItem.Caption == "Show progress";
        ShowProgress(show);
    }

    private void ShowProgress(bool show)
    {
        metadataEditorUserControl.ShowProgressColumn(showWaitForm: true, show);
        showProgressBarButtonItem.Caption = (show ? "Hide progress" : "Show progress");
    }

    private void newOpenBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            loginForm.StartPosition = FormStartPosition.CenterParent;
            loginForm.SetStartPageAfterSignIn();
            loginForm.TopMost = true;
            loginForm.ShowDialog(this);
        }
    }

    private void addRelationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddRelationHandler();
    }

    private void editRelationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.EditRelationHandler();
    }

    private void editConstraintBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.EditConstraintHandler();
    }

    private void customFieldsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            if (metadataEditorUserControl.ContinueAfterPossibleChanges())
            {
                metadataEditorUserControl.EditCustomFields();
            }
        }
        catch (Exception exception)
        {
            GeneralExceptionHandling.Handle(exception, null, this);
        }
    }

    private async void updateNowUpdatePanelControlButton_Click(object sender, EventArgs e)
    {
        UpdateBarReader update = new UpdateBarReader();
        await update.SetVersions();
        Links.OpenLink(update.MajorVersionLink, this);
    }

    private async void downloadMinorVersionButton_Click(object sender, EventArgs e)
    {
        UpdateBarReader update = new UpdateBarReader();
        await update.SetVersions();
        Links.OpenLink(update.MinorVersionLink, this);
    }

    private void closeMinorUpdateButton_Click(object sender, EventArgs e)
    {
        minorUpdatePanelControl.Visible = false;
        mainPanel.Size = new Size(mainPanel.Size.Width, mainPanel.MaximumSize.Height);
    }

    private void closeMajorUpdateButton_Click(object sender, EventArgs e)
    {
        majorUpdatePanelControl.Visible = false;
        mainPanel.Size = new Size(mainPanel.Size.Width, mainPanel.MaximumSize.Height);
    }

    private void EditTableBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        DBTreeNode focusedNode = metadataEditorUserControl.GetFocusedNode();
        if ((focusedNode != null && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Function) || (focusedNode != null && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure))
        {
            metadataEditorUserControl.EditProcedure();
        }
        else
        {
            metadataEditorUserControl.EditTable();
        }
    }

    private void ProfileTableBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ProfileSingleTable();
    }

    private void DataProfilingBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ProfileMultipleTables();
    }

    private void PreviewSampleDataBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.PreviewSampleData();
    }

    private void ProfileColumnBarButtonItemAsync_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ProfileSingleColumn();
    }

    private void designTableBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.DesignSelectedObject();
    }

    private void editDisplayOptionsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.EditDisplayOptionsOnERD();
    }

    private void editRelationERDBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.EditSelectedERDLink();
    }

    private void showHideSuggestedEntitiesBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        string text3 = (showHideAvailableEntitiesBarButtonItem.Caption = (showHideAvailableEntitiesBarButtonItem.Caption = (metadataEditorUserControl.ShowSuggestedEntities() ? "Hide available entities" : "Show available entities")));
    }

    private void progressPopupMenu_BeforePopup(object sender, CancelEventArgs e)
    {
        CreateProgressMenu();
    }

    private void allDocsProgressType_Click(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ProgressType = new ProgressTypeModel(ProgressTypeEnum.AllDocumentations, "Descriptions");
        metadataEditorUserControl.ProgressType.FieldName = "Description";
        ShowProgress(show: true);
    }

    private void tablesAndColumnsProgressType_Click(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ProgressType = new ProgressTypeModel(ProgressTypeEnum.TablesAndColumns, "Table descriptions");
        metadataEditorUserControl.ProgressType.FieldName = "Description";
        ShowProgress(show: true);
    }

    private void customFieldProgressType_Click(object sender, ItemClickEventArgs e)
    {
        CustomFieldRowExtended customFieldRowExtended = e.Item.Tag as CustomFieldRowExtended;
        metadataEditorUserControl.ProgressType = new ProgressTypeModel(ProgressTypeEnum.SelectedCustomField, customFieldRowExtended.Title);
        metadataEditorUserControl.ProgressType.CustomFieldId = customFieldRowExtended.CustomFieldId;
        metadataEditorUserControl.ProgressType.FieldName = customFieldRowExtended.FieldName;
        metadataEditorUserControl.ProgressType.CustomField = customFieldRowExtended;
        ShowProgress(show: true);
    }

    public void SetSuggestedDescriptionsByLicense()
    {
        metadataEditorUserControl.ShowSuggestions = true;
        suggestedDescriptionBarButtonItem.Caption = "Hide suggestions";
        suggestedDescriptionBarButtonItem.Hint = "Displays title, description and custom field suggestions based on other objects with the same name";
        suggestedDescriptionBarButtonItem.Enabled = true;
    }

    private void hintsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        suggestedDescriptionBarButtonItem.Caption = (metadataEditorUserControl.ShowSuggestions ? "Show suggestions" : "Hide suggestions");
        metadataEditorUserControl.ShowSuggestions = !metadataEditorUserControl.ShowSuggestions;
        metadataEditorUserControl?.GetSuggestedDescriptions?.Invoke(null, null);
    }

    private void chatBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.GetChatLink(ProgramVersion.VersionWithBuildForUrl), this);
    }

    private void AddRelatedTermBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.BusinessGlossarySupport.StartAddingTermRelatedTerm(metadataEditorUserControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, this);
    }

    private void AddDataLinkBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.DataLinksSupport.StartAddingDataLink(metadataEditorUserControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, this);
    }

    private void AddTermBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.BusinessGlossarySupport.StartAddingTerm(metadataEditorUserControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, e.Item.Tag as TermTypeObject, this);
    }

    public void SetRecentDropDownMenu()
    {
        List<RecentProject> list = RecentProjectsHelper.GetList(ProgramVersion.Major, SkinsManager.CurrentSkin.ListSelectionForeColor);
        newOpenBarButtonItem.ButtonStyle = ((list.Count > 0) ? BarButtonStyle.DropDown : BarButtonStyle.Default);
        recentPopupMenu.ClearLinks();
        PopupMenu popupMenu = recentPopupMenu;
        BarItem[] items = list.Take(5).Select(delegate (RecentProject x)
        {
            BarButtonItem barButtonItem = new BarButtonItem(recentPopupMenu.Manager, x.DisplayNameShort, 0);
            barButtonItem.Tag = x;
            barButtonItem.Glyph = ((x.Type == RepositoryType.File) ? Resources.file_repository_32 : Resources.server_repository_32);
            barButtonItem.ItemClick += delegate
            {
                RunRecentProject(x);
            };
            return barButtonItem;
        }).ToArray();
        popupMenu.AddItems(items);
    }

    private void RunRecentProject(RecentProject project)
    {
        if (!metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            return;
        }
        try
        {
            CommonFunctionsDatabase.SetWaitFormVisibility(metadataEditorUserControl.SplashScreenManager, show: true);
            if (loginForm.OpenRecent(project))
            {
                SetRecentDropDownMenu();
            }
        }
        catch (Exception)
        {
            RefreshDataSource();
            SetRecentDropDownMenu();
            throw;
        }
        finally
        {
            CommonFunctionsDatabase.SetWaitFormVisibility(metadataEditorUserControl.SplashScreenManager, show: false);
        }
    }

    public void SetWaitFormVisibility(bool visible)
    {
        CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, visible);
    }

    private void SetAddTermDropDownMenu(bool loadTermTypes)
    {
        if (loadTermTypes)
        {
            metadataEditorUserControl.BusinessGlossarySupport.LoadTermTypes();
        }
        addTermPopupMenu.ClearLinks();
        if (metadataEditorUserControl.BusinessGlossarySupport.TermTypes.Count > 1)
        {
            addTermBarButtonItem.Tag = null;
            addTermBarButtonItem.Caption = "New";
            BarButtonItem[] termTypesBarButtonItems = metadataEditorUserControl.BusinessGlossarySupport.GetTermTypesBarButtonItems(addTermPopupMenu.Manager, AddTermBarButtonItem_ItemClick, smallIcons: false);
            PopupMenu popupMenu = addTermPopupMenu;
            BarItem[] items = termTypesBarButtonItems;
            popupMenu.AddItems(items);
            addTermBarButtonItem.ButtonStyle = BarButtonStyle.DropDown;
        }
        else if (metadataEditorUserControl.BusinessGlossarySupport.TermTypes.Count == 1)
        {
            TermTypeObject termTypeObject = metadataEditorUserControl.BusinessGlossarySupport.TermTypes[0];
            addTermBarButtonItem.Tag = termTypeObject;
            addTermBarButtonItem.Caption = "New " + termTypeObject.TitleAsSuffixWord;
            addTermBarButtonItem.ButtonStyle = BarButtonStyle.Default;
        }
        else
        {
            addTermBarButtonItem.Tag = null;
            addTermBarButtonItem.Caption = "New";
            addTermBarButtonItem.ButtonStyle = BarButtonStyle.Default;
        }
    }

    private void toolTipController_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        Links.OpenLink(e.Link, this);
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
        ShowOnboardingAfterRun();
    }

    private void ShowOnboardingAfterRun()
    {
        OnboardingSupport.ShowPanel(OnboardingSupport.OnboardingMessages.Run, this, () => addDatabaseButton.Links.First().ScreenBounds);
    }

    private void ShowOnboardingAfterSavingDiagram()
    {
        OnboardingSupport.OnboardingMessages onboardingMessage = OnboardingSupport.OnboardingMessages.DiagramSave;
        if (!OnboardingSupport.CheckMessageCondition(onboardingMessage))
        {
            return;
        }
        if (metadataEditorUserControl.TreeListHelpers.StartedToFocus || metadataEditorUserControl.GoingToNodeInProgress)
        {
            onboardingAfterSavingDiagramPending = true;
            return;
        }
        mainRibbonControl.SelectedPage = mainRibbonPage;
        mainRibbonControl.PerformLayout();
        OnboardingSupport.ShowPanel(onboardingMessage, this, () => DocCreatorBarButtonItem.Links.First().ScreenBounds);
        onboardingAfterSavingDiagramPending = false;
    }

    private void ShowDataProfilingOnboarding()
    {
        if (profileTablebarButtonItem.Visibility != 0)
        {
            return;
        }
        OnboardingSupport.OnboardingMessages onboardingMessage = OnboardingSupport.OnboardingMessages.DataProfilingButtonsShown;
        if (OnboardingSupport.CheckMessageCondition(onboardingMessage))
        {
            mainRibbonControl.SelectedPage = mainRibbonPage;
            mainRibbonControl.PerformLayout();
            OnboardingSupport.ShowPanel(onboardingMessage, this, () => profileTablebarButtonItem.Links.First().ScreenBounds);
        }
    }

    public void ShowOnboardings()
    {
        if (onboardingAfterSavingDiagramPending)
        {
            ShowOnboardingAfterSavingDiagram();
        }
        ShowDataProfilingOnboarding();
    }

    private void communityBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.Community, this);
    }

    private void shareProblemBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.RibbonSupportProblem, this);
    }

    private void shareQuestionBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.RibbonSupportQuestion, this);
    }

    private void shareIdeaBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.RibbonSupportIdea, this);
    }

    private void documentationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.SupportDocumentation, this);
    }

    private void tutorialsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.SupportTutorials, this);
    }

    private void supportBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.SupportSupport, this);
    }

    private void createSupportTicketBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.SupportCreateSupportTicket, this);
    }

    private void checkPriceBuyBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.DataedoBuy);
    }

    private void mainRibbonControl_ApplicationButtonClick(object sender, EventArgs e)
    {
        if (metadataEditorUserControl.ContinueAfterPossibleChanges())
        {
            loginForm.StartPosition = FormStartPosition.CenterParent;
            loginForm.OpenRecentPage(forceClean: true);
            loginForm.TopMost = true;
            loginForm.ShowDialog(this);
        }
    }

    private void addPrimaryKeyBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddPrimaryKey();
    }

    private void addUniqueKeyBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddUniqueKey();
    }

    private void classificationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification))
        {
            new UpgradeDataClassificationForm().ShowDialog();
            return;
        }
        try
        {
            using ClassificationForm classificationForm = new ClassificationForm();
            classificationForm.SetParameters(metadataEditorUserControl);
            classificationForm.ShowDialog();
        }
        catch (Exception exception)
        {
            GeneralExceptionHandling.Handle(exception, "Error while showing classification", FindForm());
        }
    }

    private void RepositoryDocumentationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        Links.OpenLink(Links.RepositorySchema);
    }

    private void settingsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        using SettingsForm settingsForm = new SettingsForm();
        settingsForm.ShowDialog(this);
    }

    private void sortAlphabeticallyButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.SortModulesAlphabetically(fromCustomFocus: false);
    }

    private void moveUpBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.MoveUpModule();
    }

    private void moveDownBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.MoveDownModule();
    }

    private void moveToTopBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.MoveModuleToTheTop(fromCustomFocus: false);
    }

    private void moveToBottomBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.MoveModuleToTheBottom(fromCustomFocus: false);
    }

    private void addNewTableBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddNewTable(SharedObjectTypeEnum.ObjectType.Table);
    }

    private void addNewViewBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddNewTable(SharedObjectTypeEnum.ObjectType.View);
    }

    private void addNewStructureBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddNewTable(SharedObjectTypeEnum.ObjectType.Structure);
    }

    private void showStaticDataBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        new ReadOnlyTextForm("Static Data Values", StaticDataHelper.GetStaticDataValuesAsJson(excludeCommands: true, excludeLicenses: true, this)).Show();
    }

    private void addNewPostItBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.AddNewPostIt();
    }

    private void SetDataLineageButtonsVisibility(ObjectTypeEventArgs e)
    {
        if (!SharedObjectTypeEnum.IsRegularObject(e?.ObjectType))
        {
            SetZoomLineageDiagramButtonsVisibility(false);
            SetShowFlowsColumnsButtonVisibility(false);
        }
    }

    private void SetShowFlowsColumnsButtonVisibility(bool? value)
    {
        showFlowsColumnsButtonItem.Visibility = ((value != true) ? BarItemVisibility.Never : BarItemVisibility.Always);
        SetDesignGroupVisibility();
    }

    private void SetZoomLineageDiagramButtonsVisibility(bool? value)
    {
        BarItemVisibility barItemVisibility3 = (zoomInLineageDiagramButtonItem.Visibility = (zoomOutLineageDiagramButtonItem.Visibility = ((value != true) ? BarItemVisibility.Never : BarItemVisibility.Always)));
        SetDesignGroupVisibility();
    }

    private void ShowFlowsColumnsButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        ShowFlowsColumnsButtonClick();
    }

    private void ShowFlowsColumnsButtonClick()
    {
        object tag = showFlowsColumnsButtonItem.Tag;
        if (tag is bool)
        {
            bool flag = (bool)tag;
            metadataEditorUserControl.ChangeLineageDiagramColumnsVisibility(!flag);
            showFlowsColumnsButtonItem.Tag = !flag;
        }
    }

    private void ZoomInLineageDiagramButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ChangeLineageDiagramZoom(zoomIn: true);
    }

    private void ZoomOutLineageDiagramButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        metadataEditorUserControl.ChangeLineageDiagramZoom(zoomIn: false);
    }

    private void SetShowFlowsColumnsButtonCaption(bool? columnsVisible)
    {
        if (columnsVisible == true)
        {
            showFlowsColumnsButtonItem.Caption = "Hide Columns";
        }
        else
        {
            showFlowsColumnsButtonItem.Caption = "Show Columns";
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
        new DevExpress.XtraSplashScreen.SplashScreenManager(this, null, true, true);
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.MainForm));
        this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
        this.mainRibbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
        this.userBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.userPopupControlContainer = new DevExpress.XtraBars.PopupControlContainer(this.components);
        this.userPanelUserControl = new Dataedo.App.UserControls.UserPanelUserControl();
        this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
        this.refreshMenuButton = new DevExpress.XtraBars.BarButtonItem();
        this.addDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
        this.databasePopUpMenu = new DevExpress.XtraBars.PopupMenu(this.components);
        this.saveButton = new DevExpress.XtraBars.BarButtonItem();
        this.savePopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
        this.turnAutosaveBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.DocCreatorBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.aboutBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.synchronizeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addModuleBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.exporttoExcelBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.documentationXtraReportBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.searchButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.showProgressBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.progressPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
        this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
        this.newOpenBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.recentPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
        this.moveDownDependencyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.moveUpDependencyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addRelationBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.editRelationBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.editConstraintBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.supportBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.customFieldsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addUserTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addObjectPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
        this.editTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addNewTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.designTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.editDisplayOptionsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.editRelationERDBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.showHideAvailableEntitiesBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.suggestedDescriptionBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.chatBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addTermBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addTermPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
        this.addRelatedTermBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addDataLinkBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.classificationBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.skinRibbonGalleryBarItem2 = new DevExpress.XtraBars.SkinRibbonGalleryBarItem();
        this.skinRibbonGalleryBarItem1 = new DevExpress.XtraBars.SkinRibbonGalleryBarItem();
        this.skinPaletteRibbonGalleryBarItem1 = new DevExpress.XtraBars.SkinPaletteRibbonGalleryBarItem();
        this.skinPaletteDropDownButtonItem1 = new DevExpress.XtraBars.SkinPaletteDropDownButtonItem();
        this.communityBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.shareProblemBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.shareQuestionBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.shareIdeaBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.documentationBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.tutorialsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.createSupportTicketBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.talkToSalesBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addPrimaryKeyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addUniqueKeyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.checkPriceBuyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.profileTablebarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.profileColumnbarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.repositoryDocumentationBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.settingsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.previewSampleDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.sortAlphabeticallyButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.moveUpBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.moveDownBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.moveToTopBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.moveToBottomBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.dataProfilingBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addNewViewBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addNewStructureBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.addNewPostItBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.showStaticDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.showFlowsColumnsButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.zoomInLineageDiagramButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.zoomOutLineageDiagramButtonItem = new DevExpress.XtraBars.BarButtonItem();
        this.mainRibbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
        this.addRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.designRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.erdRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.importExportRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.toolsRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.viewRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.helpRibbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
        this.informationRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.supportRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.communityRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.configRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.debugToolsRibbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
        this.debugToolsRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
        this.mainPanel = new DevExpress.XtraEditors.PanelControl();
        this.majorUpdatePanelControl = new DevExpress.XtraEditors.PanelControl();
        this.majorUpdateTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.majorUpdateLabel = new System.Windows.Forms.Label();
        this.newVersionInfoLabel = new System.Windows.Forms.Label();
        this.newVersionInfoPictureEdit = new DevExpress.XtraEditors.PictureEdit();
        this.updateNowUpdatePanelControlButton = new DevExpress.XtraEditors.SimpleButton();
        this.closeMajorUpdatePanelButton = new DevExpress.XtraEditors.SimpleButton();
        this.minorUpdatePanelControl = new DevExpress.XtraEditors.PanelControl();
        this.minorUpdateTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.updateInfolabel = new System.Windows.Forms.Label();
        this.minorUpdateLabel = new System.Windows.Forms.Label();
        this.updateInfoPictureEdit = new DevExpress.XtraEditors.PictureEdit();
        this.downloadMinorUpdateButton = new DevExpress.XtraEditors.SimpleButton();
        this.closeMinorUpdatePanelButton = new DevExpress.XtraEditors.SimpleButton();
        this.xmlSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
        this.exportTypesImageCollection = new DevExpress.Utils.ImageCollection(this.components);
        this.databasesImageCollection = new DevExpress.Utils.ImageCollection(this.components);
        this.onboardingToolTipController = new DevExpress.Utils.ToolTipController(this.components);
        ((System.ComponentModel.ISupportInitialize)this.mainRibbonControl).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.userPopupControlContainer).BeginInit();
        this.userPopupControlContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.barAndDockingController).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.databasePopUpMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.savePopupMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.progressPopupMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.recentPopupMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.addObjectPopupMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.addTermPopupMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.mainPanel).BeginInit();
        this.mainPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.majorUpdatePanelControl).BeginInit();
        this.majorUpdatePanelControl.SuspendLayout();
        this.majorUpdateTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.newVersionInfoPictureEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.minorUpdatePanelControl).BeginInit();
        this.minorUpdatePanelControl.SuspendLayout();
        this.minorUpdateTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.updateInfoPictureEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.exportTypesImageCollection).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.databasesImageCollection).BeginInit();
        base.SuspendLayout();
        this.mainRibbonControl.ApplicationButtonImageOptions.Image = Dataedo.App.Properties.Resources.folder_open_16;
        this.mainRibbonControl.ApplicationButtonText = "New/Open";
        this.mainRibbonControl.CaptionBarItemLinks.Add(this.userBarButtonItem);
        this.mainRibbonControl.Controller = this.barAndDockingController;
        this.mainRibbonControl.ExpandCollapseItem.Id = 0;
        this.mainRibbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[70]
        {
            this.mainRibbonControl.SearchEditItem,
            this.mainRibbonControl.ExpandCollapseItem,
            this.userBarButtonItem,
            this.refreshMenuButton,
            this.addDatabaseButton,
            this.saveButton,
            this.DocCreatorBarButtonItem,
            this.aboutBarButtonItem,
            this.synchronizeBarButtonItem,
            this.addModuleBarButtonItem,
            this.exporttoExcelBarButtonItem,
            this.documentationXtraReportBarButtonItem,
            this.searchButtonItem,
            this.showProgressBarButtonItem,
            this.newOpenBarButtonItem,
            this.moveDownDependencyBarButtonItem,
            this.moveUpDependencyBarButtonItem,
            this.addRelationBarButtonItem,
            this.editRelationBarButtonItem,
            this.editConstraintBarButtonItem,
            this.supportBarButtonItem,
            this.customFieldsBarButtonItem,
            this.addUserTableBarButtonItem,
            this.editTableBarButtonItem,
            this.addNewTableBarButtonItem,
            this.designTableBarButtonItem,
            this.editDisplayOptionsBarButtonItem,
            this.editRelationERDBarButtonItem,
            this.showHideAvailableEntitiesBarButtonItem,
            this.barButtonItem2,
            this.suggestedDescriptionBarButtonItem,
            this.chatBarButtonItem,
            this.addTermBarButtonItem,
            this.addRelatedTermBarButtonItem,
            this.addDataLinkBarButtonItem,
            this.classificationBarButtonItem,
            this.skinRibbonGalleryBarItem2,
            this.skinRibbonGalleryBarItem1,
            this.skinPaletteRibbonGalleryBarItem1,
            this.skinPaletteDropDownButtonItem1,
            this.turnAutosaveBarButtonItem,
            this.communityBarButtonItem,
            this.shareProblemBarButtonItem,
            this.shareQuestionBarButtonItem,
            this.shareIdeaBarButtonItem,
            this.documentationBarButtonItem,
            this.tutorialsBarButtonItem,
            this.createSupportTicketBarButtonItem,
            this.talkToSalesBarButtonItem,
            this.addPrimaryKeyBarButtonItem,
            this.addUniqueKeyBarButtonItem,
            this.checkPriceBuyBarButtonItem,
            this.profileTablebarButtonItem,
            this.profileColumnbarButtonItem,
            this.repositoryDocumentationBarButtonItem,
            this.settingsBarButtonItem,
            this.previewSampleDataBarButtonItem,
            this.sortAlphabeticallyButtonItem,
            this.moveUpBarButtonItem,
            this.moveDownBarButtonItem,
            this.moveToTopBarButtonItem,
            this.moveToBottomBarButtonItem,
            this.dataProfilingBarButtonItem,
            this.addNewViewBarButtonItem,
            this.addNewStructureBarButtonItem,
            this.addNewPostItBarButtonItem,
            this.showStaticDataBarButtonItem,
            this.showFlowsColumnsButtonItem,
            this.zoomInLineageDiagramButtonItem,
            this.zoomOutLineageDiagramButtonItem
        });
        this.mainRibbonControl.Location = new System.Drawing.Point(0, 0);
        this.mainRibbonControl.MaxItemId = 172;
        this.mainRibbonControl.Name = "mainRibbonControl";
        this.mainRibbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[3] { this.mainRibbonPage, this.helpRibbonPage, this.debugToolsRibbonPage });
        this.mainRibbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2019;
        this.mainRibbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.True;
        this.mainRibbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
        this.mainRibbonControl.ShowItemCaptionsInCaptionBar = true;
        this.mainRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Show;
        this.mainRibbonControl.ShowToolbarCustomizeItem = false;
        this.mainRibbonControl.Size = new System.Drawing.Size(1938, 162);
        this.mainRibbonControl.Toolbar.ShowCustomizeItem = false;
        this.mainRibbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
        this.mainRibbonControl.ToolTipController = this.toolTipController;
        this.mainRibbonControl.ApplicationButtonClick += new System.EventHandler(mainRibbonControl_ApplicationButtonClick);
        this.mainRibbonControl.ShowCustomizationMenu += new DevExpress.XtraBars.Ribbon.RibbonCustomizationMenuEventHandler(mainRibbonControl_ShowCustomizationMenu);
        this.userBarButtonItem.ActAsDropDown = true;
        this.userBarButtonItem.AllowDrawArrow = false;
        this.userBarButtonItem.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
        this.userBarButtonItem.Caption = "User";
        this.userBarButtonItem.DropDownControl = this.userPopupControlContainer;
        this.userBarButtonItem.Id = 145;
        this.userBarButtonItem.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("userBarButtonItem.ImageOptions.SvgImage");
        this.userBarButtonItem.Name = "userBarButtonItem";
        this.userPopupControlContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.userPopupControlContainer.Controls.Add(this.userPanelUserControl);
        this.userPopupControlContainer.Location = new System.Drawing.Point(114, 118);
        this.userPopupControlContainer.Name = "userPopupControlContainer";
        this.userPopupControlContainer.Ribbon = this.mainRibbonControl;
        this.userPopupControlContainer.Size = new System.Drawing.Size(395, 251);
        this.userPopupControlContainer.TabIndex = 3;
        this.userPopupControlContainer.Visible = false;
        this.userPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
        this.userPanelUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.userPanelUserControl.Location = new System.Drawing.Point(0, 0);
        this.userPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
        this.userPanelUserControl.Name = "userPanelUserControl";
        this.userPanelUserControl.Size = new System.Drawing.Size(395, 251);
        this.userPanelUserControl.TabIndex = 0;
        this.barAndDockingController.PropertiesBar.AllowLinkLighting = false;
        this.refreshMenuButton.Caption = "Refresh view";
        this.refreshMenuButton.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
        this.refreshMenuButton.Hint = "Reload data from Dataedo repository (F5)";
        this.refreshMenuButton.Id = 1;
        this.refreshMenuButton.ImageOptions.Image = Dataedo.App.Properties.Resources.refresh_16;
        this.refreshMenuButton.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.refresh_32;
        this.refreshMenuButton.Name = "refreshMenuButton";
        this.refreshMenuButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(refreshMenuButton_ItemClick);
        this.addDatabaseButton.ActAsDropDown = true;
        this.addDatabaseButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
        this.addDatabaseButton.Caption = "Add Source\r\n";
        this.addDatabaseButton.DropDownControl = this.databasePopUpMenu;
        this.addDatabaseButton.Hint = "Add documentation to repository (ctrl+N)";
        this.addDatabaseButton.Id = 8;
        this.addDatabaseButton.ImageOptions.Image = Dataedo.App.Properties.Resources.server_add_16;
        this.addDatabaseButton.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.server_add_32;
        this.addDatabaseButton.Name = "addDatabaseButton";
        this.databasePopUpMenu.Name = "databasePopUpMenu";
        this.databasePopUpMenu.Ribbon = this.mainRibbonControl;
        this.saveButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
        this.saveButton.Caption = "Save";
        this.saveButton.DropDownControl = this.savePopupMenu;
        this.saveButton.Id = 24;
        this.saveButton.ImageOptions.Image = Dataedo.App.Properties.Resources.save_16;
        this.saveButton.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.save_32;
        this.saveButton.LargeWidth = 85;
        this.saveButton.Name = "saveButton";
        this.saveButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(saveButton_ItemClick);
        this.savePopupMenu.ItemLinks.Add(this.turnAutosaveBarButtonItem);
        this.savePopupMenu.Name = "savePopupMenu";
        this.savePopupMenu.Ribbon = this.mainRibbonControl;
        this.turnAutosaveBarButtonItem.Caption = "Turn autosave";
        this.turnAutosaveBarButtonItem.Id = 143;
        this.turnAutosaveBarButtonItem.Name = "turnAutosaveBarButtonItem";
        this.turnAutosaveBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(turnAutosaveBarButtonItem_ItemClick);
        this.DocCreatorBarButtonItem.Caption = "Export";
        this.DocCreatorBarButtonItem.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
        this.DocCreatorBarButtonItem.Hint = "Export to HTML, PDF and Excel";
        this.DocCreatorBarButtonItem.Id = 29;
        this.DocCreatorBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.icon_16;
        this.DocCreatorBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.icon_32;
        this.DocCreatorBarButtonItem.LargeWidth = 60;
        this.DocCreatorBarButtonItem.Name = "DocCreatorBarButtonItem";
        this.DocCreatorBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DocCreatorBarButtonItem_ItemClick);
        this.aboutBarButtonItem.Caption = "About";
        this.aboutBarButtonItem.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
        this.aboutBarButtonItem.Hint = "About Dataedo";
        this.aboutBarButtonItem.Id = 30;
        this.aboutBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.about_16;
        this.aboutBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.about_32;
        this.aboutBarButtonItem.Name = "aboutBarButtonItem";
        this.aboutBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(aboutBarButtonItem_ItemClick);
        this.synchronizeBarButtonItem.Caption = "Import Changes";
        this.synchronizeBarButtonItem.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
        this.synchronizeBarButtonItem.Hint = "Reimport objects from database";
        this.synchronizeBarButtonItem.Id = 36;
        this.synchronizeBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.import_changes_16;
        this.synchronizeBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.import_changes_32;
        this.synchronizeBarButtonItem.LargeWidth = 66;
        this.synchronizeBarButtonItem.Name = "synchronizeBarButtonItem";
        this.synchronizeBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.synchronizeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(synchronizeBarButtonItem_ItemClick);
        this.addModuleBarButtonItem.Caption = "Add Subject Area";
        this.addModuleBarButtonItem.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
        this.addModuleBarButtonItem.Hint = "Add Subject Area to database";
        this.addModuleBarButtonItem.Id = 38;
        this.addModuleBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.module_add_16;
        this.addModuleBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.module_add_32;
        this.addModuleBarButtonItem.Name = "addModuleBarButtonItem";
        this.addModuleBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addModuleBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addModuleBarButtonItem_ItemClick);
        this.exporttoExcelBarButtonItem.Caption = "Export to Excel";
        this.exporttoExcelBarButtonItem.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
        this.exporttoExcelBarButtonItem.Id = 41;
        this.exporttoExcelBarButtonItem.Name = "exporttoExcelBarButtonItem";
        this.exporttoExcelBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.documentationXtraReportBarButtonItem.Caption = "Create documentation";
        this.documentationXtraReportBarButtonItem.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
        this.documentationXtraReportBarButtonItem.Id = 43;
        this.documentationXtraReportBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.documentation_16;
        this.documentationXtraReportBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.documentation_32;
        this.documentationXtraReportBarButtonItem.Name = "documentationXtraReportBarButtonItem";
        this.documentationXtraReportBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(documentationXtraReportBarButtonItem_ItemClick);
        this.searchButtonItem.Caption = "Search";
        this.searchButtonItem.Hint = "Search for objects in repository";
        this.searchButtonItem.Id = 46;
        this.searchButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
        this.searchButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.search_32;
        this.searchButtonItem.LargeWidth = 60;
        this.searchButtonItem.Name = "searchButtonItem";
        this.searchButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(searchButtonItem_ItemClick);
        this.showProgressBarButtonItem.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
        this.showProgressBarButtonItem.Caption = "Show progress";
        this.showProgressBarButtonItem.DropDownControl = this.progressPopupMenu;
        this.showProgressBarButtonItem.Hint = "Show progress of documenting your databases";
        this.showProgressBarButtonItem.Id = 46;
        this.showProgressBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.progress_16;
        this.showProgressBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.progress_32;
        this.showProgressBarButtonItem.LargeWidth = 60;
        this.showProgressBarButtonItem.Name = "showProgressBarButtonItem";
        this.showProgressBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(showProgressBarButtonItem_ItemClick);
        this.progressPopupMenu.ItemLinks.Add(this.barButtonItem2);
        this.progressPopupMenu.Name = "progressPopupMenu";
        this.progressPopupMenu.Ribbon = this.mainRibbonControl;
        this.progressPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(progressPopupMenu_BeforePopup);
        this.barButtonItem2.Caption = "barButtonItem2";
        this.barButtonItem2.Id = 97;
        this.barButtonItem2.Name = "barButtonItem2";
        this.newOpenBarButtonItem.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
        this.newOpenBarButtonItem.Caption = "New/Open";
        this.newOpenBarButtonItem.DropDownControl = this.recentPopupMenu;
        this.newOpenBarButtonItem.Hint = "Create new/open existing repository";
        this.newOpenBarButtonItem.Id = 55;
        this.newOpenBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.folder_open_16;
        this.newOpenBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.folder_open_32;
        this.newOpenBarButtonItem.Name = "newOpenBarButtonItem";
        this.newOpenBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(newOpenBarButtonItem_ItemClick);
        this.recentPopupMenu.Name = "recentPopupMenu";
        this.recentPopupMenu.Ribbon = this.mainRibbonControl;
        this.moveDownDependencyBarButtonItem.Caption = "Move dependency down";
        this.moveDownDependencyBarButtonItem.Hint = "Move dependency down";
        this.moveDownDependencyBarButtonItem.Id = 56;
        this.moveDownDependencyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
        this.moveDownDependencyBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_down_32;
        this.moveDownDependencyBarButtonItem.Name = "moveDownDependencyBarButtonItem";
        this.moveUpDependencyBarButtonItem.Caption = "Move dependency up";
        this.moveUpDependencyBarButtonItem.Hint = "Move dependency up";
        this.moveUpDependencyBarButtonItem.Id = 57;
        this.moveUpDependencyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
        this.moveUpDependencyBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_up_32;
        this.moveUpDependencyBarButtonItem.Name = "moveUpDependencyBarButtonItem";
        this.addRelationBarButtonItem.Caption = "Add relationship";
        this.addRelationBarButtonItem.Hint = "Add relationship";
        this.addRelationBarButtonItem.Id = 59;
        this.addRelationBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.relation_add_16;
        this.addRelationBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.relation_add_32;
        this.addRelationBarButtonItem.Name = "addRelationBarButtonItem";
        this.addRelationBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addRelationBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addRelationBarButtonItem_ItemClick);
        this.editRelationBarButtonItem.Caption = "Edit relationship";
        this.editRelationBarButtonItem.Hint = "Edit relationship";
        this.editRelationBarButtonItem.Id = 60;
        this.editRelationBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
        this.editRelationBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
        this.editRelationBarButtonItem.Name = "editRelationBarButtonItem";
        this.editRelationBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.editRelationBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editRelationBarButtonItem_ItemClick);
        this.editConstraintBarButtonItem.Caption = "Edit key";
        this.editConstraintBarButtonItem.Hint = "Edit key";
        this.editConstraintBarButtonItem.Id = 62;
        this.editConstraintBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
        this.editConstraintBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
        this.editConstraintBarButtonItem.Name = "editConstraintBarButtonItem";
        this.editConstraintBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.editConstraintBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editConstraintBarButtonItem_ItemClick);
        this.supportBarButtonItem.Caption = "Support";
        this.supportBarButtonItem.Hint = "Get help and share your ideas";
        this.supportBarButtonItem.Id = 151;
        this.supportBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.support_alt_16;
        this.supportBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.support_alt_32;
        this.supportBarButtonItem.Name = "supportBarButtonItem";
        this.supportBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(supportBarButtonItem_ItemClick);
        this.customFieldsBarButtonItem.Caption = "Custom fields";
        this.customFieldsBarButtonItem.Hint = "Define custom fields";
        this.customFieldsBarButtonItem.Id = 66;
        this.customFieldsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.custom_fields_16;
        this.customFieldsBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.custom_fields_32;
        this.customFieldsBarButtonItem.Name = "customFieldsBarButtonItem";
        this.customFieldsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(customFieldsBarButtonItem_ItemClick);
        this.addUserTableBarButtonItem.ActAsDropDown = true;
        this.addUserTableBarButtonItem.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
        this.addUserTableBarButtonItem.Caption = "Add new table";
        this.addUserTableBarButtonItem.DropDownControl = this.addObjectPopupMenu;
        this.addUserTableBarButtonItem.Hint = "Add table, structure";
        this.addUserTableBarButtonItem.Id = 69;
        this.addUserTableBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.table_add_32;
        this.addUserTableBarButtonItem.Name = "addUserTableBarButtonItem";
        this.addUserTableBarButtonItem.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
        this.addUserTableBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addUserTableBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addUserTableBarButtonItem_ItemClick);
        this.addObjectPopupMenu.Name = "addObjectPopupMenu";
        this.addObjectPopupMenu.Ribbon = this.mainRibbonControl;
        this.editTableBarButtonItem.Caption = "Design table";
        this.editTableBarButtonItem.Hint = "Add or edit table's column definition";
        this.editTableBarButtonItem.Id = 72;
        this.editTableBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
        this.editTableBarButtonItem.Name = "editTableBarButtonItem";
        this.editTableBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.editTableBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(EditTableBarButtonItem_ItemClick);
        this.addNewTableBarButtonItem.Caption = "Add new table";
        this.addNewTableBarButtonItem.Hint = "Add new table to repository and diagram";
        this.addNewTableBarButtonItem.Id = 76;
        this.addNewTableBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.table_add_16;
        this.addNewTableBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.table_add_32;
        this.addNewTableBarButtonItem.Name = "addNewTableBarButtonItem";
        this.addNewTableBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addNewTableBarButtonItem_ItemClick);
        this.designTableBarButtonItem.Caption = "Design table";
        this.designTableBarButtonItem.Hint = "Add or edit table's column definition";
        this.designTableBarButtonItem.Id = 78;
        this.designTableBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
        this.designTableBarButtonItem.Name = "designTableBarButtonItem";
        this.designTableBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.designTableBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(designTableBarButtonItem_ItemClick);
        this.editDisplayOptionsBarButtonItem.Caption = "Edit display options";
        this.editDisplayOptionsBarButtonItem.Hint = "Edit table display on the diagram";
        this.editDisplayOptionsBarButtonItem.Id = 81;
        this.editDisplayOptionsBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
        this.editDisplayOptionsBarButtonItem.Name = "editDisplayOptionsBarButtonItem";
        this.editDisplayOptionsBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.editDisplayOptionsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editDisplayOptionsBarButtonItem_ItemClick);
        this.editRelationERDBarButtonItem.Caption = "Edit relationship";
        this.editRelationERDBarButtonItem.Hint = "Edit relationship display options";
        this.editRelationERDBarButtonItem.Id = 83;
        this.editRelationERDBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
        this.editRelationERDBarButtonItem.Name = "editRelationERDBarButtonItem";
        this.editRelationERDBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.editRelationERDBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editRelationERDBarButtonItem_ItemClick);
        this.showHideAvailableEntitiesBarButtonItem.Caption = "Hide available entities";
        this.showHideAvailableEntitiesBarButtonItem.Id = 89;
        this.showHideAvailableEntitiesBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.suggested_erd_16;
        this.showHideAvailableEntitiesBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.suggested_erd_32;
        this.showHideAvailableEntitiesBarButtonItem.LargeWidth = 95;
        this.showHideAvailableEntitiesBarButtonItem.Name = "showHideAvailableEntitiesBarButtonItem";
        this.showHideAvailableEntitiesBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(showHideSuggestedEntitiesBarButtonItem_ItemClick);
        this.suggestedDescriptionBarButtonItem.Caption = "Hide suggestions";
        this.suggestedDescriptionBarButtonItem.Hint = "Displays title, description and custom field suggestions based on other objects with the same name";
        this.suggestedDescriptionBarButtonItem.Id = 97;
        this.suggestedDescriptionBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.hints_16;
        this.suggestedDescriptionBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.hints_32;
        this.suggestedDescriptionBarButtonItem.LargeWidth = 70;
        this.suggestedDescriptionBarButtonItem.Name = "suggestedDescriptionBarButtonItem";
        this.suggestedDescriptionBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(hintsBarButtonItem_ItemClick);
        this.chatBarButtonItem.Caption = "Live chat with support";
        this.chatBarButtonItem.Hint = "Talk to us on live chat (opens in browser window)";
        this.chatBarButtonItem.Id = 98;
        this.chatBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.chat_16;
        this.chatBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.chat_32;
        this.chatBarButtonItem.Name = "chatBarButtonItem";
        this.chatBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(chatBarButtonItem_ItemClick);
        this.addTermBarButtonItem.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
        this.addTermBarButtonItem.Caption = "Add Term";
        this.addTermBarButtonItem.DropDownControl = this.addTermPopupMenu;
        this.addTermBarButtonItem.Hint = "Add term to Business Glossary";
        this.addTermBarButtonItem.Id = 99;
        this.addTermBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.term_add_16;
        this.addTermBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.term_add_32;
        this.addTermBarButtonItem.Name = "addTermBarButtonItem";
        this.addTermBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addTermBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddTermBarButtonItem_ItemClick);
        this.addTermPopupMenu.Name = "addTermPopupMenu";
        this.addTermPopupMenu.Ribbon = this.mainRibbonControl;
        this.addRelatedTermBarButtonItem.Caption = "Edit term relationships";
        this.addRelatedTermBarButtonItem.Id = 100;
        this.addRelatedTermBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.term_add_related_term_16;
        this.addRelatedTermBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.term_add_related_term_32;
        this.addRelatedTermBarButtonItem.Name = "addRelatedTermBarButtonItem";
        this.addRelatedTermBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addRelatedTermBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddRelatedTermBarButtonItem_ItemClick);
        this.addDataLinkBarButtonItem.Caption = "Edit links to Glossary";
        this.addDataLinkBarButtonItem.Hint = "Edit links to Business Glossary terms";
        this.addDataLinkBarButtonItem.Id = 103;
        this.addDataLinkBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.data_link_add_16;
        this.addDataLinkBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.data_link_add_32;
        this.addDataLinkBarButtonItem.Name = "addDataLinkBarButtonItem";
        this.addDataLinkBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addDataLinkBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddDataLinkBarButtonItem_ItemClick);
        this.classificationBarButtonItem.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
        this.classificationBarButtonItem.Caption = "Data Discovery\r\n&& Classification";
        this.classificationBarButtonItem.Hint = "Find and classify columns containing sensitive data.";
        this.classificationBarButtonItem.Id = 106;
        this.classificationBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.classification_16;
        this.classificationBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.classification_32;
        this.classificationBarButtonItem.Name = "classificationBarButtonItem";
        this.classificationBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(classificationBarButtonItem_ItemClick);
        this.skinRibbonGalleryBarItem2.Caption = "skinRibbonGalleryBarItem2";
        this.skinRibbonGalleryBarItem2.Id = 139;
        this.skinRibbonGalleryBarItem2.Name = "skinRibbonGalleryBarItem2";
        this.skinRibbonGalleryBarItem1.Caption = "skinRibbonGalleryBarItem1";
        this.skinRibbonGalleryBarItem1.Id = 140;
        this.skinRibbonGalleryBarItem1.Name = "skinRibbonGalleryBarItem1";
        this.skinPaletteRibbonGalleryBarItem1.Caption = "skinPaletteRibbonGalleryBarItem1";
        this.skinPaletteRibbonGalleryBarItem1.Id = 141;
        this.skinPaletteRibbonGalleryBarItem1.Name = "skinPaletteRibbonGalleryBarItem1";
        this.skinPaletteDropDownButtonItem1.Id = 142;
        this.skinPaletteDropDownButtonItem1.Name = "skinPaletteDropDownButtonItem1";
        this.communityBarButtonItem.Caption = "Community";
        this.communityBarButtonItem.Id = 144;
        this.communityBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.community_16;
        this.communityBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.community_32;
        this.communityBarButtonItem.Name = "communityBarButtonItem";
        this.communityBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(communityBarButtonItem_ItemClick);
        this.shareProblemBarButtonItem.Caption = "Share a problem";
        this.shareProblemBarButtonItem.Id = 145;
        this.shareProblemBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.support_problems_16;
        this.shareProblemBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.support_problems_32;
        this.shareProblemBarButtonItem.Name = "shareProblemBarButtonItem";
        this.shareProblemBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(shareProblemBarButtonItem_ItemClick);
        this.shareQuestionBarButtonItem.Caption = "Share a question";
        this.shareQuestionBarButtonItem.Id = 146;
        this.shareQuestionBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.support_questions_16;
        this.shareQuestionBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.support_questions_32;
        this.shareQuestionBarButtonItem.Name = "shareQuestionBarButtonItem";
        this.shareQuestionBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(shareQuestionBarButtonItem_ItemClick);
        this.shareIdeaBarButtonItem.Caption = "Share an idea";
        this.shareIdeaBarButtonItem.Id = 147;
        this.shareIdeaBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.support_ideas_16;
        this.shareIdeaBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.support_ideas_32;
        this.shareIdeaBarButtonItem.Name = "shareIdeaBarButtonItem";
        this.shareIdeaBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(shareIdeaBarButtonItem_ItemClick);
        this.documentationBarButtonItem.Caption = "User docs";
        this.documentationBarButtonItem.Id = 149;
        this.documentationBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.support_docs_16;
        this.documentationBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.support_docs_32;
        this.documentationBarButtonItem.Name = "documentationBarButtonItem";
        this.documentationBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(documentationBarButtonItem_ItemClick);
        this.tutorialsBarButtonItem.Caption = "Tutorials";
        this.tutorialsBarButtonItem.Id = 150;
        this.tutorialsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.support_tutorials_16;
        this.tutorialsBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.support_tutorials_32;
        this.tutorialsBarButtonItem.Name = "tutorialsBarButtonItem";
        this.tutorialsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(tutorialsBarButtonItem_ItemClick);
        this.createSupportTicketBarButtonItem.Caption = "Create support ticket";
        this.createSupportTicketBarButtonItem.Id = 152;
        this.createSupportTicketBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.support_contact_16;
        this.createSupportTicketBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.support_contact_32;
        this.createSupportTicketBarButtonItem.Name = "createSupportTicketBarButtonItem";
        this.createSupportTicketBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(createSupportTicketBarButtonItem_ItemClick);
        this.talkToSalesBarButtonItem.Caption = "Talk to sales";
        this.talkToSalesBarButtonItem.Id = 153;
        this.talkToSalesBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.chat_16;
        this.talkToSalesBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.chat_32;
        this.talkToSalesBarButtonItem.Name = "talkToSalesBarButtonItem";
        this.addPrimaryKeyBarButtonItem.Caption = "Add primary key";
        this.addPrimaryKeyBarButtonItem.Hint = "Add primary key";
        this.addPrimaryKeyBarButtonItem.Id = 155;
        this.addPrimaryKeyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.primary_key_add_16;
        this.addPrimaryKeyBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.primary_key_add_32;
        this.addPrimaryKeyBarButtonItem.Name = "addPrimaryKeyBarButtonItem";
        this.addPrimaryKeyBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addPrimaryKeyBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addPrimaryKeyBarButtonItem_ItemClick);
        this.addUniqueKeyBarButtonItem.Caption = "Add unique key";
        this.addUniqueKeyBarButtonItem.Hint = "Add unique key";
        this.addUniqueKeyBarButtonItem.Id = 156;
        this.addUniqueKeyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.unique_key_add_16;
        this.addUniqueKeyBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.unique_key_add_32;
        this.addUniqueKeyBarButtonItem.Name = "addUniqueKeyBarButtonItem";
        this.addUniqueKeyBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.addUniqueKeyBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addUniqueKeyBarButtonItem_ItemClick);
        this.checkPriceBuyBarButtonItem.Caption = "Check price/Buy";
        this.checkPriceBuyBarButtonItem.Id = 154;
        this.checkPriceBuyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.icon_16;
        this.checkPriceBuyBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.icon_32;
        this.checkPriceBuyBarButtonItem.Name = "checkPriceBuyBarButtonItem";
        this.checkPriceBuyBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(checkPriceBuyBarButtonItem_ItemClick);
        this.profileTablebarButtonItem.Caption = "Profile Table";
        this.profileTablebarButtonItem.Id = 147;
        this.profileTablebarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_table_16;
        this.profileTablebarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.profile_table_32;
        this.profileTablebarButtonItem.Name = "profileTablebarButtonItem";
        this.profileTablebarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.profileTablebarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ProfileTableBarButtonItem_ItemClick);
        this.profileColumnbarButtonItem.Caption = "Profile Column";
        this.profileColumnbarButtonItem.Id = 148;
        this.profileColumnbarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_column_16;
        this.profileColumnbarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.profile_column_32;
        this.profileColumnbarButtonItem.Name = "profileColumnbarButtonItem";
        this.profileColumnbarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.profileColumnbarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ProfileColumnBarButtonItemAsync_ItemClick);
        this.repositoryDocumentationBarButtonItem.Caption = "Repository documentation";
        this.repositoryDocumentationBarButtonItem.Id = 158;
        this.repositoryDocumentationBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.server_documentation_16;
        this.repositoryDocumentationBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.server_documentation_32;
        this.repositoryDocumentationBarButtonItem.Name = "repositoryDocumentationBarButtonItem";
        this.repositoryDocumentationBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RepositoryDocumentationBarButtonItem_ItemClick);
        this.settingsBarButtonItem.Caption = "Settings";
        this.settingsBarButtonItem.Id = 159;
        this.settingsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.settings_32;
        this.settingsBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.settings_32;
        this.settingsBarButtonItem.Name = "settingsBarButtonItem";
        this.settingsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(settingsBarButtonItem_ItemClick);
        this.previewSampleDataBarButtonItem.Caption = "Preview Sample Data";
        this.previewSampleDataBarButtonItem.Id = 160;
        this.previewSampleDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sample_data_16;
        this.previewSampleDataBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.sample_data_32;
        this.previewSampleDataBarButtonItem.Name = "previewSampleDataBarButtonItem";
        this.previewSampleDataBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.previewSampleDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(PreviewSampleDataBarButtonItem_ItemClick);
        this.sortAlphabeticallyButtonItem.Caption = "Sort\r\nalphabetically";
        this.sortAlphabeticallyButtonItem.Id = 160;
        this.sortAlphabeticallyButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sort_asc_32;
        this.sortAlphabeticallyButtonItem.Name = "sortAlphabeticallyButtonItem";
        this.sortAlphabeticallyButtonItem.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
        this.sortAlphabeticallyButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.sortAlphabeticallyButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(sortAlphabeticallyButtonItem_ItemClick);
        this.moveUpBarButtonItem.Caption = "Move up";
        this.moveUpBarButtonItem.Id = 161;
        this.moveUpBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
        this.moveUpBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_up_32;
        this.moveUpBarButtonItem.Name = "moveUpBarButtonItem";
        this.moveUpBarButtonItem.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
        this.moveUpBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.moveUpBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveUpBarButtonItem_ItemClick);
        this.moveDownBarButtonItem.Caption = "Move down";
        this.moveDownBarButtonItem.Id = 162;
        this.moveDownBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
        this.moveDownBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_down_32;
        this.moveDownBarButtonItem.Name = "moveDownBarButtonItem";
        this.moveDownBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.moveDownBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveDownBarButtonItem_ItemClick);
        this.moveToTopBarButtonItem.Caption = "Move to top";
        this.moveToTopBarButtonItem.Id = 163;
        this.moveToTopBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_top_16;
        this.moveToTopBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_top_32;
        this.moveToTopBarButtonItem.Name = "moveToTopBarButtonItem";
        this.moveToTopBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.moveToTopBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveToTopBarButtonItem_ItemClick);
        this.moveToBottomBarButtonItem.Caption = "Move to bottom";
        this.moveToBottomBarButtonItem.Id = 164;
        this.moveToBottomBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_bottom_16;
        this.moveToBottomBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_bottom_32;
        this.moveToBottomBarButtonItem.Name = "moveToBottomBarButtonItem";
        this.moveToBottomBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.moveToBottomBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveToBottomBarButtonItem_ItemClick);
        this.dataProfilingBarButtonItem.Caption = "Data Profiling";
        this.dataProfilingBarButtonItem.Hint = "Profile all selected tables";
        this.dataProfilingBarButtonItem.Id = 166;
        this.dataProfilingBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_database_16;
        this.dataProfilingBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.profile_database_32;
        this.dataProfilingBarButtonItem.Name = "dataProfilingBarButtonItem";
        this.dataProfilingBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.dataProfilingBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DataProfilingBarButtonItem_ItemClick);
        this.addNewViewBarButtonItem.Caption = "Add new view";
        this.addNewViewBarButtonItem.Description = "Add new view";
        this.addNewViewBarButtonItem.Hint = "Add new view to repository and diagram";
        this.addNewViewBarButtonItem.Id = 167;
        this.addNewViewBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.view_new_16;
        this.addNewViewBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.view_new_32;
        this.addNewViewBarButtonItem.Name = "addNewViewBarButtonItem";
        this.addNewViewBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addNewViewBarButtonItem_ItemClick);
        this.addNewStructureBarButtonItem.Caption = "Add new structure";
        this.addNewStructureBarButtonItem.Description = "Add new structure to repository and diagram";
        this.addNewStructureBarButtonItem.Hint = "Add new structure to repository and diagram";
        this.addNewStructureBarButtonItem.Id = 168;
        this.addNewStructureBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.object_new_16;
        this.addNewStructureBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.object_add_32;
        this.addNewStructureBarButtonItem.Name = "addNewStructureBarButtonItem";
        this.addNewStructureBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addNewStructureBarButtonItem_ItemClick);
        this.addNewPostItBarButtonItem.Caption = "Add new post-it";
        this.addNewPostItBarButtonItem.Hint = "Add new post-it to diagram";
        this.addNewPostItBarButtonItem.Id = 168;
        this.addNewPostItBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.post_it_add_16;
        this.addNewPostItBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.post_it_add_32;
        this.addNewPostItBarButtonItem.Name = "addNewPostItBarButtonItem";
        this.addNewPostItBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addNewPostItBarButtonItem_ItemClick);
        this.showStaticDataBarButtonItem.Caption = "Show Static Data Values";
        this.showStaticDataBarButtonItem.Id = 169;
        this.showStaticDataBarButtonItem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("showStaticDataBarButtonItem.ImageOptions.Image");
        this.showStaticDataBarButtonItem.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("showStaticDataBarButtonItem.ImageOptions.LargeImage");
        this.showStaticDataBarButtonItem.Name = "showStaticDataBarButtonItem";
        this.showStaticDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(showStaticDataBarButtonItem_ItemClick);
        this.showFlowsColumnsButtonItem.Caption = "Show Columns";
        this.showFlowsColumnsButtonItem.Id = 170;
        this.showFlowsColumnsButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.column_32;
        this.showFlowsColumnsButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.column_32;
        this.showFlowsColumnsButtonItem.Name = "showFlowsColumnsButtonItem";
        this.showFlowsColumnsButtonItem.Tag = false;
        this.showFlowsColumnsButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.showFlowsColumnsButtonItem.VisibleInSearchMenu = false;
        this.showFlowsColumnsButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ShowFlowsColumnsButtonItem_ItemClick);
        this.zoomInLineageDiagramButtonItem.Caption = "Zoom In";
        this.zoomInLineageDiagramButtonItem.Hint = "Zoom in the diagram (ctrl + mouse wheel)";
        this.zoomInLineageDiagramButtonItem.Id = 171;
        this.zoomInLineageDiagramButtonItem.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("zoomInLineageDiagramButtonItem.ImageOptions.SvgImage");
        this.zoomInLineageDiagramButtonItem.Name = "zoomInLineageDiagramButtonItem";
        this.zoomInLineageDiagramButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.zoomInLineageDiagramButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ZoomInLineageDiagramButtonItem_ItemClick);
        this.zoomOutLineageDiagramButtonItem.Caption = "Zoom Out";
        this.zoomOutLineageDiagramButtonItem.Hint = "Zoom out the diagram (ctrl + mouse wheel)";
        this.zoomOutLineageDiagramButtonItem.Id = 172;
        this.zoomOutLineageDiagramButtonItem.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("zoomOutLineageDiagramButtonItem.ImageOptions.SvgImage");
        this.zoomOutLineageDiagramButtonItem.Name = "zoomOutLineageDiagramButtonItem";
        this.zoomOutLineageDiagramButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        this.zoomOutLineageDiagramButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ZoomOutLineageDiagramButtonItem_ItemClick);
        this.mainRibbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[6] { this.addRibbonPageGroup, this.designRibbonPageGroup, this.erdRibbonPageGroup, this.importExportRibbonPageGroup, this.toolsRibbonPageGroup, this.viewRibbonPageGroup });
        this.mainRibbonPage.Name = "mainRibbonPage";
        this.mainRibbonPage.Text = "Main";
        this.addRibbonPageGroup.ItemLinks.Add(this.addDatabaseButton);
        this.addRibbonPageGroup.ItemLinks.Add(this.addModuleBarButtonItem);
        this.addRibbonPageGroup.ItemLinks.Add(this.addUserTableBarButtonItem);
        this.addRibbonPageGroup.ItemLinks.Add(this.addTermBarButtonItem);
        this.addRibbonPageGroup.Name = "addRibbonPageGroup";
        this.addRibbonPageGroup.Text = "Add";
        this.designRibbonPageGroup.AllowTextClipping = false;
        this.designRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
        this.designRibbonPageGroup.ItemLinks.Add(this.moveUpBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.moveDownBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.moveToTopBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.moveToBottomBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.sortAlphabeticallyButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.editTableBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.editConstraintBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.addRelationBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.addPrimaryKeyBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.addUniqueKeyBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.editRelationBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.addDataLinkBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.addRelatedTermBarButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.saveButton);
        this.designRibbonPageGroup.ItemLinks.Add(this.showFlowsColumnsButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.zoomInLineageDiagramButtonItem);
        this.designRibbonPageGroup.ItemLinks.Add(this.zoomOutLineageDiagramButtonItem);
        this.designRibbonPageGroup.Name = "designRibbonPageGroup";
        this.designRibbonPageGroup.Text = "Design";
        this.erdRibbonPageGroup.AllowTextClipping = false;
        this.erdRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
        this.erdRibbonPageGroup.ItemLinks.Add(this.addNewTableBarButtonItem);
        this.erdRibbonPageGroup.ItemLinks.Add(this.addNewViewBarButtonItem);
        this.erdRibbonPageGroup.ItemLinks.Add(this.addNewStructureBarButtonItem);
        this.erdRibbonPageGroup.ItemLinks.Add(this.addNewPostItBarButtonItem);
        this.erdRibbonPageGroup.ItemLinks.Add(this.designTableBarButtonItem);
        this.erdRibbonPageGroup.ItemLinks.Add(this.editDisplayOptionsBarButtonItem);
        this.erdRibbonPageGroup.ItemLinks.Add(this.editRelationERDBarButtonItem);
        this.erdRibbonPageGroup.ItemLinks.Add(this.showHideAvailableEntitiesBarButtonItem);
        this.erdRibbonPageGroup.Name = "erdRibbonPageGroup";
        this.erdRibbonPageGroup.Text = "ERD";
        this.erdRibbonPageGroup.Visible = false;
        this.importExportRibbonPageGroup.AllowTextClipping = false;
        this.importExportRibbonPageGroup.ItemLinks.Add(this.synchronizeBarButtonItem);
        this.importExportRibbonPageGroup.ItemLinks.Add(this.DocCreatorBarButtonItem);
        this.importExportRibbonPageGroup.Name = "importExportRibbonPageGroup";
        this.importExportRibbonPageGroup.Text = "Import/Export";
        this.toolsRibbonPageGroup.ItemLinks.Add(this.dataProfilingBarButtonItem);
        this.toolsRibbonPageGroup.ItemLinks.Add(this.profileTablebarButtonItem);
        this.toolsRibbonPageGroup.ItemLinks.Add(this.previewSampleDataBarButtonItem);
        this.toolsRibbonPageGroup.ItemLinks.Add(this.profileColumnbarButtonItem);
        this.toolsRibbonPageGroup.ItemLinks.Add(this.classificationBarButtonItem);
        this.toolsRibbonPageGroup.ItemLinks.Add(this.customFieldsBarButtonItem);
        this.toolsRibbonPageGroup.Name = "toolsRibbonPageGroup";
        this.toolsRibbonPageGroup.Text = "Tools";
        this.viewRibbonPageGroup.ItemLinks.Add(this.showProgressBarButtonItem);
        this.viewRibbonPageGroup.ItemLinks.Add(this.suggestedDescriptionBarButtonItem);
        this.viewRibbonPageGroup.ItemLinks.Add(this.searchButtonItem);
        this.viewRibbonPageGroup.ItemLinks.Add(this.refreshMenuButton);
        this.viewRibbonPageGroup.Name = "viewRibbonPageGroup";
        this.viewRibbonPageGroup.Text = "View";
        this.helpRibbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[4] { this.informationRibbonPageGroup, this.supportRibbonPageGroup, this.communityRibbonPageGroup, this.configRibbonPageGroup });
        this.helpRibbonPage.Name = "helpRibbonPage";
        this.helpRibbonPage.Text = "Help";
        this.informationRibbonPageGroup.AllowTextClipping = false;
        this.informationRibbonPageGroup.ItemLinks.Add(this.aboutBarButtonItem);
        this.informationRibbonPageGroup.Name = "informationRibbonPageGroup";
        this.informationRibbonPageGroup.Text = "Information";
        this.supportRibbonPageGroup.AllowTextClipping = false;
        this.supportRibbonPageGroup.ItemLinks.Add(this.documentationBarButtonItem);
        this.supportRibbonPageGroup.ItemLinks.Add(this.repositoryDocumentationBarButtonItem);
        this.supportRibbonPageGroup.ItemLinks.Add(this.tutorialsBarButtonItem);
        this.supportRibbonPageGroup.ItemLinks.Add(this.supportBarButtonItem);
        this.supportRibbonPageGroup.ItemLinks.Add(this.createSupportTicketBarButtonItem);
        this.supportRibbonPageGroup.ItemLinks.Add(this.chatBarButtonItem);
        this.supportRibbonPageGroup.Name = "supportRibbonPageGroup";
        this.supportRibbonPageGroup.Text = "Support";
        this.communityRibbonPageGroup.AllowTextClipping = false;
        this.communityRibbonPageGroup.ItemLinks.Add(this.communityBarButtonItem);
        this.communityRibbonPageGroup.ItemLinks.Add(this.shareProblemBarButtonItem);
        this.communityRibbonPageGroup.ItemLinks.Add(this.shareQuestionBarButtonItem);
        this.communityRibbonPageGroup.ItemLinks.Add(this.shareIdeaBarButtonItem);
        this.communityRibbonPageGroup.Name = "communityRibbonPageGroup";
        this.communityRibbonPageGroup.Text = "Community";
        this.configRibbonPageGroup.AllowTextClipping = false;
        this.configRibbonPageGroup.ItemLinks.Add(this.settingsBarButtonItem);
        this.configRibbonPageGroup.Name = "configRibbonPageGroup";
        this.configRibbonPageGroup.Text = "Configuration";
        this.debugToolsRibbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.debugToolsRibbonPageGroup });
        this.debugToolsRibbonPage.Name = "debugToolsRibbonPage";
        this.debugToolsRibbonPage.Text = "DEBUG tools";
        this.debugToolsRibbonPage.Visible = false;
        this.debugToolsRibbonPageGroup.AllowTextClipping = false;
        this.debugToolsRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
        this.debugToolsRibbonPageGroup.ItemLinks.Add(this.documentationXtraReportBarButtonItem);
        this.debugToolsRibbonPageGroup.ItemLinks.Add(this.showStaticDataBarButtonItem);
        this.debugToolsRibbonPageGroup.Name = "debugToolsRibbonPageGroup";
        this.debugToolsRibbonPageGroup.Text = "DEBUG tools";
        this.debugToolsRibbonPageGroup.Visible = false;
        this.toolTipController.AllowHtmlText = true;
        this.toolTipController.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(toolTipController_HyperlinkClick);
        this.mainPanel.AutoSize = true;
        this.mainPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.mainPanel.Controls.Add(this.userPopupControlContainer);
        this.mainPanel.Controls.Add(this.majorUpdatePanelControl);
        this.mainPanel.Controls.Add(this.minorUpdatePanelControl);
        this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainPanel.Location = new System.Drawing.Point(0, 162);
        this.mainPanel.Name = "mainPanel";
        this.mainPanel.Size = new System.Drawing.Size(1938, 722);
        this.mainPanel.TabIndex = 3;
        this.majorUpdatePanelControl.Appearance.BackColor = System.Drawing.Color.LemonChiffon;
        this.majorUpdatePanelControl.Appearance.Options.UseBackColor = true;
        this.majorUpdatePanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.majorUpdatePanelControl.Controls.Add(this.majorUpdateTableLayoutPanel);
        this.majorUpdatePanelControl.Dock = System.Windows.Forms.DockStyle.Top;
        this.majorUpdatePanelControl.Location = new System.Drawing.Point(0, 26);
        this.majorUpdatePanelControl.Name = "majorUpdatePanelControl";
        this.majorUpdatePanelControl.Size = new System.Drawing.Size(1938, 26);
        this.majorUpdatePanelControl.TabIndex = 1;
        this.majorUpdatePanelControl.Visible = false;
        this.majorUpdateTableLayoutPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.majorUpdateTableLayoutPanel.ColumnCount = 5;
        this.majorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35f));
        this.majorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145f));
        this.majorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.majorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100f));
        this.majorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
        this.majorUpdateTableLayoutPanel.Controls.Add(this.majorUpdateLabel, 2, 0);
        this.majorUpdateTableLayoutPanel.Controls.Add(this.newVersionInfoLabel, 1, 0);
        this.majorUpdateTableLayoutPanel.Controls.Add(this.newVersionInfoPictureEdit, 0, 0);
        this.majorUpdateTableLayoutPanel.Controls.Add(this.updateNowUpdatePanelControlButton, 3, 0);
        this.majorUpdateTableLayoutPanel.Controls.Add(this.closeMajorUpdatePanelButton, 4, 0);
        this.majorUpdateTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
        this.majorUpdateTableLayoutPanel.Name = "majorUpdateTableLayoutPanel";
        this.majorUpdateTableLayoutPanel.RowCount = 1;
        this.majorUpdateTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
        this.majorUpdateTableLayoutPanel.Size = new System.Drawing.Size(1938, 26);
        this.majorUpdateTableLayoutPanel.TabIndex = 3;
        this.majorUpdateLabel.AutoSize = true;
        this.majorUpdateLabel.Dock = System.Windows.Forms.DockStyle.Left;
        this.majorUpdateLabel.Location = new System.Drawing.Point(183, 0);
        this.majorUpdateLabel.MinimumSize = new System.Drawing.Size(250, 13);
        this.majorUpdateLabel.Name = "majorUpdateLabel";
        this.majorUpdateLabel.Size = new System.Drawing.Size(250, 26);
        this.majorUpdateLabel.TabIndex = 1;
        this.majorUpdateLabel.Text = "versionUpdatePanelControlLabel";
        this.majorUpdateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.newVersionInfoLabel.Dock = System.Windows.Forms.DockStyle.Left;
        this.newVersionInfoLabel.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
        this.newVersionInfoLabel.Location = new System.Drawing.Point(38, 0);
        this.newVersionInfoLabel.MinimumSize = new System.Drawing.Size(135, 26);
        this.newVersionInfoLabel.Name = "newVersionInfoLabel";
        this.newVersionInfoLabel.Size = new System.Drawing.Size(139, 26);
        this.newVersionInfoLabel.TabIndex = 0;
        this.newVersionInfoLabel.Text = "New version available";
        this.newVersionInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.newVersionInfoPictureEdit.CausesValidation = false;
        this.newVersionInfoPictureEdit.Dock = System.Windows.Forms.DockStyle.Left;
        this.newVersionInfoPictureEdit.EditValue = Dataedo.App.Properties.Resources.about_32;
        this.newVersionInfoPictureEdit.Location = new System.Drawing.Point(3, 3);
        this.newVersionInfoPictureEdit.MenuManager = this.mainRibbonControl;
        this.newVersionInfoPictureEdit.Name = "newVersionInfoPictureEdit";
        this.newVersionInfoPictureEdit.Properties.AllowFocused = false;
        this.newVersionInfoPictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
        this.newVersionInfoPictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
        this.newVersionInfoPictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
        this.newVersionInfoPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
        this.newVersionInfoPictureEdit.Properties.Appearance.Options.UseBackColor = true;
        this.newVersionInfoPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.newVersionInfoPictureEdit.Properties.PictureAlignment = System.Drawing.ContentAlignment.MiddleLeft;
        this.newVersionInfoPictureEdit.Properties.ReadOnly = true;
        this.newVersionInfoPictureEdit.Properties.ShowMenu = false;
        this.newVersionInfoPictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
        this.newVersionInfoPictureEdit.Size = new System.Drawing.Size(20, 20);
        this.newVersionInfoPictureEdit.TabIndex = 0;
        this.updateNowUpdatePanelControlButton.AllowFocus = false;
        this.updateNowUpdatePanelControlButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
        this.updateNowUpdatePanelControlButton.CausesValidation = false;
        this.updateNowUpdatePanelControlButton.Dock = System.Windows.Forms.DockStyle.Left;
        this.updateNowUpdatePanelControlButton.Location = new System.Drawing.Point(473, 3);
        this.updateNowUpdatePanelControlButton.Name = "updateNowUpdatePanelControlButton";
        this.updateNowUpdatePanelControlButton.Size = new System.Drawing.Size(94, 20);
        this.updateNowUpdatePanelControlButton.TabIndex = 0;
        this.updateNowUpdatePanelControlButton.Text = "Download now";
        this.updateNowUpdatePanelControlButton.Click += new System.EventHandler(updateNowUpdatePanelControlButton_Click);
        this.closeMajorUpdatePanelButton.AllowFocus = false;
        this.closeMajorUpdatePanelButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        this.closeMajorUpdatePanelButton.Appearance.BackColor = System.Drawing.Color.Transparent;
        this.closeMajorUpdatePanelButton.Appearance.BorderColor = System.Drawing.Color.Transparent;
        this.closeMajorUpdatePanelButton.Appearance.Options.UseBackColor = true;
        this.closeMajorUpdatePanelButton.Appearance.Options.UseBorderColor = true;
        this.closeMajorUpdatePanelButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
        this.closeMajorUpdatePanelButton.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
        this.closeMajorUpdatePanelButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
        this.closeMajorUpdatePanelButton.Location = new System.Drawing.Point(1908, 3);
        this.closeMajorUpdatePanelButton.Name = "closeMajorUpdatePanelButton";
        this.closeMajorUpdatePanelButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
        this.closeMajorUpdatePanelButton.Size = new System.Drawing.Size(27, 20);
        this.closeMajorUpdatePanelButton.TabIndex = 0;
        this.closeMajorUpdatePanelButton.Click += new System.EventHandler(closeMajorUpdateButton_Click);
        this.minorUpdatePanelControl.Appearance.BackColor = System.Drawing.Color.LemonChiffon;
        this.minorUpdatePanelControl.Appearance.Options.UseBackColor = true;
        this.minorUpdatePanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.minorUpdatePanelControl.Controls.Add(this.minorUpdateTableLayoutPanel);
        this.minorUpdatePanelControl.Dock = System.Windows.Forms.DockStyle.Top;
        this.minorUpdatePanelControl.Location = new System.Drawing.Point(0, 0);
        this.minorUpdatePanelControl.Name = "minorUpdatePanelControl";
        this.minorUpdatePanelControl.Size = new System.Drawing.Size(1938, 26);
        this.minorUpdatePanelControl.TabIndex = 2;
        this.minorUpdatePanelControl.Visible = false;
        this.minorUpdateTableLayoutPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.minorUpdateTableLayoutPanel.ColumnCount = 5;
        this.minorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35f));
        this.minorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110f));
        this.minorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.minorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100f));
        this.minorUpdateTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
        this.minorUpdateTableLayoutPanel.Controls.Add(this.updateInfolabel, 1, 0);
        this.minorUpdateTableLayoutPanel.Controls.Add(this.minorUpdateLabel, 2, 0);
        this.minorUpdateTableLayoutPanel.Controls.Add(this.updateInfoPictureEdit, 0, 0);
        this.minorUpdateTableLayoutPanel.Controls.Add(this.downloadMinorUpdateButton, 3, 0);
        this.minorUpdateTableLayoutPanel.Controls.Add(this.closeMinorUpdatePanelButton, 4, 0);
        this.minorUpdateTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
        this.minorUpdateTableLayoutPanel.Name = "minorUpdateTableLayoutPanel";
        this.minorUpdateTableLayoutPanel.RowCount = 1;
        this.minorUpdateTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
        this.minorUpdateTableLayoutPanel.Size = new System.Drawing.Size(1938, 26);
        this.minorUpdateTableLayoutPanel.TabIndex = 3;
        this.updateInfolabel.Dock = System.Windows.Forms.DockStyle.Left;
        this.updateInfolabel.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
        this.updateInfolabel.Location = new System.Drawing.Point(38, 0);
        this.updateInfolabel.MinimumSize = new System.Drawing.Size(110, 26);
        this.updateInfolabel.Name = "updateInfolabel";
        this.updateInfolabel.Size = new System.Drawing.Size(110, 26);
        this.updateInfolabel.TabIndex = 0;
        this.updateInfolabel.Text = "Update available";
        this.updateInfolabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.minorUpdateLabel.AutoSize = true;
        this.minorUpdateLabel.Dock = System.Windows.Forms.DockStyle.Left;
        this.minorUpdateLabel.Location = new System.Drawing.Point(148, 0);
        this.minorUpdateLabel.MinimumSize = new System.Drawing.Size(155, 13);
        this.minorUpdateLabel.Name = "minorUpdateLabel";
        this.minorUpdateLabel.Size = new System.Drawing.Size(155, 26);
        this.minorUpdateLabel.TabIndex = 1;
        this.minorUpdateLabel.Text = "label2";
        this.minorUpdateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.updateInfoPictureEdit.CausesValidation = false;
        this.updateInfoPictureEdit.Dock = System.Windows.Forms.DockStyle.Left;
        this.updateInfoPictureEdit.EditValue = Dataedo.App.Properties.Resources.about_32;
        this.updateInfoPictureEdit.Location = new System.Drawing.Point(3, 3);
        this.updateInfoPictureEdit.MenuManager = this.mainRibbonControl;
        this.updateInfoPictureEdit.Name = "updateInfoPictureEdit";
        this.updateInfoPictureEdit.Properties.AllowFocused = false;
        this.updateInfoPictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
        this.updateInfoPictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
        this.updateInfoPictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
        this.updateInfoPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
        this.updateInfoPictureEdit.Properties.Appearance.Options.UseBackColor = true;
        this.updateInfoPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.updateInfoPictureEdit.Properties.PictureAlignment = System.Drawing.ContentAlignment.MiddleLeft;
        this.updateInfoPictureEdit.Properties.ReadOnly = true;
        this.updateInfoPictureEdit.Properties.ShowMenu = false;
        this.updateInfoPictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
        this.updateInfoPictureEdit.Size = new System.Drawing.Size(20, 20);
        this.updateInfoPictureEdit.TabIndex = 0;
        this.downloadMinorUpdateButton.AllowFocus = false;
        this.downloadMinorUpdateButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
        this.downloadMinorUpdateButton.CausesValidation = false;
        this.downloadMinorUpdateButton.Dock = System.Windows.Forms.DockStyle.Left;
        this.downloadMinorUpdateButton.Location = new System.Drawing.Point(309, 3);
        this.downloadMinorUpdateButton.Name = "downloadMinorUpdateButton";
        this.downloadMinorUpdateButton.Size = new System.Drawing.Size(94, 20);
        this.downloadMinorUpdateButton.TabIndex = 0;
        this.downloadMinorUpdateButton.Text = "Download now";
        this.downloadMinorUpdateButton.Click += new System.EventHandler(downloadMinorVersionButton_Click);
        this.closeMinorUpdatePanelButton.AllowFocus = false;
        this.closeMinorUpdatePanelButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        this.closeMinorUpdatePanelButton.Appearance.BackColor = System.Drawing.Color.Transparent;
        this.closeMinorUpdatePanelButton.Appearance.BorderColor = System.Drawing.Color.Transparent;
        this.closeMinorUpdatePanelButton.Appearance.Options.UseBackColor = true;
        this.closeMinorUpdatePanelButton.Appearance.Options.UseBorderColor = true;
        this.closeMinorUpdatePanelButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
        this.closeMinorUpdatePanelButton.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
        this.closeMinorUpdatePanelButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
        this.closeMinorUpdatePanelButton.Location = new System.Drawing.Point(1908, 3);
        this.closeMinorUpdatePanelButton.Name = "closeMinorUpdatePanelButton";
        this.closeMinorUpdatePanelButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
        this.closeMinorUpdatePanelButton.Size = new System.Drawing.Size(27, 20);
        this.closeMinorUpdatePanelButton.TabIndex = 0;
        this.closeMinorUpdatePanelButton.Click += new System.EventHandler(closeMinorUpdateButton_Click);
        this.xmlSaveFileDialog.DefaultExt = "xml";
        this.xmlSaveFileDialog.Filter = "(*.xml)|*.xml";
        this.xmlSaveFileDialog.Title = "Saving as";
        this.exportTypesImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("exportTypesImageCollection.ImageStream");
        this.exportTypesImageCollection.Images.SetKeyName(0, "doc16.png");
        this.exportTypesImageCollection.Images.SetKeyName(1, "docx16.png");
        this.exportTypesImageCollection.Images.SetKeyName(2, "html16.png");
        this.exportTypesImageCollection.Images.SetKeyName(3, "odt16.png");
        this.exportTypesImageCollection.Images.SetKeyName(4, "pdf16.png");
        this.exportTypesImageCollection.Images.SetKeyName(5, "rft16.png");
        this.exportTypesImageCollection.Images.SetKeyName(6, "xls16.png");
        this.exportTypesImageCollection.Images.SetKeyName(7, "xlsx16.png");
        this.databasesImageCollection.ImageSize = new System.Drawing.Size(32, 32);
        this.databasesImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("databasesImageCollection.ImageStream");
        this.databasesImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_connect_32, "server_connect_32", typeof(Dataedo.App.Properties.Resources), 0);
        this.databasesImageCollection.Images.SetKeyName(0, "server_connect_32");
        this.databasesImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_updated_32, "server_updated_32", typeof(Dataedo.App.Properties.Resources), 1);
        this.databasesImageCollection.Images.SetKeyName(1, "server_updated_32");
        this.databasesImageCollection.InsertImage(Dataedo.App.Properties.Resources.business_glossary_32, "business_glossary_32", typeof(Dataedo.App.Properties.Resources), 2);
        this.databasesImageCollection.Images.SetKeyName(2, "business_glossary_32");
        this.databasesImageCollection.InsertImage(Dataedo.App.Properties.Resources.folder_open_32, "folder_open_32", typeof(Dataedo.App.Properties.Resources), 3);
        this.databasesImageCollection.Images.SetKeyName(3, "folder_open_32");
        this.onboardingToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(1938, 884);
        base.Controls.Add(this.mainPanel);
        base.Controls.Add(this.mainRibbonControl);
        base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("MainForm.IconOptions.Icon");
        base.IconOptions.Image = (System.Drawing.Image)resources.GetObject("MainForm.IconOptions.Image");
        this.MinimumSize = new System.Drawing.Size(800, 500);
        base.Name = "MainForm";
        this.Ribbon = this.mainRibbonControl;
        this.Text = "Dataedo Desktop";
        base.Activated += new System.EventHandler(MainForm_Activated);
        base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MainForm_FormClosing);
        base.Load += new System.EventHandler(MainForm_Load);
        base.Shown += new System.EventHandler(MainForm_Shown);
        ((System.ComponentModel.ISupportInitialize)this.mainRibbonControl).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.userPopupControlContainer).EndInit();
        this.userPopupControlContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.barAndDockingController).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.databasePopUpMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.savePopupMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.progressPopupMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.recentPopupMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.addObjectPopupMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.addTermPopupMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.mainPanel).EndInit();
        this.mainPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.majorUpdatePanelControl).EndInit();
        this.majorUpdatePanelControl.ResumeLayout(false);
        this.majorUpdateTableLayoutPanel.ResumeLayout(false);
        this.majorUpdateTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)this.newVersionInfoPictureEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.minorUpdatePanelControl).EndInit();
        this.minorUpdatePanelControl.ResumeLayout(false);
        this.minorUpdateTableLayoutPanel.ResumeLayout(false);
        this.minorUpdateTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)this.updateInfoPictureEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.exportTypesImageCollection).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.databasesImageCollection).EndInit();
        base.ResumeLayout(false);
        base.PerformLayout();
    }
}
