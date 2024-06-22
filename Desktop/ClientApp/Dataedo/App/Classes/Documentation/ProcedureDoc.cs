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

public class ProcedureDoc : ProcedureFunctionDoc
{
	public ProcedureDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int procedureOrder, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool useModuleString, Form owner = null)
		: base(database, row, docGeneratingOptions, docHeaders, procedureOrder, databaseType, SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectSubtypeEnum.GetDefaultByMainType(SharedObjectTypeEnum.ObjectType.Procedure), useModuleString, owner)
	{
		base.Parameters = ParameterDoc.GetParameters(docGeneratingOptions, base.Id.Value, owner);
	}

	private static BindingList<ProcedureDoc> LoadDataToList(DatabaseDoc database, List<ObjectDocObject> proceduresDataView, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		int procedureOrder = 1;
		return new BindingList<ProcedureDoc>(new List<ProcedureDoc>(proceduresDataView.Select((ObjectDocObject procedure) => new ProcedureDoc(database, procedure, docGeneratingOptions, docHeaders, procedureOrder++, databaseType, !database.RootDoc.HaveDocumentationsNoModules, owner))));
	}

	public static BindingList<ProcedureDoc> GetProceduresByModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int moduleId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> proceduresByModuleDoc = DB.Procedure.GetProceduresByModuleDoc(moduleId);
			return LoadDataToList(database, proceduresByModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting procedures from the module.", owner);
			return null;
		}
	}

	public static BindingList<ProcedureDoc> GetProceduresWithoutModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> proceduresWithoutModuleDoc = DB.Procedure.GetProceduresWithoutModuleDoc(database.Id.Value);
			return LoadDataToList(database, proceduresWithoutModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting procedures from the database.", owner);
			return null;
		}
	}
}
