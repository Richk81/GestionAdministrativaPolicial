

const MODELO_BASE = {
    idUsuario: 0,
    nombre: "",
    correo: "",
    telefono: "",
    idRol: 0,
    esActivo: 1,
    urlFoto: ""
}

let tablaData;

$(document).ready(function () {

    // Inicializar el combo de roles
    fetch("/Usuario/ListaRoles")
        .then(response => { 
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboRol").append($("<option>")
                        .val(item.idRol).text(item.descripcion)
                    )
                })
            }
        })


    tablaData = $('#tbdataUsuarios').DataTable({
        responsive: true,
        autoWidth: false,  // <-- importante
         "ajax": {
             "url": '/Usuario/Lista',
             "type": "GET",
             "datatype": "json"
         },
         "columns": [
             { "data": "idUsuario","visible":false, "searchable":false},
             {
                 "data": "urlFoto", render: function (data) {
                     return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`;
                 }
             },
             { "data": "nombre" },
             { "data": "correo" },
             { "data": "telefono" },
             { "data": "nombreRol" },
             {
                 "data": "esActivo", render: function (data) {
                     if (data ==1) 
                         return '<span class="badge badge-info">Activo</span>';
                     else
                         return '<span class="badge badge-danger">No Activo</span>';
                 }
             },
             {
                 "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                     '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                 "orderable": false,
                 "searchable": false,
                 "width": "90px"
             }
         ],
         order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [2,3,4,5,6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

// Mstrar MODAL
function mostrarModal(modelo = MODELO_BASE ) {
    $("#txtId").val(modelo.idUsuario);
    $("#txtNombre").val(modelo.nombre);
    $("#txtCorreo").val(modelo.correo);
    $("#txtTelefono").val(modelo.telefono);

    // si es 0 tomamos el primer elemento del combo, caso contario tomamos el valor del modelo
    $("#cboRol").val(modelo.idRol == 0 ? $("#cboRol option:first").val() : modelo.idRol);

    $("#cboEstado").val(modelo.esActivo);
    $("#txtFoto").val("");
    $("#imgUsuario").attr("src", modelo.urlFoto);


    $("#modalData").modal("show");
}

// Evento click del boton nuevoUSUARIO
$("#btnNuevo").click(function () {
    mostrarModal();
});

// Evento click del boton Guardar
$("#btnGuardar").click(function () {

    // IMPORTANTE: Validar campos requeridos
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() === "")

    if (inputs_sin_valor.length > 0) {
 // Si hay campos sin valor, mostramos un mensaje y enfocamos el primer campo vacío, acá lo creamos y los mostramos con toastr
        const mensaje = `Debe completar los siguientes campos: "${inputs_sin_valor[0].name}"`;   
        toastr.warning("", mensaje) // <-- Aquí está el toastr libreria para mostrar mensajes en pantalla
        $(`input[name=${inputs_sin_valor[0].name}]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idUsuario"] = parseInt($("#txtId").val());
    modelo["nombre"] = $("#txtNombre").val();
    modelo["correo"] = $("#txtCorreo").val();
    modelo["telefono"] = $("#txtTelefono").val();
    modelo["idRol"] = parseInt($("#cboRol").val());
    modelo["esActivo"] = parseInt($("#cboEstado").val());

    const inputFoto = document.getElementById("txtFoto");

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idUsuario == 0) {

        fetch("/Usuario/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo", "El Usuario fue Creado", "success").then(() => {
                        $("#modalData").modal("hide");
                    });
                } else {
                    swal({
                        title: "Lo sentimos",
                        text: responseJson.mensaje,
                        icon: "error",
                        button: "OK"
                    });
                }

            })
    } else {
        fetch("/Usuario/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {             //NOTA: LoadingOverlay --> muestra una animacion de carga (en este caso un circulo girando))
                $("#modalData").find("div.modal-content").LoadingOverlay("hide"); 
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalData").modal("hide")
                    swal("Listo", "El Usuario fue Actualizado", "success").then(() => {
                        $("#modalData").modal("hide");
                    });
                } else {
                    swal({
                        title: "Lo sentimos",
                        text: responseJson.mensaje,
                        icon: "error",
                        button: "OK"
                    });
                }
            })
    }


});

//Boton EDITAR
let filaSeleccionada;
$("#tbdataUsuarios tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);

})

// Botón ELIMINAR
$("#tbdataUsuarios tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "¿Estás seguro?",
        text: `Eliminar al Usuario "${data.nombre}"`,
        icon: "warning",
        buttons: {
            cancel: "Cancelar",
            confirm: {
                text: "Sí, eliminar",
                className: "btn-danger"
            }
        },
        dangerMode: true,
    }).then((respuesta) => {
        if (respuesta) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`/Usuario/Eliminar?IdUsuario=${data.idUsuario}`, {
                method: "DELETE"
            })
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw();

                        swal("Listo", `El Usuario "${data.nombre}" fue Eliminado`, "success");
                    } else {
                        swal({
                            title: "Lo sentimos",
                            text: responseJson.mensaje,
                            icon: "error",
                            button: "OK"
                        });
                    }
                })
                .catch(() => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    swal({
                        title: "Error",
                        text: "No se pudo eliminar el usuario",
                        icon: "error",
                        button: "OK"
                    });
                });
        }
    });
});
