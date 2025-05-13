using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Http;

namespace Template.Utility.Providers;

public static class CsvProvider
{
    public static async Task<IEnumerable<IEnumerable<object>>> ReadCsvRawAsync(IFormFile file, bool isContainHeaderRow)
    {
        var rows = new List<IEnumerable<object>>();

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Optionally skip header
        if (isContainHeaderRow)
        {
            await csv.ReadAsync();
            csv.ReadHeader();
        }

        while (await csv.ReadAsync())
        {
            var row = new List<object>();

            for (var i = 0; i < csv.Parser.Count; i++)
            {
                var value = csv.GetField(i);
                row.Add(string.IsNullOrWhiteSpace(value) ? null! : value);
            }

            rows.Add(row);
        }

        return rows;
    }
}