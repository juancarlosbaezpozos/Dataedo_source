using System.Windows.Forms;
using Dataedo.App.Common.WaitFormCanceling;
using Dataedo.App.Tools.CommandLine.Tools;

namespace Dataedo.App.Classes.Synchronize;

public class SynchronizeParameters
{
	public const int NumberOfSteps = 11;

	public Dataedo.App.Tools.CommandLine.Tools.Log Log { get; set; }

	public DatabaseRow DatabaseRow { get; set; }

	public string DatabaseName => this?.DatabaseRow?.Name;

	public bool IsDbAdded { get; set; }

	public int SynchObjectsCount { get; set; }

	public bool ImportDependencies { get; set; }

	public Locker DbSynchLocker { get; set; }

	public int AllObjectsCount => SynchObjectsCount + 11;

	public bool FullImport { get; set; }

	public bool UpdateEntireDocumentation { get; set; }

	public DocumentationCustomFieldRow[] CustomFields { get; set; }

	public int UpdateId { get; set; }

	public Form Owner { get; set; }
}
