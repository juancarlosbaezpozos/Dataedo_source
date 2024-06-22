using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ERD;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.ERD.Extensions;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.Search;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.Tools.UI.Skins.Base;
using Dataedo.App.UserControls.PanelControls.Appearance;
using Dataedo.App.UserControls.PanelControls.CommonHelpers;
using Dataedo.App.UserControls.SchemaImportsAndChanges;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.PanelControls;

public class ModuleUserControl : BasePanelControl, ITabSettable, ITabChangable
{
	private const int HORIZONTAL_SCROLL_MARGIN = 25;

	private const int VERTICAL_SCROLL_MARGIN = 25;

	private bool isInitialResized;

	private DiagramManager manager;

	private ScrollableControlHelper scrollableControlHelper;

	private List<Node> objectsToInsertIntoModule;

	private List<Node> objectsToRemoveFromModule;

	private int moduleId;

	private int databaseId;

	private LinkStyleEnum.LinkStyle erdLinkStyle;

	private GridHitInfo erdAvailableObjectsGridHitInfo;

	private DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode;

	private bool erdShowTypes;

	private bool isModuleEdited;

	private bool isModuleSettingsEdited;

	private bool erdShowNullable;

	public bool GoingToNodeInProgress;

	private Dictionary<string, string> moduleCustomFieldsForHistory = new Dictionary<string, string>();

	private ObjectWithTitleAndHTMLDescriptionHistory moduleTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory();

	private ResultItem lastResult;

	private CanvasObject[] SelectedObjects = new CanvasObject[0];

	private IContainer components;

	private XtraTabControl moduleXtraTabControl;

	private XtraTabPage moduleDescriptionXtraTabPage;

	private HtmlUserControl moduleHtmlUserControl;

	private NonCustomizableLayoutControl databaseLayoutControl;

	private TextEdit moduleTitleTextEdit;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private DXErrorProvider moduleTitleErrorProvider;

	private OpenFileDialog moduleImageOpenFileDialog;

	private RepositoryItemPictureEdit iconModuleProcedureRepositoryItemPictureEdit;

	private RepositoryItemLookUpEdit typeModuleProcedureRepositoryItemLookUpEdit;

	private EmptySpaceItem emptySpaceItem1;

	private InfoUserControl moduleStatusUserControl;

	private XtraTabPage moduleERDXtraTabPage;

	private DiagramControl ERD;

	private SplitContainerControl splitContainerControl;

	private ImageList nodeImageList;

	private PanelControl erdPanelControl;

	private XtraScrollableControl xtraScrollableControlERD;

	private NonCustomizableLayoutControl availableObjectsLayoutControl;

	private LayoutControlGroup layoutControlGroup2;

	private ColumnHeader columnHeader1;

	private LayoutControlItem layoutControlItem9;

	private CustomFieldsPanelControl customFieldsPanelControl;

	private LayoutControlItem customFieldsLayoutControlItem;

	private GridControl erdSuggestedObjectsGrid;

	private GridView erdAvailableObjectsGridView;

	private LayoutControlItem erdAvailableObjectsLayoutControlItem;

	private GridColumn iconGridColumn;

	private GridColumn displayNameGridColumn;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private RepositoryItemTextEdit displayNameRepositoryItemTextEdit;

	private TextEdit moduleTextEdit;

	private LabelControl clearErdFilterLabelControl;

	private CheckEdit erdObjectsTypeViewsCheckEdit;

	private CheckEdit erdObjectsTypeTablesCheckEdit;

	private TextEdit erdObjectsFilterTextEdit;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem5;

	private LayoutControlItem layoutControlItem6;

	private GridColumn isSuggested;

	private CheckEdit allDocsCheckEdit;

	private LayoutControlItem layoutControlItem7;

	private XtraTabPage schemaImportsAndChangesXtraTabPage;

	private SchemaImportsAndChangesUserControl schemaImportsAndChangesUserControl;

	private LabelControl searchCountLabelControl;

	private LabelControl labelControl1;

	private LayoutControlItem layoutControlItem8;

	private LayoutControlItem layoutControlItem10;

	private ToolTipControllerUserControl toolTipControllerUserControl;

	private HelpIconUserControl helpIconUserControl;

	private LayoutControlItem layoutControlItem11;

	private CheckEdit autoAddToModuleCheckEdit;

	private LayoutControlItem layoutControlItem3;

	private CheckEdit erdObjectsTypeObjectsCheckEdit;

	private LayoutControlItem erdObjectsTypeObjectsCheckEditLayoutControlItem;

	public override int DatabaseId => databaseId;

	public override int ObjectModuleId => moduleId;

	public override int ObjectId => moduleId;

	public override SharedObjectTypeEnum.ObjectType ObjectType => SharedObjectTypeEnum.ObjectType.Module;

	public override string ObjectSchema => string.Empty;

	public override string ObjectName => string.Empty;

	public override HtmlUserControl DescriptionHtmlUserControl => moduleHtmlUserControl;

	public override XtraTabPage SchemaImportsAndChangesXtraTabPage => schemaImportsAndChangesXtraTabPage;

	public override SchemaImportsAndChangesUserControl SchemaImportsAndChangesUserControl => schemaImportsAndChangesUserControl;

	public override CustomFieldsPanelControl CustomFieldsPanelControl => customFieldsPanelControl;

	public int ModuleId
	{
		get
		{
			return moduleId;
		}
		set
		{
			moduleId = value;
			manager.SetModuleId(value);
		}
	}

	public bool AllDocs
	{
		get
		{
			return allDocsCheckEdit.Checked;
		}
		set
		{
			manager.AllDocs = allDocsCheckEdit.Checked;
		}
	}

	public new bool? DatabaseShowSchema { get; set; }

	public new bool? DatabaseShowSchemaOverride { get; set; }

	public new bool ShowSchema
	{
		get
		{
			if (DatabaseShowSchemaOverride != true)
			{
				if (!DatabaseShowSchemaOverride.HasValue)
				{
					return DatabaseShowSchema == true;
				}
				return false;
			}
			return true;
		}
	}

	public DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode DisplayDocumentationNameMode
	{
		get
		{
			return displayDocumentationNameMode;
		}
		set
		{
			displayDocumentationNameMode = value;
			foreach (Node node in ERD.Elements.Nodes)
			{
				node.DisplayDocumentationNameMode = value;
			}
			RefreshERDView(null, calculateDiagramBox: true);
		}
	}

	public bool ErdShowTypes
	{
		get
		{
			return erdShowTypes;
		}
		set
		{
			erdShowTypes = value;
			foreach (Node node in ERD.Elements.Nodes)
			{
				node.ShowColumnsDataTypes = value;
			}
		}
	}

	public bool IsModuleEdited
	{
		get
		{
			return isModuleEdited;
		}
		set
		{
			isModuleEdited = value;
			SetTabPageTitle(isModuleEdited, moduleDescriptionXtraTabPage, base.Edit);
		}
	}

	public bool IsModuleSettingsEdited
	{
		get
		{
			return isModuleSettingsEdited;
		}
		set
		{
			isModuleSettingsEdited = value;
			SetTabPageTitle(isModuleSettingsEdited, moduleERDXtraTabPage, base.Edit);
		}
	}

	public bool ErdShowNullable
	{
		get
		{
			return erdShowNullable;
		}
		set
		{
			erdShowNullable = value;
			foreach (Node node in ERD.Elements.Nodes)
			{
				node.ShowNullable = value;
			}
		}
	}

	public bool IsErdEdited { get; set; }

	public bool IsERDTabPageActive => moduleXtraTabControl.SelectedTabPage == moduleERDXtraTabPage;

	public bool IsDescriptionTabPageActive => moduleXtraTabControl.SelectedTabPage == moduleDescriptionXtraTabPage;

	public event EventHandler ERDTabPageSelectedEvent;

	public event EventHandler DeleteSelectedERDNode;

	public event EventHandler<int> ErdSavedEvent;

	public event EventHandler GoingToNodeEnded;

