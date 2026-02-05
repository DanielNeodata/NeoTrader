$("body").off("click", ".btnCaptureSymbols").on("click", ".btnCaptureSymbols", function () {
	_FUNCTIONS.OnCaptureSymbols($(this));
});
$("body").off("click", ".btnCaptureEvents").on("click", ".btnCaptureEvents", function () {
	_FUNCTIONS.OnCaptureEvents($(this));
});
$("body").off("click", ".btnPredictiveData").on("click", ".btnPredictiveData", function () {
	_FUNCTIONS.OnPredictiveData($(this));
});
$("body").off("click", ".btnAll").on("click", ".btnAll", function () {
	_FUNCTIONS.OnAll($(this));
});
