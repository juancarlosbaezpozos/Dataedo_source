using System;
using System.IO;
using System.Text;
using Dataedo.LicenseHelperLibrary.License;
using Newtonsoft.Json;

namespace Dataedo.LicenseFile;

public class LicenseFileReader
{
    public LicenseFileModel LicenseFileModel { get; private set; }

    public bool SetLicenseFileData(string path)
    {
        LicenseFileModel licenseFileModel = null;
        using (StreamReader streamReader = new StreamReader(path))
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
            string[] array = streamReader.ReadToEnd().Split(new char[1] { '.' });
            if (array.Length != 2)
            {
                return false;
            }
            string text = array[0];
            string signature = array[1];
            if (!LicenseValidation.VerifySignature(text, signature))
            {
                return false;
            }
            licenseFileModel = JsonConvert.DeserializeObject<LicenseFileModel>(Encoding.UTF8.GetString(Convert.FromBase64String(text)), jsonSerializerSettings);
        }
        LicenseFileModel = licenseFileModel;
        return true;
    }
}
