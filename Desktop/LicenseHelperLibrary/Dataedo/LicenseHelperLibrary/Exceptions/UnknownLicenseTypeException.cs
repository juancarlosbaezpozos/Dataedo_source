using System;

namespace Dataedo.LicenseHelperLibrary.Exceptions;

public class UnknownLicenseTypeException : Exception
{
    public UnknownLicenseTypeException()
    {
    }

    public UnknownLicenseTypeException(string message)
        : base(message)
    {
    }

    public UnknownLicenseTypeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
