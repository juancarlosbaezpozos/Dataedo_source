using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dataedo.App.Forms.Helpers;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.Tools;
using Dataedo.App.UserControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.CustomFieldsBase;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.ImportDescriptions.Processing.DataToImport;

public abstract class ImportProcessorBase<T> : IImportProcessorBase where T : ImportDataModel, new()
{
	private LabelControl errorTextLabelControl;

	private LayoutControlItem errorInformationLayoutControlItem;

	private LabelControl warningTextLabelControl;

	private LayoutControlItem warningInformationLayoutControlItem;

	private LabelControl valuesTextLabelControl;

	private LayoutControlItem valuesInformationLayoutControlItem;

	private string kindOfObject;

	private Dictionary<string, string> warningsDuringImport = new Dictionary<string, string>();

	protected Dictionary<string, string> RegularGridColumns = new Dictionary<string, string>
	{
		{ "Schema", "Schema" },
		{ "Table name", "TableName" }
	};

	private List<string> StatusGridColumns = new List<string> { "Image", "TableTypeImage" };

	public List<T> Models { get; protected set; }

	public List<ImportDataModel> ModelsGeneral => Models.Cast<ImportDataModel>().ToList();

	public bool IsChanged { get; set; }

	protected int DatabaseId { get; set; }

	protected List<FieldDefinition> FieldDefinitions { get; set; }

	protected CustomGridUserControl DataGridView { get; set; }

	public IEnumerable<CustomFieldRowExtended> SelectedCustomFields => from x in FieldDefinitions
		where x.IsSelected && x is CustomFieldFieldDefinition
		select ((CustomFieldFieldDefinition)x).CustomField;

	public Dictionary<string, string> WarningsDuringImport => warningsDuringImport;

	protected ImportProcessorBase(int databaseId, List<FieldDefinition> fieldDefinitions, CustomGridUserControl dataGridView, string kindOfObject)
	{
		DatabaseId = databaseId;
		FieldDefinitions = fieldDefinitions;
		DataGridView = dataGridView;
		Models = new List<T>();
		this.kindOfObject = kindOfObject;
	}

	public void SetLabels(LabelControl errorTextLabelControl, LayoutControlItem errorInformationLayoutControlItem, LabelControl warningTextLabelControl, LayoutControlItem warningInformationLayoutControlItem, LabelControl valuesTextLabelControl, LayoutControlItem valuesInformationLayoutControlItem)
	{
		this.errorTextLabelControl = errorTextLabelControl;
		this.errorInformationLayoutControlItem = errorInformationLayoutControlItem;
		this.warningTextLabelControl = warningTextLabelControl;
		this.warningInformationLayoutControlItem = warningInformationLayoutControlItem;
		this.valuesTextLabelControl = valuesTextLabelControl;
		this.valuesInformationLayoutControlItem = valuesInformationLayoutControlItem;
	}

	public void PrepareColumns()
	{
		int visibleIndex = 0;
		foreach (string statusGridColumn in StatusGridColumns)
		{
			DataGridView.Columns.Add(GetStatusGridColumn(statusGridColumn, ref visibleIndex));
		}
		foreach (KeyValuePair<string, string> regularGridColumn in RegularGridColumns)
		{
			DataGridView.Columns.Add(GetRegularGridColumn(ref visibleIndex, regularGridColumn.Key, regularGridColumn.Value));
		}
		DataGridView.Columns.AddRange(GetFieldsGridColumns(ref visibleIndex));
		DataGridView.CustomColumnSort += DataGridView_CustomColumnSort;
	}

	private GridColumn GetStatusGridColumn(string fieldName, ref int visibleIndex)
	{
		GridColumn gridColumn = new GridColumn();
		gridColumn.Caption = " ";
		gridColumn.FieldName = fieldName;
		gridColumn.Name = "statusGridColumn";
		gridColumn.Visible = true;
		gridColumn.VisibleIndex = ++visibleIndex;
		gridColumn.Width = 20;
		gridColumn.SortMode = ColumnSortMode.Custom;
		gridColumn.OptionsColumn.AllowShowHide = false;
		gridColumn.OptionsColumn.AllowMove = false;
		gridColumn.OptionsColumn.AllowSort = DefaultBoolean.True;
		return gridColumn;
	}

