using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Dataedo.App.Data.General;

[Serializable]
public class DatabaseVersionUpdate : IComparable<DatabaseVersionUpdate>
{
	public static readonly Regex VersionRegex = new Regex("^(\\d+)\\.(\\d+)(\\.(\\d+))?$", RegexOptions.IgnoreCase);

	public int Version { get; set; }

	public int Update { get; set; }

	public int Build { get; set; }

	public DatabaseVersionUpdate()
	{
	}

	public DatabaseVersionUpdate(int version, int update, int build)
	{
		Version = version;
		Update = update;
		Build = build;
	}

	public DatabaseVersionUpdate(string version)
	{
		if (string.IsNullOrWhiteSpace(version))
		{
			throw new ArgumentException("Empty version");
		}
		Match match = VersionRegex.Match(version);
		if (!match.Success)
		{
			throw new ArgumentException("Wrong version format '" + version + "'");
		}
		Version = Convert.ToInt32(match.Groups[1].Value, NumberFormatInfo.InvariantInfo);
		Update = Convert.ToInt32(match.Groups[2].Value, NumberFormatInfo.InvariantInfo);
		string value = match.Groups[4].Value;
		if (!string.IsNullOrWhiteSpace(value))
		{
			Build = Convert.ToInt32(value, NumberFormatInfo.InvariantInfo);
		}
	}

	public static int Compare(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		if ((object)left == null && (object)right == null)
		{
			return 0;
		}
		return left?.CompareTo(right) ?? (-1);
	}

	public int CompareTo(DatabaseVersionUpdate version)
	{
		if ((object)version == null)
		{
			return -1;
		}
		if (Version == version.Version)
		{
			if (Update == version.Update)
			{
				return Build.CompareTo(version.Build);
			}
			return Update.CompareTo(version.Update);
		}
		return Version.CompareTo(version.Version);
	}

	public static bool operator >(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		return Compare(left, right) > 0;
	}

	public static bool operator <(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		return Compare(left, right) < 0;
	}

	public static bool operator >=(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		return Compare(left, right) >= 0;
	}

	public static bool operator <=(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		return Compare(left, right) <= 0;
	}

	public static bool operator ==(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		return Compare(left, right) == 0;
	}

	public static bool operator !=(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		return Compare(left, right) != 0;
	}

	public override bool Equals(object obj)
	{
		if (obj is DatabaseVersionUpdate version)
		{
			return CompareTo(version) == 0;
		}
		return false;
	}

	public static bool Equals(DatabaseVersionUpdate left, DatabaseVersionUpdate right)
	{
		return Compare(left, right) == 0;
	}

	public override int GetHashCode()
	{
		return ((17 * 29 + Version) * 29 + Update) * 29 + Build;
	}

	public override string ToString()
	{
		return $"{Version}.{Update}.{Build}";
	}
}
