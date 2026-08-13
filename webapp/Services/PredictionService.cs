using StockPricePredictor.Models;

namespace StockPricePredictor.Services;

public sealed class PredictionService
{
    private readonly FeatureService _features;
    public PredictionService(FeatureService features) => _features = features;

    public PredictResponse Run(List<StockRow> rows)
    {
        var f = _features.Build(rows);
        if (f.Count < 40) throw new ArgumentException("Not enough usable rows after feature engineering.");

        int cut = (int)(f.Count * .8);
        var train = f.Take(cut).ToList();
        var test = f.Skip(cut).ToList();

        var model = new RidgePredictor();
        model.Fit(train.Select(x => x.X).ToList(), train.Select(x => x.Target).ToList());

        var actual = test.Select(x => x.Target).ToArray();
        var pred = test.Select(x => model.Predict(x.X)).ToArray();

        double mae = actual.Zip(pred, (a,p)=>Math.Abs(a-p)).Average();
        double rmse = Math.Sqrt(actual.Zip(pred,(a,p)=>(a-p)*(a-p)).Average());
        double mean = actual.Average();
        double ssTot = actual.Sum(a => Math.Pow(a-mean,2));
        double ssRes = actual.Zip(pred,(a,p)=>Math.Pow(a-p,2)).Sum();
        double r2 = ssTot == 0 ? 0 : 1 - ssRes/ssTot;
        double direction = test.Zip(pred,(x,p)=>Math.Sign(x.Target-x.Current)==Math.Sign(p-x.Current) ? 1 : 0).Average();

        // Retrain on all historical examples and predict the next close.
        model.Fit(f.Select(x=>x.X).ToList(), f.Select(x=>x.Target).ToList());
        var latest = BuildLatest(rows);
        double next = model.Predict(latest.X);

        return new PredictResponse {
            Rows = rows.Count,
            LatestClose = latest.Current,
            PredictedClose = next,
            Direction = next >= latest.Current ? "UP" : "DOWN",
            Mae = mae, Rmse = rmse, R2 = r2,
            DirectionalAccuracy = direction,
            Message = "Prediction generated from the uploaded historical OHLCV data."
        };
    }

    private (double[] X, double Target, double Current) BuildLatest(List<StockRow> rows)
    {
        var all = _features.Build(rows, true);
        return all.Last();
    }
}
