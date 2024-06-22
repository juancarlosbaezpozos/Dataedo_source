using System.Net;

namespace Dataedo.App.API.Models;

internal class Result
{
	public UnprocessableEntityResult Errors { get; set; }

	public HttpStatusCode StatusCode { get; set; }

	public bool IsValid { get; set; }

	public bool HasErrors => Errors != null;

	public bool ShouldProposeTryAgain
	{
		get
		{
			if (StatusCode != (HttpStatusCode)429)
			{
				return StatusCode == HttpStatusCode.InternalServerError;
			}
			return true;
		}
	}

	public bool IsNotUnauthorized => StatusCode != HttpStatusCode.Unauthorized;

	public bool IsOK
	{
		get
		{
			if (!HasErrors && IsValid && !ShouldProposeTryAgain)
			{
				return IsNotUnauthorized;
			}
			return false;
		}
	}

	public Result(HttpStatusCode statusCode, bool isValid)
	{
		StatusCode = statusCode;
		IsValid = isValid;
	}

	public Result(HttpStatusCode statusCode, UnprocessableEntityResult errors)
		: this(statusCode, isValid: false)
	{
		Errors = errors;
	}
}
