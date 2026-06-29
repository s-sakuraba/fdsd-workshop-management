using System;
using System.Collections.Generic;

namespace Fdsd.Application.Common;

public record UserDto(int UserId, short EmpCd, string EmpName, string EmpUserNm, short GakkaCd, string GakkaName, DateTime? NyusyaDate, DateTime? TaisyaDate, short FdsdCd, short UserRole, short? ZaisyokuKbn, byte? EmpKubun);

public record GakkaDto(short GakkaCd, string GakkaName, string GakkaRyaku, short? FdsdCd, short OrderNo);

public record KenshuListItemDto(int KenshuCd, string KenshuName, DateTime KenshuDate, DateTime? EndDate, string FdsdName, string ShusaiName, string KenshuStyleName);

public record KenshuDetailDto(int KenshuCd, string KenshuName, DateTime KenshuDate, DateTime? EndDate, short FdsdCd, short ShusaiCd, string ShusaiName, short KstyleCd, string KstyleName, string? KenshuPlace, string? InfoDocu, List<KenshuDocumentDto> Documents, List<short> GakkaCds, List<string> GakkaNames);

public record KenshuDocumentDto(int Id, string DocumentName, string DocumentDir);

public record SearchConditionDto(DateTime? StartDate, DateTime? EndDate, short?[]? FdsdCds, short[] GakkaCds);

public record AttendItemDto(int UserId, string EmpName, short GakkaCd, string GakkaName, short Attend, short GakkaOrderNo, short UserOrderNo);

public record GakkaChangeDto(int Id, int UserId, string EmpName, short GakkaCd, string GakkaName, DateTime DateOfArrival, DateTime? DateOfDeparture);

public record LeaveOfAbsenceDto(int Id, int UserId, string EmpName, DateTime StartDate, DateTime? EndDate);

public record EmpKubunDto(short EmpKubun, string KubunName);

public record KenshuStyleDto(short Id, string Name, string Ryakusho, short Sort, string Biko);