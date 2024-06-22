using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.DataSourceControls.Tools;
using Dataedo.CustomControls;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Helpers;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer;
using DevExpress.XtraLayout;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.DataSourceControls;

public class FileSystemSourceUserControl : BaseWizardPageControl, IFileSystemNavigationSupports
{
	private string currentPath;

	private WinExplorerViewStyle viewStyle;

	private IContainer components;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private GridControl gridControl;

	private LayoutControlItem gridLayoutControlItem;

	private WinExplorerView winExplorerView;

	private BreadCrumbEdit breadCrumbEdit;

	private LayoutControlItem breadCrumbLayoutControlItem;

	private SvgImageCollection svgImageCollection;

	private GridColumn nameGridColumn;

	private GridColumn imageGridColumn;

	private GridColumn pathGridColumn;

	private GridColumn checkGridColumn;

	private GridColumn groupGridColumn;

	private LabelControl upButtonLabelControl;

	private LabelControl forwardButtonLabelControl;

	private LabelControl backButtonLabelControl;

	private LayoutControlItem backButtonLabelControlLayoutControlItem;

	private LayoutControlItem forwardButtonLabelControlLayoutControlItem;

	private LayoutControlItem upButtonLabelControlLayoutControlItem;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	string IFileSystemNavigationSupports.CurrentPath => currentPath;

