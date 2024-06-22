using System.Data.SqlClient;
using System.Threading.Tasks;
using Dataedo.Api.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Dataedo.Api.Middlewares;

public class RepositoryInfoMiddleware
{
    public const string RepositoryInfoHeaderName = "repository";

    private readonly RequestDelegate _next;

    protected DefaultConnection Connection { get; private set; }

    public RepositoryInfoMiddleware(RequestDelegate next, IOptionsMonitor<DefaultConnection> connection)
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
        SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder(Connection.ConnectionString);
        string.IsNullOrEmpty(conn.InitialCatalog);
        await _next(context);
    }
}
