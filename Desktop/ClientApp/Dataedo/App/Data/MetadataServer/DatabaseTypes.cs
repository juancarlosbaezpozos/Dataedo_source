using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

public class DatabaseTypes
{
	public static IList<DBMSGridModel> databaseTypesDataSource;

	public static IList<DBMSGridModel> databaseSubtypesDataSource;

	public static void CreateDatabaseTypesDataSource()
	{
		databaseTypesDataSource = new List<DBMSGridModel>();
		foreach (IDatabaseSupport item3 in DatabaseSupportFactory.GetOrderedDatabaseSupportObjectsForImport())
		{
			bool flag = Connectors.HasDatabaseTypeConnector(item3.SupportedDatabaseType);
			bool flag2 = SharedDatabaseTypeEnum.IsCloudStorage(item3.SupportedDatabaseType);
			Image typeImageForImport = GetTypeImageForImport(item3);
			databaseTypesDataSource.Add(new DBMSGridModel(item3.TypeValue, item3.GetFriendlyDisplayNameForImport(isLite: false), flag ? typeImageForImport : ToolStripRenderer.CreateDisabledImage(typeImageForImport), flag ? string.Empty : ("<href=" + Links.ManageAccounts + ">(upgrade to connect)</href>"), item3.ImportFolders, !flag2, isActive: true, flag, flag2));
		}
		DataLakeTypeObject[] dataLakeTypeObjects = DataLakeTypeEnum.GetDataLakeTypeObjects();
		foreach (DataLakeTypeObject dataLakeTypeObject in dataLakeTypeObjects)
		{
			bool flag3 = Connectors.HasDataLakeTypeConnector(dataLakeTypeObject.Value);
			databaseTypesDataSource.Add(new DBMSGridModel(DataLakeTypeEnum.TypeToString(dataLakeTypeObject.Value), dataLakeTypeObject.DisplayName, flag3 ? dataLakeTypeObject.Image : ToolStripRenderer.CreateDisabledImage(dataLakeTypeObject.Image), dataLakeTypeObject.URL, dataLakeTypeObject.ImportFolders, isDatabase: false, isActive: true, flag3));
		}
		DBMSGridModel item = databaseTypesDataSource.FirstOrDefault((DBMSGridModel x) => x.Type == DatabaseTypeEnum.TypeToString(SharedDatabaseTypeEnum.DatabaseType.Manual));
		databaseTypesDataSource.Remove(item);
		DBMSGridModel item2 = databaseTypesDataSource.FirstOrDefault((DBMSGridModel x) => x.Type == DatabaseTypeEnum.TypeToString(SharedDatabaseTypeEnum.DatabaseType.Odbc));
		databaseTypesDataSource.Remove(item2);
		databaseTypesDataSource = databaseTypesDataSource.OrderBy((DBMSGridModel x) => x.Name).ToList();
		databaseTypesDataSource.Add(item2);
		databaseTypesDataSource.Add(item);
	}

	public static void CreateDatabaseSubtypesDataSource(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		databaseSubtypesDataSource = new List<DBMSGridModel>();
		IEnumerable<IDatabaseSupport> databaseSubtypesSupportObjectsForImport = DatabaseSupportFactory.GetDatabaseSubtypesSupportObjectsForImport(databaseType);
		if (databaseSubtypesSupportObjectsForImport == null || !databaseSubtypesSupportObjectsForImport.Any())
		{
			return;
		}
		foreach (IDatabaseSupport item in databaseSubtypesSupportObjectsForImport)
		{
			bool flag = Connectors.HasDatabaseTypeConnector(item.SupportedDatabaseType);
			bool isActive = SharedDatabaseTypeEnum.IsConnectorActive(item.SupportedDatabaseType);
			Image typeImageForImport = GetTypeImageForImport(item);
			databaseSubtypesDataSource.Add(new DBMSGridModel(item.TypeValue, item.GetFriendlyDisplayNameForImport(isLite: false), flag ? typeImageForImport : ToolStripRenderer.CreateDisabledImage(typeImageForImport), flag ? string.Empty : ("<href=" + Links.ManageAccounts + ">(upgrade to connect)</href>"), item.ImportFolders, isDatabase: true, isActive, flag));
		}
	}

	public static Image GetTypeImageForImport(IDatabaseSupport databaseSupportObject)
	{
		SharedDatabaseTypeEnum.DatabaseType? databaseType = databaseSupportObject?.SupportedDatabaseType;
		if (databaseType.HasValue && databaseType.GetValueOrDefault() == SharedDatabaseTypeEnum.DatabaseType.DdlScript)
		{
			return Resources.ddl_script_16;
		}
		return databaseSupportObject?.TypeImage;
	}
}
