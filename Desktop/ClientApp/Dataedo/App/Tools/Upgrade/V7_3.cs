using System;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Commands.Upgrade;
using Dataedo.Data.Factories;

namespace Dataedo.App.Tools.Upgrade;

internal class V7_3
{
	private V7_3Base commands;

	public V7_3(DatabaseType databaseType, string connectionString)
	{
		commands = CommandsFactory.GetUpgradeCommands(databaseType, connectionString).V7_3;
	}

	public bool Upgrade(Form owner = null)
	{
		try
		{
			commands.Upgrade.Upgrade();
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to upgrade file.", owner);
			return false;
		}
	}
}