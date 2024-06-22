using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Enums;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class Dependency
{
	internal IDependency Model;

	[JsonProperty("object_name")]
	public string ObjectName => Model.ObjectFullDisplayName;

	[JsonProperty("object_name_show_schema")]
	public string ObjectNameShowSchema => Model.ObjectFullDisplayNameShowSchema;

	[JsonProperty("object_type")]
	public string ObjectType => SharedObjectSubtypeEnum.TypeToString(Model.ObjectType, Model.ObjectSubtype);

	[JsonProperty("object_id", NullValueHandling = NullValueHandling.Ignore)]
	public string ObjectId => GetObjectId();

	[JsonProperty("type")]
	public string Type => Model.Type;

	[JsonProperty("pk_cardinality", NullValueHandling = NullValueHandling.Ignore)]
	public string PkCardinality => CardinalityTypeEnum.TypeToId(Model.RelationPkCardinality);

	[JsonProperty("fk_cardinality", NullValueHandling = NullValueHandling.Ignore)]
	public string FkCardinality => CardinalityTypeEnum.TypeToId(Model.RelationFkCardinality);

	[JsonProperty("object_user_defined")]
	public bool IsObjectUserDefined => Model.ObjectSource == UserTypeEnum.UserType.USER;

	[JsonProperty("user_defined")]
	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	[JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
	public IEnumerable<Dependency> Children { get; private set; }

	public Dependency(IDependency model)
	{
		Model = model;
		Children = model.Children?.Select((IDependency x) => new Dependency(x));
	}

	private string GetObjectId()
	{
		try
		{
			return LinkGenerator.ToObjectId(Model.ObjectType, Model.ReferenceId.Value);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
