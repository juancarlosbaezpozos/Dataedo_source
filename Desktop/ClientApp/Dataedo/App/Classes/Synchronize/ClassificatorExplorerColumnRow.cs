using System.Drawing;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ClassificatorExplorerColumnRow : ColumnRow
{
	public new Image Icon
	{
		get
		{
			if (base.Source != UserTypeEnum.UserType.DBMS)
			{
				return Resources.column_user_16;
			}
			return Resources.column_16;
		}
	}

	public string DatabaseTitle { get; }

	public SharedObjectTypeEnum.ObjectType? ParentType { get; }

	public SharedObjectSubtypeEnum.ObjectSubtype ParentSubtype { get; }

	public string TableTitle { get; }

	public string SingleParentType { get; }

	public string TableDisplayName => GetTableDisplayName();

	public string ColumnDisplayName => GetColumnDisplayName();

	public string ColumnDisplayNameFormatted => GetColumnDisplayNameFormatted();

	public ClassificatorExplorerColumnRow()
	{
	}

	public ClassificatorExplorerColumnRow(ColumnWithTableAndDatabaseObject row, CustomFieldsSupport customFieldsSupport)
	{
		base.Id = row.ColumnId;
		base.Name = row.Name;
		base.Title = row.Title;
		base.Path = row.Path;
		base.Description = row.Description;
		base.DataType = row.DatatypeLen;
		base.CreationDateString = PrepareValue.SetDateTimeWithFormatting(row.CreationDate);
		base.CreatedBy = row.CreatedBy;
		base.LastModificationDateString = PrepareValue.SetDateTimeWithFormatting(row.LastModificationDate);
		base.ModifiedBy = row.ModifiedBy;
		base.Nullable = row.Nullable;
		base.DefaultValue = row.DefaultValue;
		base.IsIdentity = row.IsIdentity;
		base.IsComputed = row.IsComputed;
		base.ComputedFormula = row.ComputedFormula;
		DatabaseTitle = row.DatabaseTitle;
		ParentType = SharedObjectTypeEnum.StringToType(row.ObjectType);
		ParentSubtype = SharedObjectSubtypeEnum.StringToType(base.ObjectType, row.Subtype);
		base.TableSchema = row.Schema;
		base.TableName = row.TableName;
		TableTitle = row.TableTitle;
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		SingleParentType = SharedObjectSubtypeEnum.TypeToStringForSingle(ParentType, ParentSubtype);
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(row);
			base.CustomFields.SetObjectId(base.Id);
			base.CustomFields.SetObjectType(SharedObjectTypeEnum.ObjectType.Column);
		}
	}

	public ClassificatorExplorerColumnRow(ColumnWithTableAndDatabaseObject row)
	{
		base.TableSchema = row.Schema;
		base.TableName = row.TableName;
		TableTitle = row.TableTitle;
		base.Path = row.Path;
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
	}

	private string GetColumnDisplayName()
	{
		string text = (string.IsNullOrEmpty(base.Title) ? string.Empty : (" (" + base.Title + ")"));
		return base.FullName + text;
	}

	private string GetColumnDisplayNameFormatted()
	{
		string text = (string.IsNullOrEmpty(base.Title) ? string.Empty : (" (" + base.Title + ")"));
		return base.FullNameFormatted + text;
	}

	private string GetTableDisplayName()
	{
		return string.Concat(string.IsNullOrEmpty(base.TableSchema) ? string.Empty : (base.TableSchema + "."), str2: string.IsNullOrEmpty(TableTitle) ? string.Empty : (" (" + TableTitle + ")"), str1: base.TableName);
	}
}
