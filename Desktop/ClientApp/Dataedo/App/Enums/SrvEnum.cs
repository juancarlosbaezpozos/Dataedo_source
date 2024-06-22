namespace Dataedo.App.Enums;

public static class SrvEnum
{
	public enum Srv
	{
		None = 0,
		SRV = 1
	}

	public static Srv StringToType(string srv)
	{
		if (srv == "SRV")
		{
			return Srv.SRV;
		}
		return Srv.None;
	}

	public static string TypeToString(Srv? srv)
	{
		if (srv.HasValue && srv.GetValueOrDefault() == Srv.SRV)
		{
			return "SRV";
		}
		return null;
	}
}
