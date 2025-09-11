
const MODELO_BASE = {
    idPersonal: 0,
    legajo: "",
    idUsuario: 0,
    apellidoYNombre: "",
    grado: "",
    chapa: "",
    sexo: "",
    funcion: "",
    horario: "",
    situacionRevista: "",
    fechaNacimiento: "",
    telefono: "",
    telefonoEmergencia: "",
    dni: "",
    subsidioSalud: "",
    estudiosCurs: "",
    estadoCivil: "",
    especialidad: "",
    altaEnDivision: "",
    altaEnPolicia: "",
    destinoAnterior: "",
    email: "",
    trasladado: 0,
    detalles: "",
    urlImagen: "",
    nombreImagen: ""
}

//🔹 DataTable(Personal Policial / Lista)
//Esto sería la tabla principal en el front:

let tablaData;

$(document).ready(function () {
    tablaData = $('#tbdataPersonal').DataTable({
        responsive: true,
        autoWidth: false,
        "ajax": {
            "url": '/Personal/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idPersonal", "visible": false, "searchable": false }, // oculto
            {
                "data": "urlImagen", render: function (data) {
                    return `<img style="height:60px" src="${data || '/imagenes/noimage.png'}" class="rounded mx-auto d-block"/>`;

                }
            },
            { "data": "legajo" },
            { "data": "apellidoYNombre" },
            { "data": "grado" },
            { "data": "funcion" },
            { "data": "situacionRevista" },
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
                filename: 'Reporte_PersonalPolicial',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});

// 🔹 Abrir modal para "Nuevo Personal"
$("#btnNuevoPersonal").click(function () {
    mostrarModalPersonal(); // llama a la función que ya tienes
});

//🔹 Funcion Mostrar Modal
function mostrarModalPersonal(modelo = MODELO_BASE) {
    $("#txtIdPersonal").val(modelo.idPersonal);
    $("#txtLegajo").val(modelo.legajo);
    $("#txtApellidoYNombre").val(modelo.apellidoYNombre);
    $("#txtGrado").val(modelo.grado);
    $("#txtChapa").val(modelo.chapa);
    $("#txtSexo").val(modelo.sexo);
    $("#txtFuncion").val(modelo.funcion);
    $("#txtHorario").val(modelo.horario);
    $("#txtSituacionRevista").val(modelo.situacionRevista);
    $("#txtFechaNacimiento").val(modelo.fechaNacimiento);
    $("#txtTelefono").val(modelo.telefono);
    $("#txtTelefonoEmergencia").val(modelo.telefonoEmergencia);
    $("#txtDNI").val(modelo.dni);
    $("#txtSubsidioSalud").val(modelo.subsidioSalud);
    $("#txtEstudiosCurs").val(modelo.estudiosCurs);
    $("#txtEstadoCivil").val(modelo.estadoCivil);
    $("#txtEspecialidad").val(modelo.especialidad);
    $("#txtAltaEnDivision").val(modelo.altaEnDivision);
    $("#txtAltaEnPolicia").val(modelo.altaEnPolicia);
    $("#txtDestinoAnterior").val(modelo.destinoAnterior);
    $("#txtEmail").val(modelo.email);
    $("#cboTrasladado").val(modelo.trasladado);
    $("#txtDetalles").val(modelo.detalles);

    $("#imgPersonal").attr("src", modelo.urlImagen || "/imagenes/noimage.png");

    $("#modalPersonal").modal("show");
}

// 🔹 Habilitar/deshabilitar campos de arma
$("#chkArmaProvista").change(function () {
    const habilitar = $(this).is(":checked");
    $("#txtArmaMarca, #txtArmaNumeroSerie").prop("disabled", !habilitar);

    if (!habilitar) {
        // Limpiar campos si se desmarca
        $("#txtArmaMarca, #txtArmaNumeroSerie").val("");
    }
});

//🔹 Guardar (crear/editar)
$("#btnGuardar").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const vacios = inputs.filter((item) => item.value.trim() === "")

    if (vacios.length > 0) {
        const mensaje = `Debe completar: "${vacios[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name=${vacios[0].name}]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idPersonal"] = parseInt($("#txtIdPersonal").val());
    modelo["legajo"] = $("#txtLegajo").val();
    modelo["apellidoYNombre"] = $("#txtApellidoYNombre").val();
    modelo["grado"] = $("#txtGrado").val();
    modelo["chapa"] = $("#txtChapa").val();
    modelo["sexo"] = $("#txtSexo").val();
    modelo["funcion"] = $("#txtFuncion").val();
    modelo["horario"] = $("#txtHorario").val();
    modelo["situacionRevista"] = $("#txtSituacionRevista").val();
    modelo["fechaNacimiento"] = $("#txtFechaNacimiento").val();
    modelo["telefono"] = $("#txtTelefono").val();
    modelo["telefonoEmergencia"] = $("#txtTelefonoEmergencia").val();
    modelo["dni"] = $("#txtDNI").val();
    modelo["subsidioSalud"] = $("#txtSubsidioSalud").val();
    modelo["estudiosCurs"] = $("#txtEstudiosCurs").val();
    modelo["estadoCivil"] = $("#txtEstadoCivil").val();
    modelo["especialidad"] = $("#txtEspecialidad").val();
    modelo["altaEnDivision"] = $("#txtAltaEnDivision").val();
    modelo["altaEnPolicia"] = $("#txtAltaEnPolicia").val();
    modelo["destinoAnterior"] = $("#txtDestinoAnterior").val();
    modelo["email"] = $("#txtEmail").val();
    modelo["trasladado"] = parseInt($("#cboTrasladado").val());
    modelo["detalles"] = $("#txtDetalles").val();

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idPersonal == 0) {
        fetch("/PersonalPolicial/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(r => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return r.ok ? r.json() : Promise.reject(r);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo", "Personal creado", "success");
                } else {
                    swal("Error", responseJson.mensaje, "error");
                }
            })
    } else {
        fetch("/PersonalPolicial/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(r => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return r.ok ? r.json() : Promise.reject(r);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalData").modal("hide");
                    swal("Listo", "Personal actualizado", "success");
                } else {
                    swal("Error", responseJson.mensaje, "error");
                }
            })
    }
});


