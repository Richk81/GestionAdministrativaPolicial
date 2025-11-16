// 🔹 Modelo base para crear/editar un vehículo
const MODELO_BASE_VEHICULO = {
    idVehiculo: 0,
    tuc: "",
    tipo: "",
    dominio: "",
    marcayModelo: "",
    motorNumero: "",
    chasisNumero: "",
    añoFabricacion: "",
    estadoVehiculo: "",
    lugarDeReparacion: "",
    observaciones: "",
    kmActual: "",
    ultimoService: ""
};

let tablaVehiculos;
let vehiculoSeleccionado;

$(document).ready(function () {

    tablaVehiculos = $('#tbdataVehiculos').DataTable({
        responsive: true,
        autoWidth: false,
        serverSide: true,
        processing: true,
        ajax: {
            url: '/api/v1/ApiVehiculo/ListarPaginado',
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                return JSON.stringify(d);
            },
            dataSrc: 'data',
            beforeSend: function () {
                $(".card-body").LoadingOverlay("show");
            },
            complete: function () {
                $(".card-body").LoadingOverlay("hide");
            },
            error: function (xhr, status, error) {
                $(".card-body").LoadingOverlay("hide");
                console.error("Error al cargar los datos:", error);
            }
        },
        columns: [
            { // Enumeración
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + 1 + meta.settings._iDisplayStart;
                }
            },
            { data: 'tuc' },
            { data: 'tipo' },
            { data: 'marcayModelo' },
            { data: 'dominio' },
            { data: 'estadoVehiculo' },
            {
                "data": "",
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "90px"
            }
        ],
        order: [[0, "asc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Vehiculos',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6] }
            },
            'pageLength'
        ],
        language: { url: "/js/datatables/es-ES.json" }

    });

});

let filaSeleccionadaVehiculo;

// 🔹 Evento click del botón Nuevo Vehículo
$("#btnNuevoVehiculo").click(function () {
    mostrarModalVehiculo(); // ← sin parámetros toma el modelo base
});

// 🔹 Función para mostrar modal con valores cargados
function mostrarModalVehiculo(modelo = MODELO_BASE_VEHICULO) {

    $("#txtIdVehiculo").val(modelo.idVehiculo);
    $("#txtTuc").val(modelo.tuc);
    $("#cboTipo").val(modelo.tipo);
    $("#txtMarcaYModelo").val(modelo.marcayModelo);
    $("#txtDominio").val(modelo.dominio);
    $("#txtAñoFabricacion").val(modelo.añoFabricacion);
    $("#txtMotor").val(modelo.motorNumero);
    $("#txtChasis").val(modelo.chasisNumero);
    $("#cboEstado").val(modelo.estadoVehiculo);
    $("#cboLugarDeReparacion").val(modelo.lugarDeReparacion);
    $("#txtObservaciones").val(modelo.observaciones);
    // Abrir modal
    $("#modalData").modal("show");
}

// 🔹 Evento Guardar Vehículo
$("#btnGuardarVehiculo").click(async function () {

    // Validación de campos obligatorios
    const inputs = $("input.input-validar").serializeArray();
    const vacios = inputs.filter(x => x.value.trim() === "");

    if (vacios.length > 0) {
        toastr.warning(`Debe completar el campo: "${vacios[0].name}"`);
        $(`input[name="${vacios[0].name}"]`).focus();
        return;
    }

    // Construcción del modelo
    const modelo = {
        idVehiculo: parseInt($("#txtIdVehiculo").val()) || 0,
        tuc: $("#txtTuc").val().trim(),
        tipo: $("#cboTipo").val(),
        marcaYModelo: $("#txtMarcaYModelo").val().trim(),
        dominio: $("#txtDominio").val().trim(),
        añoFabricacion: $("#txtAñoFabricacion").val(),
        motorNumero: $("#txtMotor").val().trim(),
        chasisNumero: $("#txtChasis").val().trim(),
        estadoVehiculo: $("#cboEstado").val(),
        lugarDeReparacion: $("#cboLugarDeReparacion").val(),
        observaciones: $("#txtObservaciones").val().trim()
    };

    // Loading overlay
    $("#modalData .modal-content").LoadingOverlay("show");

    // Determinar si es creación o edición
    const url = modelo.idVehiculo === 0
        ? "/api/v1/ApiVehiculo/Crear"
        : "/api/v1/ApiVehiculo/Editar";

    const method = modelo.idVehiculo === 0 ? "POST" : "PUT";

    try {
        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
        });

        const result = await response.json();

        $("#modalData .modal-content").LoadingOverlay("hide");

        if (result.estado) {
            const vehiculo = result.objeto;

            if (modelo.idVehiculo === 0) {
                tablaVehiculos.row.add(vehiculo).draw(false);
                swal("Listo", "El vehículo fue registrado correctamente", "success");
            } else {
                tablaVehiculos.row(filaSeleccionadaVehiculo).data(vehiculo).draw(false);
                filaSeleccionadaVehiculo = null;
                swal("Listo", "El vehículo fue actualizado correctamente", "success");
            }

            $("#modalData").modal("hide");
        } else {
            swal("Error", result.mensaje, "error");
        }

    } catch (err) {
        $("#modalData .modal-content").LoadingOverlay("hide");
        console.error(err);
        swal("Error", "No se pudo registrar el vehículo", "error");
    }
});

