using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace Dataedo.Api;

/// <summary>
/// The class providing application entry point.
/// </summary>
public class Program
{
    /// <summary>
    /// The entry application method.
    /// </summary>
    /// <param name="args">The array of start parameters.</param>
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Creates <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> instance.
    /// </summary>
    /// <param name="args">The array of parameters.</param>
    /// <returns>A <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> instance.</returns>
    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddCommandLine(args).Build();
        string protocol = config.GetValue("protocol", "https");
        int port = config.GetValue("port", 44345);
        return WebHost.CreateDefaultBuilder(args).ConfigureLogging(delegate (ILoggingBuilder logging)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)).AddJsonFile("appsettings.json").Build();
            Logger logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            logging.AddSerilog(logger);
        }).ConfigureAppConfiguration(delegate (WebHostBuilderContext builderContext, IConfigurationBuilder webConfig)
        {
            webConfig.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
            .UseUrls($"{protocol}://*:{port}/")
            .UseStartup<Startup>();
    }
}
