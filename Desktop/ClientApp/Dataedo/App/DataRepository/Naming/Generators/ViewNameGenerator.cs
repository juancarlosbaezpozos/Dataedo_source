using System.Text;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

public class ViewNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		IView view = model as IView;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = view.Module != null && view.Documentation.Id != view.Module.Documentation.Id;
		bool flag2 = options.CurrentDocumentation != null && view.Documentation?.Id != options.CurrentDocumentation?.Id;
		if (options.Documentation && (flag || flag2))
		{
			string value = NameBuilder.For(view.Documentation).WithTitle().Build();
			stringBuilder.Append(value).Append(".");
		}
		if (options.Schema)
		{
			_ = view.Documentation.Type;
			if (!string.IsNullOrEmpty(view.Schema))
			{
				if (!view.Documentation.ShowSchemaEffective)
				{
					IDocumentation currentDocumentation = options.CurrentDocumentation;
					if (currentDocumentation == null || !currentDocumentation.ShowSchemaEffective)
					{
						goto IL_013f;
					}
				}
				stringBuilder.Append(view.Schema).Append(".");
			}
		}
		goto IL_013f;
		IL_013f:
		stringBuilder.Append(view.Name);
		if (options.Title && !string.IsNullOrEmpty(view.Title))
		{
			stringBuilder.Append(" (" + view.Title + ")");
		}
		return stringBuilder.ToString();
	}
}
