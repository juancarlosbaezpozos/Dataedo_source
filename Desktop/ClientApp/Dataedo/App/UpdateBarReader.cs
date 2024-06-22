using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UpdateBar;
using Dataedo.LicenseHelperLibrary.Repository;

namespace Dataedo.App;

public class UpdateBarReader
{
	private Versions versionList;

	private UpdateVersion bigUpdateVersion;

	private UpdateVersion smallUpdateVersion;

	private XDocument xml;

	private XElement highestVersionWithAttribute;

	private UpdateVersion highestRepositoryVersion;

	public string MajorVersionLink => Links.GetDownloadLink(bigUpdateVersion.Major);

	public string MinorVersionLink => Links.GetDownloadLink(smallUpdateVersion.Major);

	public async Task SetVersions()
	{
		await LoadVersionList();
	}

	public bool CheckMinorUpdateAvailability()
	{
		if (smallUpdateVersion.Major == ProgramVersion.Major)
		{
			if (smallUpdateVersion.Minor <= ProgramVersion.Minor)
			{
				if (smallUpdateVersion.Minor >= ProgramVersion.Minor)
				{
					return smallUpdateVersion.Build > ProgramVersion.Build;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public bool CheckMajorUpdateAvailability()
	{
		if (bigUpdateVersion.Major > ProgramVersion.Major)
		{
			return bigUpdateVersion.Major != smallUpdateVersion.Major;
		}
		return false;
	}

	public string GetHighestVersionDownloadLink()
	{
		if (CheckMajorUpdateAvailability())
		{
			return MajorVersionLink;
		}
		if (CheckMinorUpdateAvailability())
		{
			return MinorVersionLink;
		}
		return string.Empty;
	}

	public string GetMajorVersionString()
	{
		return $"{bigUpdateVersion.Major}.{bigUpdateVersion.Minor}.{bigUpdateVersion.Build}";
	}

	public string GetMinorVersionString()
	{
		return $"{smallUpdateVersion.Major}.{smallUpdateVersion.Minor}.{smallUpdateVersion.Build}";
	}

	private void SetVersionWithAttribute()
	{
		highestVersionWithAttribute = xml.Descendants("Versions").Descendants("Version").Single((XElement x) => (string)x.Attribute("id") == "all");
	}

	private void RemoveVersionWithAttribute()
	{
		(from x in xml.Descendants("Versions").Descendants("Version")
			where (string)x.Attribute("id") == "all"
			select x).Remove();
	}

	private void RemoveVersionsWithReleaseType()
	{
		(from x in xml.Descendants("Versions").Descendants("Version")
			where (string)x.Element("ReleaseType") != string.Empty
			select x).Remove();
	}

	private async Task LoadVersionList()
	{
		try
		{
			HttpClient client = new HttpClient();
			try
			{
				byte[] bytes = await client.GetByteArrayAsync("https://dataedo.com/updates");
				xml = XDocument.Parse(Encoding.UTF8.GetString(bytes).Trim().Replace(Environment.NewLine, string.Empty));
				SetVersionWithAttribute();
				SetLastRepositoryVersion();
				RemoveRepositoryVersion();
				RemoveVersionWithAttribute();
				RemoveVersionsWithReleaseType();
				versionList = Versions.LoadFromXml(xml);
			}
			finally
			{
				((IDisposable)client)?.Dispose();
			}
		}
		catch
		{
			versionList = new Versions();
		}
		SetMajorVersion();
		SetMinorVersion();
	}

	private void SetMajorVersion()
	{
		try
		{
			UpdateVersion updateVersion = UpdateVersion.LoadFromXElement(highestVersionWithAttribute);
			if (string.IsNullOrEmpty(updateVersion.ReleaseType))
			{
				bigUpdateVersion = updateVersion;
				return;
			}
			int maxMajor = versionList.VersionList.Max((UpdateVersion x) => x.Major);
			IEnumerable<UpdateVersion> source = versionList.VersionList.Where((UpdateVersion x) => x.Major == maxMajor);
			int maxMinor = source.Max((UpdateVersion x) => x.Minor);
			IEnumerable<UpdateVersion> source2 = source.Where((UpdateVersion x) => x.Minor == maxMinor);
			int maxBuild = source2.Max((UpdateVersion x) => x.Build);
			IEnumerable<UpdateVersion> source3 = source2.Where((UpdateVersion x) => x.Build == maxBuild);
			bigUpdateVersion = source3.FirstOrDefault();
		}
		catch
		{
			bigUpdateVersion = new UpdateVersion
			{
				Major = 0,
				Minor = 0,
				Build = 0,
				ReleaseType = ""
			};
		}
	}

	private void SetMinorVersion()
	{
		try
		{
			UpdateVersion updateVersion = UpdateVersion.LoadFromXElement(highestVersionWithAttribute);
			if (string.IsNullOrEmpty(updateVersion.ReleaseType) && ProgramVersion.Major == updateVersion.Major)
			{
				smallUpdateVersion = updateVersion;
				return;
			}
			int maxMajor = ProgramVersion.Major;
			IEnumerable<UpdateVersion> source = versionList.VersionList.Where((UpdateVersion x) => x.Major == maxMajor);
			int maxMinor = source.Max((UpdateVersion x) => x.Minor);
			IEnumerable<UpdateVersion> source2 = source.Where((UpdateVersion x) => x.Minor == maxMinor);
			int maxBuild = source2.Max((UpdateVersion x) => x.Build);
			IEnumerable<UpdateVersion> source3 = source2.Where((UpdateVersion x) => x.Build == maxBuild);
			smallUpdateVersion = source3.FirstOrDefault();
		}
		catch
		{
			smallUpdateVersion = new UpdateVersion
			{
				Major = 0,
				Minor = 0,
				Build = 0,
				ReleaseType = ""
			};
		}
	}

	public bool CheckIfRepositoryUpgradeIsNeeded()
	{
		if (highestRepositoryVersion == null)
		{
			return false;
		}
		RepositoryVersion.VersionDefinition matchingRepositoryVersion = new RepositoryVersion(ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build, stable: true).GetMatchingRepositoryVersion(ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build);
		if (matchingRepositoryVersion.Major != highestRepositoryVersion.Major || matchingRepositoryVersion.Minor != highestRepositoryVersion.Minor || matchingRepositoryVersion.Build != highestRepositoryVersion.Build)
		{
			return true;
		}
		return false;
	}

	private void RemoveRepositoryVersion()
	{
		(from x in xml.Descendants("Versions").Descendants("Version")
			where (string)x.Attribute("id") == "repository"
			select x).Remove();
	}

	private void SetLastRepositoryVersion()
	{
		try
		{
			UpdateVersion.LoadFromXElement(highestVersionWithAttribute);
			XElement xElement = xml.Descendants("Versions").Descendants("Version").Single((XElement x) => (string)x.Attribute("id") == "repository");
			if (xElement != null)
			{
				highestRepositoryVersion = UpdateVersion.LoadFromXElement(xElement);
			}
		}
		catch
		{
			highestRepositoryVersion = new UpdateVersion
			{
				Major = 0,
				Minor = 0,
				Build = 0,
				ReleaseType = ""
			};
		}
	}
}
