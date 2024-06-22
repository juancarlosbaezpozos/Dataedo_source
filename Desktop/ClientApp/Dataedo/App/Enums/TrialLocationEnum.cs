using System.Collections.Generic;

namespace Dataedo.App.Enums;

public static class TrialLocationEnum
{
	public enum TrialLocation
	{
		Default = 0,
		Relation = 1,
		Key = 2,
		Trigger = 3,
		Dependency = 4,
		Function = 5,
		Procedure = 6,
		ERD = 7,
		About = 8,
		Limit = 9,
		Add = 10,
		Ribbon = 11,
		Param = 12,
		Search = 13,
		Banner = 14,
		Export = 15,
		Login = 16,
		CustomizeTemplate = 17,
		Column = 18,
		SchemaImportsAndChanges = 19,
		Term = 20,
		BusinessGlossary = 21
	}

	private static Dictionary<TrialLocation, string> dictonary = new Dictionary<TrialLocation, string>
	{
		[TrialLocation.Default] = string.Empty,
		[TrialLocation.Relation] = "rel",
		[TrialLocation.Key] = "key",
		[TrialLocation.Trigger] = "trig",
		[TrialLocation.Dependency] = "dep",
		[TrialLocation.Function] = "fun",
		[TrialLocation.Procedure] = "proc",
		[TrialLocation.ERD] = "erd",
		[TrialLocation.About] = "about",
		[TrialLocation.Limit] = "limit",
		[TrialLocation.Add] = "add",
		[TrialLocation.Ribbon] = "ribb",
		[TrialLocation.Param] = "param",
		[TrialLocation.Search] = "search",
		[TrialLocation.Banner] = "main",
		[TrialLocation.Export] = "exp",
		[TrialLocation.Login] = "login",
		[TrialLocation.CustomizeTemplate] = "cust",
		[TrialLocation.Column] = "col",
		[TrialLocation.SchemaImportsAndChanges] = "sct",
		[TrialLocation.Term] = "term",
		[TrialLocation.BusinessGlossary] = "bg"
	};

	public static string ValueToString(TrialLocation value)
	{
		return dictonary[value];
	}
}
