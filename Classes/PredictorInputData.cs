namespace NeoTrader.Classes
{
	public class PredictorInputData
	{
		[LoadColumn(0)]
		public float id_symbol { get; set; }
		[LoadColumn(1)]
		public float DatePrice { get; set; }
		[LoadColumn(2)]
		public float Open { get; set; }
		[LoadColumn(3)]
		public float RegularMarketVolume { get; set; }
		[LoadColumn(4)]
		public float Volume { get; set; }
		[LoadColumn(5)]
		public float FiftyTwoWeekLow { get; set; }
		[LoadColumn(6)]
		public float FiftyTwoWeekHigh { get; set; }
		[LoadColumn(7)]
		public float RegularMarketDayLow { get; set; }
		[LoadColumn(8)]
		public float RegularMarketDayHigh { get; set; }
		[LoadColumn(9)]
		public float Low { get; set; }
		[LoadColumn(10)]
		public float High { get; set; }
		[LoadColumn(11)]
		public float Close { get; set; }
		[LoadColumn(12)]
		public float PercentageMovementPreviousDay { get; set; }
		[LoadColumn(13)]
		public float PercentageMovementPreviousWeek { get; set; }
		[LoadColumn(14)]
		public float PercentageMovementPreviousMonth { get; set; }
		[LoadColumn(15)]
		public float ModificadorBaseGauss { get; set; }
		[LoadColumn(16)]
		public float ModificadorMaterialGauss { get; set; }
		[LoadColumn(17)]
		public float ModificadorFinalGauss { get; set; }
	}
}

