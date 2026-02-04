using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NeoTrader.Controllers
{
	public class AgentController : Controller
	{
		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> Index()
		{
			return RedirectToAction("Symbols");
		}

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> Symbols() { 
			try
			{
				return View(new SymbolsViewModel() { simbolos = daNeoAgent.GetSymbols() });
			}
			catch (Exception err)
			{
				return BadRequest(err);
			}
		}

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> CaptureSymbols()
		{
			try
			{
				DataTable _max = daNeoAgent.GetRecords("SELECT CAST(max(DatePrice) as date) as DatePrice FROM dbo.mod_trader_data");
				DateTime _dateFrom = Convert.ToDateTime(_max.Rows[0]["DatePrice"].ToString()); // Captura a partir de la maxima fecha ya registrada
				//DateTime _dateFrom = new DateTime(1990, 1, 1);// Captura completa 
				await new cnNeoAgent().CaptureSymbols(daNeoAgent.GetRecords("SELECT * FROM dbo.mod_trader_symbols ORDER BY code ASC"), (neoContext.DayDiff(_dateFrom) * -1));

				return Ok(new neoResponse(true, "OK", null));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
		}

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> CaptureEvents()
		{
			try
			{
				await new cnNeoAgent().CaptureEvents(daNeoAgent.GetRecords("SELECT * FROM dbo.mod_trader_events ORDER BY id ASC"));

				return Ok(new neoResponse(true, "OK", null));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
		}

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> PredictiveData()
		{
			try
			{
				await new cnNeoAgent().PredictiveData(daNeoAgent.GetRecords("SELECT * FROM dbo.mod_trader_data ORDER BY DatePrice ASC"));

				return Ok(new neoResponse(true, "OK", null));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
		}
	}
}
