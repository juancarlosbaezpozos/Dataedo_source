using System.ComponentModel;
using DevExpress.Utils;

namespace Dataedo.App.UserControls;

public class ToolTipControllerUserControl : ToolTipController
{
	private const int InitialDelayValue = 250;

	private const DefaultBoolean CloseOnClickValue = DefaultBoolean.False;

	[DefaultValue(250)]
	public new int InitialDelay => 250;

	[DefaultValue(DefaultBoolean.False)]
	public new DefaultBoolean CloseOnClick => DefaultBoolean.False;

	public ToolTipControllerUserControl()
	{
		SetParameters();
	}

	public ToolTipControllerUserControl(IContainer container)
		: base(container)
	{
		SetParameters();
	}

	private void SetParameters()
	{
		base.InitialDelay = 250;
		base.CloseOnClick = DefaultBoolean.False;
	}

	protected override void HideToolWindow()
	{
	}
}
