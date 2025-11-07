// 🔹 Modelo base para crear un nuevo chaleco
const MODELO_BASE_CHALECO = {
    idChaleco: 0,
    serieChaleco: "",
    marcaYmodelo: "",
    talle: "",
    anoFabricacion: "",
    anoVencimiento: "",
    estadoChaleco: "",
    observaciones: ""
};

let tablaChalecos;
let tablaPersonalModal;
let chalecoSeleccionado;

$(document).ready(function () {

    // ========================
    // Loader
    // ========================
    function showLoader() { $("#overlayLoader").show(); }
    function hideLoader() { $("#overlayLoader").hide(); }

    // Tabla principal de chalecos
    tablaChalecos = $('#tbdataChalecos').DataTable({
        responsive: true,
        autoWidth: false,
        ajax: {
            url: '/api/v1/ApiChaleco/Lista',
            type: 'GET',
            datatype: 'json',
            dataSrc: 'data',
            beforeSend: showLoader,
            complete: hideLoader
        },
        columns: [
            { // Enumeración
                data: null,
                render: (data, type, row, meta) => meta.row + 1
            },
            { data: 'serieChaleco' },
            { data: 'marcaYmodelo' },
            { data: 'talle' },
            { // Asignado a
                data: 'idPersonalNavigation',
                render: function (data) {
                    if (data && data.apellidoYnombre) {
                        return data.apellidoYnombre; // Chaleco asignado → muestra nombre
                    } else {
                        // Chaleco sin asignar → muestra "DISPONIBLE" en verde
                        return '<span class="badge bg-success">Disponible</span>';
                    }
                }
            },
            { // Grado
                data: 'idPersonalNavigation',
                render: (data) => data && data.grado ? data.grado : '-'
            },
            { // Legajo
                data: 'idPersonalNavigation',
                render: (data) => data && data.legajo ? data.legajo : '-'
            },
            { // Acciones dinámicas
                data: 'idPersonalNavigation',
                render: function (data, type, row) {
                    let botones = '';

                    // Si está asignado → mostrar solo "Desasignar"
                    if (data && data.idPersonal) {
                        botones = `
                            <button class="btn btn-warning btn-desasignar btn-sm" data-id="${row.idChaleco}">
                                <i class="fas fa-times"></i> Devolución
                            </button>`;
                    } else {
                        // Si no está asignado → mostrar "Editar", "Eliminar" y "Asignar"
                        botones = `
                            <button class="btn btn-primary btn-editar btn-sm mr-2">
                                <i class="fas fa-pencil-alt"></i>
                            </button>
                            <button class="btn btn-danger btn-eliminar btn-sm mr-2">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                            <button class="btn btn-info btn-asignar btn-sm" data-id="${row.idChaleco}">
                                <i class="fas fa-user-plus"></i> Asignar
                            </button>`;
                    }
                    return botones;
                },
                orderable: false,
                searchable: false,
                width: "200px"
            }
        ],
        order: [[0, "asc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Chalecos',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6] }
            },
            'pageLength'
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" }
    });

    // --- Búsqueda híbrida: local + base de datos ---
    $('#tbdataChalecos_filter input')
        .off() // eliminamos el evento original de búsqueda automática
        .on('keyup', function (e) {
            const valorBusqueda = this.value.trim();

            // Si no hay texto, restauramos la tabla completa
            if (valorBusqueda.length === 0) {
                tablaChalecos.ajax.url('/api/v1/ApiChaleco/Lista').load();
                return;
            }

            // Si presiona ENTER, buscar directamente en la base de datos
            if (e.keyCode === 13) {
                $.ajax({
                    url: '/api/v1/ApiChaleco/BuscarPorNumeroSerie/' + encodeURIComponent(valorBusqueda),
                    type: 'GET',
                    dataType: 'json',
                    beforeSend: showLoader,
                    success: function (respuesta) {
                        hideLoader();

                        if (respuesta && respuesta.data) {
                            // Limpiamos la tabla y mostramos solo el chaleco encontrado
                            tablaChalecos.clear().rows.add([respuesta.data]).draw();
                        } else {
                            Swal.fire({
                                icon: 'warning',
                                title: 'No encontrado',
                                text: 'No se encontró ningún chaleco con ese número de serie.'
                            });
                        }
                    },
                    error: function () {
                        hideLoader();
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo realizar la búsqueda en la base de datos.'
                        });
                    }
                });
            } else {
                // Mientras escribe (sin presionar Enter), búsqueda local
                tablaChalecos.search(valorBusqueda).draw();
            }
        });


    // Evento click del botón Nuevo Chaleco
    $("#btnNuevoChaleco").click(function () {
        mostrarModalChaleco(); // ← usa el modelo base
    });

    // Función para mostrar el modal de Chaleco
    function mostrarModalChaleco(modelo = MODELO_BASE_CHALECO) {

        // Asignar los valores a los inputs
        $("#txtIdChaleco").val(modelo.idChaleco);
        $("#txtNumeroSerie").val(modelo.serieChaleco);
        $("#txtMarcayModelo").val(modelo.marcaYmodelo);
        $("#cboTalle").val(modelo.talle);
        $("#txtAnoFabricacion").val(modelo.anoFabricacion);
        $("#txtAnoVencimiento").val(modelo.anoVencimiento);
        $("#cboEstado").val(modelo.estadoChaleco);
        $("#txtObservaciones").val(modelo.observaciones);

        // Abrir modal
        $("#modalChaleco").modal("show");
    }

    // 🔹 Evento click del botón Guardar Chaleco
    $("#btnGuardarChaleco").click(async function () {

        // 🔸 Validación de campos obligatorios
        const inputs = $("input.input-validar").serializeArray();
        const inputs_sin_valor = inputs.filter((item) => item.value.trim() === "");

        if (inputs_sin_valor.length > 0) {
            const mensaje = `Debe completar el campo: "${inputs_sin_valor[0].name}"`;
            toastr.warning("", mensaje);
            $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
            return;
        }

        // Construcción del modelo
        const modelo = {
            idChaleco: parseInt($("#txtIdChaleco").val()) || 0,
            serieChaleco: $("#txtNumeroSerie").val().trim(),
            marcaYmodelo: $("#txtMarcayModelo").val().trim(),
            talle: $("#cboTalle").val(),
            anoFabricacion: $("#txtAnoFabricacion").val(),
            anoVencimiento: $("#txtAnoVencimiento").val(),
            estadoChaleco: $("#cboEstado").val(),
            observaciones: $("#txtObservaciones").val().trim()
        };

        // 🔹 Loading overlay
        $("#modalChaleco").find("div.modal-content").LoadingOverlay("show");

        // 🔸 Determinar si es creación o edición
        const url = modelo.idChaleco === 0
            ? "/api/v1/ApiChaleco/Crear"
            : "/api/v1/ApiChaleco/Editar";

        const method = modelo.idChaleco === 0 ? "POST" : "PUT";

        try {
            const response = await fetch(url, {
                method,
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(modelo)
            });

            const responseJson = await response.json();

            $("#modalChaleco").find("div.modal-content").LoadingOverlay("hide");

            // Validamos GenericResponse
            if (responseJson.estado) {
                const chaleco = responseJson.objeto;

                if (modelo.idChaleco === 0) {
                    tablaChalecos.row.add(chaleco).draw(false);
                    swal("Listo", "El Chaleco fue Creado", "success");
                } else {
                    tablaChalecos.row(filaSeleccionadaChaleco).data(chaleco).draw(false);
                    filaSeleccionadaChaleco = null;
                    swal("Listo", "El Chaleco fue Actualizado", "success");
                }

                $("#modalChaleco").modal("hide");
            } else {
                swal("Error", responseJson.mensaje, "error");
            }
        } catch (error) {
            $("#modalChaleco").find("div.modal-content").LoadingOverlay("hide");
            swal("Error", "Ocurrió un error al guardar el Chaleco", "error");
            console.error(error);
        }
    });

    // 🔹 Variable para guardar la fila seleccionada
    let filaSeleccionadaChaleco;

    // 🔹 Evento click del botón "Editar"
    $("#tbdataChalecos tbody").on("click", ".btn-editar", function () {

        // Obtenemos la fila real (evitamos filas "child" de DataTables)
        filaSeleccionadaChaleco = $(this).closest("tr");

        // Obtenemos los datos de esa fila desde DataTables
        const data = tablaChalecos.row(filaSeleccionadaChaleco).data();
        const idChaleco = data.idChaleco;

        // 🔸 Llamada al backend para obtener los datos completos
        $.ajax({
            type: "GET",
            url: `/api/v1/ApiChaleco/Obtener/${idChaleco}`, // coincide con tu endpoint
            success: function (response) {
                // ✅ Tu backend devuelve GenericResponse<VMChaleco>
                const chaleco = response.objeto;

                if (!chaleco) {
                    swal("Error", "No se encontró el chaleco solicitado.", "error");
                    return;
                }

                // Cargamos los valores en el modal
                $("#txtIdChaleco").val(chaleco.idChaleco);
                $("#txtNumeroSerie").val(chaleco.serieChaleco);
                $("#txtMarcayModelo").val(chaleco.marcaYmodelo);
                $("#cboTalle").val(chaleco.talle);
                $("#txtAnoFabricacion").val(chaleco.anoFabricacion);
                $("#txtAnoVencimiento").val(chaleco.anoVencimiento);
                $("#cboEstado").val(chaleco.estadoChaleco);
                $("#txtObservaciones").val(chaleco.observaciones);

                // Abrimos el modal
                $("#modalChaleco").modal("show");
            },
            error: function (err) {
                console.error(err);
                swal("Error", "No se pudo obtener los datos del chaleco.", "error");
            }
        });
    });




    // ========================
    // Asignar chaleco (abrir modal)
    // ========================
    $('#tbdataChalecos tbody').on('click', '.btn-asignar', function () {
        chalecoSeleccionado = $(this).data('id');
        $('#modalAsignar').modal('show');

        if (!$.fn.DataTable.isDataTable('#tbPersonalModal')) {
            tablaPersonalModal = $('#tbPersonalModal').DataTable({
                responsive: true,
                autoWidth: false,
                pageLength: 10,
                ajax: {
                    url: '/api/v1/ApiPersonal/Lista',
                    type: 'GET',
                    datatype: 'json',
                    dataSrc: 'data',
                    beforeSend: showLoader,
                    complete: hideLoader
                },
                columns: [
                    { data: null, render: (data, type, row, meta) => meta.row + 1 },
                    { data: 'legajo' },
                    { data: 'apellidoYNombre' },
                    { data: 'grado' },
                    {
                        data: null,
                        defaultContent: '<button class="btn btn-sm btn-success btn-seleccionar"><i class="fas fa-check"></i> Seleccionar</button>',
                        orderable: false,
                        searchable: false
                    }
                ],
                dom: "Bfrtip",
                buttons: ['pageLength'],
                language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" }
            });
        } else {
            tablaPersonalModal.ajax.reload();
        }
    });

    // ========================
    // Seleccionar personal en el modal
    // ========================
    $('#tbPersonalModal tbody').on('click', '.btn-seleccionar', function () {
        const data = tablaPersonalModal.row($(this).parents('tr')).data();
        const idPersonal = data.idPersonal;

        swal({
            title: "¿Estás seguro?",
            text: `¿Desea asignar este chaleco al personal "${data.apellidoYNombre}"?`,
            icon: "warning",
            buttons: {
                cancel: "Cancelar",
                confirm: {
                    text: "Sí, asignar",
                    className: "btn-success"
                }
            },
            dangerMode: true,
        }).then((respuesta) => {
            if (respuesta) {
                // Mostrar overlay de carga dentro del modal (opcional)
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/api/v1/ApiChaleco/Asignar/${chalecoSeleccionado}/${idPersonal}`, { method: 'PATCH' })
                    .then(res => res.json())
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");

                        if (response.estado) {
                            swal("¡Asignado!", `El chaleco fue asignado correctamente a ${data.apellidoYNombre}.`, "success");
                            $('#modalAsignar').modal('hide');
                            tablaChalecos.ajax.reload(null, false);
                        } else {
                            swal("Error", response.mensaje || "No se pudo asignar el chaleco.", "error");
                        }
                    })
                    .catch(error => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        console.error(error);
                        swal("Error", "Ocurrió un error al intentar asignar el chaleco.", "error");
                    });
            }
        });
    });

    // ========================
    // Desasignar chaleco
    // ========================
    $('#tbdataChalecos tbody').on('click', '.btn-desasignar', function () {
        const idChaleco = $(this).data('id');

        swal({
            title: "¿Estás seguro?",
            text: "¿Desea realizar la devolución de este chaleco?",
            icon: "warning",
            buttons: {
                cancel: "Cancelar",
                confirm: {
                    text: "Sí, devolver",
                    className: "btn-danger"
                }
            },
            dangerMode: true,
        }).then((respuesta) => {
            if (respuesta) {
                // Mostrar overlay de carga
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/api/v1/ApiChaleco/Desasignar/${idChaleco}`, { method: 'PATCH' })
                    .then(res => res.json())
                    .then(data => {
                        $(".showSweetAlert").LoadingOverlay("hide");

                        if (data.estado) {
                            swal("¡Devuelto!", "Se realizó la devolución del Chaleco correctamente.", "success");
                            tablaChalecos.ajax.reload(null, false);
                        } else {
                            swal("Error", data.mensaje || "No se pudo desasignar el chaleco.", "error");
                        }
                    })
                    .catch(err => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        console.error(err);
                        swal("Error", "Error en la comunicación con el servidor.", "error");
                    });
            }
        });
    });


 
    // Botón ELIMINAR para CHALECO
    $("#tbdataChalecos tbody").on("click", ".btn-eliminar", function () {

        // Obtenemos la fila seleccionada
        const filaSeleccionada = $(this).closest("tr");

        // Obtenemos los datos de esa fila desde DataTables
        const data = tablaChalecos.row(filaSeleccionada).data();
        const idChaleco = data.idChaleco;

        swal({
            title: "¿Estás seguro?",
            text: `¿Desea eliminar el chaleco con número de serie "${data.serieChaleco}"?`,
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

                // Llamada al back-end para eliminar el chaleco
                $.ajax({
                    type: "PATCH",
                    url: `/api/v1/ApiChaleco/Eliminar/${idChaleco}`,
                    success: function (response) {
                        $(".showSweetAlert").LoadingOverlay("hide");

                        if (response.estado) {
                            swal("¡Eliminado!", response.mensaje, "success");
                            // 🔄 Recargar DataTable sin perder el estado actual
                            tablaChalecos.ajax.reload(null, false);
                        } else {
                            swal("Error", response.mensaje || "No se pudo eliminar el chaleco.", "error");
                        }
                    },
                    error: function (err) {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        console.error(err);
                        swal("Error", "Error al intentar eliminar el chaleco.", "error");
                    }
                });
            }
        });
    });


});

