using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fdsd.Application.Abstractions;

public interface IExcelReportWriter
{
    Task<string> CreateP001Async(string tempFilePath, string period,
        List<P001DepartmentData> departments, List<P001TrainingColumn> trainingColumns,
        string fontName = "Yu Gothic", double fontSize = 11);

    Task<string> CreateP002Async(string tempFilePath, string period,
        List<P002Row> rows,
        string fontName = "Yu Gothic", double fontSize = 11);

    Task<string> CreateP003Async(string templatePath, string tempFilePath,
        string period, List<P003Row> rows,
        string fontName = "Yu Gothic", double fontSize = 11);
}

public record P001DepartmentData(string DepartmentName, List<P001PersonRow> Persons);
public record P001PersonRow(string EmpName, List<string> AttendSymbols, string? Bikou);
public record P001TrainingColumn(string TrainingName, DateTime TrainingDate, string ShusaiName, string FdsdName, string StyleName);
public record P002Row(string EmpName, string FdsdName, string GakkaName);
public record P003Row(string FdsdName, DateTime TrainingDate, string TrainingName, string ShusaiName, string ShusaiDetail, string GakkaName, string? InfoDocu, string? DocumentNames, string? Place, int TotalAttendees, int StaffCount, int TeacherCount, int DispatchCount, int OtherCount);