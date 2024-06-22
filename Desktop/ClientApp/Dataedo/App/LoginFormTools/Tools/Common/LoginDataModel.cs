namespace Dataedo.App.LoginFormTools.Tools.Common;

internal class LoginDataModel
{
	public string Token { get; protected set; }

	public string Email { get; protected set; }

	public LoginDataModel(string token, string email)
	{
		Token = token;
		Email = email;
	}
}
