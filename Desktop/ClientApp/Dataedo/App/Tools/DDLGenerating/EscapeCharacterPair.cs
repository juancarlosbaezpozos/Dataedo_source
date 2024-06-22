namespace Dataedo.App.Tools.DDLGenerating;

public struct EscapeCharacterPair
{
	public string OpenCharacter { get; set; }

	public string CloseCharacter { get; set; }

	public EscapeCharacterPair(string openCharacter, string closeCharacter)
	{
		OpenCharacter = openCharacter;
		CloseCharacter = closeCharacter;
	}
}
