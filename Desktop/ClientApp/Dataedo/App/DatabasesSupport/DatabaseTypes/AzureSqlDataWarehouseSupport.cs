using System;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data;
using Dataedo.App.Data.Azure;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

internal class AzureSqlDataWarehouseSupport : SqlServerSupport, IDatabaseSupport, IDatabaseSupportShared
{
	private enum PoolType
	{
		Dedicated = 0,
		Serverless = 1
	}

	public override bool CanCreateImportCommand => true;

	public override bool CanExportExtendedPropertiesOrComments => false;

	public override string ExportToDatabaseButtonDescription => "<color=192, 192, 192>Export extended properties to database (not available for Azure Synapse Analytics (SQL Data Warehouse))</color>";

	public override Image TypeImage => Resources.azure_synapse_16;

	protected override SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse;

	public override SharedDatabaseTypeEnum.DatabaseType? CheckVersion(object connection, DatabaseVersionUpdate version, Form owner = null)
	{
		bool flag = false;
		PoolType? poolType = GetPoolType(connection);
		if (poolType == PoolType.Dedicated && (version < base.VersionInfo.FirstSupportedVersion || version >= base.VersionInfo.FirstNotSupportedVersion))
		{
			flag = true;
		}
		else if (poolType == PoolType.Serverless)
		{
			ConnectorVersionInfo versionInfo = ConnectorsVersion.GetVersionInfo(SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase);
			if (version < versionInfo.FirstSupportedVersion || version >= versionInfo.FirstNotSupportedVersion)
			{
				flag = true;
			}
		}
		if (flag)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(GetNotSupportedText(withQuestion: true), "Unsupported version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return null;
			}
		}
		return SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse;
	}

	protected override string GetSupportedVersionsText()
	{
		return $"from {base.VersionInfo.FirstSupportedVersion} " + $"and before {base.VersionInfo.FirstNotSupportedVersion}";
	}

	public override string GetFriendlyDisplayNameForImport(bool isLite)
	{
		if (!isLite)
		{
			return "Azure Synapse Analytics (SQL Data Warehouse)";
		}
		return "Azure Synapse Analytics (SQL Data Warehouse) (Pro)";
	}

	public override SynchronizeDatabase GetSynchronizeModel(SynchronizeParameters synchronizeParameters)
	{
		PoolType? poolType = GetPoolType(synchronizeParameters.DatabaseRow.Connection);
		if (poolType == PoolType.Dedicated)
		{
			return new SynchronizeAzureSQLDataWarehouse(synchronizeParameters);
		}
		if (poolType == PoolType.Serverless)
		{
			return new SynchronizeAzureSynapseServerless(synchronizeParameters);
		}
		throw new NotSupportedException("Type of Synapse instance is not supported");
	}

	public override DatabaseType GetTypeForCommands(bool isFile)
	{
		return Dataedo.Data.Commands.Enums.DatabaseType.AzureSQLDataWarehouse;
	}

	public override ConnectionBase GetXmlConnectionModel()
	{
		return new AzureSQLDataWarehouseConnection();
	}

	private PoolType? GetPoolType(object connection)
	{
		try
		{
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT @@version;", connection);
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.Read())
			{
				if ((sqlDataReader.GetValue(0)?.ToString()).Contains("Azure SQL Data Warehouse"))
				{
					return PoolType.Dedicated;
				}
				return PoolType.Serverless;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
