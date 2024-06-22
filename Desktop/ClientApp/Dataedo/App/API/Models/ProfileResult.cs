using System.Linq;
using Newtonsoft.Json;

namespace Dataedo.App.API.Models;

public class ProfileResult
{
	[JsonProperty("email")]
	public string Email { get; set; }

	[JsonProperty("firstName")]
	public string FirstName { get; set; }

	[JsonProperty("lastName")]
	public string LastName { get; set; }

	[JsonIgnore]
	public string FullName => string.Join(" ", new string[2] { FirstName, LastName }.Where((string x) => !string.IsNullOrEmpty(x)));
}
