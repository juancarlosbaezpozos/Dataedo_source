using System;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage;

public static class AzureStorageAuthentication
{
	public enum AzureStorageAuthenticationEnum
	{
		AccessKey = 0,
		AzureADInteractive = 1,
		ConnectionString = 2,
		PublicContainer = 3,
		SharedAccessSignatureAccount = 4,
		SharedAccessSignatureDirectory = 5,
		SharedAccessSignatureURL = 6
	}

	public const string AccessKeyString = "ACCESS_KEY";

	public const string AzureADInteractiveString = "AZURE_AD_INTERACTIVE";

	public const string ConnectionStringString = "CONNECTION_STRING";

	public const string PublicContainerString = "PUBLIC_CONTAINER";

	public const string SharedAccessSignatureAccountString = "SHARED_ACCESS_SIGNATURE_ACCOUNT";

	public const string SharedAccessSignatureDirString = "SHARED_ACCESS_SIGNATURE_DIR";

	public const string SharedAccessSignatureURLString = "SHARED_ACCESS_SIGNATURE_URL";

	public const AzureStorageAuthenticationEnum DefaultAuthentication = AzureStorageAuthenticationEnum.AccessKey;

	public static string ToDisplayString(AzureStorageAuthenticationEnum authEnum)
	{
		return authEnum switch
		{
			AzureStorageAuthenticationEnum.AccessKey => "Access Key", 
			AzureStorageAuthenticationEnum.AzureADInteractive => "Azure Active Directory - Interactive", 
			AzureStorageAuthenticationEnum.ConnectionString => "Connection String", 
			AzureStorageAuthenticationEnum.PublicContainer => "Public Container", 
			AzureStorageAuthenticationEnum.SharedAccessSignatureAccount => "Shared Access Signature - Account", 
			AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory => "Shared Access Signature - Directory", 
			AzureStorageAuthenticationEnum.SharedAccessSignatureURL => "Shared Access Signature - URL", 
			_ => throw new ArgumentException("Unknown authentication method", "authEnum"), 
		};
	}

	public static string ToString(AzureStorageAuthenticationEnum? authEnum)
	{
		return authEnum switch
		{
			AzureStorageAuthenticationEnum.AccessKey => "ACCESS_KEY", 
			AzureStorageAuthenticationEnum.AzureADInteractive => "AZURE_AD_INTERACTIVE", 
			AzureStorageAuthenticationEnum.ConnectionString => "CONNECTION_STRING", 
			AzureStorageAuthenticationEnum.PublicContainer => "PUBLIC_CONTAINER", 
			AzureStorageAuthenticationEnum.SharedAccessSignatureAccount => "SHARED_ACCESS_SIGNATURE_ACCOUNT", 
			AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory => "SHARED_ACCESS_SIGNATURE_DIR", 
			AzureStorageAuthenticationEnum.SharedAccessSignatureURL => "SHARED_ACCESS_SIGNATURE_URL", 
			_ => null, 
		};
	}

	public static AzureStorageAuthenticationEnum? FromString(string authEnumString)
	{
		return authEnumString switch
		{
			"ACCESS_KEY" => AzureStorageAuthenticationEnum.AccessKey, 
			"AZURE_AD_INTERACTIVE" => AzureStorageAuthenticationEnum.AzureADInteractive, 
			"CONNECTION_STRING" => AzureStorageAuthenticationEnum.ConnectionString, 
			"PUBLIC_CONTAINER" => AzureStorageAuthenticationEnum.PublicContainer, 
			"SHARED_ACCESS_SIGNATURE_ACCOUNT" => AzureStorageAuthenticationEnum.SharedAccessSignatureAccount, 
			"SHARED_ACCESS_SIGNATURE_DIR" => AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory, 
			"SHARED_ACCESS_SIGNATURE_URL" => AzureStorageAuthenticationEnum.SharedAccessSignatureURL, 
			_ => null, 
		};
	}
}
