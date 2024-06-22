using System.Collections.Generic;
using System.Linq;

namespace Dataedo.LicenseHelperLibrary.Repository;

public class RepositoryVersion
{
    public class VersionDefinition
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int? Build { get; set; }

        public string ReleaseType { get; set; }

        public VersionDefinition(int major, int minor)
        {
            Major = major;
            Minor = minor;
            Build = null;
            ReleaseType = string.Empty;
        }

        public VersionDefinition(int major, int minor, int? build)
            : this(major, minor)
        {
            Build = build;
            ReleaseType = string.Empty;
        }

        public VersionDefinition(int major, int minor, int? build, string releaseType)
            : this(major, minor, build)
        {
            ReleaseType = releaseType;
        }

        public override bool Equals(object obj)
        {
            if (obj is VersionDefinition versionDefinition && Major == versionDefinition.Major && Minor == versionDefinition.Minor && EqualityComparer<int?>.Default.Equals(Build, versionDefinition.Build))
            {
                return ReleaseType == versionDefinition.ReleaseType;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (((-493837223 * -1521134295 + Major.GetHashCode()) * -1521134295 + Minor.GetHashCode()) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(Build)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ReleaseType);
            }
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}" + ((!Build.HasValue) ? null : $".{Build.Value}") + (string.IsNullOrEmpty(ReleaseType) ? null : (" " + ReleaseType));
        }
    }

    private static Dictionary<VersionDefinition, VersionDefinition> VersionMapping { get; set; } = new Dictionary<VersionDefinition, VersionDefinition>
    {
        {
            new VersionDefinition(10, 3, 2),
            new VersionDefinition(10, 3, 2)
        },
        {
            new VersionDefinition(10, 3, 1),
            new VersionDefinition(10, 3, 0)
        },
        {
            new VersionDefinition(10, 3, 0),
            new VersionDefinition(10, 3, 0)
        },
        {
            new VersionDefinition(10, 2, 2),
            new VersionDefinition(10, 2, 0)
        },
        {
            new VersionDefinition(10, 2, 1),
            new VersionDefinition(10, 2, 0)
        },
        {
            new VersionDefinition(10, 2, 0),
            new VersionDefinition(10, 2, 0)
        },
        {
            new VersionDefinition(10, 1, 3),
            new VersionDefinition(10, 1, 0)
        },
        {
            new VersionDefinition(10, 1, 2),
            new VersionDefinition(10, 1, 0)
        },
        {
            new VersionDefinition(10, 1, 1),
            new VersionDefinition(10, 1, 0)
        },
        {
            new VersionDefinition(10, 1, 0),
            new VersionDefinition(10, 1, 0)
        },
        {
            new VersionDefinition(10, 0, 2),
            new VersionDefinition(10, 0, 0)
        },
        {
            new VersionDefinition(10, 0, 1),
            new VersionDefinition(10, 0, 0)
        },
        {
            new VersionDefinition(10, 0, 0),
            new VersionDefinition(10, 0, 0)
        },
        {
            new VersionDefinition(9, 3, 0),
            new VersionDefinition(9, 1, 0)
        },
        {
            new VersionDefinition(9, 2, 0),
            new VersionDefinition(9, 1, 0)
        },
        {
            new VersionDefinition(9, 1, 2),
            new VersionDefinition(9, 1, 0)
        },
        {
            new VersionDefinition(9, 1, 1),
            new VersionDefinition(9, 1, 0)
        },
        {
            new VersionDefinition(9, 1, 0),
            new VersionDefinition(9, 1, 0)
        },
        {
            new VersionDefinition(9, 0, 1),
            new VersionDefinition(8, 2, 0)
        },
        {
            new VersionDefinition(9, 0, 0),
            new VersionDefinition(8, 2, 0)
        },
        {
            new VersionDefinition(8, 1, 4),
            new VersionDefinition(8, 1, 0)
        },
        {
            new VersionDefinition(8, 1, 3),
            new VersionDefinition(8, 1, 0)
        },
        {
            new VersionDefinition(8, 1, 2),
            new VersionDefinition(8, 1, 0)
        },
        {
            new VersionDefinition(8, 1, 1),
            new VersionDefinition(8, 1, 0)
        },
        {
            new VersionDefinition(8, 1, 0),
            new VersionDefinition(8, 1, 0)
        },
        {
            new VersionDefinition(8, 0, 2),
            new VersionDefinition(8, 0, 0)
        },
        {
            new VersionDefinition(8, 0, 1),
            new VersionDefinition(8, 0, 0)
        },
        {
            new VersionDefinition(8, 0, 0),
            new VersionDefinition(8, 0, 0)
        },
        {
            new VersionDefinition(7, 5, 4),
            new VersionDefinition(7, 5, 0)
        },
        {
            new VersionDefinition(7, 5, 3),
            new VersionDefinition(7, 5, 0)
        },
        {
            new VersionDefinition(7, 5, 2),
            new VersionDefinition(7, 5, 0)
        },
        {
            new VersionDefinition(7, 5, 1),
            new VersionDefinition(7, 5, 0)
        },
        {
            new VersionDefinition(7, 5, 0),
            new VersionDefinition(7, 5, 0)
        },
        {
            new VersionDefinition(7, 4, 2),
            new VersionDefinition(7, 4, 0)
        },
        {
            new VersionDefinition(7, 4, 1),
            new VersionDefinition(7, 4, 0)
        },
        {
            new VersionDefinition(7, 4, 0),
            new VersionDefinition(7, 4, 0)
        },
        {
            new VersionDefinition(7, 3, 1),
            new VersionDefinition(7, 3, 0)
        },
        {
            new VersionDefinition(7, 3, 0),
            new VersionDefinition(7, 3, 0)
        },
        {
            new VersionDefinition(7, 2, 2),
            new VersionDefinition(7, 2, 2)
        },
        {
            new VersionDefinition(7, 2, 1),
            new VersionDefinition(7, 2, 0)
        },
        {
            new VersionDefinition(7, 2, 0),
            new VersionDefinition(7, 2, 0)
        },
        {
            new VersionDefinition(7, 1, 1),
            new VersionDefinition(7, 1, 0)
        },
        {
            new VersionDefinition(7, 1, 0),
            new VersionDefinition(7, 1, 0)
        },
        {
            new VersionDefinition(7, 0, 0),
            new VersionDefinition(7, 0, 0)
        },
        {
            new VersionDefinition(6, 5, 0),
            new VersionDefinition(6, 1, 0)
        },
        {
            new VersionDefinition(6, 4, 0),
            new VersionDefinition(6, 1, 0)
        },
        {
            new VersionDefinition(6, 3, 0),
            new VersionDefinition(6, 1, 0)
        },
        {
            new VersionDefinition(6, 2, 0),
            new VersionDefinition(6, 1, 0)
        },
        {
            new VersionDefinition(6, 1, 0),
            new VersionDefinition(6, 1, 0)
        },
        {
            new VersionDefinition(6, 0, 4),
            new VersionDefinition(6, 0, 3)
        },
        {
            new VersionDefinition(6, 0, 3),
            new VersionDefinition(6, 0, 3)
        },
        {
            new VersionDefinition(6, 0, 2),
            new VersionDefinition(6, 0, 2)
        },
        {
            new VersionDefinition(6, 0, 1),
            new VersionDefinition(6, 0, 1)
        },
        {
            new VersionDefinition(6, 0, 0, "beta2"),
            new VersionDefinition(6, 0, 0)
        }
    };


    public int Version { get; set; }

    public int Update { get; set; }

    public int Build { get; set; }

    public bool Stable { get; set; }

    public VersionDefinition ApplicationVersion
    {
        get
        {
            VersionDefinition key = VersionMapping.FirstOrDefault((KeyValuePair<VersionDefinition, VersionDefinition> x) => x.Value.Equals(new VersionDefinition(Version, Update, Build))).Key;
            if (key != null)
            {
                return key;
            }
            return new VersionDefinition(Version, Update, Build);
        }
    }

    public string ApplicationVersionString => ApplicationVersion.ToString();

    public RepositoryVersion(int version, int update, int build, bool stable)
    {
        Version = version;
        Update = update;
        Build = build;
        Stable = stable;
    }

    public VersionDefinition GetMatchingRepositoryVersion(int applcationMajor, int applicationMinor, int applicationBuild, string applicationReleaseType = "")
    {
        if (VersionMapping.TryGetValue(new VersionDefinition(applcationMajor, applicationMinor, applicationBuild, applicationReleaseType), out var value))
        {
            return value;
        }
        return new VersionDefinition(applcationMajor, applicationMinor, null);
    }

    public int CompareTo(RepositoryVersion version)
    {
        return CompareTo(version.Version, version.Update, version.Build);
    }

    public int CompareTo(int version, int update, int? build)
    {
        int num = Version.CompareTo(version);
        if (num == 0)
        {
            num = Update.CompareTo(update);
            if (num == 0 && build.HasValue)
            {
                return Build.CompareTo(build);
            }
        }
        return num;
    }

    public static bool operator >(RepositoryVersion r1, RepositoryVersion r2)
    {
        return r1.CompareTo(r2) > 0;
    }

    public static bool operator <(RepositoryVersion r1, RepositoryVersion r2)
    {
        return r1.CompareTo(r2) < 0;
    }
}
