using System.Globalization;
using StockPricePredictor.Models;

namespace StockPricePredictor.Services;

public sealed class CsvService
{
    public List<StockRow> Parse(string csv)
    {
        var lines = csv.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 3) throw new ArgumentException("CSV must contain a header and historical rows.");

        var headers = lines[0].Split(',').Select(Normalize).ToArray();
        int Find(params string[] names) => Array.FindIndex(headers, h => names.Contains(h));

        int date = Find("date", "datetime", "timestamp");
        int open = Find("open");
        int high = Find("high");
        int low = Find("low");
        int close = Find("close", "adjclose", "adjustedclose", "price", "last", "ltp");
        int volume = Find("volume", "vol", "shares");

        if (new[] { date, open, high, low, close }.Any(x => x < 0))
            throw new ArgumentException("Required columns: Date, Open, High, Low and Close (or a common price alias).");

        var result = new List<StockRow>();
        for (int i = 1; i < lines.Length; i++)
        {
            var c = lines[i].Split(',');
            try
            {
                if (c.Length <= Math.Max(close, Math.Max(low, high))) continue;
                result.Add(new StockRow
                {
                    Date = DateTime.Parse(c[date], CultureInfo.InvariantCulture),
                    Open = Num(c[open]),
                    High = Num(c[high]),
                    Low = Num(c[low]),
                    Close = Num(c[close]),
                    Volume = volume >= 0 ? Num(c[volume]) : 0
                });
            }
            catch { }
        }

        result = result.Where(x => x.Close > 0 && x.High > 0 && x.Low > 0)
                       .OrderBy(x => x.Date).ToList();

        if (result.Count < 60)
            throw new ArgumentException("At least 60 valid historical rows are recommended.");
        return result;
    }

    private static double Num(string value) =>
        double.Parse(value.Trim().Replace(",", ""), CultureInfo.InvariantCulture);

    private static string Normalize(string value) =>
        new string(value.Trim().ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
}
