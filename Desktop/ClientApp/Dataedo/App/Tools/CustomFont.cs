using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Dataedo.App.Tools;

public class CustomFont
{
	private static PrivateFontCollection privateFontCollection;

	public static FontFamily AleoLight
	{
		get
		{
			if (privateFontCollection == null)
			{
				LoadFont();
			}
			return privateFontCollection.Families[0];
		}
	}

	private static void LoadFont()
	{
		privateFontCollection = new PrivateFontCollection();
		string name = "MetadataEditor.Resources.aleo-light-webfont.ttf";
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		IntPtr intPtr = Marshal.AllocCoTaskMem((int)manifestResourceStream.Length);
		byte[] array = new byte[manifestResourceStream.Length];
		manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
		Marshal.Copy(array, 0, intPtr, (int)manifestResourceStream.Length);
		privateFontCollection.AddMemoryFont(intPtr, (int)manifestResourceStream.Length);
		manifestResourceStream.Close();
		Marshal.FreeCoTaskMem(intPtr);
	}
}
