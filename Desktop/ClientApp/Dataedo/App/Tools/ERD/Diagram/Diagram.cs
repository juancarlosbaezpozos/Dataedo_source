using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Xml;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.UI;
using Dataedo.Model.Enums;

namespace Dataedo.App.Tools.ERD.Diagram;

public class Diagram
{
	public delegate void NodeContainerChangedEventHandler(object sender, RectangularObject node, bool isNew, bool byUser);

	public int NextLinkRelationId
	{
		get
		{
			if (!Elements.HasAnyLinks)
			{
				return -1;
			}
			int num = Elements.Links.Min((Link x) => x.RelationId);
			if (num > -1)
			{
				return -1;
			}
			return num - 1;
		}
	}

	public int NextPostItId
	{
		get
		{
			if (!Elements.HasAnyPostIts)
			{
				return -1;
			}
			int? num = Elements.PostIts.Min((PostIt x) => x.Id);
			if (!num.HasValue || num > -1)
			{
				return -1;
			}
			return num.Value - 1;
		}
	}

	public Rectangle LastDiagramBox { get; private set; }

	public ElementsContainer Elements { get; set; } = new ElementsContainer();


	public bool HasAnyNodes => Elements.HasAnyNodes;

	public event EventHandler Changed;

	public event NodeContainerChangedEventHandler NodeContainerChanged;

	public event EventHandler ImportantChange;

	public Node CreateNode(int currentDiagramDatabaseId, bool currentDiagramDatabaseShowSchema, int databaseId, bool? isMultipleSchemasDatabase, bool? databaseShowSchema, bool? databaseShowSchemaOverride, int key, string label, Point position, NodeTypeEnum.NodeType type, bool deleted = false, bool byUser = false)
	{
		if (Elements.NodeExists(key))
		{
			return null;
		}
		Node node = new Node(currentDiagramDatabaseId, currentDiagramDatabaseShowSchema, databaseId, isMultipleSchemasDatabase, databaseShowSchema, databaseShowSchemaOverride, key, label, position, type, deleted, visible: true);
		node.Changed += NodeChanged;
		node.ImportantChange = (EventHandler)Delegate.Combine(node.ImportantChange, new EventHandler(NodeChanged));
		node.ImportantChange = (EventHandler)Delegate.Combine(node.ImportantChange, new EventHandler(NotifyImportantChange));
		this.NodeContainerChanged?.Invoke(this, node, isNew: true, byUser);
		Elements.AddNode(node);
		NotifyChange();
		return node;
	}

	public PostIt CreatePostIt(int key, Point position, PostItLayerEnum.PostItLayer layer, string text)
	{
		PostIt postIt = new PostIt(key, position, layer, text);
		postIt.Changed += NodeChanged;
		postIt.ImportantChange = (EventHandler)Delegate.Combine(postIt.ImportantChange, new EventHandler(NodeChanged));
		postIt.ImportantChange = (EventHandler)Delegate.Combine(postIt.ImportantChange, new EventHandler(NotifyImportantChange));
		this.NodeContainerChanged?.Invoke(this, postIt, isNew: true, byUser: true);
		Elements.AddPostIt(postIt);
		NotifyChange();
		return postIt;
	}

	private void NotifyImportantChange(object sender, EventArgs e)
	{
		this.ImportantChange?.Invoke(this, EventArgs.Empty);
	}

	private void NodeChanged(object sender, EventArgs e)
	{
		NotifyChange();
	}

	public void RemoveNode(int key, bool byUser)
	{
		RectangularObject element = Elements.GetElement(key);
		this.NodeContainerChanged?.Invoke(this, element, isNew: false, byUser);
		if (element is Node node)
		{
			for (int num = node.inLinks.Count - 1; num >= 0; num--)
			{
				RemoveLink(node.inLinks[num]);
			}
			for (int num2 = node.outLinks.Count - 1; num2 >= 0; num2--)
			{
				RemoveLink(node.outLinks[num2]);
			}
		}
		Elements.RemoveElementByKey(key);
		NotifyChange();
	}

	public Link CreateLink(Node from, Node to, int relationId, bool isUserRelation = false, string pkType = "ONE", string fkType = "MANY")
	{
		if (from == null || to == null)
		{
			return null;
		}
		Link link = new Link(from, to, relationId, isUserRelation, pkType, fkType);
		link.IsModifiedInRelationForm = true;
		link.Changed += LinkChanged;
		link.ImportantChange = (EventHandler)Delegate.Combine(link.ImportantChange, new EventHandler(NotifyImportantChange));
		link.ImportantChange = (EventHandler)Delegate.Combine(link.ImportantChange, new EventHandler(LinkChanged));
		link.Name = RelationsRegexHelper.GetRelationName(to.Label, from.Label);
		if (!from.outLinks.Contains(link))
		{
			from.outLinks.Add(link);
		}
		if (!to.inLinks.Contains(link))
		{
			to.inLinks.Add(link);
		}
		NotifyChange();
		return link;
	}

	private void LinkChanged(object sender, EventArgs e)
	{
		NotifyChange();
	}

	public Link CreateLink(int fromKey, int toKey, int relationId, bool isUserRelation = false, string pkType = "ONE", string fkType = "MANY")
	{
		Node node = Elements.GetNode(fromKey);
		Node node2 = Elements.GetNode(toKey);
		if (node == null || node2 == null)
		{
			return null;
		}
		Link link = CreateLink(node, node2, relationId, isUserRelation, pkType, fkType);
		Elements.AddLink(link);
		return link;
	}

