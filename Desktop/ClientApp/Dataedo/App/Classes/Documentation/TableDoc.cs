using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class TableDoc : TableViewDoc
{
	public TableDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int tableOrder, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool useModuleString, Form owner = null)
		: base(database, row, docGeneratingOptions, docHeaders, tableOrder, databaseType, SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.GetDefaultByMainType(SharedObjectTypeEnum.ObjectType.Table), useModuleString, owner)
	{
		int value = base.Id.Value;
		base.Columns = ColumnDoc.GetColumns(docGeneratingOptions, database, this, (docGeneratingOptions.Template as PdfTemplateModel)?.Customization?.Localization?.Data, owner);
		base.Triggers = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Trigger).Any() ? new BindingList<TriggerDoc>() : TriggerDoc.GetTriggers(docGeneratingOptions, database, this, owner));
		base.UniqueConstraints = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key).Any() ? new BindingList<UniqueContraintDoc>() : UniqueContraintDoc.GetUniqueContraints(docGeneratingOptions, value, owner));
		if (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation).Any())
		{
			base.PKRelations = new BindingList<RelationsDoc>();
			base.FKRelations = new BindingList<RelationsDoc>();
		}
		else
		{
			base.PKRelations = RelationsDoc.GetRelations(docGeneratingOptions, database, value, getPKRelations: true, owner);
			base.FKRelations = RelationsDoc.GetRelations(docGeneratingOptions, database, value, getPKRelations: false, owner);
		}
	}

	private static BindingList<TableDoc> LoadDataToList(DatabaseDoc database, IEnumerable<ObjectDocObject> tablesDataView, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		int tableOrder = 1;
		return new BindingList<TableDoc>(new List<TableDoc>(tablesDataView.Select((ObjectDocObject x) => new TableDoc(database, x, docGeneratingOptions, docHeaders, tableOrder++, databaseType, !database.RootDoc.HaveDocumentationsNoModules, owner))));
	}

	public static BindingList<TableDoc> GetTablesByModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int moduleId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> tablesByModuleDoc = DB.Table.GetTablesByModuleDoc(moduleId);
			return LoadDataToList(database, tablesByModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting tables from the module.", owner);
			return null;
		}
	}

	public static BindingList<TableDoc> GetTablesWithoutModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> tablesWithoutModuleDoc = DB.Table.GetTablesWithoutModuleDoc(database.Id.Value);
			return LoadDataToList(database, tablesWithoutModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting tables from the database.", owner);
			return null;
		}
	}
}
