
const MODELO_BASE_PERSONAL = {
    idPersonal: 0,
    legajo: "",
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
    detalles: "",
    urlImagen: "",
    nombreImagen: "",

    // 🔹 Relaciones como arrays vacíos por defecto
    domicilios: [
        {
            calleBarrio: "",
            localidad: "",
            comisariaJuris: ""
        }
    ],
    armas: [
        {
            marca: "No Provista",
            numeroSerie: "No Provista"
        }
    ]
};


// 🔹 DataTable (Personal Policial / Lista)
let tablaData;

$(document).ready(function () {
    tablaData = $('#tbdataPersonal').DataTable({
        responsive: true,
        autoWidth: false,  // importante
        ajax: {
            url: '/api/v1/ApiPersonal/Lista',
            type: 'GET',
            datatype: 'json',
            beforeSend: function () {
                $(".card-body").LoadingOverlay("show"); // muestra overlay antes de la petición
            },
            complete: function () {
                $(".card-body").LoadingOverlay("hide"); // oculta overlay cuando termina la petición
            }
        },
        columns: [          
            { // Columna de enumeración
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + 1; // empieza en 1
                },
                orderable: false,
                searchable: false
            },
            {
                data: "urlImagen",
                render: function (data) {
                    // Si hay una imagen válida, la mostramos
                    if (data && data.trim() !== "") {
                        return `<img style="height:60px; width:60px; object-fit:cover;" src="${data}" class="rounded mx-auto d-block" />`;
                    } else {
                        // Si no hay imagen, dejamos el espacio vacío (sin error 404)
                        return `<div style="height:60px; width:60px; background-color:#f2f2f2; border-radius:8px; margin:auto;"></div>`;
                    }
                }
            },
            { data: "legajo" },
            { data: "apellidoYNombre" },
            { data: "grado" },
            { data: "funcion" },
            { data: "situacionRevista" },
            { data: null }, // columna de botones
            { data: "idPersonal", visible: false, searchable: false },
        ],
        columnDefs: [
            {
                targets: -2, // última columna
                orderable: false,
                searchable: false,
                className: "text-center",
                responsivePriority: 1, // nunca colapsar esta columna
                render: function () {
                    // botones directamente en el td
                    return `
                            <button class="btn btn-primary btn-editar btn-sm mr-1" title="Editar">
                                <i class="fas fa-pencil-alt"></i>
                            </button>
                             <button class="btn btn-danger btn-eliminar btn-sm mr-1" title="Trasladar">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                    `;
                }
            }
        ],
        order: [[2, "asc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_PersonalPolicial',
                exportOptions: { columns: [1, 2, 3, 4, 5, 6, 7] }
            },
            'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

    // Actualizar enumeración cada vez que se ordena o filtra
    tablaData.on('order.dt search.dt', function () {
        tablaData.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
});


// 🔹 Abrir modal para "Nuevo Personal"
$("#btnNuevoPersonal").click(function () {
    mostrarModalPersonal();
});

// 🔹 Mostrar Modal de Personal
function mostrarModalPersonal(modelo = MODELO_BASE_PERSONAL) {
    // 🔹 Datos generales
    $("#txtIdPersonal").val(modelo.idPersonal);
    $("#txtLegajo").val(modelo.legajo);
    $("#txtApellidoYNombre").val(modelo.apellidoYNombre);
    $("#cboGrado").val(modelo.grado);
    $("#txtChapa").val(modelo.chapa);
    $("#cboSexo").val(modelo.sexo);
    $("#cboFuncion").val(modelo.funcion);
    $("#cboHorario").val(modelo.horario);
    $("#cboRevista").val(modelo.situacionRevista);
    $("#txtFechaNacimiento").val(modelo.fechaNacimiento);
    $("#txtTelefono").val(modelo.telefono);
    $("#txtTelefonoEmergencia").val(modelo.telefonoEmergencia);
    $("#txtDNI").val(modelo.dni);
    $("#txtSubsidioSalud").val(modelo.subsidioSalud);
    $("#txtEstudios").val(modelo.estudiosCurs);
    $("#txtEstadoCivil").val(modelo.estadoCivil);
    $("#txtEspecialidad").val(modelo.especialidad);
    $("#txtAltaDivision").val(modelo.altaEnDivision);
    $("#txtAltaPolicia").val(modelo.altaEnPolicia);
    $("#txtDestinoAnterior").val(modelo.destinoAnterior);
    $("#txtEmail").val(modelo.email);
    $("#txtDetalles").val(modelo.detalles);

    // 🔹 Domicilio (si existe)
    const domicilio = (modelo.domicilios && modelo.domicilios.length > 0) ? modelo.domicilios[0] : {};
    $("#txtDomicilio").val(domicilio.calleBarrio || "");
    $("#txtLocalidad").val(domicilio.localidad || "");
    $("#txtCriaJurisdiccional").val(domicilio.comisariaJuris || "");
    $("#txtIdDomicilio").val(domicilio.idDomicilio || 0); // 🔹 asignar ID

    // 🔹 Arma (si existe)
    const arma = (modelo.armas && modelo.armas.length > 0) ? modelo.armas[0] : { marca: "No Provista", numeroSerie: "No Provista" };

    // 🔹 Guardar el IdArma en hidden
    $("#txtIdArma").val(arma.idArma || 0);

    if (arma.marca !== "No Provista") {
        // Caso: arma provista
        $("#chkArmaProvista").prop("checked", true);
        $("#txtArmaMarca").val(arma.marca).prop("disabled", false);
        $("#txtArmaNumeroSerie").val(arma.numeroSerie).prop("disabled", false);
    } else {
        // Caso: arma no provista
        $("#chkArmaProvista").prop("checked", false);
        $("#txtArmaMarca").val("No Provista").prop("disabled", true);
        $("#txtArmaNumeroSerie").val("No Provista").prop("disabled", true);
    }

    // 🔹 Evento al marcar el checkbox para habilitar los inputs
    $("#chkArmaProvista").off("change").on("change", function () {
        if ($(this).is(":checked")) {
            $("#txtArmaMarca, #txtArmaNumeroSerie").val("").prop("disabled", false);
        } else {
            $("#txtArmaMarca").val("No Provista").prop("disabled", true);
            $("#txtArmaNumeroSerie").val("No Provista").prop("disabled", true);
        }
    });


    //Imagen del Personal
    if (modelo.urlImagen && modelo.urlImagen.trim() !== "") {
        // Si el personal tiene imagen guardada, la mostramos
        $("#imgPersonalPolicial").attr("src", modelo.urlImagen);
    } else {
        // Si no tiene imagen, dejamos el src vacío (no mostramos nada)
        $("#imgPersonalPolicial").attr("src", "");
    }
    //Limpiamos siempre el input de archivo por si el usuario quiere subir otra
    $("#txtFotoPersonalPolicial").val("");

    //Abrir modal
    $("#modalPersonal").modal("show");
}


// Evento click del botón Nuevo Personal
$("#btnNuevoPersonal").click(function () {
    mostrarModalPersonal(); // ← usa el modelo base
});

// Evento click del botón Guardar Personal
$("#btnGuardarPersonal").click(async function () {

    // 🔹 Validación de campos obligatorios
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() === "");

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    // 🔹 Validación Domicilios
    if (!$("#txtDomicilio").val() || !$("#txtLocalidad").val() || !$("#txtCriaJurisdiccional").val()) {
        toastr.warning("", "Debe completar todos los campos de domicilio");
        return;
    }

    // 🔹 Construcción del modelo
    const modelo = structuredClone(MODELO_BASE_PERSONAL);

    modelo.idPersonal = parseInt($("#txtIdPersonal").val());
    modelo.legajo = $("#txtLegajo").val();
    modelo.apellidoYnombre = $("#txtApellidoYNombre").val();
    modelo.grado = $("#cboGrado").val();
    modelo.chapa = $("#txtChapa").val();
    modelo.sexo = $("#cboSexo").val();
    modelo.funcion = $("#cboFuncion").val();
    modelo.horario = $("#cboHorario").val();
    modelo.situacionRevista = $("#cboRevista").val();
    modelo.fechaNacimiento = $("#txtFechaNacimiento").val();
    modelo.telefono = $("#txtTelefono").val();
    modelo.telefonoEmergencia = $("#txtTelefonoEmergencia").val();
    modelo.dni = $("#txtDNI").val();
    modelo.subsidioSalud = $("#txtSubsidioSalud").val();
    modelo.estudiosCurs = $("#txtEstudios").val();
    modelo.estadoCivil = $("#txtEstadoCivil").val();
    modelo.especialidad = $("#txtEspecialidad").val();
    modelo.altaEnDivision = $("#txtAltaDivision").val();
    modelo.altaEnPolicia = $("#txtAltaPolicia").val();
    modelo.destinoAnterior = $("#txtDestinoAnterior").val();
    modelo.email = $("#txtEmail").val();
    modelo.detalles = $("#txtDetalles").val();

    // 🔹 Domicilios (array)
    modelo.domicilios = [
        {
            idDomicilio: parseInt($("#txtIdDomicilio").val()) || 0, // ← agregar ID
            calleBarrio: $("#txtDomicilio").val(),
            localidad: $("#txtLocalidad").val(),
            comisariaJuris: $("#txtCriaJurisdiccional").val()
        }
    ];

    // 🔹 Armas (array)
    modelo.armas = [];
    if ($("#chkArmaProvista").is(":checked")) {
        modelo.armas.push({
            idArma: parseInt($("#txtIdArma").val()) || 0, // ← agregar ID
            marca: $("#txtArmaMarca").val(),
            numeroSerie: $("#txtArmaNumeroSerie").val()
        });
    } else {
        modelo.armas.push({
            marca: "No Provista",
            numeroSerie: "No Provista"
        });
    }

    //foto
    const inputFoto = document.getElementById("txtFotoPersonalPolicial");
    const formData = new FormData();

    // Solo agregamos foto si el usuario seleccionó un archivo
    if (inputFoto.files.length > 0) {
        formData.append("foto", inputFoto.files[0]);
    }

    // Siempre enviar modelo como JSON
    formData.append("modelo", JSON.stringify(modelo));

    // 🔹 Loading overlay
    $("#modalPersonal").find("div.modal-content").LoadingOverlay("show");

    let url = modelo.idPersonal === 0
        ? "/api/v1/ApiPersonal/Crear"
        : "/api/v1/ApiPersonal/Editar";

    let method = modelo.idPersonal === 0 ? "POST" : "PUT";


    try {
        const response = await fetch(url, { method, body: formData });
        const responseJson = await response.json();

        $("#modalPersonal").find("div.modal-content").LoadingOverlay("hide");

        if (responseJson.estado) {
            if (modelo.idPersonal === 0) {
                tablaData.row.add(responseJson.objeto).draw(false);
                swal("Listo", "El Personal fue Creado", "success");
            } else {
                tablaData.row(filaSeleccionadaPersonal).data(responseJson.objeto).draw(false);
                filaSeleccionadaPersonal = null;
                swal("Listo", "El Personal fue Actualizado", "success");
            }
            $("#modalPersonal").modal("hide");
        } else {
            swal("Error", responseJson.mensaje, "error");
        }
    } catch (error) {
        $("#modalPersonal").find("div.modal-content").LoadingOverlay("hide");
        swal("Error", "Ocurrió un error al guardar el personal", "error");
        console.error(error);
    }
});


// Botón EDITAR para PERSONAL
let filaSeleccionadaPersonal;

$("#tbdataPersonal tbody").on("click", ".btn-editar", function () {

    // fila real (no child)
    filaSeleccionadaPersonal = $(this).closest("tr");

    // obtenemos el id del personal desde la fila
    const data = tablaData.row(filaSeleccionadaPersonal).data();
    const idPersonal = data.idPersonal;

    // hacemos fetch al back-end para traer datos completos
    $.ajax({
        type: "GET",
        url: `/api/v1/ApiPersonal/ObtenerPersonalParaEditar/${idPersonal}`, // 👈 esta es la ruta correcta
        success: function (personal) {
            mostrarModalPersonal(personal);
        },
        error: function (err) {
            console.error(err);
            alert("No se pudo obtener los datos del personal.");
        }
    });


});


// Botón TRASLADAR para PERSONAL
$("#tbdataPersonal tbody").on("click", ".btn-eliminar", function () {

    const filaSeleccionada = $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();
    const idPersonal = data.idPersonal;

    swal({
        title: "¿Estás seguro?",
        text: `¿Desea trasladar al personal "${data.apellidoYNombre}"?`,
        icon: "warning",
        buttons: {
            cancel: "Cancelar",
            confirm: {
                text: "Sí, trasladar",
                className: "btn-danger"
            }
        },
        dangerMode: true,
    }).then((respuesta) => {
        if (respuesta) {
            // Mostrar overlay de carga si lo tenés
            $(".showSweetAlert").LoadingOverlay("show");

            // Llamada al back-end para trasladar
            $.ajax({
                type: "PUT",
                url: `/api/v1/ApiPersonal/trasladar/${idPersonal}`,
                success: function (response) {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    if (response.estado) {
                        swal("Trasladado!", response.mensaje, "success");
                        // Recargar DataTable sin perder el estado
                        tablaData.ajax.reload(null, false);
                    } else {
                        swal("Error", response.mensaje, "error");
                    }
                },
                error: function (err) {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    console.error(err);
                    swal("Error", "Error al intentar trasladar al personal.", "error");
                }
            });
        }
    });

});




