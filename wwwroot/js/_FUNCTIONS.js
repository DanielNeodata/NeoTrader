var _FUNCTIONS = {
	ExecutePostAjax: function (_url, _params) {
		return new Promise(
			function (resolve, reject) {
				try {
					$.ajax({
						type: "POST",
						url: _url,
						data: _params,
						dataType: "json",
						encode: true,
						success: function (data) {
							resolve(data);
						},
						error: function (xhr, ajaxOptions, thrownError) {
							reject(thrownError);
						}
					});
				} catch (rex) {
					reject(rex);
				}
			}
		)
	},
	LoadDataAjax: function (_url) {
		return new Promise(
			function (resolve, reject) {
				try {
					setTimeout(function () {
						var xhttp = new XMLHttpRequest();
						xhttp.open('GET', _url, false);
						xhttp.onreadystatechange = function () {
							if (xhttp.readyState == 4 && xhttp.status == 200) {
								if (xhttp.responseText != "") {
									resolve(JSON.parse(xhttp.responseText));
								} else {
									resolve(null);
								}
							};
						};
						xhttp.send();
					}, 1);
				} catch (rex) {
					reject(rex);
				}
			}
		)
	},
	onWait: function (_on) {
		if (_on) {
			$.blockUI({ message: '<img src="/img/wait.gif" />', css: { border: 'none', backgroundColor: 'transparent', opacity: 1, color: 'transparent' } });
			$(".blockOverlay").css({ "z-index": 9999999 });
			$(".blockPage").css({ "z-index": 9999999 });
		} else {
			$.unblockUI();
		}
	},

	OnCaptureSymbols: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		_FUNCTIONS.ExecutePostAjax("CaptureSymbols", null).then(function (data) {
			window.location = "Symbols";
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
	OnCaptureEvents: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		_FUNCTIONS.ExecutePostAjax("CaptureEvents", null).then(function (data) {
			window.location = "Symbols";
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
	OnPredictiveData: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		_FUNCTIONS.ExecutePostAjax("PredictiveData", null).then(function (data) {
			window.location = "Symbols";
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
	OnConsolidate: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		_FUNCTIONS.ExecutePostAjax("Consolidate", null).then(function (data) {
			window.location = "Symbols";
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
	OnAll: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		_FUNCTIONS.ExecutePostAjax("All", null).then(function (data) {
			window.location = "Symbols";
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
}
