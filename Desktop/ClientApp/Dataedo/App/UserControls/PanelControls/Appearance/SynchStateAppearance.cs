using Dataedo.App.Properties;
using Dataedo.App.Tools.UI;

namespace Dataedo.App.UserControls.PanelControls.Appearance;

public class SynchStateAppearance
{
	public enum SynchState
	{
		SynchronizationNotRequired = 1,
		Synchronize = 2,
		Synchronized = 3,
		Failed = 4
	}

	private InfoUserControl infoUserControl;

	public SynchStateAppearance(InfoUserControl infoUserControl)
	{
		this.infoUserControl = infoUserControl;
	}

	public void SetAppearance(SynchState synchState, string changesDescription = null)
	{
		switch (synchState)
		{
		case SynchState.SynchronizationNotRequired:
			infoUserControl.Description = changesDescription;
			infoUserControl.BackgroundColor = SkinsManager.CurrentSkin.SynchronizationNotRequiredBackColor;
			infoUserControl.ForeColor = SkinsManager.CurrentSkin.SynchronizationNotRequiredForeColor;
			infoUserControl.Image = Resources.synchronization_not_required_32;
			break;
		case SynchState.Synchronize:
			infoUserControl.Description = changesDescription;
			infoUserControl.BackgroundColor = SkinsManager.CurrentSkin.SynchronizeBackColor;
			infoUserControl.ForeColor = SkinsManager.CurrentSkin.SynchronizeForeColor;
			infoUserControl.Image = Resources.import_changes_32;
			break;
		case SynchState.Synchronized:
			infoUserControl.Description = "Synchronization successful " + changesDescription;
			infoUserControl.BackgroundColor = SkinsManager.CurrentSkin.SynchronizationSuccessfulBackColor;
			infoUserControl.ForeColor = SkinsManager.CurrentSkin.SynchronizationSuccessfulForeColor;
			infoUserControl.Image = Resources.synchronization_success_32;
			break;
		case SynchState.Failed:
			infoUserControl.Description = "Synchronization failed";
			infoUserControl.BackgroundColor = SkinsManager.CurrentSkin.SynchronizationFailedBackColor;
			infoUserControl.ForeColor = SkinsManager.CurrentSkin.SynchronizationFailedForeColor;
			infoUserControl.Image = Resources.synchronization_failed_32;
			break;
		}
	}
}
