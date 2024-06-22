using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Dataedo.LicenseHelperLibrary.Enums;
using Dataedo.LicenseHelperLibrary.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Dataedo.LicenseHelperLibrary.License;

[DebuggerDisplay("{Type} {Version}")]
public class LicenseKey
{
    private AsymmetricKeyParameter privateKey;

    private AsymmetricKeyParameter publicKey;

    public string Code { get; set; }

    public LicenseTypeEnum.LicenseType Type { get; private set; }

    public int Version { get; set; }

    public int Number { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int PackId { get; set; }

    public int UserId { get; private set; }

    public string CompanyName { get; set; }

    public Exception InitializationException { get; set; }

    public bool IsParsableLicense => InitializationException == null;

    public DateTime ExpirationDate => PurchaseDate.AddDays(LicenseTypeEnum.GetTimeDuration(Type) + 1);

    public string CompanyNameForDisplay
    {
        get
        {
            if (LicenseTypeEnum.IsTrial(Type))
            {
                return "Trial";
            }
            return "Licensed for " + CompanyName;
        }
    }

    public bool IsTrial => LicenseTypeEnum.IsTrial(Type);

    public string TypeDisplay => LicenseTypeEnum.ToString(Type);

    public string VersionTypeDisplay => $"Dataedo {Version} {TypeDisplay}";

    public string TypeWithRemainingDays(DateTime actualDate)
    {
        string text = LicenseTypeEnum.ToStringWithoutTimeLimit(Type);
        if (!LicenseTypeEnum.IsTimeLimited(Type))
        {
            return $"{Version} {text}";
        }
        string trialLeftDaysString = GetTrialLeftDaysString(actualDate);
        return text + " (" + trialLeftDaysString + ")";
    }

    public string GetTrialLeftDaysString(DateTime actualDate)
    {
        int trialLeftDays = GetTrialLeftDays(actualDate, PurchaseDate);
        return GetTrialLeftDaysString(trialLeftDays);
    }

    private int GetTrialLeftDays(DateTime actualDate, DateTime trialPurchaseDate)
    {
        int timeDuration = LicenseTypeEnum.GetTimeDuration(Type);
        return (trialPurchaseDate.AddDays(timeDuration) - actualDate).Days;
    }

    private string GetTrialLeftDaysString(int leftDays)
    {
        string arg = ((leftDays > 1) ? "s" : string.Empty);
        return $"{leftDays} day{arg} left";
    }

    public LicenseKey()
    {
    }

    public LicenseKey(string code)
    {
        code = TrimKey(code);
        Code = code;
        if (string.IsNullOrWhiteSpace(code))
        {
            InitializationException = new IncorrectKeyException("Incorrect key", new FormatException("Key is empty."));
            return;
        }
        if (code.Length != 256)
        {
            InitializationException = new IncorrectKeyException("Incorrect key", new FormatException($"Incorrect key length (provided key length: {code.Length})." + Environment.NewLine + "Valid key length: 256 characters."));
            return;
        }
        byte[] rawData = Convert.FromBase64String("MIIBuTCCASKgAwIBAgIQfAmeN9ud3aJJDZYsiDoCAzANBgkqhkiG9w0BAQUFADAbMRkwFwYDVQQDExBUZXN0IENlcnRpZmljYXRlMB4XDTE0MDQxNTEwMDYzN1oXDTE1MTIzMTAwMDAwMFowGzEZMBcGA1UEAxMQVGVzdCBDZXJ0aWZpY2F0ZTCBnzANBgkqhkiG9w0BAQEFAAOBjQAwgYkCgYEAsf5509yFLmHETDkDmzDeK6qGejHDxJresVWyLNAFqassyRJ2c/PY65yDh4Cs+cPxHb9mgA1OFciTnBPtW5CWhSCkBDzFcTqzv4/0pSRtkllSKvLHacCuTkax+nfGlbxTGwwCLyP5qUWn9CAY04P1megNRslQxS1DFQz2xKbTJIMCAwEAATANBgkqhkiG9w0BAQUFAAOBgQBlF2r38+uyIk9sxdDRr0TF4pMFl2rzSsMPIR0dz+WkPJ9hScp38tmwoqSXl5MyJv5g4U/8PnTxh35c1qkAlLISWayGUEqJerMVrrSErQYeWfhYnj/u7+zmQwA+II0P6fP4t9QUwfrm0JM8f2W4J35Z7AVuurMT9bEo8NSrs8fhBQ==");
        string s = "BwIAAACkAABSU0EyAAQAAAEAAQCDJNOmxPYMFUMtxVDJRg3omfWD0xgg9KdFqfkjLwIMG1O8lcZ3+rFGTq7AacfyKlJZkm0kpfSPv7M6ccU8BKQghZaQW+0TnJPIFU4NgGa/HfHD+ayAh4Oc69jzc3YSySyrqQXQLLJVsd6axMMxeoaqK94wmwM5TMRhLoXc03n+sekcHnm0WxWhsJcAQAFgpnoLGswzN6zDyTRypmfLhQcH4cjJCX3asDqGN64HBWvHPUlVQ8KUv1/Il7mJgVlowe2Lov426I4cM7FLcbqXDifM8AnQKaCR2+YneQPPRcTh5yz1Vv/u0Im2L5KCSvruhPlP+DLYZsrpPoOxYhBTFae/2RgeuTkIR3Qzhy2O78UD1QH3+n4dB1Yq5slLJkhJp5OEDJtVIqGnH2ted1FOIM+YhUOAECPTOgkeBKcXQaHRhWF2XzXnh1f5watkNCdyGAJ4AWKcnMX51k+LwQPRwuwQ2AAdgUIOnRpQdX9fQI0NhRbsBDOgrV+D/JLPvDNH+jZmCJ7heswM9p8IUyr0ganO9s6a1pYYeDxykPivhIAs5RhIBQV6RQVra7Py6QX2lp9E3Y+PzjTDaVZdiKO7drplYaJXbFxhX5GzVqEdJTDr0OIqaXjPN6soKNB47jI/3FnhFSOHpLBxykhnpfpRWXw1KKYgKtZdaqH/CRfBUSem1hxCFhxS2pBNT3pLFTT5ydLfYaNo2AVXwh8WksgK5VJ0rDlQ1/MVtJvwAO8dy0m2+yVUL5gYBtGGVSYsEM1rT2Q=";
        X509Certificate2 x509Certificate = new X509Certificate2(rawData);
        RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
        rSACryptoServiceProvider.ImportCspBlob(Convert.FromBase64String(s));
        x509Certificate.PrivateKey = rSACryptoServiceProvider;
        Org.BouncyCastle.X509.X509Certificate x509Certificate2 = new X509CertificateParser().ReadCertificate(x509Certificate.GetRawCertData());
        privateKey = DotNetUtilities.GetKeyPair((RSACryptoServiceProvider)x509Certificate.PrivateKey).Private;
        publicKey = x509Certificate2.GetPublicKey();
        try
        {
            byte[] values = DecryptStringWithFileCert(code);
            SetValues(values);
        }
        catch (UnknownLicenseTypeException ex)
        {
            UnknownLicenseTypeException ex3 = (UnknownLicenseTypeException)(InitializationException = ex);
        }
        catch (Exception inner)
        {
            InitializationException = new IncorrectKeyException("Incorrect key", inner);
        }
    }

    public bool IsStrongerThan(LicenseKey anotherLicenseKey, int majorProgramVersion, DateTime actualDate)
    {
        bool flag = Version >= majorProgramVersion || (LicenseTypeEnum.IsTimeLimited(Type) && PurchaseDate.AddDays(LicenseTypeEnum.GetTimeDuration(Type)) >= actualDate);
        bool flag2 = anotherLicenseKey.Version >= majorProgramVersion || (LicenseTypeEnum.IsTimeLimited(anotherLicenseKey.Type) && anotherLicenseKey.PurchaseDate.AddDays(LicenseTypeEnum.GetTimeDuration(anotherLicenseKey.Type)) >= actualDate);
        if (flag && !flag2)
        {
            return true;
        }
        if (!flag && flag2)
        {
            return false;
        }
        if (string.IsNullOrEmpty(anotherLicenseKey.Code))
        {
            return true;
        }
        if (string.IsNullOrEmpty(Code))
        {
            return false;
        }
        int importance = LicenseTypeEnum.GetImportance(Type);
        int importance2 = LicenseTypeEnum.GetImportance(anotherLicenseKey.Type);
        if (importance != importance2)
        {
            return importance > importance2;
        }
        return Version > anotherLicenseKey.Version;
    }

    public static string TrimKey(string key)
    {
        if (key == null)
        {
            return null;
        }
        return Regex.Replace(key, "\\s+", "").Replace("-", string.Empty);
    }

    private void SetValues(byte[] key)
    {
        switch (key[0])
        {
            case 1:
                Type = LicenseTypeEnum.LicenseType.Lite;
                break;
            case 2:
            case 11:
                Type = LicenseTypeEnum.LicenseType.Pro;
                break;
            case 7:
                Type = LicenseTypeEnum.LicenseType.Demo;
                break;
            case 8:
                Type = LicenseTypeEnum.LicenseType.Pro30;
                break;
            case 9:
                Type = LicenseTypeEnum.LicenseType.Pro90;
                break;
            case 10:
            case 12:
                Type = LicenseTypeEnum.LicenseType.Pro365;
                break;
            case 13:
                Type = LicenseTypeEnum.LicenseType.Education;
                break;
            case 14:
                Type = LicenseTypeEnum.LicenseType.Education365;
                break;
            case 15:
                Type = LicenseTypeEnum.LicenseType.ProPlus;
                break;
            case 16:
                Type = LicenseTypeEnum.LicenseType.ProPlus365;
                break;
            case 17:
                Type = LicenseTypeEnum.LicenseType.Enterprise;
                break;
            case 18:
                Type = LicenseTypeEnum.LicenseType.Enterprise365;
                break;
            case 3:
            case 19:
                Type = LicenseTypeEnum.LicenseType.EnterpriseTrial;
                break;
            case 4:
            case 20:
                Type = LicenseTypeEnum.LicenseType.EnterpriseTrial30;
                break;
            case 5:
            case 21:
                Type = LicenseTypeEnum.LicenseType.EnterpriseTrial90;
                break;
            case 6:
            case 22:
                Type = LicenseTypeEnum.LicenseType.EnterpriseTrial365;
                break;
            case 23:
                Type = LicenseTypeEnum.LicenseType.Viewer365;
                break;
            case 24:
                Type = LicenseTypeEnum.LicenseType.Viewer60;
                break;
            case 25:
                Type = LicenseTypeEnum.LicenseType.Viewer90;
                break;
            case 26:
                Type = LicenseTypeEnum.LicenseType.Viewer120;
                break;
            case 27:
                Type = LicenseTypeEnum.LicenseType.Pro60;
                break;
            case 28:
                Type = LicenseTypeEnum.LicenseType.Pro120;
                break;
            case 29:
                Type = LicenseTypeEnum.LicenseType.ProPlus60;
                break;
            case 30:
                Type = LicenseTypeEnum.LicenseType.ProPlus90;
                break;
            case 31:
                Type = LicenseTypeEnum.LicenseType.ProPlus120;
                break;
            case 32:
                Type = LicenseTypeEnum.LicenseType.Enterprise60;
                break;
            case 33:
                Type = LicenseTypeEnum.LicenseType.Enterprise90;
                break;
            case 34:
                Type = LicenseTypeEnum.LicenseType.Enterprise120;
                break;
            case 35:
            case 36:
                Type = LicenseTypeEnum.LicenseType.EnterpriseTrial7;
                break;
            case 37:
                Type = LicenseTypeEnum.LicenseType.Pro180;
                break;
            case 38:
                Type = LicenseTypeEnum.LicenseType.ProPlus30;
                break;
            case 39:
                Type = LicenseTypeEnum.LicenseType.ProPlus180;
                break;
            case 40:
                Type = LicenseTypeEnum.LicenseType.Enterprise30;
                break;
            case 41:
                Type = LicenseTypeEnum.LicenseType.Enterprise180;
                break;
            default:
                throw new UnknownLicenseTypeException("This license type is not supported in Dataedo desktop.");
        }
        Version = key[1];
        Number = key[2] + key[3] * 256;
        int year = key[4] * 100 + key[5];
        byte month = key[6];
        byte day = key[7];
        PurchaseDate = new DateTime(year, month, day);
        PackId = key[8] + key[9] * 256 + key[10] * 256 * 256;
        UserId = key[11] + key[12] * 256 + key[13] * 256 * 256 + key[14] * 256 * 256 * 256;
        int i;
        for (i = 0; key[15 + i] != 0; i++)
        {
        }
        CompanyName = Encoding.ASCII.GetString(key, 15, i);
    }

    private byte[] DecryptStringWithFileCert(string key)
    {
        byte[] data = DecryptCode(key);
        return Decrypt(data) ?? throw new Exception("Decryption with RSA failed");
    }

    private byte[] Decrypt(byte[] data)
    {
        RsaEngine rsaEngine = new RsaEngine();
        rsaEngine.Init(forEncryption: false, privateKey);
        int inputBlockSize = rsaEngine.GetInputBlockSize();
        List<byte> list = new List<byte>();
        for (int i = 0; i < data.Length; i += inputBlockSize)
        {
            int inLen = Math.Min(inputBlockSize, data.Length - i * inputBlockSize);
            list.AddRange(rsaEngine.ProcessBlock(data, i, inLen));
        }
        return list.ToArray();
    }

    private byte[] DecryptCode(string code)
    {
        int num = code.Length / 2;
        byte[] array = new byte[num];
        for (int i = 0; i < num; i++)
        {
            int num2 = (code[i * 2] - 65 << 4) + code[i * 2 + 1] - 65;
            if (num2 > 255)
            {
                Console.WriteLine();
            }
            array[i] = (byte)num2;
        }
        return array;
    }

    private byte[] GenerateBytes()
    {
        byte[] bytes = Encoding.ASCII.GetBytes(CompanyName);
        int length = bytes.Length;
        byte[] array = new byte[117];
        array[0] = (byte)Type;
        array[1] = (byte)Version;
        byte[] bytes2 = BitConverter.GetBytes(Number);
        array[2] = bytes2[0];
        array[3] = bytes2[1];
        string text = PurchaseDate.Year.ToString();
        array[4] = byte.Parse(text.Substring(0, 2));
        array[5] = byte.Parse(text.Substring(2, 2));
        array[6] = (byte)PurchaseDate.Month;
        array[7] = (byte)PurchaseDate.Day;
        byte[] bytes3 = BitConverter.GetBytes(PackId);
        array[8] = bytes3[0];
        array[9] = bytes3[1];
        array[10] = bytes3[2];
        byte[] bytes4 = BitConverter.GetBytes(UserId);
        array[11] = bytes4[0];
        array[12] = bytes4[1];
        array[13] = bytes4[2];
        array[14] = bytes4[3];
        Array.Copy(bytes, 0, array, 15, length);
        return array;
    }

    private string GenerateCode(byte[] bytes)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in bytes)
        {
            byte value = (byte)((num & 0xF) + 65);
            byte value2 = (byte)((num & 0xF0) / 16 + 65);
            stringBuilder.Append((char)value2);
            stringBuilder.Append((char)value);
        }
        return stringBuilder.ToString();
    }

    public byte[] EncryptPasswordWithFileCert(byte[] bytes)
    {
        RsaEngine rsaEngine = new RsaEngine();
        rsaEngine.Init(forEncryption: true, publicKey);
        int inputBlockSize = rsaEngine.GetInputBlockSize();
        List<byte> list = new List<byte>();
        for (int i = 0; i < bytes.Length; i += inputBlockSize)
        {
            int inLen = Math.Min(inputBlockSize, bytes.Length - i * inputBlockSize);
            list.AddRange(rsaEngine.ProcessBlock(bytes, i, inLen));
        }
        return list.ToArray();
    }

    public void SetUserId(int userId)
    {
        UserId = userId;
        Code = GetLicenseCode();
    }

    public void SetType(LicenseTypeEnum.LicenseType type)
    {
        Type = type;
        Code = GetLicenseCode();
    }

    public string GetCodeWithoutUserId()
    {
        LicenseKey licenseKey = new LicenseKey(Code);
        licenseKey.SetUserId(0);
        return licenseKey.Code;
    }

    private string GetLicenseCode()
    {
        byte[] bytes = GenerateBytes();
        byte[] bytes2 = EncryptPasswordWithFileCert(bytes);
        return GenerateCode(bytes2);
    }
}
