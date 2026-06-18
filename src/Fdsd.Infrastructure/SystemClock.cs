using System;
using Fdsd.Application.Abstractions;

namespace Fdsd.Infrastructure;

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}