namespace StockPricePredictor.Models;

public sealed class PredictResponse
{
    public int Rows { get; set; }
    public double LatestClose { get; set; }
    public double PredictedClose { get; set; }
    public string Direction { get; set; } = "";
    public double Mae { get; set; }
    public double Rmse { get; set; }
    public double R2 { get; set; }
    public double DirectionalAccuracy { get; set; }
    public string Message { get; set; } = "";
}
