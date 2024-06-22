using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.ClassificationSummary;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Base.Interfaces.Manipulation.Classificator;
using Dataedo.Data.Commands;
using Dataedo.DataProcessing.Classificator;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Data.History;
using Dataedo.Model.Enums;
using DevExpress.Utils.Extensions;

namespace Dataedo.App.Data.MetadataServer;

internal class ClassificatorMetaDB : ClassificatorDB
{
	public ClassificatorMetaDB(CommandsSetBase commands)
		: base(commands)
	{
	}

	public ClassificationMaskRow GetMaskFromRepository(string maskName, int? classificatorId = null)
	{
		if (!CheckIfMaskExists(maskName))
		{
			return null;
		}
		ClassificationMaskRow maskRow = new ClassificationMaskRow
		{
			MaskName = maskName
		};
		IOrderedEnumerable<ClassificatorModel> classificatorModels = from x in GetClassificators()
			orderby x.Title
			select x;
		maskRow.SetClassificatorsInPresence(classificatorModels, classificatorId);
		foreach (Dataedo.Model.Data.Classificator.ClassificationMaskPresence p in GetClassificationMaskPresence(maskName))
		{
			maskRow.PresenceInClassificators.Where((Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x) => x.Classificator.Id == p.ClassificatorId).ForEach(delegate(Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x)
			{
				x.IsPresent = true;
			});
		}
		maskRow.Patterns = (from x in GetClassificationMaskPatterns(maskName)
			select new ClassificationMaskPatternRow(maskRow, x)).ToList();
		return maskRow;
	}

	public List<ClassificationMaskRow> GetMasksFromRepositoryWithoutPresenceData()
	{
		List<ClassificationMaskRow> list = new List<ClassificationMaskRow>();
		foreach (IGrouping<string, ClassificationMaskPattern> item in from x in DB.Classificator.GetClassificationMasksPatterns()
			group x by x.MaskName)
		{
			ClassificationMaskRow maskRow = new ClassificationMaskRow
			{
				MaskName = item.Key
			};
			item.ForEach(delegate(ClassificationMaskPattern x)
			{
				maskRow.Patterns.Add(new ClassificationMaskPatternRow(maskRow, x));
			});
			list.Add(maskRow);
		}
		return list;
	}

	public void SaveClassificationMasksPresence(IEnumerable<ClassificationMaskRow> masksToDelete, IEnumerable<ClassificationMaskRow> changedMasks)
	{
		IEnumerable<Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence> enumerable = changedMasks?.SelectMany((ClassificationMaskRow x) => x.PresenceInClassificators)?.Where((Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x) => x.IsChanged);
		IEnumerable<IGrouping<string, Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence>> source = enumerable?.Where((Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x) => x.IsChanged && !x.IsPresent)?.GroupBy((Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence x) => x.MaskName);
		IClassificatorCommands classificator = commands.Manipulation.Classificator;
		string[] masksToDelete2 = masksToDelete.Select((ClassificationMaskRow x) => x.MaskName).ToArray();
		IClassificationRule[] rulesToInsert = (from x in enumerable
			where x.IsChanged && x.IsPresent
			select x.ToClassificationRule()).ToArray();
		classificator.SaveClassificationMasksPresence(masksToDelete2, rulesToInsert, source.Select((IGrouping<string, Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence> x) => new Tuple<string, int[]>(x.Key, x.Select((Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence y) => y.ClassificatorId).ToArray())).ToArray());
	}

