using System;
using Dataedo.App.Data.General;

namespace Dataedo.App.DatabasesSupport;

[Serializable]
public class ConnectorVersionInfo
{
	public DatabaseVersionUpdate FirstSupportedVersion;

	public DatabaseVersionUpdate FirstNotSupportedVersion;

	public ConnectorVersionInfo()
	{
	}

	public ConnectorVersionInfo(string firstSupported)
	{
		FirstSupportedVersion = new DatabaseVersionUpdate(firstSupported);
	}

	public ConnectorVersionInfo(string firstSupported, string firstNotSupported)
	{
		FirstSupportedVersion = new DatabaseVersionUpdate(firstSupported);
		FirstNotSupportedVersion = new DatabaseVersionUpdate(firstNotSupported);
	}

	public ConnectorVersionInfo(DatabaseVersionUpdate firstSupported, DatabaseVersionUpdate firstNotSupported)
	{
		FirstSupportedVersion = firstSupported;
		FirstNotSupportedVersion = firstNotSupported;
	}

	public override bool Equals(object obj)
	{
		if (obj is ConnectorVersionInfo connectorVersionInfo)
		{
			if (FirstSupportedVersion == connectorVersionInfo.FirstSupportedVersion)
			{
				return FirstNotSupportedVersion == connectorVersionInfo.FirstNotSupportedVersion;
			}
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(FirstSupportedVersion?.GetHashCode(), FirstNotSupportedVersion?.GetHashCode());
	}
}
