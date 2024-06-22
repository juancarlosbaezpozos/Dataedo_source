using Dataedo.Data.Commands;
using Dataedo.Model.Data.Community;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class CommunityDB
{
	private CommandsSetBase commands;

	public CommunityDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public void InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType? objectType, int? objectId)
	{
		if (!Dataedo.App.StaticData.IsProjectFile)
		{
			string text = SharedObjectTypeEnum.TypeToStringForFollowing(objectType);
			if (objectId.HasValue && objectId != -1 && Dataedo.App.StaticData.CurrentLicenseId != -1 && !string.IsNullOrEmpty(text))
			{
				FollowingObjectDataObject followingObject = new FollowingObjectDataObject(Dataedo.App.StaticData.CurrentLicenseId, text, objectId);
				commands.Manipulation.Community.InsertFollowingDataToFollowingTable(followingObject);
			}
		}
	}

	private bool IsFollowingRowExist(int lastLicenseId, string objectType, int? objectId)
	{
		FollowingObjectDataObject followingObjectDataObject = new FollowingObjectDataObject(lastLicenseId, objectType, objectId);
		return commands.Select.Community.CheckIsFollowingRowExists(followingObjectDataObject);
	}
}
