using System.Drawing;
using Dataedo.App.Classes.Synchronize.DataLineage;
using DevExpress.Utils;

namespace Dataedo.App.UserControls.DataLineage;

public interface IDataLineageDiagramItem
{
	int DiagramNumber { get; set; }

	DataFlowRow DataFlow { get; set; }

	DataProcessRow DataProcess { get; set; }

	PointFloat Position { get; set; }

	SizeF Size { get; set; }

	Color OriginalBackColor { get; set; }

	Color OriginalBorderColor { get; set; }

	bool CanBeHighlighted { get; set; }
}
