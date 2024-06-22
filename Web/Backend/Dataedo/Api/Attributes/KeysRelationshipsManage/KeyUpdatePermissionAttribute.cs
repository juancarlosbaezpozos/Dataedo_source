using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Keys.DTO;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.KeysRelationshipsManage;

public class KeyUpdatePermissionAttribute : BaseKeysManagePermissionAttribute
{
	public KeyUpdatePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "model";
	}

	public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		await base.OnActionExecutionAsync<KeyUpdateDTO>(context, next);
	}

	protected override int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		KeyUpdateDTO param = (KeyUpdateDTO)context.ActionArguments[parameter.Name];
		return param.Id;
	}
}
