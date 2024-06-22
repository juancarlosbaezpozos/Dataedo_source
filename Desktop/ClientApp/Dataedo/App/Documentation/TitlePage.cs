using System;
using System.Drawing;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Properties;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Documentations;

namespace Dataedo.App.Documentation;

public class TitlePage
{
	public string DatabaseTitle { get; set; }

	public string DatabaseTitleNavigationUrl { get; set; }

	public string CompanyName { get; set; }

	public string CompanyNameNavigationUrl { get; set; }

	public string Subtitle { get; set; } = "Data Dictionary";


	public string SubtitleNavigationUrl { get; set; }

	public string UserName { get; set; }

	public DateTime TodayDate { get; set; }

	public string TodayDateString => TodayDate.ToShortDateString();

	public bool? ShowDate { get; set; }

	public bool HideDescription { get; set; }

	public Image DataedoLogo { get; set; }

	public bool ShowLogo { get; set; }

	public Image Logo { get; set; }

	public string LogoNavigationUrl { get; set; }

	public TitlePage(int databaseId)
	{
		CompanyName = StaticData.License.AccountName;
		DocumentationObject dataById = DB.Database.GetDataById(databaseId);
		DatabaseTitle = PrepareValue.ToString(dataById.Title);
		UserName = string.Empty;
		TodayDate = DateTime.Today;
		DataedoLogo = Resources.doc_logo;
	}
}
