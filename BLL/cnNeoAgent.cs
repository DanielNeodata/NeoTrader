using Microsoft.Extensions.Logging;
using NeoTrader.DAL;

namespace NeoTrader.BLL
{
	public class cnNeoAgent
	{
		public async Task<List<AgentViewModelItem>> CaptureSymbols(DataTable symbols, int iDaysFrom)
		{
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
		public async Task<List<EventViewModelItem>> CaptureEvents(DataTable events)
		{
			try
			{
				EventViewModel _List = new EventViewModel();
				foreach (DataRow item in events.Rows)
				{
					int? _id = Convert.ToInt32(item["id"].ToString());
					string? _code = item["code"].ToString();
					string? _description = item["description"].ToString();
					DateTime _FechaInicioGauss;
					DateTime _FechaPicoGauss;
					DateTime _FechaFinPicoGauss;
					DateTime _FechaFinGauss;

					double _ValorFechaInicioGauss = Convert.ToDouble(item["ValorFechaInicioGauss"].ToString()); 
					double _ValorFechaPicoGauss = Convert.ToDouble(item["ValorFechaPicoGauss"].ToString()); 
					double _ValorFechaFinPicoGauss = Convert.ToDouble(item["ValorFechaFinPicoGauss"].ToString()); 
					double _ValorFechaFinGauss = Convert.ToDouble(item["ValorFechaFinGauss"].ToString()); 

					DateTime.TryParse(item["FechaInicioGauss"].ToString(), out _FechaInicioGauss);
					DateTime.TryParse(item["FechaPicoGauss"].ToString(), out _FechaPicoGauss);
					DateTime.TryParse(item["FechaFinPicoGauss"].ToString(), out _FechaFinPicoGauss);
					if (!DateTime.TryParse(item["FechaFinGauss"].ToString(), out _FechaFinGauss)) { 
						_FechaFinGauss = DateTime.Today;
						TimeSpan _diferencia = (_FechaFinGauss - _FechaFinPicoGauss);
						double _dias = _diferencia.TotalDays;
						_ValorFechaFinGauss = (_ValorFechaFinPicoGauss / _dias);
					}

					/*---------------------------------------------------------------------------------------------*/
					/*Calcular modificadores Gauss*/
					/*---------------------------------------------------------------------------------------------*/
					List<PuntoCurva> puntos = new List<PuntoCurva>();
					puntos.Add(new PuntoCurva() { Fecha = _FechaInicioGauss, Valor = _ValorFechaInicioGauss });
					puntos.Add(new PuntoCurva() { Fecha = _FechaPicoGauss, Valor = _ValorFechaPicoGauss });
					puntos.Add(new PuntoCurva() { Fecha = _FechaFinPicoGauss, Valor = _ValorFechaFinPicoGauss });
					puntos.Add(new PuntoCurva() { Fecha = _FechaFinGauss, Valor = _ValorFechaFinGauss });
					double _ModificadorBaseGauss = neoContext.CalcularIntegral(puntos);
					/*---------------------------------------------------------------------------------------------*/

					/*---------------------------------------------------------------------------------------------*/
					/*Buscar eventos solapados con _ValorFechaPicoGauss y _ValorFechaFinGauss del evento analizado */
					/*---------------------------------------------------------------------------------------------*/
					string _sql = "SELECT * FROM dbo.mod_trader_events WHERE ";
					_sql += " (FechaInicioGauss<= '" + _FechaInicioGauss.ToString("yyyy-MM-dd") + "' AND FechaFinGauss <= '" + _FechaFinGauss.ToString("yyyy-MM-dd") + "') ";
					_sql += " OR ";
					_sql += " (FechaInicioGauss>= '" + _FechaInicioGauss.ToString("yyyy-MM-dd") + "' AND FechaFinGauss >= '" + _FechaFinGauss.ToString("yyyy-MM-dd") + "') ";
					DataTable _solapados = daNeoAgent.GetRecords(_sql);
					double _ModificadorMaterialGauss = 0;
					foreach (DataRow solapado in _solapados.Rows)
					{
						DateTime s_FechaInicioGauss;
						DateTime s_FechaPicoGauss;
						DateTime s_FechaFinPicoGauss;
						DateTime s_FechaFinGauss;

						double s_ValorFechaInicioGauss = Convert.ToDouble(solapado["ValorFechaInicioGauss"].ToString()); 
						double s_ValorFechaPicoGauss = Convert.ToDouble(solapado["ValorFechaPicoGauss"].ToString()); 
						double s_ValorFechaFinPicoGauss = Convert.ToDouble(solapado["ValorFechaFinPicoGauss"].ToString()); 
						double s_ValorFechaFinGauss = Convert.ToDouble(solapado["ValorFechaFinGauss"].ToString()); 

						DateTime.TryParse(solapado["FechaInicioGauss"].ToString(), out s_FechaInicioGauss);
						DateTime.TryParse(solapado["FechaPicoGauss"].ToString(), out s_FechaPicoGauss);
						DateTime.TryParse(solapado["FechaFinPicoGauss"].ToString(), out s_FechaFinPicoGauss);
						if (!DateTime.TryParse(solapado["FechaFinGauss"].ToString(), out s_FechaFinGauss))
						{
							s_FechaFinGauss = DateTime.Today;
							TimeSpan s_diferencia = (s_FechaFinGauss - s_FechaFinPicoGauss);
							double s_dias = s_diferencia.TotalDays;
							s_ValorFechaFinGauss = (s_ValorFechaFinPicoGauss / s_dias);
						}
						if (s_FechaPicoGauss > _FechaInicioGauss) { s_FechaInicioGauss = _FechaInicioGauss; }
						if (s_FechaFinPicoGauss < _FechaFinGauss) { s_FechaFinGauss = _FechaFinGauss; }

						List<PuntoCurva> s_puntos = new List<PuntoCurva>();
						s_puntos.Add(new PuntoCurva() { Fecha = s_FechaInicioGauss, Valor = s_ValorFechaInicioGauss });
						s_puntos.Add(new PuntoCurva() { Fecha = s_FechaPicoGauss, Valor = s_ValorFechaPicoGauss });
						s_puntos.Add(new PuntoCurva() { Fecha = s_FechaFinPicoGauss, Valor = s_ValorFechaFinPicoGauss });
						s_puntos.Add(new PuntoCurva() { Fecha = s_FechaFinGauss, Valor = s_ValorFechaFinGauss });
						_ModificadorMaterialGauss += neoContext.CalcularIntegral(s_puntos);
					}
					/*---------------------------------------------------------------------------------------------*/

					/*---------------------------------------------------------------------------------------------*/
					/*Modificar final consolidado*/
					/*---------------------------------------------------------------------------------------------*/
					double _ModificadorFinalGauss = (_ModificadorBaseGauss + _ModificadorMaterialGauss);
					/*---------------------------------------------------------------------------------------------*/

					_List.Add(new EventViewModelItem(
						_id, _code, _description,
						_FechaInicioGauss, _FechaPicoGauss, _FechaFinPicoGauss, _FechaFinGauss,
						_ValorFechaInicioGauss, _ValorFechaPicoGauss, _ValorFechaFinPicoGauss, _ValorFechaFinGauss,
						_ModificadorBaseGauss, _ModificadorMaterialGauss, _ModificadorFinalGauss));
				}
				return await daNeoAgent.SaveEvents(_List.eventos);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task<bool> PredictiveData(DataTable data)
		{
			try
			{
				return daNeoAgent.SavePredictiveData(data);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	}
}
