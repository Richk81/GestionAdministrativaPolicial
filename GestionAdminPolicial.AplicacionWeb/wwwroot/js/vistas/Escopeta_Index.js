// Modelo base para crear/editar una escopeta
const MODELO_BASE_ESCOPETA = {
    idEscopeta: 0,
    serieEscopeta: "",
    idUsuario: "",
    marcayModelo: "",
    estadoEscopeta: "",
    observaciones: "",
};

let tablaEscopetas;
let escopetaSeleccionada;

$(document).ready(function () {

    tablaEscopetas = $('#tbdataEscopeta').DataTable({
        responsive: true,
        autoWidth: false,
        serverSide: true,
        processing: true,
        ajax: {
            url: '/api/v1/ApiEscopeta/ListarPaginado',
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
            { data: 'serieEscopeta' },
            { data: 'marcayModelo' },
            { data: 'estadoEscopeta' },
            {
                data: 'observaciones',
                render: d => d ? d : '-'
            },
            {
                data: "",
                defaultContent:
                    '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                orderable: false,
                searchable: false,
                width: "90px"
            }
        ],
        order: [[0, "asc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: '📊 Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Escopetas',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
            },
            'pageLength'
        ],
        language: { url: "/js/datatables/es-ES.json" }

    });

});

let filaSeleccionadaEscopeta;

// Botón Nueva Escopeta
$("#btnNuevaEscopeta").click(function () {
    mostrarModalEscopeta(); // ← abrir modal en blanco
});

function mostrarModalEscopeta(modelo = MODELO_BASE_ESCOPETA) {

    $("#txtIdEscopeta").val(modelo.idEscopeta);
    $("#txtSerie").val(modelo.serieEscopeta);
    $("#txtMarcaYModelo").val(modelo.marcayModelo);
    $("#cboEstado").val(modelo.estadoEscopeta);
    $("#txtObservaciones").val(modelo.observaciones);

    $("#modalData").modal("show");
}

// Evento Guardar Vehículo
$("#btnGuardarEscopeta").click(async function () {

    // Validación de campos obligatorios
    const inputs = $("input.input-validar, select.input-validar").serializeArray();
    const vacios = inputs.filter(x => x.value.trim() === "");

    if (vacios.length > 0) {
        toastr.warning(`Debe completar el campo: "${vacios[0].name}"`);
        $(`input[name="${vacios[0].name}"], select[name="${vacios[0].name}"]`).focus();
        return;
    }

    // Construcción del modelo
    const modelo = {
        idEscopeta: parseInt($("#txtIdEscopeta").val()) || 0,
        serieEscopeta: $("#txtSerie").val().trim(),
        marcayModelo: $("#txtMarcaYModelo").val().trim(),
        estadoEscopeta: $("#cboEstado").val(),
        observaciones: $("#txtObservaciones").val().trim()
    };

    // Loading dentro del modal
    $("#modalData .modal-content").LoadingOverlay("show");

    // Detectar creación o edición
    const url = modelo.idEscopeta === 0
        ? "/api/v1/ApiEscopeta/Crear"
        : "/api/v1/ApiEscopeta/Editar";

    const method = modelo.idEscopeta === 0 ? "POST" : "PUT";

    try {

        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
        });

        const result = await response.json();

        $("#modalData .modal-content").LoadingOverlay("hide");

        if (result.estado) {

            const escopeta = result.objeto;

            if (modelo.idEscopeta === 0) {
                // Nueva escopeta → agregar fila
                tablaEscopetas.row.add(escopeta).draw(false);
                swal("Listo", "La Escopeta fue registrada correctamente", "success");

            } else {
                // Editar escopeta → actualizar fila
                tablaEscopetas.row(filaSeleccionadaEscopeta).data(escopeta).draw(false);
                filaSeleccionadaEscopeta = null;
                swal("Listo", "La Escopeta fue actualizada correctamente", "success");
            }

            $("#modalData").modal("hide");

        } else {
            swal("Error", result.mensaje, "error");
        }

    } catch (err) {

        $("#modalData .modal-content").LoadingOverlay("hide");
        console.error(err);
        swal("Error", "No se pudo registrar la escopeta", "error");
    }
});

