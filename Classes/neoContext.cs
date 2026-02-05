namespace NeoTrader.Classes
{
	public static class neoContext
	{
		public static string connString = (@"encrypt=false;database=neo_trader;server=DESARROLLO\SQLEXPRESS;user=sa;password=08Z5il37;MultipleActiveResultSets=True");
		public static double _base = 100000000000;
		public static CultureInfo deDECulture = CultureInfo.CreateSpecificCulture("de-DE");

		public static List<T> ConvertDataTableToList<T>(DataTable dt) where T : new()
		{
			List<T> list = new List<T>();
			foreach (DataRow row in dt.Rows)
			{
				T obj = new T();
				foreach (DataColumn col in dt.Columns)
				{
					var prop = obj.GetType().GetProperty(col.ColumnName);
					if (prop != null && row[col] != DBNull.Value) { prop.SetValue(obj, row[col]); }
				}
				list.Add(obj);
			}
			return list;
		}
		public static string ToMoney(object data, string _curr)
		{
			NumberFormatInfo customFormat = (NumberFormatInfo)deDECulture.NumberFormat.Clone();
			customFormat.CurrencySymbol = _curr;
			customFormat.CurrencyPositivePattern = 2;
			return Convert.ToDecimal(data.ToString()).ToString("C", customFormat);
		}
		public static string ToCompareDate(string[] _day)
		{
			StringBuilder _html = new StringBuilder();
			if (Convert.ToDateTime(_day[0]) != DateTime.Today)
			{
				_html.Append("<td class='text-start text-danger'>");
				_html.Append(_day[0]);
			}
			else
			{
				_html.Append("<td class='text-start text-success'>");
				_html.Append(_day[0]);
				_html.Append(" ");
				_html.Append(_day[1]);
			}
			_html.Append("</td>");

			return _html.ToString();
		}
		public static int DayDiff(DateTime date)
		{
			DateTime currentDate = DateTime.Now;
			TimeSpan timeDifference = (date - currentDate);
			int _diff = Math.Abs(timeDifference.Days);
			if (_diff == 0) { _diff = 1; }
			return _diff;
		}
		public static double DiffPercentage(double _actual, double _close)
		{
			double _ret = (_actual - _close) / (((_actual + _close) / 2));
			return _ret;
		}

		public static double CalcularIntegral(List<PuntoCurva> puntos)
		{
			var puntosOrdenados = puntos.OrderBy(p => p.Fecha).ToList();
			double integralTotal = 0;
			DateTime fechaBase = puntosOrdenados[0].Fecha;
			for (int i = 0; i < puntosOrdenados.Count - 1; i++)
			{
				double x1 = puntosOrdenados[i].ObtenerX(fechaBase);
				double x2 = puntosOrdenados[i + 1].ObtenerX(fechaBase);
				double y1 = puntosOrdenados[i].Valor;
				double y2 = puntosOrdenados[i + 1].Valor;

				// Sumar área del trapecio: (base mayor + base menor) / 2 * altura
				integralTotal += ((y1 + y2) / 2.0) * (x2 - x1);
			}
			return (integralTotal / _base);
		}
	}
}
