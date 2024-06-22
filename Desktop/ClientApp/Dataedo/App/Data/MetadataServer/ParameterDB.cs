using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Enums;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class ParameterDB : CommonDBSupport
{
	public ParameterDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public BindingList<ParameterRow> GetDataObjectByProcedureId(int procedureId, bool notDeletedOnly = false, CustomFieldsSupport customFieldsSupport = null)
	{
		return new BindingList<ParameterRow>((from parameter in GetDataByProcedureId(procedureId, notDeletedOnly)
			select new ParameterRow(procedureId, parameter, customFieldsSupport)).ToList());
	}

	public List<ParameterObject> GetDataByProcedureId(int procedureId, bool notDeletedOnly = false)
	{
		return commands.Select.Parameters.GetParameters(procedureId, GetNotStatusValue(notDeletedOnly));
	}

	public List<ParameterObject> GetDataByProcedureDoc(int procedureId)
	{
		return commands.Select.Parameters.GetParametersDoc(procedureId);
	}

	public int Synchronize(IEnumerable<ParameterRow> parameters, string procedureName, int procedureId, string schema, int databaseId, bool isDbAdded, int updateId, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		try
		{
			HistoryImportChangesHelper.CheckParameterChangesInParameterDB(parameters, databaseId, procedureId, isDbAdded, DB.History.SavingEnabled, customFieldsSupport);
			int result = commands.Synchronization.Parameters.SynchronizeParameters(parameters.Select((ParameterRow r) => ConvertRowToSynchronizationItem(r, procedureName, schema, databaseId, updateId)).ToArray(), procedureName, schema, databaseId, isDbAdded, updateId);
			HistoryImportChangesHelper.InsertHistoryRowsInParameterDB(parameters, databaseId, procedureId, isDbAdded);
			return result;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the parameter:", owner);
			return -1;
		}
	}

	private ParameterForSynchronization ConvertRowToSynchronizationItem(ParameterRow row, string procedureName, string schema, int databaseId, int updateId)
	{
		ParameterForSynchronization parameterForSynchronization = new ParameterForSynchronization
		{
			BaseName = procedureName,
			BaseSchema = schema,
			BaseId = databaseId,
			Name = row.Name,
			Position = row.Position,
			DataType = row.DataType,
			Description = row.Description,
			DataLength = row.DataLength,
			ParameterMode = row.ParameterMode,
			UpdateId = updateId
		};
		SetCustomFields(parameterForSynchronization, row);
		return parameterForSynchronization;
	}

	public bool Update(ParameterRow parameterRow, Form owner = null)
	{
		try
		{
			Parameter item = ConvertRowToTable(parameterRow);
			commands.Manipulation.Parameters.UpdateParameterWithCustomFields(item);
			parameterRow.CustomFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the parameter:", owner);
			return false;
		}
		return true;
	}

	private Parameter ConvertRowToTable(ParameterRow row)
	{
		Parameter parameter = new Parameter
		{
			ParameterId = row.Id,
			Description = row.Description,
			Name = row.Name,
			Source = UserTypeEnum.TypeToString(row.Source),
			DataType = row.DataType,
			DataLength = row.DataLength,
			OrdinalPosition = row.Position,
			ParameterMode = row.ParameterMode,
			Status = SynchronizeStateEnum.StateToDBString(row.Status)
		};
		SetCustomFields(parameter, row);
		return parameter;
	}

	public bool Delete(IEnumerable<int> ids, Form owner = null)
	{
		try
		{
			commands.Manipulation.Parameters.DeleteParameters(ids.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the parameter:", owner);
			return false;
		}
		return true;
	}

	public int? InsertManualParameter(ParameterRow parameterRow, int procedureId, bool isProjectFile, Form owner = null)
	{
		try
		{
			Parameter parameter = ConvertRowToTable(parameterRow);
			parameter.ProcedureId = procedureId;
			return commands.Manipulation.Parameters.InsertManualParameter(parameter, isProjectFile);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting manual parameter:", owner);
			return null;
		}
	}

	public void UpdateManualParameter(ParameterRow parameterRow, Form owner = null)
	{
		try
		{
			Parameter item = ConvertRowToTable(parameterRow);
			commands.Manipulation.Parameters.UpdateManualParameter(item);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating manual parameter:", owner);
		}
	}
}
