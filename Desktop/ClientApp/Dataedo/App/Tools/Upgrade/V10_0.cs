using System;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Commands.Upgrade;
using Dataedo.Data.Factories;

namespace Dataedo.App.Tools.Upgrade;

internal class V10_0
{
	private V10_0Base commands;

	public V10_0(DatabaseType databaseType, string connectionString)
	{
		commands = CommandsFactory.GetUpgradeCommands(databaseType, connectionString).V10_0;
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
