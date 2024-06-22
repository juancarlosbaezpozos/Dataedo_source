namespace Dataedo.App.LoginFormTools.Tools.Enums;

public static class ActionResultEnum
{
	public enum ActionResult
	{
		None = 0,
		Next = 1,
		Back = 2,
		Cancel = 3,
		SignedIn = 4,
		SignedOut = 5,
		StartPage = 6,
		AccessCode = 7,
		Licenses = 8,
		FileLicenses = 9,
		CreateRepository = 10,
		CreateFileRepository = 11,
		OpenFileRepository = 12,
		OpenFileRepositoryForInvalidFile = 13,
		ConnectToRepository = 14,
		ConnectToServerRepository = 15,
		Connected = 16,
		CreateServerRepository = 17,
		CreateRepositoryProgress = 18,
		UpgradeServerRepository = 19,
		UpgradeServerRepositoryConnectedUsers = 20,
		UpgradeRepositoryProgress = 21,
		OperationCompletedSuccessfully = 22,
		OperationCompletedNotSuccessfully = 23
	}
}
