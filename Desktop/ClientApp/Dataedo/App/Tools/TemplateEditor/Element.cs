using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Dataedo.App.Tools.TemplateEditor;

public class Element
{
	public delegate void ValueChanged(object value);

	private List<Element> subElements;

	private object value;

	[XmlIgnore]
	public int Id { get; set; }

	[XmlIgnore]
	public int ParentId
	{
		get
		{
			if (Parent != null)
			{
				return Parent.Id;
			}
			return 0;
		}
	}

	[XmlIgnore]
	public Element Parent { get; set; }

	[XmlIgnore]
	public string ElementName { get; set; }

	[XmlIgnore]
	public object Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			this.valueChanged?.Invoke(value);
		}
	}

	[XmlIgnore]
	public ElementTypeEnum.ElementType ElementType { get; set; }

	[XmlIgnore]
	public bool IsLeaf
	{
		get
		{
			List<Element> list = subElements;
			if (list == null)
			{
				return false;
			}
			return list.Count == 0;
		}
	}

	[XmlIgnore]
	public string Tooltip
	{
		get
		{
			if (ElementTypeEnum.ElementType.Font.Equals(ElementType))
			{
				return "Dataedo supports fonts with TrueType outlines. Usually, fonts with TrueType outlines have the TTF extension.";
			}
			if (ElementTypeEnum.ElementType.Link.Equals(ElementType))
			{
				return "URL to go after clicking this element.";
			}
			if ("Text".Equals(ElementName) && "Header".Equals(Parent?.ElementName) && "Title page".Equals(Parent?.Parent?.ElementName))
			{
				return "Default value: company name";
			}
			if ("Text".Equals(ElementName) && "Title".Equals(Parent?.ElementName) && "Title page".Equals(Parent?.Parent?.ElementName))
			{
				return "Default value: documentation title";
			}
			if ("Image".Equals(ElementName) && "Title page".Equals(Parent?.ElementName))
			{
				return "Paste your logo here";
			}
			return null;
		}
	}

	public event ValueChanged valueChanged;

	public Element()
	{
		subElements = new List<Element>();
	}

	public virtual void Initialize(ref int id, Element parent, string name = null)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			name = GetType().Name;
		}
		Id = id++;
		ElementName = name;
		if (parent != null)
		{
			Parent = parent;
			parent.AddSubElement(this);
		}
	}

	public void Add(ref int id, string name, object value, ValueChanged valueChanged, ElementTypeEnum.ElementType type = ElementTypeEnum.ElementType.Default)
	{
		Element element = new Element();
		element.ElementName = name;
		element.Value = value;
		element.ElementType = type;
		element.Initialize(ref id, this, name);
		element.valueChanged = valueChanged;
	}

	public void Add<T>(ref int id, T element, string name = null) where T : Element, new()
	{
		(element ?? new T())?.Initialize(ref id, this, name);
	}

	public void AddSubElement(Element element)
	{
		subElements.Add(element);
	}

	public void ClearSubElements()
	{
		subElements.Clear();
	}

	public List<Element> GetAllSubElements()
	{
		List<Element> list = new List<Element>();
		foreach (Element subElement in subElements)
		{
			list.AddRange(subElement.GetAllSubElements());
		}
		if (ParentId > 0)
		{
			list.Add(this);
		}
		return list;
	}

	public List<Element> GetSubElements()
	{
		return subElements;
	}

	public Element GetSubElement(string name)
	{
		if (string.IsNullOrWhiteSpace(name) || IsLeaf)
		{
			return null;
		}
		return GetSubElements().FirstOrDefault((Element x) => name.Equals(x.ElementName));
	}

	public bool IsChild(Type type)
	{
		Element parent = Parent;
		if (parent == null)
		{
			return false;
		}
		if (parent.GetType() == type)
		{
			return true;
		}
		return parent.IsChild(type);
	}
}
