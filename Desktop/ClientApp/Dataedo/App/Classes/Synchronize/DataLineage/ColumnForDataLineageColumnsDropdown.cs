using System.Drawing;
using Dataedo.App.Tools;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Classes.Synchronize.DataLineage;

public class ColumnForDataLineageColumnsDropdown : BasicRow
{
	private string _parentObjectSchema;

	public string ParentObjectName { get; set; }

	public string ParentObjectSchema
	{
		get
		{
			return _parentObjectSchema;
		}
		set
		{
			_parentObjectSchema = value ?? string.Empty;
		}
	}

	public string ParentObjectFullName
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(ParentObjectSchema))
			{
				return ParentObjectSchema + "." + ParentObjectName;
			}
			return ParentObjectName;
		}
	}

	public string ParentObjectSubtypeText { get; set; } = "TABLE";


	public SharedObjectSubtypeEnum.ObjectSubtype ParentObjectSubtype => SharedObjectSubtypeEnum.StringToType(base.ParentObjectType, ParentObjectSubtypeText);

	public string Path { get; set; }

	public string ObjectSubtypeText { get; set; } = "COLUMN";


	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => SharedObjectSubtypeEnum.StringToType(base.ObjectType, ObjectSubtypeText);

	public string FullName => GetFullName(Path, base.Name);

	public string FullNameWithTitle => GetFullName(Path, base.Name, base.Title);

	public string FullNameFormatted => GetFullNameFormatted(Path, base.Name);

	public string FullNameFormattedWithTitle => GetFullNameFormatted(Path, base.Name, base.Title);

	public Bitmap Icon { get; private set; }

	public Bitmap ParentObjectIcon { get; private set; }

	public DataFlowRow DataFlowRow { get; }

	public ColumnForDataLineageColumnsDropdown(DataLineageDropdownColumnObject dataLineageDropdownColumnObject)
	{
		base.Id = dataLineageDropdownColumnObject.ColumnId;
		base.Name = dataLineageDropdownColumnObject.Name;
		ParentObjectName = dataLineageDropdownColumnObject.ParentObjectName;
		ParentObjectSchema = dataLineageDropdownColumnObject.ParentObjectSchema;
		base.ParentObjectType = SharedObjectTypeEnum.StringToType(dataLineageDropdownColumnObject.ParentObjectType);
		ParentObjectSubtypeText = dataLineageDropdownColumnObject.ParentObjectSubtype;
		base.ObjectType = SharedObjectTypeEnum.StringToType(dataLineageDropdownColumnObject.ObjectType) ?? SharedObjectTypeEnum.ObjectType.Column;
		ObjectSubtypeText = dataLineageDropdownColumnObject.ObjectSubtype;
		base.Title = dataLineageDropdownColumnObject.Title;
		base.Source = UserTypeEnum.ObjectToTypeOrDbms(dataLineageDropdownColumnObject.Source);
		if (base.ObjectType == SharedObjectTypeEnum.ObjectType.Parameter)
		{
			ParameterRow.ModeEnum? mode = ParameterRow.GetMode(ObjectSubtypeText);
			Icon = Icons.GetParameterIcon(mode, base.Source);
		}
		else
		{
			Icon = IconsSupport.GetObjectIcon(base.ObjectType, ObjectSubtype, base.Source);
		}
		ParentObjectIcon = IconsSupport.GetObjectIcon(base.ParentObjectType, ParentObjectSubtype, base.Source);
	}

	public ColumnForDataLineageColumnsDropdown(DataLineageDropdownColumnObject dataReader, DataFlowRow dataFlowRow)
		: this(dataReader)
	{
		DataFlowRow = dataFlowRow;
	}

	private static string GetFullName(string path, string name)
	{
		return ColumnNames.GetFullName(path, name);
	}

	private static string GetFullName(string path, string name, string title)
	{
		return ColumnNames.GetFullName(path, name, title);
	}

	private static string GetFullNameFormatted(string path, string name)
	{
		return ColumnNames.GetFullNameFormatted(path, name);
	}

	private static string GetFullNameFormatted(string path, string name, string title)
	{
		return ColumnNames.GetFullNameFormatted(path, name, title);
	}
}
