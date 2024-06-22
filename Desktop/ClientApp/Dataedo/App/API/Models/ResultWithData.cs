using System.Net;

namespace Dataedo.App.API.Models;

internal class ResultWithData<T> : Result
{
	public T Data { get; set; }

	public ResultWithData(HttpStatusCode statusCode, bool isValid)
		: base(statusCode, isValid)
	{
		base.StatusCode = statusCode;
	}

	public ResultWithData(HttpStatusCode statusCode, T data)
		: this(statusCode, isValid: true)
	{
		Data = data;
	}

	public ResultWithData(HttpStatusCode statusCode, UnprocessableEntityResult errors)
		: base(statusCode, errors)
	{
		base.Errors = errors;
	}
}
