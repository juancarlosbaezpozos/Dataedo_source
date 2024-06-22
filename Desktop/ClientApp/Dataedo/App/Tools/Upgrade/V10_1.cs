using System;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Commands.Upgrade;
using Dataedo.Data.Factories;

namespace Dataedo.App.Tools.Upgrade;

internal class V10_1
{
	private V10_1Base commands;

	public V10_1(DatabaseType databaseType, string connectionString)
	{
		commands = CommandsFactory.GetUpgradeCommands(databaseType, connectionString).V10_1;
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
