using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Dataedo.App.Tools;

public class ToolTips
{
	public static string GetConstraintDescription(UniqueConstraintType.UniqueConstraintTypeEnum type)
	{
		return type switch
		{
			UniqueConstraintType.UniqueConstraintTypeEnum.PK_user => "Primary key (user-defined)", 
			UniqueConstraintType.UniqueConstraintTypeEnum.PK_deleted => "Primary key removed from database", 
			UniqueConstraintType.UniqueConstraintTypeEnum.PK => "Primary key", 
			UniqueConstraintType.UniqueConstraintTypeEnum.PK_disabled => "Primary key disabled", 
			UniqueConstraintType.UniqueConstraintTypeEnum.UK_user => "Unique key (user-defined)", 
			UniqueConstraintType.UniqueConstraintTypeEnum.UK_deleted => "Unique key removed from database", 
			UniqueConstraintType.UniqueConstraintTypeEnum.UK => "Unique key", 
			UniqueConstraintType.UniqueConstraintTypeEnum.UK_disabled => "Unique key disabled", 
			_ => string.Empty, 
		};
	}

	public static string GetRelationDescription(bool isFromDbms, bool isDeleted, Cardinality fkCardinality, Cardinality pkCardinality)
	{
		if (isFromDbms)
		{
			if (!isDeleted)
			{
				return fkCardinality.DisplayName + " to " + pkCardinality.DisplayName.ToLower() + " relationship";
			}
			return fkCardinality.DisplayName + " to " + pkCardinality.DisplayName.ToLower() + " relationship deleted from database";
		}
		return fkCardinality.DisplayName + " to " + pkCardinality.DisplayName.ToLower() + " relationship (user-defined)";
	}

	public static string GetRelationDescription(RelationRow relationRow)
	{
		return GetRelationDescription(relationRow.Source == UserTypeEnum.UserType.DBMS, relationRow.CanBeDeleted(), relationRow.FKCardinality, relationRow.PKCardinality);
	}

