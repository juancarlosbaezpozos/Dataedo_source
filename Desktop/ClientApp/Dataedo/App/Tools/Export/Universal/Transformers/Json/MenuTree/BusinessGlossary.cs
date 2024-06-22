using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class BusinessGlossary : IMenuTreeItem
{
	internal IBusinessGlossary Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id => ObjectId;

	public string ObjectId => LinkGenerator.ToBusinessGlossaryObjectId(Model.Id);

	public string MenuType => "business-glossary";

	public string MenuSubtype => null;

	public bool IsUserDefined => false;

	public string MenuName => NameBuilder.For(Model).Full().Build();

	public IEnumerable<string> ColumnNames => null;

	public IEnumerable<IMenuTreeItem> MenuChildren => Children();

	public BusinessGlossary(IBusinessGlossary model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}

	private IEnumerable<IMenuTreeItem> Children()
	{
		List<IMenuTreeItem> list = new List<IMenuTreeItem>();
		if (Model.Terms != null && Model.Terms.Count > 0)
		{
			Terms terms = new Terms(this)
			{
				Id = Id + "terms",
				ObjectId = Id + "terms",
				MenuType = "terms",
				MenuName = "Terms",
				ColumnNames = null
			};
			terms.MenuChildren = DataConverter.ToMenuTree(Model.Terms, this);
			list.Add(terms);
		}
		return list;
	}
}
