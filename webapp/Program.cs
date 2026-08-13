var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<StockPricePredictor.Services.CsvService>();
builder.Services.AddSingleton<StockPricePredictor.Services.FeatureService>();
builder.Services.AddSingleton<StockPricePredictor.Services.PredictionService>();

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.Run();
