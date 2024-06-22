namespace Dataedo.LicenseHelperLibrary.Enums;

public static class LicenseTypeEnum
{
    public enum LicenseType
    {
        Lite = 1,
        Pro = 2,
        Trial = 3,
        Trial30 = 4,
        Trial90 = 5,
        Trial365 = 6,
        Demo = 7,
        Pro30 = 8,
        Pro90 = 9,
        Pro365 = 10,
        Education = 13,
        Education365 = 14,
        ProPlus = 15,
        ProPlus365 = 16,
        Enterprise = 17,
        Enterprise365 = 18,
        EnterpriseTrial = 19,
        EnterpriseTrial30 = 20,
        EnterpriseTrial90 = 21,
        EnterpriseTrial365 = 22,
        Viewer365 = 23,
        Viewer60 = 24,
        Viewer90 = 25,
        Viewer120 = 26,
        Pro60 = 27,
        Pro120 = 28,
        ProPlus60 = 29,
        ProPlus90 = 30,
        ProPlus120 = 31,
        Enterprise60 = 32,
        Enterprise90 = 33,
        Enterprise120 = 34,
        Trial7 = 35,
        EnterpriseTrial7 = 36,
        Pro180 = 37,
        ProPlus30 = 38,
        ProPlus180 = 39,
        Enterprise30 = 40,
        Enterprise180 = 41
    }

    public static string ToString(LicenseType type)
    {
        return type switch
        {
            LicenseType.Lite => "Lite Edition",
            LicenseType.Pro => "Pro Edition",
            LicenseType.Trial7 => "Pro Trial - 7 days",
            LicenseType.Trial => "Pro Trial - 14 days",
            LicenseType.Trial30 => "Pro Trial - 30 days",
            LicenseType.Trial90 => "Pro Trial - 90 days",
            LicenseType.Trial365 => "Pro Trial - 1 year",
            LicenseType.Demo => "Demo",
            LicenseType.Pro30 => "Pro Edition - 30 days",
            LicenseType.Pro60 => "Pro Edition - 60 days",
            LicenseType.Pro90 => "Pro Edition - 90 days",
            LicenseType.Pro120 => "Pro Edition - 120 days",
            LicenseType.Pro180 => "Pro Edition - 180 days",
            LicenseType.Pro365 => "Pro Edition - 1 year",
            LicenseType.Education => "Education Edition",
            LicenseType.Education365 => "Education Edition - 1 year",
            LicenseType.ProPlus => "Pro Plus Edition",
            LicenseType.ProPlus30 => "Pro Plus Edition - 30 days",
            LicenseType.ProPlus60 => "Pro Plus Edition - 60 days",
            LicenseType.ProPlus90 => "Pro Plus Edition - 90 days",
            LicenseType.ProPlus120 => "Pro Plus Edition - 120 days",
            LicenseType.ProPlus180 => "Pro Plus Edition - 180 days",
            LicenseType.ProPlus365 => "Pro Plus Edition - 1 year",
            LicenseType.Enterprise => "Enterprise Edition",
            LicenseType.Enterprise30 => "Enterprise Edition - 30 days",
            LicenseType.Enterprise60 => "Enterprise Edition - 60 days",
            LicenseType.Enterprise90 => "Enterprise Edition - 90 days",
            LicenseType.Enterprise120 => "Enterprise Edition - 120 days",
            LicenseType.Enterprise180 => "Enterprise Edition - 180 days",
            LicenseType.Enterprise365 => "Enterprise Edition - 1 year",
            LicenseType.EnterpriseTrial7 => "Enterprise Trial - 7 days",
            LicenseType.EnterpriseTrial => "Enterprise Trial - 14 days",
            LicenseType.EnterpriseTrial30 => "Enterprise Trial - 30 days",
            LicenseType.EnterpriseTrial90 => "Enterprise Trial - 90 days",
            LicenseType.EnterpriseTrial365 => "Enterprise Trial - 1 year",
            LicenseType.Viewer120 => "Viewer - 120 days",
            LicenseType.Viewer365 => "Viewer - 1 year",
            LicenseType.Viewer60 => "Viewer - 60 days",
            LicenseType.Viewer90 => "Viewer - 90 days",
            _ => type.ToString(),
        };
    }

