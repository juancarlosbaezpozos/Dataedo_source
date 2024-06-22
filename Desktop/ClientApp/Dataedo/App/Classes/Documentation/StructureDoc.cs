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

public class StructureDoc : TableViewDoc
{
	public new BindingList<TriggerDoc> Triggers { get; set; }

	public List<DefinitionDoc> Definition { get; protected set; }

	public StructureDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int tableOrder, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool useModuleString, Form owner = null)
		: base(database, row, docGeneratingOptions, docHeaders, tableOrder, databaseType, SharedObjectTypeEnum.ObjectType.Structure, SharedObjectSubtypeEnum.GetDefaultByMainType(SharedObjectTypeEnum.ObjectType.Structure), useModuleString, owner)
	{
		base.Id = row.Id;
		int value = base.Id.Value;
		base.Columns = ColumnDoc.GetColumns(docGeneratingOptions, database, this, (docGeneratingOptions.Template as PdfTemplateModel)?.Customization?.Localization?.Data, owner);
		Triggers = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Trigger).Any() ? new BindingList<TriggerDoc>() : TriggerDoc.GetTriggers(docGeneratingOptions, database, this, owner));
		base.UniqueConstraints = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key).Any() ? new BindingList<UniqueContraintDoc>() : UniqueContraintDoc.GetUniqueContraints(docGeneratingOptions, value, owner));
		if (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation).Any())
		{
			base.PKRelations = new BindingList<RelationsDoc>();
			base.FKRelations = new BindingList<RelationsDoc>();
		}
		else
		{
			base.PKRelations = RelationsDoc.GetRelations(docGeneratingOptions, database, value, getPKRelations: true, owner);
			base.FKRelations = RelationsDoc.GetRelations(docGeneratingOptions, database, value, getPKRelations: false, owner);
		}
		if (!docGeneratingOptions.ExcludedObjects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Script)))
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

	private static BindingList<StructureDoc> LoadDataToList(DatabaseDoc database, IEnumerable<ObjectDocObject> structuresDataView, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		int viewOrder = 1;
		return new BindingList<StructureDoc>(new List<StructureDoc>(structuresDataView.Select((ObjectDocObject view) => new StructureDoc(database, view, docGeneratingOptions, docHeaders, viewOrder++, databaseType, !database.RootDoc.HaveDocumentationsNoModules, owner))));
	}

	public static BindingList<StructureDoc> GetStructuresByModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int moduleId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> structuresByModuleDoc = DB.Table.GetStructuresByModuleDoc(moduleId);
			return LoadDataToList(database, structuresByModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting structures from the module.", owner);
			return null;
		}
	}

	public static BindingList<StructureDoc> GetStructuresWithoutModule(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, int databaseId, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			List<ObjectDocObject> structuresWithoutModuleDoc = DB.Table.GetStructuresWithoutModuleDoc(databaseId);
			return LoadDataToList(database, structuresWithoutModuleDoc, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting structures from the database.", owner);
			return null;
		}
	}
}
