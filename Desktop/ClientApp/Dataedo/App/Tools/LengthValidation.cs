using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

namespace Dataedo.App.Tools;

public static class LengthValidation
{
	public const int MaxTitleOrNameLength = 80;

	public const int MaxFilterConditionLength = 100;

	public const int MaxCustomFieldInFileRepoLength = 3750;

	public const int MaxDataTypeLength = 250;

	public const int MaxDataSize = 50;

	private const int MaxExtendedPropertiesLength = 128;

	public static void SetTitleOrNameLengthLimit(TextEdit textEdit)
	{
		textEdit.Properties.MaxLength = 80;
	}

	public static void SetTitleOrNameLengthLimit(RepositoryItemTextEdit textEdit)
	{
		textEdit.MaxLength = 80;
	}

	public static void SetExtendedPropertiesLimit(RepositoryItemTextEdit textEdit)
	{
		textEdit.MaxLength = 128;
	}

	public static void SetFilterConditionLengthLimit(TextEdit textEdit)
	{
		textEdit.Properties.MaxLength = 100;
	}

	public static void SetCustomFieldLength(RepositoryItem repositoryItem)
	{
		if (repositoryItem is RepositoryItemTextEdit)
		{
			SetCustomFieldLength(repositoryItem as RepositoryItemTextEdit);
		}
	}

	public static void SetCustomFieldLength(RepositoryItemTextEdit textEdit)
	{
		if (StaticData.IsProjectFile)
		{
			textEdit.MaxLength = 3750;
		}
	}

	public static void SetCustomFieldLength(TextEdit textEdit)
	{
		if (StaticData.IsProjectFile)
		{
			textEdit.Properties.MaxLength = 3750;
		}
	}

	public static void SetDataTypeLength(RepositoryItemTextEdit textEdit)
	{
		textEdit.MaxLength = 250;
	}

	public static void SetDataTypeLength(TextEdit textEdit)
	{
		textEdit.Properties.MaxLength = 250;
	}

	public static void SetDataSize(RepositoryItemTextEdit textEdit)
	{
		textEdit.MaxLength = 50;
	}

	public static bool IsCustomFieldValueLenghtValid(object value)
	{
		if (!StaticData.IsProjectFile)
		{
			return true;
		}
		if (value is string text)
		{
			return text.Length <= 3750;
		}
		return true;
	}
}
