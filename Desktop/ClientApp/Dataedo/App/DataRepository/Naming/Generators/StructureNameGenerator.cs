using System.Text;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

public class StructureNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		IStructure structure = model as IStructure;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = structure.Module != null && structure.Documentation.Id != structure.Module.Documentation.Id;
		bool flag2 = options.CurrentDocumentation != null && structure.Documentation.Id != options.CurrentDocumentation.Id;
		if (options.Documentation && (flag || flag2))
		{
			string value = NameBuilder.For(structure.Documentation).WithTitle().Build();
			stringBuilder.Append(value).Append(".");
		}
		if (options.Schema)
		{
			_ = structure.Documentation.Type;
			if (!string.IsNullOrEmpty(structure.Schema) && (structure.Documentation.ShowSchemaEffective || options.SchowSchema))
			{
				stringBuilder.Append(structure.Schema).Append(".");
			}
		}
		stringBuilder.Append(structure.Name);
		if (options.Title && !string.IsNullOrEmpty(structure.Title))
		{
			stringBuilder.Append(" (" + structure.Title + ")");
		}
		return stringBuilder.ToString();
	}
}