	public void SaveClassificationMask(ClassificationMaskRow mask, string originalMaskName)
	{
		if (mask.RowState == ManagingRowsEnum.ManagingRows.Added)
		{
			IClassificatorCommands classificator = commands.Manipulation.Classificator;
			IClassificationMaskPattern[] array = mask.Patterns.Where((ClassificationMaskPatternRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Added && x.Id == 0).ToArray();
			IClassificationMaskPattern[] maskPatterns = array;
			IClassificationRule[] rulesToInsert = (from x in mask.PresenceInClassificators
				where x.IsChanged && x.IsPresent
				select x.ToClassificationRule()).ToArray();
			classificator.InsertClassificationMask(maskPatterns, rulesToInsert);
		}
		else if (mask.RowState == ManagingRowsEnum.ManagingRows.Deleted)
		{
			commands.Manipulation.Classificator.DeleteClassificationMask(mask.MaskName);
		}
		else if (mask.RowState == ManagingRowsEnum.ManagingRows.Updated)
		{
			IClassificatorCommands classificator2 = commands.Manipulation.Classificator;
			string maskName = mask.MaskName;
			IClassificationMaskPattern[] array = mask.Patterns.Where((ClassificationMaskPatternRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Added && x.Id == 0).ToArray();
			IClassificationMaskPattern[] maskPatternsToInsert = array;
			int[] maskPatternsToDelete = (from x in mask.Patterns
				where x.RowState == ManagingRowsEnum.ManagingRows.Deleted && x.Id > 0
				select x.Id).ToArray();
			array = mask.Patterns.Where((ClassificationMaskPatternRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Updated && x.Id > 0).ToArray();
			IClassificationMaskPattern[] maskPatternsToUpdate = array;
			IClassificationRule[] rulesToInsert = (from x in mask.PresenceInClassificators
				where x.IsChanged && x.IsPresent
				select x.ToClassificationRule()).ToArray();
			classificator2.UpdateClassificationMask(originalMaskName, maskName, maskPatternsToInsert, maskPatternsToDelete, maskPatternsToUpdate, rulesToInsert, (from x in mask.PresenceInClassificators
				where x.IsChanged && !x.IsPresent
				select x.ClassificatorId).ToArray());
		}
	}

	public void SaveClassification(ClassificatorModelRow classificatorModelRow, CustomFieldsSupport customFieldsSupport)
	{
		if (classificatorModelRow.RowState == ManagingRowsEnum.ManagingRows.Added)
		{
			IClassificatorCommands classificator = commands.Manipulation.Classificator;
			IClassificationRule[] rules = classificatorModelRow.Rules.Where((ClassificationRuleRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Added && x.Id == 0).ToArray();
			classificator.SaveClassificator(classificatorModelRow, rules);
			ClassificationTrackingHelper.TrackClassificationAdded();
		}
		else
		{
			if (classificatorModelRow.RowState != ManagingRowsEnum.ManagingRows.Updated)
			{
				return;
			}
			ClassificatorCustomFieldsService customFieldsService = GetCustomFieldsService(customFieldsSupport);
			List<CustomField> convertedCustomFieldWithOpenGeneralTypeChange = CustomFieldDB.GetConvertedCustomFieldWithOpenGeneralTypeChange(GetCustomFields(customFieldsSupport, classificatorModelRow.Fields));
			foreach (CustomField cf in convertedCustomFieldWithOpenGeneralTypeChange)
			{
				ClassificatorCustomFieldRow classificatorCustomFieldRow = classificatorModelRow.Fields.FirstOrDefault((ClassificatorCustomFieldRow y) => y.Id == cf.CustomFieldId);
				if (classificatorCustomFieldRow != null)
				{
					if (classificatorCustomFieldRow.RowState == ManagingRowsEnum.ManagingRows.Deleted)
					{
						cf.IsToDelete = true;
					}
					customFieldsService.UpdateCustomField(cf, classificatorCustomFieldRow, classificatorModelRow.Title);
				}
			}
			DB.CustomField.UpdateCustomFields(convertedCustomFieldWithOpenGeneralTypeChange.ToArray(), rebuildDictionaryIfNecessary: true, null, !Dataedo.App.StaticData.IsProjectFile);
			IClassificatorCommands classificator2 = commands.Manipulation.Classificator;
			IClassificationRule[] rules = classificatorModelRow.Rules.Where((ClassificationRuleRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Added && x.Id == 0).ToArray();
			IClassificationRule[] rulesToInsert = rules;
			rules = classificatorModelRow.Rules.Where((ClassificationRuleRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Updated && x.Id > 0).ToArray();
			classificator2.UpdateClassificator(classificatorModelRow, rulesToInsert, rules, (from x in classificatorModelRow.Rules
				where x.RowState == ManagingRowsEnum.ManagingRows.Deleted && x.Id > 0
				select x.Id).ToArray());
		}
	}

	public void DeleteClassification(ClassificatorModelRow classificatorModelRow, CustomFieldsSupport customFieldsSupport)
	{
		List<CustomField> convertedCustomFieldWithOpenGeneralTypeChange = CustomFieldDB.GetConvertedCustomFieldWithOpenGeneralTypeChange(GetCustomFields(customFieldsSupport, classificatorModelRow.Fields));
		convertedCustomFieldWithOpenGeneralTypeChange.ForEach(delegate(CustomField x)
		{
			x.IsToDelete = true;
		});
		DB.CustomField.UpdateCustomFields(convertedCustomFieldWithOpenGeneralTypeChange.ToArray(), rebuildDictionaryIfNecessary: true, null, !Dataedo.App.StaticData.IsProjectFile);
		commands.Manipulation.Classificator.DeleteClassificator(classificatorModelRow.Id, (from x in classificatorModelRow.Rules
			where x.Id > 0
			select x.Id).ToArray());
	}

	public List<CustomFieldRow> GetCustomFields(CustomFieldsSupport customFieldsSupport, IEnumerable<ClassificatorCustomField> fields)
	{
		List<CustomFieldRow> list = new List<CustomFieldRow>();
		ClassificatorCustomFieldsService customFieldsService = GetCustomFieldsService(customFieldsSupport);
		foreach (ClassificatorCustomField f in fields.Where((ClassificatorCustomField x) => x.Id.HasValue && x.Id != 0))
		{
			CustomFieldRow customFieldRow = customFieldsService.AllCustomFields.FirstOrDefault((CustomFieldRow x) => x.CustomFieldId == f.Id);
			if (customFieldRow != null)
			{
				list.Add(customFieldRow);
			}
		}
		return list;
	}

	public List<ClassificatorCustomFieldContainer> GetCustomFields(CustomFieldsSupport customFieldsSupport, IClassificatorModel classificatorModel)
	{
		List<ClassificatorCustomFieldContainer> list = new List<ClassificatorCustomFieldContainer>();
		List<CustomFieldRow> list2 = new List<CustomFieldRow>();
		ClassificatorCustomFieldsService customFieldsService = GetCustomFieldsService(customFieldsSupport);
		foreach (ClassificatorCustomField customField in classificatorModel.UsedFields)
		{
			ClassificatorCustomFieldContainer classificatorCustomFieldContainer = new ClassificatorCustomFieldContainer
			{
				ClassificatorCustomField = customField,
				ClassificatorTitle = classificatorModel.Title
			};
			CustomFieldRow customFieldRow = customFieldsService.AllCustomFields.FirstOrDefault((CustomFieldRow x) => x.CustomFieldId == customField.Id);
			if (customFieldRow == null)
			{
				classificatorCustomFieldContainer.CustomField = customFieldsService.CreateCustomField(customField, list2, classificatorModel.Title);
				list2.Add(classificatorCustomFieldContainer.CustomField);
			}
			else
			{
				classificatorCustomFieldContainer.CustomField = customFieldRow;
			}
			list.Add(classificatorCustomFieldContainer);
		}
		return list;
	}

	private ClassificatorCustomFieldsService GetCustomFieldsService(CustomFieldsSupport customFieldsSupport)
	{
		ClassificatorCustomFieldsService classificatorCustomFieldsService = new ClassificatorCustomFieldsService(customFieldsSupport);
		classificatorCustomFieldsService.LoadAllCustomFields();
		return classificatorCustomFieldsService;
	}

	public void UpdateClassificatorCustomFields(IEnumerable<ClassificatorDataModel> data, BackgroundWorker worker = null)
	{
		if (data == null || data.Count() == 0)
		{
			return;
		}
		IEnumerable<ClassificatorUpdateModel> source = MapToClassificatorUpdateModels(data);
		commands.Manipulation.Columns.UpdateCustomFields(source.ToArray(), SaveClassificationHistory, delegate
		{
			ProgressWaitFormInvoker.StepProgressWaitForm();
			data.Where((ClassificatorDataModel y) => y.ColumnId == y.ColumnId && y.DatabaseId == y.DatabaseId).FirstOrDefault()?.SavingCompleted();
		}, null, worker);
	}

	private void SaveClassificationHistory(ClassificatorUpdateModel classificatorUpdateModel, DbTransaction transaction)
	{
		HistoryModel[] classificatorHistoryModels = HistoryColumnsHelper.GetClassificatorHistoryModels(classificatorUpdateModel);
		DB.History.InsertHistoryModels(classificatorHistoryModels, transaction);
	}

	private IEnumerable<ClassificatorUpdateModel> MapToClassificatorUpdateModels(IEnumerable<ClassificatorDataModel> models)
	{
		List<ClassificatorUpdateModel> list = new List<ClassificatorUpdateModel>();
		foreach (ClassificatorDataModel item in models.Where((ClassificatorDataModel x) => x.IsChecked))
		{
			ClassificatorUpdateModel classificatorUpdateModel = new ClassificatorUpdateModel
			{
				ColumnId = item.ColumnId,
				DatabaseId = item.DatabaseId
			};
			for (int i = 1; i <= 5; i++)
			{
				if (item.IsFieldForSaving(i))
				{
					classificatorUpdateModel.Fields.Add(new CustomFieldModel
					{
						FieldName = item.GetFieldDBName(i),
						FieldUpdateTo = item.GetFieldUpdateValue(i),
						IsFieldChecked = item.IsChecked
					});
				}
			}
			if (classificatorUpdateModel.Fields.Count > 0)
			{
				list.Add(classificatorUpdateModel);
			}
		}
		return list;
	}
}
