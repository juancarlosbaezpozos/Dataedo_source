namespace Dataedo.App.Tools.DDLGenerating;

public class DDLGeneratorSettings
{
	public string OpenEscapeCharacter { get; set; } = "[";


	public string CloseEscapeCharacter { get; set; } = "]";


	public bool ShowSchema { get; set; } = true;


	public bool ShowNullability { get; set; } = true;


	public bool ShowDefault { get; set; } = true;


	public bool CreatePKScript { get; set; } = true;


	public bool CreateUKScript { get; set; } = true;


	public bool CreateFKScript { get; set; } = true;


	public DDLGeneratorSettings(EscapeCharacterModeEnum escapeCharacterMode = EscapeCharacterModeEnum.SQLServerLike, bool showSchema = true, bool showNullability = true, bool showDefault = true, bool createPKScript = true, bool createUKScript = true, bool createFKScript = true)
	{
		EscapeCharacterPair escapeCharacterPair = EscapeCharacter.Character[escapeCharacterMode];
		OpenEscapeCharacter = escapeCharacterPair.OpenCharacter;
		CloseEscapeCharacter = escapeCharacterPair.CloseCharacter;
		ShowSchema = showSchema;
		ShowNullability = showNullability;
		ShowDefault = showDefault;
		CreatePKScript = createPKScript;
		CreateUKScript = createUKScript;
		CreateFKScript = createFKScript;
	}
}
