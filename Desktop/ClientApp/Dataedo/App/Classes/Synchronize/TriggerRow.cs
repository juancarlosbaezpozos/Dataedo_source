using System.Text;
using Dataedo.App.Enums;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class TriggerRow : BasicRow
{
	public string DatabaseName { get; set; }

	public string TableName { get; set; }

	public string TableSchema { get; set; }

	public bool Before { get; set; }

	public bool After { get; set; }

	public bool InsteadOf { get; set; }

	public bool OnInsert { get; set; }

	public bool OnUpdate { get; set; }

	public bool OnDelete { get; set; }

	public bool Disabled { get; set; }

	public string Definition { get; set; }

	public string WhenRun { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? Subtype { get; set; }

	public string CustomFieldsString => base.CustomFields.GetCustomFieldsString(SharedObjectTypeEnum.ObjectType.Trigger);

	public string SubtypeDisplayText => SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Trigger, Subtype);

	public bool ShowEditRemoveButton
	{
		get
		{
			if (base.Source == UserTypeEnum.UserType.DBMS)
			{
				return base.Status == SynchronizeStateEnum.SynchronizeState.Deleted;
			}
			return true;
		}
	}

	public TriggerRow(TriggerObject data, CustomFieldsSupport customFieldsSupport = null)
		: base(data)
	{
		base.Id = data.TriggerId;
		base.Name = data.Name;
		WhenRun = GetWhenRunDetails(data);
		Definition = data.Definition;
		base.Description = data.Description;
		Disabled = data.Disabled;
		Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Trigger, PrepareValue.ToString(data.Subtype));
		base.Source = UserTypeEnum.UserType.DBMS;
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Trigger, base.Id, customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(data);
		}
	}

	public TriggerRow(TriggerSynchronizationObjectForInterfaceTables dataReader, CustomFieldsSupport customFieldsSupport = null)
		: this((TriggerSynchronizationObject)dataReader, customFieldsSupport)
	{
		Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Trigger, PrepareValue.ToString(dataReader.Type));
	}

	public TriggerRow()
	{
	}

	public TriggerRow(TriggerSynchronizationObject dataReader, CustomFieldsSupport customFieldsSupport = null)
	{
		base.Name = dataReader.TriggerName;
		DatabaseName = dataReader.DatabaseName;
		TableName = dataReader.TableName;
		TableSchema = dataReader.TableSchema;
		OnUpdate = dataReader.Isupdate;
		OnDelete = dataReader.Isdelete;
		OnInsert = dataReader.Isinsert;
		Before = dataReader.Isbefore;
		After = dataReader.Isafter;
		InsteadOf = dataReader.Isinsteadof;
		Disabled = dataReader.Disabled;
		Definition = dataReader.Definition;
		base.Description = dataReader.Description;
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Trigger;
		Subtype = (dataReader.Type.Equals("TA") ? SharedObjectSubtypeEnum.ObjectSubtype.CLRTrigger : SharedObjectSubtypeEnum.ObjectSubtype.Trigger);
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
	}

	public static string GetWhenRunDetails(TriggerObject row)
	{
		StringBuilder stringBuilder = new StringBuilder().Append(PrepareValue.ToString(row.WhenRun));
		if (PrepareValue.ToBool(row.OnInsert) || PrepareValue.ToBool(row.OnDelete) || PrepareValue.ToBool(row.OnUpdate))
		{
			stringBuilder.Append(" ").Append(PrepareValue.ToBool(row.OnInsert) ? "Insert, " : string.Empty).Append(PrepareValue.ToBool(row.OnUpdate) ? "Update, " : string.Empty)
				.Append(PrepareValue.ToBool(row.OnDelete) ? "Delete, " : string.Empty);
			string text = stringBuilder.ToString();
			return text.Substring(0, text.Length - 2);
		}
		return stringBuilder.ToString();
	}
}
