using System.Reflection;
using static Microsoft.ML.DataOperationsCatalog;
using NeoConsole.Classes;

namespace NeoTrader.DAL
{
	public static class daNeoAgent
	{
		public static DataTable GetRecords(string _command)
		{
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
		public static DataTable GetModels()
		{
			return GetRecords("SELECT * FROM dbo.mod_trader_training_models WHERE [offline] is null ORDER BY code ASC");
		}
		public static DataTable GetSymbols()
		{
			return GetRecords("SELECT * FROM dbo.mod_trader_symbols WHERE [offline] is null ORDER BY code ASC, DatePrice DESC");
		}
		public static DataTable GetSymbol(int id)
		{
			return GetRecords("SELECT * FROM dbo.mod_trader_symbols WHERE id=" + id.ToString() + " AND [offline] is null");
		}
		public static bool TrainingModelSave(string? _code, CalibratedBinaryClassificationMetrics _m)
		{
			try
			{
				int _i = 0;
				using (SqlConnection connection = new SqlConnection(neoContext.connString))
				{
					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.mod_trader_training_models_Save";

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@code", _code);
					cmd.Parameters.AddWithValue("@Accuracy", _m.Accuracy.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@AreaUnderPrecisionRecallCurve", _m.AreaUnderPrecisionRecallCurve.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@AreaUnderRocCurve", _m.AreaUnderRocCurve.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@NumberOfClasses", _m.ConfusionMatrix.NumberOfClasses.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@Entropy", _m.Entropy.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@F1Score", _m.F1Score.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@LogLoss", _m.LogLoss.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@LogLossReduction", _m.LogLossReduction.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@NegativePrecision", _m.NegativePrecision.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@NegativeRecall", _m.NegativeRecall.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@PositivePrecision", _m.PositivePrecision.ToString("F99").Substring(0, 12));
					cmd.Parameters.AddWithValue("@PositiveRecall", _m.PositiveRecall.ToString("F99").Substring(0, 12));

					_i = Convert.ToInt32(cmd.ExecuteScalar());
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		public static bool SavePrediction(int Id_symbol, bool Prediction)
		{
			int _iPredict = 0;
			if (Prediction) { _iPredict = 1; }
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "UPDATE dbo.mod_trader_symbols SET Predict=" + _iPredict.ToString() + " WHERE id=" + Id_symbol.ToString();
				cmd.ExecuteNonQuery();
			}
			return true;
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
						_i = Convert.ToInt32(cmd.ExecuteScalar());
						ndx++;
					}
				}
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

				if (_today >= s_FechaInicioGauss)
				{
					s_FechaInicioGauss = _today;
					s_ValorFechaInicioGauss = (s_ValorFechaInicioGauss / neoContext.DayDiff(s_FechaInicioGauss));
				}
				if (_today >= s_FechaPicoGauss)
				{
					s_FechaPicoGauss = _today;
					s_ValorFechaPicoGauss = (s_ValorFechaPicoGauss / neoContext.DayDiff(s_FechaPicoGauss));
				}
				if (_today >= s_FechaFinPicoGauss)
				{
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
			/*
			return new PredictorInputData()
			{
				DatePrice = float.Parse(Convert.ToDateTime(item["DatePrice"].ToString()).ToString("yyyyMMdd")),
				Open = float.Parse(item["Open"].ToString()),
				Volume = float.Parse(item["Volume"].ToString()),
				Close = float.Parse(item["Close"].ToString()),
				PercentageMovementPreviousDay = float.Parse(item["PercentageMovementPreviousDay"].ToString()),
				PercentageMovementPreviousWeek = float.Parse(item["PercentageMovementPreviousWeek"].ToString()),
				PercentageMovementPreviousMonth = float.Parse(item["PercentageMovementPreviousMonth"].ToString()),
				ModificadorBaseGauss = ModificadorBaseGauss,
				ModificadorMaterialGauss = ModificadorMaterialGauss,
				ModificadorFinalGauss = ModificadorFinalGauss
			};
			*/

			return new PredictorInputData()
			{
				DatePrice = float.Parse(Convert.ToDateTime(item["DatePrice"].ToString()).ToString("yyyyMMdd")),
				Ret1D = float.Parse(item["Ret1D"].ToString()),
				Ret5D = float.Parse(item["Ret5D"].ToString()),
				Ret10D = float.Parse(item["Ret10D"].ToString()),
				Ret20D = float.Parse(item["Ret20D"].ToString()),
				PriceSma10 = float.Parse(item["PriceSma10"].ToString()),
				PriceSma20 = float.Parse(item["PriceSma20"].ToString()),
				PriceSma50 = float.Parse(item["PriceSma50"].ToString()),
				Volatility10 = float.Parse(item["Volatility10"].ToString()),
				Volatility20 = float.Parse(item["Volatility20"].ToString()),
				VolumeRatio = float.Parse(item["VolumeRatio"].ToString()),
				Rsi = float.Parse(item["Rsi"].ToString()),
				BbPosition = float.Parse(item["BbPosition"].ToString()),
				DistMax = float.Parse(item["DistMax"].ToString()),
				DistMin = float.Parse(item["DistMin"].ToString()),
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

		public static async Task<bool> Consolidate()
		{
			int _i = 0;
			DataTable symbols = daNeoAgent.GetSymbols();

			/*Realizar las acciones de recalculo de indicadores precisos*/
			foreach (DataRow symbol in symbols.Rows)
			{
				string _sql = "SELECT * FROM dbo.mod_trader_data WHERE id_symbol=" + symbol["id"].ToString() + " AND Ret1D is null ORDER BY DatePrice ASC";
				DataTable _solapados = daNeoAgent.GetRecords(_sql);
				List<StockDB> Originales = new List<StockDB>();
				foreach (DataRow solapado in _solapados.Rows)
				{
					try
					{
						Originales.Add(new StockDB()
						{
							id_data = Convert.ToInt32(solapado["id"].ToString()),
							id_symbol = Convert.ToInt32(solapado["id_symbol"].ToString()),
							DatePrice = Convert.ToDateTime(solapado["DatePrice"].ToString()),
							Open = float.Parse(solapado["Open"].ToString()),
							RegularMarketVolume = float.Parse(solapado["RegularMarketVolume"].ToString()),
							Volume = float.Parse(solapado["Volume"].ToString()),
							FiftyTwoWeekLow = float.Parse(solapado["FiftyTwoWeekLow"].ToString()),
							FiftyTwoWeekHigh = float.Parse(solapado["FiftyTwoWeekHigh"].ToString()),
							RegularMarketDayLow = float.Parse(solapado["RegularMarketDayLow"].ToString()),
							RegularMarketDayHigh = float.Parse(solapado["RegularMarketDayHigh"].ToString()),
							Low = float.Parse(solapado["Low"].ToString()),
							High = float.Parse(solapado["High"].ToString()),
							Close = float.Parse(solapado["Close"].ToString()),
							PercentageMovementPreviousDay = float.Parse(solapado["PercentageMovementPreviousDay"].ToString()),
							PercentageMovementPreviousWeek = float.Parse(solapado["PercentageMovementPreviousWeek"].ToString()),
							PercentageMovementPreviousMonth = float.Parse(solapado["PercentageMovementPreviousMonth"].ToString())
						});
					}
					catch (Exception err) {
						int p = 0;
					}
				}
				UpdataData(TransformaDatos.Transforma(Originales));
			}

			/*Desnormalizacion de valores en cabecera de symbols*/
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.mod_trader_data_consolidate";
				foreach (DataRow symbol in symbols.Rows)
				{
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@Symbol", symbol["code"].ToString());
					_i = Convert.ToInt32(cmd.ExecuteScalar());
				}
				return true;
			}
		}

		public static bool UpdataData(List<StockData> Transformados) {
			int _i = 0;
			using (SqlConnection connection = new SqlConnection(neoContext.connString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.StoredProcedure;

				foreach (StockData transformado in Transformados)
				{
					cmd.CommandText = "dbo.mod_trader_data_update";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@id", transformado.id_data);
					cmd.Parameters.AddWithValue("Ret1D", transformado.Ret1D);
					cmd.Parameters.AddWithValue("Ret5D", transformado.Ret5D);
					cmd.Parameters.AddWithValue("Ret10D", transformado.Ret10D);
					cmd.Parameters.AddWithValue("Ret20D", transformado.Ret20D);
					cmd.Parameters.AddWithValue("PriceSma10", transformado.PriceSma10);
					cmd.Parameters.AddWithValue("PriceSma20", transformado.PriceSma20);
					cmd.Parameters.AddWithValue("PriceSma50", transformado.PriceSma50);
					cmd.Parameters.AddWithValue("Volatility10", transformado.Volatility10);
					cmd.Parameters.AddWithValue("Volatility20", transformado.Volatility20);
					cmd.Parameters.AddWithValue("VolumeRatio", transformado.VolumeRatio);
					cmd.Parameters.AddWithValue("Rsi", transformado.Rsi);
					cmd.Parameters.AddWithValue("BbPosition", transformado.BbPosition);
					cmd.Parameters.AddWithValue("DistMax", transformado.DistMax);
					cmd.Parameters.AddWithValue("DistMin", transformado.DistMin);
					_i = Convert.ToInt32(cmd.ExecuteScalar());
				}
			}
			return true;
		}
		public static async Task<bool> Train()
		{
			try
			{
				MLContext mlContext = new MLContext();
				List<PredictorInputData> _items = new List<PredictorInputData>();

				DataTable models = GetModels();
				DataTable symbols = GetSymbols();
				foreach (DataRow symbol in symbols.Rows)
				{
					MLOptions _opt = new MLOptions();
					DataTable datos = new DataTable();
					int _id_symbol = Convert.ToInt32(symbol["id"].ToString());
					string[] _fields = _opt.featureColumns[0].Split(",");
					using (SqlConnection connection = new SqlConnection(neoContext.connString))
					{
						connection.Open();
						SqlCommand cmd = new SqlCommand();
						cmd.Connection = connection;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "EXEC dbo.GetRows_mod_trader_data " + _id_symbol.ToString();
						//cmd.Parameters.Clear();
						//cmd.Parameters.AddWithValue("@Id_symbol", _id_symbol);
						datos.Load(cmd.ExecuteReader());
						try
						{
							foreach (DataRow item in datos.Rows)
							{
								PredictorInputData _line = new PredictorInputData();
								foreach (string s in _fields)
								{
									try
									{
										System.Type _lineType = typeof(PredictorInputData);
										PropertyInfo _property = _lineType.GetProperty(s);
										if (_property.PropertyType == typeof(float))
										{
											string _val = item[s].ToString();
											if (_val == "") { _val = "0"; }
											_property.SetValue(_line, float.Parse(_val), null);
										}
										else if (_property.PropertyType == typeof(string))
										{
											_property.SetValue(_line, item[s].ToString(), null);
										}
									}
									catch (Exception rr)
									{
										int x = 0;
									}
								}
								_line.Label = (Convert.ToInt32(item["resultado"].ToString()) == 1);
								_items.Add(_line);
							}
						}
						catch (Exception rex)
						{
							int _x = 0;
						}
						IDataView trainingData = mlContext.Data.LoadFromEnumerable(_items); //
						TrainTestData trainTestSplit = mlContext.Data.TrainTestSplit(trainingData, testFraction: 0.2);

						var inspeccionarTest = mlContext.Data.CreateEnumerable<PredictorInputData>(trainTestSplit.TestSet, reuseRowObject: false);
						int positivosTest = inspeccionarTest.Count(x => x.Label == true);
						int negativosTest = inspeccionarTest.Count(x => x.Label == false);

						var inspeccionarTrain = mlContext.Data.CreateEnumerable<PredictorInputData>(trainTestSplit.TrainSet, reuseRowObject: false);
						int positivosTrain = inspeccionarTrain.Count(x => x.Label == true);
						int negativosTrain = inspeccionarTrain.Count(x => x.Label == false);

						IEstimator<ITransformer> _concat = mlContext.Transforms.Concatenate("Features", _fields);
						IEstimator<ITransformer> _trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression();//Default

						foreach (DataRow reg in models.Rows)
						{
							bool _abort = false;
							_opt.modelTraining = reg["code"].ToString();
							string modelPath = ("./Recursos/IA/" + _opt.modelTraining + "-" + _id_symbol.ToString() + ".zip");
							switch (_opt.modelTraining)
							{
								case "SdcaLogisticRegression":
									_trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(_opt._sdca_LR_Options);
									break;
								case "SdcaNonCalibrated":
									_trainer = mlContext.BinaryClassification.Trainers.SdcaNonCalibrated(_opt._sdca_NC_Options);
									break;
								case "SgdCalibrated":
									_trainer = mlContext.BinaryClassification.Trainers.SgdCalibrated(_opt._sgd_CT_Options);
									break;
								case "SgdNonCalibrated":
									_trainer = mlContext.BinaryClassification.Trainers.SgdNonCalibrated(_opt._sgd_NC_Options);
									break;
								case "FastTree":
									_trainer = mlContext.BinaryClassification.Trainers.FastTree(_opt._ft_bt_Options);
									break;
								case "FastForest":
									_trainer = mlContext.BinaryClassification.Trainers.FastForest(_opt._ff_bt_Options);
									break;
								case "FieldAwareFactorizationMachine":
									_trainer = mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(_opt._fa_fm_Options);
									break;
								case "LightGbm":
									_trainer = mlContext.BinaryClassification.Trainers.LightGbm(_opt._lgbm_bt_Options);
									break;
								case "Gam":
									_trainer = mlContext.BinaryClassification.Trainers.Gam(_opt._gam_bt_Options);
									break;
								case "LbfgsLogisticRegression":
									_trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(_opt._lbfg_bt_Options);
									break;
								case "LinearSvm":
									_trainer = mlContext.BinaryClassification.Trainers.LinearSvm(_opt._lsvm_T_Options);
									break;
								case "SymbolicSgdLogisticRegression":
									_trainer = mlContext.BinaryClassification.Trainers.SymbolicSgdLogisticRegression(_opt._ssgd_rt_Options);
									break;
								case "Prior":
									_trainer = mlContext.BinaryClassification.Trainers.Prior(labelColumnName: "Label");
									break;
								default:
									_abort = true;
									break;
							}
							if (!_abort)
							{
								var pipeline = _concat.Append(_trainer);
								var model = pipeline.Fit(trainTestSplit.TrainSet);
								mlContext.Model.Save(model, trainingData.Schema, modelPath);
								try
								{
									IDataView _dv = model.Transform(trainTestSplit.TestSet);
									CalibratedBinaryClassificationMetrics _m = mlContext.BinaryClassification.Evaluate(_dv, labelColumnName: "Label");
									TrainingModelSave(_opt.modelTraining, _m);
								}
								catch (Exception ex)
								{
									int x = 0;
								}
							}
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		public static async Task<PredictorOutputDataFormatted> Predict(int Id_symbol)
		{
			try
			{
				MLContext mlContext = new MLContext();
				ITransformer mlModel;
				string _modelTrainning = "LightGbm";
				string modelPath = ("./Recursos/IA/" + _modelTrainning + "-" + Id_symbol.ToString() + ".zip");
				MLOptions _opt = new MLOptions();
				string[] _fields = _opt.featureColumns[0].Split(",");
				DataTable symbol = GetSymbol(Id_symbol);
				DataRow item = symbol.Rows[0];
				PredictorInputData _line = new PredictorInputData();

				foreach (string s in _fields)
				{
					try
					{
						System.Type _lineType = typeof(PredictorInputData);
						PropertyInfo _property = _lineType.GetProperty(s);
						if (_property.PropertyType == typeof(float))
						{
							string _val = item[s].ToString();
							if (_val == "") { _val = "0"; }
							_property.SetValue(_line, float.Parse(_val), null);
						}
						else if (_property.PropertyType == typeof(string))
						{
							_property.SetValue(_line, item[s].ToString(), null);
						}
					}
					catch (Exception rr)
					{
						int x = 0;
					}
				}
				using (Stream stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					mlModel = mlContext.Model.Load(modelPath, out DataViewSchema mlSchema);
				}
				PredictionEngine<PredictorInputData, PredictorOutputData> engine = mlContext.Model.CreatePredictionEngine<PredictorInputData, PredictorOutputData>(mlModel); //
				PredictorOutputData _response = engine.Predict(_line);
				SavePrediction(Id_symbol, _response.PredictedLabel);

				return new PredictorOutputDataFormatted()
				{
					PredictedLabel = _response.PredictedLabel,
					Probability = Double.Parse(_response.Probability.ToString()).ToString("F99").Substring(0, 12),
					Score = Double.Parse(_response.Score.ToString()).ToString("F99").Substring(0, 12),
					Symbol= item["code"].ToString(),
					Algoritmo = _modelTrainning
				};
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
