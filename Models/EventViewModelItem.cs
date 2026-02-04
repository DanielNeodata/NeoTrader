namespace NeoTrader.Models
{
	public class EventViewModelItem {
		public int? Id { get; set; }
		public string? code { get; set; }
		public string? description { get; set; }
		public DateTime? FechaInicioGauss { get; set; }
		public DateTime? FechaPicoGauss { get; set; }
		public DateTime? FechaFinPicoGauss { get; set; }
		public DateTime? FechaFinGauss { get; set; }
		public double ValorFechaInicioGauss { get; set; }
		public double ValorFechaPicoGauss { get; set; }
		public double ValorFechaFinPicoGauss { get; set; }
		public double ValorFechaFinGauss { get; set; }
		public double ModificadorBaseGauss { get; set; }
		// Calculado:
		// Suma de curvas Gauss de todos los EventViewModel con
		// FechaInicioGauss(otros)>=FechaPicoGauss(this) AND FechaFinPicoGauss(this)<=FechaFinGauss(otros) )
		// Debe ser la integral de la nueva curva
		public double ModificadorMaterialGauss { get; set; }
		// Calculado:
		// Suma de ModificadorBaseGauss+ModificadorMaterialGauss
		public double ModificadorFinalGauss { get; set; }

		public EventViewModelItem(int? _id,string? _code,  string? _description,
			DateTime? _FechaInicioGauss, DateTime? _FechaPicoGauss, DateTime? _FechaFinPicoGauss, DateTime? _FechaFinGauss,
			double _ValorFechaInicioGauss, double _ValorFechaPicoGauss, double _ValorFechaFinPicoGauss, double _ValorFechaFinGauss,
			double _ModificadorBaseGauss, double _ModificadorMaterialGauss, double _ModificadorFinalGauss) {
			Id = _id;
			code = _code;
			description = _description;
			FechaInicioGauss = _FechaInicioGauss;
			FechaPicoGauss = _FechaPicoGauss;
			FechaFinPicoGauss = _FechaFinPicoGauss;
			FechaFinGauss = _FechaFinGauss;
			ValorFechaInicioGauss = _ValorFechaInicioGauss;
			ValorFechaPicoGauss = _ValorFechaPicoGauss;
			ValorFechaFinPicoGauss = _ValorFechaFinPicoGauss;
			ValorFechaFinGauss = _ValorFechaFinGauss;
			ModificadorBaseGauss = _ModificadorBaseGauss;
			ModificadorMaterialGauss = _ModificadorMaterialGauss;
			ModificadorFinalGauss = _ModificadorFinalGauss;
		}
	}
}