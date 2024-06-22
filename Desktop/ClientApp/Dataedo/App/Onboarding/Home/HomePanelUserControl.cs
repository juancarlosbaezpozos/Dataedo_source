using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Dataedo.App.API.Models;
using Dataedo.App.API.Services;
using Dataedo.App.Onboarding.Home.Controls;
using Dataedo.App.Onboarding.Home.Model;
using Dataedo.App.Onboarding.Home.Model.Sections;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraWaitForm;

namespace Dataedo.App.Onboarding.Home;

public class HomePanelUserControl : BaseUserControl
{
	public delegate bool IsLinkToObjectValidDelegate(LinkToObjectByIdModel linkToObjectByIdModel);

	private HomeService homeService;

	private IsLinkToObjectValidDelegate isLinkToObjectValid;

	private WelcomePageModel welcomePage;

	private MetadataEditorUserControl metadataEditorUserControl;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LabelControl headerLabelControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private TablePanel tablePanel;

	private LayoutControlItem layoutControlItem1;

	private ProgressPanel progressPanel;

	private LayoutControlItem progressPanelLayoutControlItem;

	public bool RebuildNextTime { get; set; } = true;


	public bool IsLoaded => welcomePage != null;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler AddDatabaseButtonClick;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler<int> ImportChangesButtonClick;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler ExportDocumentationsButtonClick;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler<int> ExportDocumentationButtonClick;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler<string> LinkClick;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler<LinkToObjectByIdModel> LinkToObjectClick;

	public HomePanelUserControl(MetadataEditorUserControl metadataEditorUserControl)
	{
		InitializeComponent();
		homeService = new HomeService();
		headerLabelControl.Appearance.Options.UseFont = true;
		this.metadataEditorUserControl = metadataEditorUserControl;
	}

	public async void SetParameters(IsLinkToObjectValidDelegate isLinkToObjectValid, bool forceReload)
	{
		this.isLinkToObjectValid = isLinkToObjectValid;
		Rebuild(forceReload);
	}

	public async void Rebuild(bool forceReload)
	{
		if (IsLoaded)
		{
			foreach (object control in tablePanel.Controls)
			{
				if (control is LinksWithImageUserControl)
				{
					(control as LinksWithImageUserControl).Redraw();
				}
			}
		}
		if (!IsLoaded || RebuildNextTime || forceReload)
		{
			ForceRebuild(forceReload);
		}
	}

	public async void ForceRebuild(bool forceReload)
	{
		try
		{
			this.EnsureCreateHandle();
			progressPanelLayoutControlItem.Visibility = LayoutVisibility.Always;
			await LoadXml(forceReload);
			tablePanel.Visible = false;
			ClearData();
			headerLabelControl.Text = (string.IsNullOrEmpty(welcomePage.Title) ? "Welcome!" : welcomePage.Title);
			bool isAnyDatabaseIsImportedByUser = HomePageSupport.CheckIsAnyDatabaseIsImportedByUser();
			AddSection(headerLabelControl, null);
			foreach (SectionModel section6 in welcomePage.Sections)
			{
				if (section6 is NoUserDatabasesSection section && section6.IsApplicable(isAnyDatabaseIsImportedByUser))
				{
					AddAddDatabaseSection(section);
				}
				else if (section6 is DatabasesListSection section2 && section6.IsApplicable(isAnyDatabaseIsImportedByUser))
				{
					AddDatabasesListSection(section2);
				}
				else if (section6 is LinksToObjectsSection section3 && section6.IsApplicable(isAnyDatabaseIsImportedByUser))
				{
					AddLinksToObjectsSection(section3);
				}
				else if (section6 is LinksSection section4 && section6.IsApplicable(isAnyDatabaseIsImportedByUser))
				{
					AddLinksSection(section4);
				}
				else if (section6 is LinksWithImageSection section5 && section6.IsApplicable(isAnyDatabaseIsImportedByUser))
				{
					AddLinksWithImageSection(section5);
				}
			}
			AddRow();
			RebuildNextTime = false;
		}
		finally
		{
			progressPanelLayoutControlItem.Visibility = LayoutVisibility.Never;
			tablePanel.Visible = true;
			tablePanel.Invalidate();
		}
	}

	public void ClearData()
	{
		List<Control> list = tablePanel.Controls.OfType<Control>().ToList();
		tablePanel.Controls.Clear();
		foreach (Control item in list)
		{
			item.Dispose();
		}
		tablePanel.Rows.Clear();
	}

	private async Task LoadXml(bool forceReload)
	{
		if (IsLoaded && !forceReload)
		{
			return;
		}
		int num;
		if (StaticData.Profile != null && !string.IsNullOrEmpty(StaticData.Profile.Email))
		{
			AppLicense license = StaticData.License;
			if (license == null || license.AccountId.HasValue)
			{
				num = (string.IsNullOrEmpty(StaticData.License?.Type) ? 1 : 0);
				goto IL_007f;
			}
		}
		num = 1;
		goto IL_007f;
		IL_007f:
		bool flag = (byte)num != 0;
		if (!flag)
		{
			flag = !LoadWelcomePage(await DownloadXml());
		}
		if (flag)
		{
			await Task.Delay(250);
			LoadWelcomePage(LoadDefaultXml());
		}
	}

