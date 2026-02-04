namespace NeoTrader.Classes
{
	public struct PuntoCurva
	{
		public DateTime Fecha { get; set; }
		public double Valor { get; set; }

		public double ObtenerX(DateTime fechaBase) => (Fecha - fechaBase).TotalSeconds;
	}
}
