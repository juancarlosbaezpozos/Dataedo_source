using System;

namespace Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data;

public class RepositoryVersion : IComparable, IComparable<RepositoryVersion>
{
	public int Version { get; set; }

	public int Update { get; set; }

	public int Release { get; set; }

	public bool IsStable { get; set; }

	public static int Compare(RepositoryVersion left, RepositoryVersion right)
	{
		if ((object)left != right)
		{
			return left?.CompareTo(right) ?? (-1);
		}
		return 0;
	}

	public static bool operator ==(RepositoryVersion left, RepositoryVersion right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(RepositoryVersion left, RepositoryVersion right)
	{
		return !(left == right);
	}

	public static bool operator <(RepositoryVersion left, RepositoryVersion right)
	{
		return Compare(left, right) < 0;
	}

	public static bool operator >(RepositoryVersion left, RepositoryVersion right)
	{
		return Compare(left, right) > 0;
	}

	public static bool operator <=(RepositoryVersion left, RepositoryVersion right)
	{
		return Compare(left, right) <= 0;
	}

	public static bool operator >=(RepositoryVersion left, RepositoryVersion right)
	{
		return Compare(left, right) >= 0;
	}

	public int CompareTo(RepositoryVersion other)
	{
		if ((object)other != null)
		{
			if (Version != other.Version)
			{
				return Version.CompareTo(other.Version);
			}
			if (Update != other.Update)
			{
				return Update.CompareTo(other.Update);
			}
			return Release.CompareTo(other.Release);
		}
		return 1;
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		RepositoryVersion other = obj as RepositoryVersion;
		return CompareTo(other);
	}

	public override bool Equals(object obj)
	{
		if (obj is RepositoryVersion other)
		{
			return CompareTo(other) == 0;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Version * 23) ^ (Update * 13);
	}
}
