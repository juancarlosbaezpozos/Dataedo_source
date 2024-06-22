using System.Collections.Generic;

namespace Dataedo.App.Enums;

public static class GeneralConnectionTypeEnum
{
	public enum GeneralConnectionType
	{
		None = 0,
		Values = 1,
		ConnectionString = 2
	}

	public static GeneralConnectionType StringToType(string generalConnectionType)
	{
		if (generalConnectionType == "CONNECTION_STRING")
		{
			return GeneralConnectionType.ConnectionString;
		}
		return GeneralConnectionType.Values;
	}

	public static GeneralConnectionType ParamStringToType(string connectionType)
	{
		if (!(connectionType == "ConnectionString"))
		{
			if (connectionType == "Values")
			{
				return GeneralConnectionType.Values;
			}
			return GeneralConnectionType.None;
		}
		return GeneralConnectionType.ConnectionString;
	}

	public static string TypeToParamString(GeneralConnectionType? generalConnectionType)
	{
		return generalConnectionType switch
		{
			GeneralConnectionType.ConnectionString => "CONNECTION_STRING", 
			GeneralConnectionType.Values => "VALUES", 
			_ => "NONE", 
		};
	}

	public static string TypeToString(GeneralConnectionType? generalConnectionType)
	{
		if (generalConnectionType.HasValue && generalConnectionType.GetValueOrDefault() == GeneralConnectionType.ConnectionString)
		{
			return "CONNECTION_STRING";
		}
		return "VALUES";
	}

	public static string TypeToStringForDisplay(GeneralConnectionType? generalConnectionType)
	{
		return generalConnectionType switch
		{
			GeneralConnectionType.Values => "Values", 
			GeneralConnectionType.ConnectionString => "Connection string", 
			_ => null, 
		};
	}

	public static Dictionary<GeneralConnectionType, string> GetGeneralConnectionTypes()
	{
		return new Dictionary<GeneralConnectionType, string>
		{
			{
				GeneralConnectionType.Values,
				TypeToStringForDisplay(GeneralConnectionType.Values)
			},
			{
				GeneralConnectionType.ConnectionString,
				TypeToStringForDisplay(GeneralConnectionType.ConnectionString)
			}
		};
	}
}
