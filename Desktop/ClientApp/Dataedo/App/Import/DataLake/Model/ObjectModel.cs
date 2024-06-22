using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Helpers.FileImport;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Model;

public class ObjectModel
{
	private bool isSelected = true;

	public bool IsInitializedSuccessfully { get; set; } = true;


	public string InitializationDetails { get; set; }

	public DataLakeTypeEnum.DataLakeType? DataLakeType { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; }

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype { get; protected set; }

	public string SubtypeDisplayText => SharedObjectSubtypeEnum.TypeToStringForSingle(ObjectType, ObjectSubtype);

	public UserTypeEnum.UserType Source { get; set; } = UserTypeEnum.UserType.USER;


	public string Name { get; set; }

	public string OriginalName { get; set; }

	public string CorrectedName { get; protected set; }

	public bool IsCorrectedNameSet => Name == CorrectedName;

	public string Location { get; set; }

	public string FilePath { get; set; }

	public string SchemaScript { get; set; }

	public List<FieldModel> Fields { get; set; }

	public string FieldsString
	{
		get
		{
			List<FieldModel> fields = Fields;
			if (fields == null || fields.Count() == 0)
			{
				return null;
			}
			List<string> values = Fields.Select((FieldModel x) => x.FullName).ToList();
			return string.Join(", ", values);
		}
	}

	public string FieldsStringNarrow
	{
		get
		{
			List<FieldModel> fields = Fields;
			if (fields == null || fields.Count() == 0)
			{
				return null;
			}
			List<string> list = (from x in Fields.Take(10)
				select x.FullName).ToList();
			if (Fields.Count() > 10)
			{
				list.Add("...");
			}
			return string.Join(", ", list);
		}
	}

	public bool HasHierarchicalFiels => Fields?.Any((FieldModel x) => x.Level > 1) ?? false;

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = IsInitializedSuccessfully && value;
		}
	}

	public bool IsValid
	{
		get
		{
			if (!ObjectExists && !IsNameEmpty)
			{
				return IsInitializedSuccessfully;
			}
			return false;
		}
	}

	public bool IsValidToAdd
	{
		get
		{
			if (IsSelected)
			{
				return IsValid;
			}
			return false;
		}
	}

	public bool IsNameEmpty => string.IsNullOrEmpty(Name);

	public bool ObjectExists { get; set; }

	public bool OriginalObjectExists { get; set; }

	public int? ObjectId { get; set; }

	public ImportItem ImportItem { get; set; }

	public ObjectModel(string name, string location, string filePath, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype)
	{
		Name = name;
		OriginalName = name;
		CorrectedName = name;
		Location = location;
		FilePath = filePath;
		DataLakeType = dataLakeType;
		ObjectType = objectType;
		ObjectSubtype = objectSubtype;
		Fields = new List<FieldModel>();
	}

	public ObjectModel(string name, string location, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype)
		: this(name, location, location, dataLakeType, objectType, objectSubtype)
	{
	}

	public ObjectModel(string name, string location, string filePath, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType, bool isInitializedSuccessfully = false)
	{
		Name = name;
		OriginalName = name;
		CorrectedName = name;
		Location = location;
		FilePath = filePath;
		DataLakeType = dataLakeType;
		ObjectType = objectType;
		ObjectSubtype = SharedObjectSubtypeEnum.GetDefaultByMainType(ObjectType);
		Fields = new List<FieldModel>();
		IsInitializedSuccessfully = isInitializedSuccessfully;
		IsSelected = IsInitializedSuccessfully;
	}

	public ObjectModel(string name, string location, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType, bool isInitializedSuccessfully = false)
		: this(name, location, location, dataLakeType, objectType, isInitializedSuccessfully)
	{
	}

	public void SetNameAsCorrectedName()
	{
		CorrectedName = Name;
	}

	public void ApplyData(ObjectModel objectModel)
	{
		IsInitializedSuccessfully = objectModel.IsInitializedSuccessfully;
		InitializationDetails = objectModel.InitializationDetails;
		DataLakeType = objectModel.DataLakeType;
		ObjectType = objectModel.ObjectType;
		ObjectSubtype = objectModel.ObjectSubtype;
		IsSelected = objectModel.IsSelected;
	}
}
