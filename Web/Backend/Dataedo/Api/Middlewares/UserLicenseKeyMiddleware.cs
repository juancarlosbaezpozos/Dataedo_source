using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Dataedo.Api.AppSettings;
using Dataedo.Api.Enums;
using Dataedo.Api.Services;
using Dataedo.Repository.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Dataedo.Api.Middlewares;

public class UserLicenseKeyMiddleware
{
    public const string UserStatusHeaderName = "user-status";

    private readonly RequestDelegate _next;

    private IList<string> IgnoredUrls = new List<string> { "/api/repository", "/api/repository/set", "/api/repository/upgrade", "/api/repository/first-account", "/api/repository/first-account/check-login-exists", "/api/repository/identity", "/api/licenses/user", "/api/users/login", "/api/users/logout", "/api/platform/info" };

    protected DefaultConnection Connection { get; private set; }

    public UserLicenseKeyMiddleware(RequestDelegate next, IOptionsMonitor<DefaultConnection> connection)
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
        ClaimsPrincipal user = context.User;
        if (!user.Identity.IsAuthenticated || context.Request.Path.Equals("/api/system-settings/security"))
        {
            await _next(context);
            return;
        }
        if (IgnoredUrls.Contains(context.Request.Path))
        {
            await _next(context);
            return;
        }
        int id = JWTService.GetId(context.Request);
        if (string.IsNullOrEmpty(await GetWebAccessRole(id)))
        {
            SetResponseStatus(context, UserStatusEnum.Status.NoAccess);
        }
        else
        {
            await _next(context);
        }
    }

    private async Task<string> GetWebAccessRole(int userId)
    {
        using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
        string sql = "SELECT [ra].action_code\r\n                        FROM [permissions] AS [x]\r\n                        INNER JOIN [roles] AS [x.Role] ON [x].[role_id] = [x.Role].[role_id]\r\n                        INNER JOIN [role_actions] [ra] ON [x].[role_id] = [ra].[role_id]\r\n                        LEFT JOIN [user_groups] AS [x.UserGroup] ON [x].[user_group_id] = [x.UserGroup].[user_group_id]\r\n                        WHERE ((([x].[user_id] = @id) OR @id IN (\r\n                            SELECT [x0].[user_id]\r\n                            FROM [users_user_groups] AS [x0]\r\n                            WHERE [x.UserGroup].[user_group_id] = [x0].[user_group_id]\r\n                        ))\r\n                        AND [ra].action_code = @web_access)";
        SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", userId);
        command.Parameters.AddWithValue("@web_access", "WEB_ACCESS");
        await command.Connection.OpenAsync();
        SqlDataReader data = await command.ExecuteReaderAsync();
        data.Read();
        if (data.HasRows)
        {
            return data["action_code"] as string;
        }
        return string.Empty;
    }

    private void SetResponseStatus(HttpContext context, UserStatusEnum.Status status)
    {
        context.Response.StatusCode = 401;
        context.Response.Headers.Add("user-status", BaseEnumConversions<UserStatusEnum.Status>.ToEnumString(status));
    }
}