// Evento Editar Escopeta
$("#tbdataEscopeta tbody").on("click", ".btn-editar", function () {

    filaSeleccionadaEscopeta = $(this).closest("tr");
    const data = tablaEscopetas.row(filaSeleccionadaEscopeta).data();

    if (!data) {
        console.error("No se pudo obtener los datos de la fila seleccionada");
        return;
    }

    const idEscopeta = data.idEscopeta;

    $.ajax({
        type: "GET",
        url: `/api/v1/ApiEscopeta/Obtener/${idEscopeta}`,
        success: function (response) {

            const escopeta = response.objeto;

            if (!escopeta) {
                swal("Error", "No se encontró la Escopeta solicitada", "error");
                return;
            }

            mostrarModalEscopeta(escopeta);
        },
        error: function (err) {
            swal("Error", "No se pudo recuperar información de la Escopeta", "error");
            console.error(err);
        }
    });
});

// Botón ELIMINAR para ESCOPETA
$("#tbdataEscopeta tbody").on("click", ".btn-eliminar", function () {

    // Obtenemos la fila seleccionada
    const filaSeleccionada = $(this).closest("tr");

    // Obtenemos los datos de esa fila desde DataTables
    const data = tablaEscopetas.row(filaSeleccionada).data();
    const idEscopeta = data.idEscopeta;

    swal({
        title: "¿Estás seguro?",
        text: `¿Desea eliminar la escopeta con N° de Serie "${data.serieEscopeta}"?`,
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

            // Mostrar overlay de carga dentro del SweetAlert
            $(".showSweetAlert").LoadingOverlay("show");

            // Llamada al back-end para eliminar la escopeta
            $.ajax({
                type: "PATCH",
                url: `/api/v1/ApiEscopeta/Eliminar/${idEscopeta}`,
                success: function (response) {

                    $(".showSweetAlert").LoadingOverlay("hide");

                    if (response.estado) {
                        swal("¡Eliminada!", response.mensaje, "success");

                        // Recargar DataTable sin perder página actual
                        tablaEscopetas.ajax.reload(null, false);
                    } else {
                        swal("Error", response.mensaje || "No se pudo eliminar la escopeta.", "error");
                    }
                },
                error: function (err) {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    console.error(err);
                    swal("Error", "Error al intentar eliminar la escopeta.", "error");
                }
            });
        }
    });
});

let tablaEscopetasEliminadas;

$("#btnEscopetasEliminadas").on("click", function () {

    if (!$.fn.DataTable.isDataTable('#tbEscopetasEliminadas')) {

        tablaEscopetasEliminadas = $('#tbEscopetasEliminadas').DataTable({
            responsive: true,
            autoWidth: false,
            serverSide: true,
            processing: true,
            ajax: {
                url: '/api/v1/ApiEscopeta/ListarPaginadoEliminadas', // tu endpoint
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
                {
                    data: null,
                    render: (data, type, row, meta) => meta.row + 1 + meta.settings._iDisplayStart
                },
                { data: 'serieEscopeta' },
                { data: 'marcayModelo' },
                { data: 'estadoEscopeta', render: d => d ? d : '-' },
                { data: 'observaciones', render: d => d ? d : '-' },
                {
                    data: 'fechaEliminacion',
                    render: d => d
                        ? new Date(d).toLocaleDateString('es-AR', {
                            day: 'numeric',
                            month: 'long',
                            year: 'numeric'
                        })
                        : '-'
                }
            ],
            order: [[0, "asc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: '📊 Exportar Excel',
                    extend: 'excelHtml5',
                    title: '',
                    filename: 'Reporte_Escopetas_Eliminadas',
                    exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
                },
                'pageLength'
            ],
            language: { url: "/js/datatables/es-ES.json" }
        });

    } else {
        tablaEscopetasEliminadas.ajax.reload();
    }

    $("#modalEscopetasEliminadas").modal("show");
});


