using System.Diagnostics;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Import.DataLake.Model;

[DebuggerDisplay("{FullName}")]
public class FieldModel
{
	public int Id { get; set; }

	public FieldModel ParentField { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectType => SharedObjectTypeEnum.ObjectType.Column;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype { get; set; }

	public string SubtypeDisplayText => SharedObjectSubtypeEnum.TypeToStringForSingle(ObjectType, ObjectSubtype);

	public UserTypeEnum.UserType Source { get; set; } = UserTypeEnum.UserType.USER;


	public string Name { get; set; }

	public string DataType { get; set; }

	public int? DataTypeSize { get; set; }

	public string DataTypeSizeString => DataTypeSize?.ToString();

	public bool Nullable { get; set; }

	public int Position { get; set; }

	public string DisplayPosition
	{
		get
		{
			if (Level != 1 && ParentField?.DisplayPosition != null)
			{
				return $"{ParentField?.DisplayPosition}.{Position}";
			}
			return Position.ToString();
		}
	}

	public int? ParentId { get; set; }

	public string Path { get; set; }

	public int Level { get; set; }

	public string Title { get; set; }

	public string Description { get; set; }

	public string FullName => ColumnNames.GetFullName(Path, Name);

	public string FullNameFormatted => ColumnNames.GetFullNameFormatted(Path, Name);
}
