namespace Dataedo.App.Enums;

public class SynchronizeStateEnum
{
	public enum SynchronizeState
	{
		Unknown = 1,
		New = 2,
		Deleted = 3,
		Unsynchronized = 4,
		Synchronized = 5,
		Ignored = 6
	}

	public static string StateToString(SynchronizeState synchronizeState)
	{
		return synchronizeState switch
		{
			SynchronizeState.Unknown => "Unknown", 
			SynchronizeState.Deleted => "Deleted", 
			SynchronizeState.Ignored => "Ignored", 
			SynchronizeState.New => "New", 
			SynchronizeState.Synchronized => "Synchronized", 
			SynchronizeState.Unsynchronized => "Updated", 
			_ => null, 
		};
	}

	public static string StateToStringForFullImport(SynchronizeState synchronizeState)
	{
		if (synchronizeState == SynchronizeState.Unsynchronized)
		{
			return "Forced reimport";
		}
		return StateToString(synchronizeState);
	}

	public static string StateToStringForImage(SynchronizeState synchronizeState)
	{
		return synchronizeState switch
		{
			SynchronizeState.Deleted => "deleted", 
			SynchronizeState.New => "new", 
			SynchronizeState.Synchronized => string.Empty, 
			SynchronizeState.Unsynchronized => "updated", 
			_ => null, 
		};
	}

	public static SynchronizeState DBStringToState(string synchronizeStateString)
	{
		if (synchronizeStateString == "D")
		{
			return SynchronizeState.Deleted;
		}
		return SynchronizeState.Synchronized;
	}

	public static string StateToDBString(SynchronizeState synchronizeState)
	{
		if (synchronizeState == SynchronizeState.Deleted)
		{
			return "D";
		}
		return "A";
	}
}
