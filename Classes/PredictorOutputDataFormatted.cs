namespace NeoTrader.Classes
{
	public class PredictorOutputDataFormatted
	{
		[ColumnName("PredictedLabel")]
		public bool PredictedLabel { get; set; }

		[ColumnName("Probability")]
		public string Probability { get; set; }

		[ColumnName("Score")]
		public string Score { get; set; }

		[ColumnName("Symbol")]
		public string Symbol { get; set; }
		
		[ColumnName("Algoritmo")]
		public string Algoritmo { get; set; }
	}
}

