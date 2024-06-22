namespace Dataedo.App.Enums;

public static class SqlServerConnectionModeEnum
{
	public enum SqlServerConnectionMode
	{
		ForceEncryptionRequireTrustedCertificate = 0,
		ForceEncryptionTrustServerCertificate = 1,
		EncryptConnectionIfPossible = 2
	}

	public static SqlServerConnectionMode DefaultMode = SqlServerConnectionMode.EncryptConnectionIfPossible;

	public static string DefaultModeString => TypeToString(DefaultMode);

	public static string StringParsedOrDefault(string connectionMode)
	{
		return TypeToString(StringToType(connectionMode) ?? DefaultMode);
	}

	public static SqlServerConnectionMode StringToTypeOrDefault(string connectionMode)
	{
		return StringToType(connectionMode) ?? DefaultMode;
	}

	public static SqlServerConnectionMode? StringToType(string connectionMode)
	{
		return connectionMode switch
		{
			"FORCE_ENCRYPTION_REQUIRE_TRUSTED_CERTIFICATE" => SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate, 
			"FORCE_ENCRYPTION_TRUST_SERVER_CERTIFICATE" => SqlServerConnectionMode.ForceEncryptionTrustServerCertificate, 
			"ENCRYPT_CONNECTION_IF_POSSIBLE" => SqlServerConnectionMode.EncryptConnectionIfPossible, 
			_ => null, 
		};
	}

	public static string TypeToString(SqlServerConnectionMode? connectionMode)
	{
		return connectionMode switch
		{
			SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate => "FORCE_ENCRYPTION_REQUIRE_TRUSTED_CERTIFICATE", 
			SqlServerConnectionMode.ForceEncryptionTrustServerCertificate => "FORCE_ENCRYPTION_TRUST_SERVER_CERTIFICATE", 
			SqlServerConnectionMode.EncryptConnectionIfPossible => "ENCRYPT_CONNECTION_IF_POSSIBLE", 
			_ => null, 
		};
	}

	public static string TypeToStringForDisplay(SqlServerConnectionMode? connectionMode)
	{
		return connectionMode switch
		{
			SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate => "Force encryption, require trusted certificate", 
			SqlServerConnectionMode.ForceEncryptionTrustServerCertificate => "Force encryption, trust server certificate", 
			SqlServerConnectionMode.EncryptConnectionIfPossible => "Encrypt connection if possible", 
			_ => null, 
		};
	}
}
