using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;

namespace Dataedo.App.UserControls.DataLineage;

public static class LineageDiagramHelper
{
	public static readonly Color TableObjectColor = ColorTranslator.FromHtml("#edf2fa");

	public static readonly Color TableObjectBorderColor = ColorTranslator.FromHtml("#487ac6");

	public static readonly Color ViewObjectColor = ColorTranslator.FromHtml("#dff8ff");

	public static readonly Color ViewObjectBorderColor = ColorTranslator.FromHtml("#00a1d0");

	public static readonly Color StructureObjectColor = ColorTranslator.FromHtml("#fafce7");

	public static readonly Color StructureObjectBorderColor = ColorTranslator.FromHtml("#bace1f");

	public static readonly Color ProcedureObjectColor = ColorTranslator.FromHtml("#fae9e9");

	public static readonly Color ProcedureObjectBorderColor = ColorTranslator.FromHtml("#c62d2d");

	public static readonly Color FunctionObjectColor = ColorTranslator.FromHtml("#fdf2e8");

	public static readonly Color FunctionObjectBorderColor = ColorTranslator.FromHtml("#ed7d17");

	public static readonly Color OtherObjectColor = ColorTranslator.FromHtml("#F6F6F6");

	public static readonly Color OtherObjectBorderColor = ColorTranslator.FromHtml("#b3b3b3");

	public static readonly Color HighlightObjectColor = ColorTranslator.FromHtml("#ecf7ed");

	public static readonly Color HighlightObjectBorderColor = ColorTranslator.FromHtml("#4caf50");

	public static readonly Color HighlightArrowColor = ColorTranslator.FromHtml("#3664AF");

