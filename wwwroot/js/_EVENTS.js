$("body").off("click", ".btnCaptureSymbols").on("click", ".btnCaptureSymbols", function () {
	_FUNCTIONS.OnCaptureSymbols($(this));
});
$("body").off("click", ".btnCaptureEvents").on("click", ".btnCaptureEvents", function () {
	_FUNCTIONS.OnCaptureEvents($(this));
});
