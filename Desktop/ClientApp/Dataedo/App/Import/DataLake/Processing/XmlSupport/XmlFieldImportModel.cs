using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dataedo.App.Import.DataLake.Model;

namespace Dataedo.App.Import.DataLake.Processing.XmlSupport;

[DebuggerDisplay("{FullName}")]
public class XmlFieldImportModel : FieldModel
{
	private Dictionary<string, XmlFieldImportModel> attributes;

	private Dictionary<string, XmlFieldImportModel> childFields;

	public XmlFieldImportModel(string name, FieldModel parentField)
	{
		base.Path = parentField?.FullName;
		base.Name = name;
		base.ParentField = parentField;
		base.Level = (parentField?.Level ?? 0) + 1;
		attributes = new Dictionary<string, XmlFieldImportModel>();
		childFields = new Dictionary<string, XmlFieldImportModel>();
	}

	public Dictionary<string, XmlFieldImportModel>.Enumerator GetAttributesEnumerator()
	{
		return attributes.GetEnumerator();
	}

	public Dictionary<string, XmlFieldImportModel>.Enumerator GetChildFieldsEnumerator()
	{
		return childFields.GetEnumerator();
	}

	public bool HasAttribute(string name)
	{
		return attributes.ContainsKey(name);
	}

	public void AddAttribute(XmlFieldImportModel field)
	{
		attributes.Add(field.Name, field);
	}

	public XmlFieldImportModel GetChildField(string name)
	{
		childFields.TryGetValue(name, out var value);
		return value;
	}

	public void AddChildField(XmlFieldImportModel field)
	{
		childFields.Add(field.Name, field);
	}

	public List<XmlFieldImportModel> GetFlattenData()
	{
		List<XmlFieldImportModel> list = new List<XmlFieldImportModel>();
		list.Add(this);
		list.AddRange(attributes.Values.AsEnumerable());
		foreach (KeyValuePair<string, XmlFieldImportModel> childField in childFields)
		{
			list.AddRange(childField.Value.GetFlattenData());
		}
		return list;
	}

	internal bool HasAttributes()
	{
		return attributes.Any();
	}

	internal bool HasChildNodes()
	{
		return childFields.Any();
	}
}
