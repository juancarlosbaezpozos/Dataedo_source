using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Support.DocWizardForm;

public class DocDBObject
{
	private DocFormatEnum.DocFormat? docFormat;

	public ObjectTypeHierarchy ObjectType { get; set; }

	public virtual string Name
	{
		get
		{
			ObjectTypeHierarchy objectType = ObjectType;
			if (objectType != null && objectType.CustomType.HasValue)
			{
				return CustomExcludedTypeEnum.TypeToString(ObjectType?.CustomType.Value);
			}
			ObjectTypeHierarchy objectType2 = ObjectType;
			if (objectType2 != null && objectType2.ObjectSubtype == SharedObjectTypeEnum.ObjectType.BusinessGlossary && (docFormat == DocFormatEnum.DocFormat.HTML || docFormat == DocFormatEnum.DocFormat.PDF))
			{
				return "Business Glossary entries";
			}
			return SharedObjectTypeEnum.TypeToStringForMenu(ObjectType.ObjectSubtype);
		}
	}

	public int Id { get; set; }

	public int ParentId { get; set; }

	public bool IsEnabled { get; }

	public bool IsCheckedByDefault { get; } = true;


	public void SetParentObjectType(SharedObjectTypeEnum.ObjectType? parentObjectType)
	{
		if (ObjectType != null)
		{
			ObjectType.ParentObjectType = parentObjectType;
		}
	}

	public DocDBObject(SharedObjectTypeEnum.ObjectType objectType, SharedObjectTypeEnum.ObjectType objectSubtype, bool isEnabled = true, int parentId = 0, bool isCheckedByDefault = true)
	{
		Id = ++DocDBObjectsManager.NextId;
		ParentId = parentId;
		ObjectType = new ObjectTypeHierarchy(objectType, objectSubtype);
		IsEnabled = isEnabled;
		IsCheckedByDefault = isCheckedByDefault;
	}

	public DocDBObject(SharedObjectTypeEnum.ObjectType objectType, SharedObjectTypeEnum.ObjectType objectSubtype, int parentId = 0, bool isCheckedByDefault = true, DocFormatEnum.DocFormat? docFormat = null, bool? isEnabled = null)
		: this(objectType, objectSubtype, isEnabled.GetValueOrDefault(), parentId, isCheckedByDefault)
	{
		this.docFormat = docFormat;
	}

	public DocDBObject(CustomExcludedTypeEnum.CustomExcludedType type, bool isEnabled = true, int parentId = 0, bool isCheckedByDefault = true)
	{
		Id = ++DocDBObjectsManager.NextId;
		ParentId = parentId;
		ObjectType = new ObjectTypeHierarchy(type);
		IsEnabled = isEnabled;
		IsCheckedByDefault = isCheckedByDefault;
	}

	public DocDBObject(CustomExcludedTypeEnum.CustomExcludedType type, int parentId = 0, bool isCheckedByDefault = true)
		: this(type, isEnabled: true, parentId, isCheckedByDefault)
	{
	}
}
