using System;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Services;

namespace Dataedo.App.OverridenControls;

public class CustomRichEditCommandFactoryService : IRichEditCommandFactoryService
{
	private readonly IRichEditCommandFactoryService service;

	private readonly RichEditControl control;

	public EventHandler pasteEventHadler;

	public CustomRichEditCommandFactoryService(RichEditControl control, IRichEditCommandFactoryService service)
	{
		Guard.ArgumentNotNull(control, "control");
		Guard.ArgumentNotNull(service, "service");
		this.control = control;
		this.service = service;
	}

	public RichEditCommand CreateCommand(RichEditCommandId id)
	{
		if (id == RichEditCommandId.PasteSelection)
		{
			return new CustomPasteSelectionCommand(control as IRichEditControlWithPasteAwareness);
		}
		return service.CreateCommand(id);
	}
}
