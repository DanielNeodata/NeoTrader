namespace NeoTrader.Models
{
	public class EventViewModel
	{
		public List<EventViewModelItem> eventos = new List<EventViewModelItem>();

		public EventViewModelItem Add(EventViewModelItem item)
		{
			eventos.Add(item);
			return item;
		}
		public bool Clear()
		{
			try
			{
				eventos.Clear();
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}
