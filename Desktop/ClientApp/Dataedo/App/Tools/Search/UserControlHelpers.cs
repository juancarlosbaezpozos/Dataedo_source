using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.Tools.UI.Skins.Base;
using Dataedo.App.UserControls;
using Dataedo.DataProcessing.Synchronize;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Dataedo.App.Tools.Search;

public class UserControlHelpers
{
	private int tabsCount;

	private bool[][][] rowHighlight;

	private string[] lastSearchWords;

	private List<CustomFieldSearchItem> lastCustomFieldSearchItems;

	private int? lastTabDataIndex;

	private GridView lastGridView;

	private XtraTabControl lastTabControl;

	private int lastTabPageIndex;

	private string lastIdColumn;

	private int?[] lastElementId;

	public bool IsSearchActive { get; set; }

	public UserControlHelpers(int tabsCount)
	{
		this.tabsCount = tabsCount;
		rowHighlight = new bool[tabsCount][][];
	}

	public string[] GetLastSearchWords()
	{
		return lastSearchWords;
	}

	public void HighlightRowCellStyle(int? tabIndex, object sender, RowCellCustomDrawEventArgs e)
	{
		if (tabIndex.HasValue && rowHighlight[tabIndex.Value] != null && e.RowHandle > -1 && e.RowHandle < rowHighlight[tabIndex.Value].Length && e.Column.AbsoluteIndex < rowHighlight[tabIndex.Value][e.RowHandle].Length && rowHighlight[tabIndex.Value][e.RowHandle][e.Column.AbsoluteIndex])
		{
			BulkCopyGridUserControl bulkCopyGridUserControl = sender as BulkCopyGridUserControl;
			if (bulkCopyGridUserControl.IsCellSelected(e.RowHandle, bulkCopyGridUserControl.Columns[e.Column.AbsoluteIndex]))
			{
				e.Appearance.BackColor = SearchSupport.HighlightColorUnderSelectionColorNormal;
				if (e.Appearance.ForeColor != SkinsManager.CurrentSkin.DeletedObjectForeColor)
				{
					e.Appearance.ForeColor = SearchSupport.HighlightColorUnderSelectionForeColorNormal;
				}
			}
			else
			{
				e.Appearance.BackColor = SearchSupport.HighlightColor;
				if (e.Appearance.ForeColor != SkinsManager.CurrentSkin.DeletedObjectForeColor)
				{
					e.Appearance.ForeColor = SearchSupport.HighlightForeColor;
				}
			}
		}
		else if (e.Appearance.BackColor == SearchSupport.HighlightColor)
		{
			e.Appearance.BackColor = SkinColors.ControlColorFromSystemColors;
		}
	}

	public void ClearHighlights(bool keepSearchActive, XtraTabControl tabControl, TextEdit schemaControl, BaseControl nameControl, TextEdit titleControl, List<CustomFieldControl> customFieldsControls = null)
	{
		IsSearchActive = keepSearchActive;
		rowHighlight = new bool[tabsCount][][];
		if (tabControl != null)
		{
			ClearTabHighlights(tabControl);
		}
		if (schemaControl != null)
		{
			schemaControl.BackColor = SkinColors.ControlColorFromSystemColors;
			schemaControl.ForeColor = SkinColors.ControlForeColorFromSystemColors;
		}
		if (nameControl != null)
		{
			nameControl.BackColor = SkinColors.ControlColorFromSystemColors;
			nameControl.ForeColor = SkinColors.ControlForeColorFromSystemColors;
		}
		if (titleControl != null)
		{
			titleControl.BackColor = SkinColors.InputBackColor;
			titleControl.ForeColor = SkinColors.ControlForeColorFromSystemColors;
		}
		if (customFieldsControls == null)
		{
			return;
		}
		foreach (CustomFieldControl customFieldsControl in customFieldsControls)
		{
			customFieldsControl.ClearSearchHighlight();
		}
	}

	public void ClearTabHighlights(XtraTabControl tabControl)
	{
		for (int i = 0; i < tabControl.TabPages.Count; i++)
		{
			tabControl.TabPages[i].Appearance.Header.ForeColor = SearchSupport.TabForeColorNotHighlighted;
		}
	}

	public void SetHighlight()
	{
		SetHighlight(lastSearchWords, lastCustomFieldSearchItems, lastTabDataIndex, lastGridView, lastTabControl, lastTabPageIndex, lastIdColumn, lastElementId);
	}

