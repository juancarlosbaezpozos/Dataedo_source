namespace Dataedo.App.LoginFormTools.Tools.Common;

internal class ConnectionResult
{
	public static readonly ConnectionResult Empty = new ConnectionResult(null, null);

	public bool? IsLicenseValid { get; set; }

	public bool? IsConnectionDataValid { get; set; }

	public bool IsSuccess
	{
		get
		{
			if (IsLicenseValid == true)
			{
				return IsConnectionDataValid == true;
			}
			return false;
		}
	}

	public ConnectionResult(bool? isLicenseValid, bool? isConnectionDataValid)
	{
		IsLicenseValid = isLicenseValid;
		IsConnectionDataValid = isConnectionDataValid;
	}
}
