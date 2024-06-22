using System;
using Dataedo.App.UserControls.DataLineage;

namespace Dataedo.App.UserControls.PanelControls.Appearance;

public interface IPanelWithDataLineage
{
	void RefreshDataLineage(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false);

	void FocusDataLineageTab(int? processId, bool selectDiagramTab = false, bool? showColumns = null);

	DataLineageUserControl GetDataLineageControl();

	void DataLineageButtonsVisibleChanged(object sender, EventArgs e);
}
