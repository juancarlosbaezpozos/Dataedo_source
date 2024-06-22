using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.UserControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.XtraBars;

namespace Dataedo.App.Data.MetadataServer.History;

internal static class HistoryGeneralHelper
{
	internal static int? FindDatabaseId(int? databaseId, BasicRow column)
	{
		if (databaseId.HasValue || column == null)
		{
			return databaseId;
		}
		if (!databaseId.HasValue && column is ColumnRow columnRow)
		{
			databaseId = columnRow?.DocumentationId;
		}
		if (!databaseId.HasValue && column is ColumnRowSummary columnRowSummary)
		{
			databaseId = columnRowSummary?.DatabaseId;
		}
		return databaseId;
	}

	internal static bool DontSaveHistory(int? databaseId, int? objectId, bool saveToHistory, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (DB.History.SavingEnabled && databaseId.HasValue && objectId.HasValue && objectId != -1 && databaseId != -1 && !IsNotAcceptedType(objectType))
		{
			return !saveToHistory;
		}
		return true;
	}

	internal static bool IsNotAcceptedType(SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (!objectType.HasValue)
		{
			return true;
		}
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Database:
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
		case SharedObjectTypeEnum.ObjectType.Column:
		case SharedObjectTypeEnum.ObjectType.Module:
		case SharedObjectTypeEnum.ObjectType.Trigger:
		case SharedObjectTypeEnum.ObjectType.Parameter:
		case SharedObjectTypeEnum.ObjectType.Relation:
		case SharedObjectTypeEnum.ObjectType.Key:
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
		case SharedObjectTypeEnum.ObjectType.Term:
		case SharedObjectTypeEnum.ObjectType.Category:
		case SharedObjectTypeEnum.ObjectType.Rule:
		case SharedObjectTypeEnum.ObjectType.Policy:
		case SharedObjectTypeEnum.ObjectType.Other:
			return false;
		default:
			return true;
		}
	}

	internal static bool CheckAreValuesDiffrent(string value1, string value2)
	{
		if (value1 != value2)
		{
			if (string.IsNullOrEmpty(value1))
			{
				return !string.IsNullOrEmpty(value2);
			}
			return true;
		}
		return false;
	}

	internal static bool CheckAreValuesDiffrentAndFirstValueIsNullForImport(string value1, string value2)
	{
		if (value1 != value2 && (!string.IsNullOrEmpty(value1) || !string.IsNullOrEmpty(value2)))
		{
			return string.IsNullOrEmpty(value1);
		}
		return false;
	}

	internal static bool CheckAreHtmlValuesAreDiffrent(string descriptionPlainNew, string htmlTextNew, string descriptionPlainOld, string htmlTextNewOld)
	{
		if (CheckAreValuesDiffrent(descriptionPlainNew, descriptionPlainOld))
		{
			return true;
		}
		return CheckAreValuesDiffrent(PrepareValue.TrimAllWithInplaceCharArray(htmlTextNew), PrepareValue.TrimAllWithInplaceCharArray(htmlTextNewOld));
	}

	internal static string GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (!objectType.HasValue)
		{
			return "tables";
		}
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
			return "tables";
		case SharedObjectTypeEnum.ObjectType.Column:
			return "columns";
		case SharedObjectTypeEnum.ObjectType.Term:
		case SharedObjectTypeEnum.ObjectType.Category:
		case SharedObjectTypeEnum.ObjectType.Rule:
		case SharedObjectTypeEnum.ObjectType.Policy:
		case SharedObjectTypeEnum.ObjectType.Other:
			return "glossary_terms";
		case SharedObjectTypeEnum.ObjectType.Database:
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			return "databases";
		case SharedObjectTypeEnum.ObjectType.Module:
			return "modules";
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
			return "procedures";
		case SharedObjectTypeEnum.ObjectType.Parameter:
			return "parameters";
		case SharedObjectTypeEnum.ObjectType.Trigger:
			return "triggers";
		case SharedObjectTypeEnum.ObjectType.Relation:
			return "table_relations";
		case SharedObjectTypeEnum.ObjectType.Key:
			return "unique_constraints";
		default:
			return "tables";
		}
	}

	internal static bool IsWithHtmlDescription(string columnName, string tableName)
	{
		switch (tableName)
		{
		case "tables":
		case "glossary_terms":
		case "databases":
		case "procedures":
		case "modules":
			return columnName == "description";
		default:
			return false;
		}
	}

	internal static string UppercaseFirst(string input)
	{
		if (string.IsNullOrEmpty(input) || (input != null && input.Length < 2))
		{
			return null;
		}
		return char.ToUpper(input[0]) + input.Substring(1);
	}

	internal static void AddHistoryBarButton(BulkCopyGridUserControl gridView, PopupMenu popupMenu)
	{
		if (gridView != null && popupMenu != null && gridView.FocusedColumn != null && gridView.FocusedColumn.FieldName != null && popupMenu.ItemLinks != null)
		{
			BarItemLink barItemLink = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals("viewHistory") ?? false);
			if (barItemLink != null)
			{
				barItemLink.Visible = gridView.GetSelectedRows().Count() == 1 && (gridView.FocusedColumn.Name == "titleTableGridColumn" || gridView.FocusedColumn.FieldName.StartsWith("Field"));
			}
		}
	}

	internal static bool IsNotFieldForHistory(string fieldName)
	{
		switch (fieldName)
		{
		default:
			return !fieldName.ToLower().StartsWith("field");
		case "title":
		case "description":
			return false;
		case null:
			return true;
		}
	}
}
