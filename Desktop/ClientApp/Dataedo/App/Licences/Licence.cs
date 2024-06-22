using Dataedo.App.API.Enums;
using Dataedo.App.Tools;
using Dataedo.Shared.Licenses.Enums;

namespace Dataedo.App.Licences;

internal static class Licence
{
	public static bool IsTrial => StaticData.License.LicenseTypeValue == LicenseTypeEnum.LicenseType.Trial;

	public static bool HasWatermark => StaticData.License.LicenseTypeValue == LicenseTypeEnum.LicenseType.Trial;

	public static bool IsEducation => StaticData.License.LicenseTypeValue == LicenseTypeEnum.LicenseType.Educational;

	public static int GetCustomFieldsLimit()
	{
		return Functionalities.GetFunctionalityMaxCount(FunctionalityEnum.Functionality.CustomFields);
	}

	public static string GetCustomFieldsLimitInfoBarMessage()
	{
		int customFieldsLimit = GetCustomFieldsLimit();
		return customFieldsLimit switch
		{
			1 => $"Your current plan includes {customFieldsLimit} custom field. " + "To unlock more <href=" + Links.ManageAccounts + ">Upgrade your account</href>.", 
			100 => $"Your current plan includes {customFieldsLimit} custom fields.", 
			_ => $"Your current plan includes {customFieldsLimit} custom fields. " + "To unlock more <href=" + Links.ManageAccounts + ">Upgrade your account</href>.", 
		};
	}
}
