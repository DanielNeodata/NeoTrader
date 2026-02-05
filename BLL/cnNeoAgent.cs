using Microsoft.Extensions.Logging;
using NeoTrader.DAL;

namespace NeoTrader.BLL
{
	public class cnNeoAgent
	{
		public async Task<List<AgentViewModelItem>> CaptureSymbols()
		{
			DataTable symbols = daNeoAgent.GetRecords("SELECT * FROM dbo.mod_trader_symbols ORDER BY code ASC");
			DataTable _max = daNeoAgent.GetRecords("SELECT CAST(max(DatePrice) as date) as DatePrice FROM dbo.mod_trader_data");
			//DateTime _dateFrom = Convert.ToDateTime(_max.Rows[0]["DatePrice"].ToString()); // Captura a partir de la maxima fecha ya registrada
			DateTime _dateFrom = new DateTime(1990, 1, 1);// Captura completa 
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
		public async Task<bool> PredictiveData()
		{
			try
			{
				return daNeoAgent.SavePredictiveData();
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
				daNeoAgent.Consolidate();
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	}
}