	public void RemoveLink(Link link)
	{
		link.FromNode?.outLinks.Remove(link);
		link.ToNode?.inLinks.Remove(link);
		link.Selected = false;
		Elements.RemoveLinkByKey(link.Key);
		NotifyChange();
	}

	private void NotifyChange()
	{
		this.Changed?.Invoke(this, new EventArgs());
	}

	public void Render(Graphics g, Point startPoint, CanvasObject.Output dest = CanvasObject.Output.Control, Action<Graphics> renderAddNewRelation = null)
	{
		foreach (Node node in Elements.Nodes)
		{
			node.RecalcLinksArrows();
		}
		foreach (PostIt item in Elements.PostIts.Where((PostIt x) => x.Layer == PostItLayerEnum.PostItLayer.Back))
		{
			item.Render(g, startPoint, dest);
		}
		foreach (Link link in Elements.Links)
		{
			link.Render(g, startPoint, dest);
		}
		renderAddNewRelation?.Invoke(g);
		foreach (Node node2 in Elements.Nodes)
		{
			node2.Render(g, startPoint, dest);
		}
		foreach (PostIt item2 in Elements.PostIts.Where((PostIt x) => x.Layer == PostItLayerEnum.PostItLayer.Front))
		{
			item2.Render(g, startPoint, dest);
		}
	}

	public Image ToImage(Color? background = null, bool applyResources = false)
	{
		try
		{
			if (applyResources)
			{
				SkinsManager.SetResources(SkinsManager.DefaultSkin.ResourceManager);
			}
			RecalculateLinksArrows();
			Rectangle rectangle = CalcBBox();
			int num = 5;
			rectangle.Width += 2 * num;
			rectangle.Height += 2 * num;
			rectangle.X += -num;
			rectangle.Y += -num;
			Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			Matrix transform = graphics.Transform;
			transform.Translate(-rectangle.X, -rectangle.Y);
			graphics.Transform = transform;
			if (background.HasValue)
			{
				graphics.Clear(background.Value);
			}
			Render(graphics, new Point(0, 0), CanvasObject.Output.Image);
			return bitmap;
		}
		finally
		{
			if (applyResources)
			{
				SkinsManager.SetResources();
			}
		}
	}

	public XmlDocument ToSvg(CanvasObject.Output destination)
	{
		RecalculateLinksArrows();
		foreach (Node node in Elements.Nodes)
		{
			node.RecalcLinksArrows();
		}
		XmlDocument xmlDocument = new XmlDocument();
		Rectangle rectangle = CalcBBox();
		int num = 5;
		rectangle.Width += 2 * num;
		rectangle.Height += 2 * num;
		rectangle.X += -num;
		rectangle.Y += -num;
		XmlElement xmlElement = xmlDocument.CreateElement("svg");
		xmlElement.SetAttribute("version", "1.1");
		xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
		xmlElement.SetAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink");
		xmlElement.SetAttribute("width", rectangle.Width.ToString());
		xmlElement.SetAttribute("height", rectangle.Height.ToString());
		xmlElement.SetAttribute("class", "erd");
		xmlDocument.AppendChild(xmlElement);
		xmlElement.AppendChild(xmlDocument.CreateElement("defs"));
		XmlElement xmlElement2 = xmlDocument.CreateElement("g");
		xmlElement2.SetAttribute("transform", $"translate({-rectangle.X},{-rectangle.Y})");
		xmlElement.AppendChild(xmlElement2);
		foreach (CanvasObject item in Elements.AllElementsLinksFirst)
		{
			xmlElement2.AppendChild(item.ToSvg(xmlDocument, destination));
		}
		return xmlDocument;
	}

	public Rectangle CalcBBox()
	{
		Point point = new Point(int.MaxValue, int.MaxValue);
		Point point2 = new Point(int.MinValue, int.MinValue);
		foreach (RectangularObject rectangularObject in Elements.RectangularObjects)
		{
			point.X = Math.Min(point.X, rectangularObject.Left);
			point.Y = Math.Min(point.Y, rectangularObject.Top);
			point2.X = Math.Max(point2.X, rectangularObject.Right);
			point2.Y = Math.Max(point2.Y, rectangularObject.Bottom);
		}
		foreach (Link link in Elements.Links)
		{
			point.X = Math.Min(point.X, Math.Min(link.FromNodeArrow.MiddlePoint.X - 10, link.ToNodeArrow.MiddlePoint.X - 10));
			point.Y = Math.Min(point.Y, Math.Min(link.FromNodeArrow.MiddlePoint.Y - 10, link.ToNodeArrow.MiddlePoint.Y - 10));
			point2.X = Math.Max(point2.X, Math.Max(link.FromNodeArrow.MiddlePoint.X + 10, link.ToNodeArrow.MiddlePoint.X + 10));
			point2.Y = Math.Max(point2.Y, Math.Max(link.FromNodeArrow.MiddlePoint.Y + 10, link.ToNodeArrow.MiddlePoint.Y + 10));
		}
		LastDiagramBox = new Rectangle(point.X, point.Y, point2.X - point.X, point2.Y - point.Y);
		return LastDiagramBox;
	}

	private void RecalculateLinksArrows()
	{
		foreach (Link link in Elements.Links)
		{
			link.ToNodeArrow.RecalculatePosition(link.FromNode, link.ToNode);
			link.FromNodeArrow.RecalculatePosition(link.FromNode, link.ToNode);
		}
	}
}
