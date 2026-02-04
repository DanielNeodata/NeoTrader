namespace NeoTrader.Models
{
	public class QuestionViewModel
	{
		public List<QuestionViewModelItem> preguntas = new List<QuestionViewModelItem>();

		public QuestionViewModelItem Add(QuestionViewModelItem item)
		{
			preguntas.Add(item);
			return item;
		}
		public bool Clear()
		{
			try
			{
				preguntas.Clear();
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}
