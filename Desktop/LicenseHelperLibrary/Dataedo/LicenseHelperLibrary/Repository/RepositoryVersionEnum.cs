namespace Dataedo.LicenseHelperLibrary.Repository;

public class RepositoryVersionEnum
{
    public enum RepositoryVersion
    {
        ERROR = 1,
        EQUALS = 2,
        EQUALS_NOT_STABLE = 3,
        OLDER = 4,
        NEWER = 5,
        NOT_SET = 6
    }
}
