using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Common.WaitFormCanceling;

namespace Dataedo.App.Classes;

internal class ConnectionParameters
{
	public DatabaseRow DatabaseRow { get; set; }

	public Locker DbConnectionLocker { get; set; }

	public bool FullImport { get; set; }

	public bool UpdateEntireDocumentation { get; set; }

	public DocumentationCustomFieldRow[] CustomFields { get; set; }

	public bool IsDbAdded { get; set; }

	public Form Owner { get; set; }
}
