using System;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class TermDataLink : ITermDataLink, IModel, ICloneable
{
	private DbRepository repository;

	private Lazy<ITerm> term;

	private Lazy<IDocumentation> objectDocumentation;

	public int Id { get; private set; }

	public int ObjectId { get; private set; }

	public int TermId { get; private set; }

	public ITerm Term => term.Value;

	public SharedObjectTypeEnum.ObjectType? ObjectType { get; private set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? ObjectSubtype { get; private set; }

	public string ObjectName { get; private set; }

	public string ObjectNameWithSchemaAndTitle { get; private set; }

	public UserTypeEnum.UserType ObjectSource { get; private set; }

	public int ObjectDocumentationId { get; private set; }

	public IDocumentation ObjectDocumentation => objectDocumentation.Value;

	public int? InnerObjectId { get; private set; }

	public SharedObjectTypeEnum.ObjectType? InnerObjectType { get; private set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? InnerObjectSubtype { get; private set; }

	public string InnerPath { get; private set; }

	public string InnerName { get; private set; }

	public string InnerObjectName { get; private set; }

	public UserTypeEnum.UserType? InnerObjectSource { get; private set; }

	public TermDataLink(DbRepository repository, DataLinkObjectExtended row)
	{
		this.repository = repository;
		Id = row.ObjectId;
		TermId = row.TermId.Value;
		ObjectId = row.ObjectId;
		ObjectType = SharedObjectTypeEnum.StringToType(row.ObjectType);
		ObjectSubtype = SharedObjectSubtypeEnum.StringToType(ObjectType, row.ObjectSubtype);
		ObjectName = row.ObjectNameWithSchema;
		ObjectNameWithSchemaAndTitle = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.ObjectNameWithSchemaAndTitle : row.ObjectNameWithSchema);
		ObjectSource = UserTypeEnum.ObjectToType(row.ObjectSource).GetValueOrDefault();
		ObjectDocumentationId = row.ObjectDocumentationId;
		InnerObjectId = row.ElementId;
		InnerObjectType = SharedObjectTypeEnum.StringToType(row.ElementType);
		InnerObjectSubtype = SharedObjectSubtypeEnum.StringToType(InnerObjectType, row.ElementSubtype);
		InnerPath = row.ElementPath;
		InnerName = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.ElementNameWithTitle : row.ElementName);
		InnerObjectName = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? (row.ObjectNameWithSchema + "." + row.ElementNameWithTitle) : (row.ObjectNameWithSchema + "." + row.ElementName));
		InnerObjectSource = UserTypeEnum.ObjectToType(row.ElementSource).GetValueOrDefault();
		term = new Lazy<ITerm>(() => this.repository.GetTerm(TermId));
		objectDocumentation = new Lazy<IDocumentation>(() => this.repository.GetDocumentation(ObjectDocumentationId));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
