namespace Dataedo.App.Tools.Exceptions;

public class HandlingMethodEnumeration
{
	public enum HandlingMethod
	{
		NoAction = 0,
		NoActionStoreExceptions = 1,
		NoActionAndThrowIfOtherException = 2,
		ShowMessageBox = 3,
		ShowMessageBoxAndThrowIfOtherException = 4,
		ThrowAlways = 5,
		LogInErrorLog = 6
	}
}
