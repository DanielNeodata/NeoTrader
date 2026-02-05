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
		public async Task<IActionResult> All()
		{
			try
			{
				await new cnNeoAgent().CaptureSymbols();
				await new cnNeoAgent().CaptureEvents();
				await new cnNeoAgent().PredictiveData();

				return Ok(new neoResponse(true, "OK", null));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
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
				await new cnNeoAgent().CaptureSymbols();

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
				await new cnNeoAgent().CaptureEvents();

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
				await new cnNeoAgent().PredictiveData();

				return Ok(new neoResponse(true, "OK", null));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
		}
	}
}
