using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.XmlExportTools.Model;
using Dataedo.App.Tools.XmlExportTools.Tools;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.ERD;

public class DiagramManager
{
	private int databaseId;

	private DiagramControl control;

	public Dataedo.App.Tools.ERD.Diagram.Diagram diagram;

	private readonly int defaultNodesSpace = 100;

	private readonly int MAX_ERD_NODES = 500;

	private readonly int ADDING_NODES_QUESTION_THRESHOLD = 10;

	private int moduleId;

	private PathCreator Path = new PathCreator();

	private ElementsContainer elements => diagram.Elements;

	public int RemainingNumberOfObjectsToAdd => MAX_ERD_NODES - elements.NodesCount;

	internal Point LastMinimalDiagramPoint { get; private set; }

	public IEnumerable<int> NodesTableIds => elements.Nodes.Select((Node x) => x.TableId);

	internal List<Node> IgnoredNodes { get; private set; }

	internal List<Node> DeletedNodes { get; private set; }

	internal List<Link> DeletedLinks { get; private set; }

	internal List<PostIt> PostIts { get; private set; }

	internal List<PostIt> DeletedPostIts { get; private set; }

	public bool AllDocs { get; set; }

	public bool? DatabaseShowSchema { get; set; }

	public bool? DatabaseShowSchemaOverride { get; set; }

	public SharedDatabaseClassEnum.DatabaseClass? DatabaseClass { get; private set; }

	public bool ShowSchema
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

	internal void SetModuleId(int value)
	{
		moduleId = value;
	}

	public Node GetNodeFromList(int id)
	{
		return IgnoredNodes.FirstOrDefault((Node x) => x.Key == id);
	}

	public DiagramManager(int databaseId, int moduleId, bool? erdShowTypes, LinkStyleEnum.LinkStyle linkStyle, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, DiagramControl control, bool notDeletedOnly, bool? erdShowNullable)
	{
		DB.Database.GetDataById(databaseId);
		this.control = control;
		diagram = this.control.Diagram;
		Reload(databaseId, moduleId, erdShowTypes, linkStyle, displayDocumentationNameMode, null, null, notDeletedOnly, setLinks: false, formatted: false, forHtml: false, null, erdShowNullable);
		this.databaseId = databaseId;
	}

	public DiagramManager(int databaseId, int moduleId, bool? erdShowTypes, LinkStyleEnum.LinkStyle linkStyle, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, Dataedo.App.Tools.ERD.Diagram.Diagram diagram, List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, bool notDeletedOnly, bool setLinks, bool formatted, bool forHtml, OtherFieldsSupport otherFieldsSupport, bool? erdShowNullable)
	{
		this.databaseId = databaseId;
		control = null;
		this.diagram = diagram;
		Reload(databaseId, moduleId, erdShowTypes, linkStyle, displayDocumentationNameMode, allDocumentationsData, databaseExportData, notDeletedOnly, setLinks, formatted, forHtml, otherFieldsSupport, erdShowNullable);
	}

	private void SetLinkForWithProperDatabase(bool setLinks, List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, Node node)
	{
		ModuleExportData documentationContainingObject = null;
		if (setLinks && (databaseExportData == null || ObjectExporter.IsObjectForExport(allDocumentationsData, databaseExportData, node.ObjectType, node.TableId, out documentationContainingObject)))
		{
			SetLinkFor(documentationContainingObject, node);
		}
	}

	private void SetLinkFor(ModuleExportData databaseExportData, Node node)
	{
		if (databaseExportData == null)
		{
			char c = SharedObjectTypeEnum.TypeToString(node.ObjectType).ToLower().First();
			node.Href = $"{c}{node.TableId}";
			return;
		}
		SharedObjectTypeEnum.ObjectType value = ((node.Type == NodeTypeEnum.NodeType.Table) ? SharedObjectTypeEnum.ObjectType.Table : SharedObjectTypeEnum.ObjectType.View);
		if (node.NodeSource != ERDNodeSource.Module)
		{
			node.Href = "#" + Path.CreateLink(databaseExportData, value, node.IdStringForPath, isForLink: true);
			return;
		}
		int moduleId = node.SubjectAreaId;
		ModuleExportData moduleExportData = databaseExportData.ModulesExportDataForExport.FirstOrDefault((ModuleExportData x) => x.Module.Id == moduleId);
		node.Href = "#" + Path.CreateLink(databaseExportData, value, node.IdStringForPath, moduleId, moduleExportData.IdStringForPath, isForLink: true);
	}

