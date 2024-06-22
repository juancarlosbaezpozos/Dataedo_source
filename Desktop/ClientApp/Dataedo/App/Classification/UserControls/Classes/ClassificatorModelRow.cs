using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Data.MetadataServer;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Enums;

namespace Dataedo.App.Classification.UserControls.Classes;

public class ClassificatorModelRow : StatedObject, IClassificatorModel
{
	public string Title { get; set; }

	public string Description { get; set; }

	public int Id { get; set; }

	public List<ClassificatorCustomFieldRow> Fields { get; private set; }

	public List<ClassificationRuleRow> Rules { get; private set; }

	public IEnumerable<ClassificatorCustomField> UsedFields => Fields?.Where((ClassificatorCustomFieldRow x) => x.IsUsed && x.RowState != ManagingRowsEnum.ManagingRows.Deleted)?.OrderBy((ClassificatorCustomFieldRow x) => x.Number);

	public ClassificatorModelRow()
	{
		Fields = new List<ClassificatorCustomFieldRow>();
		Rules = new List<ClassificationRuleRow>();
	}

	public ClassificatorModelRow(ClassificatorModel classificatorModel)
	{
		Title = classificatorModel.Title;
		Description = classificatorModel.Description;
		Id = classificatorModel.Id;
		List<ClassificationRule> source;
		if (Id != 0)
		{
			source = DB.Classificator.GetClassificationRules(Id);
		}
		else
		{
			source = new List<ClassificationRule>();
			base.RowState = ManagingRowsEnum.ManagingRows.Added;
		}
		List<ClassificationMaskRow> classificationsMasks = DB.Classificator.GetMasksFromRepositoryWithoutPresenceData();
		Rules = source.Select((ClassificationRule x) => new ClassificationRuleRow(x, this, classificationsMasks.First((ClassificationMaskRow y) => y.MaskName == x.MaskName))).ToList();
		Fields = (from x in classificatorModel.Fields
			where x.IsUsed
			select new ClassificatorCustomFieldRow(this, x)).ToList();
	}

	public ClassificatorCustomFieldRow AddNewField(int? customFieldClassId, string newFieldName)
	{
		int firstNotUsedFieldNumber = this.GetFirstNotUsedFieldNumber();
		if (firstNotUsedFieldNumber == 0)
		{
			return null;
		}
		ClassificatorCustomFieldRow classificatorCustomFieldRow = new ClassificatorCustomFieldRow(this, newFieldName, firstNotUsedFieldNumber, customFieldClassId)
		{
			RowState = ManagingRowsEnum.ManagingRows.Added
		};
		Fields.Add(classificatorCustomFieldRow);
		this.SetUpdatedIfNotAdded();
		return classificatorCustomFieldRow;
	}

	public void DeleteField(ClassificatorCustomFieldRow field)
	{
		if (!Fields.Contains(field))
		{
			return;
		}
		if (field.RowState == ManagingRowsEnum.ManagingRows.Added)
		{
			Fields.Remove(field);
		}
		else
		{
			field.RowState = ManagingRowsEnum.ManagingRows.Deleted;
		}
		foreach (ClassificationRuleRow rule in Rules)
		{
			rule.SetCustomFieldValue(field.Number, null);
			rule.SetUpdatedIfNotAdded();
		}
		this.SetUpdatedIfNotAdded();
	}

	public bool AnyChangesMade()
	{
		if (!this.IsChanged() && !Fields.Any((ClassificatorCustomFieldRow x) => x.IsChanged()))
		{
			return Rules.Any((ClassificationRuleRow x) => x.IsChanged());
		}
		return true;
	}

	public void SetUnchanged()
	{
		base.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
		Fields.ForEach(delegate(ClassificatorCustomFieldRow x)
		{
			x.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
		});
		Rules.ForEach(delegate(ClassificationRuleRow x)
		{
			x.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
		});
	}

	public void RefreshRules()
	{
		if (Id == 0)
		{
			return;
		}
		List<ClassificationRule> classificationRules = DB.Classificator.GetClassificationRules(Id);
		List<ClassificationMaskRow> classificationsMasks = DB.Classificator.GetMasksFromRepositoryWithoutPresenceData();
		IEnumerable<ClassificationRuleRow> deletedRules = Rules.Where((ClassificationRuleRow x) => !classificationRules.Select((ClassificationRule y) => y.Id).Contains(x.Id));
		Rules.RemoveAll((ClassificationRuleRow x) => deletedRules.Contains(x));
		IEnumerable<ClassificationRule> source = classificationRules.Where((ClassificationRule x) => !Rules.Select((ClassificationRuleRow y) => y.Id).Contains(x.Id));
		Rules.AddRange(source.Select((ClassificationRule x) => new ClassificationRuleRow(x, this, classificationsMasks.First((ClassificationMaskRow y) => y.MaskName == x.MaskName))));
		Rules.ForEach(delegate(ClassificationRuleRow x)
		{
			x.Mask = classificationsMasks.First((ClassificationMaskRow y) => y.MaskName == x.MaskName);
		});
	}
}
