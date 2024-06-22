using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Relation : IRelation, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IModel> foreignModel;

	private Lazy<IModel> primaryModel;

	private Lazy<IList<ICustomField>> customFields;

	public int Id { get; private set; }

	public string Name { get; private set; }

	public string Title { get; private set; }

	public string Description { get; private set; }

	public UserTypeEnum.UserType Source { get; private set; }

	public IDocumentation Documentation { get; internal set; }

	public IModel ForeignModel => foreignModel.Value;

	public int ForeignTableId { get; private set; }

	public IModel PrimaryModel => primaryModel.Value;

	public int PrimaryTableId { get; private set; }

	public SharedObjectTypeEnum.ObjectType? PrimaryModelType { get; private set; }

	public SharedObjectTypeEnum.ObjectType? ForeignModelType { get; private set; }

	public CardinalityTypeEnum.CardinalityType? ForeignCardinality { get; private set; }

	public CardinalityTypeEnum.CardinalityType? PrimaryCardinality { get; private set; }

	public IEnumerable<IRelationConstraint> Constraints { get; private set; }

	public IList<ICustomField> CustomFields => customFields.Value;

	private void RowToData(RelationWithColumnAndUniqueConstraint row)
	{
		Id = row.RelationWithUniqueConstraint.TableRelationId;
		Name = row.Name;
		Title = (repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.Title : null);
		Description = (repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Source = UserTypeEnum.ObjectToType(row.Source).GetValueOrDefault();
		ForeignTableId = row.FkTableObject.Id;
		PrimaryTableId = row.PkTableObject.Id;
		PrimaryCardinality = CardinalityTypeEnum.StringToType(row.PkType);
		ForeignCardinality = CardinalityTypeEnum.StringToType(row.FkType);
		PrimaryModelType = SharedObjectTypeEnum.StringToType(row.PkTableObject.ObjectType);
		ForeignModelType = SharedObjectTypeEnum.StringToType(row.FkTableObject.ObjectType);
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in repository.GetCustomFields()
			where x.RelationVisibility
			select new CustomField(repository, row, x))).ToList());
		foreignModel = new Lazy<IModel>(() => ForeignModelType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => repository.GetTable(ForeignTableId), 
			SharedObjectTypeEnum.ObjectType.View => repository.GetView(ForeignTableId), 
			SharedObjectTypeEnum.ObjectType.Structure => repository.GetStructure(ForeignTableId), 
			_ => throw new Exception("Unsupported relationship type."), 
		});
		primaryModel = new Lazy<IModel>(() => PrimaryModelType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => repository.GetTable(PrimaryTableId), 
			SharedObjectTypeEnum.ObjectType.View => repository.GetView(PrimaryTableId), 
			SharedObjectTypeEnum.ObjectType.Structure => repository.GetStructure(PrimaryTableId), 
			_ => throw new Exception("Unsupported relationship type."), 
		});
	}

	public Relation(DbRepository repository, RelationWithColumnAndUniqueConstraint row, IDocumentation documentation)
	{
		this.repository = repository;
		RowToData(row);
		Documentation = documentation;
		Constraints = new List<IRelationConstraint>
		{
			new RelationConstraint(repository, row.RelationWithUniqueConstraint)
		};
	}

	public Relation(DbRepository repository, IGrouping<int, RelationWithColumnAndUniqueConstraint> rows)
	{
		this.repository = repository;
		RelationWithColumnAndUniqueConstraint row = rows.First();
		RowToData(row);
		Constraints = rows.Select((RelationWithColumnAndUniqueConstraint x) => new RelationConstraint(repository, x.RelationWithUniqueConstraint));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
