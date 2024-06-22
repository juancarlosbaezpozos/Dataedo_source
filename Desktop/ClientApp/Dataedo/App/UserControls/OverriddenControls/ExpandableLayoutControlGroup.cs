using System;
using System.ComponentModel;
using System.Windows.Forms;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Properties;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.XtraLayout;

namespace Dataedo.App.UserControls.OverriddenControls;

public class ExpandableLayoutControlGroup : LayoutControlGroup
{
	private const string InternalButtonCaption = "ExpandCustomButton";

	protected bool InDesignMode;

	private GroupBoxButton expandButton;

	public EventHandler GroupExpandChanged;

	public AnchorStyles ExpandDirection { get; set; }

	public bool IsExpandDisabled { get; set; }

	public ExpandableLayoutControlGroup()
	{
		InDesignMode = base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
		base.CustomButtonClick += ExpandCustomButtonClick;
		base.CustomDrawCaption += ExpandableLayoutControlGroup_CustomDrawCaption;
		base.CustomDrawBackground += ExpandableLayoutControlGroup_CustomDrawBackground;
		base.Shown += ExpandableLayoutControlGroup_Shown;
	}

	private void ExpandableLayoutControlGroup_Shown(object sender, EventArgs e)
	{
		UpdateExpandState();
	}

	private void ExpandableLayoutControlGroup_CustomDrawBackground(object sender, GroupBackgroundCustomDrawEventArgs e)
	{
		e.Handled = true;
	}

	private void ExpandableLayoutControlGroup_CustomDrawCaption(object sender, GroupCaptionCustomDrawEventArgs e)
	{
		e.DefaultDrawImage();
		e.DefaultDrawButtons();
		e.DefaultDrawText();
		e.Handled = true;
	}

	private void UpdateExpandState()
	{
		if (!InDesignMode)
		{
			base.ExpandButtonVisible = false;
			if (expandButton == null)
			{
				expandButton = new GroupBoxButton("ExpandCustomButton", null, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, "", useCaption: false, -1, enabled: true, null, useImage: true, @checked: false, visible: true, null, null, -1);
				CustomHeaderButtons.Add(expandButton);
			}
			if (Expanded)
			{
				SetForExpanded();
			}
			else
			{
				SetForCollapsed();
			}
		}
	}

	private void SetForExpanded()
	{
		switch (ExpandDirection)
		{
		case AnchorStyles.Left:
			base.HeaderButtonsLocation = GroupElementLocation.AfterText;
			expandButton.ImageOptions.Image = Resources.arrow_left;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Near;
			TextLocation = Locations.Top;
			break;
		case AnchorStyles.Right:
			base.HeaderButtonsLocation = GroupElementLocation.BeforeText;
			expandButton.ImageOptions.Image = Resources.arrow_right;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Near;
			TextLocation = Locations.Top;
			break;
		case AnchorStyles.Top:
			base.HeaderButtonsLocation = GroupElementLocation.BeforeText;
			expandButton.ImageOptions.Image = Resources.arrow_down;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Near;
			TextLocation = Locations.Top;
			break;
		case AnchorStyles.Bottom:
			base.HeaderButtonsLocation = GroupElementLocation.BeforeText;
			expandButton.ImageOptions.Image = Resources.arrow_up;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Near;
			TextLocation = Locations.Top;
			break;
		}
		GroupExpandChanged?.Invoke(null, new BoolEventArgs(value: true));
	}

	private void SetForCollapsed()
	{
		switch (ExpandDirection)
		{
		case AnchorStyles.Left:
			base.HeaderButtonsLocation = GroupElementLocation.AfterText;
			expandButton.ImageOptions.Image = Resources.arrow_down;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Far;
			TextLocation = Locations.Left;
			break;
		case AnchorStyles.Right:
			base.HeaderButtonsLocation = GroupElementLocation.BeforeText;
			expandButton.ImageOptions.Image = Resources.arrow_down;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Near;
			TextLocation = Locations.Right;
			break;
		case AnchorStyles.Top:
			base.HeaderButtonsLocation = GroupElementLocation.BeforeText;
			expandButton.ImageOptions.Image = Resources.arrow_up;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Near;
			TextLocation = Locations.Top;
			break;
		case AnchorStyles.Bottom:
			base.HeaderButtonsLocation = GroupElementLocation.BeforeText;
			expandButton.ImageOptions.Image = Resources.arrow_down;
			base.AppearanceGroup.TextOptions.HAlignment = HorzAlignment.Near;
			TextLocation = Locations.Top;
			break;
		}
		GroupExpandChanged?.Invoke(null, new BoolEventArgs(value: false));
	}

	private void ExpandCustomButtonClick(object sender, BaseButtonEventArgs e)
	{
		if (!InDesignMode && !IsExpandDisabled && e.Button is GroupBoxButton groupBoxButton && !(groupBoxButton.Caption != "ExpandCustomButton"))
		{
			if (Expanded)
			{
				SetForCollapsed();
				Expanded = false;
			}
			else
			{
				SetForExpanded();
				Expanded = true;
			}
		}
	}

	public void Expand()
	{
		Expanded = true;
		UpdateExpandState();
	}

	public void Collapse()
	{
		Expanded = false;
		UpdateExpandState();
	}

	public void SetExpandButtonTooltip(string text)
	{
		UpdateExpandState();
		expandButton.ToolTip = text;
	}
}
