namespace Dataedo.App.Common.WaitFormCanceling;

public class Locker
{
	public bool IsCanceled { get; set; }

	public Locker()
	{
		IsCanceled = false;
	}
}
