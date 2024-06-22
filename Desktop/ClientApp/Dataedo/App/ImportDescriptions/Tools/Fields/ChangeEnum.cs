using System;
using System.Drawing;
using Dataedo.App.Tools.UI;

namespace Dataedo.App.ImportDescriptions.Tools.Fields;

public class ChangeEnum
{
	public enum Change
	{
		NoChange = 0,
		New = 1,
		Erase = 2,
		Update = 3
	}

	public static string ToDisplayName(Change change)
	{
		return change switch
		{
			Change.NoChange => "No change", 
			Change.New => "New", 
			Change.Erase => "Erase", 
			Change.Update => "Update", 
			_ => throw new ArgumentException($"Provided change ({change}) is not valid."), 
		};
	}

	public static Color GetTextColor(Change change, bool isSelected)
	{
		return change switch
		{
			Change.NoChange => SkinsManager.CurrentSkin.ImportDescriptionsNoChangeForeColor, 
			Change.New => SkinsManager.CurrentSkin.ImportDescriptionsNewForeColor, 
			Change.Erase => SkinsManager.CurrentSkin.ImportDescriptionsEraseForeColor, 
			Change.Update => SkinsManager.CurrentSkin.ImportDescriptionsUpdateForeColor, 
			_ => throw new ArgumentException($"Provided change ({change}) is not valid."), 
		};
	}
}