	public static Color GetObjectColor(DataFlowRow dataFlowRow)
	{
		return dataFlowRow?.ObjectType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => TableObjectColor, 
			SharedObjectTypeEnum.ObjectType.View => ViewObjectColor, 
			SharedObjectTypeEnum.ObjectType.Structure => StructureObjectColor, 
			SharedObjectTypeEnum.ObjectType.Procedure => ProcedureObjectColor, 
			SharedObjectTypeEnum.ObjectType.Function => FunctionObjectColor, 
			_ => OtherObjectColor, 
		};
	}

	public static Color GetObjectBorderColor(DataFlowRow dataFlowRow)
	{
		return dataFlowRow?.ObjectType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => TableObjectBorderColor, 
			SharedObjectTypeEnum.ObjectType.View => ViewObjectBorderColor, 
			SharedObjectTypeEnum.ObjectType.Structure => StructureObjectBorderColor, 
			SharedObjectTypeEnum.ObjectType.Procedure => ProcedureObjectBorderColor, 
			SharedObjectTypeEnum.ObjectType.Function => FunctionObjectBorderColor, 
			_ => OtherObjectBorderColor, 
		};
	}

	public static void UpdateDiagramPositionX(IEnumerable<IDataLineageDiagramItem> items)
	{
		if (items == null || !items.Any())
		{
			return;
		}
		foreach (IDataLineageDiagramItem item in items)
		{
			if (item is DataLineageProcessDiagramItem dataLineageProcessDiagramItem)
			{
				dataLineageProcessDiagramItem.Position = new PointFloat(600f, dataLineageProcessDiagramItem.Position.Y);
			}
			else if (item is DataLineageDbTreeNodeContainer dataLineageDbTreeNodeContainer)
			{
				dataLineageDbTreeNodeContainer.Position = new PointFloat(600f, dataLineageDbTreeNodeContainer.Position.Y);
			}
			else if (item is DataLineageDiagramDatabaseContainer dataLineageDiagramDatabaseContainer)
			{
				if (dataLineageDiagramDatabaseContainer.IsInflowContainer)
				{
					dataLineageDiagramDatabaseContainer.Position = new PointFloat(-20f, dataLineageDiagramDatabaseContainer.Position.Y);
				}
				else
				{
					dataLineageDiagramDatabaseContainer.Position = new PointFloat(1180f, dataLineageDiagramDatabaseContainer.Position.Y);
				}
			}
			else
			{
				if (!(item is DataLineageDiagramContainer dataLineageDiagramContainer))
				{
					continue;
				}
				string text = dataLineageDiagramContainer.DataFlow?.Direction;
				if (text != null)
				{
					if (text == FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN))
					{
						float x = (dataLineageDiagramContainer.DataFlow.IsProcessInSameProcessor ? 0f : 1200f);
						dataLineageDiagramContainer.Position = new PointFloat(x, dataLineageDiagramContainer.Position.Y);
					}
					else
					{
						float x2 = (dataLineageDiagramContainer.DataFlow.IsProcessInSameProcessor ? 1200f : 0f);
						dataLineageDiagramContainer.Position = new PointFloat(x2, dataLineageDiagramContainer.Position.Y);
					}
				}
			}
		}
	}

	public static void UpdateDiagramOffset(IEnumerable<IDataLineageDiagramItem> items, float diagramOffset)
	{
		if (items == null || !items.Any())
		{
			return;
		}
		int num = items.Min((IDataLineageDiagramItem x) => x.DiagramNumber);
		int num2 = items.Max((IDataLineageDiagramItem x) => x.DiagramNumber);
		int i;
		for (i = num; i <= num2; i++)
		{
			if (i == num)
			{
				continue;
			}
			IEnumerable<IDataLineageDiagramItem> enumerable = items.Where((IDataLineageDiagramItem x) => x.DiagramNumber == i - 1);
			IEnumerable<IDataLineageDiagramItem> enumerable2 = items.Where((IDataLineageDiagramItem x) => x.DiagramNumber == i);
			if (enumerable2 == null || enumerable == null || !enumerable2.Any() || !enumerable.Any())
			{
				continue;
			}
			IDataLineageDiagramItem dataLineageDiagramItem = enumerable.OrderByDescending((IDataLineageDiagramItem x) => x.Position.Y).FirstOrDefault();
			float num3 = enumerable2.Min((IDataLineageDiagramItem x) => x.Position.Y);
			float num4 = Math.Abs(dataLineageDiagramItem.Position.Y + dataLineageDiagramItem.Size.Height - num3);
			float num5 = dataLineageDiagramItem.Position.Y + dataLineageDiagramItem.Size.Height;
			float num6 = num3 - num5;
			num4 = ((num6 > diagramOffset) ? (num6 - diagramOffset) : ((!(num6 < diagramOffset)) ? (diagramOffset - num6) : (0f - (diagramOffset - num6))));
			foreach (IDataLineageDiagramItem item in enumerable2)
			{
				if (!(item is DiagramConnector))
				{
					if (item is DataLineageProcessDiagramItem dataLineageProcessDiagramItem)
					{
						_ = dataLineageProcessDiagramItem.Position;
					}
					item.Position = new PointFloat(item.Position.X, item.Position.Y - num4);
				}
			}
		}
	}

	public static DiagramShape DrawProcessItem(string processName, int diagramNumber, float processPositionX, float processItemWidth, float borderSize, float itemHeight, Font itemFont)
	{
		DataLineageProcessDiagramItem dataLineageProcessDiagramItem = new DataLineageProcessDiagramItem();
		dataLineageProcessDiagramItem.Shape = BasicShapes.Rectangle;
		dataLineageProcessDiagramItem.X = processPositionX;
		dataLineageProcessDiagramItem.Y = 0f;
		dataLineageProcessDiagramItem.Width = processItemWidth - borderSize * 2f;
		dataLineageProcessDiagramItem.Height = itemHeight - borderSize * 2f;
		dataLineageProcessDiagramItem.Content = processName;
		dataLineageProcessDiagramItem.DiagramNumber = diagramNumber;
		dataLineageProcessDiagramItem.OriginalBackColor = GetObjectColor(null);
		dataLineageProcessDiagramItem.OriginalBorderColor = GetObjectBorderColor(null);
		dataLineageProcessDiagramItem.CanBeHighlighted = true;
		dataLineageProcessDiagramItem.Appearance.Font = itemFont;
		dataLineageProcessDiagramItem.Appearance.BackColor = GetObjectColor(null);
		dataLineageProcessDiagramItem.Appearance.ForeColor = Color.Black;
		return dataLineageProcessDiagramItem;
	}

	public static DataLineageDiagramShape DrawEmptyDiagramInfo(string text, float processPositionX, float processItemWidth, float borderSize, float itemHeight, Font itemFont)
	{
		DataLineageDiagramShape dataLineageDiagramShape = new DataLineageDiagramShape();
		dataLineageDiagramShape.Shape = BasicShapes.Rectangle;
		dataLineageDiagramShape.X = processPositionX;
		dataLineageDiagramShape.Y = 0f;
		dataLineageDiagramShape.Width = processItemWidth - borderSize * 2f;
		dataLineageDiagramShape.Height = itemHeight - borderSize * 2f;
		dataLineageDiagramShape.Content = text;
		dataLineageDiagramShape.OriginalBackColor = GetObjectColor(null);
		dataLineageDiagramShape.OriginalBorderColor = GetObjectBorderColor(null);
		dataLineageDiagramShape.CanBeHighlighted = false;
		dataLineageDiagramShape.Appearance.Font = itemFont;
		dataLineageDiagramShape.Appearance.BackColor = GetObjectColor(null);
		dataLineageDiagramShape.Appearance.ForeColor = Color.Black;
		return dataLineageDiagramShape;
	}

	public static DiagramConnector SetConnection(DiagramItem startItem, DiagramItem endItem)
	{
		DiagramConnector diagramConnector = new DiagramConnector(startItem, endItem);
		diagramConnector.EndArrowSize = new SizeF(25f, 15f);
		diagramConnector.BeginItemPointIndex = ((!(startItem is DiagramContainer)) ? 1 : 3);
		diagramConnector.EndItemPointIndex = ((endItem is DiagramContainer) ? 10 : 3);
		diagramConnector.Type = ConnectorType.Curved;
		diagramConnector.Appearance.BorderColor = OtherObjectBorderColor;
		return diagramConnector;
	}

	public static void GoToObjectAndShowDiagram(MetadataEditorUserControl metadataEditorUserControl, IDataLineageDiagramItem dataLineageDiagramItem, bool showColumns)
	{
		if (metadataEditorUserControl != null)
		{
			DataFlowRow dataFlowRow = dataLineageDiagramItem?.DataFlow;
			if (dataFlowRow != null && !(dataLineageDiagramItem is DataLineageProcessDiagramItem) && dataFlowRow.DatabaseId.HasValue)
			{
				metadataEditorUserControl.SelectObjectAndShowDataLineageTab(dataFlowRow.ObjectType, new ObjectEventArgs(dataFlowRow.DatabaseId.Value, dataFlowRow.ObjectId, null), null, selectDiagramTab: true, showColumns);
			}
		}
	}

	public static DataLineageDiagramContainer CreateDatabaseContainer(IGrouping<int?, DataFlowRow> databaseFlowsGroup, int diagramNumber, bool isInflowContainer, float itemWidth, float imageFieldWidth, float containerPadding, float itemHeight, float spaceBetweenItems, Font itemFont)
	{
		DataLineageDiagramDatabaseContainer dataLineageDiagramDatabaseContainer = new DataLineageDiagramDatabaseContainer
		{
			Shape = StandardContainers.Plain,
			Header = databaseFlowsGroup.FirstOrDefault().DatabaseName,
			ShowHeader = true,
			Size = new SizeF(itemWidth + imageFieldWidth + containerPadding * 2f, (itemHeight + spaceBetweenItems) * (float)databaseFlowsGroup.Count() + spaceBetweenItems * 2f),
			HeaderPadding = new Padding(0, 20, 0, 20),
			OriginalBackColor = SkinsManager.CurrentSkin.ControlBackColor,
			OriginalBorderColor = OtherObjectBorderColor,
			CanBeHighlighted = false,
			DiagramNumber = diagramNumber,
			IsInflowContainer = isInflowContainer
		};
		dataLineageDiagramDatabaseContainer.Appearance.Font = itemFont;
		dataLineageDiagramDatabaseContainer.Appearance.BackColor = SkinsManager.CurrentSkin.ControlBackColor;
		dataLineageDiagramDatabaseContainer.Appearance.ForeColor = SkinsManager.CurrentSkin.ControlForeColor;
		dataLineageDiagramDatabaseContainer.Appearance.BorderColor = dataLineageDiagramDatabaseContainer.OriginalBorderColor;
		dataLineageDiagramDatabaseContainer.DataProcess = databaseFlowsGroup.FirstOrDefault()?.Process;
		return dataLineageDiagramDatabaseContainer;
	}

	public static DataLineageDiagramContainer CreateTreeNodeDiagramItem(DBTreeNode node, int diagramNumber, float positionX, float itemWidth, float imageFieldWidth, float itemHeight, float borderSize, Font itemFont)
	{
		return CreateFlowDiagramItem(null, diagramNumber, positionX, itemWidth, imageFieldWidth, itemHeight, borderSize, itemFont, node);
	}

	public static DataLineageDiagramContainer CreateFlowDiagramItem(DataFlowRow flow, int diagramNumber, float positionX, float itemWidth, float imageFieldWidth, float itemHeight, float borderSize, Font itemFont, DBTreeNode node = null)
	{
		DataLineageDiagramContainer dataLineageDiagramContainer = GenerateDiagramContainer(flow, diagramNumber, itemWidth, imageFieldWidth, itemHeight, node);
		dataLineageDiagramContainer.Appearance.Font = itemFont;
		dataLineageDiagramContainer.Appearance.BackColor = dataLineageDiagramContainer.OriginalBackColor;
		dataLineageDiagramContainer.Appearance.ForeColor = Color.Black;
		dataLineageDiagramContainer.Appearance.BorderSize = (int)borderSize;
		dataLineageDiagramContainer.Appearance.BorderColor = dataLineageDiagramContainer.OriginalBorderColor;
		DataLineageFlowDiagramItem dataLineageFlowDiagramItem = new DataLineageFlowDiagramItem
		{
			Shape = BasicShapes.Rectangle,
			X = positionX,
			Y = 0f,
			Width = itemWidth - borderSize * 2f,
			Height = itemHeight - borderSize * 2f,
			Content = (node?.DisplayNameWithShema ?? flow?.ObjectCaptionWithoutDatabase),
			DataProcess = flow?.Process,
			DataFlow = flow,
			Position = new PointFloat(imageFieldWidth, borderSize),
			OriginalBackColor = GetObjectColor(flow),
			OriginalBorderColor = Color.Transparent,
			CanBeHighlighted = true
		};
		dataLineageFlowDiagramItem.Appearance.Font = itemFont;
		dataLineageFlowDiagramItem.Appearance.BackColor = dataLineageFlowDiagramItem.OriginalBackColor;
		dataLineageFlowDiagramItem.Appearance.ForeColor = Color.Black;
		dataLineageFlowDiagramItem.Appearance.BorderSize = 0;
		dataLineageFlowDiagramItem.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
		dataLineageDiagramContainer.Items.Add(dataLineageFlowDiagramItem);
		DataLineageFlowDiagramImage item = new DataLineageFlowDiagramImage
		{
			Size = new SizeF(30f, 30f),
			Position = new PointFloat(25f, 35f),
			Image = ((node != null) ? IconsSupport.GetObjectIcon(node.ObjectType, node.Subtype, node.Source) : flow?.Icon),
			DataProcess = flow?.Process,
			DataFlow = flow,
			OriginalBackColor = Color.Transparent,
			OriginalBorderColor = Color.Transparent,
			CanBeHighlighted = true
		};
		dataLineageDiagramContainer.Items.Add(item);
		return dataLineageDiagramContainer;
	}

	private static DataLineageDiagramContainer GenerateDiagramContainer(DataFlowRow flow, int diagramNumber, float itemWidth, float imageFieldWidth, float itemHeight, DBTreeNode node = null)
	{
		if (node == null)
		{
			return new DataLineageDiagramContainer
			{
				HeaderPadding = new Padding(0, 20, 0, 20),
				Shape = StandardContainers.Plain,
				ShowHeader = false,
				Size = new Size((int)itemWidth + (int)imageFieldWidth, (int)itemHeight),
				DiagramNumber = diagramNumber,
				DataProcess = flow?.Process,
				DataFlow = flow,
				OriginalBackColor = GetObjectColor(flow),
				OriginalBorderColor = GetObjectBorderColor(flow),
				CanBeHighlighted = true
			};
		}
		return new DataLineageDbTreeNodeContainer
		{
			HeaderPadding = new Padding(0, 20, 0, 20),
			Shape = StandardContainers.Plain,
			ShowHeader = false,
			Size = new Size((int)itemWidth + (int)imageFieldWidth, (int)itemHeight),
			DiagramNumber = diagramNumber,
			DataProcess = flow?.Process,
			DataFlow = flow,
			OriginalBackColor = GetObjectColor(null),
			OriginalBorderColor = GetObjectBorderColor(null),
			CanBeHighlighted = true
		};
	}

	public static void RecalculateDiagramContainerHeight(DataLineageDiagramContainer diagramItemContainer, float spaceBetweenItems)
	{
		if (diagramItemContainer == null)
		{
			return;
		}
		DiagramItem diagramItem = diagramItemContainer.Items.OrderByDescending((DiagramItem x) => x.Position.Y).FirstOrDefault();
		if (diagramItem != null)
		{
			float num = diagramItem.Position.Y + diagramItem.Size.Height;
			float num2 = diagramItemContainer.Size.Height - (float)diagramItemContainer.ActualPadding.Top;
			if (num > num2)
			{
				float num3 = num - num2 + spaceBetweenItems;
				diagramItemContainer.Size = new SizeF(diagramItemContainer.Width, diagramItemContainer.Height + num3);
			}
			else if (num2 - num < spaceBetweenItems)
			{
				float num4 = spaceBetweenItems - (num2 - num);
				diagramItemContainer.Size = new SizeF(diagramItemContainer.Width, diagramItemContainer.Height + num4);
			}
			else if (num + spaceBetweenItems < num2)
			{
				float num5 = num2 - num - spaceBetweenItems;
				diagramItemContainer.Size = new SizeF(diagramItemContainer.Width, diagramItemContainer.Height - num5);
			}
		}
	}

	public static List<DataFlowRow> GenerateFlowsToDrawTableColumns(IEnumerable<ColumnDataLineageResult> columnDataLineageResults, DataProcessRow dataProcessRow, bool areInflows, ref DataFlowRow currentTableDataFlowRowForColumns, DBTreeNode currentObjectNode)
	{
		List<DataFlowRow> list = new List<DataFlowRow>();
		if (columnDataLineageResults == null || dataProcessRow == null || currentObjectNode == null)
		{
			return list;
		}
		foreach (IGrouping<int, ColumnDataLineageResult> item in from c in columnDataLineageResults
			group c by (!areInflows) ? c.OutflowObjectId : c.InflowObjectId)
		{
			ColumnDataLineageResult columnDataLineageResult = item.FirstOrDefault();
			if (columnDataLineageResult == null)
			{
				continue;
			}
			SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(areInflows ? columnDataLineageResult.InflowObjectType : columnDataLineageResult.OutflowObjectType);
			DataFlowRow dataFlowRow = new DataFlowRow(-1, null, isProcessInSameProcessor: true, areInflows ? FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN) : FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT), areInflows ? columnDataLineageResult.InflowObjectId : columnDataLineageResult.OutflowObjectId, objectType ?? SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.StringToType(objectType, areInflows ? columnDataLineageResult.InflowObjectSubtype : columnDataLineageResult.OutflowObjectSubtype), UserTypeEnum.ObjectToTypeOrDbms(areInflows ? columnDataLineageResult.InflowObjectSource : columnDataLineageResult.OutflowObjectSource), UserTypeEnum.UserType.USER, areInflows ? columnDataLineageResult.InflowDatabaseTitle : columnDataLineageResult.OutflowDatabaseTitle, areInflows ? columnDataLineageResult.InflowDatabaseId : columnDataLineageResult.OutflowDatabaseId, areInflows ? columnDataLineageResult.InflowObjectName : columnDataLineageResult.OutflowObjectName, null, areInflows ? columnDataLineageResult.InflowObjectSchema : columnDataLineageResult.OutflowObjectSchema, areInflows ? columnDataLineageResult.InflowObjectShowSchema : columnDataLineageResult.OutflowObjectShowSchema)
			{
				Process = dataProcessRow
			};
			list.Add(dataFlowRow);
			if (currentTableDataFlowRowForColumns == null)
			{
				bool flag = areInflows;
				currentTableDataFlowRowForColumns = new DataFlowRow(0, null, isProcessInSameProcessor: true, flag ? FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT) : FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN), flag ? columnDataLineageResult.OutflowObjectId : columnDataLineageResult.InflowObjectId, currentObjectNode.ObjectType, currentObjectNode.Subtype, currentObjectNode.Source ?? UserTypeEnum.UserType.DBMS, UserTypeEnum.UserType.USER, flag ? columnDataLineageResult.OutflowDatabaseTitle : columnDataLineageResult.InflowDatabaseTitle, flag ? columnDataLineageResult.OutflowDatabaseId : columnDataLineageResult.InflowDatabaseId, flag ? columnDataLineageResult.OutflowObjectName : columnDataLineageResult.InflowObjectName, null, flag ? columnDataLineageResult.OutflowObjectSchema : columnDataLineageResult.InflowObjectSchema, flag ? columnDataLineageResult.OutflowObjectShowSchema : columnDataLineageResult.InflowObjectShowSchema)
				{
					Process = dataProcessRow
				};
			}
			foreach (ColumnDataLineageResult item2 in item)
			{
				DataLineageColumnsFlowRow dataLineageColumnsFlowRow = new DataLineageColumnsFlowRow();
				dataLineageColumnsFlowRow.InflowRow = (areInflows ? dataFlowRow : currentTableDataFlowRowForColumns);
				dataLineageColumnsFlowRow.InflowColumnId = item2.InflowColumnId;
				dataLineageColumnsFlowRow.InflowColumnName = item2.InflowColumnName;
				dataLineageColumnsFlowRow.InflowColumnImage = IconsSupport.GetObjectIcon(SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column), item2.InflowColumnType, null);
				dataLineageColumnsFlowRow.OutflowRow = (areInflows ? currentTableDataFlowRowForColumns : dataFlowRow);
				dataLineageColumnsFlowRow.OutflowColumnId = item2.OutflowColumnId;
				dataLineageColumnsFlowRow.OutflowColumnName = item2.OutflowColumnName;
				dataLineageColumnsFlowRow.OutflowColumnImage = IconsSupport.GetObjectIcon(SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column), item2.OutflowColumnType, null);
				dataLineageColumnsFlowRow.ProcessRow = dataProcessRow;
				dataLineageColumnsFlowRow.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
				dataProcessRow.Columns.Add(dataLineageColumnsFlowRow);
			}
		}
		return list;
	}

	public static void GenerateTableColumnsConnections(DiagramControl diagramControl, DataFlowRow currentTableDataFlowRowForColumns, List<DataLineageDiagramColumn> columns)
	{
		DataLineageDiagramContainer dataLineageDiagramContainer = diagramControl.Items.OfType<DataLineageDiagramContainer>().FirstOrDefault(delegate(DataLineageDiagramContainer x)
		{
			Guid? guid5 = x.DataFlow?.Guid;
			Guid guid6 = currentTableDataFlowRowForColumns.Guid;
			if (!guid5.HasValue)
			{
				return false;
			}
			return !guid5.HasValue || guid5.GetValueOrDefault() == guid6;
		});
		if (dataLineageDiagramContainer == null)
		{
			return;
		}
		foreach (DataLineageDiagramColumn mainItemColumn in dataLineageDiagramContainer.Items.OfType<DataLineageDiagramColumn>())
		{
			foreach (DataLineageColumnsFlowRow relatedColumn in mainItemColumn.DataLineageColumnsFlowRows)
			{
				foreach (DataLineageDiagramColumn column in columns.Where((DataLineageDiagramColumn x) => x.DataLineageColumnsFlowRows.Any((DataLineageColumnsFlowRow c) => c.Guid == relatedColumn.Guid)))
				{
					if (mainItemColumn == column)
					{
						continue;
					}
					if (column.FlowDirection == FlowDirectionEnum.Direction.IN)
					{
						if (!diagramControl.Items.OfType<DiagramConnector>().Any(delegate(DiagramConnector x)
						{
							if ((x.BeginItem as DataLineageDiagramColumn)?.Guid == column.Guid)
							{
								Guid? guid3 = (x.EndItem as DataLineageDiagramColumn)?.Guid;
								Guid guid4 = mainItemColumn.Guid;
								if (!guid3.HasValue)
								{
									return false;
								}
								if (!guid3.HasValue)
								{
									return true;
								}
								return guid3.GetValueOrDefault() == guid4;
							}
							return false;
						}))
						{
							diagramControl.Items.Add(SetConnection(column, mainItemColumn));
						}
					}
					else if (!diagramControl.Items.OfType<DiagramConnector>().Any(delegate(DiagramConnector x)
					{
						if ((x.BeginItem as DataLineageDiagramColumn)?.Guid == mainItemColumn.Guid)
						{
							Guid? guid = (x.EndItem as DataLineageDiagramColumn)?.Guid;
							Guid guid2 = column.Guid;
							if (!guid.HasValue)
							{
								return false;
							}
							if (!guid.HasValue)
							{
								return true;
							}
							return guid.GetValueOrDefault() == guid2;
						}
						return false;
					}))
					{
						diagramControl.Items.Add(SetConnection(mainItemColumn, column));
					}
				}
			}
		}
	}

	public static DataLineageDiagramContainer CreateDiagramContainerWithItem(DataFlowRow flow, int diagramNumber, float positionX, float positionY, int columnsCount, out double columnPositionX, out float columnPositionY, float itemWidth, float imageFieldWidth, float itemHeight, float columnItemHeight, float spaceBetweenItems, int borderSize, Font itemFont)
	{
		DataLineageDiagramContainer dataLineageDiagramContainer = CreateFlowDiagramItem(flow, diagramNumber, positionX, itemWidth, imageFieldWidth, itemHeight, borderSize, itemFont);
		DataLineageDiagramContainer dataLineageDiagramContainer2 = new DataLineageDiagramContainer
		{
			Shape = StandardContainers.Plain,
			ShowHeader = false,
			Size = new Size((int)dataLineageDiagramContainer.Width, (int)(dataLineageDiagramContainer.Height + (float)columnsCount * columnItemHeight)),
			DiagramNumber = diagramNumber,
			DataProcess = flow?.Process,
			DataFlow = flow,
			OriginalBackColor = Color.Transparent,
			OriginalBorderColor = GetObjectBorderColor(flow),
			CanBeHighlighted = false
		};
		dataLineageDiagramContainer2.Appearance.BorderSize = borderSize;
		dataLineageDiagramContainer2.Appearance.BackColor = Color.Transparent;
		dataLineageDiagramContainer2.Appearance.BorderColor = dataLineageDiagramContainer2.OriginalBorderColor;
		dataLineageDiagramContainer2.Position = new PointFloat(positionX, positionY + spaceBetweenItems);
		dataLineageDiagramContainer2.Items.Add(dataLineageDiagramContainer);
		columnPositionX = (double)(dataLineageDiagramContainer.Position.X + (float)(borderSize / 2)) + 0.5;
		columnPositionY = dataLineageDiagramContainer.Position.Y + dataLineageDiagramContainer.Size.Height + (float)(borderSize / 2);
		return dataLineageDiagramContainer2;
	}
}
