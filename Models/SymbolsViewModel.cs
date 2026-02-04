namespace NeoTrader.Models
{
	public class SymbolsViewModel
	{
		public List<SymbolsViewModelItems> simbolos = new List<SymbolsViewModelItems>();

		public SymbolsViewModelItems Add(SymbolsViewModelItems item)
		{
			simbolos.Add(item);
			return item;
		}
		public bool Clear()
		{
			try
			{
				simbolos.Clear();
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}
