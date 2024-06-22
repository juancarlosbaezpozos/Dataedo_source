using System;
using Dataedo.App.Tools.Exceptions;
using DevExpress.Services.Implementation;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Commands;

namespace Dataedo.App.UserControls;

public class CustomCommandExecutionListenerService : CommandExecutionListenerService
{
	private RichEditControl control;

	private int beforePastePostion;

	public CustomCommandExecutionListenerService(RichEditControl control)
	{
		this.control = control;
	}

	public override void BeginCommandExecution(Command command, ICommandUIState state)
	{
		try
		{
			if (command is PasteSelectionCommand)
			{
				beforePastePostion = control.Document.CaretPosition.ToInt();
			}
			base.BeginCommandExecution(command, state);
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, control?.FindForm());
		}
	}

	public override void EndCommandExecution(Command command, ICommandUIState state)
	{
		try
		{
			int num = control.Document.CaretPosition.ToInt() - beforePastePostion - 1;
			if (command is PasteSelectionCommand && num > 0)
			{
				DocumentRange range = control.Document.CreateRange(beforePastePostion, num);
				DocumentImageCollection images = control.Document.GetImages(range);
				for (int i = 0; i < images.Count; i++)
				{
					images[i].ScaleX = 1f;
					images[i].ScaleY = 1f;
				}
			}
			base.EndCommandExecution(command, state);
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, control?.FindForm());
		}
	}
}
