namespace Dataedo.App.Tools;

public static class ConnectionHelper
{
	public static HostAndPort GetSplittedHost(string host)
	{
		if (string.IsNullOrWhiteSpace(host))
		{
			return null;
		}
		char[] separator = new char[2] { ':', ',' };
		string[] array = host.Split(separator);
		if (array.Length == 0)
		{
			return null;
		}
		if (array.Length != 2)
		{
			return null;
		}
		_ = array[1];
		if (!ushort.TryParse(array[1], out var result))
		{
			return null;
		}
		string host2 = array[0];
		return new HostAndPort
		{
			Host = host2,
			Port = result.ToString()
		};
	}
}
