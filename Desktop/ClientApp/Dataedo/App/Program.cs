using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Forms;
using Dataedo.App.Helpers.Licenses;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.GeneralHandling;
using Dataedo.App.Tools.Tracking;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.CustomMessageBox;
using Dataedo.Data.Base.Tools;
using Dataedo.Data.Tools;
using Dataedo.Log.Error;
using Dataedo.Log.Execution;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using Microsoft.Win32;

namespace Dataedo.App;

internal static class Program
{
    private static Dataedo.App.Tools.CommandLine.Tools.Log log;

    public static bool IncludeDependenciesOnInport { get; private set; }

    private static Dataedo.App.Tools.CommandLine.Tools.Log GetLog()
    {
        return log;
    }

    [STAThread]
    private static void Main()
    {
        var flag = true;
        DataProfilingStringFormatter.SetDataProfilingCulture();
        LoginStrategy.SetStrategy(new DataedoLoginStrategy());
        DataProfilingTestsThatShouldNotHaveBeenPushedToMaster();
        //CallRunUrl();
        log = new Dataedo.App.Tools.CommandLine.Tools.Log();
        Initialize();
        var list = Environment.GetCommandLineArgs().ToList();
        DebugOptionsThatShouldNotHaveBeenPushedToMaster(list);
        if (list.Any(x => x == "/?" || x == "/help"))
        {
            Console.WriteLine();
            Console.WriteLine("Dataedo Command Line");
            Console.WriteLine();
            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName + " [[[path[filename | [/dataedocmd [path[filename[] [/t timeout] [/l | /log] [/lp path[] | [/? | /help[");
            Console.WriteLine();
            Console.WriteLine("[path[filename                     Opens Dataedo repository file (*.dataedo).");
            Console.WriteLine("/dataedocmd [path[filename         Executes the Dataedo command line file (*.dataedocmd).");
            Console.WriteLine("/t timeout                         Sets the time in seconds to wait for queries to repository to execute.");
            Console.WriteLine("                                   A value of 0 indicates no limit.");
            Console.WriteLine("/dxoff                             Disables the DirectX hardware acceleration.");
            Console.WriteLine("/l, /log                           Runs Dataedo with logging to default path ");
            Console.WriteLine("                                   (%My Documents%/[yyyy-MM-dd HHmmss] DataedoExecution.log) ");
            Console.WriteLine("                                   if /lp parameter is not specified.");
            Console.WriteLine("/lp path                           Sets the path for logging (if /l or /log parameter is specified).");
            Console.WriteLine("/el, /errorlog                     Runs Dataedo with errors logging to default path ");
            Console.WriteLine("                                   (%My Documents%/[yyyy-MM-dd HHmmss] DataedoError.log) ");
            Console.WriteLine("                                   if /elp parameter is not specified.");
            Console.WriteLine("/elp path                          Sets the path for logging (if /el or /errorlog parameter is specified).");
            Console.WriteLine("/?, /help                          Prints Dataedo command line help.");
            Console.WriteLine("/excludeDependencies               Excludes import of dependencies.");
            Console.WriteLine();
            return;
        }
        if (list.Any(x => x == "/l" || x == "/log"))
        {
            ExecutionLog.HasLogArgument = true;
            if (list.Any(x => x == "/lp"))
            {
                var num = list.IndexOf("/lp");
                if (list.Count > num + 1)
                {
                    ExecutionLog.InitializeLog(list[num + 1]);
                    list.RemoveAt(num + 1);
                }
                else
                {
                    ExecutionLog.InitializeLog(null);
                }
                list.RemoveAt(num);
            }
            else
            {
                ExecutionLog.InitializeLog();
            }
            ExecutionLog.StoreLogExceptions = false;
            list.RemoveAll(x => x == "/l" || x == "/log");
            if (!ExecutionLog.IsLogEnabled)
            {
                Console.WriteLine("Unable to create application log. Details:");
                Console.WriteLine(ExecutionLog.LogExceptionsString);
            }
        }
        if (list.Any(x => x == "/el" || x == "/errorlog"))
        {
            ErrorLog.HasLogArgument = true;
            if (list.Any(x => x == "/elp"))
            {
                var num2 = list.IndexOf("/elp");
                if (list.Count > num2 + 1)
                {
                    ErrorLog.InitializeLog(list[num2 + 1]);
                    list.RemoveAt(num2 + 1);
                }
                else
                {
                    ErrorLog.InitializeLog(null);
                }
                list.RemoveAt(num2);
            }
            else
            {
                ErrorLog.InitializeLog();
            }
            ErrorLog.StoreLogExceptions = false;
            list.RemoveAll(x => x == "/el" || x == "/errorlog");
            if (!ErrorLog.IsLogEnabled)
            {
                Console.WriteLine("Unable to create application error log. Details:");
                Console.WriteLine(ErrorLog.LogExceptionsString);
            }
        }
        if (list.Any(x => x.Equals("/t")))
        {
            var num3 = list.IndexOf(list.FirstOrDefault(x => x.Equals("/t")));
            CommandsTimeout.Timeout = list.Count <= num3 + 1 || !int.TryParse(list[num3 + 1], out var result) ? CommandsTimeout.Timeout = 120 : CommandsTimeout.Timeout = result;
            if (list.Count > num3 + 1)
            {
                list.RemoveAt(num3 + 1);
            }
            list.RemoveAt(num3);
        }
        if (list.Any(x => x.Equals("/dxoff")))
        {
            var num4 = list.IndexOf(list.FirstOrDefault(x => x.Equals("/dxoff")));
            flag = false;
            if (list.Count > num4 + 1)
            {
                list.RemoveAt(num4 + 1);
            }
            list.RemoveAt(num4);
        }
        if (list.Any(x => x.Equals("/di")))
        {
            QueryViewer.ShowQueryWindow = true;
        }
        IncludeDependenciesOnInport = true;
        if (list.Any(x => x == "/excludeDependencies"))
        {
            IncludeDependenciesOnInport = false;
            var index = list.IndexOf("/excludeDependencies");
            list.RemoveAt(index);
        }
        string text = null;
        if (list.Count >= 3)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == "/dataedocmd" && i + 1 < list.Count)
                {
                    text = list[i + 1];
                    list.RemoveAt(i);
                    list.RemoveAt(i);
                }
            }
        }
        if (text != null)
        {
            StaticData.IsCmdImport = true;
            if (File.Exists(text))
            {
                Exception exception = null;
                var commands = DataedoCommandsBase.GetCommands(text, ref exception);
                if (commands != null)
                {
                    GeneralHandlingSupport.OverrideHandlingMethod = HandlingMethodEnumeration.HandlingMethod.NoActionStoreExceptions;
                    var commandsProcessor = new CommandsProcessor();
                    var licenseInfo = new LoginInfo(ConfigurationFileHelper.GetConfPath(ProgramVersion.Major), emptyValues: false);
                    LastConnectionInfo.LOGIN_INFO = new LoginInfo(emptyValues: false);
                    CommandsTimeout.Timeout = LastConnectionInfo.LOGIN_INFO.RepositoryTimeout;
                    ConnectorsVersion.LoadFromXML();
                    var fileData = LicenseHelper.CreateLicenseForCmdImport();
                    LicenseFileDataHelper.Save(fileData.LicenseFile, fileData.Licenses.FirstOrDefault()?.FileLicenseModel);
                    commandsProcessor.PrepareLog(commands);
                    if (commandsProcessor.log != null)
                    {
                        log = commandsProcessor.log;
                    }
                    var result2 = commandsProcessor.ProcessCommands(commands).Result;
                    ConfigurationFileHelper.SetLicenseInfo(licenseInfo);
                    log.EnvironmentExit(result2.FailedCommandsCount);
                }
                else
                {
                    log.Write("Dataedo command line scripts file: \"" + text + "\" is not valid.");
                    if (exception != null)
                    {
                        log.WriteSimple("Details:", exception.ToString());
                    }
                    log.EnvironmentExit(-3);
                }
            }
            else
            {
                log.Write("Dataedo command line scripts file: \"" + text + "\" does not exist.");
                log.EnvironmentExit(-2);
            }
            return;
        }
        log.FreeConsoleFromProcess();
        var flag2 = list != null && list.Count >= 0 && list.Count == 2 && list[1] != "/di";
        var assembly = typeof(Customized).Assembly;
        SkinManager.Default.RegisterAssembly(assembly);
        var assembly2 = typeof(DevExpress.UserSkins.ToggleSwitch).Assembly;
        SkinManager.Default.RegisterAssembly(assembly2);
        LastConnectionInfo.LOGIN_INFO = new LoginInfo(emptyValues: false);
        CommandsTimeout.Timeout = LastConnectionInfo.LOGIN_INFO.RepositoryTimeout;
        ConnectorsVersion.LoadFromXML();
        if (flag)
        {
            WindowsFormsSettings.ForceDirectXPaint();
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(defaultValue: false);
        if (string.IsNullOrEmpty(LastConnectionInfo.LOGIN_INFO.SkinPalette))
        {
            var theme = true;
            try
            {
                theme = (Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1) as int? ?? 1) == 1;
            }
            finally
            {
                StartupSettingsForm.SetTheme(theme);
            }
        }
        else
        {
            SkinsManager.SetSkin();
        }
        try
        {
            using var mainForm = new StartForm(!flag2 ? null : list?[1]);
            TrackingRunner.Track(delegate
            {
                TrackingService.MakeAsyncRequest(new ParametersWithOsDataedoBuilder(new TrackingOSParameters(), new TrackingDataedoParameters()), TrackingEventEnum.RunDesktop);
            });
            Application.Run(mainForm);
        }
        catch (Exception exception2)
        {
            GeneralExceptionHandling.Handle(exception2, "An unexpected error occurred.");
        }
        finally
        {
            ExecutionLog.FlushBuffer();
            ErrorLog.FlushBuffer();
        }
    }

    private static void DataProfilingTestsThatShouldNotHaveBeenPushedToMaster()
    {
    }

    private static void DebugOptionsThatShouldNotHaveBeenPushedToMaster(List<string> args)
    {
    }

    private static void Initialize()
    {
        CustomMessageBoxForm.InitializeErrorReporting(Links.CrashReports, "fecfba50-a116-4d68-afa3-29f941f3998f", "Dataedo", ProgramVersion.VersionWithBuild, "Dataedo application");
        Application.ThreadException += delegate (object sender, ThreadExceptionEventArgs e)
        {
            GeneralHandlingSupport.StoreResult(GeneralExceptionHandling.Handle(e.Exception));
            log.Write(GeneralHandlingSupport.StoredHandlingResults);
            GeneralHandlingSupport.ClearStoredHandlingResults();
            ExecutionLog.FlushBuffer();
            ErrorLog.FlushBuffer();
        };
        AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e)
        {
            GeneralHandlingSupport.StoreResult(GeneralExceptionHandling.Handle((Exception)e.ExceptionObject, null, null, Environment.NewLine + "The application will be closed."));
            log.Write(GeneralHandlingSupport.StoredHandlingResults);
            GeneralHandlingSupport.ClearStoredHandlingResults();
            log.EnvironmentExit(-1);
            ExecutionLog.FlushBuffer();
            ErrorLog.FlushBuffer();
        };
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
    }

    /*private static void CallRunUrl()
    {
        try
        {
            var thread = new Thread((ThreadStart)delegate
            {
                try
                {
                    new LoginInfo(emptyValues: false);
                    Links.MakeGetRequestWithoutResponse(Links.AfterRunUrl(ProgramVersion.VersionWithBuildForUrl));
                }
                catch (Exception)
                {
                    try
                    {
                        Links.MakeGetRequestWithoutResponse(Links.AfterRunUrl(ProgramVersion.VersionWithBuildForUrl));
                    }
                    catch (Exception)
                    {
                    }
                }
            });
            try
            {
                thread.IsBackground = true;
                thread.Start();
                thread.Join();
            }
            catch (Exception)
            {
            }
        }
        catch (Exception)
        {
        }
    }*/
}
