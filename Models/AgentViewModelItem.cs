namespace NeoTrader.Models
{
	public class AgentViewModelItem
	{
		public string? Symbol { get; set; }
		public HistoricalFullData Data { get; set; }
		public Exception? Err { get; set; }

		public AgentViewModelItem(string _symbol, AssetProfile _asset, HistoricalFullData _data)
		{
			Symbol = _symbol;
			Data = _data;
			Err = null;
		}
		public AgentViewModelItem(Exception _err)
		{
			Symbol = null;
			Data = new HistoricalFullData();
			Err = _err;
		}
	}
}
