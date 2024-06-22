using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Dataedo.App.API.Services;

internal class HomeService
{
    public async Task<string> GetHomeData(string email, int accountId, string type, string licenseId, string theme, string version)
    {
        HttpClient httpClient = new HttpClient();
        try
        {
            httpClient.ConfigureHomeClient();
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection["app_type"] = "DESKTOP";
            nameValueCollection["email"] = email;
            nameValueCollection["account_id"] = accountId.ToString();
            nameValueCollection["type"] = type;
            nameValueCollection["license_id"] = licenseId;
            nameValueCollection["theme"] = theme;
            nameValueCollection["version"] = version;
            HttpResponseMessage response = await httpClient.GetAsync(new Uri($"?{nameValueCollection}", UriKind.Relative));
            try
            {
                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                ((IDisposable)response)?.Dispose();
            }
        }
        finally
        {
            ((IDisposable)httpClient)?.Dispose();
        }
    }
}
