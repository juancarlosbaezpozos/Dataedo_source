using System.Collections.Generic;
using Dataedo.App.Tools.Search;
using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls.PanelControls.Appearance;

public interface ITabSettable
{
	void SetTab(ResultItem row, SharedObjectTypeEnum.ObjectType? type, bool changeTab, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, params int?[] elementId);

	void ClearHighlights(bool keepSearchActive);

	void ForceLayoutChange(bool forceAll = false);

	void SetLastResult(ResultItem result);
}