// 🔹 Evento Editar Vehículo
$("#tbdataVehiculos tbody").on("click", ".btn-editar", function () {

    filaSeleccionadaVehiculo = $(this).closest("tr");
    const data = tablaVehiculos.row(filaSeleccionadaVehiculo).data();

    if (!data) {
        console.error("No se pudo obtener los datos de la fila seleccionada");
        return;
    }

    const idVehiculo = data.idVehiculo;

    $.ajax({
        type: "GET",
        url: `/api/v1/ApiVehiculo/Obtener/${idVehiculo}`,
        success: function (response) {

            const vehiculo = response.objeto;

            if (!vehiculo) {
                swal("Error", "No se encontró el vehículo solicitado", "error");
                return;
            }

            mostrarModalVehiculo(vehiculo);
        },
        error: function (err) {
            swal("Error", "No se pudo recuperar información del vehículo", "error");
            console.error(err);
        }
    });
});

// Botón ELIMINAR para VEHÍCULO
$("#tbdataVehiculos tbody").on("click", ".btn-eliminar", function () {

    // Obtenemos la fila seleccionada
    const filaSeleccionada = $(this).closest("tr");

    // Obtenemos los datos de esa fila desde DataTables
    const data = tablaVehiculos.row(filaSeleccionada).data();
    const idVehiculo = data.idVehiculo;

    swal({
        title: "¿Estás seguro?",
        text: `¿Desea eliminar el vehículo con TUC N° "${data.tuc}"?`,
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
            // Mostrar overlay de carga
            $(".showSweetAlert").LoadingOverlay("show");

            // Llamada al back-end para eliminar el vehículo
            $.ajax({
                type: "PATCH",
                url: `/api/v1/ApiVehiculo/Eliminar/${idVehiculo}`,
                success: function (response) {
                    $(".showSweetAlert").LoadingOverlay("hide");

                    if (response.estado) {
                        swal("¡Eliminado!", response.mensaje, "success");
                        // Recargar DataTable sin perder la página actual
                        tablaVehiculos.ajax.reload(null, false);
                    } else {
                        swal("Error", response.mensaje || "No se pudo eliminar el vehículo.", "error");
                    }
                },
                error: function (err) {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    console.error(err);
                    swal("Error", "Error al intentar eliminar el vehículo.", "error");
                }
            });
        }
    });
});


let tablaVehiculosEliminados;

$("#btnVehiculosEliminados").on("click", function () {

    if (!$.fn.DataTable.isDataTable('#tbVehiculosEliminados')) {

        tablaVehiculosEliminados = $('#tbVehiculosEliminados').DataTable({
            responsive: true,
            autoWidth: false,
            serverSide: true,
            processing: true,
            ajax: {
                url: '/api/v1/ApiVehiculo/ListarPaginadoEliminados', // tu endpoint
                type: 'POST',
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
                dataSrc: 'data',
                beforeSend: function () {
                    $(".card-body").LoadingOverlay("show");
                },
                complete: function () {
                    $(".card-body").LoadingOverlay("hide");
                },
                error: function (xhr, status, error) {
                    $(".card-body").LoadingOverlay("hide");
                    console.error("Error al cargar los datos:", error);
                }
            },
            columns: [
                { data: null, render: (data, type, row, meta) => meta.row + 1 + meta.settings._iDisplayStart },
                { data: 'tuc' },
                { data: 'marcayModelo' },
                { data: 'tipo' },
                { data: 'estadoVehiculo', render: d => d ? d : '-' },
                { data: 'observaciones', render: d => d ? d : '-' },
                {
                    data: 'fechaEliminacion',
                    render: d => d
                        ? new Date(d).toLocaleDateString('es-AR', { day: 'numeric', month: 'long', year: 'numeric' })
                        : '-'
                }
            ],
            order: [[0, "asc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: '',
                    filename: 'Reporte_Vehiculos_Eliminados',
                    exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6] }
                },
                'pageLength'
            ],
            language: { url: "/js/datatables/es-ES.json" }
        });

    } else {
        tablaVehiculosEliminados.ajax.reload();
    }

    $("#modalVehiculosEliminados").modal("show");
});
