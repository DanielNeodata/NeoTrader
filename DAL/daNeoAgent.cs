using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NeoTrader.DAL
{
	public static class daNeoAgent
	{
		public static DataTable GetRecords(string _command) {
			DataTable dtResponse = new DataTable();
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = _command;
				dtResponse.Load(cmd.ExecuteReader());
			}
			return dtResponse;
		}
		public static DataTable GetSymbols()
		{
			return GetRecords("SELECT * FROM dbo.mod_trader_symbols WHERE [offline] is null ORDER BY code ASC");
		}
		public static async Task<List<AgentViewModelItem>> SaveData(List<AgentViewModelItem> records)
		{
			int _i = 0;
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.mod_trader_data_save";

				foreach (AgentViewModelItem record in records)
				{
					int ndx = 0;
					foreach (HistoricalChartInfo item in record.Data.Prices)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.AddWithValue("@Symbol", item.Meta.Symbol);
						cmd.Parameters.AddWithValue("@ShortName", item.Meta.ShortName);
						cmd.Parameters.AddWithValue("@InstrumentType", item.Meta.InstrumentType);
						cmd.Parameters.AddWithValue("@Currency", item.Meta.Currency);
						cmd.Parameters.AddWithValue("@DatePrice", item.Date);
						cmd.Parameters.AddWithValue("@Open", item.Open);
						cmd.Parameters.AddWithValue("@RegularMarketVolume", item.Meta.RegularMarketVolume);
						cmd.Parameters.AddWithValue("@Volume", item.Volume);
						cmd.Parameters.AddWithValue("@FiftyTwoWeekLow", item.Meta.FiftyTwoWeekLow);
						cmd.Parameters.AddWithValue("@FiftyTwoWeekHigh", item.Meta.FiftyTwoWeekHigh);
						cmd.Parameters.AddWithValue("@RegularMarketDayLow", item.Meta.RegularMarketDayLow);
						cmd.Parameters.AddWithValue("@RegularMarketDayHigh", item.Meta.RegularMarketDayHigh);
						cmd.Parameters.AddWithValue("@Low", item.Low);
						cmd.Parameters.AddWithValue("@High", item.High);
						cmd.Parameters.AddWithValue("@Close", item.Close);
						double _PercentageMovementPreviousDay = 0;
						double _PercentageMovementPreviousWeek = 0;
						double _PercentageMovementPreviousMonth = 0;
						if (ndx > 0)
						{
							/*Calcular % mov dia previo*/
							_PercentageMovementPreviousDay = neoContext.DiffPercentage(Convert.ToDouble(item.Close), Convert.ToDouble(record.Data.Prices[ndx - 1].Close));

							/*Si es divisible por 7 calcular % mov semana previa*/
							if (ndx > 7) { _PercentageMovementPreviousWeek = neoContext.DiffPercentage(Convert.ToDouble(item.Close), Convert.ToDouble(record.Data.Prices[(ndx - 7)].Close)); }

							/*Si es divisible por 30 calcular % mov mes previo*/
							if (ndx > 30)
							{ _PercentageMovementPreviousMonth = neoContext.DiffPercentage(Convert.ToDouble(item.Close), Convert.ToDouble(record.Data.Prices[(ndx - 30)].Close)); }
						}
						cmd.Parameters.AddWithValue("@PercentageMovementPreviousDay", _PercentageMovementPreviousDay);
						cmd.Parameters.AddWithValue("@PercentageMovementPreviousWeek", _PercentageMovementPreviousWeek);
						cmd.Parameters.AddWithValue("@PercentageMovementPreviousMonth", _PercentageMovementPreviousMonth);
						cmd.Parameters.AddWithValue("@SplitFactor", 0);
						_i = Convert.ToInt32(cmd.ExecuteScalar());
						ndx++;
					}
				}
				Consolidate();
				return records;
			}
		}
		public static async Task<List<EventViewModelItem>> SaveEvents(List<EventViewModelItem> records)
		{
			int _i = 0;
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.mod_trader_events_save";

				foreach (EventViewModelItem record in records)
				{
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@id", record.Id);
					cmd.Parameters.AddWithValue("@ModificadorBaseGauss", record.ModificadorBaseGauss);
					cmd.Parameters.AddWithValue("@ModificadorMaterialGauss", record.ModificadorMaterialGauss);
					cmd.Parameters.AddWithValue("@ModificadorFinalGauss", record.ModificadorFinalGauss);
					_i = Convert.ToInt32(cmd.ExecuteScalar());
				}

				return records;
			}
		}
		public static bool SavePredictiveData() {
			int _i = 0;
			DataTable data = daNeoAgent.GetRecords("SELECT * FROM dbo.mod_trader_data ORDER BY DatePrice ASC");
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				/*Entrenamiento del modelo*/
				Predictor _state = new Predictor();
				List<PredictorInputData> trainning = new List<PredictorInputData>();
				foreach (DataRow item in data.Rows) { trainning.Add(CalculateGaussDay(item)); }
				_state.Load(trainning);
				
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.mod_trader_data_predictive";
				int ndx = 0;
				foreach (DataRow item in data.Rows)
				{
					float _PredictNextDay = 0;
					float _PredictNextWeek = 0;
					float _PredictNextMonth = 0;

					PredictorInputData _dataToday = CalculateGaussDay(item);

					PredictorOutputData _prediction = _state.Predict(_dataToday);
					_PredictNextDay = 0;
					if (_prediction.Score < _dataToday.Close) { _PredictNextDay = -1; }
					if (_prediction.Score > _dataToday.Close) { _PredictNextDay = 1; }

					DateTime datePrice = Convert.ToDateTime(item["DatePrice"].ToString());

					_dataToday.DatePrice = float.Parse(datePrice.AddDays(7).ToString("yyyyMMdd"));
					PredictorOutputData _prediction7 = _state.Predict(_dataToday);
					_PredictNextWeek = 0;
					if (_prediction7.Score < _dataToday.Close) { _PredictNextWeek = -1; }
					if (_prediction7.Score > _dataToday.Close) { _PredictNextWeek = 1; }

					_dataToday.DatePrice = float.Parse(datePrice.AddMonths(1).ToString("yyyyMMdd"));
					PredictorOutputData _prediction30 = _state.Predict(_dataToday);
					_PredictNextMonth = 0;
					if (_prediction30.Score < _dataToday.Close) { _PredictNextMonth = -1; }
					if (_prediction30.Score > _dataToday.Close) { _PredictNextMonth = 1; }

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@id", item["id"].ToString());
					cmd.Parameters.AddWithValue("@PredictNextDay", _PredictNextDay);
					cmd.Parameters.AddWithValue("@PredictNextWeek", _PredictNextWeek);
					cmd.Parameters.AddWithValue("@PredictNextMonth", _PredictNextMonth);
					cmd.Parameters.AddWithValue("@ModificadorBaseGauss", _dataToday.ModificadorBaseGauss);
					cmd.Parameters.AddWithValue("@ModificadorMaterialGauss", _dataToday.ModificadorMaterialGauss);
					cmd.Parameters.AddWithValue("@ModificadorFinalGauss", _dataToday.ModificadorFinalGauss);

					_i = Convert.ToInt32(cmd.ExecuteScalar());
					ndx++;
				}
				return true;
			}
		}
		public static void Consolidate() {
			List<SymbolsViewModelItems> _simbolos = neoContext.ConvertDataTableToList<SymbolsViewModelItems>(GetSymbols());
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.mod_trader_symbols_consolida";
				foreach (SymbolsViewModelItems record in _simbolos)
				{
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@Symbol", record.code);
					cmd.ExecuteScalar();
				}
			}
		}
		public static PredictorInputData CalculateGaussDay(DataRow item)
		{
			int i = 0;
			float ModificadorBaseGauss = 0;
			float ModificadorMaterialGauss = 0;
			float ModificadorFinalGauss = 0;

			DateTime _today = DateTime.Today;

			string _sql = "SELECT * FROM dbo.mod_trader_events WHERE ";
			_sql += " (CAST(GETDATE() AS DATE) BETWEEN cast(FechaInicioGauss as date) AND isnull(FechaFinGauss,cast(getdate() as date))) ";
			DataTable _solapados = daNeoAgent.GetRecords(_sql);
			foreach (DataRow solapado in _solapados.Rows)
			{
				i++;
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
				if (!DateTime.TryParse(solapado["FechaFinGauss"].ToString(), out s_FechaFinGauss)) { s_FechaFinGauss = DateTime.Today; }

				if (_today >= s_FechaInicioGauss) { 
					s_FechaInicioGauss = _today;
					s_ValorFechaInicioGauss = (s_ValorFechaInicioGauss / neoContext.DayDiff(s_FechaInicioGauss));
				}
				if (_today >= s_FechaPicoGauss) {
					s_FechaPicoGauss = _today;
					s_ValorFechaPicoGauss = (s_ValorFechaPicoGauss / neoContext.DayDiff(s_FechaPicoGauss));
				}
				if (_today >= s_FechaFinPicoGauss) { 
					s_FechaFinPicoGauss = _today;
					s_ValorFechaPicoGauss = (s_ValorFechaPicoGauss / neoContext.DayDiff(s_FechaPicoGauss));
				}
				if (_today >= s_FechaFinGauss)
				{
					s_ValorFechaFinGauss = (s_ValorFechaFinGauss / neoContext.DayDiff(s_FechaFinGauss));
				}

				List<PuntoCurva> s_puntos = new List<PuntoCurva>();
				s_puntos.Add(new PuntoCurva() { Fecha = s_FechaInicioGauss, Valor = s_ValorFechaInicioGauss });
				s_puntos.Add(new PuntoCurva() { Fecha = s_FechaPicoGauss, Valor = s_ValorFechaPicoGauss });
				s_puntos.Add(new PuntoCurva() { Fecha = s_FechaFinPicoGauss, Valor = s_ValorFechaFinPicoGauss });
				s_puntos.Add(new PuntoCurva() { Fecha = s_FechaFinGauss, Valor = s_ValorFechaFinGauss });
				ModificadorMaterialGauss += float.Parse(neoContext.CalcularIntegral(s_puntos).ToString());
			}
			ModificadorBaseGauss = (ModificadorMaterialGauss / i);
			ModificadorFinalGauss = (ModificadorBaseGauss + ModificadorMaterialGauss);
			return new PredictorInputData()
			{
				id_symbol = float.Parse(item["id"].ToString()),
				DatePrice = float.Parse(Convert.ToDateTime(item["DatePrice"].ToString()).ToString("yyyyMMdd")),
				Open = float.Parse(item["Open"].ToString()),
				RegularMarketVolume = float.Parse(item["RegularMarketVolume"].ToString()),
				Volume = float.Parse(item["Volume"].ToString()),
				FiftyTwoWeekLow = float.Parse(item["FiftyTwoWeekLow"].ToString()),
				FiftyTwoWeekHigh = float.Parse(item["FiftyTwoWeekHigh"].ToString()),
				RegularMarketDayLow = float.Parse(item["RegularMarketDayLow"].ToString()),
				RegularMarketDayHigh = float.Parse(item["RegularMarketDayHigh"].ToString()),
				Low = float.Parse(item["Low"].ToString()),
				High = float.Parse(item["High"].ToString()),
				Close = float.Parse(item["Close"].ToString()),
				PercentageMovementPreviousDay = float.Parse(item["PercentageMovementPreviousDay"].ToString()),
				PercentageMovementPreviousWeek = float.Parse(item["PercentageMovementPreviousWeek"].ToString()),
				PercentageMovementPreviousMonth = float.Parse(item["PercentageMovementPreviousMonth"].ToString()),
				ModificadorBaseGauss = ModificadorBaseGauss,
				ModificadorMaterialGauss = ModificadorMaterialGauss,
				ModificadorFinalGauss = ModificadorFinalGauss
			};

		}
		public static EventViewModelItem CalculateGaussEvent(DataRow item)
		{
			double[] _valsGauss = [0, 0, 0];

			DateTime _FechaInicioGauss;
			DateTime _FechaPicoGauss;
			DateTime _FechaFinPicoGauss;
			DateTime _FechaFinGauss;

			int? _id = Convert.ToInt32(item["id"].ToString());
			string? _code = item["code"].ToString();
			string? _description = item["description"].ToString();

			double _ValorFechaInicioGauss = Convert.ToDouble(item["ValorFechaInicioGauss"].ToString());
			double _ValorFechaPicoGauss = Convert.ToDouble(item["ValorFechaPicoGauss"].ToString());
			double _ValorFechaFinPicoGauss = Convert.ToDouble(item["ValorFechaFinPicoGauss"].ToString());
			double _ValorFechaFinGauss = Convert.ToDouble(item["ValorFechaFinGauss"].ToString());

			DateTime.TryParse(item["FechaInicioGauss"].ToString(), out _FechaInicioGauss);
			DateTime.TryParse(item["FechaPicoGauss"].ToString(), out _FechaPicoGauss);
			DateTime.TryParse(item["FechaFinPicoGauss"].ToString(), out _FechaFinPicoGauss);
			if (!DateTime.TryParse(item["FechaFinGauss"].ToString(), out _FechaFinGauss))
			{
				_FechaFinGauss = DateTime.Today;
				TimeSpan _diferencia = (_FechaFinGauss - _FechaFinPicoGauss);
				double _dias = _diferencia.TotalDays;
				_ValorFechaFinGauss = (_ValorFechaFinPicoGauss / _dias);
			}
			List<PuntoCurva> puntos = new List<PuntoCurva>();
			puntos.Add(new PuntoCurva() { Fecha = _FechaInicioGauss, Valor = _ValorFechaInicioGauss });
			puntos.Add(new PuntoCurva() { Fecha = _FechaPicoGauss, Valor = _ValorFechaPicoGauss });
			puntos.Add(new PuntoCurva() { Fecha = _FechaFinPicoGauss, Valor = _ValorFechaFinPicoGauss });
			puntos.Add(new PuntoCurva() { Fecha = _FechaFinGauss, Valor = _ValorFechaFinGauss });

			_valsGauss[0] = neoContext.CalcularIntegral(puntos);

			/*---------------------------------------------------------------------------------------------*/
			/*Buscar eventos solapados con _ValorFechaPicoGauss y _ValorFechaFinGauss del evento analizado */
			/*---------------------------------------------------------------------------------------------*/
			string _sql = "SELECT * FROM dbo.mod_trader_events WHERE ";
			_sql += " (FechaInicioGauss<= '" + _FechaInicioGauss.ToString("yyyy-MM-dd") + "' AND FechaFinGauss <= '" + _FechaFinGauss.ToString("yyyy-MM-dd") + "') ";
			_sql += " OR ";
			_sql += " (FechaInicioGauss>= '" + _FechaInicioGauss.ToString("yyyy-MM-dd") + "' AND FechaFinGauss >= '" + _FechaFinGauss.ToString("yyyy-MM-dd") + "') ";
			DataTable _solapados = daNeoAgent.GetRecords(_sql);
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
				_valsGauss[1] += neoContext.CalcularIntegral(s_puntos);
			}
			/*---------------------------------------------------------------------------------------------*/

			/*---------------------------------------------------------------------------------------------*/
			/*Modificar final consolidado*/
			/*---------------------------------------------------------------------------------------------*/
			_valsGauss[2] = (_valsGauss[0] + _valsGauss[1]);
			/*---------------------------------------------------------------------------------------------*/

			return new EventViewModelItem(
						_id, _code, _description,
						_FechaInicioGauss, _FechaPicoGauss, _FechaFinPicoGauss, _FechaFinGauss,
						_ValorFechaInicioGauss, _ValorFechaPicoGauss, _ValorFechaFinPicoGauss, _ValorFechaFinGauss,
						_valsGauss[0], _valsGauss[1], _valsGauss[2]);
		}
	}
}
