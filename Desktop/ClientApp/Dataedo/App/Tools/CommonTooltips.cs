using System;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class CommonTooltips
{
	public static string GetModule(SharedObjectTypeEnum.ObjectType objectType)
	{
		return "Subject Areas represent logical areas of our data that group objects in a single or multiple databases. Each object can be assigned to a number of Subject Areas. To create new Subject Area and assign " + SharedObjectTypeEnum.TypeToStringForSingleLower(objectType) + ", select number of " + SharedObjectTypeEnum.TypeToStringForMenu(objectType)?.ToLower() + ", right click and choose “Add to new Subject Area”. To assign to existing Subject Area choose “Assign Subject Area” and select a Subject Area.";
	}

	public static string GetTitle(SharedObjectTypeEnum.ObjectType objectType)
	{
		string text = ((objectType == SharedObjectTypeEnum.ObjectType.Column) ? "column/field" : SharedObjectTypeEnum.TypeToStringForSingleLower(objectType));
		return "Alternative, user-fiendly " + text + " alias provided in Dataedo metadata repository.";
	}

	public static string GetSchema(SharedObjectTypeEnum.ObjectType objectType)
	{
		return SharedObjectTypeEnum.TypeToStringForSingle(objectType) + " schema name in source database.";
	}

	public static string GetName(SharedObjectTypeEnum.ObjectType objectType)
	{
		return SharedObjectTypeEnum.TypeToStringForSingle(objectType) + " name in source database.";
	}

	public static string GetDescription(SharedObjectTypeEnum.ObjectType objectType)
	{
		string text = ((objectType == SharedObjectTypeEnum.ObjectType.Column) ? "column/field" : SharedObjectTypeEnum.TypeToStringForSingleLower(objectType));
		return "Description of " + text + " that can be edited with Dataedo. Description is imported from the source at first import and then it’s maintained in Dataedo." + Environment.NewLine + "You can add multiple descriptive fields using “Custom fields” option.";
	}

	public static string GetType(SharedObjectTypeEnum.ObjectType objectType)
	{
		return "Type of " + SharedObjectTypeEnum.TypeToStringForSingleLower(objectType) + ".";
	}
}
