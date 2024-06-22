using System.Text;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

public class FunctionNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		IFunction function = model as IFunction;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = function.Module != null && function.Documentation.Id != function.Module.Documentation.Id;
		bool flag2 = options.CurrentDocumentation != null && function.Documentation?.Id != options.CurrentDocumentation?.Id;
		if (options.Documentation && (flag || flag2))
		{
			string value = NameBuilder.For(function.Documentation).WithTitle().Build();
			stringBuilder.Append(value).Append(".");
		}
		if (options.Schema)
		{
			_ = function.Documentation.Type;
			if (!string.IsNullOrEmpty(function.Schema))
			{
				if (!function.Documentation.ShowSchemaEffective)
				{
					IDocumentation currentDocumentation = options.CurrentDocumentation;
					if (currentDocumentation == null || !currentDocumentation.ShowSchemaEffective)
					{
						goto IL_013f;
					}
				}
				stringBuilder.Append(function.Schema).Append(".");
			}
		}
		goto IL_013f;
		IL_013f:
		stringBuilder.Append(function.Name);
		if (options.Title && !string.IsNullOrEmpty(function.Title))
		{
			stringBuilder.Append(" (" + function.Title + ")");
		}
		return stringBuilder.ToString();
	}
}
