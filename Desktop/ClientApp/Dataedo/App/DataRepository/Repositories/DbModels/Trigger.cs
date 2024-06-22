using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Trigger : ITrigger, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IList<ICustomField>> customFields;

	public int Id { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public bool Before { get; private set; }

	public bool After { get; private set; }

	public bool InsteadOf { get; private set; }

	public bool OnInsert { get; private set; }

	public bool OnUpdate { get; private set; }

	public bool OnDelete { get; private set; }

	public string Script { get; private set; }

	public IList<ICustomField> CustomFields => customFields.Value;

	public Trigger(DbRepository repository, SharedObjectTypeEnum.ObjectType parentObjectType, TriggerObject row)
	{
		Trigger trigger = this;
		this.repository = repository;
		Id = row.TriggerId;
		Name = row.Name;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Before = row.Before;
		After = row.After;
		InsteadOf = row.InsteadOf;
		OnInsert = row.OnInsert;
		OnUpdate = row.OnUpdate;
		OnDelete = row.OnDelete;
		Script = (repository.Excluded.IsExluded(parentObjectType, SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script) ? null : PrepareValue.ToString(row.Definition)?.Trim());
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in trigger.repository.GetCustomFields()
			where x.TriggerVisibility
			select new CustomField(trigger.repository, row, x))).ToList());
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
