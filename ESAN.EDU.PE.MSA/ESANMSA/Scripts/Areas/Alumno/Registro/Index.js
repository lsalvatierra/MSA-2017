$(document).ready(function () {
    $("#btnVerificarDatos").on("click", function () {
        var formValidado = true;
        var nroDocumento = $("#txtNroDocumento").val();
        if (nroDocumento == "") {
            MostrarMensajeError("xnrodoc", "Ingrese su código de alumno.");
            formValidado = false;
        } else {
            LimpiarMensajeError("xnrodoc");
        }
        if (formValidado) {
            waitingDialog.show('Cargando', { dialogSize: 'sm', progressType: 'danger' });
            $.ajax({
                type: "POST",
                url: URL_PAGE + "Alumno/Registro/VerificarUsuario",
                cache: false,
                data: {
                    p_idTipoDocumento: 4,
                    p_nroDocumento: nroDocumento,
                    p_idPromocion: $("#hddIdPromocion").val(),
                    p_idMedicion: $("#hddIdMedicion").val()
                },
                success: function (data) {                   
                    if (data.Existe == -1) {
                        $("#divExisteEval").fadeOut("fast");
                        $("#divRegistrar").fadeIn("slow");
                    } else if (data.Existe == 1) {
                        $("#divRegistrar").fadeOut("fast");
                        $("#divExisteEval").fadeIn("slow");
                    } else if (data.Existe == 0) {
                        $("#frmRegistro").submit();
                    }
                    waitingDialog.hide();
                }
            });
        }
    });

    $("#btnGuardarEmpezar").on("click", function () {
        var formValidado = true;
        var nroDocumento = $("#txtNroDocumento").val();
        var apePaterno = $("#txtApellidoPaterno").val();
        var apeMaterno = $("#txtApellidoMaterno").val();
        var nombres = $("#txtNombres").val();

        if (apePaterno == "") {
            MostrarMensajeError("xapepat", "Ingrese su apellido paterno.");
            formValidado = false;
        } else 
            LimpiarMensajeError("xapepat");
        
        if (apeMaterno == "") {
            MostrarMensajeError("xapemat", "Ingrese su apellido materno.");
            formValidado = false;
        } else 
            LimpiarMensajeError("xapemat");

        if (nombres == "") {
            MostrarMensajeError("xnom", "Ingrese sus nombres.");
            formValidado = false;
        } else
            LimpiarMensajeError("xnom");

        
        if (formValidado) {
            //alert("vamos a registrar");
            waitingDialog.show('Cargando', { dialogSize: 'sm', progressType: 'danger' });
            $.ajax({
                type: "POST",
                url: URL_PAGE + "Alumno/Registro/RegistrarUsuario",
                cache: false,
                data: {
                    p_idTipoDocumento: 4,
                    p_nroDocumento: nroDocumento,
                    p_apePaterno: apePaterno,
                    p_apeMaterno: apeMaterno,
                    p_nombres: nombres,
                    p_idPromocion: $("#hddIdPromocion").val(),
                    p_idMedicion: $("#hddIdMedicion").val(),
                    p_Externo: $("#hddExterno").val()
                },
                success: function (data) {
                    if (data.rpta) {
                        $("#frmRegistro").submit();
                    } else {
                      alert("Ocurrió un error en la base de datos.")
                    }
                    waitingDialog.hide();
                }
            });

        } 
    });


    $("#btnGuardarEmpezarExterno").on("click", function () {
        var formValidado = true;
        var tipoRelacion = $("#cboTipoRelacion").val();

        if (tipoRelacion == "0") {
            MostrarMensajeError("xtiporel", "Seleccione Tipo de relación.");
            formValidado = false;
        } else
            LimpiarMensajeError("xtiporel");
        
        if (formValidado) {

            waitingDialog.show('Cargando', { dialogSize: 'sm', progressType: 'danger' });
            $.ajax({
                type: "POST",
                url: URL_PAGE + "Alumno/Registro/RegistrarUsuario",
                cache: false,
                data: {
                    p_idPromocion: $("#hddIdPromocion").val(),
                    p_idMedicion: $("#hddIdMedicion").val(),
                    p_Externo: $("#hddExterno").val(),
                    p_idTipoRelacion: tipoRelacion
                },
                success: function (data) {
                    if (data.rpta) {
                        $("#frmRegistro").submit();
                    } else {
                        alert("Ocurrió un error en la base de datos.")
                    }
                    waitingDialog.hide();
                }
            });

        }
    });

    $("#txtApellidoPaterno").keyup(function () {
        if ($("#xapepat").html().length > 0) {
            LimpiarMensajeError("xapepat");
        }        
    });
    $("#txtApellidoMaterno").keyup(function () {
        if ($("#xapemat").html().length > 0) {
            LimpiarMensajeError("xapemat");
        }
    });
    $("#txtNombres").keyup(function () {
        if ($("#xnom").html().length > 0) {
            LimpiarMensajeError("xnom");
        }
    });
});

// funcion para mostrar mensaje de error
function MostrarMensajeError(div, mensaje) {
    LimpiarMensajeError(div);
    $("#" + div).append("<h6 class='text-danger'><span class='glyphicon glyphicon-asterisk' aria-hidden='true'></span>&nbsp;" + mensaje + "</h6>");
    $("#" + div).hide().removeClass('hide').slideDown();
}
// funcion que limpia el mensaje de error
function LimpiarMensajeError(div) {
    $("#" + div).empty();
}
