using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dataedo.Api.Configuration.Swashbuckle;

/// <summary>
/// The class providing operation filter for adding parameter description and removing parameter for parameters unsupported in Swagger.
/// </summary>
public class ParameterOperationFilter : IOperationFilter
{
	/// <summary>
	/// Applies adding parameter description and removing parameter for parameters unsupported in Swagger.
	/// </summary>
	/// <param name="operation">The operation</param>
	/// <param name="context">The context.</param>
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		if (!context.ApiDescription.ParameterDescriptions.Any((ApiParameterDescription x) => x.Type.IsGenericType && (x.Type.GetGenericTypeDefinition() == typeof(IDictionary<, >) || x.Type.GetGenericTypeDefinition() == typeof(Dictionary<, >))))
		{
			return;
		}
		string operationDescription = operation.Description;
		bool isAnyParameterDescriptionAdded = false;
		List<OpenApiParameter> filterParameters = new List<OpenApiParameter>();
		for (int i = 0; i < context.ApiDescription.ParameterDescriptions.Count; i++)
		{
			ApiParameterDescription apiParameterDescription = context.ApiDescription.ParameterDescriptions[i];
			OpenApiParameter operationParameter = operation.Parameters.FirstOrDefault((OpenApiParameter x) => x.Name.ToLower() == apiParameterDescription.Name.ToLower());
			if (operationParameter == null)
			{
				break;
			}
			if (apiParameterDescription.Type.IsGenericType && (apiParameterDescription.Type.GetGenericTypeDefinition() == typeof(IDictionary<, >) || apiParameterDescription.Type.GetGenericTypeDefinition() == typeof(Dictionary<, >)))
			{
				filterParameters.Add(operationParameter);
			}
		}
		foreach (OpenApiParameter filterParameter in filterParameters)
		{
			filterParameter.Style = ParameterStyle.DeepObject;
			filterParameter.Explode = true;
		}
		if (!isAnyParameterDescriptionAdded)
		{
			operation.Description = operationDescription;
		}
	}
}
