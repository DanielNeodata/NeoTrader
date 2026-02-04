namespace NeoTrader.Models
{
	public class SymbolsViewModelItems
	{
		public int id { get; set; }
		public string code { get; set; }
		public string description { get; set; }
		public string instrumentType { get; set; }
		public string currency { get; set; }
		public DateTime created { get; set; }
		public DateTime verified { get; set; }
		public DateTime? offline { get; set; }
		public DateTime fum { get; set; }
		public int id_symbol { get; set; }
		public DateTime DatePrice { get; set; }
		public double Open { get; set; }
		public double RegularMarketVolume { get; set; }
		public double Volume { get; set; }
		public double FiftyTwoWeekLow { get; set; }
		public double FiftyTwoWeekHigh { get; set; }
		public double RegularMarketDayLow { get; set; }
		public double RegularMarketDayHigh { get; set; }
		public double Low { get; set; }
		public double High { get; set; }
		public double Close { get; set; }
		public double PercentageMovementPreviousDay { get; set; }
		public double PercentageMovementPreviousWeek { get; set; }
		public double PercentageMovementPreviousMonth { get; set; }
		public double SplitFactor { get; set; }
		public double PredictNextDay { get; set; }
		public double PredictNextWeek { get; set; }
		public double PredictNextMonth { get; set; }
	}
}
