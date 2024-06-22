using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms.Helpers;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.Extensions;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.UserControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classificator;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.Classificator;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace Dataedo.App.Tools.ClassificationSummary;

public class Classificator
{
	private readonly BandedGridViewManager manager;

	private ClasificatorDataService dataService;

	private readonly CustomFieldsSupport support;

	private Dictionary<string, IList<string>> classificatorDistinctValues;

	private ClassificationSummaryUserControl classificationSummaryUserControl;

	private int savingObjectsCount;

	public List<ClassificatorDataModel> DataSource { get; private set; }

	public IEnumerable<ClassificatorCustomFieldContainer> SelectedCustomFields { get; set; }

	public int ClassificatorId { get; set; }

	public Form Owner => classificationSummaryUserControl?.FindForm();

	public Classificator()
	{
		DataSource = new List<ClassificatorDataModel>();
		manager = new BandedGridViewManager();
		SelectedCustomFields = new List<ClassificatorCustomFieldContainer>();
		classificatorDistinctValues = new Dictionary<string, IList<string>>();
		dataService = new ClasificatorDataService();
	}

	public Classificator(CustomFieldsSupport support, ClassificationSummaryUserControl classificationSummaryUserControl)
		: this()
	{
		this.support = support;
		this.classificationSummaryUserControl = classificationSummaryUserControl;
	}

	public void SetBandedGridView(BandedGridView view)
	{
		manager.SetParameters(view, support, classificatorDistinctValues, SelectedCustomFields);
	}

	public async Task GetClassificationsAsync(IEnumerable<int> databasesIds, CancellationToken cancellationToken)
	{
		support.Fields.Where((CustomFieldRowExtended x) => !string.IsNullOrEmpty(x.Description));
		if (SelectedCustomFields.Any())
		{
			SetData(await DB.Classificator.GetClassifications(ClassificatorId, SelectedCustomFields.Select((ClassificatorCustomFieldContainer x) => x.CustomField), SelectedCustomFields.Select((ClassificatorCustomFieldContainer x) => x.ClassificatorCustomField.ValueFieldName), databasesIds, cancellationToken, delegate
			{
				ProgressWaitFormInvoker.StepProgressWaitForm();
			}));
		}
	}

	public void SetData(List<Dataedo.Model.Data.Classificator.Classification> data)
	{
		if (data == null || data.Count <= 0)
		{
			return;
		}
		SetDataSource(data);
		manager.GridView.RefreshData();
		manager.GridView.GridControl.DataSource = DataSource;
		foreach (GridColumn column in manager.GridView.Columns)
		{
			if (!(column.FieldName == "Description"))
			{
				column.BestFit();
				if (column.Width > 200)
				{
					column.Width = 200;
				}
			}
		}
		dataService.InitializeDistinctValuesDictionary(classificatorDistinctValues, SelectedCustomFields.Select((ClassificatorCustomFieldContainer x) => x.CustomField).ToList(), DataSource);
	}

	private void SetDataSource(IEnumerable<Dataedo.Model.Data.Classificator.Classification> data)
	{
		foreach (Dataedo.Model.Data.Classificator.Classification datum in data)
		{
			ClassificatorDataModel item = new ClassificatorDataModelBuilder(GetFieldName(1), GetFieldName(2), GetFieldName(3), GetFieldName(4), GetFieldName(5)).Build(datum, SelectedCustomFields.Count());
			DataSource.Add(item);
		}
	}

	private string GetFieldName(int i)
	{
		Guard.ArgumentIsInRange(1, 5, i, "Classification Custom Field number");
		if (SelectedCustomFields.Count() <= i - 1)
		{
			return string.Empty;
		}
		return SelectedCustomFields.ToArray()[i - 1].CustomField.FieldName;
	}

	private void SaveCustomFields()
	{
		ClassificatorCustomFieldsService classificatorCustomFieldsService = new ClassificatorCustomFieldsService(support);
		classificatorCustomFieldsService.LoadAllCustomFields();
		if (!DataSource.Any((ClassificatorDataModel x) => x.IsChecked))
		{
			return;
		}
		for (int i = 0; i < SelectedCustomFields.Count(); i++)
		{
			CustomFieldRow customField = SelectedCustomFields.ToArray()[i].CustomField;
			ClassificatorCustomField classificatorCustomField = SelectedCustomFields.ToArray()[i].ClassificatorCustomField;
			if (customField.CustomFieldId == 0)
			{
				classificatorCustomFieldsService.AddCustomField(customField);
				classificatorCustomField.Id = customField.CustomFieldId;
			}
		}
	}

	public void Save()
	{
		if (DataSource.TrueForAll((ClassificatorDataModel x) => !x.IsChecked))
		{
			GeneralMessageBoxesHandling.Show("There are no changes checked for saving.", "No changes checked", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, Owner);
			classificationSummaryUserControl.AfterSaving(savingSuccessful: false);
			return;
		}
		BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.WorkerSupportsCancellation = true;
		backgroundWorker.DoWork += BackgroundWorker_DoWork;
		backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
		backgroundWorker.RunWorkerAsync();
	}

