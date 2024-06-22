using System;
using System.Collections.Generic;

namespace Dataedo.App.Enums;

public static class SalesforceConnectionTypeEnum
{
	public enum SalesforceConnectionType
	{
		None = 0,
		Interactive = 1,
		Values = 2
	}

	public static SalesforceConnectionType StringToType(string connectionType)
	{
		if (connectionType == "VALUES")
		{
			return SalesforceConnectionType.Values;
		}
		return SalesforceConnectionType.Interactive;
	}

	public static SalesforceConnectionType ParamStringToType(string connectionType)
	{
		if (!(connectionType == "Interactive"))
		{
			if (connectionType == "Values")
			{
				return SalesforceConnectionType.Values;
			}
			return SalesforceConnectionType.None;
		}
		return SalesforceConnectionType.Interactive;
	}

	public static string TypeToParamString(SalesforceConnectionType? connectionType)
	{
		return connectionType switch
		{
			SalesforceConnectionType.Interactive => "INTERACTIVE", 
			SalesforceConnectionType.Values => "VALUES", 
			_ => "NONE", 
		};
	}

	public static string TypeToString(SalesforceConnectionType? connectionType)
	{
		if (connectionType.HasValue && connectionType.GetValueOrDefault() == SalesforceConnectionType.Values)
		{
			return "VALUES";
		}
		return "INTERACTIVE";
	}

	public static string TypeToStringForDisplay(SalesforceConnectionType? connectionType)
	{
		return connectionType switch
		{
			SalesforceConnectionType.Values => "Sign-in with values", 
			SalesforceConnectionType.Interactive => "Interactive sign-in", 
			_ => null, 
		};
	}

	public static Dictionary<SalesforceConnectionType, string> GetSalesforceConnectionTypes()
	{
		return new Dictionary<SalesforceConnectionType, string>
		{
			{
				SalesforceConnectionType.Interactive,
				TypeToStringForDisplay(SalesforceConnectionType.Interactive)
			},
			{
				SalesforceConnectionType.Values,
				TypeToStringForDisplay(SalesforceConnectionType.Values)
			}
		};
	}

	public static string GetSalesforceConnectionTypeDescriptions(SalesforceConnectionType? connectionType)
	{
		return connectionType switch
		{
			SalesforceConnectionType.Values => "A more advanced login option." + Environment.NewLine + "Requires configuration of Connected App in Salesforce by an administrator." + Environment.NewLine + Environment.NewLine + "Use the Learn More button above to find out more about that option.", 
			SalesforceConnectionType.Interactive => "Default interactive online login option.", 
			_ => null, 
		};
	}
}
