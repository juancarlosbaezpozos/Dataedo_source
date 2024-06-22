using System.Drawing;
using DevExpress.Utils;

namespace Dataedo.App.Onboarding;

public class OnboardingMessage
{
	public string Message { get; set; }

	public Size Size { get; set; }

	public BeakPanelBeakLocation BeakPanelBeakLocation { get; set; }

	public Point Offset { get; set; }

	public bool OffsetFromEnd { get; set; }

	public OnboardingMessage(string message)
	{
		Message = message;
	}

	public OnboardingMessage(string message, Size size, BeakPanelBeakLocation beakPanelBeakLocation)
		: this(message)
	{
		Size = size;
		BeakPanelBeakLocation = beakPanelBeakLocation;
	}

	public OnboardingMessage(string message, Size size, BeakPanelBeakLocation beakPanelBeakLocation, Point offset, bool offsetFromEnd)
		: this(message, size, beakPanelBeakLocation)
	{
		Offset = offset;
		OffsetFromEnd = offsetFromEnd;
	}
}
