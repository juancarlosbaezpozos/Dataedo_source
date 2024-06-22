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

internal class Function : IFunction, IModel, ICloneable, ICustomFieldsContainer
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

	public Function(DbRepository repository, ProcedureObject row)
	{
		Function function = this;
		this.repository = repository;
		Id = row.Id;
		DocumentationId = row.DatabaseId;
		Schema = row.Schema;
		Name = row.Name;
		Title = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.Title : null);
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Source = UserTypeEnum.ObjectToType(row.Source).GetValueOrDefault();
		Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Function, PrepareValue.ToString(row.Subtype));
		ImportedAt = PrepareValue.ToDateTime(row.SynchronizationDate);
		Script = (repository.Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Script) ? null : PrepareValue.ToString(row.Definition)?.Trim());
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in function.repository.GetCustomFields()
			where x.ProcedureVisibility
			select new CustomField(function.repository, row, x))).ToList());
		documentation = new Lazy<IDocumentation>(() => function.repository.GetDocumentation(function.DocumentationId));
		parameters = new Lazy<IList<IParameter>>(() => function.repository.GetFunctionParameters(function.Id));
		dependencies = new Lazy<IDependencies>(() => function.repository.GetFunctionDependencies(function.Id));
		modules = new Lazy<IList<IModule>>(() => function.repository.GetFunctionModules(function.Id));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
