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

internal class Table : ITable, IModel, ICloneable, ICustomFieldsContainer, IHasLinkedTerms
{
	private DbRepository repository;

	private Lazy<IDocumentation> documentation;

	private Lazy<IList<ITerm>> linkedTerms;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<IColumn>> columns;

	private Lazy<IList<IRelation>> relations;

	private Lazy<IList<IKey>> keys;

	private Lazy<IList<ITrigger>> triggers;

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

	public DateTime? ImportedAt { get; private set; }

	public IModule Module { get; set; }

	public IDocumentation Documentation => documentation.Value;

	public IList<ITerm> LinkedTerms => linkedTerms.Value;

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<IColumn> Columns => columns.Value;

	public IList<IRelation> Relations => relations.Value;

	public IList<IKey> Keys => keys.Value;

	public IList<ITrigger> Triggers => triggers.Value;

	public IDependencies Dependencies => dependencies.Value;

	public IList<IModule> Modules => modules.Value;

	public Table(DbRepository repository, TableObject row)
	{
		Table table = this;
		this.repository = repository;
		Id = row.Id;
		DocumentationId = row.DatabaseId;
		Schema = row.Schema;
		Name = row.Name;
		Title = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.Title : null);
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Source = UserTypeEnum.ObjectToType(row.Source).GetValueOrDefault();
		Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Table, row.Subtype);
		ImportedAt = PrepareValue.ToDateTime(row.SynchronizationDate);
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in table.repository.GetCustomFields()
			where x.TableVisibility
			select new CustomField(table.repository, row, x))).ToList());
		documentation = new Lazy<IDocumentation>(() => table.repository.GetDocumentation(table.DocumentationId));
		linkedTerms = new Lazy<IList<ITerm>>(() => table.repository.GetTableLinkedTerms(table.Id));
		columns = new Lazy<IList<IColumn>>(() => table.repository.GetTableColumns(table.Id));
		relations = new Lazy<IList<IRelation>>(() => table.repository.GetTableRelations(table.Id));
		keys = new Lazy<IList<IKey>>(() => table.repository.GetTableKeys(table.Id));
		triggers = new Lazy<IList<ITrigger>>(() => table.repository.GetTableTriggers(table.Id));
		dependencies = new Lazy<IDependencies>(() => table.repository.GetTableDependencies(table.Id));
		modules = new Lazy<IList<IModule>>(() => table.repository.GetTableModules(table.Id));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
