using Microsoft.AspNetCore.Mvc;
using StockPricePredictor.Models;
using StockPricePredictor.Services;

namespace StockPricePredictor.Controllers;

[ApiController]
[Route("api/predict")]
public sealed class PredictController : ControllerBase
{
    private readonly CsvService _csv;
    private readonly PredictionService _prediction;

    public PredictController(CsvService csv, PredictionService prediction)
    {
        _csv = csv; _prediction = prediction;
    }

    [HttpPost]
    public ActionResult<PredictResponse> Predict([FromBody] PredictRequest request)
    {
        try
        {
            var rows = _csv.Parse(request.Csv);
            return Ok(_prediction.Run(rows));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
