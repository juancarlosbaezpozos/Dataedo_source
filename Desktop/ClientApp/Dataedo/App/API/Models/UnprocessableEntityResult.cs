using System.Collections.Generic;

namespace Dataedo.App.API.Models;

internal class UnprocessableEntityResult : MessageResult
{
	public Dictionary<string, string[]> Errors { get; set; }
}