	internal string ConvertToString(object toConvert)
	{
		if (!Convert.IsDBNull(toConvert))
		{
			return Convert.ToString(toConvert);
		}
		return null;
	}

	private int? ConvertToInt(object toConvert)
	{
		if (!Convert.IsDBNull(toConvert))
		{
			return Convert.ToInt32(toConvert);
		}
		return null;
	}

	private void LoadData(int databaseId, int moduleId, bool? erdShowTypes, LinkStyleEnum.LinkStyle linkStyle, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, bool notDeletedOnly, bool setLinks, bool formatted, bool forHtml, OtherFieldsSupport otherFieldsSupport, bool? erdShowNullable)
	{
		this.databaseId = databaseId;
		DocumentationObject dataById = DB.Database.GetDataById(databaseId);
		if (dataById != null)
		{
			DatabaseShowSchema = dataById.ShowSchema;
			DatabaseShowSchemaOverride = dataById.ShowSchemaOverride;
			DatabaseClass = SharedDatabaseClassEnum.StringToType(dataById.DatabaseClass);
		}
		if (control != null)
		{
			control.ContextShowSchema = DatabaseRow.GetShowSchema(DatabaseShowSchema, DatabaseShowSchemaOverride);
			control.StartPoint = new Point(-(25 - DiagramControl.GetDiagramGridReminder(110)), -(25 - DiagramControl.GetDiagramGridReminder(19)));
			control.GridStartPoint = control.StartPoint;
			control.ShowColumnTypes = erdShowTypes == true;
			control.ShowColumnNullable = erdShowNullable == true;
			control.LinkStyle = linkStyle;
			control.DisplayDocumentationNameMode = displayDocumentationNameMode;
		}
		DeletedNodes = new List<Node>();
		DeletedLinks = new List<Link>();
		DeletedPostIts = new List<PostIt>();
		this.moduleId = moduleId;
		IEnumerable<IGrouping<int, ErdNodeObject>> enumerable = from x in DB.ErdNode.GetDataByModuleId(moduleId)
			group x by x.NodeId;
		elements.ClearElements();
		Point point = new Point(int.MaxValue, int.MaxValue);
		foreach (IGrouping<int, ErdNodeObject> item in enumerable)
		{
			ErdNodeObject erdNodeObject = item.First();
			if (notDeletedOnly && !(ConvertToString(erdNodeObject.Status) != "D"))
			{
				continue;
			}
			NodeTypeEnum.NodeType type = NodeTypeEnum.NodeType.Structure;
			if (ConvertToString(erdNodeObject.ObjectType).Equals("TABLE"))
			{
				type = NodeTypeEnum.NodeType.Table;
			}
			else if (ConvertToString(erdNodeObject.ObjectType).Equals("VIEW"))
			{
				type = NodeTypeEnum.NodeType.View;
			}
			Node node = diagram.CreateNode(databaseId, dataById.ShowSchemaOverride == true || (!dataById.ShowSchemaOverride.HasValue && dataById.ShowSchema == true), erdNodeObject.DatabaseId, PrepareValue.ToBool(erdNodeObject.DatabaseMultipleSchemas), erdNodeObject.DatabaseShowSchema, erdNodeObject.DatabaseShowSchemaOverride, ConvertToInt(erdNodeObject.TableId).Value, ConvertToString(erdNodeObject.Name), new Point(Convert.ToInt32(erdNodeObject.PosX), Convert.ToInt32(erdNodeObject.PosY)), type, ConvertToString(erdNodeObject.Status) == "D");
			point.X = Math.Min(point.X, node.Left);
			point.Y = Math.Min(point.Y, node.Top);
			if (node == null)
			{
				continue;
			}
			node.Color = NodeColors.GetColorFromHtml(ConvertToString(erdNodeObject.Color));
			node.Id = ConvertToInt(erdNodeObject.NodeId);
			node.Schema = ConvertToString(erdNodeObject.Schema);
			node.SubjectAreaId = Convert.ToInt32(erdNodeObject.ModuleId);
			node.TableId = Convert.ToInt32(erdNodeObject.TableId);
			node.DatabaseType = DatabaseTypeEnum.StringToType(ConvertToString(erdNodeObject.DatabaseType));
			node.DatabaseName = ConvertToString(erdNodeObject.DatabaseName);
			node.TableSource = UserTypeEnum.ObjectToType(ConvertToString(erdNodeObject.Source)).Value;
			node.TableSubtype = SharedObjectSubtypeEnum.StringToType(node.ObjectType, ConvertToString(erdNodeObject.Subtype));
			node.ShowColumnsDataTypes = erdShowTypes == true;
			node.ShowNullable = erdShowNullable == true;
			node.DisplayDocumentationNameMode = displayDocumentationNameMode;
			node.IsInSubjectArea = erdNodeObject.IsInModule;
			if (otherFieldsSupport == null || otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Title))
			{
				node.Title = ConvertToString(erdNodeObject.Title);
			}
			int? num = PrepareValue.ToInt(erdNodeObject.Width);
			if (num.HasValue)
			{
				node.Width = num.Value;
			}
			bool flag = databaseExportData != null && ObjectExporter.IsInModule(databaseExportData.ModulesExportDataForExport.FirstOrDefault((ModuleExportData x) => x.Module.Id == moduleId), node.TableId, ObjectTypeEnum.NodeTypeToObjectType(node.Type));
			node.NodeSource = (flag ? ERDNodeSource.Module : ERDNodeSource.Undefined);
			node.SetColumns(selectPrimaryKeyColumns: false, selectUniqueKeyColumns: false, selectFkColumns: false, otherFieldsSupport);
			SetLinkForWithProperDatabase(setLinks, allDocumentationsData, databaseExportData, node);
			foreach (ErdNodeObject item2 in item)
			{
				int? num2 = PrepareValue.ToInt(item2.FkTableId);
				int? num3 = PrepareValue.ToInt(item2.PkTableId);
				string text = PrepareValue.ToString(item2.RelationStatus);
				if (num3 == node.TableId && text != null)
				{
					node.RelationsFkTables.Add(new NodeRelationContainer(num2, text));
				}
				else if (num2 == node.TableId && text != null)
				{
					node.RelationsPkTables.Add(new NodeRelationContainer(num3, text));
				}
			}
			elements.AddNode(node);
		}
		elements.ClearLinks();
		List<RelationLinkObject> dataBySavedNodesInModule = DB.Relation.GetDataBySavedNodesInModule(moduleId);
		List<RelationColumnRow> columnsWithUniqueConstraintsByModuleId = DB.RelationColumns.GetColumnsWithUniqueConstraintsByModuleId(moduleId);
		foreach (RelationLinkObject relation in dataBySavedNodesInModule)
		{
			Link link = diagram.CreateLink(relation.PkTableId, relation.FkTableId, relation.TableRelationId, relation.Source.Equals("USER"), relation.PkType.ToString(), relation.FkType.ToString());
			if (link != null)
			{
				link.Columns = columnsWithUniqueConstraintsByModuleId.Where((RelationColumnRow x) => x.TableRelationId == relation.TableRelationId).ToList();
				link.GetDataFromRelationRow(relation, moduleId, formatted, forHtml, otherFieldsSupport);
				elements.AddLink(link);
			}
		}
		foreach (ErdPostIt item3 in DB.ErdPostIt.GetPostItsByModuleId(moduleId))
		{
			PostIt postIt = diagram.CreatePostIt(ConvertToInt(item3.Id).Value, new Point(Convert.ToInt32(item3.PositionX), Convert.ToInt32(item3.PositionY)), PostItLayerEnum.IntToType(Convert.ToInt32(item3.PositionZ)), item3.Text);
			point.X = Math.Min(point.X, postIt.Left);
			point.Y = Math.Min(point.Y, postIt.Top);
			if (postIt != null)
			{
				postIt.Color = NodeColors.GetColorFromHtml(ConvertToString(item3.Color));
				postIt.Id = ConvertToInt(item3.Id);
				postIt.SubjectAreaId = Convert.ToInt32(item3.ModuleId);
				int? num4 = PrepareValue.ToInt(item3.Width);
				if (num4.HasValue)
				{
					postIt.Width = num4.Value;
				}
				int? num5 = PrepareValue.ToInt(item3.Height);
				if (num5.HasValue)
				{
					postIt.Height = num5.Value;
				}
			}
			elements.AddPostIt(postIt);
		}
	}

	public void UpdateNodeById(int tableId, string schema, string name, string title, SharedObjectSubtypeEnum.ObjectSubtype subtype, UserTypeEnum.UserType source, List<ColumnRow> columnsToRemove)
	{
		Node node = elements.Nodes.FirstOrDefault((Node x) => x.TableId == tableId);
		if (node == null)
		{
			return;
		}
		node.Schema = schema;
		node.Label = name;
		node.Title = title;
		node.TableSubtype = subtype;
		node.TableSource = source;
		List<ColumnRow> list = columnsToRemove;
		if (list == null || !list.Any())
		{
			return;
		}
		List<NodeColumnDB> list2 = node.Columns?.Where((NodeColumnDB x) => columnsToRemove.Any((ColumnRow c) => c.Id == x.ColumnId))?.ToList();
		if (list2 == null)
		{
			return;
		}
		foreach (NodeColumnDB item in list2)
		{
			node.Columns.Remove(item);
		}
	}

	public Point GetMinimalDiagramPoint(bool refreshMinimalDiagramPoint)
	{
		if (refreshMinimalDiagramPoint)
		{
			if (elements.HasAnyNodes)
			{
				Point lastMinimalDiagramPoint = new Point(int.MaxValue, int.MaxValue);
				foreach (RectangularObject rectangularObject in elements.RectangularObjects)
				{
					lastMinimalDiagramPoint.X = Math.Min(lastMinimalDiagramPoint.X, rectangularObject.Left);
					lastMinimalDiagramPoint.Y = Math.Min(lastMinimalDiagramPoint.Y, rectangularObject.Top);
				}
				LastMinimalDiagramPoint = lastMinimalDiagramPoint;
				return LastMinimalDiagramPoint;
			}
			return default(Point);
		}
		return LastMinimalDiagramPoint;
	}

	public Point NormalizeNodesPositions(bool refreshMinimalDiagramPoint)
	{
		return NormalizeNodesPositions(GetMinimalDiagramPoint(refreshMinimalDiagramPoint));
	}

	public Point NormalizeNodesPositions(Point minimalDiagramPoint, bool normalizeNegativeOnly = false)
	{
		int num = ((minimalDiagramPoint.X < 0) ? DiagramControl.GetEntireGridCellsSize(minimalDiagramPoint.X, toAbsoluteFloor: false) : DiagramControl.GetEntireGridCellsSize(minimalDiagramPoint.X, toAbsoluteFloor: true));
		int num2 = ((minimalDiagramPoint.Y < 0) ? DiagramControl.GetEntireGridCellsSize(minimalDiagramPoint.Y, toAbsoluteFloor: false) : DiagramControl.GetEntireGridCellsSize(minimalDiagramPoint.Y, toAbsoluteFloor: true));
		if (!normalizeNegativeOnly || minimalDiagramPoint.X < 0 || minimalDiagramPoint.Y < 0)
		{
			foreach (RectangularObject rectangularObject in elements.RectangularObjects)
			{
				Point position = default(Point);
				position.X = ((!normalizeNegativeOnly || num < 0) ? (rectangularObject.Position.X - num) : rectangularObject.Position.X);
				position.Y = ((!normalizeNegativeOnly || num2 < 0) ? (rectangularObject.Position.Y - num2) : rectangularObject.Position.Y);
				rectangularObject.Position = position;
			}
			foreach (Link link in elements.Links)
			{
				link.FromNodeArrow.RecalculatePosition(link.FromNode, link.ToNode);
				link.ToNodeArrow.RecalculatePosition(link.FromNode, link.ToNode);
			}
		}
		return new Point(num, num2);
	}

	internal void ReloadTables(List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, int databaseId, int moduleId, bool setLinks, bool allDocs)
	{
		IgnoredNodes = new List<Node>();
		ReloadIgnoredTables(allDocumentationsData, databaseExportData, databaseId, moduleId, setLinks);
		ReloadOtherModuleTables(allDocumentationsData, databaseExportData, databaseId, moduleId, setLinks);
		ReloadOutsideModuleTables(allDocumentationsData, databaseExportData, databaseId, setLinks, allDocs);
	}

	private void ReloadIgnoredTables(List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, int databaseId, int moduleId, bool setLinks)
	{
		foreach (ErdTableWithModuleObject item in DB.Table.GetTablesAndViewsIgnoredOnERD(moduleId, NodesTableIds.ToList()))
		{
			CreateNodeByTable(databaseId, item, ERDNodeSource.Module, setLinks, allDocumentationsData, databaseExportData);
		}
	}

	private void ReloadOtherModuleTables(List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, int databaseId, int moduleId, bool setLinks)
	{
		foreach (ErdTableObject item in DB.Table.GetTablesWithIntermoduleRelation(moduleId, NodesTableIds.ToList()))
		{
			CreateNodeByTable(databaseId, item, ERDNodeSource.ConnectedWithModule, setLinks, allDocumentationsData, databaseExportData);
		}
	}

	private void ReloadOutsideModuleTables(List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, int databaseId, bool setLinks, bool allDocs)
	{
		List<int> tableIds = (from x in elements.Nodes.Union(IgnoredNodes)
			where x.NodeSource != ERDNodeSource.OutsideTheModule
			select x.TableId).ToList();
		List<ErdTableObject> list = ((!allDocs) ? DB.Table.GetTablesAndViewsOutsideModuleOnErd(databaseId, tableIds) : DB.Table.GetAllDocsTablesAndViewsOutsideModuleOnErd(tableIds));
		foreach (ErdTableObject item in list)
		{
			CreateNodeByTable(databaseId, item, ERDNodeSource.OutsideTheModule, setLinks, allDocumentationsData, databaseExportData);
		}
	}

	private void CreateNodeByTable(int databaseId, ErdTableObject table, ERDNodeSource source, bool setLinks, List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData)
	{
		NodeTypeEnum.NodeType type = NodeTypeEnum.NodeType.Structure;
		if (table.ObjectType.Equals("TABLE"))
		{
			type = NodeTypeEnum.NodeType.Table;
		}
		else if (table.ObjectType.Equals("VIEW"))
		{
			type = NodeTypeEnum.NodeType.View;
		}
		Node node = new Node(databaseId, ShowSchema, table.DatabaseId, table.DatabaseMultipleSchemas, table.DatabaseShowSchema, table.DatabaseShowSchemaOverride, table.TableId, table.Name, Point.Empty, type, table.Status == "D", visible: false);
		if (node != null)
		{
			node.Schema = table.Schema;
			node.SubjectAreaId = moduleId;
			node.TableId = table.TableId;
			node.DatabaseType = DatabaseTypeEnum.StringToType(table.DatabaseType);
			node.DatabaseName = ConvertToString(table.DatabaseName);
			node.Title = table.Title;
			node.NodeSource = source;
			node.TableSource = UserTypeEnum.ObjectToType(table.Source) ?? UserTypeEnum.UserType.DBMS;
			node.TableSubtype = SharedObjectSubtypeEnum.StringToType(node.ObjectType, table.Subtype);
			SetLinkForWithProperDatabase(setLinks, allDocumentationsData, databaseExportData, node);
			IgnoredNodes.Add(node);
		}
	}

	internal Box AddIgnoredNode(int node, LinkStyleEnum.LinkStyle style, bool showColumnTypes, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, bool calculatePositon, bool showColumnNullable)
	{
		return AddIgnoredNodes(new List<int> { node }, GetStartPoint(), setDiagramControlValidPosition: false, style, showColumnTypes, displayDocumentationNameMode, calculatePositon, showColumnNullable);
	}

	internal Box AddOtherModuleNode(int node, LinkStyleEnum.LinkStyle style, bool showColumnTypes, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, bool calculatePositon, bool showColumnNullable)
	{
		return AddOtherModuleNodes(new List<int> { node }, GetStartPoint(), setDiagramControlValidPosition: false, style, showColumnTypes, displayDocumentationNameMode, calculatePositon, showColumnNullable);
	}

	internal Box AddOutsideModuleNode(int node, LinkStyleEnum.LinkStyle style, bool showColumnTypes, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, bool calculatePositon, bool showColumnNullable)
	{
		return AddOutsideModuleNodes(new List<int> { node }, GetStartPoint(), setDiagramControlValidPosition: false, style, showColumnTypes, displayDocumentationNameMode, calculatePositon, showColumnNullable);
	}

	public int RemainingERDObjectsToAdd(int newObjectsCount, Form owner = null)
	{
		int remainingERDObjectsToAdd = GetRemainingERDObjectsToAdd(newObjectsCount);
		if (remainingERDObjectsToAdd < 1)
		{
			GeneralMessageBoxesHandling.Show($"You can add up to {MAX_ERD_NODES} objects to ERD. Limit exceeded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
			return 0;
		}
		if (remainingERDObjectsToAdd == newObjectsCount)
		{
			if (newObjectsCount <= ADDING_NODES_QUESTION_THRESHOLD)
			{
				return remainingERDObjectsToAdd;
			}
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show($"Are you sure you want to add <b>{remainingERDObjectsToAdd} new objects</b>?", "Add objects", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return 0;
			}
			return remainingERDObjectsToAdd;
		}
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult2 = GeneralMessageBoxesHandling.Show($"You can add up to {MAX_ERD_NODES} objects to ERD. Do you want to add only {remainingERDObjectsToAdd} first of your list?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, owner);
		if (handlingDialogResult2 == null || handlingDialogResult2.DialogResult != DialogResult.Yes)
		{
			return 0;
		}
		return remainingERDObjectsToAdd;
	}

	private int GetRemainingERDObjectsToAdd(int newObjectsCount)
	{
		int nodesCount = elements.NodesCount;
		if (nodesCount + newObjectsCount <= MAX_ERD_NODES)
		{
			return newObjectsCount;
		}
		return MAX_ERD_NODES - nodesCount;
	}

	public Point GetNewNodePoint(Point startPoint, bool calculatePositon)
	{
		if (!calculatePositon)
		{
			return startPoint;
		}
		int i = startPoint.Y;
		int num = Math.Max(control.Parent.Width, control.Diagram.LastDiagramBox.Width) - 25;
		int x = startPoint.X;
		for (; i + 19 < DiagramControl.MaxControlSize; i += 50)
		{
			int j = x;
			if (j == x && j + 110 >= num && IsNodePositionEmpty(j, i))
			{
				return new Point(j, i);
			}
			for (; j + 110 < num; j += 50)
			{
				if (IsNodePositionEmpty(j, i))
				{
					return new Point(j, i);
				}
			}
		}
		return new Point(0, 0);
	}

	public Point GetStartPoint()
	{
		int num = control.StartPoint.X - DiagramControl.GetDiagramGridReminder(control.GridStartPoint.X);
		int num2 = control.StartPoint.Y - DiagramControl.GetDiagramGridReminder(control.GridStartPoint.Y);
		int num3 = 110;
		int num4 = 19;
		int num5 = DiagramControl.GetEntireGridCellsSize(num3, toAbsoluteFloor: false) - num;
		int num6 = DiagramControl.GetEntireGridCellsSize(num4, toAbsoluteFloor: false) - num2;
		if (control.StartPoint.X + num5 - num3 < 25)
		{
			num5 += 50;
		}
		if (control.StartPoint.Y + num6 - num4 < 25)
		{
			num6 += 50;
		}
		return new Point(num5, num6);
	}

	private bool IsNodePositionEmpty(int x, int y)
	{
		Rectangle r = new Rectangle(x - 110 - defaultNodesSpace, y - 19 - defaultNodesSpace, 220 + 2 * defaultNodesSpace, 38 + 2 * defaultNodesSpace);
		foreach (RectangularObject rectangularObject in elements.RectangularObjects)
		{
			if (rectangularObject.Intersects(r))
			{
				return false;
			}
		}
		return true;
	}

	internal Box AddIgnoredNodes(List<int> tablesIDsToAdd, Point p, bool setDiagramControlValidPosition, LinkStyleEnum.LinkStyle? linkStyle, bool showColumnTypes, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, bool calculatePositon, bool showColumnNullable)
	{
		if (tablesIDsToAdd.Count == 0)
		{
			return null;
		}
		Box result = AddNodes(tablesIDsToAdd, IgnoredNodes, p, setDiagramControlValidPosition, showColumnTypes, displayDocumentationNameMode, calculatePositon, showColumnNullable);
		List<RelationLinkObject> dataByModule = DB.Relation.GetDataByModule(moduleId);
		AddRelations(tablesIDsToAdd, dataByModule, linkStyle);
		return result;
	}

	internal Box AddOtherModuleNodes(List<int> tablesIDsToAdd, Point p, bool setDiagramControlValidPosition, LinkStyleEnum.LinkStyle? linkStyle, bool showColumnTypes, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, bool calculatePositon, bool showColumnNullable)
	{
		if (tablesIDsToAdd.Count == 0)
		{
			return null;
		}
		Box result = AddNodes(tablesIDsToAdd, IgnoredNodes.Where((Node x) => x.NodeSource == ERDNodeSource.ConnectedWithModule).ToList(), p, setDiagramControlValidPosition, showColumnTypes, displayDocumentationNameMode, calculatePositon, showColumnNullable);
		List<RelationLinkObject> dataByRelationsWithOtherModule = DB.Relation.GetDataByRelationsWithOtherModule(moduleId);
		AddRelations(tablesIDsToAdd, dataByRelationsWithOtherModule, linkStyle);
		return result;
	}

	internal Box AddOutsideModuleNodes(List<int> tablesIDsToAdd, Point p, bool setDiagramControlValidPosition, LinkStyleEnum.LinkStyle? linkStyle, bool showColumnTypes, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, bool calculatePositon, bool showColumnNullable)
	{
		if (tablesIDsToAdd.Count == 0)
		{
			return null;
		}
		Box result = AddNodes(tablesIDsToAdd, IgnoredNodes.Where((Node x) => x.NodeSource == ERDNodeSource.OutsideTheModule).ToList(), p, setDiagramControlValidPosition, showColumnTypes, displayDocumentationNameMode, calculatePositon, showColumnNullable);
		List<RelationLinkObject> dataByRelationsWithOtherModule = DB.Relation.GetDataByRelationsWithOtherModule(moduleId);
		AddRelations(tablesIDsToAdd, dataByRelationsWithOtherModule, linkStyle);
		return result;
	}

	private Box AddNodes(List<int> tablesIDsToAdd, List<Node> targetList, Point p, bool setDiagramControlValidPosition, bool showColumnTypes, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, bool calculatePositon, bool showColumnNullable)
	{
		Box box = new Box();
		tablesIDsToAdd.ForEach(delegate(int x)
		{
			Node node = targetList.SingleOrDefault((Node y) => y.TableId == x);
			if (node != null)
			{
				DeletedNodes.Remove(node);
				targetList.Remove(node);
				Node node2 = diagram.CreateNode(node.CurrentDiagramDatabaseId, ShowSchema, node.DatabaseId, node.IsMultipleSchemasDatabase, node.DatabaseShowSchema, node.DatabaseShowSchemaOverride, node.Key, node.Label, node.Position, node.Type, node.Deleted, byUser: true);
				if (node2 != null)
				{
					node2.Color = node.Color;
					node2.Id = node.Id;
					node2.Schema = node.Schema;
					node2.SubjectAreaId = node.SubjectAreaId;
					node2.TableId = node.TableId;
					node2.DatabaseId = node.DatabaseId;
					node2.DatabaseType = node.DatabaseType;
					node2.DatabaseName = node.DatabaseName;
					node2.Href = node.Href;
					node2.Title = node.Title;
					node2.Width = node.Width;
					node2.IsTableAddedToDatabase = node.IsTableAddedToDatabase;
					node2.SetColumns(selectPrimaryKeyColumns: true, selectUniqueKeyColumns: true, selectFkColumns: true);
					node2.ShowColumnsDataTypes = showColumnTypes;
					node2.ShowNullable = showColumnNullable;
					node2.Position = node.Position;
					node2.TableSource = node.TableSource;
					node2.TableSubtype = node.TableSubtype;
					if (control != null)
					{
						DiagramManager diagramManager = this;
						Point startPoint = p;
						int calculatePositon2;
						if (!calculatePositon)
						{
							List<int> list = tablesIDsToAdd;
							calculatePositon2 = ((list == null || list.Count() != 1) ? 1 : 0);
						}
						else
						{
							calculatePositon2 = 1;
						}
						Point newNodePoint = diagramManager.GetNewNodePoint(startPoint, (byte)calculatePositon2 != 0);
						node2.Position = control.ToLocal(newNodePoint.X, newNodePoint.Y);
						if (setDiagramControlValidPosition)
						{
							node2.UpdatePosition(node2.Position, diagram.LastDiagramBox);
						}
					}
					if (!elements.Nodes.Select((Node y) => y.Key).Contains(node2.Key))
					{
						elements.AddNode(node2);
					}
					box.UpdateMinMax(node2);
				}
			}
		});
		return box;
	}

	private void AddRelations(List<int> tablesIDsToAdd, List<RelationLinkObject> relations, LinkStyleEnum.LinkStyle? linkStyle)
	{
		SetRelations(tablesIDsToAdd, relations);
		IEnumerable<int> existingIDs = elements.Nodes.Select((Node x) => x.Key);
		IEnumerable<RelationLinkObject> enumerable = relations.Where((RelationLinkObject x) => (tablesIDsToAdd.Contains(x.PkTableId) && existingIDs.Contains(x.FkTableId)) || (tablesIDsToAdd.Contains(x.FkTableId) && existingIDs.Contains(x.PkTableId)));
		List<RelationColumnRow> columnsWithUniqueConstraintsByModuleId = DB.RelationColumns.GetColumnsWithUniqueConstraintsByModuleId(moduleId);
		foreach (RelationLinkObject relation in enumerable)
		{
			Link link = diagram.CreateLink(relation.PkTableId, relation.FkTableId, relation.TableRelationId, relation.Source.Equals("USER"), relation.PkType.ToString(), relation.FkType.ToString());
			if (link != null)
			{
				link.Columns = columnsWithUniqueConstraintsByModuleId.Where((RelationColumnRow x) => x.TableRelationId == relation.TableRelationId).ToList();
				link.GetDataFromRelationRow(relation, moduleId, formatted: false, forHtml: false, null);
				link.LinkStyle = linkStyle;
				elements.AddLink(link);
			}
		}
	}

	public PostIt AddPostIt(int key, Point position, string text, PostItLayerEnum.PostItLayer layer = PostItLayerEnum.PostItLayer.Front)
	{
		PostIt postIt = diagram.CreatePostIt(key, position, layer, text);
		postIt.SubjectAreaId = moduleId;
		postIt.IsInAddingMode = true;
		elements.AddPostIt(postIt);
		return postIt;
	}

	public void SetRelations(List<int> tablesIDsToAdd, List<RelationLinkObject> relations)
	{
		foreach (RelationLinkObject relation in relations)
		{
			foreach (Node item in elements.Nodes.Where((Node x) => tablesIDsToAdd.Contains(x.TableId)))
			{
				if (relation.PkTableId == item.TableId && !item.RelationsFkTables.Any((NodeRelationContainer x) => x.TableId == relation.FkTableId))
				{
					item.RelationsFkTables.Add(new NodeRelationContainer(relation.FkTableId, relation.Status));
				}
				else if (relation.FkTableId == item.TableId && !item.RelationsFkTables.Any((NodeRelationContainer x) => x.TableId == relation.PkTableId))
				{
					item.RelationsFkTables.Add(new NodeRelationContainer(relation.PkTableId, relation.Status));
				}
			}
		}
	}

	public static void SetRelatedTables(Node selectedNode, List<Node> allNodes)
	{
		if (selectedNode == null)
		{
			return;
		}
		selectedNode.RelatedNodes.Clear();
		foreach (Node node in allNodes)
		{
			List<NodeRelationContainer> relationsPkTables = selectedNode.RelationsPkTables;
			if (relationsPkTables == null || !relationsPkTables.Any((NodeRelationContainer x) => x.TableId == node.TableId && x.RelationStatus.Equals("A")))
			{
				List<NodeRelationContainer> relationsFkTables = selectedNode.RelationsFkTables;
				if (relationsFkTables == null || !relationsFkTables.Any((NodeRelationContainer x) => x.TableId == node.TableId && x.RelationStatus.Equals("A")))
				{
					continue;
				}
			}
			node.IsRelatedToSelected = true;
			selectedNode.IsRelatedToSelected = true;
			selectedNode.RelatedNodes.Add(node);
		}
	}

	internal void Reload(int databaseId, int moduleId, bool? erdShowTypes, LinkStyleEnum.LinkStyle linkStyle, DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode, List<ModuleExportData> allDocumentationsData, ModuleExportData databaseExportData, bool notDeletedOnly, bool setLinks, bool formatted, bool forHtml, OtherFieldsSupport otherFieldsSupport, bool? erdShowNullable)
	{
		if (control != null)
		{
			control.SelectAllNodes();
			control.DeleteSelectedNodes();
			control.CurrentDatabaseId = databaseId;
		}
		LoadData(databaseId, moduleId, erdShowTypes, linkStyle, displayDocumentationNameMode, allDocumentationsData, databaseExportData, notDeletedOnly, setLinks, formatted, forHtml, otherFieldsSupport, erdShowNullable);
		if (databaseExportData == null)
		{
			ReloadTables(allDocumentationsData, databaseExportData, databaseId, moduleId, setLinks, AllDocs);
		}
	}
}
