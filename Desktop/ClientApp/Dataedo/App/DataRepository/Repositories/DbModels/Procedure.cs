using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Procedures.Procedures;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Procedure : IProcedure, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IDocumentation> documentation;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<IParameter>> parameters;

	private Lazy<IDependencies> dependencies;

	private Lazy<IList<IModule>> modules;

	public int Id { get; private set; }

	public int DocumentationId { get; private set; }

	public string Schema { get; private set; }

	public string Name { get; private set; }

	public string Title { get; private set; }

	public string Description { get; private set; }

	public UserTypeEnum.UserType Source { get; private set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; private set; }

	public string Script { get; private set; }

	public DateTime? ImportedAt { get; private set; }

	public IModule Module { get; set; }

	public IDocumentation Documentation => documentation.Value;

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<IParameter> Parameters => parameters.Value;

	public IDependencies Dependencies => dependencies.Value;

	public IList<IModule> Modules => modules.Value;

	public Procedure(DbRepository repository, ProcedureObject row)
	{
		Procedure procedure = this;
		this.repository = repository;
		Id = row.Id;
		DocumentationId = row.DatabaseId;
		Schema = row.Schema;
		Name = row.Name;
		Title = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.Title : null);
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Source = UserTypeEnum.ObjectToType(row.Source).GetValueOrDefault();
		Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Procedure, PrepareValue.ToString(row.Subtype));
		ImportedAt = PrepareValue.ToDateTime(row.SynchronizationDate);
		Script = (repository.Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Script) ? null : PrepareValue.ToString(row.Definition)?.Trim());
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in procedure.repository.GetCustomFields()
			where x.ProcedureVisibility
			select new CustomField(procedure.repository, row, x))).ToList());
		documentation = new Lazy<IDocumentation>(() => procedure.repository.GetDocumentation(procedure.DocumentationId));
		parameters = new Lazy<IList<IParameter>>(() => procedure.repository.GetProcedureParameters(procedure.Id));
		dependencies = new Lazy<IDependencies>(() => procedure.repository.GetProcedureDependencies(procedure.Id));
		modules = new Lazy<IList<IModule>>(() => procedure.repository.GetProcedureModules(procedure.Id));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
