using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dataedo.Api.Authorization.Users;

/// <summary>
/// The class providing logged users data.
/// </summary>
public static class UsersStorage
{
    private static readonly object LockObject = new object();

    /// <summary>
    /// The dictionary providing base data about user identified by user identifier (key).
    /// </summary>
    private static readonly ConcurrentDictionary<string, UserData> Users = new ConcurrentDictionary<string, UserData>();

    /// <summary>
    /// Adds user to session storage.
    /// </summary>
    /// <param name="identifier">The unique identifier of user.</param>
    /// <param name="userData">The base data about user</param>
    /// <returns><c>true</c> if user data is added successfully; otherwise, <c>false</c>.</returns>
    public static bool AddUser(string identifier, UserData userData)
    {
        bool status = Users.TryAdd(identifier, userData);
        if (status)
        {
            lock (LockObject)
            {
                IEnumerable<string> identifiers = from x in Users
                                                  where x.Key != identifier
                                                  where x.Value.Username == userData.Username
                                                  select x.Key;
                foreach (string id in identifiers)
                {
                    RemoveUser(id);
                }
                return status;
            }
        }
        return status;
    }

    /// <summary>
    /// Gets user from session storage by user identifier.
    /// </summary>
    /// <param name="identifier">The unique identifier of user.</param>
    /// <returns>The user data if it is stored; otherwise <c>null</c>.</returns>
    public static UserData GetUser(string identifier)
    {
        Users.TryGetValue(identifier, out var userData);
        return userData;
    }

    /// <summary>
    /// Removes user from storage.
    /// </summary>
    /// <param name="identifier">The unique identifier of user.</param>
    public static void RemoveUser(string identifier)
    {
        Users.TryRemove(identifier, out var _);
    }

    /// <summary>
    /// Checks whether user exsists in storage.
    /// </summary>
    /// <param name="identifier">The unique identifier of user.</param>
    public static bool CheckIfUserExists(string identifier)
    {
        return Users.ContainsKey(identifier);
    }

    /// <summary>
    /// Removes from storage users that theirs refresh token is expired.
    /// </summary>
    public static void RemoveRecordsWithExpiredTokens()
    {
        try
        {
            lock (LockObject)
            {
                IEnumerable<KeyValuePair<string, UserData>> records = Users.Where((KeyValuePair<string, UserData> x) => x.Value.IsRefreshTokenExpired);
                using IEnumerator<KeyValuePair<string, UserData>> enumerator = records.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Users.Remove(enumerator.Current.Key,out var _);
                }
            }
        }
        catch
        {
        }
    }
}
