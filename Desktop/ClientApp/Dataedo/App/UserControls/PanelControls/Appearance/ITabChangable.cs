using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls.PanelControls.Appearance;

public interface ITabChangable
{
	void ChangeTab(SharedObjectTypeEnum.ObjectType? type);
}
