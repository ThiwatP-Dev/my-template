using ClosedXML.Excel;
using ExcelDataReader;
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

    public static async Task<Dictionary<string, IEnumerable<IEnumerable<object>>>> ReadAsync(IFormFile file, bool isContainHeaderRow)
    {
        var result = new Dictionary<string, IEnumerable<IEnumerable<object>>>();

        using var stream = file.OpenReadStream();
        using var reader = ExcelReaderFactory.CreateReader(stream);

        do
        {
            var sheetName = reader.Name;
            var sheetData = new List<IEnumerable<object>>();

            // Optionally skip header
            if (isContainHeaderRow)
            {
                reader.Read();
            }

            while (reader.Read())
            {
                var row = new List<object>();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.IsDBNull(i) ? null! : reader.GetValue(i));
                }

                sheetData.Add(row);
            }

            result[sheetName] = sheetData;

        } while (reader.NextResult());

        return await Task.FromResult(result);
    }
}