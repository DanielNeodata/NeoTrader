using System.Text;

namespace NeoTrader.Models
{
	public class QuestionViewModelItem {
		public int? Id { get; set; }
		public string? Descripcion { get; set; }
		public float? Prediccion { get; set; }
		public int DiasPreviosEvaluables { get; set; }
		public int DiasPosterioresEvaluables { get; set; }
		public float? Correcta { get; set; }
	}
}
