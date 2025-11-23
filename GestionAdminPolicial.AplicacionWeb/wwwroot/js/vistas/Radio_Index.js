// Modelo base para crear/editar una radio
const MODELO_BASE_RADIO = {
    idRadio: 0,
    serieRadio: "",
    idUsuario: "",
    marcayModelo: "",
    estadoRadio: "",
    tipo: "",
    observaciones: "",
};

let tablaRadios;
let radioSeleccionada;

$(document).ready(function () {

    tablaRadios = $('#tbdataRadios').DataTable({
        responsive: true,
        autoWidth: false,
        serverSide: true,
        processing: true,
        ajax: {
            url: '/api/v1/ApiRadio/ListarPaginado',
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
            { // Enumeración dinámica
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + 1 + meta.settings._iDisplayStart;
                }
            },
            { data: 'serieRadio' },
            { data: 'marcayModelo' },
            { data: 'tipo' },
            { data: 'estadoRadio' },
            { data: 'observaciones' },
            {
                "data": "",
                "defaultContent":
                    '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
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
                text: '📊 Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Radios',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
            },
            'pageLength'
        ],
        language: { url: "/js/datatables/es-ES.json" }
    });
});

let filaSeleccionadaRadio;

$("#btnNuevaRadio").click(function () {
    mostrarModalRadio(); // abre usando MODELO_BASE_RADIO
});

function mostrarModalRadio(modelo = MODELO_BASE_RADIO) {

    $("#txtIdRadio").val(modelo.idRadio);
    $("#txtSerieRadio").val(modelo.serieRadio);
    $("#txtMarcaYModelo").val(modelo.marcayModelo);
    $("#cboTipo").val(modelo.tipo);
    $("#cboEstado").val(modelo.estadoRadio);
    $("#txtObservaciones").val(modelo.observaciones);

    // Abrir modal
    $("#modalData").modal("show");
}

// Evento Guardar radio
$("#btnGuardarRadio").click(async function () {

    // Validación de campos obligatorios
    const inputs = $("input.input-validar, select.input-validar").serializeArray();
    const vacios = inputs.filter(x => x.value.trim() === "");

    if (vacios.length > 0) {
        toastr.warning(`Debe completar el campo: "${vacios[0].name}"`);
        $(`input[name="${vacios[0].name}"], select[name="${vacios[0].name}"]`).focus();
        return;
    }

    // Construcción del modelo para enviar al backend
    const modelo = {
        idRadio: parseInt($("#txtIdRadio").val()) || 0,
        serieRadio: $("#txtSerieRadio").val().trim(),
        marcayModelo: $("#txtMarcaYModelo").val().trim(),
        tipo: $("#cboTipo").val(),
        estadoRadio: $("#cboEstado").val(),
        observaciones: $("#txtObservaciones").val().trim()
    };

    $("#modalData .modal-content").LoadingOverlay("show");

    // Crear o Editar
    const url = modelo.idRadio === 0
        ? "/api/v1/ApiRadio/Crear"
        : "/api/v1/ApiRadio/Editar";

    const method = modelo.idRadio === 0 ? "POST" : "PUT";

    try {

        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo)
        });

        const result = await response.json();

        $("#modalData .modal-content").LoadingOverlay("hide");

        if (result.estado) {

            const radio = result.objeto;

            if (modelo.idRadio === 0) {
                // Alta
                tablaRadios.row.add(radio).draw(false);
                swal("Listo", "La Radio fue registrada correctamente", "success");
            } else {
                // Edición
                tablaRadios.row(filaSeleccionadaRadio).data(radio).draw(false);
                filaSeleccionadaRadio = null;
                swal("Listo", "La Radio fue actualizada correctamente", "success");
            }

            $("#modalData").modal("hide");

        } else {
            swal("Error", result.mensaje, "error");
        }

    } catch (err) {
        $("#modalData .modal-content").LoadingOverlay("hide");
        console.error(err);
        swal("Error", "No se pudo registrar la radio", "error");
    }
});

$("#tbdataRadios tbody").on("click", ".btn-editar", function () {

    filaSeleccionadaRadio = $(this).closest("tr");

    const data = tablaRadios.row(filaSeleccionadaRadio).data();

    mostrarModalRadio(data);
});

// Evento Editar Radio
$("#tbdataRadios tbody").on("click", ".btn-editar", function () {

    filaSeleccionadaRadio = $(this).closest("tr");
    const data = tablaRadios.row(filaSeleccionadaRadio).data();

    if (!data) {
        console.error("No se pudo obtener los datos de la fila seleccionada");
        return;
    }

    const idRadio = data.idRadio;

    $.ajax({
        type: "GET",
        url: `/api/v1/ApiRadio/Obtener/${idRadio}`,
        success: function (response) {

            const radio = response.objeto;

            if (!radio) {
                swal("Error", "No se encontró la Radio solicitada", "error");
                return;
            }

            mostrarModalRadio(radio);
        },
        error: function (err) {
            swal("Error", "No se pudo recuperar información de la radio", "error");
            console.error(err);
        }
    });
});

// Botón ELIMINAR para RADIO
$("#tbdataRadios tbody").on("click", ".btn-eliminar", function () {

    // Fila seleccionada
    const filaSeleccionada = $(this).closest("tr");

    // Datos de la fila desde DataTables
    const data = tablaRadios.row(filaSeleccionada).data();
    const idRadio = data.idRadio;

    swal({
        title: "¿Estás seguro?",
        text: `¿Desea eliminar la Radio con Serie N° "${data.serieRadio}"?`,
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

            // Llamada al back-end
            $.ajax({
                type: "PATCH",
                url: `/api/v1/ApiRadio/Eliminar/${idRadio}`,
                success: function (response) {

                    $(".showSweetAlert").LoadingOverlay("hide");

                    if (response.estado) {
                        swal("¡Eliminada!", response.mensaje, "success");
                        // Recargar DataTable sin perder la página
                        tablaRadios.ajax.reload(null, false);
                    } else {
                        swal("Error", response.mensaje || "No se pudo eliminar la radio.", "error");
                    }
                },
                error: function (err) {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    console.error(err);
                    swal("Error", "Error al intentar eliminar la radio.", "error");
                }
            });
        }
    });
});

let tablaRadiosEliminadas;

$("#btnRadiosEliminadas").on("click", function () {

    if (!$.fn.DataTable.isDataTable('#tbRadiosEliminadas')) {

        tablaRadiosEliminadas = $('#tbRadiosEliminadas').DataTable({
            responsive: true,
            autoWidth: false,
            serverSide: true,
            processing: true,
            ajax: {
                url: '/api/v1/ApiRadio/ListarPaginadoEliminadas', // tu endpoint
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
                { data: 'serieRadio' },
                { data: 'marcayModelo' },
                { data: 'tipo' },
                { data: 'estadoRadio', render: d => d ? d : '-' },
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
                    text: '📊 Exportar Excel',
                    extend: 'excelHtml5',
                    title: '',
                    filename: 'Reporte_Radios_Eliminadas',
                    exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6] }
                },
                'pageLength'
            ],
            language: { url: "/js/datatables/es-ES.json" }
        });

    } else {
        tablaRadiosEliminadas.ajax.reload();
    }

    $("#modalRadiosEliminadas").modal("show");
});


