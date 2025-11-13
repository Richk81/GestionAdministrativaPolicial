const MODELO_BASE = {
    idReporte: 0,
    idRecurso: "",
    tipoRecurso: "",
    accion: "",
    usuario: "",
    fechaAccion: "",
    observaciones: ""
};

let tablaReportes;

$(document).ready(function () {

    tablaReportes = $('#tbdataReportes').DataTable({
        responsive: true,
        autoWidth: false,
        serverSide: true,   // Paginación y filtrado del lado servidor
        processing: true,
        ajax: {
            url: '/api/v1/ApiReportes/ListarPaginado',
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                console.log("📤 Enviando request al backend:");
                console.log(JSON.stringify(d, null, 2));
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
                console.error("❌ Error al cargar los datos:", error);
            }
        },

        columns: [
            { data: "idReporte", visible: false }, // Oculto
            { data: "idRecurso", visible: false }, // Oculto
            { // Enumeración
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1; // Enumeración
                },
                orderable: false,
                searchable: false,
                className: "text-center"
            },

            { data: "tipoRecurso", defaultContent: "—" },
            { data: "accion", defaultContent: "—" },

            {
                data: "idUsuarioNavigation.nombre",
                defaultContent: "—"
            },
            {
                data: "fechaAccion",
                render: function (data) {
                    if (!data) return "—";
                    const fecha = new Date(data);
                    return fecha.toLocaleString("es-AR", {
                        day: '2-digit', month: '2-digit', year: 'numeric',
                        hour: '2-digit', minute: '2-digit'
                    });
                }
            },
            {
                data: "observaciones",
                render: function (data) {
                    return data ? data : "—";
                }
            }
        ],

        order: [[6, "desc"]], // Orden por fecha
        dom: "Bfrtip",
        buttons: [
            {
                text: '📊 Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_Acciones',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6, 7]
                }
            },
            'pageLength'
        ],

        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });
});
