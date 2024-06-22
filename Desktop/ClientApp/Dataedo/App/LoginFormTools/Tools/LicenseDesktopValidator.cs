using System.Collections.Generic;
using System.Linq;
using Dataedo.Shared.Licenses.Models;

namespace Dataedo.App.LoginFormTools.Tools;

public class LicenseDesktopValidator
{
    private const string requiredModule = "DESKTOP";

    public static bool Validate(IEnumerable<ModuleDataResult> modules)
    {
        if (modules != null && modules.Count() > 0)
        {
            return modules.Any((ModuleDataResult x) => x.Module.Equals(requiredModule));
        }
        return false;
    }
}
