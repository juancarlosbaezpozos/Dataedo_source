using System.Windows.Forms;
using DevExpress.Office.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;

namespace Dataedo.App.OverridenControls;

public class CustomPasteSelectionCoreCommand : PasteSelectionCoreCommand
{
	public CustomPasteSelectionCoreCommand(IRichEditControlWithPasteAwareness control, PasteSource pasteSource)
		: base(control, pasteSource)
	{
	}

	public override void ForceExecute(ICommandUIState state)
	{
		base.Control.Document.InsertText(base.Control.Document.CaretPosition, Clipboard.GetText());
		(base.Control as IRichEditControlWithPasteAwareness)?.TriggerTextPastedEvent();
	}
}
