using System;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json;

public static class LinkGenerator
{
	public static string ToId(string type, int id)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(type);
		if (!objectType.HasValue)
		{
			throw new Exception($"Cannot generate link to object with id {id} of type '{type}'.");
		}
		return ToId(objectType.Value, id);
	}

	public static string ToId(SharedObjectTypeEnum.ObjectType type, int id)
	{
		return ToObjectId(type, id);
	}

	public static string ToId(string type, int moduleId, int id)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(type);
		if (!objectType.HasValue)
		{
			throw new Exception($"Cannot generate link to module object with id {id} of type '{type}' in module with id {moduleId}.");
		}
		return ToId(objectType.Value, moduleId, id);
	}

	public static string ToId(SharedObjectTypeEnum.ObjectType type, int moduleId, int id)
	{
		return ToModuleObjectId(moduleId) + ToId(type, id);
	}

	public static string ToObjectId(string type, int id)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(type);
		if (!objectType.HasValue)
		{
			throw new Exception($"Cannot generate uid for object with id {id} of type '{type}'.");
		}
		return ToObjectId(objectType.Value, id);
	}

	public static string ToObjectId(SharedObjectTypeEnum.ObjectType type, int id)
	{
		return type switch
		{
			SharedObjectTypeEnum.ObjectType.Database => ToDocumntationObjectId(id), 
			SharedObjectTypeEnum.ObjectType.Module => ToModuleObjectId(id), 
			SharedObjectTypeEnum.ObjectType.Table => ToTableObjectId(id), 
			SharedObjectTypeEnum.ObjectType.View => ToViewObjectId(id), 
			SharedObjectTypeEnum.ObjectType.Procedure => ToProcedureObjectId(id), 
			SharedObjectTypeEnum.ObjectType.Function => ToFunctionObjectId(id), 
			SharedObjectTypeEnum.ObjectType.Structure => ToStructureObjectId(id), 
			SharedObjectTypeEnum.ObjectType.BusinessGlossary => ToBusinessGlossaryObjectId(id), 
			SharedObjectTypeEnum.ObjectType.Term => ToTermObjectId(id), 
			_ => throw new Exception($"Cannot generate link to object with id {id} of type '{SharedObjectTypeEnum.TypeToString(type)}'."), 
		};
	}

	public static string ToObjectId(SharedObjectTypeEnum.ObjectType? type, int id)
	{
		if (!type.HasValue)
		{
			throw new Exception($"Cannot generate link to object with id {id}, type not specified.");
		}
		return ToObjectId(type.Value, id);
	}

	public static string ToDocumntationObjectId(int id)
	{
		return $"d{id}";
	}

	public static string ToModuleObjectId(int id)
	{
		return $"m{id}";
	}

	public static string ToTableObjectId(int id)
	{
		return $"t{id}";
	}

	public static string ToViewObjectId(int id)
	{
		return $"v{id}";
	}

	public static string ToProcedureObjectId(int id)
	{
		return $"p{id}";
	}

	public static string ToFunctionObjectId(int id)
	{
		return $"f{id}";
	}

	public static string ToStructureObjectId(int id)
	{
		return $"s{id}";
	}

	public static string ToBusinessGlossaryObjectId(int id)
	{
		return $"bg{id}";
	}

	public static string ToTermObjectId(int id)
	{
		return $"term{id}";
	}
}
