using Dataedo.App.Import.DataLake.Processing.CsvSupport;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Model;

internal class CsvFieldModel : FieldModel
{
	public CsvFieldModel(CsvColumn csvColumn)
	{
		base.Name = csvColumn.HeaderName;
		base.DataType = csvColumn.ColumnDataType.ToString();
		base.DataTypeSize = null;
		base.Nullable = true;
		base.Position = csvColumn.Position;
		base.ParentId = null;
		base.ParentField = null;
		base.Path = null;
		base.Level = 1;
		base.ObjectSubtype = SharedObjectSubtypeEnum.ObjectSubtype.Field;
	}
}
