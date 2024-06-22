using System.Linq;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Configuration.DTO;
using Dataedo.Repository.Services.Features.Configuration.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.Configuration;

public class StandardLoginMethodAttribute : LoginMethodAttribute
{
	private const string loginMethodParameterName = "loginMethod";

	public StandardLoginMethodAttribute(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager)
	{
	}

	protected override async Task OnActionExecutionAsync(AuthenticationDTO authenticationMethods, ActionExecutingContext context, ActionExecutionDelegate next)
	{
		_ = context.HttpContext.Request.Path;
		ParameterDescriptor parameterDescriptor = context.ActionDescriptor.Parameters.FirstOrDefault((ParameterDescriptor x) => x.Name.Equals("loginMethod"));
		LoginMethodEnum.Enum parameterValue = (LoginMethodEnum.Enum)context.ActionArguments[parameterDescriptor.Name];
		if ((parameterValue == LoginMethodEnum.Enum.ActiveDirectory && authenticationMethods.AD.Value == true) || (parameterValue == LoginMethodEnum.Enum.SqlServer && authenticationMethods.SQL.Value == true) || (parameterValue == LoginMethodEnum.Enum.LocalWindows && authenticationMethods.Windows.Value == true))
		{
			await next();
		}
		else
		{
			context.Result = new StatusCodeResult(405);
		}
	}
}
