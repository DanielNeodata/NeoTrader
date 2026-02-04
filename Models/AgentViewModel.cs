namespace NeoTrader.Models
{
	public class AgentViewModel
	{
		public CultureInfo deDECulture = CultureInfo.CreateSpecificCulture("de-DE");
		public List<AgentViewModelItem> simbolos = new List<AgentViewModelItem>();
		public List<EventViewModelItem> eventos = new List<EventViewModelItem>();
		public List<QuestionViewModelItem> preguntas = new List<QuestionViewModelItem>();

		public AgentViewModelItem Add(AgentViewModelItem item)
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
