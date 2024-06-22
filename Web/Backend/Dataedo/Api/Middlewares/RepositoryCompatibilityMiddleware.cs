using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using Dataedo.Api.AppSettings;
using Dataedo.Api.Configuration;
using Dataedo.Api.Enums;
using Dataedo.Repository.Services.RepositoryAccess;
using Dataedo.Repository.Services.Services;
using Dataedo.Repository.Services.SqlServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Dataedo.Api.Middlewares;

public class RepositoryCompatibilityMiddleware
{
    public const string RepositoryStatusHeaderName = "upgrade-status";

    private readonly RequestDelegate _next;

    private IList<string> IgnoredUrls = new List<string> { "/api/repository", "/api/repository/set", "/api/repository/upgrade", "/api/repository/first-account", "/api/repository/first-account/check-login-exists", "/api/repository/identity", "/api/platform/info" };

    protected DefaultConnection Connection { get; private set; }

    public RepositoryCompatibilityMiddleware(RequestDelegate next, IOptionsMonitor<DefaultConnection> connection)
    {
        _next = next;
        Connection = connection.CurrentValue;
        connection.OnChange(Listener);
    }

    private void Listener(DefaultConnection connection)
    {
        Connection = connection;
    }

    public async Task Invoke(HttpContext context)
    {
        if (BackgroundProcessing.Instance.IsProcessing())
        {
            SetResponseStatus(context, RepositoryStatusEnum.Status.BackgroundProcessing);
            return;
        }
        if (IgnoredUrls.Contains(context.Request.Path))
        {
            await _next(context);
            return;
        }
        SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(Connection.ConnectionString);
        string repositoryName = connectionBuilder.InitialCatalog;
        if (string.IsNullOrEmpty(repositoryName))
        {
            SetResponseStatus(context, RepositoryStatusEnum.Status.RepositoryNotConfigured);
            return;
        }
        RepositoryVersion version;
        try
        {
            version = await GetRepositoryVersion(connectionBuilder.ConnectionString);
        }
        catch
        {
            SetResponseStatus(context, RepositoryStatusEnum.Status.LostConnection);
            return;
        }
        if (!IsRepositoryVersionSupported(version))
        {
            SetResponseStatus(context, RepositoryStatusEnum.Status.VersionNotSupported);
            return;
        }
        if (IsRepositoryUpgradeRequired(version))
        {
            SetResponseStatus(context, RepositoryStatusEnum.Status.UpgradeRequired);
            return;
        }
        if (!IsRepositoryValid())
        {
            SetResponseStatus(context, RepositoryStatusEnum.Status.Invalid);
            return;
        }
        if (await RepositoryService.CountRepositoryUsers(connectionBuilder.ConnectionString) == 0)
        {
            SetResponseStatus(context, RepositoryStatusEnum.Status.AccountNotConfigured);
            return;
        }
        await SetFlagPortalConnected(connectionBuilder.ConnectionString);
        await _next(context);
    }

    private async Task SetFlagPortalConnected(string connectionString)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        string sql = "UPDATE guid SET is_web_portal_connected = 1";
        SqlCommand command = new SqlCommand(sql, connection);
        await command.Connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private bool IsRepositoryVersionSupported(RepositoryVersion repoVersion)
    {
        if (repoVersion.Major < 7)
        {
            return false;
        }
        Version appVersion = Assembly.GetEntryAssembly()!.GetName().Version;
        if (repoVersion.IsGreaterThan(appVersion))
        {
            return false;
        }
        return true;
    }

    private bool IsRepositoryValid()
    {
        return true;
    }

    private void SetResponseStatus(HttpContext context, RepositoryStatusEnum.Status status)
    {
        context.Response.StatusCode = 426;
        context.Response.Headers.Add("upgrade-status", BaseEnumConversions<RepositoryStatusEnum.Status>.ToEnumString(status));
    }

    private bool IsRepositoryUpgradeRequired(RepositoryVersion currentRepoVersion)
    {
        Version appVersion = Assembly.GetEntryAssembly()!.GetName().Version;
        RepositoryVersion requiredRepoVersion = RepositoryService.FindCompatibleRepositoryVersionFor(appVersion);
        return currentRepoVersion.IsLowerThan(requiredRepoVersion);
    }

    private async Task<RepositoryVersion> GetRepositoryVersion(string connectionString)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        string sql = "SELECT TOP(1) [version], [update], [release] FROM [version] ORDER BY [version] desc, [update] desc, [release] desc";
        SqlCommand command = new SqlCommand(sql, connection);
        await command.Connection.OpenAsync();
        SqlDataReader data = await command.ExecuteReaderAsync();
        data.Read();
        return new RepositoryVersion((int)data["version"], (int)data["update"], (int)data["release"]);
    }
}
