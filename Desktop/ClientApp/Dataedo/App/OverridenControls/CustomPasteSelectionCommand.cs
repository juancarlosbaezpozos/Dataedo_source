using DevExpress.Office.Commands.Internal;
using DevExpress.XtraRichEdit.Commands;

namespace Dataedo.App.OverridenControls;

public class CustomPasteSelectionCommand : PasteSelectionCommand
{
	public CustomPasteSelectionCommand(IRichEditControlWithPasteAwareness control)
		: base(control)
	{
	}

	protected override RichEditCommand CreateInsertObjectCommand()
	{
		return new CustomPasteSelectionCoreCommand(base.Control as IRichEditControlWithPasteAwareness, new ClipboardPasteSource());
	}
}
