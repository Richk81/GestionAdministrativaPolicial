$(function () {

    // Mostrar el overlay de carga
    $(".card-body").LoadingOverlay("show");

    // Obtener datos de la dependencia
    fetch("/Division/Obtener")
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto;

                $("#txtRazonSocial").val(d.nombre);
                $("#txtCorreo").val(d.correo);
                $("#txtDireccion").val(d.direccion);
                $("#txtTelefono").val(d.telefono);
            } else {
                swal("Lo sentimos", responseJson.mensaje, "error");
            }
        })
        .catch(error => {
            $(".card-body").LoadingOverlay("hide");
            console.error(error);
            swal("Error", "No se pudieron cargar los datos de la dependencia", "error");
        });

});

// Guardar cambios al presionar el botón
$("#btnGuardarCambios").on("click", function () {

    // Validar campos requeridos
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter(item => item.value.trim() === "");

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar los siguientes campos: "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        document.querySelector(`input[name=${inputs_sin_valor[0].name}]`).focus();
        return;
    }

    const modelo = {
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txtTelefono").val(),
    };

    const formData = new FormData();
    formData.append("modelo", JSON.stringify(modelo));

    $(".card-body").LoadingOverlay("show");

    fetch("/Division/GuardarCambios", {
        method: "POST",
        body: formData
    })
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                swal("Listo", "Cambios actualizados correctamente", "success");
            } else {
                swal("Lo sentimos", responseJson.mensaje, "error");
            }
        })
        .catch(error => {
            $(".card-body").LoadingOverlay("hide");
            console.error(error);
            swal("Error", "Ocurrió un problema al guardar los datos", "error");
        });

});