	public ModuleUserControl(MetadataEditorUserControl control)
		: base(control)
	{
		InitializeComponent();
		Initialize();
		objectsToInsertIntoModule = new List<Node>();
		objectsToRemoveFromModule = new List<Node>();
		moduleHtmlUserControl.ContentChangedEvent += moduleHtmlUserControl_PreviewKeyDown;
		base.Edit = new Edit(moduleTextEdit);
		manager = new DiagramManager(databaseId, ModuleId, erdShowTypes, erdLinkStyle, displayDocumentationNameMode, ERD, notDeletedOnly: false, erdShowNullable);
		ERD.Diagram.ImportantChange += Diagram_ObjectPropertyChanged;
		ERD.Diagram.NodeContainerChanged += Diagram_NodeContainerChanged;
		ERD.NodeDblClicked += delegate(Node node)
		{
			EditNode(node);
		};
		ERD.PostItDblClicked += delegate(PostIt postIt)
		{
			EditPostIt(postIt);
		};
		ERD.NodeDeleted += delegate
		{
			RefreshERDView(null, calculateDiagramBox: true);
		};
		ERD.NewNodeDeleted += delegate(Node node)
		{
			if (objectsToInsertIntoModule.Any((Node x) => x.TableId == node.TableId))
			{
				objectsToInsertIntoModule = objectsToInsertIntoModule.Where((Node x) => x.TableId != node.TableId).ToList();
			}
		};
		ERD.ColumnsChanged += delegate
		{
			RefreshERDView(null, calculateDiagramBox: true);
		};
		ERD.AddNode += delegate(int databaseId, Point position, SharedObjectTypeEnum.ObjectType objectType)
		{
			AddNode(position, objectType);
		};
		ERD.AddPostIt += delegate(Point position)
		{
			AddPostIt(position);
		};
		ERD.LinkDblClicked += delegate(Link link)
		{
			EditLink(link);
		};
		ERD.LoadRelatedNodes += delegate(List<Node> selectedNodes)
		{
			RefreshIgnoredNodesData(selectedNodes);
		};
		ERD.LinkAdded += delegate(Link link)
		{
			ERD.Elements.AddLink(link);
			DiagramControl.AddLinkInformationToColumns(link);
		};
		ERD.LinkDeleted += delegate(Link link)
		{
			ERD.Elements.RemoveLink(link);
			manager.DeletedLinks.Add(link);
			ERD.RemoveLinkInformationFromToNodeColumns(link);
			SetTabPageTitle(isEdited: true, moduleERDXtraTabPage, base.Edit);
		};
		ERD.GoToNodeClicked += delegate(Node node)
		{
			GoToNode(node);
		};
		ERD.LinkStyleChanged += delegate(LinkStyleEnum.LinkStyle style)
		{
			ChangeLinkStyle(style);
		};
		ERD.DisplayDocumentationNameModeChanged += delegate(DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode mode)
		{
			ChangeDisplayDocumentationNameMode(mode);
		};
		ERD.ShowColumnTypesChanged += delegate(bool show)
		{
			ChangeShowColumnTypes(show);
		};
		ERD.ShowColumnNullableChanged += delegate(bool show)
		{
			ChangeShowColumnNullable(show);
		};
		ERD.AddRelatedTables += delegate(Node node)
		{
			List<int> ignoredNodes = new List<int>();
			List<int> otherModuleNodes = new List<int>();
			List<int> outsideModuleNodes = new List<int>();
			List<Node> nodesPresentOnErd = new List<Node>();
			List<Node> list = new List<Node>();
			Point position2 = node.Position;
			if (node.RelatedNodes.Count > 0)
			{
				AddNodesToERD(position2, node.RelatedNodes, ignoredNodes, otherModuleNodes, outsideModuleNodes, list);
				UpdateERDNodes(position2, node.RelatedNodes, ignoredNodes, otherModuleNodes, outsideModuleNodes, list, nodesPresentOnErd, calculatePositon: true);
			}
			ERD.Elements.Links.ToList().ForEach(delegate(Link x)
			{
				x.SubjecrAreaId = ModuleId;
			});
			ERD.CustomFieldsSupport = base.CustomFieldsSupport;
			AddNewlyCreatedNodesIntoAddToModuleList(list?.ToArray());
		};
		ERD.DragMouseUp += delegate(Box box, Point point)
		{
			RefreshERDView(point, calculateDiagramBox: true);
			ScrollForArea(box, ERD.StartPoint);
		};
		ERD.RefreshIgnoredNodes += delegate
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			SuspendLayout();
			ReloadIgnoredNodesPanel();
			ResumeLayout();
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
		};
		ERD.NodeAddedToSubjectArea += delegate(Node node)
		{
			AddNewlyCreatedNodesIntoAddToModuleList(node);
		};
		ERD.NodeRemovedFromSubjectArea += (Node node) => RemoveNodeFromAddToModuleList(node);
		LengthValidation.SetTitleOrNameLengthLimit(moduleTitleTextEdit);
		scrollableControlHelper = new ScrollableControlHelper(xtraScrollableControlERD)
		{
			ScrollValue = 50
		};
		scrollableControlHelper.EnableMouseWheel();
		base.UserControlHelpers = new UserControlHelpers(0);
		SchemaImportsAndChangesSupport = new SchemaImportsAndChangesSupport(this);
		ERD.BackColor = SkinsManager.CurrentSkin.ErdBackColor;
		autoAddToModuleCheckEdit.Checked = LastConnectionInfo.LOGIN_INFO.AutoAddToModule;
		moduleStatusUserControl.SetShouldLoadColorsAfterLoad(shouldLoadColorsAfterLoad: false);
	}

	public void UpdateERDNode(int tableId, string schema, string name, string title, SharedObjectSubtypeEnum.ObjectSubtype subtype, UserTypeEnum.UserType source, List<ColumnRow> columnsToRemove)
	{
		manager.UpdateNodeById(tableId, schema, name, title, subtype, source, columnsToRemove);
	}

	public void AddNode(Point? position = null, SharedObjectTypeEnum.ObjectType? objectType = null)
	{
		SharedObjectTypeEnum.ObjectType objectType2 = SharedObjectTypeEnum.ObjectType.Structure;
		NodeTypeEnum.NodeType type = NodeTypeEnum.NodeType.Structure;
		if (objectType == SharedObjectTypeEnum.ObjectType.Table)
		{
			objectType2 = SharedObjectTypeEnum.ObjectType.Table;
			type = NodeTypeEnum.NodeType.Table;
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.View)
		{
			objectType2 = SharedObjectTypeEnum.ObjectType.View;
			type = NodeTypeEnum.NodeType.View;
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			objectType2 = SharedObjectTypeEnum.ObjectType.Structure;
			type = NodeTypeEnum.NodeType.Structure;
		}
		Point point = ((!position.HasValue) ? manager.GetNewNodePoint(manager.GetStartPoint(), calculatePositon: true) : position.Value.Subtract(ERD.StartPoint));
		DesignTableForm designTableForm = new DesignTableForm(null, databaseId, objectType2, base.CustomFieldsSupport);
		if (designTableForm.ShowDialog() == DialogResult.OK)
		{
			Node node = new Node(databaseId, ShowSchema, databaseId, false, null, null, designTableForm.TableDesigner.TableId, designTableForm.TableDesigner.Name, point, type, deleted: false, visible: true, UserTypeEnum.UserType.USER, designTableForm.TableDesigner.Type, isTableAddedToDatabase: false);
			node.SubjectAreaId = ModuleId;
			node.TableId = designTableForm.TableDesigner.TableId;
			node.Schema = designTableForm.TableDesigner.Schema;
			node.Title = designTableForm.TableDesigner.Title;
			node.DatabaseName = designTableForm.TableDesigner.DocumentationTitle;
			node.DatabaseType = SharedDatabaseTypeEnum.DatabaseType.Manual;
			node.TableSubtype = designTableForm.TableDesigner.Type;
			manager.IgnoredNodes.Add(node);
			manager.AddIgnoredNodes(new List<int> { node.TableId }, point, setDiagramControlValidPosition: false, erdLinkStyle, ErdShowTypes, displayDocumentationNameMode, calculatePositon: false, ErdShowNullable);
			DB.Module.InsertManualTable(node.SubjectAreaId, node.TableId);
			DBTreeMenu.AddObjectToModule(databaseId, designTableForm.TableDesigner.TableId, ModuleId, designTableForm.TableDesigner.Schema, designTableForm.TableDesigner.Name, designTableForm.TableDesigner.Title, designTableForm.TableDesigner.Source, objectType2, designTableForm.TableDesigner.Type);
			RefreshERDView(default(Point), calculateDiagramBox: true);
			Box box = new Box();
			box.UpdateMinMax(node);
			ScrollForArea(box, ERD.StartPoint);
		}
	}

	public void AddPostIt(Point? position = null)
	{
		Point position2 = ((!position.HasValue) ? manager.GetNewNodePoint(manager.GetStartPoint(), calculatePositon: true) : position.Value.Subtract(ERD.StartPoint));
		PostIt postIt = manager.AddPostIt(manager.diagram.NextPostItId, position2, null);
		RefreshERDView(default(Point), calculateDiagramBox: true);
		Box box = new Box();
		box.UpdateMinMax(postIt);
		ScrollForArea(box, ERD.StartPoint);
		EditPostIt(postIt);
	}

	public void GoToNode(Node node)
	{
		if (node != null)
		{
			GoingToNodeInProgress = true;
			if (DB.Module.IsModuleContainsTable(ModuleId, node.TableId))
			{
				base.MainControl.SelectNodeFromCurrentModule(node);
			}
			else
			{
				base.MainControl.SelectNodeFromOtherModule(node);
			}
			GoingToNodeInProgress = false;
			this.GoingToNodeEnded?.Invoke(this, null);
		}
	}

	private void ChangeLinkStyle(LinkStyleEnum.LinkStyle style)
	{
		erdLinkStyle = style;
		IsModuleSettingsEdited = true;
		IsErdEdited = true;
		foreach (Link link in ERD.Elements.Links)
		{
			link.IsModifiedInRelationForm = false;
			link.LinkStyle = style;
		}
	}

	private void ChangeDisplayDocumentationNameMode(DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode mode)
	{
		IsModuleSettingsEdited = true;
		IsErdEdited = true;
		DisplayDocumentationNameMode = mode;
	}

	private void ChangeShowColumnTypes(bool show)
	{
		ErdShowTypes = show;
		IsModuleSettingsEdited = true;
		IsErdEdited = true;
	}

	private void ChangeShowColumnNullable(bool show)
	{
		ErdShowNullable = show;
		IsModuleSettingsEdited = true;
		IsErdEdited = true;
	}

	private void ScrollForArea(Box box, Point? startPoint)
	{
		if (box != null)
		{
			if (startPoint.HasValue)
			{
				ScrollForArea(box.StartPoint.Add(startPoint.Value), box.EndPoint.Add(startPoint.Value));
			}
			else
			{
				ScrollForArea(box.StartPoint, box.EndPoint);
			}
		}
	}

	private void ScrollForArea(Point boxStartPoint, Point boxEndPoint)
	{
		int num = xtraScrollableControlERD.HorizontalScroll.Value + xtraScrollableControlERD.ClientSize.Width - 25;
		if (boxEndPoint.X > num)
		{
			xtraScrollableControlERD.HorizontalScroll.Value += boxEndPoint.X - num;
		}
		else
		{
			num = xtraScrollableControlERD.HorizontalScroll.Value + 25;
			if (boxStartPoint.X < num)
			{
				int num2 = xtraScrollableControlERD.HorizontalScroll.Value + boxStartPoint.X - num;
				xtraScrollableControlERD.HorizontalScroll.Value = ((num2 >= 0) ? num2 : 0);
			}
		}
		num = xtraScrollableControlERD.VerticalScroll.Value + xtraScrollableControlERD.ClientSize.Height - 25;
		if (boxEndPoint.Y > num)
		{
			xtraScrollableControlERD.VerticalScroll.Value += boxEndPoint.Y - num;
			return;
		}
		num = xtraScrollableControlERD.VerticalScroll.Value + 25;
		if (boxStartPoint.Y < num)
		{
			int num3 = xtraScrollableControlERD.VerticalScroll.Value + boxStartPoint.Y - num;
			xtraScrollableControlERD.VerticalScroll.Value = ((num3 >= 0) ? num3 : 0);
		}
	}

	private void RefreshERDView(Point? point, bool calculateDiagramBox, bool normalizeNodesPositions = true)
	{
		bool visible = xtraScrollableControlERD.HorizontalScroll.Visible;
		bool visible2 = xtraScrollableControlERD.VerticalScroll.Visible;
		ERD.RefreshControlSize(calculateDiagramBox: true, ERD.StartPoint);
		Point? point2 = null;
		if (normalizeNodesPositions)
		{
			point2 = manager.NormalizeNodesPositions(refreshMinimalDiagramPoint: true);
		}
		if (point2.HasValue && ERD.Elements.HasAnyNodes)
		{
			ERD.StartPoint = new Point(ERD.StartPoint.X + point2.Value.X, ERD.StartPoint.Y + point2.Value.Y);
			Point minimalDiagramPoint = manager.GetMinimalDiagramPoint(refreshMinimalDiagramPoint: true);
			if (minimalDiagramPoint.X + ERD.StartPoint.X - 25 < 0)
			{
				ERD.StartPoint = ERD.StartPoint.SubtractX(minimalDiagramPoint.X + ERD.StartPoint.X - 25);
			}
			if (minimalDiagramPoint.Y + ERD.StartPoint.Y - 25 < 0)
			{
				ERD.StartPoint = ERD.StartPoint.SubtractY(minimalDiagramPoint.Y + ERD.StartPoint.Y - 25);
			}
		}
		if (point.HasValue)
		{
			Point minimalDiagramPoint2 = manager.GetMinimalDiagramPoint(refreshMinimalDiagramPoint: true);
			int num = minimalDiagramPoint2.X + ERD.StartPoint.X - 25;
			int num2 = xtraScrollableControlERD.ClientSize.Width - (ERD.Diagram.LastDiagramBox.Width + num) - 50;
			if (num2 < 0)
			{
				ERD.StartPoint = ERD.StartPoint.AddX((num >= -num2) ? num2 : (-num));
			}
			num = minimalDiagramPoint2.Y + ERD.StartPoint.Y - 25;
			num2 = xtraScrollableControlERD.ClientSize.Height - (ERD.Diagram.LastDiagramBox.Height + num) - 50;
			if (num2 < 0)
			{
				ERD.StartPoint = ERD.StartPoint.AddY((num >= -num2) ? num2 : (-num));
			}
		}
		AdjustGridStartPoint();
		ERD.RefreshControlSize(calculateDiagramBox: true, ERD.StartPoint);
		xtraScrollableControlERD.HorizontalScroll.Visible = xtraScrollableControlERD.ClientSize.Width < ERD.Width;
		ERD.RefreshControlSize(calculateDiagramBox: false, ERD.StartPoint);
		xtraScrollableControlERD.VerticalScroll.Visible = xtraScrollableControlERD.ClientSize.Height < ERD.Height;
		if (visible && !xtraScrollableControlERD.HorizontalScroll.Visible)
		{
			CenterDiagram(centerHorizontally: true, visible2 && !xtraScrollableControlERD.VerticalScroll.Visible);
			visible2 = xtraScrollableControlERD.VerticalScroll.Visible;
			xtraScrollableControlERD.VerticalScroll.Visible = xtraScrollableControlERD.ClientSize.Height < ERD.Height;
			CenterDiagram(centerHorizontally: false, visible2 && !xtraScrollableControlERD.VerticalScroll.Visible);
		}
		else
		{
			CenterDiagram(centerHorizontally: false, visible2 && !xtraScrollableControlERD.VerticalScroll.Visible);
		}
		ERD.RefreshControlSize(calculateDiagramBox: false, ERD.StartPoint);
		foreach (Node node in ERD.Elements.Nodes)
		{
			node.RecalcLinksArrows();
		}
	}

	private void AdjustGridStartPoint()
	{
		int diagramGridReminder = DiagramControl.GetDiagramGridReminder(ERD.StartPoint.X);
		int diagramGridReminder2 = DiagramControl.GetDiagramGridReminder(ERD.StartPoint.Y);
		ERD.GridStartPoint = new Point((diagramGridReminder < 0) ? diagramGridReminder : (-50 + diagramGridReminder), (diagramGridReminder2 < 0) ? diagramGridReminder2 : (-50 + diagramGridReminder2));
	}

	private void EditLink(Link link)
	{
		if (link != null)
		{
			ERD.EditERDLink(link, databaseId, this);
		}
	}

	public void EditNode(Node node)
	{
		if (node != null)
		{
			new ERDNodeForm(node).ShowDialog(this, setCustomMessageDefaultOwner: true);
			ERD.RefreshNode(node);
		}
	}

	public void EditPostIt(PostIt postIt)
	{
		if (postIt != null)
		{
			ERD.EditPostIt(postIt);
		}
	}

	public void DesignSelectedObject()
	{
		ERD.DesignSelectedObject();
	}

	public void RemoveSelectedObjects()
	{
		(from x in ERD.Elements.Nodes
			where x.Selected
			where objectsToInsertIntoModule.Any((Node y) => y.TableId == x.TableId)
			select x).ForEach(delegate(Node x)
		{
			objectsToInsertIntoModule.Remove(x);
		});
		ERD.DeleteSelectedObjects();
	}

	public void EditSelectedERDNode()
	{
		ERD.EditSelectedERDNode();
	}

	public void EditSelectedERDLink()
	{
		ERD.EditSelectedERDLink();
	}

	public bool ShowSuggestedEntities()
	{
		if (splitContainerControl.PanelVisibility == SplitPanelVisibility.Both)
		{
			splitContainerControl.PanelVisibility = SplitPanelVisibility.Panel1;
		}
		else
		{
			splitContainerControl.PanelVisibility = SplitPanelVisibility.Both;
		}
		return splitContainerControl.PanelVisibility == SplitPanelVisibility.Both;
	}

	public void ClearHighlights(bool keepSearchActive)
	{
		base.UserControlHelpers.ClearHighlights(keepSearchActive, moduleXtraTabControl, null, null, moduleTitleTextEdit, customFieldsPanelControl.FieldControls);
		moduleHtmlUserControl.ClearHighlights();
		searchCountLabelControl.Text = string.Empty;
		searchCountLabelControl.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public override void SetTabsProgressHighlights()
	{
		base.UserControlHelpers.ClearTabHighlights(moduleXtraTabControl);
		if (base.MainControl.ShowProgress)
		{
			CreateKeyValuePairs();
			base.UserControlHelpers.SetTabsProgressHighlights(moduleXtraTabControl, base.KeyValuePairs);
		}
	}

	private void CreateKeyValuePairs()
	{
		base.KeyValuePairs.Clear();
		foreach (XtraTabPage tabPage in moduleXtraTabControl.TabPages)
		{
			int key = moduleXtraTabControl.TabPages.IndexOf(tabPage);
			if (tabPage.Equals(moduleDescriptionXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.ObjectPoints, base.CurrentNode.TotalObjectPoints);
				base.KeyValuePairs.Add(key, value);
			}
		}
	}

	public void ForceLayoutChange(bool forceAll = false)
	{
	}

	public void SetTab(ResultItem row, SharedObjectTypeEnum.ObjectType? type, bool changeTab, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, params int?[] elementId)
	{
		if (!type.HasValue)
		{
			base.UserControlHelpers.SetHighlight(row, searchWords, customFieldSearchItems, null, 0, moduleXtraTabControl, null, null, moduleTitleTextEdit, customFieldsPanelControl.FieldControls, moduleHtmlUserControl, null, null);
			searchCountLabelControl.Text = moduleHtmlUserControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(searchCountLabelControl, moduleHtmlUserControl.OccurrencesCount > 0);
		}
	}

	public new void SetLastResult(ResultItem result)
	{
		lastResult = result;
	}

	private void Diagram_NodeContainerChanged(object sender, RectangularObject element, bool isNew, bool byUser = false)
	{
		if (!isNew)
		{
			ERD.Elements.RemoveElement(element);
			if (element is Node item)
			{
				manager.DeletedNodes.Add(item);
			}
			else if (element is PostIt item2)
			{
				manager.DeletedPostIts.Add(item2);
			}
		}
		SetTabPageTitle(isEdited: true, moduleERDXtraTabPage, base.Edit);
	}

	private void ReloadIgnoredNodesPanel(List<Node> nodes = null)
	{
		manager.ReloadTables(null, null, databaseId, ModuleId, setLinks: false, allDocsCheckEdit.Checked);
		RefreshIgnoredNodesData(nodes);
	}

	private void RefreshIgnoredNodesData(List<Node> selectedNodes = null)
	{
		bool @checked = erdObjectsTypeTablesCheckEdit.Checked;
		bool checked2 = erdObjectsTypeViewsCheckEdit.Checked;
		bool checked3 = erdObjectsTypeObjectsCheckEdit.Checked;
		string filter = erdObjectsFilterTextEdit.Text.ToLower()?.Trim();
		List<Node> nodes = (from x in manager.IgnoredNodes
			where !x.Deleted
			orderby x.DisplayName
			select x).ToList();
		SetDataSource(selectedNodes, @checked, checked2, checked3, filter, nodes);
	}

	public void SetDataSource(List<Node> selectedNodes, bool showTables, bool showViews, bool showObjects, string filter, List<Node> nodes = null)
	{
		new List<Node>();
		erdSuggestedObjectsGrid.BeginUpdate();
		HashSet<SharedObjectTypeEnum.ObjectType> typesToShow = new HashSet<SharedObjectTypeEnum.ObjectType>();
		if (showTables)
		{
			typesToShow.Add(SharedObjectTypeEnum.ObjectType.Table);
		}
		if (showViews)
		{
			typesToShow.Add(SharedObjectTypeEnum.ObjectType.View);
		}
		if (showObjects)
		{
			typesToShow.Add(SharedObjectTypeEnum.ObjectType.Structure);
		}
		if (nodes == null)
		{
			return;
		}
		List<Node> list = nodes.Where((Node x) => x.DisplayName.ToLower().Contains(filter) && typesToShow.Contains(x.ObjectType)).ToList();
		foreach (Node item in list)
		{
			item.IsRelatedToSelected = false;
		}
		if (selectedNodes != null && selectedNodes.Count > 0)
		{
			foreach (Node item2 in list?.Where((Node y) => selectedNodes?.Any((Node x) => x.Selected && ((x != null && x.RelationsPkTables?.Any((NodeRelationContainer b) => b.TableId == y.TableId && b.RelationStatus.Equals("A")) == true) || (x != null && x.RelationsFkTables?.Any((NodeRelationContainer b) => b.TableId == y.TableId && b.RelationStatus.Equals("A")) == true))) ?? false))
			{
				item2.IsRelatedToSelected = true;
			}
		}
		if (selectedNodes == null || selectedNodes.Count == 0)
		{
			list.Where((Node y) => ERD.Elements.Nodes.Any((Node x) => (x != null && x.RelationsPkTables.Any((NodeRelationContainer b) => b.TableId == y.TableId && b.RelationStatus.Equals("A"))) || (x != null && x.RelationsFkTables?.Any((NodeRelationContainer b) => b.TableId == y.TableId && b.RelationStatus.Equals("A")) == true))).ToList().ForEach(delegate(Node x)
			{
				x.IsRelatedToSelected = true;
			});
			list.ForEach(delegate(Node x)
			{
				x.IsSuggested = x.NodeSource == ERDNodeSource.Module || x.IsRelatedToSelected;
			});
		}
		if (selectedNodes != null && selectedNodes.Count > 0)
		{
			Node node = selectedNodes.FirstOrDefault();
			if (node != null)
			{
				DiagramManager.SetRelatedTables(node, nodes);
			}
			list.ForEach(delegate(Node x)
			{
				x.IsSuggested = x.IsRelatedToSelected;
			});
		}
		IEnumerable<Node> enumerable;
		if (!AllDocs)
		{
			enumerable = list.Where((Node x) => x.IsFromCurrentDatabase);
		}
		else
		{
			IEnumerable<Node> enumerable2 = list;
			enumerable = enumerable2;
		}
		IEnumerable<Node> source = enumerable;
		erdSuggestedObjectsGrid.DataSource = source.OrderByDescending((Node x) => x.DisplayName);
		erdSuggestedObjectsGrid.EndUpdate();
	}

	private IEnumerable<Node> GetRelatedNodes(List<Node> selectedNodes, List<Node> nodes)
	{
		return nodes?.Where((Node y) => selectedNodes?.Any((Node x) => x.Selected && ((x != null && x.RelationsPkTables.Any((NodeRelationContainer b) => b.TableId == y.TableId && b.RelationStatus.Equals("A"))) || (x != null && x.RelationsFkTables?.Any((NodeRelationContainer b) => b.TableId == y.TableId && b.RelationStatus.Equals("A")) == true))) ?? false);
	}

	private void Diagram_ObjectPropertyChanged(object sender, EventArgs e)
	{
		IsErdEdited = true;
		SetTabPageTitle(isEdited: true, moduleERDXtraTabPage, base.Edit);
	}

	private void moduleHtmlUserControl_PreviewKeyDown(object sender, EventArgs e)
	{
		IsModuleEdited = true;
	}

	private void moduleTitleTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (editedTitleFromTreeList)
		{
			editedTitleFromTreeList = false;
			return;
		}
		IsTitleValid();
		IsModuleEdited = true;
	}

	private void modulePictureEdit_EditValueChanged(object sender, EventArgs e)
	{
		IsModuleEdited = true;
	}

	private void UpdateTitle(string title)
	{
		string text3 = (moduleTitleTextEdit.Text = (moduleTextEdit.Text = title));
		if (moduleTitleAndDescriptionHistory != null)
		{
			moduleTitleAndDescriptionHistory.Title = title;
		}
		if (moduleHtmlUserControl != null && moduleHtmlUserControl.ModuleRow != null)
		{
			moduleHtmlUserControl.ModuleRow.Title = title;
		}
	}

	public void SetNewTitle(string title)
	{
		editedTitleFromTreeList = true;
		UpdateTitle(title);
	}

	private bool IsTitleValid()
	{
		return ValidateFields.IsEditNotEmptyRaiseError(moduleTitleTextEdit, moduleTitleErrorProvider);
	}

	public override void SetParameters(DBTreeNode dbSelectedNode, CustomFieldsSupport customFieldsSupport, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		base.SetParameters(dbSelectedNode, customFieldsSupport, dependencyType);
		objectsToInsertIntoModule.Clear();
		objectsToRemoveFromModule.Clear();
		base.KeyValuePairs.Clear();
		CheckEdit checkEdit = erdObjectsTypeTablesCheckEdit;
		bool @checked = (erdObjectsTypeViewsCheckEdit.Checked = true);
		checkEdit.Checked = @checked;
		erdObjectsFilterTextEdit.Text = string.Empty;
		allDocsCheckEdit.Checked = false;
		moduleHtmlUserControl.CanListen = false;
		CommonFunctionsPanels.SetSelectedTabPage(moduleXtraTabControl, dependencyType);
		ModuleId = dbSelectedNode.Id;
		databaseId = dbSelectedNode.DatabaseId;
		ModuleObject dataById = DB.Module.GetDataById(ModuleId);
		DocumentationObject dataById2 = DB.Database.GetDataById(databaseId);
		if (dataById2 != null)
		{
			DatabaseShowSchema = dataById2.ShowSchema;
			DatabaseShowSchemaOverride = dataById2.ShowSchemaOverride;
		}
		if (dataById != null)
		{
			UpdateTitle(PrepareValue.ToString(dataById.Title));
			moduleHtmlUserControl.HtmlText = PrepareValue.ToString(dataById.Description);
			erdLinkStyle = LinkStyleEnum.ObjectToType(PrepareValue.ToString(dataById.ErdLinkStyle));
			displayDocumentationNameMode = DisplayDocumentationNameModeEnum.ObjectToType(PrepareValue.ToString(dataById.DisplayDocumentationNameMode));
			ErdShowTypes = PrepareValue.ToBool(dataById.ErdShowTypes);
			ErdShowNullable = PrepareValue.ToBool(dataById.ErdShowNullable);
			base.CustomFieldsSupport = customFieldsSupport;
			customFields = new CustomFieldContainer(ObjectType, ObjectId, customFieldsSupport);
			customFields.RetrieveCustomFields(dataById);
			customFields.ClearAddedDefinitionValues(null);
			SetCustomFieldsDataSource();
			RefreshERD(forceRefresh: true);
			IsModuleEdited = false;
			IsModuleSettingsEdited = false;
			IsErdEdited = false;
			CommonFunctionsPanels.ClearTabPagesTitle(moduleXtraTabControl, base.Edit);
			moduleStatusUserControl.Hide();
			schemaImportsAndChangesUserControl.ClearData();
			RefreshSchemaImportsAndChanges(forceRefresh: true);
		}
		else
		{
			UpdateTitle(dbSelectedNode.Name);
			moduleHtmlUserControl.HtmlText = null;
			IsModuleEdited = false;
			IsModuleSettingsEdited = false;
			IsErdEdited = false;
			CommonFunctionsPanels.ClearTabPagesTitle(moduleXtraTabControl, base.Edit);
			moduleStatusUserControl.SetDeletedObjectProperties();
			BaseSkin.SetSearchHighlight(searchCountLabelControl);
		}
		SetFunctionality();
		FillControlProgressHighlights();
		ERD.RefreshControlSize(calculateDiagramBox: true, ERD.StartPoint);
		ERD.CustomFieldsSupport = base.CustomFieldsSupport;
		if (ERD.Diagram.LastDiagramBox.Width > xtraScrollableControlERD.ClientSize.Width || ERD.Diagram.LastDiagramBox.Height > xtraScrollableControlERD.ClientSize.Height)
		{
			isInitialResized = false;
		}
		SetTabsProgressHighlights();
		moduleTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
		{
			ObjectId = ModuleId,
			Title = moduleTitleTextEdit.Text,
			Description = PrepareValue.GetHtmlText(moduleHtmlUserControl?.PlainText, moduleHtmlUserControl?.HtmlText),
			DescriptionPlain = moduleHtmlUserControl?.PlainText
		};
		DescriptionHtmlUserControl.ClearHistoryObjects();
		DescriptionHtmlUserControl.ModuleRow = dataById;
		moduleCustomFieldsForHistory = HistoryCustomFieldsHelper.GetOldCustomFieldsInObjectUserControl(customFields);
	}

	private void RefreshSchemaImportsAndChanges(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(schemaImportsAndChangesXtraTabPage, additionalCondition: true, forceRefresh) || refreshImmediatelyIfNotLoaded || refreshImmediatelyIfLoaded)
		{
			schemaImportsAndChangesUserControl.RefreshImports();
		}
	}

	private void RefreshERD(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(moduleERDXtraTabPage, additionalCondition: true, forceRefresh) || refreshImmediatelyIfNotLoaded || refreshImmediatelyIfLoaded)
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			InitERDGraph();
			base.MainControl.SetWaitformVisibility(visible: false);
		}
	}

	private bool IsRefreshRequired(XtraTabPage tabPage, bool additionalCondition = true, bool forceRefresh = false)
	{
		if (!forceRefresh || moduleXtraTabControl.SelectedTabPage != tabPage)
		{
			if (moduleXtraTabControl.SelectedTabPage == tabPage && additionalCondition)
			{
				return !base.TabPageChangedProgrammatically;
			}
			return false;
		}
		return true;
	}

	private void SetCustomFieldsDataSource()
	{
		CustomFieldsPanelControl.EditValueChanging += delegate
		{
			SetCurrentTabPageTitle(isEdited: true, moduleDescriptionXtraTabPage);
		};
		customFieldsPanelControl.ShowHistoryClick -= CustomFieldsPanelControl_ShowHistoryClick;
		customFieldsPanelControl.ShowHistoryClick += CustomFieldsPanelControl_ShowHistoryClick;
		IEnumerable<CustomFieldDefinition> customFieldRows = customFields.CustomFieldsData.Where((CustomFieldDefinition x) => x.CustomField?.ModuleVisibility ?? false);
		customFieldsPanelControl.LoadFields(customFieldRows, delegate
		{
			IsModuleEdited = true;
		}, customFieldsLayoutControlItem);
	}

	private void InitERDGraph()
	{
		manager.Reload(databaseId, ModuleId, erdShowTypes, erdLinkStyle, displayDocumentationNameMode, null, null, notDeletedOnly: false, setLinks: false, formatted: false, forHtml: false, null, erdShowNullable);
		RefreshIgnoredNodesData();
		RefreshERDView(null, calculateDiagramBox: true);
		CenterDiagram();
		base.MainControl.SetERDRemoveButtonVisibility(new ErdButtonArgs(visible: true, isRelation: false, isTable: false, hasMultipleLevelColumns: false, null, manager.DatabaseClass));
	}

	private void xtraScrollableControlERD_Resize(object sender, EventArgs e)
	{
		RefreshERDView(null, calculateDiagramBox: false);
		if (!isInitialResized && ERD.Elements.HasAnyNodes)
		{
			RefreshERDView(null, calculateDiagramBox: true);
			CenterDiagram();
			isInitialResized = true;
		}
	}

	private Point CenterDiagram(bool centerHorizontally = true, bool centerVertically = true, bool refreshMinimalDiagramPoint = true)
	{
		Point current = new Point(ERD.StartPoint.X, ERD.StartPoint.Y);
		if (ERD.Elements.HasAnyNodes)
		{
			Point minimalDiagramPoint = manager.GetMinimalDiagramPoint(refreshMinimalDiagramPoint);
			int entireGridCellsSize;
			if (centerHorizontally)
			{
				entireGridCellsSize = DiagramControl.GetEntireGridCellsSize((xtraScrollableControlERD.ClientSize.Width - ERD.Diagram.LastDiagramBox.Width - minimalDiagramPoint.X) / 2, toAbsoluteFloor: true);
				entireGridCellsSize = (xtraScrollableControlERD.ClientSize.Width - ERD.Diagram.LastDiagramBox.Width - minimalDiagramPoint.X) / 2;
			}
			else
			{
				entireGridCellsSize = ERD.StartPoint.X;
			}
			int entireGridCellsSize2;
			if (centerVertically)
			{
				entireGridCellsSize2 = DiagramControl.GetEntireGridCellsSize((xtraScrollableControlERD.ClientSize.Height - ERD.Diagram.LastDiagramBox.Height - minimalDiagramPoint.Y) / 2, toAbsoluteFloor: true);
				entireGridCellsSize2 = (xtraScrollableControlERD.ClientSize.Height - ERD.Diagram.LastDiagramBox.Height - minimalDiagramPoint.Y) / 2;
			}
			else
			{
				entireGridCellsSize2 = ERD.StartPoint.Y;
			}
			ERD.StartPoint = new Point(entireGridCellsSize, entireGridCellsSize2).GetNotLessThan(-minimalDiagramPoint.X, -minimalDiagramPoint.Y);
			AdjustGridStartPoint();
		}
		return current.Subtract(ERD.StartPoint);
	}

	public override bool Save()
	{
		try
		{
			bool flag = false;
			if (base.Edit.IsEdited)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
				if (!Licenses.CheckRepositoryVersionAfterLogin())
				{
					flag = true;
				}
				else
				{
					if (IsTitleValid())
					{
						if (base.UserControlHelpers.IsSearchActive)
						{
							ClearHighlights(keepSearchActive: true);
						}
						ERD.ClosePostItEditor();
						ModuleSyncRow moduleSyncRow = new ModuleSyncRow(ModuleId, moduleTitleTextEdit.Text, PrepareValue.GetHtmlText(moduleHtmlUserControl.PlainText, moduleHtmlUserControl.HtmlText), moduleHtmlUserControl.PlainTextForSearch, LinkStyleEnum.TypeToString(erdLinkStyle), erdShowTypes, DisplayDocumentationNameModeEnum.TypeToString(displayDocumentationNameMode), erdShowNullable)
						{
							CustomFields = customFields
						};
						customFieldsPanelControl.SetCustomFieldsValuesInRow(moduleSyncRow);
						if (DB.Module.Update(moduleSyncRow, FindForm()))
						{
							moduleHtmlUserControl.SetHtmlTextAsOriginal();
							DBTreeMenu.RefeshNodeTitle(ModuleId, moduleTitleTextEdit.Text, SharedObjectTypeEnum.ObjectType.Module);
							IsModuleEdited = false;
							IsModuleSettingsEdited = false;
							customFieldsPanelControl.UpdateDefinitionValues();
							moduleSyncRow.CustomFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
							DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Module, moduleSyncRow.Id);
							SaveHistory(moduleSyncRow);
						}
						else
						{
							flag = true;
						}
						if (!flag)
						{
							UpdateTitle(moduleTitleTextEdit.Text);
							base.Edit.SetUnchanged();
						}
					}
					else
					{
						CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
						GeneralMessageBoxesHandling.Show("Title of the Subject Area can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
						flag = true;
					}
					List<NodeDB> nodes = ERD.Elements.Nodes.Select((Node x) => new NodeDB(x)).ToList();
					IEnumerable<int> ids = from x in manager.DeletedNodes
						select new NodeDB(x) into x
						where x.node_id.HasValue
						select x.node_id.Value;
					if (!DB.ErdNode.DeleteNodes(ids, FindForm()) || DB.ErdNode.InsertOrUpdateNodes(nodes, FindForm()) != 0)
					{
						flag = true;
					}
					if (!flag && !DB.Module.InsertModuleObjects(objectsToInsertIntoModule, moduleId, FindForm()))
					{
						flag = true;
					}
					IEnumerable<NodeColumnDB> columns = ERD.Elements.Nodes.SelectMany((Node x) => x.Columns);
					if (DB.NodeColumn.InsertOrUpdateErdNodeColumns(columns, FindForm()) != 0)
					{
						flag = true;
					}
					List<int> relations = manager.DeletedLinks.Select((Link x) => x.RelationId).ToList();
					if (!DB.Relation.Delete(relations, FindForm()))
					{
						flag = true;
					}
					IEnumerable<LinkDB> links = ERD.Elements.Links.Select((Link x) => new LinkDB(x));
					if (DB.ErdLink.InsertOrUpdateLinks(links, FindForm()) != 0)
					{
						flag = true;
					}
					List<PostItDB> postIts = ERD.Elements.PostIts.Select((PostIt x) => new PostItDB(x)).ToList();
					IEnumerable<int> ids2 = from x in manager.DeletedPostIts
						select new PostItDB(x) into x
						select x.post_it_id;
					if (!DB.ErdPostIt.DeletePostIts(ids2) || DB.ErdPostIt.InsertOrUpdatePostIts(postIts) != 0)
					{
						flag = true;
					}
					if (SchemaImportsAndChangesSupport.UpdateSchemaImportsAndChangesComments(FindForm()))
					{
						SetTabPageTitle(isEdited: false, schemaImportsAndChangesXtraTabPage);
					}
					else
					{
						flag = true;
					}
					if (!flag)
					{
						foreach (Node item in objectsToInsertIntoModule)
						{
							DBTreeMenu.AddObjectToModule(databaseId, item.TableId, moduleId, item.Schema, item.Label, item.Title, item.TableSource, item.ObjectType, item.TableSubtype);
							base.MainControl.RefreshObjectProgress(showWaitForm: false, item.TableId, item.ObjectType);
						}
						foreach (Node item2 in objectsToRemoveFromModule)
						{
							CommonFunctionsDatabase.RemoveObjectFromModule(databaseId, item2.TableId, moduleId, item2.ObjectType);
							base.MainControl.RefreshObjectProgress(showWaitForm: false, item2.TableId, item2.ObjectType);
						}
						base.MainControl.RefreshObjectProgress(showWaitForm: false, ObjectId, ObjectType);
						RefreshERD();
						SetTabPageTitle(isEdited: false, moduleERDXtraTabPage, base.Edit);
						base.Edit.SetUnchanged();
						objectsToInsertIntoModule.Clear();
						objectsToRemoveFromModule.Clear();
						if (IsErdEdited)
						{
							WorkWithDataedoTrackingHelper.TrackFirstInSessionERDiagramEdit();
							this.ErdSavedEvent?.Invoke(this, databaseId);
						}
						IsErdEdited = false;
						if (base.UserControlHelpers.IsSearchActive)
						{
							base.MainControl.OpenCurrentlySelectedSearchRow();
							moduleHtmlUserControl.SetNotChanged();
							if (moduleHtmlUserControl.Highlight())
							{
								base.UserControlHelpers.SetHighlight();
								searchCountLabelControl.Text = moduleHtmlUserControl.Occurrences;
							}
							ForceLostFocus();
						}
					}
				}
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			return !flag;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			return false;
		}
	}

	internal void SetERDEnabled(bool enabled)
	{
		ERD.Enabled = enabled;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == (Keys.F | Keys.Control) && moduleHtmlUserControl.PerformFindActionIfFocused())
			{
				return true;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void moduleXtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (IsERDTabPageActive)
		{
			WorkWithDataedoTrackingHelper.TrackFirstInSessionERDiagramView();
		}
		this.ERDTabPageSelectedEvent?.Invoke(null, new BoolEventArgs(moduleXtraTabControl.SelectedTabPage == moduleERDXtraTabPage));
		base.MainControl.SetERDGroupVisibility(new BoolEventArgs(moduleXtraTabControl.SelectedTabPage == moduleERDXtraTabPage));
		base.MainControl.SetModulePositionButtonsVisibility(new BoolEventArgs(moduleXtraTabControl.SelectedTabPage == moduleDescriptionXtraTabPage));
		if (moduleXtraTabControl.SelectedTabPageIndex == 0)
		{
			SelectedTab.selectedTabCaption = "info";
		}
		else
		{
			SelectedTab.selectedTabCaption = moduleXtraTabControl.SelectedTabPage.Text;
		}
		RefreshERD();
		IsModuleEdited = false;
		IsModuleSettingsEdited = false;
		IsErdEdited = false;
		RefreshSchemaImportsAndChanges();
	}

	private void ModuleXtraTabControl_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
	{
		if (e.PrevPage == moduleERDXtraTabPage)
		{
			if (!base.MainControl.ContinueAfterPossibleChanges())
			{
				e.Cancel = true;
			}
			else
			{
				Save();
			}
		}
	}

	private void erdAvailableObjectsGrid_MouseDown(object sender, MouseEventArgs e)
	{
		GridView gridView = (sender as GridControl).MainView as GridView;
		erdAvailableObjectsGridHitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
	}

	private void erdAvailableObjectsGrid_MouseMove(object sender, MouseEventArgs e)
	{
		if (erdAvailableObjectsGridHitInfo == null || e.Button != MouseButtons.Left)
		{
			return;
		}
		GridControl gridControl = sender as GridControl;
		GridView gridView = gridControl.MainView as GridView;
		if (erdAvailableObjectsGridHitInfo.InGroupRow || erdAvailableObjectsGridHitInfo.RowHandle < 0)
		{
			DXMouseEventArgs.GetMouseArgs(e).Handled = true;
			return;
		}
		Rectangle rectangle = new Rectangle(new Point(erdAvailableObjectsGridHitInfo.HitPoint.X - SystemInformation.DragSize.Width / 2, erdAvailableObjectsGridHitInfo.HitPoint.Y - SystemInformation.DragSize.Height / 2), SystemInformation.DragSize);
		if (!rectangle.Contains(new Point(e.X, e.Y)))
		{
			List<Node> list = new List<Node>();
			int[] selectedRows = gridView.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				list.Add(gridView.GetRow(selectedRows[i]) as Node);
			}
			if (list.Count > 0)
			{
				int topRowIndex = gridView.TopRowIndex;
				_ = gridView.FocusedRowHandle;
				gridControl.DoDragDrop(list, DragDropEffects.Copy);
				gridView.TopRowIndex = topRowIndex;
				gridView.FocusedRowHandle = int.MinValue;
			}
		}
	}

	private void ERD_DragEnter(object sender, DragEventArgs e)
	{
		SetDragDropEffect(sender, e);
	}

	private void SetDragDropEffect(object sender, DragEventArgs e)
	{
		e.Effect = DragDropEffects.Copy;
		if (e.Data.GetData(typeof(List<Node>)) is List<Node>)
		{
			e.Effect = DragDropEffects.Copy;
		}
		else if (e.Data.GetData(typeof(TreeListNode)) is TreeListNode)
		{
			DBTreeNode dbTreeNode = GetDbTreeNode(e);
			if (CheckIfObjectIsProperForErd(dbTreeNode))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void ERD_DragDrop(object sender, DragEventArgs e)
	{
		base.MainControl.EnableFocusNodeChangedEventActions();
		List<int> ignoredNodes = new List<int>();
		List<int> otherModuleNodes = new List<int>();
		List<int> outsideModuleNodes = new List<int>();
		List<Node> nodesPresentOnErd = new List<Node>();
		List<Node> list = new List<Node>();
		Point destinationPoint = ((DiagramControl)sender).PointToClient(new Point(e.X, e.Y).Subtract(ERD.StartPoint));
		if (e.Data.GetData(typeof(List<Node>)) is List<Node>)
		{
			DragDropFromErdList(sender, e, destinationPoint, ignoredNodes, otherModuleNodes, outsideModuleNodes, list);
		}
		else
		{
			if (!(e.Data.GetData(typeof(TreeListNode)) is TreeListNode))
			{
				return;
			}
			DragDropFromMainTree(sender, e, destinationPoint, ignoredNodes, otherModuleNodes, outsideModuleNodes, nodesPresentOnErd, list);
		}
		UpdateERDNodes(destinationPoint, new List<Node>(), ignoredNodes, otherModuleNodes, outsideModuleNodes, list, nodesPresentOnErd, calculatePositon: false);
		AddNewlyCreatedNodesIntoAddToModuleList(list?.ToArray());
	}

	private void AddNewlyCreatedNodesIntoAddToModuleList(params Node[] newlyCreatedNodes)
	{
		if (newlyCreatedNodes != null && newlyCreatedNodes.Length != 0 && autoAddToModuleCheckEdit.Checked)
		{
			AddNodesIntoAddToModuleList(newlyCreatedNodes);
		}
	}

	private void AddNodesIntoAddToModuleList(params Node[] nodes)
	{
		objectsToInsertIntoModule.AddRange(nodes.Where((Node x) => !objectsToInsertIntoModule.Any((Node y) => y.TableId == x.TableId)));
		nodes.ForEach(delegate(Node x)
		{
			objectsToRemoveFromModule.Remove(x);
		});
	}

	private bool RemoveNodeFromAddToModuleList(Node node)
	{
		if (node == null || !CommonFunctionsDatabase.AskIfDeleting(node, fromSubjectArea: true, moduleTitleTextEdit.Text, FindForm()))
		{
			return false;
		}
		node.IsInSubjectArea = false;
		objectsToInsertIntoModule.Remove(node);
		objectsToRemoveFromModule.Add(node);
		return true;
	}

	private void DragDropFromErdList(object sender, DragEventArgs e, Point destinationPoint, List<int> ignoredNodes, List<int> otherModuleNodes, List<int> outsideModuleNodes, List<Node> newlyCreatedNodes)
	{
		List<Node> nodes = e.Data.GetData(typeof(List<Node>)) as List<Node>;
		AddNodesToERD(destinationPoint, nodes, ignoredNodes, otherModuleNodes, outsideModuleNodes, newlyCreatedNodes);
		RefreshIgnoredNodesData();
	}

	private void AddNodesToERD(Point destinationPoint, List<Node> nodes, List<int> ignoredNodes, List<int> otherModuleNodes, List<int> outsideModuleNodes, List<Node> newlyCreatedNodes)
	{
		if (manager.RemainingERDObjectsToAdd(nodes.Count, FindForm()) < 1)
		{
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
		SuspendLayout();
		foreach (Node node in nodes)
		{
			AddNode(node, ignoredNodes, otherModuleNodes, outsideModuleNodes);
			node.SubjectAreaId = ModuleId;
			newlyCreatedNodes.Add(node);
		}
	}

	private void UpdateERDNodes(Point destinationPoint, List<Node> nodes, List<int> ignoredNodes, List<int> otherModuleNodes, List<int> outsideModuleNodes, List<Node> newlyCreatedNodes, List<Node> nodesPresentOnErd, bool calculatePositon)
	{
		Box box = new Box();
		if (ignoredNodes.Count > 0)
		{
			box.UpdateMinMax(manager.AddIgnoredNodes(ignoredNodes, destinationPoint, setDiagramControlValidPosition: true, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, calculatePositon, erdShowNullable));
		}
		if (otherModuleNodes.Count > 0)
		{
			box.UpdateMinMax(manager.AddOtherModuleNodes(otherModuleNodes, destinationPoint, setDiagramControlValidPosition: true, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, calculatePositon, erdShowNullable));
		}
		if (outsideModuleNodes.Count > 0)
		{
			box.UpdateMinMax(manager.AddOutsideModuleNodes(outsideModuleNodes, destinationPoint, setDiagramControlValidPosition: true, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, calculatePositon, erdShowNullable));
		}
		if (nodesPresentOnErd.Count > 0 || newlyCreatedNodes.Count > 0)
		{
			ERD.UnselectAllNodes();
			foreach (Node item in nodesPresentOnErd)
			{
				ERD.SelectNode(item.Key);
			}
			foreach (Node newlyCreatedNode in newlyCreatedNodes)
			{
				ERD.SelectNode(newlyCreatedNode.Key);
			}
		}
		List<Node> nodes2 = manager.diagram.Elements.Nodes.Where((Node x) => x.Selected).ToList();
		ReloadIgnoredNodesPanel(nodes2);
		SetTabPageTitle(isEdited: true, moduleERDXtraTabPage, base.Edit);
		ERD.RefreshControlSize(calculateDiagramBox: true, ERD.StartPoint);
		if (box != null)
		{
			RefreshERDView(new Point(0, 0), calculateDiagramBox: true);
			ScrollForArea(box, ERD.StartPoint);
		}
		ResumeLayout();
		CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
		ERD.Focus();
	}

	private void DragDropFromMainTree(object sender, DragEventArgs e, Point destinationPoint, List<int> ignoredNodes, List<int> otherModuleNodes, List<int> outsideModuleNodes, List<Node> nodesPresentOnErd, List<Node> newlyCreatedNodes)
	{
		List<Node> list = new List<Node>();
		DBTreeNode dbTreeNode = GetDbTreeNode(e);
		if (dbTreeNode == null)
		{
			return;
		}
		IEnumerable<DBTreeNode> enumerable = null;
		if (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View || dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			enumerable = new List<DBTreeNode>(new DBTreeNode[1] { dbTreeNode });
		}
		else if (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
		{
			enumerable = dbTreeNode.Nodes.Where((DBTreeNode x) => CheckIfObjectIsProperForErd(x)).ToList();
		}
		else if (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			enumerable = (from x in dbTreeNode.Nodes.Where((DBTreeNode x) => x.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table) || x.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.View) || x.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Structure)).SelectMany((DBTreeNode x) => x.Nodes)
				where CheckIfObjectIsProperForErd(x)
				select x).ToList();
		}
		else if (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database)
		{
			enumerable = dbTreeNode.Nodes.Where((DBTreeNode x) => CheckIfObjectIsProperForErd(x)).ToList();
		}
		List<DBTreeNode> list2 = new List<DBTreeNode>();
		foreach (DBTreeNode node in enumerable)
		{
			Node node2 = manager.diagram.Elements.Nodes.FirstOrDefault((Node y) => (node.Id == y.Key && node.ObjectType == y.ObjectType) ? true : false);
			if (node2 != null)
			{
				list.Add(node2);
			}
			else
			{
				list2.Add(node);
			}
		}
		if (list2.Count() > 0)
		{
			if (manager.RemainingERDObjectsToAdd(list2.Count(), FindForm()) < 1 || !CheckIfObjectIsProperForErd(dbTreeNode))
			{
				return;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			SuspendLayout();
			foreach (DBTreeNode item in list2)
			{
				Node node3 = manager.GetNodeFromList(item.Id);
				if (node3 == null)
				{
					ErdTableObject tableAsErdTableObject = DB.Table.GetTableAsErdTableObject(item.DatabaseId, item.Id);
					if (tableAsErdTableObject != null)
					{
						node3 = new Node(databaseId, ShowSchema, tableAsErdTableObject.DatabaseId, tableAsErdTableObject.DatabaseMultipleSchemas, tableAsErdTableObject.DatabaseShowSchema, tableAsErdTableObject.DatabaseShowSchemaOverride, tableAsErdTableObject.TableId, tableAsErdTableObject.Name, Point.Empty, (!tableAsErdTableObject.ObjectType.Equals("TABLE")) ? NodeTypeEnum.NodeType.View : NodeTypeEnum.NodeType.Table, tableAsErdTableObject.Status == "D", visible: false);
						node3.SubjectAreaId = ModuleId;
						node3.TableId = tableAsErdTableObject.TableId;
						node3.DatabaseType = DatabaseTypeEnum.StringToType(tableAsErdTableObject.DatabaseType);
						node3.DatabaseName = manager.ConvertToString(tableAsErdTableObject.DatabaseName);
						node3.Schema = tableAsErdTableObject.Schema;
						node3.Title = tableAsErdTableObject.Title;
						node3.NodeSource = ERDNodeSource.OutsideTheModule;
						manager.IgnoredNodes.Add(node3);
					}
				}
				if (node3 != null)
				{
					node3.SubjectAreaId = ModuleId;
					newlyCreatedNodes.Add(node3);
					AddNode(node3, ignoredNodes, otherModuleNodes, outsideModuleNodes);
				}
			}
			List<int> list3 = new List<int>();
			list3.AddRange(newlyCreatedNodes.Select((Node x) => x.Key));
		}
		nodesPresentOnErd.AddRange(list);
	}

	private void AddNode(Node node, List<int> ignoredNodes, List<int> otherModuleNodes, List<int> outsideModuleNodes)
	{
		switch (node.NodeSource)
		{
		case ERDNodeSource.Module:
			ignoredNodes.Add(node.Key);
			break;
		case ERDNodeSource.ConnectedWithModule:
			otherModuleNodes.Add(node.Key);
			break;
		case ERDNodeSource.OutsideTheModule:
			outsideModuleNodes.Add(node.Key);
			break;
		}
	}

	private DBTreeNode GetDbTreeNode(DragEventArgs e)
	{
		TreeListNode node = (TreeListNode)e.Data.GetData("DevExpress.XtraTreeList.Nodes.TreeListNode");
		return base.MainControl.MetadataTreeList.GetDataRecordByNode(node) as DBTreeNode;
	}

	private bool CheckIfObjectIsProperForErd(DBTreeNode dbTreeNode)
	{
		if (dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Table && dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.View && dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Structure && (dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module || (!(dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table)) && !(dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.View)) && !(dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Structure)))) && dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Module)
		{
			if (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database)
			{
				if (!(dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table)) && !(dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.View)))
				{
					return dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Structure);
				}
				return true;
			}
			return false;
		}
		return true;
	}

	private void ERD_SelectionChanged(CanvasObject[] objects)
	{
		SelectedObjects = objects;
		bool visible = false;
		bool isRelation = false;
		bool value = false;
		bool isTable = false;
		bool hasMultipleLevelColumns = false;
		Node node = null;
		CanvasObject[] selectedObjects = SelectedObjects;
		if (selectedObjects != null && selectedObjects.Count() == 1)
		{
			RectangularObject obj = SelectedObjects.FirstOrDefault() as RectangularObject;
			node = obj as Node;
			Link link = SelectedObjects.FirstOrDefault() as Link;
			if (obj != null)
			{
				visible = true;
			}
			if (node != null)
			{
				hasMultipleLevelColumns = DB.Table.HasMultipleLevelColumns(node.TableId);
				isTable = node.Type == NodeTypeEnum.NodeType.Table;
			}
			else if (link != null)
			{
				visible = (isRelation = true);
				value = link.UserRelation;
			}
		}
		else
		{
			CanvasObject[] selectedObjects2 = SelectedObjects;
			if (selectedObjects2 != null && selectedObjects2.Count() > 1)
			{
				visible = true;
			}
		}
		this.DeleteSelectedERDNode?.Invoke(null, new ErdButtonArgs(visible, isRelation, isTable, hasMultipleLevelColumns, node?.ObjectType, manager.DatabaseClass, value));
	}

	public void ChangeTab(SharedObjectTypeEnum.ObjectType? type)
	{
		if (type.HasValue)
		{
			switch (type.GetValueOrDefault())
			{
			case SharedObjectTypeEnum.ObjectType.Module:
				moduleXtraTabControl.SelectedTabPage = moduleDescriptionXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.Erd:
				moduleXtraTabControl.SelectedTabPage = moduleERDXtraTabPage;
				break;
			}
		}
	}

	private void SetFunctionality()
	{
		SchemaImportsAndChangesUserControl.SetFunctionality();
		SetRichEditControlBackground();
	}

	private void erdAvailableObjectsGrid_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		if (erdAvailableObjectsGridView.CalcHitInfo(e.Location).InGroupRow)
		{
			DXMouseEventArgs.GetMouseArgs(e).Handled = true;
			return;
		}
		GridView gridView = (sender as GridControl).MainView as GridView;
		if (gridView.SelectedRowsCount == 0 || manager.RemainingERDObjectsToAdd(1, FindForm()) < 1)
		{
			return;
		}
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			Node node = gridView.GetRow(gridView.GetSelectedRows()[0]) as Node;
			node.SubjectAreaId = ModuleId;
			int key = node.Key;
			Box box = null;
			if (node != null)
			{
				AddNewlyCreatedNodesIntoAddToModuleList(node);
				switch (node.NodeSource)
				{
				case ERDNodeSource.Module:
					box = manager.AddIgnoredNode(key, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, calculatePositon: true, erdShowNullable);
					break;
				case ERDNodeSource.ConnectedWithModule:
					box = manager.AddOtherModuleNode(key, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, calculatePositon: true, erdShowNullable);
					break;
				case ERDNodeSource.OutsideTheModule:
					box = manager.AddOutsideModuleNode(key, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, calculatePositon: true, erdShowNullable);
					break;
				}
				ERD.UnselectAllNodes();
				ERD.SelectNode(node.Key);
			}
			ERD.Elements.Links.ToList().ForEach(delegate(Link x)
			{
				x.SubjecrAreaId = ModuleId;
			});
			List<Node> nodes = manager.diagram.Elements.Nodes.Where((Node x) => x.Selected).ToList();
			ReloadIgnoredNodesPanel(nodes);
			RefreshERDView(default(Point), calculateDiagramBox: true);
			if (box != null)
			{
				ScrollForArea(box, ERD.StartPoint);
			}
			SetTabPageTitle(isEdited: true, moduleERDXtraTabPage, base.Edit);
			ERD.Focus();
			erdAvailableObjectsGridView.FocusedRowHandle = int.MinValue;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
		}
	}

	[DllImport("user32.dll")]
	public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

	private void ModuleUserControl_Load(object sender, EventArgs e)
	{
		ShowScrollBar(erdSuggestedObjectsGrid.Handle, 0, bShow: false);
		moduleHtmlUserControl.ProgressValueChanged += SetRichEditControlBackground;
		moduleHtmlUserControl.IsEditorFocused += SetRichEditControlBackgroundWhileFocused;
	}

	private void erdObjectsTypeTablesCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		RefreshIgnoredNodesData(ERD.Elements.Nodes.Where((Node x) => x.Selected).ToList());
	}

	private void clearErdFilterLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			ClearIgnoredObjectsFilter();
		}
	}

	private void ClearIgnoredObjectsFilter()
	{
		CheckEdit checkEdit = erdObjectsTypeTablesCheckEdit;
		bool @checked = (erdObjectsTypeViewsCheckEdit.Checked = true);
		checkEdit.Checked = @checked;
		allDocsCheckEdit.Checked = false;
		erdObjectsFilterTextEdit.Text = string.Empty;
	}

	private void erdObjectsTypeViewsCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		RefreshIgnoredNodesData(ERD.Elements.Nodes.Where((Node x) => x.Selected).ToList());
	}

	private void erdObjectsTypeObjectsCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		RefreshIgnoredNodesData(ERD.Elements.Nodes.Where((Node x) => x.Selected).ToList());
	}

	private void allDocsCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		manager.ReloadTables(null, null, databaseId, ModuleId, setLinks: false, allDocsCheckEdit.Checked);
		RefreshIgnoredNodesData(ERD.Elements.Nodes.Where((Node x) => x.Selected).ToList());
		AllDocs = allDocsCheckEdit.Checked;
	}

	private void erdObjectsFilterTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		RefreshIgnoredNodesData(ERD.Elements.Nodes.Where((Node x) => x.Selected).ToList());
	}

	private void erdAvailableObjectsGridView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column.FieldName.Equals("IsSuggested") && e.DisplayText.Equals("Checked"))
		{
			e.DisplayText = "Suggested entities";
		}
		else if (e.Column.FieldName.Equals("IsSuggested") && e.DisplayText.Equals("Unchecked"))
		{
			e.DisplayText = "Other entities";
		}
	}

	private void erdAvailableObjectsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		if (erdAvailableObjectsGridHitInfo.InGroupRow || erdAvailableObjectsGridHitInfo.RowHandle < 0)
		{
			DXMouseEventArgs.GetMouseArgs(e).Handled = true;
		}
	}

	private void erdAvailableObjectsGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (erdAvailableObjectsGridView.IsGroupRow(e.RowHandle))
		{
			e.Appearance.BackColor = erdAvailableObjectsGridView.PaintAppearance.GroupRow.BackColor;
			e.HighPriority = true;
		}
	}

	private void erdAvailableObjectsGridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
	{
		e.Appearance.BackColor = SkinColors.GridViewBandBackColor;
		e.Appearance.ForeColor = SkinColors.GridViewBandForeColor;
	}

	private void erdAvailableObjectsGridView_GroupRowCollapsing(object sender, RowAllowEventArgs e)
	{
		e.Allow = false;
	}

	private void autoAddToModuleCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		LastConnectionInfo.SetAutoAddToModule(autoAddToModuleCheckEdit.Checked);
	}

	private void ModuleTitleTextEdit_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
	{
		string viewHistoryMenuItemCaption = "View History";
		if (!e.Menu.Items.Any((DXMenuItem x) => x.Caption == viewHistoryMenuItemCaption))
		{
			DXMenuItem item = new DXMenuItem(viewHistoryMenuItemCaption, ViewHistoryClicked_DXMenuItem, Resources.search_16);
			e.Menu.Items.Add(item);
		}
	}

	public void ViewHistoryClicked_DXMenuItem(object sender, EventArgs e)
	{
		ViewHistoryForField("title");
	}

	public void ViewHistoryForField(string fieldName)
	{
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == fieldName)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.SetParameters(ObjectId, fieldName, ObjectName, ObjectSchema, DatabaseShowSchema, DatabaseShowSchemaOverride, moduleTitleAndDescriptionHistory?.Title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), SharedObjectTypeEnum.TypeToString(ObjectType), SharedObjectSubtypeEnum.TypeToString(ObjectType, base.Subtype), null, null);
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void CustomFieldsPanelControl_ShowHistoryClick(object sender, EventArgs e)
	{
		if (e is TextEventArgs textEventArgs && textEventArgs.Text.StartsWith("field"))
		{
			ViewHistoryForField(textEventArgs.Text);
		}
	}

	private void SaveHistory(ModuleSyncRow moduleRow)
	{
		bool saveTitle = HistoryGeneralHelper.CheckAreValuesDiffrent(moduleTitleAndDescriptionHistory?.Title, moduleTitleTextEdit.Text);
		bool saveDescription = HistoryGeneralHelper.CheckAreHtmlValuesAreDiffrent(moduleRow?.DescriptionPlain, moduleRow?.Description, moduleTitleAndDescriptionHistory?.DescriptionPlain, moduleTitleAndDescriptionHistory?.Description);
		moduleTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
		{
			ObjectId = moduleId,
			Title = moduleTitleTextEdit.Text,
			Description = PrepareValue.GetHtmlText(DescriptionHtmlUserControl?.PlainText, DescriptionHtmlUserControl?.HtmlText),
			DescriptionPlain = DescriptionHtmlUserControl?.PlainText
		};
		HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTablePanel(moduleId, moduleCustomFieldsForHistory, customFields, databaseId, ObjectType);
		moduleCustomFieldsForHistory = new Dictionary<string, string>();
		CustomFieldDefinition[] customFieldsData = customFields.CustomFieldsData;
		foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
		{
			moduleCustomFieldsForHistory.Add(customFieldDefinition.CustomField.FieldName, customFieldDefinition.FieldValue);
		}
		DB.History.InsertHistoryRow(databaseId, moduleId, moduleTitleAndDescriptionHistory?.Title, moduleTitleAndDescriptionHistory?.Description, moduleTitleAndDescriptionHistory?.DescriptionPlain, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), saveTitle, saveDescription, ObjectType);
		if (DescriptionHtmlUserControl?.ModuleRow != null)
		{
			DescriptionHtmlUserControl.ModuleRow.Title = moduleTitleTextEdit.Text;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.PanelControls.ModuleUserControl));
		this.iconModuleProcedureRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.typeModuleProcedureRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.moduleXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.moduleDescriptionXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.databaseLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.searchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.customFieldsPanelControl = new Dataedo.App.UserControls.CustomFieldsPanelControl();
		this.moduleHtmlUserControl = new Dataedo.App.UserControls.HtmlUserControl();
		this.moduleTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.customFieldsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
		this.moduleERDXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.erdPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.xtraScrollableControlERD = new DevExpress.XtraEditors.XtraScrollableControl();
		this.ERD = new Dataedo.App.Tools.ERD.Diagram.DiagramControl();
		this.availableObjectsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.autoAddToModuleCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.helpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.allDocsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.clearErdFilterLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.erdObjectsTypeViewsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.erdObjectsTypeTablesCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.erdObjectsFilterTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.erdSuggestedObjectsGrid = new DevExpress.XtraGrid.GridControl();
		this.erdAvailableObjectsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.displayNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.displayNameRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.isSuggested = new DevExpress.XtraGrid.Columns.GridColumn();
		this.toolTipControllerUserControl = new Dataedo.App.UserControls.ToolTipControllerUserControl(this.components);
		this.erdObjectsTypeObjectsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.erdAvailableObjectsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.schemaImportsAndChangesXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.schemaImportsAndChangesUserControl = new Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesUserControl();
		this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
		this.nodeImageList = new System.Windows.Forms.ImageList(this.components);
		this.moduleTitleErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
		this.moduleImageOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.moduleStatusUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.moduleTextEdit = new DevExpress.XtraEditors.TextEdit();
		((System.ComponentModel.ISupportInitialize)this.iconModuleProcedureRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeModuleProcedureRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleXtraTabControl).BeginInit();
		this.moduleXtraTabControl.SuspendLayout();
		this.moduleDescriptionXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControl).BeginInit();
		this.databaseLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.moduleTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		this.moduleERDXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).BeginInit();
		this.splitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.erdPanelControl).BeginInit();
		this.erdPanelControl.SuspendLayout();
		this.xtraScrollableControlERD.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.availableObjectsLayoutControl).BeginInit();
		this.availableObjectsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.autoAddToModuleCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allDocsCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeViewsCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeTablesCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsFilterTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdSuggestedObjectsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdAvailableObjectsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.displayNameRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeObjectsCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdAvailableObjectsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeObjectsCheckEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		this.schemaImportsAndChangesXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.moduleTitleErrorProvider).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleTextEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.iconModuleProcedureRepositoryItemPictureEdit.AllowFocused = false;
		this.iconModuleProcedureRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconModuleProcedureRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconModuleProcedureRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconModuleProcedureRepositoryItemPictureEdit.Name = "iconModuleProcedureRepositoryItemPictureEdit";
		this.iconModuleProcedureRepositoryItemPictureEdit.ShowMenu = false;
		this.typeModuleProcedureRepositoryItemLookUpEdit.AutoHeight = false;
		this.typeModuleProcedureRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeModuleProcedureRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("title", "title")
		});
		this.typeModuleProcedureRepositoryItemLookUpEdit.DisplayMember = "title";
		this.typeModuleProcedureRepositoryItemLookUpEdit.Name = "typeModuleProcedureRepositoryItemLookUpEdit";
		this.typeModuleProcedureRepositoryItemLookUpEdit.NullText = "";
		this.typeModuleProcedureRepositoryItemLookUpEdit.ShowFooter = false;
		this.typeModuleProcedureRepositoryItemLookUpEdit.ShowHeader = false;
		this.typeModuleProcedureRepositoryItemLookUpEdit.ShowLines = false;
		this.typeModuleProcedureRepositoryItemLookUpEdit.ValueMember = "name";
		this.moduleXtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.moduleXtraTabControl.Location = new System.Drawing.Point(0, 69);
		this.moduleXtraTabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.moduleXtraTabControl.Name = "moduleXtraTabControl";
		this.moduleXtraTabControl.SelectedTabPage = this.moduleDescriptionXtraTabPage;
		this.moduleXtraTabControl.Size = new System.Drawing.Size(1182, 577);
		this.moduleXtraTabControl.TabIndex = 1;
		this.moduleXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.moduleDescriptionXtraTabPage, this.moduleERDXtraTabPage, this.schemaImportsAndChangesXtraTabPage });
		this.moduleXtraTabControl.ToolTipController = this.toolTipControllerUserControl;
		this.moduleXtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(moduleXtraTabControl_SelectedPageChanged);
		this.moduleXtraTabControl.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(ModuleXtraTabControl_SelectedPageChanging);
		this.moduleDescriptionXtraTabPage.Controls.Add(this.databaseLayoutControl);
		this.moduleDescriptionXtraTabPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.moduleDescriptionXtraTabPage.Name = "moduleDescriptionXtraTabPage";
		this.moduleDescriptionXtraTabPage.Size = new System.Drawing.Size(1180, 552);
		this.moduleDescriptionXtraTabPage.Text = "Subject Area";
		this.databaseLayoutControl.AllowCustomization = false;
		this.databaseLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.databaseLayoutControl.Controls.Add(this.searchCountLabelControl);
		this.databaseLayoutControl.Controls.Add(this.labelControl1);
		this.databaseLayoutControl.Controls.Add(this.customFieldsPanelControl);
		this.databaseLayoutControl.Controls.Add(this.moduleHtmlUserControl);
		this.databaseLayoutControl.Controls.Add(this.moduleTitleTextEdit);
		this.databaseLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.databaseLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.databaseLayoutControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.databaseLayoutControl.Name = "databaseLayoutControl";
		this.databaseLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(332, 17, 250, 350);
		this.databaseLayoutControl.Root = this.layoutControlGroup1;
		this.databaseLayoutControl.Size = new System.Drawing.Size(1180, 552);
		this.databaseLayoutControl.TabIndex = 0;
		this.databaseLayoutControl.Text = "layoutControl1";
		this.searchCountLabelControl.Location = new System.Drawing.Point(69, 62);
		this.searchCountLabelControl.Name = "searchCountLabelControl";
		this.searchCountLabelControl.Size = new System.Drawing.Size(1099, 13);
		this.searchCountLabelControl.StyleController = this.databaseLayoutControl;
		this.searchCountLabelControl.TabIndex = 6;
		this.labelControl1.Location = new System.Drawing.Point(12, 62);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(53, 13);
		this.labelControl1.StyleController = this.databaseLayoutControl;
		this.labelControl1.TabIndex = 5;
		this.labelControl1.Text = "Description";
		this.customFieldsPanelControl.BackColor = System.Drawing.Color.Transparent;
		this.customFieldsPanelControl.Location = new System.Drawing.Point(10, 38);
		this.customFieldsPanelControl.Margin = new System.Windows.Forms.Padding(0);
		this.customFieldsPanelControl.Name = "customFieldsPanelControl";
		this.customFieldsPanelControl.Size = new System.Drawing.Size(1158, 20);
		this.customFieldsPanelControl.TabIndex = 3;
		this.moduleHtmlUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.moduleHtmlUserControl.CanListen = false;
		this.moduleHtmlUserControl.DatabaseRow = null;
		this.moduleHtmlUserControl.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.moduleHtmlUserControl.HtmlText = resources.GetString("moduleHtmlUserControl.HtmlText");
		this.moduleHtmlUserControl.IsHighlighted = false;
		this.moduleHtmlUserControl.Location = new System.Drawing.Point(12, 79);
		this.moduleHtmlUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.moduleHtmlUserControl.Name = "moduleHtmlUserControl";
		this.moduleHtmlUserControl.OccurrencesCount = 0;
		this.moduleHtmlUserControl.OriginalHtmlText = resources.GetString("moduleHtmlUserControl.OriginalHtmlText");
		this.moduleHtmlUserControl.PlainText = "\u00a0";
		this.moduleHtmlUserControl.Size = new System.Drawing.Size(1156, 471);
		this.moduleHtmlUserControl.SplashScreenManager = null;
		this.moduleHtmlUserControl.TabIndex = 4;
		this.moduleHtmlUserControl.TableRow = null;
		this.moduleHtmlUserControl.TermObject = null;
		this.moduleTitleTextEdit.Location = new System.Drawing.Point(97, 12);
		this.moduleTitleTextEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.moduleTitleTextEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.moduleTitleTextEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.moduleTitleTextEdit.Name = "moduleTitleTextEdit";
		this.moduleTitleTextEdit.Properties.MaxLength = 250;
		this.moduleTitleTextEdit.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(ModuleTitleTextEdit_Properties_BeforeShowMenu);
		this.moduleTitleTextEdit.Size = new System.Drawing.Size(411, 22);
		this.moduleTitleTextEdit.StyleController = this.databaseLayoutControl;
		this.moduleTitleTextEdit.TabIndex = 2;
		this.moduleTitleTextEdit.EditValueChanged += new System.EventHandler(moduleTitleTextEdit_EditValueChanged);
		this.layoutControlGroup1.CustomizationFormText = "Root";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.layoutControlItem1, this.emptySpaceItem1, this.layoutControlItem9, this.customFieldsLayoutControlItem, this.layoutControlItem8, this.layoutControlItem10 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1180, 552);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.moduleTitleTextEdit;
		this.layoutControlItem1.CustomizationFormText = "Title:";
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(500, 26);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(500, 26);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(500, 26);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Text = "Title";
		this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(82, 13);
		this.layoutControlItem1.TextToControlDistance = 3;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(500, 0);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(660, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.Control = this.moduleHtmlUserControl;
		this.layoutControlItem9.Location = new System.Drawing.Point(0, 67);
		this.layoutControlItem9.MinSize = new System.Drawing.Size(104, 300);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(1160, 475);
		this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem9.Text = "Description";
		this.layoutControlItem9.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.customFieldsLayoutControlItem.Control = this.customFieldsPanelControl;
		this.customFieldsLayoutControlItem.Location = new System.Drawing.Point(0, 26);
		this.customFieldsLayoutControlItem.Name = "customFieldsLayoutControlItem";
		this.customFieldsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.customFieldsLayoutControlItem.Size = new System.Drawing.Size(1160, 24);
		this.customFieldsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsLayoutControlItem.TextVisible = false;
		this.layoutControlItem8.Control = this.labelControl1;
		this.layoutControlItem8.Location = new System.Drawing.Point(0, 50);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(57, 17);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.layoutControlItem10.Control = this.searchCountLabelControl;
		this.layoutControlItem10.Location = new System.Drawing.Point(57, 50);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.Size = new System.Drawing.Size(1103, 17);
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.moduleERDXtraTabPage.Controls.Add(this.splitContainerControl);
		this.moduleERDXtraTabPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.moduleERDXtraTabPage.Name = "moduleERDXtraTabPage";
		this.moduleERDXtraTabPage.Size = new System.Drawing.Size(1180, 552);
		this.moduleERDXtraTabPage.Text = "ERD";
		this.splitContainerControl.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
		this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
		this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl.Name = "splitContainerControl";
		this.splitContainerControl.Panel1.Controls.Add(this.erdPanelControl);
		this.splitContainerControl.Panel1.MinSize = 300;
		this.splitContainerControl.Panel1.Text = "Panel1";
		this.splitContainerControl.Panel2.Controls.Add(this.availableObjectsLayoutControl);
		this.splitContainerControl.Panel2.Text = "Panel2";
		this.splitContainerControl.Size = new System.Drawing.Size(1180, 552);
		this.splitContainerControl.SplitterPosition = 298;
		this.splitContainerControl.TabIndex = 6;
		this.splitContainerControl.Text = "splitContainerControl";
		this.erdPanelControl.Controls.Add(this.xtraScrollableControlERD);
		this.erdPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.erdPanelControl.Location = new System.Drawing.Point(0, 0);
		this.erdPanelControl.Name = "erdPanelControl";
		this.erdPanelControl.Size = new System.Drawing.Size(872, 552);
		this.erdPanelControl.TabIndex = 10;
		this.xtraScrollableControlERD.AlwaysScrollActiveControlIntoView = false;
		this.xtraScrollableControlERD.Controls.Add(this.ERD);
		this.xtraScrollableControlERD.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraScrollableControlERD.Location = new System.Drawing.Point(2, 2);
		this.xtraScrollableControlERD.Name = "xtraScrollableControlERD";
		this.xtraScrollableControlERD.Size = new System.Drawing.Size(868, 548);
		this.xtraScrollableControlERD.TabIndex = 6;
		this.xtraScrollableControlERD.Resize += new System.EventHandler(xtraScrollableControlERD_Resize);
		this.ERD.AllowDrop = true;
		this.ERD.BackColor = System.Drawing.Color.White;
		this.ERD.ContextShowSchema = false;
		this.ERD.CurrentDatabaseId = 0;
		this.ERD.CustomFieldsSupport = null;
		this.ERD.DisplayDocumentationNameMode = Dataedo.Shared.Enums.DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode.ExternalEntitiesOnly;
		this.ERD.GridStartPoint = new System.Drawing.Point(0, 0);
		this.ERD.LinkStyle = Dataedo.Shared.Enums.LinkStyleEnum.LinkStyle.Straight;
		this.ERD.Location = new System.Drawing.Point(0, 0);
		this.ERD.Name = "ERD";
		this.ERD.ShowColumnNullable = false;
		this.ERD.ShowColumnTypes = false;
		this.ERD.Size = new System.Drawing.Size(979, 510);
		this.ERD.StartPoint = new System.Drawing.Point(0, 0);
		this.ERD.TabIndex = 5;
		this.ERD.Text = "diagramControl1";
		this.ERD.SelectionChanged += new Dataedo.App.Tools.ERD.Diagram.DiagramControl.SelectionChangedHandler(ERD_SelectionChanged);
		this.ERD.DragDrop += new System.Windows.Forms.DragEventHandler(ERD_DragDrop);
		this.ERD.DragEnter += new System.Windows.Forms.DragEventHandler(ERD_DragEnter);
		this.availableObjectsLayoutControl.AllowCustomization = false;
		this.availableObjectsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.availableObjectsLayoutControl.Controls.Add(this.autoAddToModuleCheckEdit);
		this.availableObjectsLayoutControl.Controls.Add(this.helpIconUserControl);
		this.availableObjectsLayoutControl.Controls.Add(this.allDocsCheckEdit);
		this.availableObjectsLayoutControl.Controls.Add(this.clearErdFilterLabelControl);
		this.availableObjectsLayoutControl.Controls.Add(this.erdObjectsTypeViewsCheckEdit);
		this.availableObjectsLayoutControl.Controls.Add(this.erdObjectsTypeTablesCheckEdit);
		this.availableObjectsLayoutControl.Controls.Add(this.erdObjectsFilterTextEdit);
		this.availableObjectsLayoutControl.Controls.Add(this.erdSuggestedObjectsGrid);
		this.availableObjectsLayoutControl.Controls.Add(this.erdObjectsTypeObjectsCheckEdit);
		this.availableObjectsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.availableObjectsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.availableObjectsLayoutControl.Name = "availableObjectsLayoutControl";
		this.availableObjectsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3441, 434, 936, 591);
		this.availableObjectsLayoutControl.OptionsFocus.EnableAutoTabOrder = false;
		this.availableObjectsLayoutControl.Root = this.layoutControlGroup2;
		this.availableObjectsLayoutControl.Size = new System.Drawing.Size(298, 552);
		this.availableObjectsLayoutControl.TabIndex = 6;
		this.availableObjectsLayoutControl.Text = "layoutControl1";
		this.availableObjectsLayoutControl.ToolTipController = this.toolTipControllerUserControl;
		this.autoAddToModuleCheckEdit.Location = new System.Drawing.Point(2, 2);
		this.autoAddToModuleCheckEdit.Name = "autoAddToModuleCheckEdit";
		this.autoAddToModuleCheckEdit.Properties.Caption = "Automatically add to Subject Area";
		this.autoAddToModuleCheckEdit.Size = new System.Drawing.Size(294, 20);
		this.autoAddToModuleCheckEdit.StyleController = this.availableObjectsLayoutControl;
		this.autoAddToModuleCheckEdit.TabIndex = 16;
		this.autoAddToModuleCheckEdit.CheckedChanged += new System.EventHandler(autoAddToModuleCheckEdit_CheckedChanged);
		this.helpIconUserControl.AutoPopDelay = 5000;
		this.helpIconUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.helpIconUserControl.KeepWhileHovered = false;
		this.helpIconUserControl.Location = new System.Drawing.Point(67, 24);
		this.helpIconUserControl.MaximumSize = new System.Drawing.Size(24, 24);
		this.helpIconUserControl.MaxToolTipWidth = 500;
		this.helpIconUserControl.MinimumSize = new System.Drawing.Size(24, 24);
		this.helpIconUserControl.Name = "helpIconUserControl";
		this.helpIconUserControl.Size = new System.Drawing.Size(24, 24);
		this.helpIconUserControl.TabIndex = 15;
		this.helpIconUserControl.ToolTipHeader = "";
		this.helpIconUserControl.ToolTipText = "Suggested entities are objects from current Subject Area or related to those currently selected or already on ERD.";
		this.allDocsCheckEdit.Location = new System.Drawing.Point(189, 50);
		this.allDocsCheckEdit.Name = "allDocsCheckEdit";
		this.allDocsCheckEdit.Properties.Caption = "All docs";
		this.allDocsCheckEdit.Size = new System.Drawing.Size(58, 20);
		this.allDocsCheckEdit.StyleController = this.availableObjectsLayoutControl;
		this.allDocsCheckEdit.TabIndex = 14;
		this.allDocsCheckEdit.CheckedChanged += new System.EventHandler(allDocsCheckEdit_CheckedChanged);
		this.clearErdFilterLabelControl.AllowHtmlString = true;
		this.clearErdFilterLabelControl.Appearance.Options.UseTextOptions = true;
		this.clearErdFilterLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clearErdFilterLabelControl.Location = new System.Drawing.Point(251, 50);
		this.clearErdFilterLabelControl.MaximumSize = new System.Drawing.Size(40, 19);
		this.clearErdFilterLabelControl.MinimumSize = new System.Drawing.Size(40, 19);
		this.clearErdFilterLabelControl.Name = "clearErdFilterLabelControl";
		this.clearErdFilterLabelControl.Size = new System.Drawing.Size(40, 19);
		this.clearErdFilterLabelControl.StyleController = this.availableObjectsLayoutControl;
		this.clearErdFilterLabelControl.TabIndex = 13;
		this.clearErdFilterLabelControl.Text = "<href>Default</href>";
		this.clearErdFilterLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(clearErdFilterLabelControl_HyperlinkClick);
		this.erdObjectsTypeViewsCheckEdit.EditValue = true;
		this.erdObjectsTypeViewsCheckEdit.Location = new System.Drawing.Point(59, 50);
		this.erdObjectsTypeViewsCheckEdit.Name = "erdObjectsTypeViewsCheckEdit";
		this.erdObjectsTypeViewsCheckEdit.Properties.Caption = "Views";
		this.erdObjectsTypeViewsCheckEdit.Size = new System.Drawing.Size(49, 20);
		this.erdObjectsTypeViewsCheckEdit.StyleController = this.availableObjectsLayoutControl;
		this.erdObjectsTypeViewsCheckEdit.TabIndex = 6;
		this.erdObjectsTypeViewsCheckEdit.CheckedChanged += new System.EventHandler(erdObjectsTypeViewsCheckEdit_CheckedChanged);
		this.erdObjectsTypeTablesCheckEdit.EditValue = true;
		this.erdObjectsTypeTablesCheckEdit.Location = new System.Drawing.Point(2, 50);
		this.erdObjectsTypeTablesCheckEdit.Name = "erdObjectsTypeTablesCheckEdit";
		this.erdObjectsTypeTablesCheckEdit.Properties.Caption = "Tables";
		this.erdObjectsTypeTablesCheckEdit.Size = new System.Drawing.Size(53, 20);
		this.erdObjectsTypeTablesCheckEdit.StyleController = this.availableObjectsLayoutControl;
		this.erdObjectsTypeTablesCheckEdit.TabIndex = 6;
		this.erdObjectsTypeTablesCheckEdit.CheckedChanged += new System.EventHandler(erdObjectsTypeTablesCheckEdit_CheckedChanged);
		this.erdObjectsFilterTextEdit.Location = new System.Drawing.Point(2, 74);
		this.erdObjectsFilterTextEdit.Name = "erdObjectsFilterTextEdit";
		this.erdObjectsFilterTextEdit.Properties.NullValuePrompt = "Type here to filter...";
		this.erdObjectsFilterTextEdit.Size = new System.Drawing.Size(294, 20);
		this.erdObjectsFilterTextEdit.StyleController = this.availableObjectsLayoutControl;
		this.erdObjectsFilterTextEdit.TabIndex = 6;
		this.erdObjectsFilterTextEdit.EditValueChanged += new System.EventHandler(erdObjectsFilterTextEdit_EditValueChanged);
		this.erdSuggestedObjectsGrid.Location = new System.Drawing.Point(0, 96);
		this.erdSuggestedObjectsGrid.MainView = this.erdAvailableObjectsGridView;
		this.erdSuggestedObjectsGrid.Margin = new System.Windows.Forms.Padding(0);
		this.erdSuggestedObjectsGrid.Name = "erdSuggestedObjectsGrid";
		this.erdSuggestedObjectsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.iconRepositoryItemPictureEdit, this.displayNameRepositoryItemTextEdit });
		this.erdSuggestedObjectsGrid.Size = new System.Drawing.Size(296, 456);
		this.erdSuggestedObjectsGrid.TabIndex = 13;
		this.erdSuggestedObjectsGrid.ToolTipController = this.toolTipControllerUserControl;
		this.erdSuggestedObjectsGrid.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.False;
		this.erdSuggestedObjectsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.erdAvailableObjectsGridView });
		this.erdSuggestedObjectsGrid.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(erdAvailableObjectsGrid_MouseDoubleClick);
		this.erdSuggestedObjectsGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(erdAvailableObjectsGrid_MouseDown);
		this.erdSuggestedObjectsGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(erdAvailableObjectsGrid_MouseMove);
		this.erdAvailableObjectsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.iconGridColumn, this.displayNameGridColumn, this.isSuggested });
		this.erdAvailableObjectsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.erdAvailableObjectsGridView.GridControl = this.erdSuggestedObjectsGrid;
		this.erdAvailableObjectsGridView.GroupCount = 1;
		this.erdAvailableObjectsGridView.GroupFormat = "[#image]{1} {2}";
		this.erdAvailableObjectsGridView.Name = "erdAvailableObjectsGridView";
		this.erdAvailableObjectsGridView.OptionsBehavior.AutoExpandAllGroups = true;
		this.erdAvailableObjectsGridView.OptionsBehavior.Editable = false;
		this.erdAvailableObjectsGridView.OptionsBehavior.KeepFocusedRowOnUpdate = false;
		this.erdAvailableObjectsGridView.OptionsBehavior.ReadOnly = true;
		this.erdAvailableObjectsGridView.OptionsSelection.MultiSelect = true;
		this.erdAvailableObjectsGridView.OptionsView.ShowColumnHeaders = false;
		this.erdAvailableObjectsGridView.OptionsView.ShowDetailButtons = false;
		this.erdAvailableObjectsGridView.OptionsView.ShowGroupExpandCollapseButtons = false;
		this.erdAvailableObjectsGridView.OptionsView.ShowGroupPanel = false;
		this.erdAvailableObjectsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.erdAvailableObjectsGridView.OptionsView.ShowIndicator = false;
		this.erdAvailableObjectsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.erdAvailableObjectsGridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.isSuggested, DevExpress.Data.ColumnSortOrder.Descending)
		});
		this.erdAvailableObjectsGridView.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(erdAvailableObjectsGridView_CustomDrawGroupRow);
		this.erdAvailableObjectsGridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(erdAvailableObjectsGridView_RowStyle);
		this.erdAvailableObjectsGridView.GroupRowCollapsing += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(erdAvailableObjectsGridView_GroupRowCollapsing);
		this.erdAvailableObjectsGridView.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(erdAvailableObjectsGridView_CustomColumnDisplayText);
		this.erdAvailableObjectsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(erdAvailableObjectsGridView_MouseDown);
		this.iconGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.iconGridColumn.FieldName = "NodeIcon";
		this.iconGridColumn.MaxWidth = 18;
		this.iconGridColumn.MinWidth = 18;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.AllowFocus = false;
		this.iconGridColumn.OptionsColumn.ReadOnly = true;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 18;
		this.iconRepositoryItemPictureEdit.AllowFocused = false;
		this.iconRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.iconRepositoryItemPictureEdit.ReadOnly = true;
		this.displayNameGridColumn.ColumnEdit = this.displayNameRepositoryItemTextEdit;
		this.displayNameGridColumn.FieldName = "DisplayName";
		this.displayNameGridColumn.Name = "displayNameGridColumn";
		this.displayNameGridColumn.OptionsColumn.AllowEdit = false;
		this.displayNameGridColumn.OptionsColumn.AllowFocus = false;
		this.displayNameGridColumn.OptionsColumn.AllowMove = false;
		this.displayNameGridColumn.OptionsColumn.ReadOnly = true;
		this.displayNameGridColumn.Visible = true;
		this.displayNameGridColumn.VisibleIndex = 1;
		this.displayNameGridColumn.Width = 166;
		this.displayNameRepositoryItemTextEdit.AllowFocused = false;
		this.displayNameRepositoryItemTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.displayNameRepositoryItemTextEdit.AllowMouseWheel = false;
		this.displayNameRepositoryItemTextEdit.AutoHeight = false;
		this.displayNameRepositoryItemTextEdit.Name = "displayNameRepositoryItemTextEdit";
		this.displayNameRepositoryItemTextEdit.ReadOnly = true;
		this.isSuggested.Caption = " ";
		this.isSuggested.FieldName = "IsSuggested";
		this.isSuggested.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.Value;
		this.isSuggested.Name = "isSuggested";
		this.isSuggested.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.isSuggested.Visible = true;
		this.isSuggested.VisibleIndex = 2;
		this.toolTipControllerUserControl.AllowHtmlText = true;
		this.toolTipControllerUserControl.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.erdObjectsTypeObjectsCheckEdit.EditValue = true;
		this.erdObjectsTypeObjectsCheckEdit.Location = new System.Drawing.Point(112, 50);
		this.erdObjectsTypeObjectsCheckEdit.Name = "erdObjectsTypeObjectsCheckEdit";
		this.erdObjectsTypeObjectsCheckEdit.Properties.Caption = "Structures";
		this.erdObjectsTypeObjectsCheckEdit.Size = new System.Drawing.Size(73, 20);
		this.erdObjectsTypeObjectsCheckEdit.StyleController = this.availableObjectsLayoutControl;
		this.erdObjectsTypeObjectsCheckEdit.TabIndex = 16;
		this.erdObjectsTypeObjectsCheckEdit.CheckedChanged += new System.EventHandler(erdObjectsTypeObjectsCheckEdit_CheckedChanged);
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.erdAvailableObjectsLayoutControlItem, this.layoutControlItem2, this.layoutControlItem4, this.layoutControlItem5, this.layoutControlItem11, this.layoutControlItem3, this.erdObjectsTypeObjectsCheckEditLayoutControlItem, this.layoutControlItem7, this.layoutControlItem6 });
		this.layoutControlGroup2.Name = "Root";
		this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup2.Size = new System.Drawing.Size(298, 552);
		this.layoutControlGroup2.TextVisible = false;
		this.erdAvailableObjectsLayoutControlItem.Control = this.erdSuggestedObjectsGrid;
		this.erdAvailableObjectsLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.erdAvailableObjectsLayoutControlItem.Name = "erdAvailableObjectsLayoutControlItem";
		this.erdAvailableObjectsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 0, 0);
		this.erdAvailableObjectsLayoutControlItem.Size = new System.Drawing.Size(298, 456);
		this.erdAvailableObjectsLayoutControlItem.Text = "layoutControlItem";
		this.erdAvailableObjectsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.erdAvailableObjectsLayoutControlItem.TextVisible = false;
		this.layoutControlItem2.Control = this.erdObjectsFilterTextEdit;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 72);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(298, 24);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem4.Control = this.erdObjectsTypeTablesCheckEdit;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(57, 23);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(57, 23);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(57, 24);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.erdObjectsTypeViewsCheckEdit;
		this.layoutControlItem5.Location = new System.Drawing.Point(57, 48);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(53, 23);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(53, 23);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(53, 24);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem11.Control = this.helpIconUserControl;
		this.layoutControlItem11.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem11.MaxSize = new System.Drawing.Size(0, 24);
		this.layoutControlItem11.MinSize = new System.Drawing.Size(106, 24);
		this.layoutControlItem11.Name = "layoutControlItem11";
		this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem11.Size = new System.Drawing.Size(298, 24);
		this.layoutControlItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem11.Text = "Display list of:";
		this.layoutControlItem11.TextSize = new System.Drawing.Size(64, 13);
		this.layoutControlItem3.Control = this.autoAddToModuleCheckEdit;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(298, 24);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.Control = this.erdObjectsTypeObjectsCheckEdit;
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.Location = new System.Drawing.Point(110, 48);
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.MaxSize = new System.Drawing.Size(77, 24);
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.MinSize = new System.Drawing.Size(77, 24);
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.Name = "erdObjectsTypeObjectsCheckEditLayoutControlItem";
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.Size = new System.Drawing.Size(77, 24);
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.erdObjectsTypeObjectsCheckEditLayoutControlItem.TextVisible = false;
		this.layoutControlItem7.Control = this.allDocsCheckEdit;
		this.layoutControlItem7.Location = new System.Drawing.Point(187, 48);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(62, 23);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(62, 23);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(62, 24);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem6.Control = this.clearErdFilterLabelControl;
		this.layoutControlItem6.Location = new System.Drawing.Point(249, 48);
		this.layoutControlItem6.MinSize = new System.Drawing.Size(39, 17);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(49, 24);
		this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.schemaImportsAndChangesXtraTabPage.Controls.Add(this.schemaImportsAndChangesUserControl);
		this.schemaImportsAndChangesXtraTabPage.Name = "schemaImportsAndChangesXtraTabPage";
		this.schemaImportsAndChangesXtraTabPage.Size = new System.Drawing.Size(1180, 552);
		this.schemaImportsAndChangesXtraTabPage.Text = "Schema Changes";
		this.schemaImportsAndChangesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.schemaImportsAndChangesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.schemaImportsAndChangesUserControl.ErrorOccurred = false;
		this.schemaImportsAndChangesUserControl.ExpandAllRequested = false;
		this.schemaImportsAndChangesUserControl.IsChanged = false;
		this.schemaImportsAndChangesUserControl.Location = new System.Drawing.Point(0, 0);
		this.schemaImportsAndChangesUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.schemaImportsAndChangesUserControl.Name = "schemaImportsAndChangesUserControl";
		this.schemaImportsAndChangesUserControl.ShowAllImports = true;
		this.schemaImportsAndChangesUserControl.Size = new System.Drawing.Size(1180, 552);
		this.schemaImportsAndChangesUserControl.TabIndex = 1;
		this.columnHeader1.Width = 0;
		this.nodeImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("nodeImageList.ImageStream");
		this.nodeImageList.TransparentColor = System.Drawing.Color.Transparent;
		this.nodeImageList.Images.SetKeyName(0, "table_16.png");
		this.nodeImageList.Images.SetKeyName(1, "view_16.png");
		this.nodeImageList.Images.SetKeyName(2, "table_deleted_16.png");
		this.nodeImageList.Images.SetKeyName(3, "view_deleted_16.png");
		this.moduleTitleErrorProvider.ContainerControl = this;
		this.moduleImageOpenFileDialog.FileName = "openFileDialog1";
		this.moduleStatusUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.moduleStatusUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.moduleStatusUserControl.Description = "This Subject Area has been removed from the repository.";
		this.moduleStatusUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.moduleStatusUserControl.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
		this.moduleStatusUserControl.Image = Dataedo.App.Properties.Resources.warning_16;
		this.moduleStatusUserControl.Location = new System.Drawing.Point(0, 20);
		this.moduleStatusUserControl.Name = "moduleStatusUserControl";
		this.moduleStatusUserControl.Size = new System.Drawing.Size(1182, 49);
		this.moduleStatusUserControl.TabIndex = 10;
		this.moduleTextEdit.Dock = System.Windows.Forms.DockStyle.Top;
		this.moduleTextEdit.EditValue = "";
		this.moduleTextEdit.Location = new System.Drawing.Point(0, 0);
		this.moduleTextEdit.Name = "moduleTextEdit";
		this.moduleTextEdit.Properties.AllowFocused = false;
		this.moduleTextEdit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.moduleTextEdit.Properties.Appearance.Options.UseFont = true;
		this.moduleTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.moduleTextEdit.Properties.ContextImageOptions.Image = Dataedo.App.Properties.Resources.module_16;
		this.moduleTextEdit.Properties.ReadOnly = true;
		this.moduleTextEdit.Size = new System.Drawing.Size(1182, 20);
		this.moduleTextEdit.TabIndex = 11;
		this.moduleTextEdit.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.moduleXtraTabControl);
		base.Controls.Add(this.moduleStatusUserControl);
		base.Controls.Add(this.moduleTextEdit);
		this.Font = new System.Drawing.Font("Arial", 10f);
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "ModuleUserControl";
		base.Size = new System.Drawing.Size(1182, 646);
		base.Load += new System.EventHandler(ModuleUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.iconModuleProcedureRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeModuleProcedureRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleXtraTabControl).EndInit();
		this.moduleXtraTabControl.ResumeLayout(false);
		this.moduleDescriptionXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControl).EndInit();
		this.databaseLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.moduleTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		this.moduleERDXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).EndInit();
		this.splitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.erdPanelControl).EndInit();
		this.erdPanelControl.ResumeLayout(false);
		this.xtraScrollableControlERD.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.availableObjectsLayoutControl).EndInit();
		this.availableObjectsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.autoAddToModuleCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allDocsCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeViewsCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeTablesCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsFilterTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdSuggestedObjectsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdAvailableObjectsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.displayNameRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeObjectsCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdAvailableObjectsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.erdObjectsTypeObjectsCheckEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		this.schemaImportsAndChangesXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.moduleTitleErrorProvider).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleTextEdit.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
