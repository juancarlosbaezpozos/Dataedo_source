using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class Term : IMenuTreeItem
{
	internal ITerm Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id => ObjectId;

	public string ObjectId => LinkGenerator.ToTermObjectId(Model.Id);

	public string MenuType => "term";

	public string MenuSubtype => BusinessGlossarySupport.GetTermIconName(Model.TypeIconId, string.Empty);

	public bool IsUserDefined => false;

	public string MenuName => NameBuilder.For(Model).Full().Build();

	public IEnumerable<string> ColumnNames => null;

	public IEnumerable<IMenuTreeItem> MenuChildren => Children();

	public Term(ITerm model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}

	private IEnumerable<IMenuTreeItem> Children()
	{
		return DataConverter.ToMenuTree(Model.Terms, this);
	}
}