	private GridColumn GetRegularGridColumn(ref int visibleIndex, string caption, string fieldName)
	{
		RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit
		{
			AutoHeight = false
		};
		LengthValidation.SetTitleOrNameLengthLimit(repositoryItemTextEdit);
		GridColumn gridColumn = new GridColumn();
		gridColumn.Caption = caption;
		gridColumn.ColumnEdit = repositoryItemTextEdit;
		gridColumn.FieldName = fieldName;
		gridColumn.Visible = true;
		gridColumn.VisibleIndex = ++visibleIndex;
		gridColumn.OptionsColumn.AllowShowHide = false;
		gridColumn.OptionsColumn.AllowMove = false;
		return gridColumn;
	}

	protected GridColumn[] GetFieldsGridColumns(ref int visibleIndex)
	{
		int internalVisibleIndex = visibleIndex;
		GridColumn[] result = (from x in FieldDefinitions
			where x.IsSelected
			select x.GetGridColumn(ref internalVisibleIndex)).ToArray();
		visibleIndex = internalVisibleIndex;
		return result;
	}

	private void DataGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column.FieldName == "Image")
		{
			e.Handled = true;
			e.Result = Comparer.Default.Compare((e.RowObject1 as ImportDataModel).Status, (e.RowObject2 as ImportDataModel).Status);
		}
		else if (e.Column.FieldName == "TableTypeImage")
		{
			e.Handled = true;
			e.Result = Comparer.Default.Compare((e.RowObject1 as ImportDataModel).TableObjectType, (e.RowObject2 as ImportDataModel).TableObjectType);
		}
	}

	public void AddRows(string data, BackgroundWorker backgroundWorker, DoWorkEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(data))
		{
			e.Result = false;
			return;
		}
		bool importedDataHasHeaders;
		Dictionary<string, int> importTranslator = ProcessHeaders(data, out importedDataHasHeaders);
		List<string> rowsFromRawData = GetRowsFromRawData(data);
		int num = (importedDataHasHeaders ? 1 : 0);
		OverlayWithProgress.SetProgressMax(rowsFromRawData.Count - num);
		OverlayWithProgress.SetSubtitle("Importing data rows");
		bool flag = false;
		for (int i = num; i < rowsFromRawData.Count; i++)
		{
			try
			{
				if (backgroundWorker.CancellationPending)
				{
					break;
				}
				if (string.IsNullOrWhiteSpace(rowsFromRawData[i]))
				{
					continue;
				}
				string[] array = (from x in rowsFromRawData[i].Split('\t')
					select PrepareValue.FixNewLineChars(x)?.Trim('"')).ToArray();
				if (!IsEmptyRow(array))
				{
					T val = PrepareRow(array, importTranslator);
					if (val != null)
					{
						Models.Add(val);
						flag = val != null || flag;
					}
				}
				continue;
			}
			catch
			{
				continue;
			}
			finally
			{
				OverlayWithProgress.UpdateStatusProgress();
			}
		}
		OverlayWithProgress.ClearProgress();
		SetExistingData(backgroundWorker);
		if (flag)
		{
			IsChanged = true;
			e.Result = true;
		}
		else
		{
			e.Result = false;
		}
	}

	private Dictionary<string, int> ProcessHeaders(string data, out bool importedDataHasHeaders)
	{
		IEnumerable<GridColumn> source = DataGridView.VisibleColumns.Where((GridColumn x) => !string.IsNullOrWhiteSpace(x.Caption));
		int num = source.Count();
		Dictionary<string, int> importTranslator = new Dictionary<string, int>();
		importedDataHasHeaders = false;
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		int num2 = data.IndexOf(Environment.NewLine);
		int num3;
		if (num2 < 0)
		{
			num3 = data.Count((char x) => x == '\t') + 1;
		}
		else
		{
			string[] array = data.Substring(0, num2).Trim('\r').Split('\t');
			num3 = array.Count();
			for (int j = 0; j < array.Count(); j++)
			{
				string header = array[j];
				if (string.IsNullOrWhiteSpace(header))
				{
					continue;
				}
				GridColumn gridColumn = source.Where((GridColumn x) => x.Caption == header).FirstOrDefault();
				if (gridColumn != null)
				{
					importedDataHasHeaders = true;
					if (importTranslator.ContainsKey(gridColumn.FieldName))
					{
						list2.Add(header);
					}
					else
					{
						importTranslator.Add(gridColumn.FieldName, j);
					}
				}
				else if (!list.Contains(header))
				{
					list.Add(header);
				}
			}
		}
		if (!importedDataHasHeaders)
		{
			int i = 0;
			source.Take(num3).ToList().ForEach(delegate(GridColumn x)
			{
				importTranslator.Add(x.FieldName, i++);
			});
			warningsDuringImport.Add("No headers detected", "The imported data does not include headers. It will be pasted in accordance to the order of the visible columns.");
			if (num3 > num)
			{
				warningsDuringImport.Add("Excessive columns", $"The imported data contains {num3} columns " + $"while there are only {num} expected. " + "The data from excessive columns will be ignored.");
			}
			else if (num3 < num)
			{
				int num4 = num - num3;
				IEnumerable<GridColumn> enumerable = source.Skip(source.Count() - num4);
				string text = "    - <i>";
				string text2 = "</i>" + Environment.NewLine;
				string text3 = $"The imported data contains {num3} columns " + $"while there are {num} expected.{Environment.NewLine}";
				text3 = ((num4 == 1) ? (text3 + $"The <i>{enumerable.First()}</i> column will contain empty data.{Environment.NewLine}") : ((num4 <= 15) ? (text3 + "The following columns will contain empty data:" + Environment.NewLine + text + string.Join(text2 + text, enumerable) + text2) : (text3 + $"The last {num4} columns will contain empty data.{Environment.NewLine}")));
				text3 += "Please verify the imported data.";
				warningsDuringImport.Add("Missing columns", text3);
			}
		}
		else
		{
			string text4 = "    - <i>";
			string text5 = "</i>" + Environment.NewLine;
			if (list.Any())
			{
				warningsDuringImport.Add("Unknown import headers", "The following imported headers were not recognized:" + Environment.NewLine + text4 + string.Join(text5 + text4, list) + text5 + "The data associated to them will be ignored.");
			}
			if (list2.Any())
			{
				warningsDuringImport.Add("Duplicate headers", "The imported data contains following duplicate headers:" + Environment.NewLine + text4 + string.Join(text5 + text4, list2) + text5 + "Only the data associated to the firstly encountered column will be imported, the rest will be ignored.");
			}
			List<GridColumn> list3 = source.Where((GridColumn x) => !importTranslator.Keys.Contains(x.FieldName)).ToList();
			if (list3.Any())
			{
				if (list3.Count() > 15)
				{
					warningsDuringImport.Add("Missing headers", $"{list3.Count()} headers were not included in the imported data " + "(" + string.Join(", ", list3) + ")." + Environment.NewLine + "The associated columns will contain empty data." + Environment.NewLine + "Please verify the imported data.");
				}
				else
				{
					warningsDuringImport.Add("Missing headers", "The following headers were not included in the imported data:" + Environment.NewLine + text4 + string.Join(text5 + text4, list3) + text5 + "The associated columns will contain empty data." + Environment.NewLine + "Please verify the imported data.");
				}
			}
		}
		return importTranslator;
	}

	private static List<string> GetRowsFromRawData(string data)
	{
		return data.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
	}

	private T PrepareRow(string[] rowData, Dictionary<string, int> importTranslator)
	{
		T val = new T();
		UpdateDescriptionTitleImported(val);
		val.Initialize();
		bool flag = false;
		foreach (KeyValuePair<string, int> item in importTranslator)
		{
			string key = item.Key;
			if (rowData.Length < item.Value)
			{
				break;
			}
			string text = rowData[item.Value];
			if (PrepareRegularColumnField(val, text, key))
			{
				flag = true;
				continue;
			}
			if (key == "Title.OverwriteValue")
			{
				val.SetTitle(text);
				flag = true;
				continue;
			}
			if (key == "Description.OverwriteValue")
			{
				val.SetDescription(text);
				flag = true;
				continue;
			}
			try
			{
				GridColumn gridColumn = DataGridView.Columns[key];
				int fieldColumnIndexForNestedProperty = CustomFieldsDataBase<string>.GetFieldColumnIndexForNestedProperty(key);
				CustomFieldRowExtended customFieldRowExtended = gridColumn?.Tag as CustomFieldRowExtended;
				if (customFieldRowExtended.IsDomainValueType)
				{
					if (customFieldRowExtended.IsValueProperForDomainValuesType(text))
					{
						text = customFieldRowExtended.GetPreparedValueForDomainValuesType(text)?.ToString();
					}
				}
				else if (customFieldRowExtended.Type != CustomFieldTypeEnum.CustomFieldType.Text)
				{
					text = text.Replace(Environment.NewLine, " ").Replace("\n", " ");
				}
				FieldData fieldData = new FieldData();
				fieldData.InitializeOverwriteValue(text);
				val.SetField(fieldColumnIndexForNestedProperty, fieldData, isActive: true);
				flag = true;
			}
			catch (ArgumentException)
			{
			}
		}
		if (flag)
		{
			IsChanged = true;
			return val;
		}
		return null;
	}

	protected virtual bool PrepareRegularColumnField(T rowModel, string fieldDataString, string columnFieldName)
	{
		if (columnFieldName == "Schema")
		{
			rowModel.Schema = ImportDataModel.ShortenedString(fieldDataString, 80);
			return true;
		}
		if (columnFieldName == "TableName")
		{
			rowModel.TableName = ImportDataModel.ShortenedString(fieldDataString, 80);
			return true;
		}
		return false;
	}

	public void RemoveSelectedRows()
	{
		List<T> list = new List<T>();
		DataGridView.CloseEditor();
		int[] selectedRows = DataGridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			T item = DataGridView.GetRow(rowHandle) as T;
			list.Add(item);
		}
		DataGridView.GridControl.BeginUpdate();
		list.ForEach(delegate(T x)
		{
			Models.Remove(x);
		});
		DataGridView.RefreshData();
		DataGridView.GridControl.EndUpdate();
		if (DataGridView.FocusedRowHandle >= Models.Count - 1)
		{
			DataGridView.MoveLast();
		}
		if (Models.Count == 0)
		{
			IsChanged = false;
		}
		CheckRows();
	}

	public void RemoveAllRows()
	{
		DataGridView.GridControl.BeginUpdate();
		Models.Clear();
		DataGridView.GridControl.EndUpdate();
		IsChanged = false;
		CheckRows();
	}

	public void CheckRows()
	{
		DataGridView.GridControl.BeginUpdate();
		SetExistingMessage();
		CheckDuplicates();
		CheckIfValuesCorrect();
		DataGridView.GridControl.EndUpdate();
	}

	public void CheckValueChanged(object row, string fieldName)
	{
		DataGridView.BeginDataUpdate();
		DataGridView.GridControl.BeginUpdate();
		if (ShouldCheckExisting(fieldName))
		{
			SetExistingData(row as T);
			SetExistingMessage();
			CheckDuplicates();
		}
		CheckIfValuesCorrect(row as T);
		RefreshIncorrectValuesInfo();
		DataGridView.GridControl.EndUpdate();
		DataGridView.EndDataUpdate();
	}

	public abstract string GetTooltipString(ImportDataModel row, string fieldName);

	protected string GetTooltipStringBase(ImportDataModel row, string fieldName, string objectType)
	{
		if (row == null)
		{
			return null;
		}
		if (fieldName == "Image")
		{
			if (!row.Exists)
			{
				return objectType + " not found";
			}
			if (row.IsDuplicated)
			{
				return objectType + " is duplicated";
			}
			if (row.IncorrectValues)
			{
				return objectType + " contains incorrect values";
			}
		}
		return null;
	}

	protected abstract bool ShouldCheckExisting(string fieldName);

	protected abstract void SetExistingData(T model);

	protected abstract void SetExistingData(BackgroundWorker backgroundWorker);

	protected void SetExistingMessage()
	{
		int num = Models.Where((T x) => !x.Exists).Count();
		if (num > 0)
		{
			errorTextLabelControl.Text = $"Could not identify {num} " + ((num > 1) ? (kindOfObject + "s") : kindOfObject);
			errorInformationLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else
		{
			errorInformationLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	protected void CheckDuplicates()
	{
		IEnumerable<IGrouping<int?, T>> enumerable = from x in Models
			group x by x.IdProperty into x
			where x.Key.HasValue && x.Count() > 1
			select x;
		Models.ForEach(delegate(T x)
		{
			x.IsDuplicated = false;
		});
		foreach (IGrouping<int?, T> item in enumerable)
		{
			foreach (T item2 in item)
			{
				item2.IsDuplicated = true;
			}
		}
		int num = enumerable.Count();
		if (num > 0)
		{
			warningTextLabelControl.Text = $"Found {num} duplicate " + ((num > 1) ? (kindOfObject + "s") : kindOfObject);
			warningInformationLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else
		{
			warningInformationLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	protected void CheckIfValuesCorrect()
	{
		Models.ForEach(delegate(T x)
		{
			CheckIfValuesCorrect(x);
		});
		RefreshIncorrectValuesInfo();
	}

	protected void CheckIfValuesCorrect(T model)
	{
		model.ValidateCustomFieldsValues(SelectedCustomFields);
	}

	private void RefreshIncorrectValuesInfo()
	{
		int num = Models.Where((T x) => x.IncorrectValues).Count();
		if (num > 0)
		{
			valuesTextLabelControl.Text = $"Incorrect values in {num} " + ((num > 1) ? (kindOfObject + "s") : kindOfObject);
			valuesInformationLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else
		{
			valuesInformationLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	protected bool IsEmptyRow(string[] data)
	{
		return data.ToList().TrueForAll((string x) => string.IsNullOrWhiteSpace(x) || x.Equals("Unchecked"));
	}

	public bool CheckIfDataValid()
	{
		if (Models.Any())
		{
			return Models.All((T x) => x.IsValid);
		}
		return false;
	}

	public void UnselectField(FieldDefinition fieldDefinition)
	{
		foreach (T model in Models)
		{
			if (fieldDefinition is CustomFieldFieldDefinition)
			{
				model.RemoveCustomFields((fieldDefinition as CustomFieldFieldDefinition).CustomField);
			}
			else
			{
				UpdateDescriptionTitleImported(model);
			}
		}
	}

	protected void UpdateDescriptionTitleImported(T rowModel)
	{
		FieldDefinition fieldDefinition = FieldDefinitions.Where((FieldDefinition x) => x.FieldName == "Description").FirstOrDefault();
		rowModel.Description.IsImported = fieldDefinition?.IsSelected ?? false;
		FieldDefinition fieldDefinition2 = FieldDefinitions.Where((FieldDefinition x) => x.FieldName == "Title").FirstOrDefault();
		rowModel.Title.IsImported = fieldDefinition2?.IsSelected ?? false;
	}
}
