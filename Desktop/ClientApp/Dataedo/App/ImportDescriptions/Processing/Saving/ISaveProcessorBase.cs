using System.Windows.Forms;

namespace Dataedo.App.ImportDescriptions.Processing.Saving;

public interface ISaveProcessorBase
{
	bool ProcessSaving(int databaseId, Form owner);
}
