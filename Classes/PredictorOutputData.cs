namespace NeoTrader.Classes
{
	public class PredictorOutputData
	{
		[ColumnName("PredictedLabel")]
		public bool PredictedLabel { get; set; }

		[ColumnName("Probability")]
		public float Probability { get; set; }

		[ColumnName("Score")]
		public float Score { get; set; }
	}
}

