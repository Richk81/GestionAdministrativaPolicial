

$(function () {

    //mostrar el overlay una rueda cargando
    $(".card-body").LoadingOverlay("show");

    // Inicializar el combo de roles
    fetch("/Division/Obtener")
        .then(response => {
            $(".card-body").LoadingOverlay("hide"); //oculta el overlay una rueda cargando
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            console.log(responseJson);

            if (responseJson.estado) {
                const d = responseJson.objeto; //variable que almacena nuestro objeto (nombre, id, etc)

                $("#txtRazonSocial").val(d.nombre);
                $("#txtCorreo").val(d.correo);
                $("#txtDireccion").val(d.direccion);
                $("#txtTelefono").val(d.telefono);
                $("#imgLogo").attr("src", d.urlLogo);

            } else {
                swal("Lo sentimos", responseJson.mensaje, "error")

            }
        })

})

$("#btnGuardarCambios").on("click", function () {

    // IMPORTANTE: Validar campos requeridos; de los impus o caja de textos (nombre, telefono, direccion, etc.)
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() === "")

    if (inputs_sin_valor.length > 0) {
        // Si hay campos sin valor, mostramos un mensaje y enfocamos el primer campo vacío, acá lo creamos y los mostramos con toastr
        const mensaje = `Debe completar los siguientes campos: "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje) // <-- Aquí está el toastr libreria para mostrar mensajes en pantalla
        document.querySelector(`input[name=${inputs_sin_valor[0].name}]`).focus();

        return;
    }

    const modelo = {
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txtTelefono").val(),
    }

    const inputLogo = document.getElementById("txtNombreLogo")

    const formData = new FormData()

    formData.append("logo", inputLogo.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    //mostrar el overlay una rueda cargando
    $(".card-body").LoadingOverlay("show");

    // Inicializar el combo de roles
    fetch("/Division/GuardarCambios", {
        method: "POST",
        body: formData
    })
        .then(response => {
            $(".card-body").LoadingOverlay("hide"); //oculta el overlay una rueda cargando
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            if (responseJson.estado) {
                const d = responseJson.objeto; //variable que almacena nuestro objeto (nombre, id, etc)

                $("#imgLogo").attr("src", d.urlLogo)

                // ✅ Mensaje de confirmación
                swal("Listo", "Cambios Actualizados Correctamente", "success");

            } else {
                swal("Lo sentimos", responseJson.mensaje, "error")

            }
        })

})