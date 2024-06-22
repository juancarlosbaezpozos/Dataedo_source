using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Dataedo.App.Tools;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public static class XmlFileSimpleAesService
{
	public static string EncryptData<T>(T data)
	{
		try
		{
			using StringWriter stringWriter = new StringWriter();
			new XmlSerializer(typeof(T)).Serialize(stringWriter, data);
			string textValue = stringWriter.ToString();
			return Convert.ToBase64String(new SimpleAES().Encrypt(textValue));
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static T DecryptData<T>(string data) where T : class
	{
		try
		{
			byte[] encryptedValue = Convert.FromBase64String(data);
			using StringReader input = new StringReader(new SimpleAES().Decrypt(encryptedValue));
			return new XmlSerializer(typeof(T)).Deserialize(XmlReader.Create(input)) as T;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
