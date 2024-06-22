namespace Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data;

public class ConnectedUser
{
	public string Host { get; set; }

	public string Login { get; set; }

	public string Application { get; set; }

	public ConnectedUser(string host, string login, string application)
	{
		Host = host;
		Login = login;
		Application = application;
	}
}
