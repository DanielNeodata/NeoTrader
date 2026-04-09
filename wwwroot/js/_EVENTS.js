$("body").off("click", ".btnCaptureSymbols").on("click", ".btnCaptureSymbols", function () {
	_FUNCTIONS.OnCaptureSymbols($(this));
	_FUNCTIONS.OnCaptureEvents($(this));
	_FUNCTIONS.OnConsolidate($(this));
});
$("body").off("click", ".btnTrain").on("click", ".btnTrain", function () {
	_FUNCTIONS.OnTrain($(this));
});
$("body").off("click", ".btnPredict").on("click", ".btnPredict", function () {
	_FUNCTIONS.OnPredict($(this));
});
