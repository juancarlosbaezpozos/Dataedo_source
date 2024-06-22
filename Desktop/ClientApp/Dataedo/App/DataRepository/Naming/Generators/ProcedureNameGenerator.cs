using System.Text;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

public class ProcedureNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		IProcedure procedure = model as IProcedure;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = procedure.Module != null && procedure.Documentation.Id != procedure.Module.Documentation.Id;
		bool flag2 = options.CurrentDocumentation != null && procedure.Documentation?.Id != options.CurrentDocumentation?.Id;
		if (options.Documentation && (flag || flag2))
		{
			string value = NameBuilder.For(procedure.Documentation).WithTitle().Build();
			stringBuilder.Append(value).Append(".");
		}
		if (options.Schema)
		{
			_ = procedure.Documentation.Type;
			if (!string.IsNullOrEmpty(procedure.Schema))
			{
				if (!procedure.Documentation.ShowSchemaEffective)
				{
					IDocumentation currentDocumentation = options.CurrentDocumentation;
					if (currentDocumentation == null || !currentDocumentation.ShowSchemaEffective)
					{
						goto IL_013f;
					}
				}
				stringBuilder.Append(procedure.Schema).Append(".");
			}
		}
		goto IL_013f;
		IL_013f:
		stringBuilder.Append(procedure.Name);
		if (options.Title && !string.IsNullOrEmpty(procedure.Title))
		{
			stringBuilder.Append(" (" + procedure.Title + ")");
		}
		return stringBuilder.ToString();
	}
}
