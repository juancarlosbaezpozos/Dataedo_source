using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class TermDataLink
{
	internal ITermDataLink Model;

	[JsonProperty("id")]
	public string Id => LinkGenerator.ToObjectId(Model.ObjectType.Value, Model.ObjectId);

	[JsonProperty("object_name")]
	public string ObjectName => Model.ObjectName;

	[JsonProperty("name")]
	public string Name => Model.ObjectNameWithSchemaAndTitle;

	[JsonProperty("type")]
	public string Type => SharedObjectTypeEnum.TypeToString(Model.ObjectType);

	[JsonProperty("subtype")]
	public string Subtype => SharedObjectSubtypeEnum.TypeToString(Model.ObjectType, Model.ObjectSubtype);

	[JsonProperty("is_user_defined")]
	public bool IsUserDefined => Model.ObjectSource == UserTypeEnum.UserType.USER;

	[JsonProperty("has_inner_object")]
	public bool HasInner => Model.InnerObjectId.HasValue;

	[JsonProperty("inner_path")]
	public string InnerPath => Model.InnerPath;

	[JsonProperty("inner_name")]
	public string InnerName => Model.InnerName;

	[JsonProperty("inner_type")]
	public string InnerType => SharedObjectTypeEnum.TypeToString(Model.InnerObjectType);

	[JsonProperty("inner_subtype")]
	public string InnerSubtype
	{
		get
		{
			string text = SharedObjectSubtypeEnum.TypeToString(Model.InnerObjectType, Model.InnerObjectSubtype);
			if (Model.InnerObjectType != SharedObjectTypeEnum.ObjectType.Column)
			{
				return text;
			}
			if (Model.InnerObjectSubtype == SharedObjectSubtypeEnum.ObjectSubtype.Dimension || Model.InnerObjectSubtype == SharedObjectSubtypeEnum.ObjectSubtype.Object)
			{
				text = "COLUMN_" + text;
			}
			return text;
		}
	}

	[JsonProperty("inner_is_user_defined")]
	public bool InnerIsUserDefined => Model.InnerObjectSource == UserTypeEnum.UserType.USER;

	[JsonProperty("documentation_id")]
	public string DocumentationId => LinkGenerator.ToDocumntationObjectId(Model.ObjectDocumentation.Id);

	[JsonProperty("documentation_name")]
	public string DocumentationName => NameBuilder.For(Model.ObjectDocumentation).WithSchema().WithTitle()
		.Build();

	public TermDataLink(ITermDataLink model)
	{
		Model = model;
	}
}
