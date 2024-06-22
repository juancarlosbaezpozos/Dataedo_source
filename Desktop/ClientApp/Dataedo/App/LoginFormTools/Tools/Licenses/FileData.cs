using System;
using System.Collections.Generic;
using Dataedo.App.API.Enums;
using Dataedo.LicenseFile;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public class FileData
{
	public string Type { get; set; }

	public int AccountId { get; set; }

	public string AccountName { get; set; }

	public string Username { get; set; }

	public string University { get; set; }

	public long? DocumentDate { get; set; }

	public int DocumentId { get; set; }

	public long CreatedOn { get; set; }

	public long? Start { get; set; }

	public long? End { get; set; }

	public LicenseTypeEnum.LicenseType TypeEnum => LicenseTypeEnum.GetValue(Type);

	public FileLicenseData LastSelectedLicense { get; set; }

	public IList<FileLicenseData> Licenses { get; set; }

	public LicenseFileModel LicenseFile { get; set; }

	public DateTime? GetEndUtcDateTime => GetUtcDateTime(End);

	private DateTime? GetStartUtcDateTime => GetUtcDateTime(Start);

	public FileData()
	{
		Licenses = new List<FileLicenseData>();
	}

	public void Map(LicenseFileModel licenseFileModel, FileLicenseModel lastSelectedLicense = null)
	{
		Type = licenseFileModel.Type;
		AccountId = licenseFileModel.AccountId;
		AccountName = licenseFileModel.AccountName;
		Username = licenseFileModel.Username;
		University = licenseFileModel.University;
		DocumentDate = licenseFileModel.DocumentDate;
		DocumentId = licenseFileModel.DocumentId;
		Start = licenseFileModel.Start;
		End = licenseFileModel.End;
		CreatedOn = licenseFileModel.CreatedOn;
		foreach (FileLicenseModel license in licenseFileModel.Licenses)
		{
			FileLicenseData item = new FileLicenseData(license, GetEndUtcDateTime, AccountName, AccountId, TypeEnum, University);
			Licenses.Add(item);
		}
		if (lastSelectedLicense != null)
		{
			LastSelectedLicense = new FileLicenseData(lastSelectedLicense, GetEndUtcDateTime, AccountName, AccountId, TypeEnum, University);
		}
		LicenseFile = licenseFileModel;
	}

	private static DateTime? GetUtcDateTime(long? timestamp)
	{
		if (!timestamp.HasValue)
		{
			return null;
		}
		return DateTimeOffset.FromUnixTimeSeconds(timestamp.Value).UtcDateTime;
	}
}
