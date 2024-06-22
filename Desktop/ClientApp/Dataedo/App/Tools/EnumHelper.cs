using System;
using System.ComponentModel;

namespace Dataedo.App.Tools;

public static class EnumHelper
{
	public static string GetAttributeDescription(this Enum enumValue)
	{
		DescriptionAttribute attributeOfType = enumValue.GetAttributeOfType<DescriptionAttribute>();
		if (attributeOfType != null)
		{
			return attributeOfType.Description;
		}
		return string.Empty;
	}

	private static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
	{
		object[] customAttributes = enumVal.GetType().GetMember(enumVal.ToString())[0].GetCustomAttributes(typeof(T), inherit: false);
		if (customAttributes.Length == 0)
		{
			return null;
		}
		return (T)customAttributes[0];
	}
}
