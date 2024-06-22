using System.Collections.Generic;

namespace Dataedo.App.Enums;

public static class InstanceIdentifierEnum
{
	public enum InstanceIdentifier
	{
		ServiceName = 0,
		SID = 1
	}

	public static InstanceIdentifier StringToType(string instanceIdentifierString)
	{
		if (!(instanceIdentifierString == "SERVICE_NAME"))
		{
			if (instanceIdentifierString == "SID")
			{
				return InstanceIdentifier.SID;
			}
			return InstanceIdentifier.ServiceName;
		}
		return InstanceIdentifier.ServiceName;
	}

	public static string TypeToString(InstanceIdentifier? instanceIdentifier)
	{
		return instanceIdentifier switch
		{
			InstanceIdentifier.ServiceName => "SERVICE_NAME", 
			InstanceIdentifier.SID => "SID", 
			_ => null, 
		};
	}

	public static string TypeToStringForDisplay(InstanceIdentifier? instanceIdentifier)
	{
		return instanceIdentifier switch
		{
			InstanceIdentifier.ServiceName => "Service name", 
			InstanceIdentifier.SID => "SID", 
			_ => null, 
		};
	}

	public static Dictionary<InstanceIdentifier, string> GetInstanceIdentifiers()
	{
		return new Dictionary<InstanceIdentifier, string>
		{
			{
				InstanceIdentifier.ServiceName,
				TypeToStringForDisplay(InstanceIdentifier.ServiceName)
			},
			{
				InstanceIdentifier.SID,
				TypeToStringForDisplay(InstanceIdentifier.SID)
			}
		};
	}
}
