using System;

namespace Fdsd.Application.Common;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}