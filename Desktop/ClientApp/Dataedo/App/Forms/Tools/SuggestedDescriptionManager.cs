using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.SuggestedDescription;
using Dataedo.Shared.Enums;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Forms.Tools;

public class SuggestedDescriptionManager
{
	private readonly bool contextShowSchema;

	private readonly int objectId;

	private readonly List<string> fieldNames;

	private readonly string tableName;

	private readonly string objectIdName;

	private readonly GridView gridView;

	private readonly SharedObjectTypeEnum.ObjectType objectType;

	private CancellationTokenSource token;

	public List<BaseRow> Rows { get; set; }

	public Task Task { get; set; }

	public Action Redraw { get; set; }

	public SuggestedDescriptionManager(bool contextShowSchema, int objectId, List<string> fieldNames, string tableName, string objectIdName, List<BaseRow> rows, GridView gridView, SharedObjectTypeEnum.ObjectType objectType, CancellationTokenSource token)
	{
		this.contextShowSchema = contextShowSchema;
		this.objectId = objectId;
		this.fieldNames = fieldNames;
		this.tableName = tableName;
		this.objectIdName = objectIdName;
		Rows = rows;
		this.gridView = gridView;
		this.objectType = objectType;
		this.token = new CancellationTokenSource();
	}

	public void GetSuggestedDescriptions()
	{
		TaskFactory taskFactory = new TaskFactory();
		Task = taskFactory.StartNew(delegate
		{
			if (!token.Token.IsCancellationRequested)
			{
				foreach (string fieldName in fieldNames)
				{
					List<SuggestedDescriptionContainer> suggestedDescriptionsByColumnName = DB.Table.GetSuggestedDescriptionsByColumnName(objectId, fieldName, tableName, objectIdName, GetJoinTableName(), GetJoinColumnName());
					if (token.Token.IsCancellationRequested)
					{
						break;
					}
					AddDescriptions(fieldName, suggestedDescriptionsByColumnName);
					GridView obj = gridView;
					if (obj != null && obj.GridControl?.IsHandleCreated == true)
					{
						gridView.GridControl.Invoke((Action)delegate
						{
							Redraw?.Invoke();
						});
					}
				}
			}
		});
	}

	public void Cancel()
	{
		token.Cancel();
	}

	private string GetJoinTableName()
	{
		if (objectType != SharedObjectTypeEnum.ObjectType.Table && objectType != SharedObjectTypeEnum.ObjectType.View && objectType != SharedObjectTypeEnum.ObjectType.Structure)
		{
			return "procedures";
		}
		return "tables";
	}

	private string GetJoinColumnName()
	{
		if (objectType != SharedObjectTypeEnum.ObjectType.Table && objectType != SharedObjectTypeEnum.ObjectType.View && objectType != SharedObjectTypeEnum.ObjectType.Structure)
		{
			return "procedure_id";
		}
		return "table_id";
	}

	private void AddDescriptions(string fieldName, List<SuggestedDescriptionContainer> result)
	{
		if (Rows == null)
		{
			return;
		}
		foreach (BaseRow row in Rows)
		{
			if (!row.SuggestedDescriptions.ContainsKey(fieldName))
			{
				row.SuggestedDescriptions.Add(fieldName, new List<SuggestedDescriptionContainer>());
			}
			AddDescription(fieldName, result, row);
		}
	}

	private void AddDescription(string fieldName, List<SuggestedDescriptionContainer> result, BaseRow row)
	{
		foreach (SuggestedDescriptionContainer item in result)
		{
			if (item.ColumnName.ToLower().Equals(row.Name.ToLower()) && !string.IsNullOrEmpty(item.Description))
			{
				row.SuggestedDescriptions[fieldName].Add(item);
			}
		}
	}

