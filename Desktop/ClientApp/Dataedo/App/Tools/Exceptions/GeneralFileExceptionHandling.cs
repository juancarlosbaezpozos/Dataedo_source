using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Dataedo.App.Tools.Exceptions;

internal static class GeneralFileExceptionHandling
{
	private static readonly Type[] handledExceptionsTypes = new Type[2]
	{
		typeof(UnauthorizedAccessException),
		typeof(IOException)
	};

	public static bool IsTypeSupportedForHandling(Type exceptionType)
	{
		return handledExceptionsTypes.Contains(exceptionType);
	}

	public static HandlingResult Handle(Exception exception, Form handlingWindowOwner = null)
	{
		return Handle(exception, null, null, HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, HandlingMethodEnumeration.HandlingMethod handlingMethod, Form handlingWindowOwner = null)
	{
		return Handle(exception, null, null, handlingMethod, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, string message, Form handlingWindowOwner = null)
	{
		return Handle(exception, message, null, HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, string message, string caption, Form handlingWindowOwner = null)
	{
		return Handle(exception, caption, message, HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, string message, string caption, HandlingMethodEnumeration.HandlingMethod handlingMethod, Form handlingWindowOwner = null)
	{
		message = (string.IsNullOrEmpty(message) ? message : (message + Environment.NewLine));
		if (string.IsNullOrEmpty(caption))
		{
			caption = "Error";
		}
		HandlingResult handlingResult = ((exception is UnauthorizedAccessException) ? new HandlingResult(caption, (message ?? "").TrimEnd(), exception?.Message) : ((!(exception is IOException)) ? OtherExceptionHandler(exception, message) : new HandlingResult(caption, message + " File is in use.", exception?.Message)));
		handlingResult.Process(handlingMethod, handlingWindowOwner);
		return handlingResult;
	}

	private static HandlingResult OtherExceptionHandler(Exception exception, string message = null)
	{
		HandlingResult handlingResult = ((!string.IsNullOrEmpty(message)) ? new HandlingResult(null, message + exception?.Message, null) : new HandlingResult(null, "An unexpected error occurred.", null));
		handlingResult.Exception = exception;
		handlingResult.IsException = true;
		handlingResult.IsUnhandled = true;
		return handlingResult;
	}
}
