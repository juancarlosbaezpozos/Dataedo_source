using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dataedo.App.Tools;

public class SimpleAES
{
	private byte[] Key = new byte[32]
	{
		54, 25, 27, 22, 2, 56, 99, 44, 113, 144,
		217, 45, 67, 146, 141, 222, 55, 44, 12, 54,
		165, 21, 22, 34, 32, 221, 64, 121, 123, 211,
		122, 29
	};

	private byte[] Vector = new byte[16]
	{
		186, 23, 211, 33, 132, 87, 135, 121, 99, 120,
		66, 36, 122, 234, 114, 142
	};

	private ICryptoTransform EncryptorTransform;

	private ICryptoTransform DecryptorTransform;

	private UTF8Encoding UTFEncoder;

	public SimpleAES()
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		EncryptorTransform = rijndaelManaged.CreateEncryptor(Key, Vector);
		DecryptorTransform = rijndaelManaged.CreateDecryptor(Key, Vector);
		UTFEncoder = new UTF8Encoding();
	}

	public static byte[] GenerateEncryptionKey()
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.GenerateKey();
		return rijndaelManaged.Key;
	}

	public static byte[] GenerateEncryptionVector()
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.GenerateIV();
		return rijndaelManaged.IV;
	}

	public string EncryptToString(string TextValue)
	{
		return ByteArrToString(Encrypt(TextValue));
	}

	public byte[] Encrypt(string TextValue)
	{
		byte[] bytes = UTFEncoder.GetBytes(TextValue);
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.FlushFinalBlock();
		memoryStream.Position = 0L;
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		cryptoStream.Close();
		memoryStream.Close();
		return array;
	}

	public string DecryptString(string EncryptedString)
	{
		return Decrypt(StrToByteArray(EncryptedString));
	}

	public string Decrypt(byte[] EncryptedValue)
	{
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, DecryptorTransform, CryptoStreamMode.Write);
		cryptoStream.Write(EncryptedValue, 0, EncryptedValue.Length);
		cryptoStream.FlushFinalBlock();
		memoryStream.Position = 0L;
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		memoryStream.Close();
		return UTFEncoder.GetString(array);
	}

	public byte[] StrToByteArray(string str)
	{
		if (str == null || str.Length == 0)
		{
			throw new Exception("Invalid string value in StrToByteArray");
		}
		byte[] array = new byte[str.Length / 3];
		int num = 0;
		int num2 = 0;
		do
		{
			byte b = byte.Parse(str.Substring(num, 3));
			array[num2++] = b;
			num += 3;
		}
		while (num < str.Length);
		return array;
	}

	public string ByteArrToString(byte[] byteArr)
	{
		string text = "";
		for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
		{
			byte b = byteArr[i];
			text = ((b >= 10) ? ((b >= 100) ? (text + b) : (text + "0" + b)) : (text + "00" + b));
		}
		return text;
	}
}
