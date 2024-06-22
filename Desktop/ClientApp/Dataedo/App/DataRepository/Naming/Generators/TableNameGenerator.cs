using System.Text;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

public class TableNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		ITable table = model as ITable;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = table.Module != null && table.Documentation.Id != table.Module.Documentation.Id;
		bool flag2 = options.CurrentDocumentation != null && table.Documentation.Id != options.CurrentDocumentation.Id;
		if (options.Documentation && (flag || flag2))
		{
			string value = NameBuilder.For(table.Documentation).WithTitle().Build();
			stringBuilder.Append(value).Append(".");
		}
		if (options.Schema)
		{
			_ = table.Documentation.Type;
			if (!string.IsNullOrEmpty(table.Schema) && (table.Documentation.ShowSchemaEffective || options.SchowSchema))
			{
				stringBuilder.Append(table.Schema).Append(".");
			}
		}
		stringBuilder.Append(table.Name);
		if (options.Title && !string.IsNullOrEmpty(table.Title))
		{
			stringBuilder.Append(" (" + table.Title + ")");
		}
		return stringBuilder.ToString();
	}
}