	private bool LoadWelcomePage(string xmlData)
	{
		try
		{
			using (StringReader textReader = new StringReader(xmlData))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(WelcomePageModel));
				welcomePage = xmlSerializer.Deserialize(textReader) as WelcomePageModel;
			}
			SetSamples();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void SetSamples()
	{
		if (welcomePage != null && !welcomePage.Sections.Any((SectionModel x) => x is LinksToObjectsSection))
		{
			int index = welcomePage.Sections.Where((SectionModel x) => x is NoUserDatabasesSection || x is DatabasesListSection).Max((SectionModel x) => welcomePage.Sections.IndexOf(x));
			LinksToObjectsSection linksToObjectsSection = new LinksToObjectsSection("Samples", ifAnyUserDatabases: false, ifNoUserDatabases: true);
			linksToObjectsSection.Links = new List<LinkToObjectByIdModel>
			{
				new LinkToObjectByIdModel("Sample table: Purchasing.PurchaseOrderHeader", 2, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table), 175, new ToolTipModel("Sample table", "Purchasing.PurchaseOrderHeader")),
				new LinkToObjectByIdModel("Sample data profiling: dbo.Orders", 9, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table), 3749, new ToolTipModel("Sample data profiling", "dbo.Orders")),
				new LinkToObjectByIdModel("Sample MongoDB Collection: projects", 4, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table), 3718, new ToolTipModel("Sample MongoDB Collection", "projects")),
				new LinkToObjectByIdModel("Sample JSON file: company_tickers.json (Company tickers)", 5, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Structure), 3714, new ToolTipModel("Sample JSON file", "company_tickers.json (Company tickers)")),
				new LinkToObjectByIdModel("Sample CSV file: bds2018_cty.csv (Business Dynamics 2018 - City)", 5, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Structure), 3698, new ToolTipModel("Sample CSV file", "bds2018_cty.csv (Business Dynamics 2018 - City)")),
				new LinkToObjectByIdModel("Sample diagram: Purchasing", 2, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Erd), 11, new ToolTipModel("Sample diagram", "Purchasing")),
				new LinkToObjectByIdModel("Sample glossary term: Purchase Order", 3, SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Term), 28, new ToolTipModel("Sample glossary term", "Purchase Order"))
			};
			welcomePage.Sections.Insert(index, linksToObjectsSection);
		}
	}

	private string LoadDefaultXml()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		string text = (SkinsManager.IsCurrentSkinDark ? "HomePageDark.xml" : "HomePage.xml");
		string name = "Dataedo.App.Onboarding.Home.Data." + text;
		using StreamReader streamReader = new StreamReader(executingAssembly.GetManifestResourceStream(name));
		return streamReader.ReadToEnd();
	}

	private async Task<string> DownloadXml()
	{
		try
		{
			string text = await homeService.GetHomeData(StaticData.Profile?.Email, StaticData.License?.AccountId ?? (-1), StaticData.License?.Type, StaticData.License?.Id, SkinsManager.IsCurrentSkinDark ? "DARK" : "LIGHT", ProgramVersion.VersionWithBuild);
			LoadWelcomePage(text);
			return text;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private void AddAddDatabaseSection(NoUserDatabasesSection section)
	{
		AddDatabaseUserControl addDatabaseUserControl = new AddDatabaseUserControl();
		addDatabaseUserControl.Title = section.Title;
		addDatabaseUserControl.Text = section.Text;
		addDatabaseUserControl.AddDatabaseButtonClick += delegate(object s, EventArgs e)
		{
			this.AddDatabaseButtonClick?.Invoke(s, e);
		};
		AddSection(addDatabaseUserControl, section);
	}

	private void AddDatabasesListSection(DatabasesListSection section)
	{
		DatabasesListUserControl databasesListUserControl = new DatabasesListUserControl(metadataEditorUserControl, tablePanel);
		databasesListUserControl.Dock = DockStyle.Fill;
		databasesListUserControl.Title = section.Title;
		databasesListUserControl.ExportDocumentationsButtonClick += delegate(object s, EventArgs e)
		{
			this.ExportDocumentationsButtonClick?.Invoke(s, e);
		};
		databasesListUserControl.ExportDocumentationButtonClick += delegate(object s, int e)
		{
			this.ExportDocumentationButtonClick?.Invoke(s, e);
		};
		databasesListUserControl.ImportChangesButtonClick += delegate(object s, int e)
		{
			this.ImportChangesButtonClick?.Invoke(s, e);
		};
		AddSection(databasesListUserControl, section);
		databasesListUserControl.SetParameters(section.RowsCount);
	}

	public void RefreshDatabasesList()
	{
		(tablePanel?.Controls?.OfType<DatabasesListUserControl>()?.FirstOrDefault())?.RefreshDatabases();
	}

	private void AddLinksToObjectsSection(LinksToObjectsSection section)
	{
		List<LinkToObjectByIdModel> list = section.Links.Where((LinkToObjectByIdModel x) => isLinkToObjectValid(x)).ToList();
		if (list.Count != 0)
		{
			LinksToObjectsUserControl linksToObjectsUserControl = new LinksToObjectsUserControl();
			linksToObjectsUserControl.Title = section.Title;
			linksToObjectsUserControl.TablePanel = tablePanel;
			linksToObjectsUserControl.LinkToObjectClick += delegate(object s, LinkToObjectByIdModel e)
			{
				this.LinkToObjectClick?.Invoke(s, e);
			};
			AddSection(linksToObjectsUserControl, section);
			linksToObjectsUserControl.SetParameters(list);
		}
	}

	private void AddLinksWithImageSection(LinksWithImageSection section)
	{
		LinksWithImageUserControl linksWithImageUserControl = new LinksWithImageUserControl();
		linksWithImageUserControl.Title = section.Title;
		linksWithImageUserControl.TablePanel = tablePanel;
		linksWithImageUserControl.LinkClick += delegate(object s, string e)
		{
			this.LinkClick?.Invoke(s, e);
		};
		AddSection(linksWithImageUserControl, section);
		linksWithImageUserControl.Dock = DockStyle.Fill;
		linksWithImageUserControl.SetParameters(section.TileSize, section.ImageSize, section.Links);
	}

	private void AddLinksSection(LinksSection section)
	{
		LinksUserControl linksUserControl = new LinksUserControl();
		linksUserControl.Title = section.Title;
		linksUserControl.TablePanel = tablePanel;
		linksUserControl.LinkClick += delegate(object s, string e)
		{
			this.LinkClick?.Invoke(s, e);
		};
		AddSection(linksUserControl, section);
		linksUserControl.SetParameters(section.Links);
	}

	private void AddSection(Control control, SectionModel section)
	{
		if (section != null)
		{
			control.Padding = section.Padding.ToPadding();
		}
		AddRow();
		int row = tablePanel.Rows.Count - 1;
		tablePanel.Controls.Add(control);
		tablePanel.SetCell(control, row, 0);
	}

	private void AddRow()
	{
		tablePanel.Rows.Add(new TablePanelRow(TablePanelEntityStyle.AutoSize, 1f));
	}

	private void HomePanelUserControl_Resize(object sender, EventArgs e)
	{
		if (tablePanel.Rows.Count != 0)
		{
			tablePanel.Rows.RemoveAt(tablePanel.Rows.Count - 1);
			AddRow();
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tablePanel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressPanelLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.progressPanel);
		this.mainLayoutControl.Controls.Add(this.tablePanel);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(654, 496);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.progressPanel.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.progressPanel.Appearance.Options.UseBackColor = true;
		this.progressPanel.Location = new System.Drawing.Point(20, 2);
		this.progressPanel.Name = "progressPanel";
		this.progressPanel.Size = new System.Drawing.Size(632, 96);
		this.progressPanel.StyleController = this.mainLayoutControl;
		this.progressPanel.TabIndex = 6;
		this.progressPanel.Text = "progressPanel1";
		this.tablePanel.AutoScroll = true;
		this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 55f));
		this.tablePanel.FireScrollEventOnMouseWheel = true;
		this.tablePanel.Location = new System.Drawing.Point(2, 143);
		this.tablePanel.Name = "tablePanel";
		this.tablePanel.Size = new System.Drawing.Size(650, 351);
		this.tablePanel.TabIndex = 5;
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.Location = new System.Drawing.Point(5, 110);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(119, 29);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 4;
		this.headerLabelControl.Text = "Welcome!";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem1, this.progressPanelLayoutControlItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(654, 496);
		this.mainLayoutControlGroup.TextVisible = false;
		this.layoutControlItem1.Control = this.tablePanel;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 141);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(654, 355);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.progressPanelLayoutControlItem.Control = this.progressPanel;
		this.progressPanelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.progressPanelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 100);
		this.progressPanelLayoutControlItem.MinSize = new System.Drawing.Size(54, 100);
		this.progressPanelLayoutControlItem.Name = "progressPanelLayoutControlItem";
		this.progressPanelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 2, 2, 2);
		this.progressPanelLayoutControlItem.Size = new System.Drawing.Size(654, 100);
		this.progressPanelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.progressPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressPanelLayoutControlItem.TextVisible = false;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "HomePanelUserControl";
		base.Size = new System.Drawing.Size(654, 496);
		base.Resize += new System.EventHandler(HomePanelUserControl_Resize);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tablePanel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressPanelLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