	public FileSystemSourceUserControl()
	{
		InitializeComponent();
		Initialize();
		mainNonCustomizableLayoutControl.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	private void Initialize()
	{
		currentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		viewStyle = WinExplorerViewStyle.Large;
		breadCrumbEdit.Path = currentPath;
		winExplorerView.OptionsView.Style = viewStyle;
		foreach (DriveInfo fixedDrife in FileSystemHelper.GetFixedDrives())
		{
			breadCrumbEdit.Properties.History.Add(new BreadCrumbHistoryItem(fixedDrife.RootDirectory.ToString()));
		}
	}

	private void breadCrumbEdit_PathChanged(object sender, BreadCrumbPathChangedEventArgs e)
	{
		Cursor current = Cursor.Current;
		Cursor.Current = Cursors.WaitCursor;
		try
		{
			currentPath = e.Path;
			gridControl.DataSource = FileSystemHelper.GetFileSystemEntries(breadCrumbEdit.Path, WinExplorerViewTools.GetIconSizeType(viewStyle), WinExplorerViewTools.GetItemSize(viewStyle));
		}
		finally
		{
			Cursor.Current = current;
		}
	}

	private void winExplorerView_ItemClick(object sender, WinExplorerViewItemClickEventArgs e)
	{
	}

	private void winExplorerView_ItemDoubleClick(object sender, WinExplorerViewItemDoubleClickEventArgs e)
	{
		if (e.MouseInfo.Button == MouseButtons.Left)
		{
			winExplorerView.ClearSelection();
			((FileSystemEntry)e.ItemInfo.Row.RowKey).DoAction(this);
		}
	}

	private void winExplorerView_KeyDown(object sender, KeyEventArgs e)
	{
	}

	private void breadCrumbEdit_Properties_QueryChildNodes(object sender, BreadCrumbQueryChildNodesEventArgs e)
	{
		if (e.Node.Caption == "Root")
		{
			e.Node.ChildNodes.Add(new BreadCrumbNode("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
			e.Node.ChildNodes.Add(new BreadCrumbNode("Documents", Environment.GetFolderPath(Environment.SpecialFolder.Recent)));
			e.Node.ChildNodes.Add(new BreadCrumbNode("Music", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
			e.Node.ChildNodes.Add(new BreadCrumbNode("Pictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));
			e.Node.ChildNodes.Add(new BreadCrumbNode("Video", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));
		}
		if (e.Node.Caption == "Computer")
		{
			foreach (DriveInfo fixedDrife in FileSystemHelper.GetFixedDrives())
			{
				e.Node.ChildNodes.Add(new BreadCrumbNode(fixedDrife.Name, fixedDrife.RootDirectory));
			}
		}
		string path = e.Node.Path;
		if (FileSystemHelper.IsDirExists(path))
		{
			string[] subFolders = FileSystemHelper.GetSubFolders(path);
			foreach (string path2 in subFolders)
			{
				e.Node.ChildNodes.Add(CreateBreadCrumbNode(path2));
			}
		}
	}

	private BreadCrumbNode CreateBreadCrumbNode(string path)
	{
		string dirName = FileSystemHelper.GetDirName(path);
		return new BreadCrumbNode(dirName, dirName, populateOnDemand: true);
	}

	private void breadCrumbEdit_Properties_NewNodeAdding(object sender, BreadCrumbNewNodeAddingEventArgs e)
	{
		e.Node.PopulateOnDemand = true;
	}

	private void breadCrumbEdit_Properties_RootGlyphClick(object sender, EventArgs e)
	{
		breadCrumbEdit.Properties.BreadCrumbMode = BreadCrumbMode.Edit;
		breadCrumbEdit.SelectAll();
	}

	private void backButtonLabelControl_Click(object sender, EventArgs e)
	{
		breadCrumbEdit.GoBack();
	}

	private void forwardButtonLabelControl_Click(object sender, EventArgs e)
	{
		breadCrumbEdit.GoForward();
	}

	private void upButtonLabelControl_Click(object sender, EventArgs e)
	{
		breadCrumbEdit.GoUp();
	}

	private void breadCrumbEdit_Properties_ValidatePath(object sender, BreadCrumbValidatePathEventArgs e)
	{
		if (!FileSystemHelper.IsDirExists(e.Path))
		{
			e.ValidationResult = BreadCrumbValidatePathResult.Cancel;
		}
		else
		{
			e.ValidationResult = BreadCrumbValidatePathResult.CreateNodes;
		}
	}

	void IFileSystemNavigationSupports.UpdatePath(string path)
	{
		breadCrumbEdit.Path = path;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.DataSourceControls.FileSystemSourceUserControl));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.BreadCrumbNode breadCrumbNode = new DevExpress.XtraEditors.BreadCrumbNode();
		DevExpress.XtraEditors.BreadCrumbNode breadCrumbNode2 = new DevExpress.XtraEditors.BreadCrumbNode();
		this.mainNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.upButtonLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.forwardButtonLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.backButtonLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.breadCrumbEdit = new DevExpress.XtraEditors.BreadCrumbEdit();
		this.svgImageCollection = new DevExpress.Utils.SvgImageCollection(this.components);
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.winExplorerView = new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.pathGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.checkGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.groupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.imageGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.gridLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.breadCrumbLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.backButtonLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.forwardButtonLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.upButtonLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.breadCrumbEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.svgImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.winExplorerView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.breadCrumbLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backButtonLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.forwardButtonLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.upButtonLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.SystemColors.Control;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.upButtonLabelControl);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.forwardButtonLabelControl);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.backButtonLabelControl);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.breadCrumbEdit);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.gridControl);
		this.mainNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainNonCustomizableLayoutControl.MenuManager = this.barManager;
		this.mainNonCustomizableLayoutControl.Name = "mainNonCustomizableLayoutControl";
		this.mainNonCustomizableLayoutControl.Root = this.Root;
		this.mainNonCustomizableLayoutControl.Size = new System.Drawing.Size(603, 534);
		this.mainNonCustomizableLayoutControl.TabIndex = 0;
		this.mainNonCustomizableLayoutControl.Text = "layoutControl";
		this.upButtonLabelControl.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("upButtonLabelControl.ImageOptions.SvgImage");
		this.upButtonLabelControl.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
		this.upButtonLabelControl.Location = new System.Drawing.Point(64, 12);
		this.upButtonLabelControl.MaximumSize = new System.Drawing.Size(22, 22);
		this.upButtonLabelControl.MinimumSize = new System.Drawing.Size(22, 22);
		this.upButtonLabelControl.Name = "upButtonLabelControl";
		this.upButtonLabelControl.Size = new System.Drawing.Size(22, 22);
		this.upButtonLabelControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.upButtonLabelControl.TabIndex = 9;
		this.upButtonLabelControl.Click += new System.EventHandler(upButtonLabelControl_Click);
		this.forwardButtonLabelControl.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("forwardButtonLabelControl.ImageOptions.SvgImage");
		this.forwardButtonLabelControl.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
		this.forwardButtonLabelControl.Location = new System.Drawing.Point(38, 12);
		this.forwardButtonLabelControl.MaximumSize = new System.Drawing.Size(22, 22);
		this.forwardButtonLabelControl.MinimumSize = new System.Drawing.Size(22, 22);
		this.forwardButtonLabelControl.Name = "forwardButtonLabelControl";
		this.forwardButtonLabelControl.Size = new System.Drawing.Size(22, 22);
		this.forwardButtonLabelControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.forwardButtonLabelControl.TabIndex = 8;
		this.forwardButtonLabelControl.Click += new System.EventHandler(forwardButtonLabelControl_Click);
		this.backButtonLabelControl.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("backButtonLabelControl.ImageOptions.SvgImage");
		this.backButtonLabelControl.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
		this.backButtonLabelControl.Location = new System.Drawing.Point(12, 12);
		this.backButtonLabelControl.MaximumSize = new System.Drawing.Size(22, 22);
		this.backButtonLabelControl.MinimumSize = new System.Drawing.Size(22, 22);
		this.backButtonLabelControl.Name = "backButtonLabelControl";
		this.backButtonLabelControl.Size = new System.Drawing.Size(22, 22);
		this.backButtonLabelControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.backButtonLabelControl.TabIndex = 6;
		this.backButtonLabelControl.Click += new System.EventHandler(backButtonLabelControl_Click);
		this.breadCrumbEdit.Location = new System.Drawing.Point(90, 12);
		this.breadCrumbEdit.Name = "breadCrumbEdit";
		this.breadCrumbEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinDown, "", 18, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.breadCrumbEdit.Properties.DropDownRows = 12;
		this.breadCrumbEdit.Properties.ImageIndex = 0;
		this.breadCrumbEdit.Properties.Images = this.svgImageCollection;
		breadCrumbNode.Caption = "Root";
		breadCrumbNode.Persistent = true;
		breadCrumbNode.PopulateOnDemand = true;
		breadCrumbNode.ShowCaption = false;
		breadCrumbNode.Value = "Root";
		breadCrumbNode2.Caption = "Computer";
		breadCrumbNode2.Persistent = true;
		breadCrumbNode2.PopulateOnDemand = true;
		breadCrumbNode2.Value = "Computer";
		this.breadCrumbEdit.Properties.Nodes.AddRange(new DevExpress.XtraEditors.BreadCrumbNode[2] { breadCrumbNode, breadCrumbNode2 });
		this.breadCrumbEdit.Properties.RootImageIndex = 0;
		this.breadCrumbEdit.Properties.SortNodesByCaption = true;
		this.breadCrumbEdit.Properties.RootGlyphClick += new System.EventHandler(breadCrumbEdit_Properties_RootGlyphClick);
		this.breadCrumbEdit.Properties.QueryChildNodes += new DevExpress.XtraEditors.BreadCrumbQueryChildNodesEventHandler(breadCrumbEdit_Properties_QueryChildNodes);
		this.breadCrumbEdit.Properties.ValidatePath += new DevExpress.XtraEditors.BreadCrumbValidatePathEventHandler(breadCrumbEdit_Properties_ValidatePath);
		this.breadCrumbEdit.Properties.NewNodeAdding += new DevExpress.XtraEditors.BreadCrumbNewNodeAddingEventHandler(breadCrumbEdit_Properties_NewNodeAdding);
		this.breadCrumbEdit.Size = new System.Drawing.Size(501, 22);
		this.breadCrumbEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.breadCrumbEdit.TabIndex = 5;
		this.breadCrumbEdit.PathChanged += new DevExpress.XtraEditors.BreadCrumbPathChangedEventHandler(breadCrumbEdit_PathChanged);
		this.svgImageCollection.Add("open", "image://svgimages/actions/open.svg");
		this.gridControl.Location = new System.Drawing.Point(12, 38);
		this.gridControl.MainView = this.winExplorerView;
		this.gridControl.Name = "gridControl";
		this.gridControl.Size = new System.Drawing.Size(579, 484);
		this.gridControl.TabIndex = 4;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.winExplorerView });
		this.winExplorerView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.nameGridColumn, this.pathGridColumn, this.checkGridColumn, this.groupGridColumn, this.imageGridColumn });
		this.winExplorerView.ColumnSet.CheckBoxColumn = this.checkGridColumn;
		this.winExplorerView.ColumnSet.DescriptionColumn = this.pathGridColumn;
		this.winExplorerView.ColumnSet.ExtraLargeImageColumn = this.imageGridColumn;
		this.winExplorerView.ColumnSet.GroupColumn = this.groupGridColumn;
		this.winExplorerView.ColumnSet.LargeImageColumn = this.imageGridColumn;
		this.winExplorerView.ColumnSet.MediumImageColumn = this.imageGridColumn;
		this.winExplorerView.ColumnSet.SmallImageColumn = this.imageGridColumn;
		this.winExplorerView.ColumnSet.TextColumn = this.nameGridColumn;
		this.winExplorerView.GridControl = this.gridControl;
		this.winExplorerView.GroupCount = 1;
		this.winExplorerView.Name = "winExplorerView";
		this.winExplorerView.OptionsBehavior.Editable = false;
		this.winExplorerView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.groupGridColumn, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.winExplorerView.ItemClick += new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerViewItemClickEventHandler(winExplorerView_ItemClick);
		this.winExplorerView.ItemDoubleClick += new DevExpress.XtraGrid.Views.WinExplorer.WinExplorerViewItemDoubleClickEventHandler(winExplorerView_ItemDoubleClick);
		this.winExplorerView.KeyDown += new System.Windows.Forms.KeyEventHandler(winExplorerView_KeyDown);
		this.nameGridColumn.Caption = "Name";
		this.nameGridColumn.FieldName = "Name";
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 0;
		this.pathGridColumn.Caption = "Path";
		this.pathGridColumn.FieldName = "Path";
		this.pathGridColumn.Name = "pathGridColumn";
		this.pathGridColumn.Visible = true;
		this.pathGridColumn.VisibleIndex = 1;
		this.checkGridColumn.Caption = "Check";
		this.checkGridColumn.FieldName = "IsCheck";
		this.checkGridColumn.Name = "checkGridColumn";
		this.checkGridColumn.Visible = true;
		this.checkGridColumn.VisibleIndex = 1;
		this.groupGridColumn.Caption = "Group";
		this.groupGridColumn.FieldName = "Group";
		this.groupGridColumn.Name = "groupGridColumn";
		this.groupGridColumn.Visible = true;
		this.groupGridColumn.VisibleIndex = 1;
		this.imageGridColumn.Caption = "Image";
		this.imageGridColumn.FieldName = "Image";
		this.imageGridColumn.Name = "imageGridColumn";
		this.imageGridColumn.Visible = true;
		this.imageGridColumn.VisibleIndex = 0;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(603, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 534);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(603, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 534);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(603, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 534);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.gridLayoutControlItem, this.breadCrumbLayoutControlItem, this.backButtonLabelControlLayoutControlItem, this.forwardButtonLabelControlLayoutControlItem, this.upButtonLabelControlLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(603, 534);
		this.Root.TextVisible = false;
		this.gridLayoutControlItem.Control = this.gridControl;
		this.gridLayoutControlItem.Location = new System.Drawing.Point(0, 26);
		this.gridLayoutControlItem.Name = "gridLayoutControlItem";
		this.gridLayoutControlItem.Size = new System.Drawing.Size(583, 488);
		this.gridLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridLayoutControlItem.TextVisible = false;
		this.breadCrumbLayoutControlItem.Control = this.breadCrumbEdit;
		this.breadCrumbLayoutControlItem.Location = new System.Drawing.Point(78, 0);
		this.breadCrumbLayoutControlItem.Name = "breadCrumbLayoutControlItem";
		this.breadCrumbLayoutControlItem.Size = new System.Drawing.Size(505, 26);
		this.breadCrumbLayoutControlItem.Text = "Address";
		this.breadCrumbLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.breadCrumbLayoutControlItem.TextVisible = false;
		this.backButtonLabelControlLayoutControlItem.Control = this.backButtonLabelControl;
		this.backButtonLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.backButtonLabelControlLayoutControlItem.Name = "backButtonLabelControlLayoutControlItem";
		this.backButtonLabelControlLayoutControlItem.Size = new System.Drawing.Size(26, 26);
		this.backButtonLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.backButtonLabelControlLayoutControlItem.TextVisible = false;
		this.forwardButtonLabelControlLayoutControlItem.Control = this.forwardButtonLabelControl;
		this.forwardButtonLabelControlLayoutControlItem.Location = new System.Drawing.Point(26, 0);
		this.forwardButtonLabelControlLayoutControlItem.Name = "forwardButtonLabelControlLayoutControlItem";
		this.forwardButtonLabelControlLayoutControlItem.Size = new System.Drawing.Size(26, 26);
		this.forwardButtonLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.forwardButtonLabelControlLayoutControlItem.TextVisible = false;
		this.upButtonLabelControlLayoutControlItem.Control = this.upButtonLabelControl;
		this.upButtonLabelControlLayoutControlItem.Location = new System.Drawing.Point(52, 0);
		this.upButtonLabelControlLayoutControlItem.Name = "upButtonLabelControlLayoutControlItem";
		this.upButtonLabelControlLayoutControlItem.Size = new System.Drawing.Size(26, 26);
		this.upButtonLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.upButtonLabelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		base.Controls.Add(this.mainNonCustomizableLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "FileSystemSourceUserControl";
		base.Size = new System.Drawing.Size(603, 534);
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.breadCrumbEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.svgImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.winExplorerView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.breadCrumbLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backButtonLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.forwardButtonLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.upButtonLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
