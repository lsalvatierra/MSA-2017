$(document).ready(function () {
    $("[data-toggle='tooltip']").tooltip();
    
    $(".btnVerParticipantes").on("click", function () {
        //alert($(this).attr("data-idPromocion"));
        waitingDialog.show('Cargando', { dialogSize: 'sm', progressType: 'danger' });
        $.ajax({
            type: "POST",
            url: URL_PAGE + "Administrador/ResultadoEvaluacion/ParticipantesPromocion",
            cache: false,
            data: {
                p_idPromocion: $(this).attr("data-idPromocion")
            },
            success: function (data) {
                $("#divfrmResultado").html("");
                $("#divfrmResultado").html(data);
                waitingDialog.hide();
            }
        });
    });

    

});