    public static string ToStringWithoutTimeLimit(LicenseType type)
    {
        switch (type)
        {
            case LicenseType.Lite:
                return "Lite Edition";
            case LicenseType.Pro:
                return "Pro Edition";
            case LicenseType.Trial:
            case LicenseType.Trial30:
            case LicenseType.Trial90:
            case LicenseType.Trial365:
            case LicenseType.Trial7:
                return "Pro Trial";
            case LicenseType.Demo:
                return "Demo";
            case LicenseType.Pro30:
            case LicenseType.Pro90:
            case LicenseType.Pro365:
            case LicenseType.Pro60:
            case LicenseType.Pro120:
            case LicenseType.Pro180:
                return "Pro Edition";
            case LicenseType.Education:
            case LicenseType.Education365:
                return "Education Edition";
            case LicenseType.ProPlus:
            case LicenseType.ProPlus365:
            case LicenseType.ProPlus60:
            case LicenseType.ProPlus90:
            case LicenseType.ProPlus120:
            case LicenseType.ProPlus30:
            case LicenseType.ProPlus180:
                return "Pro Plus Edition";
            case LicenseType.Enterprise:
            case LicenseType.Enterprise365:
            case LicenseType.Enterprise60:
            case LicenseType.Enterprise90:
            case LicenseType.Enterprise120:
            case LicenseType.Enterprise30:
            case LicenseType.Enterprise180:
                return "Enterprise Edition";
            case LicenseType.EnterpriseTrial:
            case LicenseType.EnterpriseTrial30:
            case LicenseType.EnterpriseTrial90:
            case LicenseType.EnterpriseTrial365:
            case LicenseType.EnterpriseTrial7:
                return "Enterprise Trial";
            default:
                return type.ToString();
        }
    }

    public static string ToShortStringWithoutEdition(LicenseType type)
    {
        switch (type)
        {
            case LicenseType.Lite:
                return "Lite";
            case LicenseType.Education:
            case LicenseType.Education365:
                return "Education";
            case LicenseType.ProPlus:
            case LicenseType.ProPlus365:
            case LicenseType.ProPlus60:
            case LicenseType.ProPlus90:
            case LicenseType.ProPlus120:
            case LicenseType.ProPlus30:
            case LicenseType.ProPlus180:
                return "Pro Plus";
            case LicenseType.Enterprise:
            case LicenseType.Enterprise365:
            case LicenseType.EnterpriseTrial:
            case LicenseType.EnterpriseTrial30:
            case LicenseType.EnterpriseTrial90:
            case LicenseType.EnterpriseTrial365:
            case LicenseType.Enterprise60:
            case LicenseType.Enterprise90:
            case LicenseType.Enterprise120:
            case LicenseType.EnterpriseTrial7:
            case LicenseType.Enterprise30:
            case LicenseType.Enterprise180:
                return "Enterprise";
            case LicenseType.Viewer365:
            case LicenseType.Viewer60:
            case LicenseType.Viewer90:
            case LicenseType.Viewer120:
                return "Viewer";
            default:
                return "Pro";
        }
    }

    public static string ToShortString(LicenseType type)
    {
        return ToShortStringWithoutEdition(type) + " Edition";
    }

    public static bool IsPerpetual(LicenseType type)
    {
        if (type != LicenseType.Pro && type != LicenseType.Education && type != LicenseType.ProPlus)
        {
            return type == LicenseType.Enterprise;
        }
        return true;
    }

    public static bool IsTimeLimited(LicenseType type)
    {
        if (type != LicenseType.Trial && type != LicenseType.Trial7 && type != LicenseType.Trial30 && type != LicenseType.Trial90 && type != LicenseType.Trial365 && type != LicenseType.Pro30 && type != LicenseType.Pro60 && type != LicenseType.Pro90 && type != LicenseType.Pro120 && type != LicenseType.Pro180 && type != LicenseType.Pro365 && type != LicenseType.Education365 && type != LicenseType.ProPlus30 && type != LicenseType.ProPlus60 && type != LicenseType.ProPlus90 && type != LicenseType.ProPlus120 && type != LicenseType.ProPlus180 && type != LicenseType.ProPlus365 && type != LicenseType.Enterprise30 && type != LicenseType.Enterprise60 && type != LicenseType.Enterprise90 && type != LicenseType.Enterprise120 && type != LicenseType.Enterprise180 && type != LicenseType.Enterprise365 && type != LicenseType.EnterpriseTrial && type != LicenseType.EnterpriseTrial7 && type != LicenseType.EnterpriseTrial30 && type != LicenseType.EnterpriseTrial90 && type != LicenseType.EnterpriseTrial365 && type != LicenseType.Viewer120 && type != LicenseType.Viewer365 && type != LicenseType.Viewer60)
        {
            return type == LicenseType.Viewer90;
        }
        return true;
    }

    public static bool IsTrial(LicenseType type)
    {
        if (!IsProTrial(type))
        {
            return IsEnterpriseTrial(type);
        }
        return true;
    }

    public static bool IsProTrial(LicenseType type)
    {
        if (type != LicenseType.Trial && type != LicenseType.Trial7 && type != LicenseType.Trial30 && type != LicenseType.Trial90)
        {
            return type == LicenseType.Trial365;
        }
        return true;
    }

