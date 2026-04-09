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
	onDestroyModal: function (_id) {
		const myModalEl = document.querySelector('#'+_id);
		const modal = bootstrap.Modal.getOrCreateInstance(myModalEl);
		modal.hide();
	},
	onShowStaticModal: function (_params, _callback) {
		var _html = "";
		$(".modal-backdrop").remove();
		$("#" + _params.id).remove();
		_html += "<div class='modal fade' id='" + _params.id + "' data-bs-backdrop='static' data-bs-keyboard='false' tabindex='-1' aria-labelledby='staticBackdropLabel' aria-hidden='true'>";
		_html += "   <div class='modal-dialog modal-xl modal-dialog-top'>";
		_html += "      <div class='modal-content' style='position:absolute;top:0px;'>";
		_html += "         <div class='modal-header'>";
		_html += "            <h4 class='modal-title'>" + _params.title + "</h4>";
		_html += "         </div>";
		_html += "         <div class='modal-body'>" + _params.body + "</div>";
		_html += "		   <div class='modal-footer'></hr>";
		_html += "            <a href='#' class='btn btn-sm  btn-success btn-accept-modal'>Aceptar</a>";
		_html += "            <a href='#' class='btn btn-sm btn-danger btn-cancel-modal'>Cancelar</a>";
		_html += "         </div>";
		_html += "      </div>";
		_html += "   </div>";
		_html += "</div>";
		$("body").append(_html);
		var myModal = new bootstrap.Modal(document.getElementById(_params.id), { backdrop: 'static', keyboard: false });
		myModal.toggle();
		if ($.isFunction(_callback)) { _callback(); }
	},

	OnCaptureSymbols: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		var _reset = 0;
		if ($(".chkReset").prop("checked")) { _reset = 1; }
		var _params = { "_reset": _reset };
		_FUNCTIONS.ExecutePostAjax("CaptureSymbols", _params).then(function (data) {
			window.location.reload();
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
			window.location.reload();
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
	OnTrain: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		_FUNCTIONS.ExecutePostAjax("Train", null).then(function (data) {
			window.location.reload();
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
	OnPredict: function (_this) {
		_FUNCTIONS.onWait(true);
		var _params = { "Id_symbol": _this.attr("data-id") };
		_FUNCTIONS.ExecutePostAjax("Predict", _params).then(function (data) {
			_FUNCTIONS.onWait(false);
			_params = { "id": "modalResponse", "title": "NeoTrader informa", "body": data.message };
			_FUNCTIONS.onShowStaticModal(_params, function () {
				$(".btn-accept-modal").remove();
				$(".btn-cancel-modal").html("Cerrar");
			
				$("body").off("click", ".btn-cancel-modal").on("click", ".btn-cancel-modal", function () {
					_FUNCTIONS.onDestroyModal(_params.id);
					window.location.reload();
				});
			});
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_FUNCTIONS.onWait(false);
		});
	},
	OnConsolidate: function (_this) {
		_FUNCTIONS.onWait(true);
		_this.fadeOut("fast");
		_FUNCTIONS.ExecutePostAjax("Consolidate", null).then(function (data) {
			window.location.reload();
		}).catch(function (e) {
			if (e.Error != "") { alert(e.Error); }
			_this.fadeIn("slow");
			_FUNCTIONS.onWait(false);
		});
	},
}
