namespace NeoTrader.Classes
{
	public class neoResponse
	{
		public bool? Status { get; set; }
		public string? Message { get; set; }
		public string? Error { get; set; }
		public neoResponse(bool _status, string? _message, string? _error)
		{
			Status = _status;
			Message = _message;
			Error = _error;
		}
	}
}
