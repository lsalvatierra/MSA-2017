<script>



    CargarListado();

function CargarListado() {
    $.ajax({
        'url': URL_PAGE + 'Administrador/Promocion/ListadoPromocion',
        'type': 'POST',
        //'data': $('#yourFormId').serialize(),
        'success': function (view) {
            //process here
            $("#divListadoPromocion").html(view);
        }
    });
}

$("#btnNuevaPromocion").click(function () {
    var modal = $("#mPopup");
    $.ajax({
        'url': URL_PAGE + 'Administrador/Promocion/FormPromocion',
        'type': 'POST',
        'success': function (view) {
            //process here
            modal.find(".modal-title").html("Registro de Promoción");
            modal.find(".btnGrabarPopup").val("Grabar");
            modal.find(".modal-body").html(view);

            modal.find(".modal-body #txtFechaInicio").datepicker({ changeMonth: true, changeYear: true, minDate: 0, maxDate: "+4Y", dateFormat: 'dd/mm/yy' });
            modal.find(".modal-body #txtFechaFin").datepicker({ changeMonth: true, changeYear: true, minDate: 0, maxDate: "+4Y", dateFormat: 'dd/mm/yy' });

            modal.modal();
        }
    });
});

$(document).on('keyup', '#txtNombrePromocion', function () {
    var dInput = $(this).val();
    var matches = dInput.match(/\b(\w)/g);
    $("#txtCodigoPromocion").val(matches.join('').toUpperCase());
});


$("#mPopup .modal-footer").on('click', '.btnGrabarPopup', function () {
    //
    var vNomPromo = $("#mPopup").find("#txtNombrePromocion").val();
    var vCodPromo = $("#mPopup").find("#txtCodigoPromocion").val();
    var vEmail = $("#mPopup").find("#txtCorreoPromocion").val();
    var vEvaluacion = $("#mPopup").find(".cboEvaluacion").val();
    var vFecIni = $("#mPopup").find("#txtFechaInicio").val();
    var vFecFin = $("#mPopup").find("#txtFechaFin").val();

    $.ajax({
        'url': URL_PAGE + 'Administrador/Promocion/GrabarPromocion',
        'type': 'POST',
        'data': {
            EvaluacionID: vEvaluacion, EvaluacionPromocionCodigo: vCodPromo
            , EvaluacionPromocionDescripcion: vNomPromo, EvaluacionPromocionCorreo: vEmail
            , EvaluacionPromocionFecIni: vFecIni, EvaluacionPromocionFecFin: vFecFin
        },
        'success': function (resultMsg) {
            //process here
            alert(resultMsg);
            $("#mPopup").modal("hide");
            CargarListado();
        }
    });

});

$(document).on('change', '.chkPersonalizarCodigo', function () {
    if (this.checked) {
        // checkbox is checked
        $("#mPopup").find("#txtCodigoPromocion").prop('disabled', false);
    } else {
        $("#mPopup").find("#txtCodigoPromocion").val("");
        $("#mPopup").find("#txtCodigoPromocion").prop('disabled', true);
    }
});

$(document).on('click', '.btnConfigurarMedicion', function () {
    var vPromocionID = $(this).attr("idPromo");
    //alert(vPromocionID);
    //return false;
    $.ajax({
        'url': URL_PAGE + 'Administrador/Promocion/ListadoMedicion',
        'type': 'POST',
        'data': {
            EvaluacionPromocionID: vPromocionID
        },
        'success': function (view) {
            //process here
            $("#divListadoMedicion").html(view);
        }
    });
});

$(document).on('change', '.btnAsignarFechaIni, .btnAsignarFechaFin', function () {

    var vPromocionID = $(this).attr("idpromo");
    var vCicloID = $(this).attr("idciclo");
    var vMedicionID = $(this).attr("idmedicion");
    var vFecha = $(this).val();
    var vTipo = $(this).attr("tipo");
    var vBoton = $(this);
    //alert($(this).html);
    //return false;
    $.ajax({
        'url': URL_PAGE + 'Administrador/Promocion/GrabarFechaMedicion',
        'type': 'POST',
        'data': {
            tipo: vTipo, fecha: vFecha, PromocionID: vPromocionID, CicloID: vCicloID, MedicionID: vMedicionID,
        },
        'success': function (resultMsg) {
            if (vBoton.hasClass("btn-danger")) {
                vBoton.removeClass('btn-danger');
                vBoton.toggleClass('btn-success');
            }
        }
    });
});

</script>