	public static string GetRowDescription(GridView gridView, SharedObjectTypeEnum.ObjectType objectType, int rowHandle)
	{
		bool flag = SynchronizeStateEnum.DBStringToState(gridView.GetRowCellValue(rowHandle, "Status") as string) == SynchronizeStateEnum.SynchronizeState.Deleted;
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Column:
			if (!flag)
			{
				return "Column";
			}
			return "Column removed from database";
		case SharedObjectTypeEnum.ObjectType.Trigger:
		{
			string text2 = (gridView.GetRow(rowHandle) as TriggerRow)?.SubtypeDisplayText;
			if (text2 == null)
			{
				return string.Empty;
			}
			string text3 = text2.ToLower();
			bool flag2 = Convert.ToBoolean(gridView.GetRowCellValue(rowHandle, "Disabled"));
			if (flag)
			{
				if (!flag2)
				{
					return text2 + " removed from database";
				}
				return "Disabled " + text3;
			}
			if (!flag2)
			{
				return "Active " + text3;
			}
			return "Disabled " + text3;
		}
		case SharedObjectTypeEnum.ObjectType.Parameter:
			if (!(gridView.GetRow(rowHandle) is ParameterRow parameterRow))
			{
				return string.Empty;
			}
			switch (parameterRow.Mode)
			{
			case ParameterRow.ModeEnum.In:
				if (!flag)
				{
					return "Input";
				}
				return "Input removed from database";
			case ParameterRow.ModeEnum.Out:
				if (!flag)
				{
					return "Output";
				}
				return "Output removed from database";
			case ParameterRow.ModeEnum.InOut:
				if (!flag)
				{
					return "Input/output";
				}
				return "Input/output removed from database";
			default:
				return null;
			}
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
		{
			if (!(gridView.GetRow(rowHandle) is ObjectWithModulesObject objectWithModulesObject))
			{
				return string.Empty;
			}
			string text = ((objectWithModulesObject.Source == "USER") ? " (user-defined)" : null);
			if (!flag)
			{
				return objectWithModulesObject.SubtypeDisplayText + text;
			}
			return objectWithModulesObject.SubtypeDisplayText + " removed from database";
		}
		case SharedObjectTypeEnum.ObjectType.Term:
			return SharedObjectTypeEnum.TypeToStringForSingle(objectType);
		default:
			return string.Empty;
		}
	}

	public static string GetNodeDescription(SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subtype, SynchronizeStateEnum.SynchronizeState synchronizeState, UserTypeEnum.UserType? source)
	{
		switch (type)
		{
		case SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database:
			return "Subject Areas are your custom “folders” to group database objects and describe topics related to your database.\r\nAssign tables, views etc., write a narrative and visualize schema with Entity Relationship Diagram.";
		default:
		{
			string text = ((source == UserTypeEnum.UserType.USER) ? " (user-defined)" : null);
			return synchronizeState switch
			{
				SynchronizeStateEnum.SynchronizeState.Synchronized => SharedObjectSubtypeEnum.TypeToStringForSingle(type, subtype) + text, 
				SynchronizeStateEnum.SynchronizeState.Deleted => SharedObjectSubtypeEnum.TypeToStringForSingle(type, subtype) + " removed from database", 
				_ => new StringBuilder().Append(SynchronizeStateEnum.StateToString(synchronizeState)).Append(" ").Append(SharedObjectSubtypeEnum.TypeToStringForSingle(type, subtype) + text)
					.ToString(), 
			};
		}
		case SharedObjectTypeEnum.ObjectType.Database:
		case SharedObjectTypeEnum.ObjectType.Module:
		case SharedObjectTypeEnum.ObjectType.Folder_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Module:
			return string.Empty;
		}
	}

	public static void ShowToolTip(ToolTipControllerGetActiveObjectInfoEventArgs e, List<ToolTipData> toolTipDataList)
	{
		GridControl grid = e.SelectedControl as GridControl;
		IEnumerable<ToolTipData> enumerable = toolTipDataList.Where((ToolTipData x) => x.GridControl == grid);
		if (grid == null)
		{
			return;
		}
		ToolTipControlInfo toolTipControlInfo = null;
		GridView gridView = grid.ViewCollection[0] as GridView;
		GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.ControlMousePosition);
		if (!gridHitInfo.InRowCell)
		{
			return;
		}
		foreach (ToolTipData item in enumerable)
		{
			if (item == null || gridHitInfo.Column.VisibleIndex != item.ColumnVisibleIndex)
			{
				continue;
			}
			string text = string.Empty;
			switch (item.ObjectType)
			{
			case SharedObjectTypeEnum.ObjectType.Key:
				if (gridView.GetRow(gridHitInfo.RowHandle) is UniqueConstraintRow uniqueConstraintRow)
				{
					text = GetConstraintDescription(uniqueConstraintRow.ConstraintType);
				}
				break;
			case SharedObjectTypeEnum.ObjectType.ColumnPK:
				if (gridView.GetRow(gridHitInfo.RowHandle) is ColumnRow columnRow2)
				{
					text = GetConstraintDescription(columnRow2.UniqueConstraintsDataContainer.FirstItemConstraintType);
				}
				break;
			case SharedObjectTypeEnum.ObjectType.Relation:
				if (gridView.GetRow(gridHitInfo.RowHandle) is RelationRow relationRow)
				{
					text = GetRelationDescription(relationRow);
				}
				break;
			case SharedObjectTypeEnum.ObjectType.Column:
				if (gridView.GetRow(gridHitInfo.RowHandle) is ColumnRow columnRow)
				{
					string text2 = SharedObjectSubtypeEnum.TypeToStringForSingle(columnRow.ObjectType, columnRow.ObjectSubtype);
					text = ((columnRow.Source == UserTypeEnum.UserType.DBMS) ? (text2 ?? "") : (text2 + " (user-defined)"));
				}
				break;
			case SharedObjectTypeEnum.ObjectType.DataLink:
			{
				object row3 = gridView.GetRow(gridHitInfo.RowHandle);
				if (row3 is ColumnRow)
				{
					DataLinkDataContainer dataLinksDataContainer = (row3 as ColumnRow).DataLinksDataContainer;
					if (dataLinksDataContainer != null)
					{
						text = dataLinksDataContainer.DataLinksDescription;
					}
				}
				else if (row3 is DataLinkObjectExtendedForTerms)
				{
					DataLinkObjectExtendedForTerms obj = row3 as DataLinkObjectExtendedForTerms;
					SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(obj.LinkedObjectType);
					if (objectType.HasValue)
					{
						objectType.GetValueOrDefault();
					}
					text = obj.ShortDescriptionFormatted;
				}
				break;
			}
			case SharedObjectTypeEnum.ObjectType.Term:
			{
				object row2 = gridView.GetRow(gridHitInfo.RowHandle);
				if (row2 is TermRelationshipObject)
				{
					text = (row2 as TermRelationshipObject).RelatedTermShortDescriptionFormatted;
				}
				else if (row2 is ObjectWithDataLinkExtended)
				{
					ObjectWithDataLinkExtended objectWithDataLinkExtended = row2 as ObjectWithDataLinkExtended;
					SharedObjectTypeEnum.ObjectType num3 = SharedObjectTypeEnum.StringToType(objectWithDataLinkExtended.TypeForIcon) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
					text = GetNodeDescription(num3, SharedObjectSubtypeEnum.StringToType(num3, objectWithDataLinkExtended.SubtypeForIcon), SynchronizeStateEnum.DBStringToState(objectWithDataLinkExtended.StatusForIcon), UserTypeEnum.ObjectToType(objectWithDataLinkExtended.SourceForIcon));
				}
				else if (row2 is TermObjectWithRelationship)
				{
					text = (row2 as TermObjectWithRelationship).ShortDescriptionFormatted;
				}
				else if (row2 is TermObject)
				{
					text = (row2 as TermObject).TypeTitle;
				}
				break;
			}
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
			case SharedObjectTypeEnum.ObjectType.Trigger:
			case SharedObjectTypeEnum.ObjectType.Parameter:
			{
				object row = gridView.GetRow(gridHitInfo.RowHandle);
				if (row is DataLinkObjectExtendedForTerms)
				{
					DataLinkObjectExtended dataLinkObjectExtended = row as DataLinkObjectExtendedForTerms;
					SharedObjectTypeEnum.ObjectType num = SharedObjectTypeEnum.StringToType(dataLinkObjectExtended.TypeForIcon) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
					text = GetNodeDescription(num, SharedObjectSubtypeEnum.StringToType(num, dataLinkObjectExtended.SubtypeForIcon), SynchronizeStateEnum.DBStringToState(dataLinkObjectExtended.StatusForIcon), UserTypeEnum.ObjectToType(dataLinkObjectExtended.SourceForIcon));
				}
				else if (row is DataLinkObjectExtended)
				{
					DataLinkObjectExtended dataLinkObjectExtended2 = row as DataLinkObjectExtended;
					SharedObjectTypeEnum.ObjectType num2 = SharedObjectTypeEnum.StringToType(dataLinkObjectExtended2.TypeForIcon) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
					text = GetNodeDescription(num2, SharedObjectSubtypeEnum.StringToType(num2, dataLinkObjectExtended2.SubtypeForIcon), SynchronizeStateEnum.DBStringToState(dataLinkObjectExtended2.StatusForIcon), UserTypeEnum.ObjectToType(dataLinkObjectExtended2.SourceForIcon));
				}
				else
				{
					text = ((row is ObjectModel objectModel) ? GetNodeDescription(objectModel.ObjectType, objectModel.ObjectSubtype, SynchronizeStateEnum.SynchronizeState.Synchronized, objectModel.Source) : ((!(row is FieldModel fieldModel)) ? GetRowDescription(gridView, item.ObjectType, gridHitInfo.RowHandle) : GetNodeDescription(fieldModel.ObjectType, fieldModel.ObjectSubtype, SynchronizeStateEnum.SynchronizeState.Synchronized, fieldModel.Source)));
				}
				break;
			}
			}
			if (!string.IsNullOrEmpty(text))
			{
				toolTipControlInfo = new ToolTipControlInfo(new StringBuilder().Append(gridHitInfo.HitTest).Append(gridHitInfo.RowHandle).ToString(), text);
				if (toolTipControlInfo != null)
				{
					e.Info = toolTipControlInfo;
				}
			}
			break;
		}
	}
}
