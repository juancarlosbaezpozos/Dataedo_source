using System;
using Dataedo.Repository.Services.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Dataedo.Api.Configuration;

/// <summary>
/// The class providing exception handling in controllers.
/// </summary>
public static class CustomExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds exception handler to application's request pipeline.
    /// </summary>
    /// <param name="app">The application's request pipeline object.</param>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(delegate (IApplicationBuilder configure)
        {
            configure.Run(async delegate (HttpContext context)
            {
                IExceptionHandlerPathFeature exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                Exception error = exceptionFeature.Error;
                if (error is ObjectNotFoundException)
                {
                    context.Response.StatusCode = 404;
                }
                else if (error is UnauthorizedException || error is UnauthorizedAccessException || error is SecurityTokenException)
                {
                    context.Response.StatusCode = 401;
                }
                else if (error is BadRequestException || error is ArgumentException)
                {
                    context.Response.StatusCode = 400;
                }
                else if (error is InvalidLicenseKeyException invalidLicenseKeyException)
                {
                    context.Response.StatusCode = 422;
                    context.Response.Headers.Add("validation-result", invalidLicenseKeyException.ValidationResult.ToString());
                }
                else
                {
                    FormErrorsException formErrors = error as FormErrorsException;
                    if (formErrors != null || error is InvalidValueException || error is AlreadyExistsException)
                    {
                        context.Response.StatusCode = 422;
                    }
                    else if (error is ForbiddenException)
                    {
                        context.Response.StatusCode = 403;
                    }
                    else if (error is InvalidOperationException)
                    {
                        context.Response.StatusCode = 405;
                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                    }
                }
                context.Response.ContentType = "application/json";
                string resultJson = JsonConvert.SerializeObject(error);
                await context.Response.WriteAsync(resultJson);
            });
        });
        return app;
    }
}
