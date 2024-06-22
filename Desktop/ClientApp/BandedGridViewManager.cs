using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dataedo.App.Tools.ClassificationSummary;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Pannels;
using Dataedo.DataProcessing.Classificator;
using Dataedo.DataProcessing.CustomFields;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;

public class BandedGridViewManager
{
    private BandedGridCreator creator;

    private CustomFieldsSupport support;

    private Dictionary<string, IList<string>> classificatorDistinctValues;

    private IEnumerable<ClassificatorCustomFieldContainer> selectedCustomFields;

    public BandedGridView GridView { get; set; }

    public BandedGridViewManager()
    {
        creator = new ClassificatorBandedGridCreator();
    }

    public BandedGridViewManager(BandedGridView gridView)
    {
        creator = new BandedGridCreator();
        GridView = gridView;
    }

    public void CreateCustomFieldBand(string title, ref int index)
    {
        GridBand band = creator.GetBand(title);
        band.AppearanceHeader.Options.UseTextOptions = true;
        band.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
        string tag = "IsChecked";
        BandedGridColumn readonlyColumn = creator.GetReadonlyColumn("Current value", $"Field{index}Value");
        readonlyColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
        readonlyColumn.OptionsColumn.AllowEdit = false;
        readonlyColumn.OptionsColumn.ReadOnly = true;
        readonlyColumn.Tag = tag;
        readonlyColumn.Width = 80;
        readonlyColumn.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
        BandedGridColumn column = creator.GetColumn("Update to", $"Field{index}Update");
        column.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
        column.Tag = tag;
        column.Width = 80;
        column.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
        column.ColumnEdit = new RepositoryItemMRUEdit();
        GridView.Columns.Add(readonlyColumn);
        GridView.Columns.Add(column);
        band.Columns.Add(readonlyColumn);
        band.Columns.Add(column);
        index++;
    }

    public void SetParameters(BandedGridView gridView, CustomFieldsSupport support, Dictionary<string, IList<string>> classificatorDistinctValues, IEnumerable<ClassificatorCustomFieldContainer> selectedCustomFields)
    {
        GridView = gridView;
        this.support = support;
        this.classificatorDistinctValues = classificatorDistinctValues;
        this.selectedCustomFields = selectedCustomFields;
        GridView.CustomRowCellEditForEditing += SetRepositoryItem;
        GridView.Bands[0].Caption = string.Empty;
        int index = 1;
        GridView.Bands[0].Caption = "Columns found by " + this.selectedCustomFields?.FirstOrDefault()?.ClassificatorTitle;
        foreach (ClassificatorCustomFieldContainer selectedCustomField in this.selectedCustomFields)
        {
            CreateCustomFieldBand(selectedCustomField.CustomField.Title ?? "", ref index);
        }
    }

    private void SetRepositoryItem(object sender, CustomRowCellEditEventArgs e)
    {
        if (!e.Column.FieldName.Equals("Field1Update") && !e.Column.FieldName.Equals("Field2Update") && !e.Column.FieldName.Equals("Field3Update") && !e.Column.FieldName.Equals("Field4Update") && !e.Column.FieldName.Equals("Field5Update"))
        {
            return;
        }
        string customFieldName = (GridView.FocusedColumn as BandedGridColumn).OwnerBand.Tag.ToString();
        CustomFieldRowExtended customFieldRowExtended = support.Fields.FirstOrDefault((CustomFieldRowExtended x) => x.Title.Equals(customFieldName));
        RepositoryItemMRUEdit edit = CustomFieldsRepositoryItems.GetClassificatorOpenListRepositoryItem(customFieldRowExtended, GridView);
        if (customFieldRowExtended == null)
        {
            edit.Leave += delegate
            {
                if (GridView.FocusedRowHandle >= 0 && GridView.FocusedValue != null && !classificatorDistinctValues[customFieldName].Any((string x) => x.Equals(GridView.EditingValue) && !edit.Items.Contains(GridView.FocusedValue)))
                {
                    if (GridView.EditingValue != null)
                    {
                        classificatorDistinctValues[customFieldName].Add(GridView.EditingValue.ToString());
                    }
                    else
                    {
                        classificatorDistinctValues[customFieldName].Add(GridView.FocusedValue.ToString());
                    }
                }
            };
        }
        edit.EditValueChanged += EditValueChanged;
        if (classificatorDistinctValues.ContainsKey(customFieldName))
        {
            MRUEditItemCollection items = edit.Items;
            object[] items2 = classificatorDistinctValues[customFieldName].OrderBy((string x) => x).ToArray();
            items.AddRange(items2);
        }
        e.RepositoryItem = edit;
    }

    private void EditValueChanged(object sender, EventArgs e)
    {
        if (GridView.FocusedRowHandle >= 0)
        {
            ClassificatorDataModel classificatorDataModel = GridView.GetRow(GridView.FocusedRowHandle) as ClassificatorDataModel;
            PropertyInfo property = classificatorDataModel.GetType().GetProperty(GridView.FocusedColumn.Tag.ToString());
            property.SetValue(classificatorDataModel, Convert.ChangeType(true, property.PropertyType), null);
        }
    }
}
