using System;

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

		public static List<SymbolsViewModelItems> GetSymbols() {
			return neoContext.ConvertDataTableToList<SymbolsViewModelItems>(GetRecords("SELECT * FROM dbo.mod_trader_symbols ORDER BY code ASC"));
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
							_PercentageMovementPreviousDay = neoContext.DiffPercetage(Convert.ToDouble(item.Close), Convert.ToDouble(record.Data.Prices[ndx - 1].Close));

							/*Si es divisible por 7 calcular % mov semana previa*/
							if (ndx % 7 == 0) { _PercentageMovementPreviousWeek = neoContext.DiffPercetage(Convert.ToDouble(item.Close), Convert.ToDouble(record.Data.Prices[(ndx - 7)].Close)); }

							/*Si es divisible por 30 calcular % mov mes previo*/
							if (ndx % 30 == 0)
							{ _PercentageMovementPreviousMonth = neoContext.DiffPercetage(Convert.ToDouble(item.Close), Convert.ToDouble(record.Data.Prices[(ndx - 30)].Close)); }
						}
						cmd.Parameters.AddWithValue("@PercentageMovementPreviousDay", _PercentageMovementPreviousDay);
						cmd.Parameters.AddWithValue("@PercentageMovementPreviousWeek", _PercentageMovementPreviousWeek);
						cmd.Parameters.AddWithValue("@PercentageMovementPreviousMonth", _PercentageMovementPreviousMonth);
						cmd.Parameters.AddWithValue("@SplitFactor", 0);
						_i = Convert.ToInt32(cmd.ExecuteScalar());
						ndx++;
					}
				}
				Consolidar();
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

		public static bool SavePredictiveData(DataTable data) {
			int _i = 0;
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				/*Entrenamiento del modelo*/
				Predictor _state = new Predictor();
				List<PredictorInputData> trainning = new List<PredictorInputData>();
				foreach (DataRow item in data.Rows)
				{
					trainning.Add(new PredictorInputData()
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
						ModificadorBaseGauss = 0,
						ModificadorMaterialGauss = 0,
						ModificadorFinalGauss = 0
					});
				}
				_state.Load(trainning);
				
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.mod_trader_data_predictive";
				int ndx = 0;
				foreach (DataRow item in data.Rows)
				{
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@id", item["id"].ToString());
					float _PredictNextDay = 0;
					float _PredictNextWeek = 0;
					float _PredictNextMonth = 0;
					PredictorInputData _dataToday = new PredictorInputData()
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
						/*Calcular los modificadores gauss especificos para el dia considerando el solapamiento de eventos.
						 * Se deberia hacer una funcion reutilizable para el calculo de esto squi y en cada evento general.
						 * REVISAR!!!!!!!!
						 */
						ModificadorBaseGauss = 0,
						ModificadorMaterialGauss = 0,
						ModificadorFinalGauss = 0
					};

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

					cmd.Parameters.AddWithValue("@PredictNextDay", _PredictNextDay);
					cmd.Parameters.AddWithValue("@PredictNextWeek", _PredictNextWeek);
					cmd.Parameters.AddWithValue("@PredictNextMonth", _PredictNextMonth);

					_i = Convert.ToInt32(cmd.ExecuteScalar());
					ndx++;
				}
				Consolidar();
				return true;
			}
		}
		public static void Consolidar() {
			List<SymbolsViewModelItems> _simbolos = GetSymbols();
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
	}
}
