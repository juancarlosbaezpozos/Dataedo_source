using System.Collections.Generic;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.Model.Data.Interfaces;

namespace Dataedo.App.UserControls.DataLineage;

public class ProcessesContainer : IName
{
	private DataProcessesCollection dataProcessesCollection;

	public string Name { get; set; }

	public List<DataProcessRow> Processes => dataProcessesCollection.AllDataProcesses;

	protected ProcessesContainer(DataProcessesCollection dataProcessesCollection)
	{
		this.dataProcessesCollection = dataProcessesCollection;
	}
}
