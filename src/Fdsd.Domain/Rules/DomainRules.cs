using System;
using Fdsd.Domain.Enums;

namespace Fdsd.Domain.Rules;

public static class DomainRules
{
    public static readonly DateTime MaxDate = new DateTime(9999, 12, 31);
    public const int MinYear = 2000;
    public const int KCodeFdsd = 101;
    public const int KCodeShusai = 102;

    public static bool IsTargetDate(DateTime targetStart, DateTime targetEnd, DateTime compareStart, DateTime compareEnd)
    {
        return targetStart <= compareEnd && targetEnd >= compareStart;
    }

    public static DateTime EffectiveEndDate(DateTime? date)
    {
        return date ?? MaxDate;
    }

    /// <summary>
    /// Check if user belongs to the target department at the time of training.
    /// </summary>
    public static bool IsBelongingAt(DateTime dateOfArrival, DateTime? dateOfDeparture, DateTime trainingStartDate, DateTime? trainingEndDate)
    {
        var effectiveEnd = EffectiveEndDate(dateOfDeparture);
        var effectiveTrainingEnd = trainingEndDate ?? trainingStartDate;
        return dateOfArrival <= effectiveTrainingEnd && effectiveEnd >= trainingStartDate;
    }

    /// <summary>
    /// Determine attendance symbol for display/report
    /// </summary>
    public static string GetAttendSymbol(short? attend, DateTime? kenshuDate, DateTime? nyusyaYmd, DateTime? taisyaYmd, DateTime? leaveStart, DateTime? leaveEnd)
    {
        if (kenshuDate == null)
            return "／";

        if (nyusyaYmd.HasValue && kenshuDate.Value < nyusyaYmd.Value)
            return "／";

        if (taisyaYmd.HasValue && taisyaYmd.Value < kenshuDate.Value)
            return "／";

        if (leaveStart.HasValue && leaveEnd.HasValue && leaveStart.Value <= kenshuDate.Value && kenshuDate.Value <= leaveEnd.Value)
            return "／";

        if (leaveStart.HasValue && !leaveEnd.HasValue && leaveStart.Value <= kenshuDate.Value)
            return "／";

        return attend switch
        {
            0 => "－",
            1 => "○",
            3 => "／",
            _ => "－"
        };
    }

    /// <summary>
    /// Determine FDSD kubun from department code.
    /// 1000s = staff (SD=1), 2000s = faculty (FD=2)
    /// </summary>
    public static FdsdKubun GetFdsdKubunFromGakkaCd(short gakkaCd)
    {
        return gakkaCd >= 2000 ? FdsdKubun.Fd : FdsdKubun.Sd;
    }

    /// <summary>
    /// Check if person is a non-attendee (teacher checks both FD+SD, staff checks SD only)
    /// </summary>
    public static bool IsNonAttendeeTarget(FdsdKubun userFdsd)
    {
        return userFdsd is FdsdKubun.Fd or FdsdKubun.Sd;
    }
}