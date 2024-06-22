using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.OverriddenControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Shared.Enums;
using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Layout;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraDiagram;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageDiagramUserControl : UserControl
{
	private const float ItemWidth = 250f;

	private const float ItemHeight = 100f;

	private const float ProcessItemWidth = 325f;

	private const float SpaceBetweenItems = 30f;

	private const float DiagramOffset = 250f;

	private const int BorderSize = 3;

	private const int ImageFieldWidth = 75;

	private const float ColumnItemHeight = 30f;

	private float lowestInflowColumnContainerPositionY;

	private float lowestOutflowColumnContainerPositionY;

	private bool isCtrlPressed;

	private bool showColumns;

	private DataFlowRow currentTableDataFlowRowForColumns;

	private MetadataEditorUserControl metadataEditorUserControl;

	public const float ContainerPadding = 20f;

	public const float InflowPositionX = 0f;

	public const float ProcessPositionX = 600f;

	public const float OutflowPositionX = 1200f;

	public static readonly Font ItemFont = new Font("Tahoma", 13f);

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup Root;

	private DiagramControlWithTransparentFocusedBorder diagramControl;

	private LayoutControlItem layoutControlItem1;

	private PopupMenu popupMenu;

	private BarButtonItem goToBarButtonItem;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	public DBTreeNode CurrentObjectNode { get; private set; }

	public List<DataLineageDiagramColumn> Columns { get; private set; }

	public List<DiagramConnector> HighlightedConnections { get; private set; }

	public List<DiagramItem> HighlightedDiagramItems { get; private set; }

	public DataLineageDiagramUserControl()
	{
		InitializeComponent();
	}

	internal void SetParameters(MetadataEditorUserControl metadataEditorUserControl, List<DataFlowRow> inflowRows, List<DataFlowRow> outflowRows, DBTreeNode currentObjectNode = null, bool drawWithColumns = false)
	{
		this.metadataEditorUserControl = metadataEditorUserControl;
		showColumns = drawWithColumns;
		CurrentObjectNode = currentObjectNode;
		if (inflowRows == null || outflowRows == null || CurrentObjectNode == null)
		{
			return;
		}
		diagramControl.BeginUpdate();
		diagramControl.Items?.Clear();
		diagramControl.OptionsBehavior.ActiveTool = diagramControl.OptionsBehavior.PanTool;
		if (showColumns)
		{
			if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				GenerateTableColumnsDiagram(currentObjectNode);
			}
			else
			{
				DrawColumnDiagram(inflowRows, outflowRows);
			}
		}
		else
		{
			DrawFlowDiagram(inflowRows, outflowRows);
		}
		diagramControl.UpdateRoute();
		diagramControl.FitToItems(diagramControl.Items);
		if (diagramControl.Items.Count < 3)
		{
			diagramControl.OptionsView.ZoomFactor = 0.75f;
		}
		else
		{
			diagramControl.OptionsView.ZoomFactor *= 0.95f;
		}
		diagramControl.EndUpdate();
	}

	private void GenerateTableColumnsDiagram(DBTreeNode currentObjectNode)
	{
		if (currentObjectNode == null)
		{
			return;
		}
		List<ColumnDataLineageResult> columnDataLineageResultsForTable = DB.DataProcess.GetColumnDataLineageResultsForTable(currentObjectNode.Id, base.ParentForm);
		if (columnDataLineageResultsForTable != null)
		{
			IEnumerable<ColumnDataLineageResult> columnDataLineageResults = columnDataLineageResultsForTable.Where((ColumnDataLineageResult c) => c.OutflowObjectId == currentObjectNode.Id);
			IEnumerable<ColumnDataLineageResult> columnDataLineageResults2 = columnDataLineageResultsForTable.Where((ColumnDataLineageResult c) => c.InflowObjectId == currentObjectNode.Id);
			DataProcessRow dataProcessRow = new DataProcessRow();
			dataProcessRow.Id = -1;
			currentTableDataFlowRowForColumns = null;
			lowestInflowColumnContainerPositionY = 0f;
			lowestOutflowColumnContainerPositionY = 0f;
			List<DataFlowRow> list = LineageDiagramHelper.GenerateFlowsToDrawTableColumns(columnDataLineageResults, dataProcessRow, areInflows: true, ref currentTableDataFlowRowForColumns, CurrentObjectNode);
			list.ForEach(delegate(DataFlowRow x)
			{
				x.ProcessId = dataProcessRow.Id;
			});
			if (currentTableDataFlowRowForColumns != null)
			{
				currentTableDataFlowRowForColumns.ProcessId = dataProcessRow.Id;
				DrawColumnDiagram(list, new List<DataFlowRow> { currentTableDataFlowRowForColumns }, 0f, 600f, clearColumns: true, isTableColumnDiagram: true);
			}
			List<DataFlowRow> list2 = LineageDiagramHelper.GenerateFlowsToDrawTableColumns(columnDataLineageResults2, dataProcessRow, areInflows: false, ref currentTableDataFlowRowForColumns, CurrentObjectNode);
			list2.ForEach(delegate(DataFlowRow x)
			{
				x.ProcessId = dataProcessRow.Id;
			});
			lowestInflowColumnContainerPositionY = 0f;
			lowestOutflowColumnContainerPositionY = 0f;
			if (currentTableDataFlowRowForColumns != null)
			{
				currentTableDataFlowRowForColumns.ProcessId = dataProcessRow.Id;
			}
			DrawColumnDiagram(new List<DataFlowRow> { currentTableDataFlowRowForColumns }, list2, 600f, 1200f, clearColumns: false, isTableColumnDiagram: true);
			LineageDiagramHelper.GenerateTableColumnsConnections(diagramControl, currentTableDataFlowRowForColumns, Columns);
		}
	}

	private void DrawColumnDiagram(List<DataFlowRow> inflowRows, List<DataFlowRow> outflowRows, float inflowPositionX = 0f, float outflowPositionX = 1200f, bool clearColumns = true, bool isTableColumnDiagram = false)
	{
		int num = 0;
		lowestInflowColumnContainerPositionY = 0f;
		lowestOutflowColumnContainerPositionY = 0f;
		if (clearColumns)
		{
			Columns = new List<DataLineageDiagramColumn>();
		}
		IEnumerable<int> enumerable = new List<int>();
		if (inflowRows.Any())
		{
			enumerable = (from x in inflowRows
				where x != null
				select x.ProcessId).Distinct();
		}
		if (outflowRows.Any())
		{
			enumerable = enumerable.Concat(from x in outflowRows
				where x != null
				select x.ProcessId).Distinct();
		}
		foreach (int processId in enumerable)
		{
			List<DataFlowRow> flows = inflowRows.Where((DataFlowRow x) => x.ProcessId == processId).ToList();
			List<DataFlowRow> flows2 = outflowRows.Where((DataFlowRow x) => x.ProcessId == processId).ToList();
			bool flag = DrawColumnsDiagramElements(inflowPositionX, flows, areInflows: true, num, isTableColumnDiagram);
			bool flag2 = DrawColumnsDiagramElements(outflowPositionX, flows2, areInflows: false, num, isTableColumnDiagram);
			if (!isTableColumnDiagram && (flag || flag2))
			{
				num++;
			}
		}
		if (!isTableColumnDiagram)
		{
			SetColumnsConnections();
		}
		UpdateDiagramsOffset();
		if (diagramControl.Items.Count == 0)
		{
			string text = "Please define column-level flows first.";
			if (CurrentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || CurrentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				text = "Please go to the processing object (procedure, function, etc.) and define column-level flows first.";
			}
			diagramControl.Items.Add(LineageDiagramHelper.DrawEmptyDiagramInfo(text, 600f, 325f, 3f, 100f, ItemFont));
		}
	}

	private void DrawFlowDiagram(List<DataFlowRow> inflowRows, List<DataFlowRow> outflowRows)
	{
		int num = 0;
		IEnumerable<DataFlowRow> source = inflowRows.Concat(outflowRows);
		if (!source.Any())
		{
			diagramControl.Items.Add(LineageDiagramHelper.DrawEmptyDiagramInfo("Please define flows first.", 600f, 325f, 3f, 100f, ItemFont));
			return;
		}
		if ((from x in source
			where !x.IsProcessInSameProcessor
			select x.ProcessId).Distinct().Any())
		{
			DataLineageDiagramContainer dataLineageDiagramContainer = LineageDiagramHelper.CreateTreeNodeDiagramItem(CurrentObjectNode, num, 600f, 250f, 75f, 100f, 3f, ItemFont);
			diagramControl.Items.Add(dataLineageDiagramContainer);
			DrawProcessDiagramElements(0f, inflowRows.Where((DataFlowRow x) => !x.IsProcessInSameProcessor).ToList(), areInflows: true, dataLineageDiagramContainer, num);
			DrawProcessDiagramElements(1200f, outflowRows.Where((DataFlowRow x) => !x.IsProcessInSameProcessor).ToList(), areInflows: false, dataLineageDiagramContainer, num);
			num++;
		}
		foreach (int processId in (from x in source
			where x.IsProcessInSameProcessor
			orderby x.ProcessName
			select x.ProcessId).Distinct())
		{
			_ = source.FirstOrDefault((DataFlowRow x) => x.ProcessId == processId).ColumnProcessName;
			DiagramShape diagramShape = LineageDiagramHelper.DrawProcessItem(source.FirstOrDefault((DataFlowRow x) => x.ProcessId == processId).ColumnProcessName, num, 600f, 325f, 3f, 100f, ItemFont);
			diagramControl.Items.Add(diagramShape);
			DrawProcessDiagramElements(0f, inflowRows.Where((DataFlowRow x) => x.ProcessId == processId).ToList(), areInflows: true, diagramShape, num);
			DrawProcessDiagramElements(1200f, outflowRows.Where((DataFlowRow x) => x.ProcessId == processId).ToList(), areInflows: false, diagramShape, num);
			num++;
		}
		AutoArrangeFlowDiagram();
		LineageDiagramHelper.UpdateDiagramPositionX(diagramControl.Items.OfType<IDataLineageDiagramItem>());
	}

	private void HighlightItems(DiagramItem diagramItem)
	{
		if (diagramItem is IDataLineageDiagramItem dataLineageDiagramItem)
		{
			HighlightDiagramItem(diagramItem, dataLineageDiagramItem);
		}
		else
		{
			HighlightDiagramItemColumns(diagramItem);
		}
	}

	private void HighlightDiagramItem(DiagramItem diagramItem, IDataLineageDiagramItem dataLineageDiagramItem)
	{
		ClearHighlight();
		if (dataLineageDiagramItem.CanBeHighlighted && !showColumns)
		{
			HighlightedDiagramItems.Add(diagramItem);
		}
		if (dataLineageDiagramItem is DataLineageProcessDiagramItem dataLineageProcessDiagramItem)
		{
			HighlightedConnections.AddRange(dataLineageProcessDiagramItem.IncomingConnectors.OfType<DiagramConnector>().ToList());
			HighlightedConnections.AddRange(dataLineageProcessDiagramItem.OutgoingConnectors.OfType<DiagramConnector>().ToList());
		}
		else
		{
			GetHighlightedItems(dataLineageDiagramItem, dataLineageDiagramItem.DataFlow?.Direction != FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN));
		}
		HighlightedDiagramItems.ForEach(delegate(DiagramItem x)
		{
			x.Appearance.BackColor = LineageDiagramHelper.HighlightObjectColor;
		});
		HighlightedConnections.ForEach(delegate(DiagramConnector x)
		{
			x.Appearance.BorderColor = LineageDiagramHelper.HighlightArrowColor;
		});
		BringHighlightedArrowsToFront();
	}

	private void HighlightDiagramItemColumns(DiagramItem diagramItem, bool clearPreviousSelection = true)
	{
		if (clearPreviousSelection)
		{
			ClearHighlight();
		}
		if (!(diagramItem is DataLineageDiagramColumn dataLineageDiagramColumn))
		{
			return;
		}
		HighlightedConnections.AddRange(dataLineageDiagramColumn.OutgoingConnectors.OfType<DiagramConnector>().ToList());
		HighlightedConnections.AddRange(dataLineageDiagramColumn.IncomingConnectors.OfType<DiagramConnector>().ToList());
		BringHighlightedArrowsToFront();
		foreach (DiagramConnector highlightedConnection in HighlightedConnections)
		{
			if (highlightedConnection.BeginItem is DataLineageDiagramColumn dataLineageDiagramColumn2)
			{
				dataLineageDiagramColumn2.Appearance.BackColor = LineageDiagramHelper.HighlightObjectColor;
				dataLineageDiagramColumn2.Appearance.ForeColor = Color.Black;
				HighlightedDiagramItems.Add(dataLineageDiagramColumn2);
			}
			if (highlightedConnection.EndItem is DataLineageDiagramColumn dataLineageDiagramColumn3)
			{
				dataLineageDiagramColumn3.Appearance.BackColor = LineageDiagramHelper.HighlightObjectColor;
				dataLineageDiagramColumn3.Appearance.ForeColor = Color.Black;
				HighlightedDiagramItems.Add(dataLineageDiagramColumn3);
			}
			highlightedConnection.Appearance.BorderColor = LineageDiagramHelper.HighlightArrowColor;
		}
	}

	private void GetHighlightedItems(IDataLineageDiagramItem dataLineageDiagramItem, bool highlightInflows)
	{
		DiagramItem diagramItem = dataLineageDiagramItem as DiagramItem;
		if (dataLineageDiagramItem.DataFlow != null && dataLineageDiagramItem.DataFlow.Process == null)
		{
			highlightInflows = !highlightInflows;
		}
		while (diagramItem.ParentItem != null && (highlightInflows ? (!diagramItem.IncomingConnectors.Any()) : (!diagramItem.OutgoingConnectors.Any())))
		{
			diagramItem = diagramItem.ParentItem;
		}
		if (highlightInflows)
		{
			HighlightedConnections.AddRange(diagramItem.IncomingConnectors.OfType<DiagramConnector>().ToList());
		}
		else
		{
			HighlightedConnections.AddRange(diagramItem.OutgoingConnectors.OfType<DiagramConnector>().ToList());
		}
		if (dataLineageDiagramItem is DataLineageDiagramDatabaseContainer dataLineageDiagramDatabaseContainer)
		{
			foreach (DiagramItem item in dataLineageDiagramDatabaseContainer.Items)
			{
				HighlightedDiagramItems.Add(item);
				if (item is DataLineageDiagramContainer dataLineageDiagramContainer)
				{
					HighlightedDiagramItems.AddRange(dataLineageDiagramContainer.Items);
				}
			}
			if (highlightInflows)
			{
				HighlightedConnections.AddRange(dataLineageDiagramDatabaseContainer.OutgoingConnectors.OfType<DiagramConnector>().ToList());
			}
			else
			{
				HighlightedConnections.AddRange(dataLineageDiagramDatabaseContainer.IncomingConnectors.OfType<DiagramConnector>().ToList());
			}
		}
		else
		{
			if (!(dataLineageDiagramItem is DiagramItem diagramItem2) || (!(dataLineageDiagramItem is DataLineageFlowDiagramItem) && !(dataLineageDiagramItem is DataLineageFlowDiagramImage)) || !(diagramItem2.ParentItem is DataLineageDiagramContainer dataLineageDiagramContainer2))
			{
				return;
			}
			if (!showColumns)
			{
				HighlightedDiagramItems.Add(dataLineageDiagramContainer2);
				HighlightedDiagramItems.AddRange(dataLineageDiagramContainer2.Items);
			}
			else
			{
				if (!(dataLineageDiagramContainer2.ParentItem is DataLineageDiagramContainer dataLineageDiagramContainer3))
				{
					return;
				}
				foreach (DataLineageDiagramColumn item2 in dataLineageDiagramContainer3.Items.OfType<DataLineageDiagramColumn>())
				{
					HighlightDiagramItemColumns(item2, clearPreviousSelection: false);
					if (highlightInflows)
					{
						HighlightedConnections.AddRange(item2.IncomingConnectors.OfType<DiagramConnector>().ToList());
					}
					else
					{
						HighlightedConnections.AddRange(item2.OutgoingConnectors.OfType<DiagramConnector>().ToList());
					}
				}
			}
		}
	}

	private void ClearHighlight()
	{
		if (isCtrlPressed)
		{
			return;
		}
		HighlightedConnections?.ForEach(delegate(DiagramConnector x)
		{
			x.Appearance.BorderColor = LineageDiagramHelper.OtherObjectBorderColor;
		});
		HighlightedConnections = new List<DiagramConnector>();
		if (HighlightedDiagramItems != null)
		{
			foreach (DiagramItem highlightedDiagramItem in HighlightedDiagramItems)
			{
				if (highlightedDiagramItem is DataLineageDiagramColumn dataLineageDiagramColumn)
				{
					dataLineageDiagramColumn.Appearance.BackColor = Color.Transparent;
					dataLineageDiagramColumn.Appearance.ForeColor = SkinsManager.CurrentSkin.ControlForeColor;
				}
				else if (highlightedDiagramItem is IDataLineageDiagramItem dataLineageDiagramItem)
				{
					highlightedDiagramItem.Appearance.BackColor = dataLineageDiagramItem.OriginalBackColor;
					highlightedDiagramItem.Appearance.BorderColor = dataLineageDiagramItem.OriginalBorderColor;
				}
			}
		}
		HighlightedDiagramItems = new List<DiagramItem>();
	}

	private void AutoArrangeFlowDiagram()
	{
		diagramControl.ApplyTreeLayout(LayoutDirection.TopToBottom, diagramControl.Items.Where((DiagramItem x) => !(x is DiagramConnector)), SplitToConnectedComponentsMode.AllComponents);
		diagramControl.ApplyTreeLayoutForSubordinates(diagramControl.Items.Where((DiagramItem x) => !(x is DiagramConnector)), new TreeLayoutSettings(400.0, 30.0, LayoutDirection.LeftToRight, isCompact: false));
		UpdateDiagramsOffset();
	}

	private bool DrawColumnsDiagramElements(float positionX, List<DataFlowRow> flows, bool areInflows, int diagramNumber, bool isTableColumnDiagram = false)
	{
		IEnumerable<IGrouping<int?, DataFlowRow>> enumerable = from x in flows
			group x by x.DatabaseId;
		bool result = false;
		foreach (IGrouping<int?, DataFlowRow> item in enumerable)
		{
			foreach (DataFlowRow item2 in item)
			{
				DataLineageDiagramContainer dataLineageDiagramContainer = CreateFlowDiagramItemWithColumns(item2, diagramNumber, positionX, areInflows ? lowestInflowColumnContainerPositionY : lowestOutflowColumnContainerPositionY, areInflows, isTableColumnDiagram);
				if (dataLineageDiagramContainer != null)
				{
					float num = dataLineageDiagramContainer.Position.Y + dataLineageDiagramContainer.Size.Height;
					if (areInflows)
					{
						lowestInflowColumnContainerPositionY = ((num > lowestInflowColumnContainerPositionY) ? num : lowestInflowColumnContainerPositionY);
					}
					else
					{
						lowestOutflowColumnContainerPositionY = ((num > lowestOutflowColumnContainerPositionY) ? num : lowestOutflowColumnContainerPositionY);
					}
					if (!diagramControl.Items.Contains(dataLineageDiagramContainer))
					{
						diagramControl.Items.Add(dataLineageDiagramContainer);
					}
					result = true;
				}
			}
		}
		return result;
	}

	private void DrawProcessDiagramElements(float positionX, List<DataFlowRow> flows, bool areInflows, DiagramItem processItem, int diagramNumber)
	{
		foreach (IGrouping<int?, DataFlowRow> item in from x in flows
			group x by x.DatabaseId)
		{
			DataLineageDiagramContainer dataLineageDiagramContainer = null;
			if (!string.IsNullOrEmpty(item.FirstOrDefault()?.DatabaseName))
			{
				dataLineageDiagramContainer = LineageDiagramHelper.CreateDatabaseContainer(item, diagramNumber, areInflows, 250f, 75f, 20f, 100f, 30f, ItemFont);
				diagramControl.Items.Add(dataLineageDiagramContainer);
				diagramControl.Items.Add(LineageDiagramHelper.SetConnection(areInflows ? dataLineageDiagramContainer : processItem, areInflows ? processItem : dataLineageDiagramContainer));
			}
			foreach (DataFlowRow item2 in item)
			{
				DataLineageDiagramContainer dataLineageDiagramContainer2 = LineageDiagramHelper.CreateFlowDiagramItem(item2, diagramNumber, positionX, 250f, 75f, 100f, 3f, ItemFont);
				if (dataLineageDiagramContainer != null)
				{
					DiagramItem diagramItem = dataLineageDiagramContainer.Items.LastOrDefault();
					dataLineageDiagramContainer2.Position = new PointFloat(20f, (diagramItem != null) ? (diagramItem.Position.Y + diagramItem.Height + 30f) : 0f);
					dataLineageDiagramContainer.Items.Add(dataLineageDiagramContainer2);
				}
				else
				{
					diagramControl.Items.Add(LineageDiagramHelper.SetConnection(areInflows ? dataLineageDiagramContainer2 : processItem, areInflows ? processItem : dataLineageDiagramContainer2));
					diagramControl.Items.Add(dataLineageDiagramContainer2);
				}
			}
			if (dataLineageDiagramContainer != null)
			{
				LineageDiagramHelper.RecalculateDiagramContainerHeight(dataLineageDiagramContainer, 30f);
			}
		}
	}

	private DataLineageDiagramContainer CreateFlowDiagramItemWithColumns(DataFlowRow flow, int diagramNumber, float positionX, float positionY, bool areInflows, bool isTableColumnDiagram = false)
	{
		List<DataLineageColumnsFlowRow> list = null;
		if (flow?.Process == null)
		{
			list = DB.DataFlows.GetDataColumnsFlows(flow.ProcessId, base.ParentForm);
			if (list != null)
			{
				list = list.Where((DataLineageColumnsFlowRow x) => x.IsRowComplete).ToList();
			}
		}
		else
		{
			list = flow.Process.Columns.Where(delegate(DataLineageColumnsFlowRow c)
			{
				bool num2;
				if (!areInflows)
				{
					Guid guid3 = flow.Guid;
					Guid? guid4 = c.OutflowRow?.Guid;
					if (guid4.HasValue)
					{
						num2 = guid3 == guid4.GetValueOrDefault();
						goto IL_0094;
					}
				}
				else
				{
					Guid guid3 = flow.Guid;
					Guid? guid4 = c.InflowRow?.Guid;
					if (guid4.HasValue)
					{
						num2 = guid3 == guid4.GetValueOrDefault();
						goto IL_0094;
					}
				}
				goto IL_009d;
				IL_009d:
				return false;
				IL_0094:
				if (num2)
				{
					return c.IsRowComplete;
				}
				goto IL_009d;
			}).ToList();
		}
		if (list == null || list.Count == 0)
		{
			return null;
		}
		DataLineageDiagramContainer dataLineageDiagramContainer = null;
		double columnPositionX = 1.5;
		float columnPositionY = 0f;
		if (isTableColumnDiagram && currentTableDataFlowRowForColumns != null && flow == currentTableDataFlowRowForColumns)
		{
			dataLineageDiagramContainer = diagramControl.Items.OfType<DataLineageDiagramContainer>().FirstOrDefault(delegate(DataLineageDiagramContainer x)
			{
				Guid? guid = x.DataFlow?.Guid;
				Guid guid2 = currentTableDataFlowRowForColumns.Guid;
				if (!guid.HasValue)
				{
					return false;
				}
				return !guid.HasValue || guid.GetValueOrDefault() == guid2;
			});
			if (dataLineageDiagramContainer != null)
			{
				columnPositionY = dataLineageDiagramContainer.Height;
			}
		}
		if (dataLineageDiagramContainer == null)
		{
			dataLineageDiagramContainer = LineageDiagramHelper.CreateDiagramContainerWithItem(flow, diagramNumber, positionX, positionY, list.Count(), out columnPositionX, out columnPositionY, 250f, 75f, 100f, 30f, 30f, 3, ItemFont);
		}
		foreach (DataLineageColumnsFlowRow item in list)
		{
			if (Columns == null)
			{
				Columns = new List<DataLineageDiagramColumn>();
			}
			string content = (areInflows ? item.InflowColumnName : item.OutflowColumnName);
			DataLineageDiagramColumn dataLineageDiagramColumn = dataLineageDiagramContainer.Items.OfType<DataLineageDiagramColumn>().FirstOrDefault((DataLineageDiagramColumn c) => c.Content == content && c.DataFlow.Guid == flow.Guid);
			if (dataLineageDiagramColumn != null)
			{
				dataLineageDiagramColumn.DataLineageColumnsFlowRows.Add(item);
				continue;
			}
			if (isTableColumnDiagram)
			{
				dataLineageDiagramColumn = new DataLineageDiagramTableColumn();
				if (!string.IsNullOrWhiteSpace(flow?.Direction))
				{
					(dataLineageDiagramColumn as DataLineageDiagramTableColumn).SetDirection(FlowDirectionEnum.StringToType(flow.Direction).Value);
				}
			}
			else
			{
				dataLineageDiagramColumn = new DataLineageDiagramColumn();
			}
			dataLineageDiagramColumn.Shape = BasicShapes.Rectangle;
			dataLineageDiagramColumn.Width = dataLineageDiagramContainer.Width - 3f;
			dataLineageDiagramColumn.Height = 27f;
			dataLineageDiagramColumn.Content = content;
			dataLineageDiagramColumn.Position = new PointFloat((float)columnPositionX, columnPositionY);
			dataLineageDiagramColumn.DataFlow = flow;
			dataLineageDiagramColumn.DataLineageColumnsFlowRows.Add(item);
			float num = (float)TextRenderer.MeasureText(dataLineageDiagramColumn.Content, ItemFont).Width / dataLineageDiagramColumn.Width;
			if (num > 1f)
			{
				dataLineageDiagramColumn.Height *= (int)num + 1;
			}
			dataLineageDiagramColumn.Appearance.Font = ItemFont;
			dataLineageDiagramColumn.Appearance.BackColor = Color.Transparent;
			dataLineageDiagramColumn.Appearance.ForeColor = SkinsManager.CurrentSkin.ControlForeColor;
			dataLineageDiagramColumn.Appearance.BorderSize = 0;
			dataLineageDiagramColumn.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
			columnPositionY += dataLineageDiagramColumn.Height + 3f;
			Columns.Add(dataLineageDiagramColumn);
			dataLineageDiagramContainer.Items.Add(dataLineageDiagramColumn);
		}
		LineageDiagramHelper.RecalculateDiagramContainerHeight(dataLineageDiagramContainer, 3f);
		return dataLineageDiagramContainer;
	}

	private void SetColumnsConnections()
	{
		List<DataLineageDiagramColumn> columns = Columns;
		foreach (DataLineageDiagramColumn item in columns.Where((DataLineageDiagramColumn c) => c.FlowDirection == FlowDirectionEnum.Direction.IN))
		{
			foreach (DataLineageColumnsFlowRow column in item.DataLineageColumnsFlowRows)
			{
				IEnumerable<DataLineageDiagramColumn> enumerable = columns.Where((DataLineageDiagramColumn c) => c.DataLineageColumnsFlowRows.Contains(column) && c.FlowDirection == FlowDirectionEnum.Direction.OUT);
				if (enumerable == null)
				{
					continue;
				}
				foreach (DataLineageDiagramColumn item2 in enumerable)
				{
					diagramControl.Items.Add(LineageDiagramHelper.SetConnection(item, item2));
				}
			}
		}
	}

	private void UpdateDiagramsOffset()
	{
		LineageDiagramHelper.UpdateDiagramOffset(diagramControl.Items.OfType<IDataLineageDiagramItem>(), 250f);
		diagramControl.UpdateRoute();
	}

	public void ChangeDiagramZoom(bool zoomIn)
	{
		if (zoomIn)
		{
			diagramControl.ZoomIn();
		}
		else
		{
			diagramControl.ZoomOut();
		}
	}

	private void BringHighlightedArrowsToFront()
	{
		try
		{
			List<DiagramConnector> items = (from x in HighlightedConnections.Distinct()
				where diagramControl.Items.Contains(x)
				select x).ToList();
			diagramControl.BringItemsToFront(items);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while moving connectors forward.", base.ParentForm);
		}
	}

	private void DiagramControl_MouseClick(object sender, MouseEventArgs e)
	{
		DiagramItem diagramItem = diagramControl.CalcHitItem(new Point(e.X, e.Y));
		if (e.Button == MouseButtons.Left)
		{
			HighlightItems(diagramItem);
		}
		else if (e.Button == MouseButtons.Right && diagramItem is IDataLineageDiagramItem dataLineageDiagramItem)
		{
			popupMenu.ItemLinks.Clear();
			DataFlowRow dataFlow = dataLineageDiagramItem.DataFlow;
			if (dataFlow != null)
			{
				goToBarButtonItem.ImageOptions.Image = dataFlow.Icon;
				goToBarButtonItem.Caption = "Go to " + dataFlow.ObjectCaption;
				goToBarButtonItem.Tag = dataLineageDiagramItem;
				popupMenu.ItemLinks.Add(goToBarButtonItem);
				popupMenu.ShowPopup(barManager, diagramControl.PointToScreen(e.Location));
			}
		}
	}

	private void DiagramControl_CustomDrawBackground(object sender, CustomDrawBackgroundEventArgs e)
	{
		e.Graphics.FillRectangle(new SolidBrush(SkinsManager.CurrentSkin.ControlBackColor), e.TotalBounds);
	}

	private void DiagramControl_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		ClearHighlight();
		DiagramItem diagramItem = diagramControl.CalcHitItem(new Point(e.X, e.Y));
		LineageDiagramHelper.GoToObjectAndShowDiagram(metadataEditorUserControl, diagramItem as IDataLineageDiagramItem, showColumns);
	}

	private void DiagramControl_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.ControlKey)
		{
			isCtrlPressed = false;
		}
	}

	private void DiagramControl_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.ControlKey)
		{
			isCtrlPressed = true;
		}
	}

	private void DiagramControl_CustomCursor(object sender, DiagramCustomCursorEventArgs e)
	{
		e.Cursor = Cursors.Arrow;
	}

	private void GoToBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.Item.Tag is IDataLineageDiagramItem dataLineageDiagramItem)
		{
			LineageDiagramHelper.GoToObjectAndShowDiagram(metadataEditorUserControl, dataLineageDiagramItem, showColumns);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.diagramControl = new Dataedo.App.UserControls.OverriddenControls.DiagramControlWithTransparentFocusedBorder();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.goToBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.diagramControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.diagramControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(1420, 904);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.diagramControl.Location = new System.Drawing.Point(2, 2);
		this.diagramControl.Name = "diagramControl";
		this.diagramControl.OptionsBehavior.ScrollMode = DevExpress.Diagram.Core.DiagramScrollMode.Content;
		this.diagramControl.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection("BasicShapes", "BasicFlowchartShapes");
		this.diagramControl.OptionsExport.PrintExportMode = DevExpress.Diagram.Core.PrintExportMode.Content;
		this.diagramControl.OptionsProtection.AllowAddRemoveItems = false;
		this.diagramControl.OptionsProtection.AllowChangeConnectorsRoute = false;
		this.diagramControl.OptionsProtection.AllowChangeItemBackground = false;
		this.diagramControl.OptionsProtection.AllowChangeItemStroke = false;
		this.diagramControl.OptionsProtection.AllowChangeItemStyle = false;
		this.diagramControl.OptionsProtection.AllowChangeItemZOrder = false;
		this.diagramControl.OptionsProtection.AllowCopyItems = false;
		this.diagramControl.OptionsProtection.AllowEditItems = false;
		this.diagramControl.OptionsProtection.AllowMoveItems = false;
		this.diagramControl.OptionsProtection.AllowResizeItems = false;
		this.diagramControl.OptionsProtection.AllowRotateItems = false;
		this.diagramControl.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
		this.diagramControl.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Letter;
		this.diagramControl.OptionsView.ShowGrid = false;
		this.diagramControl.OptionsView.ShowPageBreaks = false;
		this.diagramControl.OptionsView.ShowRulers = false;
		this.diagramControl.Size = new System.Drawing.Size(1416, 900);
		this.diagramControl.StyleController = this.nonCustomizableLayoutControl1;
		this.diagramControl.TabIndex = 4;
		this.diagramControl.Text = "diagramControl1";
		this.diagramControl.CustomCursor += new System.EventHandler<DevExpress.XtraDiagram.DiagramCustomCursorEventArgs>(DiagramControl_CustomCursor);
		this.diagramControl.CustomDrawBackground += new System.EventHandler<DevExpress.XtraDiagram.CustomDrawBackgroundEventArgs>(DiagramControl_CustomDrawBackground);
		this.diagramControl.KeyDown += new System.Windows.Forms.KeyEventHandler(DiagramControl_KeyDown);
		this.diagramControl.KeyUp += new System.Windows.Forms.KeyEventHandler(DiagramControl_KeyUp);
		this.diagramControl.MouseClick += new System.Windows.Forms.MouseEventHandler(DiagramControl_MouseClick);
		this.diagramControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(DiagramControl_MouseDoubleClick);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(1420, 904);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.diagramControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(1420, 904);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.popupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.goToBarButtonItem)
		});
		this.popupMenu.Manager = this.barManager;
		this.popupMenu.Name = "popupMenu";
		this.goToBarButtonItem.Caption = "Go to";
		this.goToBarButtonItem.Id = 0;
		this.goToBarButtonItem.Name = "goToBarButtonItem";
		this.goToBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(GoToBarButtonItem_ItemClick);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[1] { this.goToBarButtonItem });
		this.barManager.MaxItemId = 1;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1420, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 904);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1420, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 904);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1420, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 904);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "DataLineageDiagramUserControl";
		base.Size = new System.Drawing.Size(1420, 904);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.diagramControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
