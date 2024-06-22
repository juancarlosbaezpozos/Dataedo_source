using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dataedo.App.API.Models;
using Dataedo.App.LoginFormTools.Tools.Common;
using Newtonsoft.Json;

namespace Dataedo.App.API.Services;

internal class LoginService
{
    public async Task<ResultWithData<MessageResult>> SignInAsync(string email)
    {
        HttpClient httpClient = new HttpClient();
        try
        {
            httpClient.ConfigureAccountClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("sessions", new
            {
                email = email,
                service = ApiConfiguration.Service
            });
            try
            {
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<MessageResult>(statusCode, await response.Content.ReadAsAsync<MessageResult>());
                }
                if (response.StatusCode == (HttpStatusCode)422)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<MessageResult>(statusCode, await response.Content.ReadAsAsync<UnprocessableEntityResult>());
                }
                return new ResultWithData<MessageResult>(response.StatusCode, isValid: false);
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

    public async Task<ResultWithData<SessionResult>> EnterAccessCodeAsync(string email, string accessCode, bool keepSignedIn)
    {
        HttpClient httpClient = new HttpClient();
        try
        {
            httpClient.ConfigureAccountClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("sessions/confirm", new
            {
                email = email,
                code = accessCode,
                remember = (keepSignedIn ? 1 : 0)
            });
            try
            {
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<SessionResult>(statusCode, await response.Content.ReadAsAsync<SessionResult>());
                }
                if (response.StatusCode == (HttpStatusCode)422)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<SessionResult>(statusCode, await response.Content.ReadAsAsync<UnprocessableEntityResult>());
                }
                return new ResultWithData<SessionResult>(response.StatusCode, isValid: false);
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

    public async Task<ResultWithData<LicensesResult>> GetLicenses(string token)
    {
        /*HttpClient httpClient = new HttpClient();
        try
        {
            httpClient.ConfigureAccountClient(token);
            HttpResponseMessage response = httpClient.GetAsync(new Uri("licenses/available", UriKind.Relative)).Result;
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<LicensesResult>(statusCode, await response.Content.ReadAsAsync<LicensesResult>());
                }
                if (response.StatusCode == (HttpStatusCode)422)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<LicensesResult>(statusCode, await response.Content.ReadAsAsync<UnprocessableEntityResult>());
                }
                return new ResultWithData<LicensesResult>(response.StatusCode, isValid: false);
            }
            finally
            {
                ((IDisposable)response)?.Dispose();
            }
        }
        finally
        {
            ((IDisposable)httpClient)?.Dispose();
        }*/

        var fullModules = new System.Collections.Generic.List<Shared.Licenses.Models.ModuleDataResult>();
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "BUSINESS_GLOSSARY",
            Name = "Business Glossary",
            Sort = 550,
            Group = "MODULE",
            GroupName = "Modules",
            GroupSort = 20,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "COMMAND_LINE",
            Name = "Command Line",
            Sort = 250,
            Group = "FUNCTIONALITIES",
            GroupName = "Additional functionalities",
            GroupSort = 40,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "COMMUNITY",
            Name = "Data Community",
            Sort = 6,
            Group = "ACCESS",
            GroupName = "Access levels",
            GroupSort = 5,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "CONNECTOR_ALL",
            Name = "All metadata connectors",
            Sort = 5,
            Group = "CONNECTOR",
            GroupName = "Metadata Scanners",
            GroupSort = 50,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "CUSTOM_FIELDS",
            Name = "Custom Fields",
            Sort = 200,
            Group = "FUNCTIONALITIES",
            GroupName = "Additional functionalities",
            GroupSort = 40,
            Count = 100
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "DATA_CLASSIFICATION",
            Name = "Sensitive Data Discovery and Classification",
            Sort = 520,
            Group = "MODULE",
            GroupName = "Modules",
            GroupSort = 20,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "DATA_LINEAGE",
            Name = "Data Lineage",
            Sort = 590,
            Group = "MODULE",
            GroupName = "Modules",
            GroupSort = 20,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "DATA_PROFILING",
            Name = "Data Profiling",
            Sort = 570,
            Group = "MODULE",
            GroupName = "Modules",
            GroupSort = 20,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "DESIGNER",
            Name = "Data Designer",
            Sort = 210,
            Group = "FUNCTIONALITIES",
            GroupName = "Additional functionalities",
            GroupSort = 40,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "DESKTOP",
            Name = "Dataedo Desktop",
            Sort = 1,
            Group = "UI",
            GroupName = "Products",
            GroupSort = 1,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "DOC_COPY",
            Name = "Copy Documentation",
            Sort = 240,
            Group = "FUNCTIONALITIES",
            GroupName = "Additional functionalities",
            GroupSort = 40,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "EDITOR",
            Name = "Creator/Editor",
            Sort = 5,
            Group = "ACCESS",
            GroupName = "Access levels",
            GroupSort = 5,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "EXPORT_DB",
            Name = "Export comments to database",
            Sort = 850,
            Group = "EXPORT",
            GroupName = "Export options",
            GroupSort = 60,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "EXPORT_DDL",
            Name = "Export DDL",
            Sort = 860,
            Group = "EXPORT",
            GroupName = "Export options",
            GroupSort = 60,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "EXPORT_EXCEL",
            Name = "Excel Export",
            Sort = 820,
            Group = "EXPORT",
            GroupName = "Export options",
            GroupSort = 60,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "EXPORT_HTML",
            Name = "HTML Docs Export",
            Sort = 810,
            Group = "EXPORT",
            GroupName = "Export options",
            GroupSort = 60,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "EXPORT_PDF",
            Name = "PDF Export",
            Sort = 800,
            Group = "EXPORT",
            GroupName = "Export options",
            GroupSort = 60,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "REFERENCE_DATA",
            Name = "Reference Data",
            Sort = 580,
            Group = "MODULE",
            GroupName = "Modules",
            GroupSort = 20,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "REPORT_CATALOG",
            Name = "Report Catalog",
            Sort = 585,
            Group = "MODULE",
            GroupName = "Modules",
            GroupSort = 20,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "REPO_ALL",
            Name = "Repository - File and Server",
            Sort = 100,
            Group = "REPO",
            GroupName = "Repository options",
            GroupSort = 10,
            Count = null
        });
        fullModules.Add(new Shared.Licenses.Models.ModuleDataResult()
        {
            Module = "SCHEMA_CHANGE_TRACKING",
            Name = "Schema Change Tracking",
            Sort = 510,
            Group = "MODULE",
            GroupName = "Modules",
            GroupSort = 20,
            Count = null
        });

        DateTimeOffset dtToday = DateTimeOffset.Now;
        DateTimeOffset dtFinish = dtToday.AddDays(90);

        LicenseDataResult fullLicense = new()
        {
            AccountId = 0,
            AccountName = null,
            AccountRoles = new System.Collections.Generic.List<AccountRoleResult>(),
            End = /*16_68_55_68_00*/ dtFinish.ToUnixTimeSeconds(),
            Id = "SUB_54396", //TRIAL_54396
            Package = "SUB_DATAEDO",  //TRIAL_DATAEDO
            PackageName = "Subscription",
            Start = /*16_67_34_72_00*/ dtToday.ToUnixTimeSeconds(),
            Token = "5BS0P6hB",
            Type = "SUB",   //TRIAL
            University = null,
            ValidTill =/* 16_68_55_68_00*/dtFinish.ToUnixTimeSeconds(),
            Modules = fullModules
        };

        System.Collections.Generic.List<LicenseDataResult> licenses = new();
        licenses.Add(fullLicense);

        LicensesResult licResult = new()
        {
            Licenses = licenses
        };

        ResultWithData<LicensesResult> result = new ResultWithData<LicensesResult>(HttpStatusCode.OK, true)
        {
            Data = licResult
        };

        return result;
    }

