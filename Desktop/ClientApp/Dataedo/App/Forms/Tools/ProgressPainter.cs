using System.Drawing;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;

namespace Dataedo.App.Forms.Tools;

public static class ProgressPainter
{
	public static readonly Color Color = SkinsManager.CurrentSkin.ProgressPainterColor;

	public static readonly Color TabColor = SkinsManager.CurrentSkin.ProgressPainterTabColor;

	public static void SetEmptyDescriptionCellsBackground(RowCellCustomDrawEventArgs e, ProgressTypeModel progressType, bool showProgress)
	{
		if (progressType.Type == ProgressTypeEnum.TablesAndColumns && showProgress && e.Column.FieldName.ToLower().Equals("description") && !(e.Column.RealColumnEdit is RepositoryItemPictureEdit) && (e.CellValue == null || (e.CellValue != null && string.IsNullOrEmpty(e.CellValue.ToString()))))
		{
			e.Appearance.BackColor = Color;
		}
	}

	public static void SetEmptyProgressCellsBackground(RowCellCustomDrawEventArgs e, ProgressTypeModel progressType, bool showProgress)
	{
		if ((progressType.Type == ProgressTypeEnum.AllDocumentations || progressType.Type == ProgressTypeEnum.SelectedCustomField) && showProgress && e.Column.FieldName.ToLower().Equals(progressType.FieldName?.ToLower()) && !(e.Column.RealColumnEdit is RepositoryItemPictureEdit) && (e.CellValue == null || (e.CellValue != null && string.IsNullOrEmpty(e.CellValue.ToString()))))
		{
			e.Appearance.BackColor = Color;
		}
	}
}
