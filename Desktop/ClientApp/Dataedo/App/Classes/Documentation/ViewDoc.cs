using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Documentation.Common;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class ViewDoc : TableViewDoc
{
	public new BindingList<TriggerDoc> Triggers { get; set; }

	public List<DefinitionDoc> Definition { get; protected set; }

	public ViewDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int tableOrder, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool useModuleString, Form owner = null)
		: base(database, row, docGeneratingOptions, docHeaders, tableOrder, databaseType, SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.GetDefaultByMainType(SharedObjectTypeEnum.ObjectType.View), useModuleString, owner)
	{
		base.Id = row.Id;
		int value = base.Id.Value;
		base.Columns = ColumnDoc.GetColumns(docGeneratingOptions, database, this, (docGeneratingOptions.Template as PdfTemplateModel)?.Customization?.Localization?.Data, owner);
		Triggers = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.View && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Trigger).Any() ? new BindingList<TriggerDoc>() : TriggerDoc.GetTriggers(docGeneratingOptions, database, this, owner));
		base.UniqueConstraints = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.View && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key).Any() ? new BindingList<UniqueContraintDoc>() : UniqueContraintDoc.GetUniqueContraints(docGeneratingOptions, value, owner));
		if (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.View && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation).Any())
		{
			base.PKRelations = new BindingList<RelationsDoc>();
			base.FKRelations = new BindingList<RelationsDoc>();
		}
		else
		{
			base.PKRelations = RelationsDoc.GetRelations(docGeneratingOptions, database, value, getPKRelations: true, owner);
			base.FKRelations = RelationsDoc.GetRelations(docGeneratingOptions, database, value, getPKRelations: false, owner);
		}
		if (!docGeneratingOptions.ExcludedObjects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Script)))
		{
			string definition = row.Definition;
			if (!string.IsNullOrEmpty(definition))
			{
				Definition = new List<DefinitionDoc>
				{
					new DefinitionDoc(definition, row.Language, docGeneratingOptions)
				};
			}
		}
	}

	private static BindingList<ViewDoc> LoadDataToList(DatabaseDoc database, IEnumerable<ObjectDocObject> viewsDataView, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		int viewOrder = 1;
		return new BindingList<ViewDoc>(new List<ViewDoc>(viewsDataView.Select((ObjectDocObject view) => new ViewDoc(database, view, docGeneratingOptions, docHeaders, viewOrder++, databaseType, !database.RootDoc.HaveDocumentationsNoModules, owner))));
	}

	public static BindingList<ViewDoc> GetViewsByModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int moduleId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> viewsByModuleDoc = DB.Table.GetViewsByModuleDoc(moduleId);
			return LoadDataToList(database, viewsByModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting views from the module.", owner);
			return null;
		}
	}

	public static BindingList<ViewDoc> GetViewsWithoutModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int databaseId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> viewsWithoutModuleDoc = DB.Table.GetViewsWithoutModuleDoc(databaseId);
			return LoadDataToList(database, viewsWithoutModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting views from the database.", owner);
			return null;
		}
	}
}
