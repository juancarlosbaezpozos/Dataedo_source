using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dataedo.Api.Configuration.Swashbuckle;

/// <summary>
/// The class providing modifying <see cref="!:SwaggerDocument" /> comments for proper HTML result.
/// </summary>
public class FormatDocumentXmlComments : IDocumentFilter
{
	/// <summary>
	/// Applies custom formatting for Swagger document properties.
	/// </summary>
	/// <param name="swaggerDoc">The Swagger document.</param>
	/// <param name="context">The context of filter.</param>
	public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
	{
		foreach (KeyValuePair<string, OpenApiPathItem> path in swaggerDoc.Paths)
		{
			foreach (OpenApiParameter propertyItem in path.Value.Parameters)
			{
				propertyItem.Description = FormatValue(propertyItem.Description);
			}
		}
	}

	/// <summary>
	/// Formats comment values for proper HTML result.
	/// </summary>
	/// <param name="value">The comment value.</param>
	/// <returns>Formatted comment value.</returns>
	private string FormatValue(string value)
	{
		return value?.Replace("<item><para>", "<li>").Replace("</para></item>", "</li>").Replace("<para>", "<br/>")
			.Replace("</para>", string.Empty)
			.Replace("<list type=\"bullet\">", "<ul>")
			.Replace("</list>", "</ul>")
			.Replace("<item>", "<li>")
			.Replace("</item>", "</li>");
	}
}
