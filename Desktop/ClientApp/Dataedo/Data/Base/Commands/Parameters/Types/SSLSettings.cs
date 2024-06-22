using Dataedo.App.Tools;

namespace Dataedo.Data.Base.Commands.Parameters.Types;

public class SSLSettings
{
    private string _pfxCertPassword;

    public string KeyPath { get; set; }

    public string CertPath { get; set; }

    public string CAPath { get; set; }

    public string Cipher { get; set; }

    public string CertPassword
    {
        get
        {
            return _pfxCertPassword;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _pfxCertPassword = value;
            }
            try
            {
                SimpleAES simpleAES = new SimpleAES();
                _pfxCertPassword = simpleAES.EncryptToString(value);
            }
            catch
            {
                _pfxCertPassword = string.Empty;
            }
        }
    }

    public static string DecryptCertPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return password;
        }
        return new SimpleAES().DecryptString(password);
    }

    public void SetEncryptedPassword(string encryptedPassword)
    {
        _pfxCertPassword = encryptedPassword;
    }
}
