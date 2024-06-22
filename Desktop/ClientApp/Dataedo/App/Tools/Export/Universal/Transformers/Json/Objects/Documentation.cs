using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class Documentation : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Documentation MenuTreeItem;

	internal IDocumentation Model;

	public string ObjectId => MenuTreeItem.ObjectId;

	[JsonProperty("name")]
	public string Name => Model.Title;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("summary")]
	public IList<SummaryField> Summary { get; private set; }

	[JsonProperty("modules")]
	public IEnumerable<IdName> Modules { get; private set; }

	[JsonProperty("dbObjects")]
	public IEnumerable<IdNameCount> DbObjects { get; private set; }

	[JsonProperty("show_schema")]
	public bool ShowSchema { get; set; }

	public Documentation(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Documentation menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		Model = menuTreeItem.Model;
		Summary = new List<SummaryField>();
		if (!string.IsNullOrEmpty(Model.Host))
		{
			Summary.Add(new SummaryField("Host", Model.Host));
		}
		if (!string.IsNullOrEmpty(Model.Name))
		{
			Summary.Add(new SummaryField("Database", Model.Name));
		}
		if (!string.IsNullOrEmpty(Model.Dbms))
		{
			Summary.Add(new SummaryField("DBMS", Model.Dbms));
		}
		foreach (ICustomField item in Model.CustomFields.Where((ICustomField x) => CustomField.HasValue(x)))
		{
			SummaryFieldCustomFieldValue customField = new SummaryFieldCustomFieldValue(item);
			Summary.Add(new SummaryField(item.Name, customField));
		}
		Modules = from x in GetModules()
			select new IdName(x.Id, x.MenuName);
		DbObjects = from x in GetDbObjects()
			select new IdNameCount(x.Id, x.MenuName, x.MenuChildren.Count());
		ShowSchema = menuTreeItem.Model.ShowSchemaEffective;
	}

	private IEnumerable<IMenuTreeItem> GetModules()
	{
		List<IMenuTreeItem> list = new List<IMenuTreeItem>();
		foreach (IMenuTreeItem menuChild in MenuTreeItem.MenuChildren)
		{
			if (typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Modules).IsAssignableFrom(menuChild.GetType()))
			{
				list.AddRange(menuChild.MenuChildren);
			}
		}
		return list;
	}

	private IEnumerable<IMenuTreeItem> GetDbObjects()
	{
		List<IMenuTreeItem> list = new List<IMenuTreeItem>();
		foreach (IMenuTreeItem menuChild in MenuTreeItem.MenuChildren)
		{
			if (!typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Modules).IsAssignableFrom(menuChild.GetType()))
			{
				list.Add(menuChild);
			}
		}
		return list;
	}
}
