using System.Collections.Generic;

namespace Dataedo.App.Tools.DDLGenerating;

public static class EscapeCharacter
{
	public static Dictionary<EscapeCharacterModeEnum, EscapeCharacterPair> Character { get; } = new Dictionary<EscapeCharacterModeEnum, EscapeCharacterPair>
	{
		[EscapeCharacterModeEnum.None] = new EscapeCharacterPair(string.Empty, string.Empty),
		[EscapeCharacterModeEnum.SQLServerLike] = new EscapeCharacterPair("[", "]"),
		[EscapeCharacterModeEnum.MySQLLike] = new EscapeCharacterPair("`", "`"),
		[EscapeCharacterModeEnum.OracleLike] = new EscapeCharacterPair("\"", "\"")
	};

}
