using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Documentations;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Documentation : IDocumentation, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<IModule>> modules;

	private Lazy<IList<ITable>> tables;

	private Lazy<IList<IView>> views;

	private Lazy<IList<IStructure>> structures;

	private Lazy<IList<IProcedure>> procedures;

	private Lazy<IList<IFunction>> functions;

	public int Id { get; private set; }

	public string Name { get; internal set; }

	public string Title { get; private set; }

	public string Description { get; private set; }

	public string Host { get; internal set; }

	public string Dbms { get; private set; }

	public SharedDatabaseTypeEnum.DatabaseType? Type { get; private set; }

	public bool HasMultipleSchemas { get; private set; }

	public bool? ShowSchema { get; private set; }

	public bool? ShowSchemaOverride { get; private set; }

	public bool ShowSchemaEffective => GetShowSchema(ShowSchema, ShowSchemaOverride);

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<IModule> Modules => modules.Value;

	public IList<ITable> Tables => tables.Value;

	public IList<IView> Views => views.Value;

	public IList<IStructure> Structures => structures.Value;

	public IList<IProcedure> Procedures => procedures.Value;

	public IList<IFunction> Functions => functions.Value;

	public Documentation(DbRepository repository, DocumentationObject row)
	{
		Documentation documentation = this;
		this.repository = repository;
		Id = PrepareValue.ToInt(row.DatabaseId).Value;
		Name = PrepareValue.ToString(row.Name);
		Title = PrepareValue.ToString(row.Title);
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Host = PrepareValue.ToString(row.Host);
		Dbms = DatabaseTypeEnum.TypeToStringForDisplay(DatabaseTypeEnum.StringToType(PrepareValue.ToString(row.Type)));
		Type = DatabaseTypeEnum.StringToType(PrepareValue.ToString(row.Type));
		HasMultipleSchemas = PrepareValue.ToBool(row.MultipleSchemas);
		ShowSchema = row.ShowSchema;
		ShowSchemaOverride = row.ShowSchemaOverride;
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in documentation.repository.GetCustomFields()
			where x.DocumentationVisibility
			select new CustomField(documentation.repository, row, x))).ToList());
		modules = new Lazy<IList<IModule>>(() => documentation.repository.GetModules(documentation.Id));
		tables = new Lazy<IList<ITable>>(() => documentation.repository.GetTables(documentation.Id));
		views = new Lazy<IList<IView>>(() => documentation.repository.GetViews(documentation.Id));
		structures = new Lazy<IList<IStructure>>(() => documentation.repository.GetStructures(documentation.Id));
		procedures = new Lazy<IList<IProcedure>>(() => documentation.repository.GetProcedures(documentation.Id));
		functions = new Lazy<IList<IFunction>>(() => documentation.repository.GetFunctions(documentation.Id));
	}

	public static bool GetShowSchema(bool? databaseShowSchema, bool? databaseShowSchemaOverride)
	{
		if (databaseShowSchemaOverride != true)
		{
			if (!databaseShowSchemaOverride.HasValue)
			{
				return databaseShowSchema == true;
			}
			return false;
		}
		return true;
	}

	public static bool GetShowSchema(bool? databaseShowSchema, bool? databaseShowSchemaOverride, bool contextShowSchema)
	{
		return databaseShowSchemaOverride == true || (!databaseShowSchemaOverride.HasValue && databaseShowSchema == true) || contextShowSchema;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
