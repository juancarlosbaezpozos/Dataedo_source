using System;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Commands.Upgrade;
using Dataedo.Data.Factories;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Tools.Upgrade;

internal class V9_1
{
	private V9_1Base commands;

	public V9_1(DatabaseType databaseType, string connectionString)
	{
		commands = CommandsFactory.GetUpgradeCommands(databaseType, connectionString).V9_1;
		RichEditControl richEditControl = new RichEditControl();
		commands.Upgrade.UpgradeDescriptionFunction = delegate(string description)
		{
			richEditControl.HtmlText = description;
			return richEditControl.Text;
		};
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
