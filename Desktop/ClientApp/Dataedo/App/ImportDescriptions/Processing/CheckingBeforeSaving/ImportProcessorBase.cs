using System.Collections.Generic;
using System.Linq;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using DevExpress.DataProcessing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace Dataedo.App.ImportDescriptions.Processing.CheckingBeforeSaving;

public abstract class ImportProcessorBase<T> : IImportProcessorBase where T : ImportDataModel
{
	protected Dictionary<string, string> RegularGridColumns = new Dictionary<string, string>
	{
		{ "Schema", "Schema" },
		{ "Table name", "TableName" }
	};

	private List<FieldDefinition> FieldDefinitions { get; }

	private BandedGridView DataGridView { get; set; }

	private RepositoryItemCheckEdit RepositoryItemCheckEdit { get; set; }

	protected ImportProcessorBase(List<FieldDefinition> fieldDefinitions, BandedGridView dataGridView, RepositoryItemCheckEdit repositoryItemCheckEdit)
	{
		FieldDefinitions = fieldDefinitions;
		DataGridView = dataGridView;
		RepositoryItemCheckEdit = repositoryItemCheckEdit;
	}

	public void PrepareColumns()
	{
		int visibleIndex = 0;
		GridBand gridBand = DataGridView.Bands.AddBand("");
		gridBand.OptionsBand.AllowMove = false;
		foreach (KeyValuePair<string, string> regularGridColumn in RegularGridColumns)
		{
			gridBand.Columns.Add(GetRegularGridColumn(ref visibleIndex, regularGridColumn.Key, regularGridColumn.Value));
		}
		SetFieldsGridColumns(ref visibleIndex);
	}

	private void SetFieldsGridColumns(ref int visibleIndex)
	{
		int internalVisibleIndex = visibleIndex;
		FieldDefinitions.Where((FieldDefinition x) => x.IsSelected).ForEach(delegate(FieldDefinition x)
		{
			GridBand band = DataGridView.Bands.AddBand(x.DisplayName);
			BandedGridColumn bandedGridColumn = new BandedGridColumn
			{
				Caption = "Current value",
				FieldName = x.FieldName + ".CurrentValue",
				Visible = true,
				VisibleIndex = ++internalVisibleIndex,
				Tag = (x as CustomFieldFieldDefinition)?.CustomField
			};
			bandedGridColumn.ColumnEdit = x.GetEdit();
			bandedGridColumn.OptionsColumn.ReadOnly = true;
			bandedGridColumn.OptionsColumn.AllowShowHide = false;
			bandedGridColumn.OptionsColumn.AllowMove = false;
			BandedGridColumn bandedGridColumn2 = new BandedGridColumn
			{
				Caption = "Update to",
				FieldName = x.FieldName + ".OverwriteValue",
				Visible = true,
				VisibleIndex = ++internalVisibleIndex,
				Tag = (x as CustomFieldFieldDefinition)?.CustomField
			};
			bandedGridColumn2.ColumnEdit = x.GetEdit();
			bandedGridColumn2.OptionsColumn.AllowShowHide = false;
			bandedGridColumn2.OptionsColumn.AllowMove = false;
			new BandedGridColumn[4]
			{
				GetCheckEditGridColumn(ref internalVisibleIndex, x.FieldName + ".IsSelected"),
				GetRegularGridColumn(ref internalVisibleIndex, "Change", x.FieldName + ".ChangeDescription"),
				bandedGridColumn,
				bandedGridColumn2
			}.ForEach(delegate(BandedGridColumn y)
			{
				band.Columns.Add(y);
			});
		});
		visibleIndex = internalVisibleIndex;
	}

	private BandedGridColumn GetRegularGridColumn(ref int visibleIndex, string caption, string fieldName)
	{
		BandedGridColumn bandedGridColumn = new BandedGridColumn();
		bandedGridColumn.Caption = caption;
		bandedGridColumn.FieldName = fieldName;
		bandedGridColumn.Visible = true;
		bandedGridColumn.VisibleIndex = ++visibleIndex;
		bandedGridColumn.OptionsColumn.ReadOnly = true;
		bandedGridColumn.OptionsColumn.AllowShowHide = false;
		bandedGridColumn.OptionsColumn.AllowMove = false;
		return bandedGridColumn;
	}

	private BandedGridColumn GetCheckEditGridColumn(ref int visibleIndex, string fieldName)
	{
		BandedGridColumn bandedGridColumn = new BandedGridColumn();
		bandedGridColumn.Caption = "Apply";
		bandedGridColumn.ColumnEdit = RepositoryItemCheckEdit;
		bandedGridColumn.FieldName = fieldName;
		bandedGridColumn.Visible = true;
		bandedGridColumn.VisibleIndex = ++visibleIndex;
		bandedGridColumn.OptionsColumn.AllowShowHide = false;
		bandedGridColumn.OptionsColumn.AllowMove = false;
		return bandedGridColumn;
	}
}
