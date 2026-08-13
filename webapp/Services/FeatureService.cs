using StockPricePredictor.Models;

namespace StockPricePredictor.Services;

public sealed class FeatureService
{
    public static readonly string[] Names =
    {
        "ret1","ret5","ret10","sma5","sma20","vol5","vol20","range","volumeChange","rsi"
    };

    public List<(double[] X, double Target, double Current)> Build(List<StockRow> rows, bool includeLast = false)
    {
        var output = new List<(double[], double, double)>();
        var p = rows.Select(r => r.Close).ToArray();

        for (int i = 20; i < rows.Count - (includeLast ? 0 : 1); i++)
        {
            double Ret(int n) => p[i] / p[i - n] - 1.0;
            double Mean(int start, int end) => Enumerable.Range(start, end - start + 1).Select(k => p[k]).Average();
            double StdReturns(int n)
            {
                var a = Enumerable.Range(i - n + 1, n)
                    .Select(k => p[k] / p[k - 1] - 1.0).ToArray();
                var m = a.Average();
                return Math.Sqrt(a.Select(x => (x - m) * (x - m)).Average());
            }
            double Rsi()
            {
                var d = Enumerable.Range(Math.Max(1, i - 13), Math.Min(14, i))
                    .Select(k => p[k] - p[k - 1]).ToArray();
                double gain = d.Select(x => Math.Max(0, x)).Average();
                double loss = d.Select(x => Math.Max(0, -x)).Average();
                return loss == 0 ? 100 : 100 - 100 / (1 + gain / loss);
            }

            double volumeChange = rows[i - 1].Volume > 0
                ? rows[i].Volume / rows[i - 1].Volume - 1.0 : 0;

            var x = new[]
            {
                Ret(1), Ret(5), Ret(10),
                p[i] / Mean(i - 4, i) - 1,
                p[i] / Mean(i - 19, i) - 1,
                StdReturns(5), StdReturns(20),
                (rows[i].High - rows[i].Low) / rows[i].Close,
                volumeChange, Rsi()
            };

            double target = p[i + 1];
            if (x.All(double.IsFinite))
                output.Add((x, target, p[i]));
        }
        return output;
    }
}
