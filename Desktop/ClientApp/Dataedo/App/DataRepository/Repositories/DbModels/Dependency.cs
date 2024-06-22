using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Export;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Dependency : IDependency, IModel, ICloneable
{
	private DbRepository repository;

	public int Id { get; private set; }

	public string ReferenceType { get; private set; }

	public int? ReferenceId { get; private set; }

	public string ObjectName { get; private set; }

	public string ObjectFullDisplayName { get; private set; }

	public string ObjectFullDisplayNameShowSchema { get; private set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; private set; }

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype { get; private set; }

	public UserTypeEnum.UserType ObjectSource { get; private set; }

	public string Type { get; private set; }

	public UserTypeEnum.UserType Source { get; private set; }

	public CardinalityTypeEnum.CardinalityType? RelationFkCardinality { get; private set; }

	public CardinalityTypeEnum.CardinalityType? RelationPkCardinality { get; private set; }

	public IEnumerable<IDependency> Children { get; private set; }

	public Dependency(DbRepository repository, DependencyRow dependency)
	{
		this.repository = repository;
		Id = dependency.Id;
		ReferenceId = dependency.DestinationObjectId;
		ReferenceType = dependency.DestinationObjectType;
		ObjectName = dependency.Name;
		ObjectType = dependency.TypeEnum;
		ObjectSubtype = dependency.SubtypeEnum;
		ObjectSource = UserTypeEnum.ObjectToType(dependency.ObjectSource).GetValueOrDefault();
		ObjectFullDisplayName = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? dependency.DisplayName : dependency.DisplayNameWithoutTitle);
		ObjectFullDisplayNameShowSchema = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? dependency.DisplayNameShowSchema : dependency.DisplayNameShowSchemaWithoutTitle);
		Type = dependency.DependencyCommonTypeAsString;
		Source = UserTypeEnum.ObjectToType(dependency.Source).GetValueOrDefault();
		RelationPkCardinality = dependency.PKCardinality.Type;
		RelationFkCardinality = dependency.FKCardinality.Type;
		Children = dependency.Children?.Select((DependencyRow x) => new Dependency(repository, x));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
