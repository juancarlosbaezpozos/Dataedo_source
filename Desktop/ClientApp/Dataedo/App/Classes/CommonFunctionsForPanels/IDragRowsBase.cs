using System.Collections.Generic;

namespace Dataedo.App.Classes.CommonFunctionsForPanels;

public interface IDragRowsBase
{
	List<object> DraggedRows { get; set; }

	void AddEvents();
}
