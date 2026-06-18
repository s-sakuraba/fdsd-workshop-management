using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Fdsd.Application.Abstractions;

namespace Fdsd.Infrastructure.Excel;

public class ExcelReportWriter : IExcelReportWriter
{
    public async Task<string> CreateP001Async(
        string tempFilePath,
        string period,
        List<P001DepartmentData> departments,
        List<P001TrainingColumn> trainingColumns,
        string fontName = "Yu Gothic",
        double fontSize = 11)
    {
        var workbook = new XLWorkbook();

        foreach (var dept in departments)
        {
            var ws = workbook.Worksheets.Add(dept.DepartmentName);
            ws.ColumnWidth = 20.62;
            var font = ws.Style.Font;
            font.FontName = fontName;
            font.FontSize = fontSize;

            int row = 1;
            ws.Cell(row, 1).Value = "研修会出席状況一覧表";
            row++;
            ws.Cell(row, 1).Value = dept.DepartmentName;
            row++;
            ws.Cell(row, 1).Value = period;
            row++;

            int col = 2;
            foreach (var tc in trainingColumns)
            {
                ws.Cell(row, col).Value = tc.TrainingName;
                ws.Cell(row + 1, col).Value = tc.ShusaiName;
                ws.Cell(row + 2, col).Value = tc.FdsdName;
                ws.Cell(row + 3, col).Value = tc.StyleName;
                ws.Cell(row + 4, col).Value = tc.TrainingDate.ToString("yyyy/MM/dd");
                col++;
            }
            ws.Cell(row, col).Value = "備考";
            ws.Cell(row + 1, col).Value = "";
            ws.Cell(row + 2, col).Value = "";
            ws.Cell(row + 3, col).Value = "";
            ws.Cell(row + 4, col).Value = "";

            int dataRow = row + 5;
            foreach (var person in dept.Persons)
            {
                int c = 1;
                ws.Cell(dataRow, c).Value = person.EmpName;
                c++;
                foreach (var symbol in person.AttendSymbols)
                {
                    ws.Cell(dataRow, c).Value = symbol;
                    c++;
                }
                ws.Cell(dataRow, c).Value = person.Bikou ?? "";
                dataRow++;
            }
        }

        await Task.Run(() => workbook.SaveAs(tempFilePath));
        return tempFilePath;
    }

    /// <summary>
    /// Non-attendee report (P002): Single sheet listing users with 0 attendance.
    /// </summary>
    public async Task<string> CreateP002Async(
        string tempFilePath,
        string period,
        List<P002Row> rows,
        string fontName = "Yu Gothic",
        double fontSize = 11)
    {
        var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("研修会未受講者");
        var font = ws.Style.Font;
        font.FontName = fontName;
        font.FontSize = fontSize;

        int row = 1;
        ws.Cell(row, 1).Value = "研修会未受講者";
        row++;
        ws.Cell(row, 1).Value = period;
        row++;
        ws.Cell(row, 1).Value = "氏名";
        ws.Cell(row, 2).Value = "FD・SD区分";
        ws.Cell(row, 3).Value = "学科";
        row++;

        foreach (var r in rows)
        {
            ws.Cell(row, 1).Value = r.EmpName;
            ws.Cell(row, 2).Value = r.FdsdName;
            ws.Cell(row, 3).Value = r.GakkaName;
            row++;
        }

        await Task.Run(() => workbook.SaveAs(tempFilePath));
        return tempFilePath;
    }

    /// <summary>
    /// Training achievement report (P003): Template-based.
    /// </summary>
    public async Task<string> CreateP003Async(
        string templatePath,
        string tempFilePath,
        string period,
        List<P003Row> rows,
        string fontName = "Yu Gothic",
        double fontSize = 11)
    {
        var workbook = new XLWorkbook(templatePath);
        var ws = workbook.Worksheet(1);

        ws.Cell(2, 1).Value = period;

        int dataRow = 4;
        foreach (var r in rows)
        {
            ws.Cell(dataRow, 1).Value = r.FdsdName;
            ws.Cell(dataRow, 2).Value = r.TrainingDate.ToString("yyyy/MM/dd");
            ws.Cell(dataRow, 3).Value = r.TrainingName;
            ws.Cell(dataRow, 4).Value = r.ShusaiName;
            ws.Cell(dataRow, 5).Value = r.ShusaiDetail;
            ws.Cell(dataRow, 6).Value = r.GakkaName;
            ws.Cell(dataRow, 7).Value = r.InfoDocu;
            ws.Cell(dataRow, 8).Value = r.DocumentNames;
            ws.Cell(dataRow, 9).Value = r.Place;
            ws.Cell(dataRow, 10).Value = r.TotalAttendees;
            ws.Cell(dataRow, 11).Value = r.StaffCount;
            ws.Cell(dataRow, 12).Value = r.TeacherCount;
            ws.Cell(dataRow, 13).Value = r.DispatchCount;
            ws.Cell(dataRow, 14).Value = r.OtherCount;
            dataRow++;
        }

        await Task.Run(() => workbook.SaveAs(tempFilePath));
        return tempFilePath;
    }
}