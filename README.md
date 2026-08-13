# Stock Price Prediction

Predict short-term stock price direction using historical OHLCV data.

**Skills:** Time Series, pandas, scikit-learn, Jupyter, C#, ASP.NET Core, HTML, CSS, JavaScript.

## Dataset

The project is designed for the Reliance Industries OHLCV dataset. The expected columns are:

`Date, Open, High, Low, Close, Volume`

There is no separate `Price` column requirement. **Close is the stock price used for prediction.**

## ML workflow

1. Load Reliance OHLCV data with pandas.
2. Clean and sort chronologically.
3. Engineer returns, moving-average ratios, volatility, RSI, range and volume-change features.
4. Create a next-trading-day `Close` target and an UP/DOWN direction target.
5. Use a chronological 80/20 train-test split to avoid time-series leakage.
6. Standardize features and train a scikit-learn Ridge Regression model.
7. Evaluate MAE, RMSE, R² and directional accuracy.
8. Save model artifacts with pickle/joblib.
9. Run the C# ASP.NET Core web app to upload a Reliance CSV and generate a next-close estimate and direction.

## Files

```text
Stock-Price-Prediction/
├── Stock_Price_Prediction.ipynb
├── README.md
├── requirements.txt
├── .gitignore
├── data/
│   ├── .gitkeep
│   └── sample_format.csv
├── artifacts/
│   └── .gitkeep
└── webapp/
    ├── StockPricePredictor.csproj
    ├── Program.cs
    ├── Controllers/PredictController.cs
    ├── Models/PredictRequest.cs
    ├── Models/PredictResponse.cs
    ├── Models/StockRow.cs
    ├── Services/CsvService.cs
    ├── Services/FeatureService.cs
    ├── Services/PredictionService.cs
    ├── Services/RidgePredictor.cs
    └── wwwroot/
        ├── index.html
        ├── style.css
        └── app.js
```

## Run the notebook

```bash
pip install -r requirements.txt
jupyter notebook Stock_Price_Prediction.ipynb
```

Put your Reliance CSV in `data/` before running it. The notebook saves trained artifacts into `artifacts/`.

## Run the C# web app

Install .NET 8 SDK, then:

```bash
cd webapp
dotnet run
```

Open the localhost URL printed by ASP.NET Core. Upload the same OHLCV CSV and click **Train & Predict**.

The C# application performs its own lightweight Ridge Regression calculation from the uploaded historical data, so the demo works without a Python server.

## Important

This is an educational machine-learning project. Stock predictions are estimates and are not guaranteed or financial advice.

## Author

**Praveen Kumar**

- GitHub: https://github.com/InfinitePraveen
- LinkedIn: https://www.linkedin.com/in/infinitepraveen/
