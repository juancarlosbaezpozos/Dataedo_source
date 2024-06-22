using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Dataedo.Api.AppSettings;
using Dataedo.Api.Attributes.ClassificationView;
using Dataedo.Api.Attributes.CommunityEditManage;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.Configuration;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationEditPermission.SCT;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission.ScriptsViewPermission;
using Dataedo.Api.Attributes.KeysRelationshipsManage;
using Dataedo.Api.Attributes.LineageView;
using Dataedo.Api.Attributes.Profiling;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.Attributes.SystemManagementSettings;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.Authorization.JWT;
using Dataedo.Api.Configuration;
using Dataedo.Api.Configuration.Swashbuckle;
using Dataedo.Api.Middlewares;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Api.Services;
using Dataedo.Repository.Services.Features.Notifications.Interfaces;
using Dataedo.Repository.Services.Features.Notifications.Services;
using Dataedo.Repository.Services.Services;
using Dataedo.Repository.Services.Settings;
using Dataedo.Repository.Services.SqlServer.Services;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Dataedo.Api;

/// <summary>
/// The class providing configuring services and application's request pipeline.
/// </summary>
public class Startup
{
    private readonly IWebHostEnvironment environment;

    /// <summary>
    /// Gets the object providing application's configuration properties.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Dataedo.Api.Startup" /> class with configuration properties.
    /// </summary>
    /// <param name="configuration">The object providing application's configuration properties.</param>
    /// <param name="environment">The object providing information about hosting environment an application is running in.</param>
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        this.environment = environment;
    }

    /// <summary>
    /// Configures available services for application.
    /// </summary>
    /// <param name="services">The collection of service descriptors.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<ClassService>();
        services.AddTransient<TypeService>();
        services.AddTransient<DbmsService>();
        services.AddScoped<IUsersSessionsService, UsersSessionsService>();
        services.AddScoped<IRepositoryAccessManager, RepositoryAccessManager>();
        services.AddScoped<INotificationService, EmailSender>();
        services.AddScoped<StandardLoginMethodAttribute>();
        services.AddScoped<SSOLoginMethodAttribute>();
        services.AddScoped<DatabaseViewPermissionAttribute>();
        services.AddScoped<RepositoryDocumentationViewPermssionAttribute>();
        services.AddScoped<DatabaseLineageViewPermissionAttribute>();
        services.AddScoped<DatabaseCommunityViewPermissionAttribute>();
        services.AddScoped<DatabaseEditPermissionAttribute>();
        services.AddScoped<DatabaseUserViewPermissionAttribute>();
        services.AddScoped<DatabaseUserManagePermissionAttribute>();
        services.AddScoped<DatabaseSchemaChangesViewAttribute>();
        services.AddScoped<TableViewStructureSchemaChangesViewAttribute>();
        services.AddScoped<TriggerSchemaChangesViewAttribute>();
        services.AddScoped<ColumnSchemaChangesViewAttribute>();
        services.AddScoped<ProcedureFunctionSchemaChangesViewAttribute>();
        services.AddScoped<SchemaUpdatesViewAttribute>();
        services.AddScoped<TableChangesViewAttribute>();
        services.AddScoped<TriggerChangesViewAttribute>();
        services.AddScoped<ColumnChangesViewAttribute>();
        services.AddScoped<ProcedureChangesViewAttribute>();
        services.AddScoped<ParameterChangesViewAttribute>();
        services.AddScoped<SubjectAreaSchemaChangesViewAttribute>();
        services.AddScoped<SchemaUpdateDocumentationEditAttribute>();
        services.AddScoped<TableChangeDocumentationEditAttribute>();
        services.AddScoped<ColumnChangeDocumentationEditAttribute>();
        services.AddScoped<TriggerChangeDocumentationEditAttribute>();
        services.AddScoped<ProcedureChangeDocumentationEditAttribute>();
        services.AddScoped<ParameterChangeDocumentationEditAttribute>();
        services.AddScoped<GlossaryUserViewPermissionAttribute>();
        services.AddScoped<GlossaryUserManagePermissionAttribute>();
        services.AddScoped<GlossaryViewPermissionAttribute>();
        services.AddScoped<GlossaryEditPermissionAttribute>();
        services.AddScoped<TargetGlossaryEditPermissionAttribute>();
        services.AddScoped<GlossaryCommunityViewPermissionAttribute>();
        services.AddScoped<GlossaryEntriesRelationshipEditPermissionAttribute>();
        services.AddScoped<GlossaryEntryCreatePermissionAttribute>();
        services.AddScoped<GlossaryAnyObjectDocumentationViewPermissionAttribute>();
        services.AddScoped<EntryRelationshipEditPermission>();
        services.AddScoped<EntryMappingEditPermissionAttribute>();
        services.AddScoped<SubjectAreaDocumentationViewPermissionAttribute>();
        services.AddScoped<SubjectAreaCommunityViewPermissionAttribute>();
        services.AddScoped<ERDDocumentationViewPermissionAttribute>();
        services.AddScoped<TableViewStructurePermissionViewAttribute>();
        services.AddScoped<TableViewStructureEditPermissionAttribute>();
        services.AddScoped<TableViewStructureCommunityViewPermissionAttribute>();
        services.AddScoped<TableViewStructureDependencyViewAttribute>();
        services.AddScoped<TableViewStructureLineageViewPermissionAttribute>();
        services.AddScoped<ViewScriptsViewPermissionAttribute>();
        services.AddScoped<ColumnPermissionViewAttribute>();
        services.AddScoped<ColumnCommunityViewPermissionAttribute>();
        services.AddScoped<ColumnEditPermissionAttribute>();
        services.AddScoped<TriggerPermissionViewAttribute>();
        services.AddScoped<TriggerEditPermissionAttribute>();
        services.AddScoped<TriggerScriptViewPermissionAttribute>();
        services.AddScoped<TriggerCommunityViewPermissionAttribute>();
        services.AddScoped<TriggerLineageViewPermissionAttribute>();
        services.AddScoped<ProcedureFunctionViewPermissionAttribute>();
        services.AddScoped<ProcedureFunctionEditPermissionAttribute>();
        services.AddScoped<ProcedureFunctionCommunityViewPermissionAttribute>();
        services.AddScoped<ProcedureFunctionScriptsViewPermissionAttribute>();
        services.AddScoped<ProcedureFunctionDependencyViewAttribute>();
        services.AddScoped<ProcedureFunctionLineageViewPermissionAttribute>();
        services.AddScoped<ParametersPermissionEditAttribute>();
        services.AddScoped<EntryViewPermissionAttribute>();
        services.AddScoped<GlossaryEntryEditPermissionAttribute>();
        services.AddScoped<GlossaryEntryCommunityViewPermissionAttribute>();
        services.AddScoped<RepositoryCommunityViewPermissionAttribute>();
        services.AddScoped<CommunityEditPermissionAttribute>();
        services.AddScoped<CommunityAddWithLinkedObjectAttribute>();
        services.AddScoped<CommunityAddWithoutLinkedObjectAttribute>();
        services.AddScoped<CommunityEditCommentPermissionAttribute>();
        services.AddScoped<CommunityAddCommentPermissionAttribute>();
        services.AddScoped<CommunityManagePermissionAttribute>();
        services.AddScoped<ProfilingAccesibilityAttribute>();
        services.AddScoped<ProfilingPermissionViewAttribute>();
        services.AddScoped<ClassificationViewPermissionAttribute>();
        services.AddScoped<UserViewPermissionAttribute>();
        services.AddScoped<AnyUserViewPermissionAttribute>();
        services.AddScoped<UserManagePermissionAttribute>();
        services.AddScoped<SystemManagementSettingsAttribute>();
        services.AddScoped<BaseKeysManagePermissionAttribute>();
        services.AddScoped<KeyUpdatePermissionAttribute>();
        services.AddScoped<KeyCreatePermissionAttribute>();
        services.AddScoped<RelationshipBasePermissionAttribute>();
        services.AddScoped<RelationshipCreatePermissionAttribute>();
        services.AddScoped<RelationshipUpdatePermissionAttribute>();
        services.AddScoped<InsertSubjectAreaDocumentationEditPermissionAttribute>();
        services.AddScoped<ManageSubjectAreaDocumentationEditPermissionAttribute>();
        services.AddScoped<InsertTablesIntoSubjectAreaAttribute>();
        services.AddScoped<InsertProceduresIntoSubjectAreaAttribute>();
        services.AddScoped<ManageSubjectAreaOnInsertIntoDocumentationEditPermissionAttribute>();
        services.AddScoped<RemoveProcedureFromSubjectAreaPermissionAttribute>();
        services.AddScoped<RemoveTableFromSubjectAreaPermissionAttribute>();
        services.AddScoped<SubjectAreaLineageViewPermissionAttribute>();
        services.AddScoped<FollowingObjectDocumentationViewAttribute>();
        services.AddScoped<ConfigurationService>();
        IConfigurationSection connectionStringsSection = Configuration.GetSection("DefaultConnection");
        services.Configure<DefaultConnection>(connectionStringsSection);
        services.Configure<SMTPConfiguration>(Configuration.GetSection("SMTP"));
        services.AddCors(delegate (CorsOptions options)
        {
            options.AddDefaultPolicy(delegate (CorsPolicyBuilder builder)
            {
                builder.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                    .WithExposedHeaders("upgrade-status", "repository", "user-status", "validation-result");
            });
        });
        services.AddMvc(delegate (MvcOptions option)
        {
            option.EnableEndpointRouting = false;
        });
        services.AddRouting(delegate (RouteOptions options)
        {
            options.LowercaseUrls = true;
        });
        IConfigurationSection jwtAuthenticationSection = Configuration.GetSection("JwtAuthentication");
        services.Configure<JwtAuthentication>(jwtAuthenticationSection);
        services.AddAuthentication(delegate (AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(delegate (JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = Tokens.GetTokenValidationParameters(Configuration);
            options.IncludeErrorDetails = true;
        });
        services.AddSwaggerGen(delegate (SwaggerGenOptions c)
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Dataedo",
                Description = "Dataedo API",
                Contact = new OpenApiContact
                {
                    Name = "Support",
                    Email = "support@dataedo.com"
                },
                License = new OpenApiLicense
                {
                    Name = "Dataedo License",
                    Url = new Uri("https://dataedo.com/license")
                }
            });
            OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme.",
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", openApiSecurityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                openApiSecurityScheme,
                new string[1] { "Bearer" }
            } });
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Dataedo.Repository.Services.xml"));
            string path = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
            string filePath = Path.Combine(AppContext.BaseDirectory, path);
            c.IncludeXmlComments(filePath);
            c.DocumentFilter<FormatDocumentXmlComments>(Array.Empty<object>());
            c.DescribeAllParametersInCamelCase();
            c.OperationFilter<ParameterOperationFilter>(Array.Empty<object>());
        });
        services.AddControllers().AddJsonOptions(delegate (JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddLogging(delegate (ILoggingBuilder configure)
        {
            configure.AddConsole();
            configure.AddFilter((LogLevel level) => level == LogLevel.Information);
        });
        services.Configure<Saml2Client>(Configuration.GetSection("Saml2Client"));
        services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));
        services.Configure(delegate (Saml2Configuration saml2Configuration)
        {
            saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);
            EntityDescriptor entityDescriptor = new EntityDescriptor();
            entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(Configuration["Saml2:IdPMetadata"]));
            if (entityDescriptor.IdPSsoDescriptor != null)
            {
                saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                return;
            }
            throw new Exception("IdPSsoDescriptor not loaded from metadata.");
        });
        services.AddSaml2();
    }

    /// <summary>
    /// Configures application's request pipeline object and environment.
    /// </summary>
    /// <param name="app">The application's request pipeline object.</param>
    /// <param name="env">The object providing information about hosting environment an application is running in.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseStaticFiles();
        app.UseSwagger(delegate (SwaggerOptions c)
        {
            c.SerializeAsV2 = false;
        });
        app.UseSwaggerUI(delegate (SwaggerUIOptions c)
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dataedo API V1");
        });
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }
        app.UseCors();
        app.UseHttpsRedirection();
        app.UseCustomExceptionHandler();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = (ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto)
        });
        app.UseAuthentication();
        app.UseMiddleware<RepositoryCompatibilityMiddleware>(Array.Empty<object>());
        app.UseMiddleware<RepositoryInfoMiddleware>(Array.Empty<object>());
        app.UseMiddleware<UserLicenseKeyMiddleware>(Array.Empty<object>());
        app.UseMvc(delegate (IRouteBuilder routes)
        {
            routes.MapRoute("default", "{controller=Home}/{action=Index}");
        });
        app.UseSaml2();
    }
}
