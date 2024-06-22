using Dataedo.Api.RepositoryAccess;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class TargetGlossaryEditPermissionAttribute : GlossaryEditPermissionAttribute
{
	public TargetGlossaryEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "glossaryId";
	}
}
