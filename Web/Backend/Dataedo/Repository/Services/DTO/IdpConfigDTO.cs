namespace Dataedo.Repository.Services.DTO;

public class IdpConfigDTO
{
	public string DisplayName { get; private set; }

	public IdpConfigDTO(string displayName)
	{
		DisplayName = displayName;
	}
}
