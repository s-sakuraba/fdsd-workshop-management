using System;

namespace Fdsd.Application.Abstractions;

public interface IClock
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}