using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Dataedo.LicenseHelperLibrary.Enums;
using Dataedo.LicenseHelperLibrary.Repository;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Dataedo.LicenseHelperLibrary.License;

public class LicenseValidation
{
    private const string LicensePublicKey = "-----BEGIN PUBLIC KEY-----\r\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA5DSfZB7L/HCwlrZieJH8\r\nusnpobBqelUCBd8XkBLAmZ7mQivenJigz7Yy+BJLcIt8U1+BgtXnylUBRM5dDYJe\r\nfHDA5KvriupXDYPUj0gL0Z16ydXIBjanVWzAgYgIh6xiOvOaIHxDOD0pwdflA1bz\r\nqVws+BkxTzDG02wmnyDb7liprLOppFLtiKol2FDaU8U1n4y5jn2kAIwlHByobaZh\r\nzWUAzCweLhT1LNIObtAqYNYYFwfVrVpKmSSJg8Wsjai0H3itSoIv8GU5uGyjMtoT\r\nswpqcZjqYaOZLlup2xr29Jxr7lM+G1Ga6Y4onafIge4n8h1At0T1/MW80Wvou2y/\r\n7QIDAQAB\r\n-----END PUBLIC KEY-----\\n";

    private readonly int programVersion;

    private readonly bool isFileRepository;

    private readonly string connectionString;

    private Dictionary<ValidationResultEnum.ValidationResult, Action<LicenseKey, ConnectionParemeters>> validationActions;

    public LicenseValidation(int programVersion, bool isFileRepository, string connectionString)
    {
        this.programVersion = programVersion;
        this.isFileRepository = isFileRepository;
        this.connectionString = connectionString;
    }

    public LicenseValidation(int programVersion, string connectionString)
    {
        this.programVersion = programVersion;
        isFileRepository = false;
        this.connectionString = connectionString;
    }

    public LicenseValidation(int programVersion)
    {
        this.programVersion = programVersion;
        isFileRepository = true;
        connectionString = null;
    }

    public LicenseValidation AddOrReplaceValidationAction(ValidationResultEnum.ValidationResult validationResultType, Action<LicenseKey, ConnectionParemeters> action)
    {
        if (validationActions == null)
        {
            validationActions = new Dictionary<ValidationResultEnum.ValidationResult, Action<LicenseKey, ConnectionParemeters>>();
        }
        if (validationActions.ContainsKey(validationResultType))
        {
            validationActions[validationResultType] = action;
        }
        else
        {
            validationActions.Add(validationResultType, action);
        }
        return this;
    }

    public ValidationResultEnum.ValidationResult CheckIfIsValid(string keyCode, ConnectionParemeters connectionParemeters)
    {
        return CheckIfIsValid(new LicenseKey(keyCode), connectionParemeters);
    }

    public ValidationResultEnum.ValidationResult CheckIfIsValid(LicenseKey key, ConnectionParemeters connectionParemeters, bool allowViewer = false)
    {
        ValidationResultEnum.ValidationResult validationResult = ValidationResultEnum.ValidationResult.None;
        if (key.InitializationException != null)
        {
            validationResult = ((!string.IsNullOrEmpty(key.Code)) ? ValidationResultEnum.ValidationResult.IncorrectLicenseKey : ValidationResultEnum.ValidationResult.NoKey);
        }
        if (validationResult == ValidationResultEnum.ValidationResult.None && key.Type == LicenseTypeEnum.LicenseType.Lite)
        {
            validationResult = ValidationResultEnum.ValidationResult.Lite;
        }
        if (validationResult == ValidationResultEnum.ValidationResult.None && !LicenseTypeEnum.IsTimeLimited(key.Type) && programVersion > key.Version)
        {
            validationResult = ValidationResultEnum.ValidationResult.VersionNotSupported;
        }
        if (validationResult == ValidationResultEnum.ValidationResult.None && LicenseTypeEnum.IsTimeLimited(key.Type) && key.PurchaseDate.AddDays(LicenseTypeEnum.GetTimeDuration(key.Type)) < LicenseHelper.GetActualMaxDate(connectionString, isFileRepository))
        {
            if (LicenseTypeEnum.IsTrial(key.Type))
            {
                validationResult = ValidationResultEnum.ValidationResult.TrialEnded;
            }
            else if (!LicenseTypeEnum.IsTrial(key.Type))
            {
                validationResult = ValidationResultEnum.ValidationResult.SubscriptionExpired;
            }
        }
        if (!allowViewer && validationResult == ValidationResultEnum.ValidationResult.None && LicenseTypeEnum.IsNotSupportedViewer(key.Type))
        {
            validationResult = ValidationResultEnum.ValidationResult.InsufficientLicenseLevel;
        }
        if (validationResult == ValidationResultEnum.ValidationResult.None)
        {
            validationResult = ValidationResultEnum.ValidationResult.Valid;
        }
        InvokeValidationAction(validationResult, key, connectionParemeters);
        return validationResult;
    }

    private void InvokeValidationAction(ValidationResultEnum.ValidationResult validationResultType, LicenseKey key, ConnectionParemeters connectionParemeters)
    {
        Dictionary<ValidationResultEnum.ValidationResult, Action<LicenseKey, ConnectionParemeters>> dictionary = validationActions;
        if (dictionary != null && dictionary.ContainsKey(validationResultType))
        {
            validationActions[validationResultType]?.Invoke(key, connectionParemeters);
        }
    }

    public static bool VerifySignature(string encodedLicense, string signature)
    {
        if (string.IsNullOrEmpty(encodedLicense) || string.IsNullOrEmpty(signature))
        {
            return false;
        }
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(encodedLicense);
            byte[] signature2 = Convert.FromBase64String(signature);
            RSAParameters parameters = DotNetUtilities.ToRSAParameters((RsaKeyParameters)(AsymmetricKeyParameter)new PemReader(new StringReader(LicensePublicKey)).ReadObject());
            RSACng rSACng = new RSACng();
            rSACng.ImportParameters(parameters);
            if (rSACng.VerifyData(bytes, signature2, HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
