namespace Dataedo.LicenseHelperLibrary.Repository;

public class RepositoryLicense
{
    public string Login { get; set; }

    public string Pass { get; set; }

    public string HiddenPass { get; set; }

    public bool IsActive { get; set; }

    public bool IsDomainUser { get; set; }

    public bool IsLoginExists { get; set; }

    public RepositoryLicense(string login, string pass, bool isActive, bool isLoginExists)
    {
        Login = login;
        Pass = pass;
        HiddenPass = (string.IsNullOrEmpty(pass) ? string.Empty : "**********");
        IsActive = isActive;
        IsDomainUser = login.Contains("\\");
        IsLoginExists = isLoginExists;
    }

    public void Copy(RepositoryLicense license)
    {
        Login = license.Login;
        Pass = license.Pass;
        IsActive = license.IsActive;
    }

    public bool IsComplete(bool domain)
    {
        if (domain)
        {
            return !string.IsNullOrEmpty(Login);
        }
        if (!string.IsNullOrEmpty(Login))
        {
            return !string.IsNullOrEmpty(Pass);
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        RepositoryLicense repositoryLicense = obj as RepositoryLicense;
        return Login == repositoryLicense.Login;
    }

    public override int GetHashCode()
    {
        return Login.GetHashCode();
    }
}
