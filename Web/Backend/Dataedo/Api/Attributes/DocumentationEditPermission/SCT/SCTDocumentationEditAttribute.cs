using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationEditPermission.SCT;

public abstract class SCTDocumentationEditAttribute : BasePermissionAttribute
{
	public SCTDocumentationEditAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "descriptionUpdateDTO";
	}

	protected override int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		DescriptionUpdateDTO param = (DescriptionUpdateDTO)context.ActionArguments[parameter.Name];
		return param.Id;
	}
}
