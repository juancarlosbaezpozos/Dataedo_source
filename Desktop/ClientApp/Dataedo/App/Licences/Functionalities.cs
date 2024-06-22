using System;
using Dataedo.App.Tools;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;

namespace Dataedo.App.Licences;

internal static class Functionalities
{
	public static bool HasFunctionality(FunctionalityEnum.Functionality functionality)
	{
		if (StaticData.License == null)
		{
			return false;
		}
		return StaticData.License.HasFunctionality(functionality);
	}

	public static bool HasAnyFunctionality(params FunctionalityEnum.Functionality[] functionalities)
	{
		if (StaticData.License == null)
		{
			return false;
		}
		foreach (FunctionalityEnum.Functionality functionality in functionalities)
		{
			if (StaticData.License.HasFunctionality(functionality))
			{
				return true;
			}
		}
		return false;
	}

	public static int GetFunctionalityMaxCount(FunctionalityEnum.Functionality functionality)
	{
		if (StaticData.License == null)
		{
			return 0;
		}
		return StaticData.License.GetFunctionalityMaxCount(functionality);
	}

	public static SuperToolTip GetUnavailableActionToolTip(FunctionalityEnum.Functionality functionality)
	{
		SuperToolTip superToolTip = new SuperToolTip();
		ToolTipTitleItem item = new ToolTipTitleItem
		{
			AllowHtmlText = DefaultBoolean.True,
			Text = "Unavailable"
		};
		superToolTip.Items.Add(item);
		ToolTipItem item2 = new ToolTipItem
		{
			AllowHtmlText = DefaultBoolean.True,
			Text = GetUnavailableActionMessage(functionality, withNewLine: true)
		};
		superToolTip.Items.Add(item2);
		return superToolTip;
	}

	public static string GetUnavailableActionMessage(FunctionalityEnum.Functionality functionality, bool withNewLine = false)
	{
		string text = (withNewLine ? Environment.NewLine : " ");
		return FunctionalityEnum.GetDisplayName(functionality) + " functionality is not available in this license." + text + "Visit <href=" + Links.ManageAccounts + ">Dataedo Account</href> to manage available functionalities.";
	}
}
