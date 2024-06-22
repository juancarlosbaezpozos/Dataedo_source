using System.Collections.Generic;
using System.Xml.Serialization;
using Dataedo.App.API.Models;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public class SessionData
{
	[XmlElement]
	public string Token { get; set; }

	[XmlElement]
	public ProfileResult Profile { get; set; }

	[XmlElement]
	public List<LicenseDataResult> Licenses { get; set; }

	[XmlElement]
	public string LastLicenseId { get; set; }
}