    public static bool IsEnterpriseTrial(LicenseType type)
    {
        if (type != LicenseType.EnterpriseTrial && type != LicenseType.EnterpriseTrial7 && type != LicenseType.EnterpriseTrial30 && type != LicenseType.EnterpriseTrial90)
        {
            return type == LicenseType.EnterpriseTrial365;
        }
        return true;
    }

    public static bool IsPro(LicenseType type)
    {
        if (type != LicenseType.Pro && type != LicenseType.Pro30 && type != LicenseType.Pro60 && type != LicenseType.Pro90 && type != LicenseType.Pro120 && type != LicenseType.Pro180)
        {
            return type == LicenseType.Pro365;
        }
        return true;
    }

    public static bool IsProPlus(LicenseType type)
    {
        if (type != LicenseType.ProPlus && type != LicenseType.ProPlus30 && type != LicenseType.ProPlus60 && type != LicenseType.ProPlus90 && type != LicenseType.ProPlus120 && type != LicenseType.ProPlus180)
        {
            return type == LicenseType.ProPlus365;
        }
        return true;
    }

    public static bool IsEnterprise(LicenseType type)
    {
        if (type != LicenseType.Enterprise && type != LicenseType.Enterprise30 && type != LicenseType.Enterprise60 && type != LicenseType.Enterprise90 && type != LicenseType.Enterprise120 && type != LicenseType.Enterprise180)
        {
            return type == LicenseType.Enterprise365;
        }
        return true;
    }

    public static bool IsEducation(LicenseType type)
    {
        if (type != LicenseType.Education)
        {
            return type == LicenseType.Education365;
        }
        return true;
    }

    public static bool IsNotSupportedViewer(LicenseType type)
    {
        if (type != LicenseType.Viewer60 && type != LicenseType.Viewer90 && type != LicenseType.Viewer120)
        {
            return type == LicenseType.Viewer365;
        }
        return true;
    }

    public static bool HasWatermark(LicenseType type)
    {
        if (type != LicenseType.Demo)
        {
            return IsTrial(type);
        }
        return true;
    }

    public static int GetTimeDuration(LicenseType type)
    {
        switch (type)
        {
            case LicenseType.Trial7:
            case LicenseType.EnterpriseTrial7:
                return 7;
            case LicenseType.Trial:
            case LicenseType.EnterpriseTrial:
                return 14;
            case LicenseType.Trial30:
            case LicenseType.Pro30:
            case LicenseType.EnterpriseTrial30:
            case LicenseType.ProPlus30:
            case LicenseType.Enterprise30:
                return 30;
            case LicenseType.Viewer60:
            case LicenseType.Pro60:
            case LicenseType.ProPlus60:
            case LicenseType.Enterprise60:
                return 60;
            case LicenseType.Trial90:
            case LicenseType.Pro90:
            case LicenseType.EnterpriseTrial90:
            case LicenseType.Viewer90:
            case LicenseType.ProPlus90:
            case LicenseType.Enterprise90:
                return 90;
            case LicenseType.Viewer120:
            case LicenseType.Pro120:
            case LicenseType.ProPlus120:
            case LicenseType.Enterprise120:
                return 120;
            case LicenseType.Pro180:
            case LicenseType.ProPlus180:
            case LicenseType.Enterprise180:
                return 180;
            case LicenseType.Trial365:
            case LicenseType.Pro365:
            case LicenseType.Education365:
            case LicenseType.ProPlus365:
            case LicenseType.Enterprise365:
            case LicenseType.EnterpriseTrial365:
            case LicenseType.Viewer365:
                return 365;
            default:
                return 0;
        }
    }

    public static LicenseType GetEnterpriseTrialByProTrial(LicenseType type)
    {
        return type switch
        {
            LicenseType.Trial => LicenseType.EnterpriseTrial,
            LicenseType.Trial7 => LicenseType.EnterpriseTrial7,
            LicenseType.Trial30 => LicenseType.EnterpriseTrial30,
            LicenseType.Trial90 => LicenseType.EnterpriseTrial90,
            LicenseType.Trial365 => LicenseType.EnterpriseTrial365,
            _ => type,
        };
    }

    public static string GetTrialVersionText(LicenseType type)
    {
        if (IsEnterpriseTrial(type) || IsPro(type) || IsProPlus(type))
        {
            return "Enterprise";
        }
        return "Pro";
    }

    public static int GetImportance(LicenseType type)
    {
        if (IsEnterprise(type))
        {
            return 7;
        }
        if (IsEnterpriseTrial(type))
        {
            return 6;
        }
        if (IsProPlus(type))
        {
            return 5;
        }
        if (IsPro(type))
        {
            return 4;
        }
        if (IsProTrial(type))
        {
            return 3;
        }
        if (IsEducation(type))
        {
            return 2;
        }
        if (IsNotSupportedViewer(type))
        {
            return -1;
        }
        return 1;
    }
}
