namespace NeoTrader.BLL
{
	public class cnNeoAgent
	{
		public async Task<List<AgentViewModelItem>> CaptureSymbols(int? _reset)
		{
			DataTable symbols = daNeoAgent.GetSymbols();
			DataTable _max = daNeoAgent.GetRecords("SELECT CAST(max(DatePrice) as date) as DatePrice FROM dbo.mod_trader_data");
			// Captura a partir de la maxima fecha ya registrada
			DateTime _dateFrom = Convert.ToDateTime(_max.Rows[0]["DatePrice"].ToString());
			// Captura completa 
			if (_reset.HasValue && _reset != 0) { _dateFrom = new DateTime(1990, 1, 1); }
			int iDaysFrom = (neoContext.DayDiff(_dateFrom) * -1);
			AgentViewModel _List = new AgentViewModel();
			YahooClient yahooFinanceClient = new YahooClient();
			try
			{
				foreach (DataRow symbol in symbols.Rows)
				{
					string _key = symbol["code"].ToString();
					HistoricalFullData data = await yahooFinanceClient.GetAllHistoricalDataAsync(_key, DataFrequency.Daily, DateTime.Today.AddDays(iDaysFrom));
					_List.Add(new AgentViewModelItem(_key, null, data));
				}
				return await daNeoAgent.SaveData(_List.simbolos);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task<List<EventViewModelItem>> CaptureEvents()
		{
			try
			{
				DataTable events = daNeoAgent.GetRecords("SELECT * FROM dbo.mod_trader_events ORDER BY id ASC");
				EventViewModel _List = new EventViewModel();
				foreach (DataRow item in events.Rows)
				{
					/*---------------------------------------------------------------------------------------------*/
					/*Calcular modificadores Gauss*/
					/*---------------------------------------------------------------------------------------------*/
					EventViewModelItem _eventItem = daNeoAgent.CalculateGaussEvent(item);
					/*---------------------------------------------------------------------------------------------*/
					_List.Add(_eventItem);
				}
				return await daNeoAgent.SaveEvents(_List.eventos);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task<bool> Consolidate()
		{
			try
			{
				return await daNeoAgent.Consolidate();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task<bool> Train()
		{
			try
			{
				await daNeoAgent.Train();
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task<neoResponse> Predict(int Id_symbol)
		{
			neoResponse _ret = new neoResponse(true, "", "");
			try
			{
				StringBuilder _html = new StringBuilder();
				if (Id_symbol == 0)
				{
					DataTable symbols = daNeoAgent.GetSymbols();
					foreach (DataRow reg in symbols.Rows)
					{
						await daNeoAgent.Predict(Convert.ToInt32(reg["id"].ToString()));
					}
					_html.Append("<h4>Se han realizado las predicciones del día para todos los símbolos activos</h4>");
					_html.Append("<h5>Vea la información en la columna 'Predicción'</h5>");
				}
				else {
					PredictorOutputDataFormatted _prediction = await daNeoAgent.Predict(Id_symbol);
					string _pre = "<span class='badge bg-danger'>BAJA</span>";
					if (_prediction.PredictedLabel) { _pre = "<span class='badge bg-info'>SUBE</span>"; }
					_html.Append($"<h5>Predicción solicitada <b>{_prediction.Symbol}</b></h5>");
					_html.Append("<table class='table table-sm table-condensed'>");
					_html.Append($"<tr><td>Predicción</td><td>{_pre}</td></tr>");
					_html.Append($"<tr><td>Probabilidad</td><td>{_prediction.Probability}</td></tr>");
					_html.Append($"<tr><td>Score</td><td>{_prediction.Score}</td></tr>");
					_html.Append($"<tr><td>Algoritmo</td><td>{_prediction.Algoritmo}</td></tr>");
					_html.Append("</table>");
				}
				_ret.Message= _html.ToString();
			}
			catch (Exception ex)
			{
				_ret.Error = ex.Message;
			}
			return _ret;
		}
	}
}