	public void CreateSuggestedDescriptionContextMenuItems(PopupMenu popupMenu, bool isSeparatorDrawn = true, bool showHints = false, bool forceJoin = false)
	{
		BaseRow baseRow = gridView.GetFocusedRow() as BaseRow;
		List<BarButtonItem> items = new List<BarButtonItem>();
		if (showHints && baseRow != null && baseRow.SuggestedDescriptions.ContainsKey(gridView.FocusedColumn.FieldName.ToLower()))
		{
			BarItemLinkCollection itemLinks = popupMenu.ItemLinks;
			BarItem[] items2 = CreateItems(baseRow, items);
			itemLinks.AddRange(items2);
			if (!forceJoin)
			{
				AddSeparator(popupMenu, isSeparatorDrawn, items);
			}
		}
		if ((forceJoin && popupMenu.ItemLinks.Count == 0) || (popupMenu.ItemLinks.Count == 0 && !isSeparatorDrawn))
		{
			BarButtonItem barButtonItem = new BarButtonItem();
			barButtonItem.Caption = (showHints ? "(No suggestions)" : "(Please enable hints in the ribbon)");
			barButtonItem.Enabled = false;
			popupMenu.ItemLinks.Add(barButtonItem);
		}
	}

	private BarButtonItem[] CreateItems(BaseRow column, List<BarButtonItem> items)
	{
		column.SuggestedDescriptions[gridView.FocusedColumn.FieldName.ToLower()].ForEach(delegate(SuggestedDescriptionContainer x)
		{
			object focusedRowCellValue = gridView.GetFocusedRowCellValue(gridView.FocusedColumn);
			if (focusedRowCellValue == null || !focusedRowCellValue.Equals(x.Description))
			{
				items.Add(CreateBarButtonItem(x));
			}
		});
		return items.ToArray();
	}

	private BarButtonItem CreateBarButtonItem(SuggestedDescriptionContainer x)
	{
		string shortenedDisplayValue = MultilineStringHelper.GetShortenedDisplayValue(x.Description);
		BarButtonItem barButtonItem = new BarButtonItem();
		bool showSchema = DatabaseRow.GetShowSchema(x.ShowSchema, x.ShowSchemaOverride, contextShowSchema);
		string text = ((x.Count == 1 && !string.IsNullOrEmpty(x.CommonTableName)) ? ((showSchema && !string.IsNullOrEmpty(x.ObjectSchema)) ? (x.ObjectSchema + "." + x.ObjectName) : x.ObjectName) : $"({x.Count} other {GetPluralOrSingularObjectName(x)})");
		barButtonItem.Caption = shortenedDisplayValue ?? "";
		barButtonItem.Tag = x.Description;
		barButtonItem.ItemClick += ContextMenuItem_Click;
		barButtonItem.ShortcutKeyDisplayString = text;
		if (!x.Description.Equals(shortenedDisplayValue))
		{
			barButtonItem.Hint = text;
		}
		return barButtonItem;
	}

	private void AddSeparator(PopupMenu contextMenu, bool isSeparatorDrawn, List<BarButtonItem> items)
	{
		if (items.Count > 0 && !isSeparatorDrawn)
		{
			items.LastOrDefault().Links[0].BeginGroup = true;
			contextMenu.ItemLinks.LastOrDefault().BeginGroup = true;
		}
	}

	private string GetPluralOrSingularObjectName(SuggestedDescriptionContainer x)
	{
		if (x.Count <= 1)
		{
			return SharedObjectTypeEnum.TypeToString(objectType).ToLower();
		}
		return SharedObjectTypeEnum.TypeToString(objectType).ToLower() + "s";
	}

	private void ContextMenuItem_Click(object sender, ItemClickEventArgs e)
	{
		gridView.ShowEditor();
		gridView.ActiveEditor.EditValue = e.Item.Tag.ToString();
	}

	private void ContextMenuItem_Click(object sender, EventArgs e)
	{
		gridView.ShowEditor();
		gridView.ActiveEditor.EditValue = (sender as ToolStripMenuItem).Tag.ToString();
	}
}
