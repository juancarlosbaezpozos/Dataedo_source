using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Dataedo.App.Tools;
using Microsoft.Win32;

namespace Dataedo.App.API;

internal static class ApiConfiguration
{
    private static string AccountApiAddress = "https://api.account.dataedo.com/api/v1/";

    private static string PublishApiAddress = "https://api.publish.dataedo.com/api/v1/";

    private static string StagingAccountApiAddress = "https://staging.api.account.dataedo.com/api/v1/";

    private static string StagingPublishApiAddress = "https://staging.api.publish.dataedo.com/api/v1/";

    public static string HomeApiAddress = "https://dataedo.com/api/desktop/homepage";

    public static string Service = "DESKTOP_APP";

    public const string UseStagingAPI = "UseStagingAPI";

    public static void ConfigureAccountClient(this HttpClient httpClient)
    {
        httpClient.ConfigureClient(GetAccountApiAddress());
    }

    public static void ConfigureAccountClient(this HttpClient httpClient, string token)
    {
        httpClient.ConfigureClient(GetAccountApiAddress(), token);
    }

    public static void ConfigurePublishClient(this HttpClient httpClient)
    {
        httpClient.ConfigureClient(GetPublishApiAddress());
    }

    public static void ConfigurePublishClient(this HttpClient httpClient, string token)
    {
        httpClient.ConfigureClient(GetPublishApiAddress(), token);
    }

    public static void ConfigureHomeClient(this HttpClient httpClient)
    {
        httpClient.ConfigureClient(HomeApiAddress);
    }

    public static void ConfigureHomeClient(this HttpClient httpClient, string token)
    {
        httpClient.ConfigureClient(HomeApiAddress);
    }

    private static void ConfigureClient(this HttpClient httpClient, string address)
    {
        //IL_001c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0026: Expected O, but got Unknown
        httpClient.BaseAddress = (new Uri(address));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        string text = "Unknown";
        try
        {
            text = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", "").ToString();
        }
        catch (Exception)
        {
        }
        text += (Environment.Is64BitOperatingSystem ? " (64-bit)" : " (32-bit)");
        ((HttpHeaders)httpClient.DefaultRequestHeaders).TryAddWithoutValidation("User-Agent", "Dataedo Desktop " + ProgramVersion.VersionWithBitVersion + " @" + text);
    }

    private static void ConfigureClient(this HttpClient httpClient, string address, string token)
    {
        httpClient.ConfigureClient(address);
        ((HttpHeaders)httpClient.DefaultRequestHeaders).Add("Authorization", token);
    }

    public static string GetAccountApiAddress()
    {
        if (GetUseStagingApiConfigValue())
        {
            return StagingAccountApiAddress;
        }
        return AccountApiAddress;
    }

    public static string GetPublishApiAddress()
    {
        if (GetUseStagingApiConfigValue())
        {
            return StagingPublishApiAddress;
        }
        return PublishApiAddress;
    }

    private static bool GetUseStagingApiConfigValue()
    {
        bool result = false;
        if (ConfigurationManager.AppSettings["UseStagingAPI"] != null && bool.TryParse(ConfigurationManager.AppSettings["UseStagingAPI"], out var result2) && result2)
        {
            result = true;
        }
        return result;
    }
}
