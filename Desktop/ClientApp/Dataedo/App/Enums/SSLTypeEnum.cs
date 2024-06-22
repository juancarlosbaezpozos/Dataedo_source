using System.Collections.Generic;
using Npgsql;

namespace Dataedo.App.Enums;

public static class SSLTypeEnum
{
	public enum SSLType
	{
		Prefer = 0,
		Require = 1,
		Disable = 2,
		VerifyCA = 3,
		VerifyFull = 4
	}

	public static SSLType StringToType(string sslType)
	{
		return sslType switch
		{
			"DISABLE" => SSLType.Disable, 
			"REQUIRE" => SSLType.Require, 
			"VERIFY-CA" => SSLType.VerifyCA, 
			"VERIFYFULL" => SSLType.VerifyFull, 
			_ => SSLType.Prefer, 
		};
	}

	public static string TypeToString(SSLType sslType)
	{
		return sslType switch
		{
			SSLType.Prefer => "PREFER", 
			SSLType.Require => "REQUIRE", 
			SSLType.Disable => "DISABLE", 
			SSLType.VerifyCA => "VERIFY-CA", 
			SSLType.VerifyFull => "VERIFYFULL", 
			_ => null, 
		};
	}

	public static string TypeToStringForDisplay(SSLType sslType)
	{
		return sslType switch
		{
			SSLType.Prefer => "Prefer", 
			SSLType.Require => "Require", 
			SSLType.Disable => "Disable", 
			SSLType.VerifyCA => "Verify CA", 
			SSLType.VerifyFull => "Verify Full", 
			_ => sslType.ToString(), 
		};
	}

	public static SslMode StringToNpgsqlType(string sslType)
	{
		return sslType switch
		{
			"PREFER" => SslMode.Prefer, 
			"REQUIRE" => SslMode.Require, 
			"DISABLE" => SslMode.Disable, 
			_ => SslMode.Prefer, 
		};
	}

	public static Dictionary<SSLType, string> GetRedshiftSSLTypes()
	{
		return GetPostgreSqlSSLTypes();
	}

	public static Dictionary<SSLType, string> GetPostgreSqlSSLTypes()
	{
		return new Dictionary<SSLType, string>
		{
			[SSLType.Prefer] = TypeToStringForDisplay(SSLType.Prefer),
			[SSLType.Disable] = TypeToStringForDisplay(SSLType.Disable),
			[SSLType.Require] = TypeToStringForDisplay(SSLType.Require)
		};
	}

	public static Dictionary<SSLType, string> GetBasicSSLTypes()
	{
		return new Dictionary<SSLType, string>
		{
			[SSLType.Prefer] = TypeToStringForDisplay(SSLType.Prefer),
			[SSLType.Disable] = TypeToStringForDisplay(SSLType.Disable),
			[SSLType.Require] = TypeToStringForDisplay(SSLType.Require)
		};
	}

	public static Dictionary<SSLType, string> GetMySqlSSLTypes()
	{
		return new Dictionary<SSLType, string>
		{
			[SSLType.Prefer] = TypeToStringForDisplay(SSLType.Prefer),
			[SSLType.Disable] = TypeToStringForDisplay(SSLType.Disable),
			[SSLType.Require] = TypeToStringForDisplay(SSLType.Require),
			[SSLType.VerifyCA] = TypeToStringForDisplay(SSLType.VerifyCA),
			[SSLType.VerifyFull] = TypeToStringForDisplay(SSLType.VerifyFull)
		};
	}
}
