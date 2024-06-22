using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Dataedo.App.Tools.Exceptions;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport;

public static class ConnectorsVersion
{
    private const string fileName = "connectorsVersions.xml";

    private static Dictionary<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo> connectorsVersionInfo = new Dictionary<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo>
    {
        {
            SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Athena,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Aurora,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.AuroraPostgreSQL,
            new ConnectorVersionInfo("9.3.0", "13.7.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase,
            new ConnectorVersionInfo("12.0.0", "13.0.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse,
            new ConnectorVersionInfo("10.0.0", "11.0.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Cassandra,
            new ConnectorVersionInfo("3.9.0", "4.1.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDB,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB,
            new ConnectorVersionInfo("3.6.0", "4.5.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Db2Cloud,
            new ConnectorVersionInfo("9.0.0", "11.2.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Db2LUW,
            new ConnectorVersionInfo("9.0.0", "11.2.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.DdlScript,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Elasticsearch,
            new ConnectorVersionInfo("7.0.0", "8.0.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.ApacheHiveMetastore,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.ApacheImpalaMetastore,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.ApacheSparkMetastore,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.DatabricksMetastore,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastore,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.IBMDb2BigQuery,
            new ConnectorVersionInfo("9.0.0", "11.2.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Manual,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.MariaDB,
            new ConnectorVersionInfo("5.1.0", "10.8.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.MongoDB,
            new ConnectorVersionInfo("3.6.0", "5.3.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.MySQL,
            new ConnectorVersionInfo("5.1.0", "8.1.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.MySQL8,
            new ConnectorVersionInfo("5.1.0", "8.1.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Neo4j,
            new ConnectorVersionInfo("3.5.0", "4.3.3")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Odbc,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Oracle,
            new ConnectorVersionInfo("9.0.0", "20.0.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL,
            new ConnectorVersionInfo("5.6.0", "8.0.28")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL8,
            new ConnectorVersionInfo("5.6.0", "8.0.14")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.PostgreSQL,
            new ConnectorVersionInfo("9.0.0", "15.0.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Redshift,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Salesforce,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.SapAse,
            new ConnectorVersionInfo("16.0.0", "16.0.1")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Snowflake,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.SqlServer,
            new ConnectorVersionInfo("9.0.3042", "16.0.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.SsasTabular,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Tableau,
            new ConnectorVersionInfo("10.1.0")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Teradata,
            new ConnectorVersionInfo("16.10.0", "17.10.1")
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Vertica,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.AmazonS3,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.InterfaceTables,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Dataverse,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Dynamics365,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.DynamoDB,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.SapHana,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.Astra,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.NetSuite,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.DBT,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.SSIS,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage,
            new ConnectorVersionInfo()
        },
        {
            SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage,
            new ConnectorVersionInfo()
        }
    };

    public static Dictionary<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo> ConnectorsVersionInfo => connectorsVersionInfo;

    public static ConnectorVersionInfo GetVersionInfo(SharedDatabaseTypeEnum.DatabaseType databaseType)
    {
        return ConnectorsVersionInfo[databaseType];
    }

    private static string GetFilePathForSaving()
    {
        string confFolderPath = ConfigurationFileHelper.GetConfFolderPath();
        if (string.IsNullOrWhiteSpace(confFolderPath))
        {
            return string.Empty;
        }
        return Path.Combine(confFolderPath, "connectorsVersions.xml");
    }

    public static void SaveToXML()
    {
        SaveToXML(GetFilePathForSaving());
    }

    public static void SaveToXML(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        using StreamWriter textWriter = new StreamWriter(filePath, append: false, Encoding.UTF8);
        try
        {
            new XmlSerializer(typeof(SerializableConnectorVersion[])).Serialize(textWriter, ConnectorsVersionInfo.Select((KeyValuePair<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo> x) => new SerializableConnectorVersion(x)).ToArray());
        }
        catch (Exception exception)
        {
            GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog);
        }
    }

    private static string GetFilePathForLoading()
    {
        string confFolderPath = ConfigurationFileHelper.GetConfFolderPath(Assembly.GetExecutingAssembly().GetName().Version.Major);
        if (string.IsNullOrWhiteSpace(confFolderPath) || !Directory.Exists(confFolderPath))
        {
            return string.Empty;
        }
        string text = Path.Combine(confFolderPath, "connectorsVersions.xml");
        if (!File.Exists(text))
        {
            return string.Empty;
        }
        return text;
    }

    public static void LoadFromXML()
    {
        try
        {
            string filePathForLoading = GetFilePathForLoading();
            if (!string.IsNullOrWhiteSpace(filePathForLoading) && File.Exists(filePathForLoading))
            {
                Dictionary<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo> dictionary = LoadFromXML(filePathForLoading);
                if (dictionary == null)
                {
                    throw new Exception("The dictionary was not loaded from the file " + filePathForLoading);
                }
                string message = string.Empty;
                if (!VerifyDictionary(dictionary, out message))
                {
                    throw new Exception("The loaded dictionary was not successfully verified:" + Environment.NewLine + message);
                }
                connectorsVersionInfo = dictionary;
            }
        }
        catch (Exception exception)
        {
            GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog);
        }
    }

    public static Dictionary<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo> LoadFromXML(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return null;
        }
        try
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SerializableConnectorVersion[]));
            using FileStream input = new FileStream(filePath, FileMode.Open);
            XmlReader xmlReader = XmlReader.Create(input);
            if (!xmlSerializer.CanDeserialize(xmlReader))
            {
                throw new Exception("The file " + filePath + " could not be deserialized");
            }
            return ((SerializableConnectorVersion[])xmlSerializer.Deserialize(xmlReader)).ToDictionary((SerializableConnectorVersion x) => (SharedDatabaseTypeEnum.DatabaseType)Enum.Parse(typeof(SharedDatabaseTypeEnum.DatabaseType), x.databaseType), (SerializableConnectorVersion x) => x.versionInfo);
        }
        catch (Exception exception)
        {
            GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog);
        }
        return null;
    }

    public static bool VerifyDictionary(Dictionary<SharedDatabaseTypeEnum.DatabaseType, ConnectorVersionInfo> dictionary, out string message)
    {
        StringBuilder stringBuilder = new StringBuilder();
        bool result = true;
        foreach (SharedDatabaseTypeEnum.DatabaseType item in Enum.GetValues(typeof(SharedDatabaseTypeEnum.DatabaseType)).Cast<SharedDatabaseTypeEnum.DatabaseType>())
        {
            if (!dictionary.ContainsKey(item))
            {
                stringBuilder.AppendLine($"Database type '{item}' is not present in the dictionary.");
                result = false;
                continue;
            }
            ConnectorVersionInfo connectorVersionInfo = dictionary[item];
            if (connectorVersionInfo.FirstSupportedVersion != null && connectorVersionInfo.FirstNotSupportedVersion != null && connectorVersionInfo.FirstSupportedVersion > connectorVersionInfo.FirstNotSupportedVersion)
            {
                stringBuilder.AppendLine("First supported version is higher" + $" than first not supported version for '{item}'.");
                result = false;
            }
        }
        message = stringBuilder.ToString();
        return result;
    }
}