    public async Task<Result> UseLicense(string token, LicenseDataResult license)
    {
        /*HttpClient httpClient = new HttpClient();
        try
        {
            httpClient.ConfigureAccountClient(token);
            HttpResponseMessage response = httpClient.PutAsync(new Uri("licenses/use/" + Uri.EscapeUriString(license.Id), UriKind.Relative), (HttpContent)null).Result;
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UsedLicenseResponse usedLicenseResponse = JsonConvert.DeserializeObject<UsedLicenseResponse>(await response.Content.ReadAsStringAsync());
                    license.Id = usedLicenseResponse.Data.Id;
                    return new Result(response.StatusCode, isValid: true);
                }
                if (response.StatusCode == (HttpStatusCode)422)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new Result(statusCode, await response.Content.ReadAsAsync<UnprocessableEntityResult>());
                }
                return new Result(response.StatusCode, isValid: false);
            }
            finally
            {
                ((IDisposable)response)?.Dispose();
            }
        }
        finally
        {
            ((IDisposable)httpClient)?.Dispose();
        }*/

        return new Result(HttpStatusCode.OK, isValid: true);
    }

    public async Task<Result> CheckIfAuthorized(string token)
    {
        /*HttpClient val = new HttpClient();
        try
        {
            val.ConfigureAccountClient(token);
            HttpResponseMessage result = val.GetAsync(new Uri("profile", UriKind.Relative)).Result;
            try
            {
                return new Result(result.StatusCode, isValid: true);
            }
            finally
            {
                ((IDisposable)result)?.Dispose();
            }
        }
        finally
        {
            ((IDisposable)val)?.Dispose();
        }*/

        return new Result(HttpStatusCode.OK, isValid: true);
    }

    public async Task<ResultWithData<ProfileResult>> GetProfile(string token)
    {
        /*HttpClient httpClient = new HttpClient();
        try
        {
            httpClient.ConfigureAccountClient(token);
            HttpResponseMessage response = httpClient.GetAsync(new Uri("profile", UriKind.Relative)).Result;
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<ProfileResult>(statusCode, await response.Content.ReadAsAsync<ProfileResult>());
                }
                if (response.StatusCode == (HttpStatusCode)422)
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    return new ResultWithData<ProfileResult>(statusCode, await response.Content.ReadAsAsync<UnprocessableEntityResult>());
                }
                return new ResultWithData<ProfileResult>(response.StatusCode, isValid: false);
            }
            finally
            {
                ((IDisposable)response)?.Dispose();
            }
        }
        finally
        {
            ((IDisposable)httpClient)?.Dispose();
        }*/

        ResultWithData<ProfileResult> resultWithData = new ResultWithData<ProfileResult>(System.Net.HttpStatusCode.OK, true);
        resultWithData.Errors = null;
        resultWithData.Data = new ProfileResult()
        {
            Email = "admin@domain.com",
            FirstName = "Admin",
            LastName = "Guy"
        };

        return resultWithData;
    }
}
