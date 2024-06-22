using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dataedo.App.Tools;

public static class StaticDataHelper
{
	public static string GetStaticDataValuesAsJson(bool excludeCommands = true, bool excludeLicenses = true, Form owner = null)
	{
		string result = string.Empty;
		try
		{
			Dictionary<string, object> item = typeof(StaticData).GetFields().ToDictionary((FieldInfo x) => x.Name, (FieldInfo x) => (!x.GetType().IsEnum) ? x.GetValue(null) : Convert.ChangeType(x.GetValue(null), Enum.GetUnderlyingType(x.GetValue(null).GetType())));
			Dictionary<string, object> dictionary = typeof(StaticData).GetProperties().ToDictionary((PropertyInfo x) => x.Name, (PropertyInfo x) => (!x.GetType().IsEnum) ? x.GetValue(null) : Convert.ChangeType(x.GetValue(null), Enum.GetUnderlyingType(x.GetValue(null).GetType())));
			if (excludeCommands)
			{
				dictionary.Remove("Commands");
			}
			if (excludeLicenses)
			{
				dictionary.Remove("Licenses");
			}
			result = JsonConvert.SerializeObject(new List<Dictionary<string, object>> { item, dictionary }.SelectMany((Dictionary<string, object> x) => x).ToDictionary((KeyValuePair<string, object> x) => x.Key, (KeyValuePair<string, object> y) => y.Value), Formatting.Indented, new JsonSerializerSettings
			{
				Converters = { (JsonConverter)new StringEnumConverter() }
			});
			return result;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to get StaticData values.", owner);
			return result;
		}
	}
}