	public bool SetHighlight(string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, int? tabDataIndex, GridView gridView, XtraTabControl tabControl, int tabPageIndex, string idColumn, params int?[] elementId)
	{
		IsSearchActive = true;
		lastSearchWords = searchWords;
		lastCustomFieldSearchItems = customFieldSearchItems;
		lastTabDataIndex = tabDataIndex;
		lastGridView = gridView;
		lastTabControl = tabControl;
		lastTabPageIndex = tabPageIndex;
		lastIdColumn = idColumn;
		lastElementId = elementId;
		if (tabControl != null && tabDataIndex.HasValue)
		{
			HighlightTabPage(tabControl, tabPageIndex);
		}
		if (tabDataIndex.HasValue)
		{
			if (gridView != null)
			{
				rowHighlight[tabDataIndex.Value] = new bool[gridView.RowCount][];
				for (int i = 0; i < gridView.RowCount; i++)
				{
					rowHighlight[tabDataIndex.Value][i] = new bool[gridView.Columns.Count];
					BaseRow baseRow = gridView.GetRow(i) as BaseRow;
					if (baseRow != null && elementId.Any((int? o) => o.Equals(baseRow.Id)))
					{
						SetHightlightForColumn(searchWords, customFieldSearchItems, gridView, tabDataIndex.Value, i);
					}
				}
				return true;
			}
			rowHighlight[tabDataIndex.Value] = new bool[0][];
		}
		return false;
	}

	public void SetHighlight(ResultItem row, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, int? tabDataIndex, int tabPageIndex, XtraTabControl tabControl, TextEdit schemaControl, BaseControl nameControl, TextEdit titleControl, List<CustomFieldControl> customFieldsControls, HtmlUserControl htmlUserControl, GridView gridView, string idColumn, params int?[] elementId)
	{
		SetHighlight(row, searchWords, customFieldSearchItems, tabDataIndex, tabPageIndex, null, null, tabControl, schemaControl, nameControl, titleControl, customFieldsControls, htmlUserControl, null, null, gridView, idColumn, elementId);
	}

	public void SetTabsProgressHighlights(XtraTabControl tabControl, Dictionary<int, KeyValuePair<int, int>> keyValuePairs)
	{
		foreach (KeyValuePair<int, KeyValuePair<int, int>> keyValuePair in keyValuePairs)
		{
			if (keyValuePair.Value.Value > keyValuePair.Value.Key)
			{
				tabControl.TabPages[keyValuePair.Key].Appearance.Header.ForeColor = ProgressPainter.TabColor;
			}
		}
	}

	public void SetProgressHighlights(IEnumerable<CustomFieldControl> controls, string fieldName, XtraTabPage tabControl = null)
	{
		if (controls == null)
		{
			return;
		}
		foreach (CustomFieldControl control in controls)
		{
			if (control.FieldName.Equals(fieldName) && string.IsNullOrEmpty(control.Value) && !control.ContainsFocus)
			{
				control.ValueEditBackColor = ProgressPainter.Color;
			}
			else if (control.ValueEditBackColor.Equals(ProgressPainter.Color))
			{
				control.ValueEditBackColor = SkinColors.ControlColorFromSystemColors;
			}
			control.IsProgressPainterActive = control.FieldName.Equals(fieldName);
		}
	}

