using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.MessageBoxes;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public abstract class DBDescriptionExporter<Connection> where Connection : IDbConnection, new()
{
    protected string types;

    protected string connectionString;

    protected int databaseId;

    protected BackgroundProcessingWorker exportWorker;

    protected List<int> modulesId;

    public List<DBDescription> DescriptionObjects { get; protected set; }

    public string Types => types;

    public abstract string DescriptionLabel { get; }

    public DBDescriptionExporter(BackgroundProcessingWorker exportWorker, string connectionString, int databaseId, List<int> modulesId)
    {
        this.exportWorker = exportWorker;
        this.connectionString = connectionString;
        this.databaseId = databaseId;
        this.modulesId = modulesId;
    }

    public void GetTypeNames(List<ObjectTypeHierarchy> types)
    {
        this.types = string.Join(",", types.Select((ObjectTypeHierarchy x) => (x.ObjectType != x.ObjectSubtype) ? $"'{x.ObjectType}_{x.ObjectSubtype}'" : $"'{x.ObjectType}'")).ToLower();
    }

    public abstract void InitializeExportObjects(bool exportDescriptions = true);

    protected abstract List<DBDescription> GetDescriptionObjects();

    protected abstract void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description);

    public virtual void ExportDescription(bool exportDescriptions = true, Form owner = null)
    {
        try
        {
            exportWorker.ReportProgress("Preparing...", 0);
            exportWorker.SetTotalProgressStep(100f);
            if (string.IsNullOrEmpty(types))
            {
                types = "''";
            }
            exportWorker.DivideProgressStep(DescriptionObjects.Count);
            List<DBDescription> list = new List<DBDescription>();
            if (DescriptionObjects.Count != 0)
            {
                using IDbConnection dbConnection = new Connection();
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();
                int num = 1;
                foreach (DBDescription descriptionObject in DescriptionObjects)
                {
                    exportWorker.ReportProgress($"Processing {num++} of {DescriptionObjects.Count}");
                    IDbCommand command = dbConnection.CreateCommand();
                    try
                    {
                        command.CommandText = descriptionObject.Command;
                        HandleExceptions(delegate
                        {
                            command.ExecuteNonQuery();
                        }, list, descriptionObject);
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }
                    }
                    exportWorker.IncreaseProgress();
                }
                dbConnection.Close();
            }
            if (list.Count > 0)
            {
                string empty = string.Empty;
                string text = "descriptions were not exported because objects are not available or don't support " + DescriptionLabel;
                empty = ((list.Count > 50) ? $"{list.Count} {text}." : ("Following " + text + ":" + Environment.NewLine + Environment.NewLine + string.Join(", ", list.Select((DBDescription x) => x.Name)) + "."));
                if (!string.IsNullOrWhiteSpace(empty))
                {
                    GeneralMessageBoxesHandling.Show(empty, "Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex2)
        {
            exportWorker.HasError = true;
            if (ex2 is SqlException && (ex2 as SqlException).Number == 3906)
            {
                GeneralMessageBoxesHandling.Show("Database is in read only mode." + Environment.NewLine + "Extended property / Comment export requires database to be writeable.", "Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
            }
            else
            {
                GeneralExceptionHandling.Handle(ex2, "Error while exporting descriptions.", owner);
            }
        }
        exportWorker.HasResult = true;
    }
}
