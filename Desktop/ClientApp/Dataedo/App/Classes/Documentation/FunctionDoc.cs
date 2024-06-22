using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class FunctionDoc : ProcedureFunctionDoc
{
	public FunctionDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int procedureOrder, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool useModuleString, Form owner = null)
		: base(database, row, docGeneratingOptions, docHeaders, procedureOrder, databaseType, SharedObjectTypeEnum.ObjectType.Function, SharedObjectSubtypeEnum.GetDefaultByMainType(SharedObjectTypeEnum.ObjectType.Function), useModuleString, owner)
	{
		base.Parameters = ParameterDoc.GetParameters(docGeneratingOptions, row.Id, owner);
	}

	private static BindingList<FunctionDoc> LoadDataToList(DatabaseDoc database, List<ObjectDocObject> functionsDataView, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		int procedureOrder = 1;
		return new BindingList<FunctionDoc>(new List<FunctionDoc>(functionsDataView.Select((ObjectDocObject function) => new FunctionDoc(database, function, docGeneratingOptions, docHeaders, procedureOrder++, databaseType, !database.RootDoc.HaveDocumentationsNoModules, owner))));
	}

	public static BindingList<FunctionDoc> GetFunctionByModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int moduleId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> functionsByModuleDoc = DB.Procedure.GetFunctionsByModuleDoc(moduleId);
			return LoadDataToList(database, functionsByModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting functions from the module.", owner);
			return null;
		}
	}

	public static BindingList<FunctionDoc> GetFunctionWithoutModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int databaseId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> functionsWithoutModuleDoc = DB.Procedure.GetFunctionsWithoutModuleDoc(databaseId);
			return LoadDataToList(database, functionsWithoutModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting functions from the database.", owner);
			return null;
		}
	}
}