	private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Cancelled || e?.Error?.InnerException is OperationCanceledException || e?.Error?.InnerException?.InnerException is OperationCanceledException)
		{
			ProgressWaitFormInvoker.CloseProgressWaitForm();
			ClassificationTrackingHelper.TrackClassificationSavingCanceled(savingObjectsCount);
			classificationSummaryUserControl.AfterSaving(savingSuccessful: false);
			GeneralMessageBoxesHandling.Show("Saving stopped successfully.", "Saving stopped", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, Owner);
		}
		else if (e.Error != null)
		{
			ProgressWaitFormInvoker.CloseProgressWaitForm();
			classificationSummaryUserControl.AfterSaving(savingSuccessful: false);
			GeneralExceptionHandling.Handle(e.Error, "Error while saving classified data:", Owner);
		}
		else
		{
			classificationSummaryUserControl.AfterSaving(savingSuccessful: true);
			GeneralMessageBoxesHandling.Show("Changes were successfully saved to the repository.", "Changes saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, Owner);
		}
		savingObjectsCount = 0;
	}

	private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
		IEnumerable<ClassificatorDataModel> enumerable = DataSource.Where((ClassificatorDataModel x) => x.IsChecked && x.AnythingToSave);
		savingObjectsCount = enumerable.Count();
		ClassificationTrackingHelper.TrackClassificationSave(savingObjectsCount);
		ProgressWaitFormSettings progressWaitFormSettings = new ProgressWaitFormSettings
		{
			FormTitle = "Saving classification in progress...",
			ProgressLabel = "Saving classification's custom fields...",
			Picture = Resources.cavemen,
			BackgroundWorker = backgroundWorker,
			ButtonText = "Stop",
			ButtonWarning = "Stopping the process means that objects already saved remain with new values."
		};
		ProgressWaitFormInvoker.ShowProgressWaitForm(enumerable.Count() + 2, Owner, progressWaitFormSettings);
		ProgressWaitFormInvoker.ChangeWaitFormLabel("Saving classification's custom fields...");
		SaveClassificationCustomFields();
		ProgressWaitFormInvoker.StepProgressWaitForm();
		backgroundWorker.ThrowIfCancellationPending();
		ProgressWaitFormInvoker.ChangeWaitFormLabel("Saving classification...");
		DB.Classificator.UpdateClassificatorCustomFields(enumerable, backgroundWorker);
		backgroundWorker.ThrowIfCancellationPending();
		ProgressWaitFormInvoker.ChangeWaitFormLabel("Updating custom fields...");
		IEnumerable<CustomFieldRow> enumerable2 = from x in SelectedCustomFields
			select x.CustomField into y
			where y.Type == CustomFieldTypeEnum.CustomFieldType.ListOpen && y.CustomFieldId > 0
			select y;
		if (support != null)
		{
			IEnumerable<CustomFieldRow> enumerable3 = enumerable2.Where((CustomFieldRow x) => support.Fields.Any((CustomFieldRowExtended f) => f.CustomFieldId != x.CustomFieldId && f.OrdinalPosition == x.OrdinalPosition));
			int num = support.Fields.Max((CustomFieldRowExtended x) => x.OrdinalPosition);
			using IEnumerator<CustomFieldRow> enumerator = enumerable3.GetEnumerator();
			while (enumerator.MoveNext())
			{
				num = (enumerator.Current.OrdinalPosition = num + 1);
			}
		}
		List<CustomField> convertedCustomFieldWithOpenGeneralTypeChange = CustomFieldDB.GetConvertedCustomFieldWithOpenGeneralTypeChange(enumerable2);
		DB.CustomField.UpdateCustomFields(convertedCustomFieldWithOpenGeneralTypeChange.ToArray(), rebuildDictionaryIfNecessary: true, null, !StaticData.IsProjectFile);
		ProgressWaitFormInvoker.StepProgressWaitForm();
		ProgressWaitFormInvoker.CloseProgressWaitForm();
	}

	private void SaveClassificationCustomFields()
	{
		SaveCustomFields();
		ClassificatorCustomFieldContainer[] array = SelectedCustomFields.ToArray();
		int? field1Id = null;
		if (SelectedCustomFields.Count() > 0 && array[0].CustomField.CustomFieldId > 0)
		{
			field1Id = array[0].CustomField.CustomFieldId;
		}
		int? field2Id = null;
		if (SelectedCustomFields.Count() > 1 && array[1].CustomField.CustomFieldId > 0)
		{
			field2Id = array[1].CustomField.CustomFieldId;
		}
		int? field3Id = null;
		if (SelectedCustomFields.Count() > 2 && array[2].CustomField.CustomFieldId > 0)
		{
			field3Id = array[2].CustomField.CustomFieldId;
		}
		int? field4Id = null;
		if (SelectedCustomFields.Count() > 3 && array[3].CustomField.CustomFieldId > 0)
		{
			field4Id = array[3].CustomField.CustomFieldId;
		}
		int? field5Id = null;
		if (SelectedCustomFields.Count() > 4 && array[4].CustomField.CustomFieldId > 0)
		{
			field5Id = array[4].CustomField.CustomFieldId;
		}
		DB.Classificator.UpdateClassification(ClassificatorId, field1Id, field2Id, field3Id, field4Id, field5Id);
	}
}
