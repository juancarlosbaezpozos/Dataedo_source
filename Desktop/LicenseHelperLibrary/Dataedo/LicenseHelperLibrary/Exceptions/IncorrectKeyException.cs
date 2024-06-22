using System;

namespace Dataedo.LicenseHelperLibrary.Exceptions;

public class IncorrectKeyException : Exception
{
    public IncorrectKeyException()
    {
    }

    public IncorrectKeyException(string message)
        : base(message)
    {
    }

    public IncorrectKeyException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
