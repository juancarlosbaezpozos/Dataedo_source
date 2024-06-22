using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Structure : IStructure, IModel, ICloneable, ICustomFieldsContainer, IHasLinkedTerms
{
	private DbRepository repository;

	private Lazy<IDocumentation> documentation;

	private Lazy<IList<ITerm>> linkedTerms;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<IColumn>> columns;

	private Lazy<IList<IRelation>> relations;

	private Lazy<IList<IKey>> keys;

	private Lazy<IDependencies> dependencies;

	private Lazy<IList<IModule>> modules;

	public int Id { get; private set; }

	public int DocumentationId { get; private set; }

	public string Schema { get; private set; }

	public string Name { get; private set; }

	public string Title { get; private set; }

	public string Location { get; private set; }

	public string Description { get; private set; }

	public UserTypeEnum.UserType Source { get; private set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; private set; }

	public string Script { get; private set; }

	public DateTime? ImportedAt { get; private set; }

	public IModule Module { get; set; }

	public IDocumentation Documentation => documentation.Value;

	public IList<ITerm> LinkedTerms => linkedTerms.Value;

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<IColumn> Columns => columns.Value;

	public IList<IRelation> Relations => relations.Value;

	public IList<IKey> Keys => keys.Value;

	public IDependencies Dependencies => dependencies.Value;

	public IList<IModule> Modules => modules.Value;

	public Structure(DbRepository repository, TableObject row)
	{
		Structure structure = this;
		this.repository = repository;
		Id = row.Id;
		DocumentationId = row.DatabaseId;
		Schema = row.Schema;
		Name = row.Name;
		Title = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.Title : null);
		Location = row.Location;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Source = UserTypeEnum.ObjectToType(row.Source).GetValueOrDefault();
		Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Structure, row.Subtype);
		ImportedAt = PrepareValue.ToDateTime(row.SynchronizationDate);
		Script = (repository.Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Schema) ? null : row.Definition?.Trim());
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in structure.repository.GetCustomFields()
			where x.TableVisibility
			select new CustomField(structure.repository, row, x))).ToList());
		documentation = new Lazy<IDocumentation>(() => structure.repository.GetDocumentation(structure.DocumentationId));
		linkedTerms = new Lazy<IList<ITerm>>(() => structure.repository.GetStructureLinkedTerms(structure.Id));
		columns = new Lazy<IList<IColumn>>(() => structure.repository.GetStructureColumns(structure.Id));
		relations = new Lazy<IList<IRelation>>(() => structure.repository.GetStructureRelations(structure.Id));
		keys = new Lazy<IList<IKey>>(() => structure.repository.GetStructureKeys(structure.Id));
		dependencies = new Lazy<IDependencies>(() => structure.repository.GetStructureDependencies(structure.Id));
		modules = new Lazy<IList<IModule>>(() => structure.repository.GetStructureModules(structure.Id));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
