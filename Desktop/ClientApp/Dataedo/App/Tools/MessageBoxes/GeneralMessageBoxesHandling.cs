using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.GeneralHandling;
using Dataedo.CustomMessageBox;

namespace Dataedo.App.Tools.MessageBoxes;

public static class GeneralMessageBoxesHandling
{
	public class HandlingDialogResult
	{
		public HandlingResult HandlingResult { get; set; }

		public DialogResult? DialogResult { get; set; }

		public HandlingDialogResult(HandlingResult handlingResult, DialogResult? dialogResult)
		{
			HandlingResult = handlingResult;
			DialogResult = dialogResult;
		}
	}

	public static HandlingDialogResult Show(string message, string title, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon, string details = null, int numberOfSelectedButton = 1, Form owner = null, string messageSimple = null, MessageBoxParameters parameters = null)
	{
		HandlingResult obj = new HandlingResult(title, message, details)
		{
			MessageSimple = messageSimple
		};
		DialogResult? dialogResult = obj.Process(GeneralHandlingSupport.OverrideHandlingMethod ?? HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, messageBoxButtons, messageBoxIcon, numberOfSelectedButton, null, owner, parameters);
		StoreHandlingResultIfNeeded(obj);
		return new HandlingDialogResult(obj, dialogResult);
	}

	private static void StoreHandlingResultIfNeeded(HandlingResult handlingResult)
	{
		if (GeneralHandlingSupport.OverrideHandlingMethod == HandlingMethodEnumeration.HandlingMethod.NoActionStoreExceptions)
		{
			GeneralHandlingSupport.StoreResult(handlingResult);
		}
	}
}
