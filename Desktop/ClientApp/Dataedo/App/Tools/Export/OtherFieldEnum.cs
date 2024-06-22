using System;

namespace Dataedo.App.Tools.Export;

public class OtherFieldEnum
{
	public enum OtherField
	{
		Description = 0,
		Title = 1,
		DataType = 2,
		Nullable = 3,
		Identity = 4,
		DefaultComputed = 5
	}

	public static string ToStringForDisplay(OtherField otherField)
	{
		return otherField switch
		{
			OtherField.Description => "Description", 
			OtherField.Title => "Title", 
			OtherField.DataType => "Data type", 
			OtherField.Nullable => "Nullable", 
			OtherField.Identity => "Identity", 
			OtherField.DefaultComputed => "Default/computed", 
			_ => throw new ArgumentException($"Provided value ({otherField}) is not valid."), 
		};
	}
}
