using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace Template.Utility.Providers;

public static class ExcelProvider
{
    public static byte[] Create(List<List<object>> rows, List<string>? headers = null)
    {
        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add();

        var row = 1;
        if (headers != null && headers.Count > 0)
        {
            worksheet.Cell(row, 1).InsertData(headers, transpose: true);
            row++;
        }

        worksheet.Cell(row, 1).InsertData(rows);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    public static async Task<IEnumerable<IEnumerable<object>>> ReadAsync(IFormFile file, bool isContainHeaderRow)
    {
        using var stream = new MemoryStream();
        
        await file.CopyToAsync(stream);

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        var response = new List<List<object>>();

        var columnUsed = worksheet.ColumnsUsed().Count();

        foreach (var row in worksheet.RowsUsed().Skip(isContainHeaderRow ? 1 : 0))
        {
            var columns = new List<object>();

            for (var columnNumber = 1; columnNumber <= columnUsed; columnNumber++)
            {
                columns.Add(row.Cell(columnNumber).Value);
            }

            response.Add(columns);
        }
        
        return response;
    }
}