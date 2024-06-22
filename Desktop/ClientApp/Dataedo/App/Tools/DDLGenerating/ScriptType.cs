namespace Dataedo.App.Tools.DDLGenerating;

public static class ScriptType
{
	public enum ScriptTypeEnum
	{
		New = 0,
		Update = 1
	}

	public static ScriptTypeEnum StringToType(string type)
	{
		if (!(type == "new"))
		{
			if (type == "update")
			{
				return ScriptTypeEnum.Update;
			}
			return ScriptTypeEnum.New;
		}
		return ScriptTypeEnum.New;
	}
}
