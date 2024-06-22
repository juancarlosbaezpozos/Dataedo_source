using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.PersonalSettings;

namespace Dataedo.App.Data.MetadataServer;

internal class UserPersonalSettingsDB : CommonDBSupport
{
	public UserPersonalSettingsDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public bool Insert(UserPersonalSettings settings, bool insertFilter, Form owner = null)
	{
		try
		{
			if (insertFilter)
			{
				commands.Manipulation.PersonalSettings.InsertUserPersonalSettingsWithFilter(settings);
			}
			else
			{
				commands.Manipulation.PersonalSettings.InsertUserPersonalSettings(settings);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "error while inserting user personal settings", owner);
			return false;
		}
		return true;
	}

	public bool Update(UserPersonalSettings settings, bool updateFilter, Form owner = null)
	{
		try
		{
			if (updateFilter)
			{
				commands.Manipulation.PersonalSettings.UpdateUserPersonalSettingsWithFilter(settings);
			}
			else
			{
				commands.Manipulation.PersonalSettings.UpdateUserPersonalSettings(settings);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "error while updating user personal settings", owner);
			return false;
		}
		return true;
	}

	public PersonalSettingsObject GetPersonalSettings(int licenseId, int databaseId)
	{
		List<PersonalSettingsObject> personalSettings = commands.Select.PersonalSettings.GetPersonalSettings(licenseId, databaseId);
		if (personalSettings.Count == 0)
		{
			return null;
		}
		return personalSettings[0];
	}
}
