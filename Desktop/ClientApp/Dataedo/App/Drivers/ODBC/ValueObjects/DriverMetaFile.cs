using System;

namespace Dataedo.App.Drivers.ODBC.ValueObjects;

public class DriverMetaFile : ICloneable
{
	public const string FileName = "driver.xml";

	public string UID { get; set; }

	public string Name { get; set; }

	public string Version { get; set; }

	public DriverMetaFile()
	{
	}

	public DriverMetaFile(string uid, string name, string version = null)
	{
		UID = uid;
		Name = name;
		Version = version;
	}

	public object Clone()
	{
		DriverMetaFile obj = (DriverMetaFile)MemberwiseClone();
		obj.UID = obj.UID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
		obj.Name = obj.Name + " (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")";
		return obj;
	}

	public override string ToString()
	{
		return Name ?? UID;
	}
}
