using System.Drawing;
using Dataedo.App.Classes.Synchronize.DataLineage;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageDiagramShape : DiagramShape, IDataLineageDiagramItem, IDiagramItem
{
	public int DiagramNumber { get; set; }

	public DataFlowRow DataFlow { get; set; }

	public DataProcessRow DataProcess { get; set; }

	public Color OriginalBackColor { get; set; }

	public Color OriginalBorderColor { get; set; }

	public bool CanBeHighlighted { get; set; }
}
