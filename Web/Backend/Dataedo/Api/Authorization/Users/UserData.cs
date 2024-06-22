using System;
using Dataedo.Repository;

namespace Dataedo.Api.Authorization.Users;

/// <summary>
/// The class providing base data about user for storing in session.
/// </summary>
public class UserData
{
	/// <summary>
	/// Gets or sets the unique identifier of user.
	/// </summary>
	public string Identifier { get; set; }

	/// <summary>
	/// Gets or sets the username of user.
	/// </summary>
	public string Username { get; set; }

	/// <summary>
	/// Gets or sets the type of repository.
	/// </summary>
	public RepositoryType RepositoryType { get; set; }

	/// <summary>
	/// Gets or sets the connection string used for connecting to server repository.
	/// </summary>
	public string ConnectionString { get; set; }

	/// <summary>
	/// Gets or sets the token used for refreshing authentication token.
	/// </summary>
	public string RefreshToken { get; set; }

	/// <summary>
	/// Gets or sets the expiration date the token used for refreshing authentication token.
	/// </summary>
	public DateTime RefreshTokenExpirationDate { get; set; }

	/// <summary>
	/// Gets or sets the value indicating whether the token used for refreshing authentication token is expired.
	/// </summary>
	public bool IsRefreshTokenExpired => RefreshTokenExpirationDate <= DateTime.UtcNow;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Authorization.Users.UserData" /> class with values.
	/// </summary>
	/// <param name="identifier">The unique identifier of user.</param>
	/// <param name="username">The username of user.</param>
	/// <param name="repositoryType">The type of repository.</param>
	/// <param name="connectionString">The connection string used for connecting to server repository.</param>
	public UserData(string identifier, string username, RepositoryType repositoryType, string connectionString)
	{
		Identifier = identifier;
		Username = username;
		RepositoryType = repositoryType;
		ConnectionString = connectionString;
	}

	/// <summary>
	/// Sets the refresh token data.
	/// </summary>
	/// <param name="refreshToken">The token used for refreshing authentication token.</param>
	/// <param name="refreshTokenExpirationDate">The expiration date of token used for refreshing authentication token.</param>
	public void SetRefreshToken(string refreshToken, DateTime refreshTokenExpirationDate)
	{
		RefreshToken = refreshToken;
		RefreshTokenExpirationDate = refreshTokenExpirationDate;
	}
}
