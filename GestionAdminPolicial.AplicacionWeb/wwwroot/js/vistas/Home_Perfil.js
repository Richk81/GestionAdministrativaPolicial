$(function () {

    //mostrar el overlay una rueda cargando
    $(".container-fluid").LoadingOverlay("show");

    fetch("/api/v1/ApiHome/ObtenerUsuario")
        .then(response => {
            $(".container-fluid").LoadingOverlay("hide"); //oculta el overlay una rueda cargando
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            if (responseJson.estado) {
                const d = responseJson.objeto; //variable que almacena nuestro objeto (nombre, id, etc)


                //$("#imgFoto").attr("src", urlFoto);

                $("#txtNombre").val(d.nombre);
                $("#txtCorreo").val(d.correo);
                $("#txtTelefono").val(d.telefono); // 👈 corregido
                $("#txtRol").val(d.nombreRol);

            } else {
                swal("Lo sentimos", responseJson.mensaje, "error");
            }
        });

});

$(function () {

    $("#btnGuardarCambios").click(function () {

        // 1️⃣ Validaciones
        if ($("#txtCorreo").val().trim() === "") {
            toastr.warning("", "Debe completar el campo Correo");
            $("#txtCorreo").focus();
            return;
        }

        if ($("#txtTelefono").val().trim() === "") {
            toastr.warning("", "Debe completar el campo Teléfono");
            $("#txtTelefono").focus();
            return;
        }

        // 2️⃣ SweetAlert confirmación (estilo moderno)
        swal({
            title: "¿Desea Guardar los Cambios?",
            text: "Se actualizarán los datos del perfil",
            icon: "warning",
            buttons: {
                cancel: "No",
                confirm: {
                    text: "Sí",
                    className: "btn-primary"
                }
            },
            dangerMode: true
        })
            .then(respuesta => {
                if (respuesta) {
                    // Si confirma el usuario
                    $(".container-fluid").LoadingOverlay("show"); // Mostrar overlay

                    const modelo = {
                        Correo: $("#txtCorreo").val().trim(),
                        Telefono: $("#txtTelefono").val().trim()
                    };

                    fetch("/api/v1/ApiHome/GuardarPerfil", {
                        method: "POST",
                        headers: { "Content-Type": "application/json; charset=utf-8" },
                        body: JSON.stringify(modelo)
                    })
                        .then(response => {
                            $(".container-fluid").LoadingOverlay("hide");
                            return response.ok ? response.json() : Promise.reject(response);
                        })
                        .then(responseJson => {
                            if (responseJson.estado) {
                                swal("Listo!", "Cambios Actualizados Correctamente", "success");
                            } else {
                                swal("Lo sentimos", responseJson.mensaje, "error");
                            }
                        })
                        .catch(err => {
                            $(".container-fluid").LoadingOverlay("hide");
                            swal("Error", "No se pudo guardar. " + err, "error");
                        });
                }
            });

    });

});

$(function () {

    $("#btnCambiarClave").click(function () {

        // 1️⃣ Validar todos los campos con clase "input-validar"
        const inputs = $("input.input-validar").serializeArray();
        const vacios = inputs.filter(item => item.value.trim() === "");

        if (vacios.length > 0) {
            const mensaje = `Debe completar el campo: "${vacios[0].name}"`;
            toastr.warning("", mensaje);
            $(`input[name="${vacios[0].name}"]`).focus();
            return; // detener ejecución si hay campos vacíos
        }

        // 2️⃣ Verificar que la nueva contraseña y la confirmación coincidan
        if ($("#txtClaveNueva").val().trim() !== $("#txtConfirmarClave").val().trim()) {
            toastr.warning("", "Las Contraseñas y NO Coinciden");
            $("#txtConfirmarClave").focus();
            return;
        }

        // 3️⃣ Confirmación con SweetAlert moderno
        swal({
            title: "¿Desea Cambiar la Contraseña?",
            text: "Se actualizará la contraseña de su perfil",
            icon: "warning",
            buttons: {
                cancel: "No",
                confirm: {
                    text: "Sí",
                    className: "btn-primary"
                }
            },
            dangerMode: true
        })
            .then(respuesta => {
                if (!respuesta) return;

                $(".container-fluid").LoadingOverlay("show");

                const modelo = {
                    ClaveActual: $("#txtClaveActual").val().trim(),
                    NuevaClave: $("#txtClaveNueva").val().trim()
                };


                fetch("/api/v1/ApiHome/CambiarClave", {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify(modelo)
                })
                    .then(response => {
                        $(".container-fluid").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            swal("Listo!", "Contraseña Actualizada Correctamente", "success");
                            $("input.input-validar").val(""); // limpiar campos
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error");
                        }
                    })
                    .catch(err => {
                        $(".container-fluid").LoadingOverlay("hide");
                        swal("Error", "No se pudo cambiar la contraseña. " + err, "error");
                    });
            });

    });

});








