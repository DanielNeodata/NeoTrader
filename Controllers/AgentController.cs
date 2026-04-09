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
		public async Task<IActionResult> Symbols()
		{
			try
			{
				return View(new SymbolsViewModel() { simbolos = neoContext.ConvertDataTableToList<SymbolsViewModelItems>(daNeoAgent.GetSymbols()) });
			}
			catch (Exception err)
			{
				return BadRequest(err);
			}
		}

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> CaptureSymbols(int? _reset)
		{
			try
			{
				await new cnNeoAgent().CaptureSymbols(_reset);

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
		public async Task<IActionResult> Consolidate()
		{
			try
			{
				await new cnNeoAgent().Consolidate();

				return Ok(new neoResponse(true, "OK", null));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
		}
		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> Train()
		{
			try
			{
				await new cnNeoAgent().Train();

				return Ok(new neoResponse(true, "OK", null));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
		}
		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> Predict(int Id_symbol)
		{
			try
			{
				return Ok(await new cnNeoAgent().Predict(Id_symbol));
			}
			catch (Exception err)
			{
				return BadRequest(new neoResponse(false, "ERR", err.Message));
			}
		}
	}
}
