using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Commands;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class SessionDB
{
	protected CommandsSetBase commands;

	public SessionDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public bool InsertSessionLog(bool isLoginSuccess, Form owner = null)
	{
		try
		{
			if (!Dataedo.App.StaticData.IsProjectFile)
			{
				DateTime now = DateTime.Now;
				string ip2 = string.Empty;
				if (NetworkInterface.GetIsNetworkAvailable())
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
					if (hostEntry != null)
					{
						IPAddress iPAddress = hostEntry.AddressList.FirstOrDefault((IPAddress ip) => ip.AddressFamily == AddressFamily.InterNetwork);
						if (iPAddress != null)
						{
							ip2 = iPAddress.ToString();
						}
					}
				}
				DateTime? licenseStart = null;
				if (Dataedo.App.StaticData.License.Start.HasValue)
				{
					licenseStart = DateTimeOffset.FromUnixTimeSeconds(Dataedo.App.StaticData.License.Start.Value).UtcDateTime;
				}
				string authentication = null;
				if (LastConnectionInfo.LOGIN_INFO.AuthenticationType != null)
				{
					authentication = AuthenticationType.GetAuthenticationNameForLogging(AuthenticationType.StringToType(LastConnectionInfo.LOGIN_INFO.AuthenticationType));
				}
				commands.Manipulation.Sessions.InsertSession(LastConnectionInfo.LOGIN_INFO.DataedoLogin, null, now, authentication, isLoginSuccess ? "OK" : "FAIL", (Dataedo.App.StaticData.LicenseEnum == LicenseEnum.DataedoAccount) ? "ONLINE" : "LOCAL", Dataedo.App.StaticData.License.Type, Dataedo.App.StaticData.License.Id, Dataedo.App.StaticData.License.PackageCode, Dataedo.App.StaticData.License.PackageName, licenseStart, Dataedo.App.StaticData.License.EndUtcDateTime, Dataedo.App.StaticData.License.AccountId, Dataedo.App.StaticData.License.AccountName, "DESKTOP", ProgramVersion.VersionWithBuild, Dataedo.App.StaticData.Host, ip2, now, now, null);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting value of the 'sessions' record:", owner);
			return false;
		}
		return true;
	}
}
