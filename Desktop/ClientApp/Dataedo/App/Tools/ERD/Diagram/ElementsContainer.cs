using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Tools.ERD.Canvas;

namespace Dataedo.App.Tools.ERD.Diagram;

public class ElementsContainer
{
	private Dictionary<int, Node> nodes = new Dictionary<int, Node>();

	private Dictionary<int, Link> links = new Dictionary<int, Link>();

	private Dictionary<int, PostIt> postIts = new Dictionary<int, PostIt>();

	public IEnumerable<Node> Nodes => nodes.Values;

	public IEnumerable<Link> Links => links.Values;

	public IEnumerable<PostIt> PostIts => postIts.Values;

	public IEnumerable<RectangularObject> RectangularObjects
	{
		get
		{
			foreach (Node node in Nodes)
			{
				yield return node;
			}
			foreach (PostIt postIt in PostIts)
			{
				yield return postIt;
			}
		}
	}

	public IEnumerable<CanvasObject> AllElementsLinksFirst
	{
		get
		{
			foreach (PostIt item in PostIts.Where((PostIt x) => x.Layer == PostItLayerEnum.PostItLayer.Back))
			{
				yield return item;
			}
			foreach (Link link in Links)
			{
				yield return link;
			}
			foreach (Node node in Nodes)
			{
				yield return node;
			}
			foreach (PostIt item2 in PostIts.Where((PostIt x) => x.Layer == PostItLayerEnum.PostItLayer.Front))
			{
				yield return item2;
			}
		}
	}

	public IEnumerable<CanvasObject> FrontToBackAllElementsNodesFirst
	{
		get
		{
			foreach (PostIt item in PostIts.Where((PostIt x) => x.Layer == PostItLayerEnum.PostItLayer.Front).Reverse())
			{
				yield return item;
			}
			foreach (Node item2 in Nodes.Reverse())
			{
				yield return item2;
			}
			foreach (Link item3 in Links.Reverse())
			{
				yield return item3;
			}
			foreach (PostIt item4 in PostIts.Where((PostIt x) => x.Layer == PostItLayerEnum.PostItLayer.Back).Reverse())
			{
				yield return item4;
			}
		}
	}

	public int NodesCount => nodes.Count;

	public int PostItsCount => postIts.Count;

	public bool HasAnyNodes => nodes.Any();

	public bool HasAnyLinks => links.Any();

	public bool HasAnyPostIts => postIts.Any();

	public event EventHandler Changed;

	public void ClearElements()
	{
		nodes.Clear();
		postIts.Clear();
	}

	public RectangularObject GetElement(int key)
	{
		if (nodes.TryGetValue(key, out var value))
		{
			return value;
		}
		if (postIts.TryGetValue(key, out var value2))
		{
			return value2;
		}
		return null;
	}

	public Node GetNode(int key)
	{
		if (nodes.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public PostIt GetPostIt(int key)
	{
		if (postIts.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool HasCollectionMultipleTypes(IEnumerable<RectangularObjectKey> keys)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (RectangularObjectKey key in keys)
		{
			if (!flag && key.Type == RectangularObjectType.Node)
			{
				if (flag2)
				{
					return true;
				}
				flag = true;
			}
			if (!flag2 && key.Type == RectangularObjectType.PostIt)
			{
				if (flag)
				{
					return true;
				}
				flag2 = true;
			}
		}
		return false;
	}

	public void AddNode(Node node)
	{
		if (!NodeExists(node.Key))
		{
			node.Changed += ItemValueChanged;
			nodes[node.Key] = node;
			NotifyChange();
		}
	}

	public void AddPostIt(PostIt postIt)
	{
		postIt.Changed += ItemValueChanged;
		postIts[postIt.Key] = postIt;
		NotifyChange();
	}

	public void RemoveElementByKey(int key)
	{
		RectangularObject element = GetElement(key);
		RemoveElement(element);
	}

	public void RemoveElement(RectangularObject element)
	{
		if (element != null)
		{
			element.Changed -= ItemValueChanged;
			NotifyChange();
			if (element is Node)
			{
				nodes.Remove(element.Key);
			}
			else if (element is PostIt)
			{
				postIts.Remove(element.Key);
			}
		}
	}

	public bool NodeExists(int key)
	{
		return nodes.ContainsKey(key);
	}

	public void ClearLinks()
	{
		links.Clear();
	}

	public Link GetLink(int key)
	{
		if (links.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public void AddLink(Link link)
	{
		if (!LinkExists(link.Key))
		{
			link.Changed += ItemValueChanged;
			links[link.Key] = link;
			NotifyChange();
		}
	}

	public void RemoveLinkByKey(int key)
	{
		Link link = GetLink(key);
		RemoveLink(link);
	}

	public void RemoveLink(Link link)
	{
		if (link != null)
		{
			link.Changed -= ItemValueChanged;
			NotifyChange();
			links.Remove(link.Key);
		}
	}

	public bool LinkExists(int key)
	{
		return links.ContainsKey(key);
	}

	private void ItemValueChanged(object sender, EventArgs e)
	{
		NotifyChange();
	}

	protected void NotifyChange(EventArgs e = null)
	{
		EventArgs e2 = e ?? EventArgs.Empty;
		this.Changed?.Invoke(this, e2);
	}

	public RectangularObject GetFirstSelectedObject()
	{
		RectangularObject rectangularObject = RectangularObjects.FirstOrDefault((RectangularObject x) => x.Selected);
		if (rectangularObject != null)
		{
			return rectangularObject;
		}
		return null;
	}
}
