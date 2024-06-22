using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;

namespace Dataedo.App.Tools.XmlExportTools.Model;

public class ExportData
{
	public List<ObjectRow> TablesSource { get; set; }

	public List<ObjectRow> ViewsSource { get; set; }

	public List<ObjectRow> ProceduresSource { get; set; }

	public List<ObjectRow> FunctionsSource { get; set; }

	public List<ObjectRow> StructuresSource { get; set; }

	public List<ModuleRow> ModulesSource { get; set; }

	public ModuleExportData ModulesData { get; set; }

	public bool SetObjectsDatabaseContext()
	{
		List<ObjectRow> tablesSource = TablesSource;
		if (tablesSource == null || !tablesSource.Any((ObjectRow x) => x.IsFromAnotherDatabase == true))
		{
			List<ObjectRow> viewsSource = ViewsSource;
			if (viewsSource == null || !viewsSource.Any((ObjectRow x) => x.IsFromAnotherDatabase == true))
			{
				List<ObjectRow> proceduresSource = ProceduresSource;
				if (proceduresSource == null || !proceduresSource.Any((ObjectRow x) => x.IsFromAnotherDatabase == true))
				{
					List<ObjectRow> functionsSource = FunctionsSource;
					if (functionsSource == null || !functionsSource.Any((ObjectRow x) => x.IsFromAnotherDatabase == true))
					{
						return false;
					}
				}
			}
		}
		TablesSource?.ForEach(delegate(ObjectRow x)
		{
			x.IsInMultipleDatabaseModule = true;
		});
		ViewsSource?.ForEach(delegate(ObjectRow x)
		{
			x.IsInMultipleDatabaseModule = true;
		});
		ProceduresSource?.ForEach(delegate(ObjectRow x)
		{
			x.IsInMultipleDatabaseModule = true;
		});
		FunctionsSource?.ForEach(delegate(ObjectRow x)
		{
			x.IsInMultipleDatabaseModule = true;
		});
		StructuresSource?.ForEach(delegate(ObjectRow x)
		{
			x.IsInMultipleDatabaseModule = true;
		});
		return true;
	}
}
