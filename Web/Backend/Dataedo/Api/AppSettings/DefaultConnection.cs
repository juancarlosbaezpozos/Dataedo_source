using Dataedo.Repository.Services.Services.Cryptography;

namespace Dataedo.Api.AppSettings;

/// <summary>
/// The class providing default settings for connecting to database repository.
/// </summary>
public class DefaultConnection
{
    private string connectionString;

    /// <summary>
    /// Gets or sets the default connection string used for connecting to database repository.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return SimpleAES.GetDecrypted(connectionString);
        }
        set
        {
            connectionString = value;
        }
    }
}
