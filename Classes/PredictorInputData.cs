namespace NeoTrader.Classes
{
	public class PredictorInputData
	{
		[ColumnName("id_symbol"), LoadColumn(0)]
		public float id_symbol { get; set; }

		[ColumnName("DatePrice"), LoadColumn(1)]
		public float DatePrice { get; set; }

		[ColumnName("Open"), LoadColumn(2)]
		public float Open { get; set; }

		[ColumnName("RegularMarketVolume"), LoadColumn(3)]
		public float RegularMarketVolume { get; set; }

		[ColumnName("Volume"), LoadColumn(4)]
		public float Volume { get; set; }

		[ColumnName("FiftyTwoWeekLow"), LoadColumn(5)]
		public float FiftyTwoWeekLow { get; set; }

		[ColumnName("FiftyTwoWeekHigh"), LoadColumn(6)]
		public float FiftyTwoWeekHigh { get; set; }

		[ColumnName("RegularMarketDayLow"), LoadColumn(7)]
		public float RegularMarketDayLow { get; set; }

		[ColumnName("RegularMarketDayHigh"), LoadColumn(8)]
		public float RegularMarketDayHigh { get; set; }

		[ColumnName("Low"), LoadColumn(9)]
		public float Low { get; set; }

		[ColumnName("High"), LoadColumn(10)]
		public float High { get; set; }

		[ColumnName("Close"), LoadColumn(11)]
		public float Close { get; set; }

		[ColumnName("PercentageMovementPreviousDay"), LoadColumn(12)]
		public float PercentageMovementPreviousDay { get; set; }

		[ColumnName("PercentageMovementPreviousWeek"), LoadColumn(13)]
		public float PercentageMovementPreviousWeek { get; set; }

		[ColumnName("PercentageMovementPreviousMonth"), LoadColumn(14)]
		public float PercentageMovementPreviousMonth { get; set; }

		[ColumnName("ModificadorBaseGauss"), LoadColumn(15)]
		public float ModificadorBaseGauss { get; set; }

		[ColumnName("ModificadorMaterialGauss"), LoadColumn(16)]
		public float ModificadorMaterialGauss { get; set; }
		
		[ColumnName("ModificadorFinalGauss"), LoadColumn(17)]
		public float ModificadorFinalGauss { get; set; }
		
		[ColumnName("Label"), LoadColumn(18)]
		public Boolean Label { get; set; }
	}
}

