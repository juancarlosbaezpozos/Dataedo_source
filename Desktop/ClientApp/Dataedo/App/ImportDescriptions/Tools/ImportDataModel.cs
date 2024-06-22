using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.Properties;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Model.Data.Common.CustomFieldsBase;
using Dataedo.Shared.Enums;
using DevExpress.DataProcessing;

namespace Dataedo.App.ImportDescriptions.Tools;

public abstract class ImportDataModel : CustomFieldsDataBase<FieldData>
{
	public SharedObjectTypeEnum.ObjectType TableObjectType { get; set; }

	public Image TableTypeImage => TableObjectType switch
	{
		SharedObjectTypeEnum.ObjectType.Table => Resources.table_16, 
		SharedObjectTypeEnum.ObjectType.View => Resources.view_16, 
		SharedObjectTypeEnum.ObjectType.Structure => Resources.structure_16, 
		_ => Resources.question_16, 
	};

	public int? TableId { get; set; }

	public string Schema { get; set; }

	public string TableName { get; set; }

	public DefaultFieldData Title { get; set; }

	public DefaultFieldData Description { get; set; }

	public Image Image
	{
		get
		{
			if (!Exists)
			{
				return Resources.error_16;
			}
			if (IsDuplicated)
			{
				return Resources.warning_16;
			}
			if (IncorrectValues)
			{
				return Resources.about_16;
			}
			return Resources.ok_16;
		}
	}

	public string Status
	{
		get
		{
			if (!Exists)
			{
				return "DoNotExists";
			}
			if (IsDuplicated)
			{
				return "Duplicated";
			}
			if (IncorrectValues)
			{
				return "HaveWrongValues";
			}
			return "Ok";
		}
	}

	public bool IsValid
	{
		get
		{
			if (Exists && !IsDuplicated)
			{
				return !IncorrectValues;
			}
			return false;
		}
	}

	public bool IsDuplicated { get; set; }

	public bool IncorrectValues { get; set; }

	public bool Exists => IdProperty.HasValue;

	public abstract int? IdProperty { get; }

	public bool IsAnySelected
	{
		get
		{
			if (!base.Fields.Any((FieldData x) => x?.IsSelected ?? false) && (!Title.IsImported || !Title.IsSelected))
			{
				if (Description.IsImported)
				{
					return Description.IsSelected;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsAnyChanged
	{
		get
		{
			if (!base.Fields.Any((FieldData x) => x?.IsChanged ?? false) && (!Title.IsImported || !Title.IsChanged))
			{
				if (Description.IsImported)
				{
					return Description.IsChanged;
				}
				return false;
			}
			return true;
		}
	}

	public ImportDataModel()
	{
		Title = new DefaultFieldData();
		Description = new DefaultFieldData();
	}

	public void SetTitle(string value)
	{
		Title.InitializeOverwriteValue(ShortenedString(value, 80).Replace(Environment.NewLine, " ").Replace("\n", " "));
	}

	public void SetDescription(string value)
	{
		Description.InitializeOverwriteValue(value);
	}

	public static string ShortenedString(string value, int length)
	{
		if (value.Length <= length)
		{
			return value;
		}
		return new string(value.Take(length).ToArray());
	}

	public int CountAllValues(ChangeEnum.Change change)
	{
		return base.Fields.Where((FieldData x) => x != null && x.Change == change).Count() + ((Title.IsImported && Title.Change == change) ? 1 : 0) + ((Description.IsImported && Description.Change == change) ? 1 : 0);
	}

	public int CountSelectedValues(ChangeEnum.Change change)
	{
		return base.Fields.Where((FieldData x) => x != null && x.Change == change).Count((FieldData x) => x?.IsSelected ?? false) + ((Title.IsImported && Title.Change == change && Title.IsSelected) ? 1 : 0) + ((Description.IsImported && Description.Change == change && Description.IsSelected) ? 1 : 0);
	}

	public void SetCustomFields(IEnumerable<CustomFieldRowExtended> customFields, CustomFieldsData customFieldsData)
	{
		foreach (CustomFieldRowExtended customField in customFields)
		{
			int columnIndexInArray = customField.ColumnIndexInArray;
			if (base.Fields[columnIndexInArray] == null)
			{
				base.Fields[columnIndexInArray] = new FieldData();
			}
			base.Fields[columnIndexInArray].CurrentValue = customFieldsData.Fields[columnIndexInArray];
		}
	}

	public void RemoveCustomFields(CustomFieldRowExtended customField)
	{
		int columnIndexInArray = customField.ColumnIndexInArray;
		if (base.Fields[columnIndexInArray] != null)
		{
			base.Fields[columnIndexInArray] = null;
		}
	}

	public void ValidateCustomFieldsValues(IEnumerable<CustomFieldRowExtended> customFields)
	{
		IncorrectValues = false;
		foreach (CustomFieldRowExtended customField in customFields)
		{
			int columnIndexInArray = customField.ColumnIndexInArray;
			if (base.Fields[columnIndexInArray] != null && customField.IsDomainValueType && !customField.IsValueProperForDomainValuesType(base.Fields[columnIndexInArray].OverwriteValue))
			{
				IncorrectValues = true;
				break;
			}
		}
	}

	public void UnselectAll(ChangeEnum.Change changeType)
	{
		base.Fields.Where((FieldData x) => x != null && x.Change == changeType).ForEach(delegate(FieldData x)
		{
			x.IsSelected = false;
		});
		if (Title.Change == changeType)
		{
			Title.IsSelected = false;
		}
		if (Description.Change == changeType)
		{
			Description.IsSelected = false;
		}
	}

	public void SelectAll(ChangeEnum.Change changeType, bool ignoreEmptyOverwriteValues)
	{
		base.Fields.Where((FieldData x) => x != null && x.Change == changeType && (!x.IsOverwriteValueEmpty || !ignoreEmptyOverwriteValues)).ForEach(delegate(FieldData x)
		{
			x.IsSelected = true;
		});
		if (Title.IsImported && Title.Change == changeType)
		{
			Title.IsSelected = !Title.IsOverwriteValueEmpty || !ignoreEmptyOverwriteValues;
		}
		if (Description.IsImported && Description.Change == changeType)
		{
			Description.IsSelected = !Description.IsOverwriteValueEmpty || !ignoreEmptyOverwriteValues;
		}
	}

	public void SelectAllNew()
	{
		base.Fields.Where((FieldData x) => x != null).ForEach(delegate(FieldData x)
		{
			x.SelectIfNew();
		});
		if (Title.IsImported)
		{
			Title.SelectIfNew();
		}
		if (Description.IsImported)
		{
			Description.SelectIfNew();
		}
	}

	public virtual void ClearData()
	{
		base.Fields.Where((FieldData x) => x != null).ForEach(delegate(FieldData x)
		{
			x.ClearCurrentData();
		});
		IsDuplicated = false;
		Title.CurrentValue = null;
		Description.CurrentValue = null;
		TableObjectType = SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
		TableId = null;
	}
}