	public void SetHighlight(ResultItem row, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, int? tabDataIndex, int tabPageIndex, int? tabPageIndexForScript, int? tabPageIndexForSchema, XtraTabControl tabControl, TextEdit schemaControl, BaseControl nameControl, TextEdit titleControl, List<CustomFieldControl> customFieldsControls, HtmlUserControl htmlUserControl, RichEditUserControl scriptRichEditUserControl, RichEditUserControl schemaTextRichEditUserControl, GridView gridView, string idColumn, params int?[] elementId)
	{
		if (row.IsProperResult)
		{
			bool flag = SetHighlight(searchWords, customFieldSearchItems, tabDataIndex, gridView, tabControl, tabPageIndex, idColumn, elementId);
			if (customFieldsControls != null)
			{
				customFieldsControls.Where((CustomFieldControl x) => customFieldSearchItems.Any((CustomFieldSearchItem y) => y.CustomField.FieldName == x.FieldName));
				foreach (CustomFieldControl customFieldControl in customFieldsControls)
				{
					CustomFieldSearchItem customFieldSearchItem = customFieldSearchItems.FirstOrDefault((CustomFieldSearchItem x) => x.CustomField.FieldName == customFieldControl.FieldName);
					if (HasAllWords(searchWords, customFieldControl.Value) || (customFieldSearchItem != null && HasAllWords(customFieldSearchItem.SearchWords, customFieldControl.Value)))
					{
						BaseSkin.SetSearchHighlight(customFieldControl);
						flag = true;
					}
				}
			}
			bool flag2 = schemaControl != null && HasAllWords(searchWords, schemaControl.Text);
			bool flag3 = nameControl != null && HasAllWords(searchWords, nameControl.Text);
			if (flag2 && flag3)
			{
				BaseSkin.SetSearchHighlight(schemaControl);
				BaseSkin.SetSearchHighlight(nameControl);
				flag = true;
			}
			else
			{
				bool num = schemaControl != null && !string.IsNullOrEmpty(schemaControl.Text) && nameControl != null && HasAllWords(searchWords, schemaControl.Text + "." + nameControl.Text) && !HasAllWords(searchWords, ".");
				bool flag4 = nameControl != null && HasAllWords(searchWords, nameControl.Text);
				if (num || flag4)
				{
					bool flag5 = schemaControl != null && HasAllWords(searchWords, schemaControl.Text + ".");
					bool flag6 = HasAllWords(searchWords, "." + nameControl.Text);
					bool flag7 = schemaControl != null && HasAnyWord(searchWords, schemaControl.Text);
					bool flag8 = HasAnyWord(searchWords, nameControl.Text);
					bool flag9 = schemaControl != null && HasAnyWord(searchWords, schemaControl.Text + ".");
					bool flag10 = HasAnyWord(searchWords, "." + nameControl.Text);
					if ((flag2 || flag5) && (flag3 || flag6))
					{
						if (schemaControl != null)
						{
							BaseSkin.SetSearchHighlight(schemaControl);
						}
						BaseSkin.SetSearchHighlight(nameControl);
						flag = true;
					}
					else if (flag2 || flag5)
					{
						if (schemaControl != null)
						{
							BaseSkin.SetSearchHighlight(schemaControl);
						}
						flag = true;
					}
					else if (flag3 || flag6)
					{
						BaseSkin.SetSearchHighlight(nameControl);
						flag = true;
					}
					else if ((flag7 && flag8) || (!flag7 && !flag8 && !flag9 && !flag10))
					{
						if (schemaControl != null)
						{
							BaseSkin.SetSearchHighlight(schemaControl);
						}
						BaseSkin.SetSearchHighlight(nameControl);
						flag = true;
					}
					else
					{
						if (flag7 || flag9)
						{
							BaseSkin.SetSearchHighlight(schemaControl);
							flag = true;
						}
						if (flag8 || flag10)
						{
							BaseSkin.SetSearchHighlight(nameControl);
							flag = true;
						}
					}
				}
			}
			if (titleControl != null && HasAllWords(searchWords, titleControl.Text))
			{
				BaseSkin.SetSearchHighlight(titleControl);
				flag = true;
			}
			if (htmlUserControl != null)
			{
				flag = htmlUserControl.Highlight(searchWords) || flag;
			}
			if (flag)
			{
				HighlightTabPage(tabControl, tabPageIndex);
			}
			if (tabPageIndexForScript.HasValue && row.FoundInDefinition && scriptRichEditUserControl.Highlight(searchWords))
			{
				HighlightTabPage(tabControl, tabPageIndexForScript.Value);
			}
			if (tabPageIndexForSchema.HasValue && row.FoundInDefinition && schemaTextRichEditUserControl.Highlight(searchWords))
			{
				HighlightTabPage(tabControl, tabPageIndexForSchema.Value);
			}
		}
		else
		{
			htmlUserControl.OccurrencesCount = 0;
		}
	}

	private void HighlightTabPage(XtraTabControl tabControl, int tabPageIndex)
	{
		tabControl.TabPages[tabPageIndex].Appearance.Header.ForeColor = SearchSupport.TabForeColorHighlighted;
	}

	public void HiglightProgressTabPage(XtraTabControl tabControl, int tabPageIndex)
	{
		tabControl.TabPages[tabPageIndex].Appearance.Header.ForeColor = ProgressPainter.Color;
	}

	private void SetHightlightForColumn(string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, GridView gridView, int tabIndex, int rowIndex)
	{
		foreach (GridColumn column in gridView.Columns.Where((GridColumn c) => c.FieldName.ToLower() == "fullnameformatted" || c.FieldName.ToLower().Equals("name") || c.FieldName.ToLower() == "title" || c.FieldName.ToLower() == "description" || c.FieldName.ToLower().StartsWith("field")))
		{
			SetHightlightData(gridView, tabIndex, rowIndex, column, searchWords);
			if (column.FieldName.ToLower().StartsWith("field"))
			{
				string[] words = customFieldSearchItems.FirstOrDefault((CustomFieldSearchItem x) => x.CustomField.FieldName.ToLower() == column.FieldName.ToLower())?.SearchWords;
				SetHightlightData(gridView, tabIndex, rowIndex, column, words);
			}
		}
	}

	private void SetHightlightData(GridView gridView, int tabIndex, int rowIndex, GridColumn column, string[] words)
	{
		string empty = string.Empty;
		empty = ((!column.FieldName.Equals("FullNameFormatted")) ? (gridView.GetRowCellValue(rowIndex, column) as string) : (gridView.GetRow(rowIndex) as ColumnRow).Name);
		if (empty != null)
		{
			rowHighlight[tabIndex][rowIndex][column.AbsoluteIndex] = rowHighlight[tabIndex][rowIndex][column.AbsoluteIndex] || HasAllWords(words, empty.ToString());
		}
	}

	private bool HasAllWords(string[] searchWords, string input)
	{
		if (input != null && searchWords != null && searchWords.Length != 0)
		{
			foreach (string value in searchWords)
			{
				if (input.IndexOf(value, 0, StringComparison.InvariantCultureIgnoreCase) == -1)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private bool HasAnyWord(string[] searchWords, string input)
	{
		if (input != null && searchWords != null && searchWords.Length != 0)
		{
			foreach (string value in searchWords)
			{
				if (input.IndexOf(value, 0, StringComparison.InvariantCultureIgnoreCase) != -1)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}
}
