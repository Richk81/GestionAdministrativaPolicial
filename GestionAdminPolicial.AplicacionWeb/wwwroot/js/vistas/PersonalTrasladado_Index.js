const MODELO_BASE_PERSONAL_Trasladado = {
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
    fechaEliminacion: "",
    domicilios: [{ calleBarrio: "", localidad: "", comisariaJuris: "" }],
    armas: [{ marca: "No Provista", numeroSerie: "No Provista" }]
};

//Función para formatear la fecha de Traslado
function formatearFecha(fechaISO) {
    if (!fechaISO) return "-";

    const fecha = new Date(fechaISO);

    const meses = [
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];

    const dia = fecha.getDate();
    const mes = meses[fecha.getMonth()];
    const anio = fecha.getFullYear();

    return `${dia} ${mes} ${anio}`;
}


let tablaData;

$(document).ready(function () {
    tablaData = $('#tbdataPersonalTrasladado').DataTable({
        responsive: true,
        autoWidth: false,
        serverSide: true,
        processing: true,
        ajax: {
            url: '/api/v1/ApiPersonal/ListarPaginadoTrasladados',
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                console.log("Request enviado:", d); // Verificá que se envíe correctamente
                return JSON.stringify(d);
            },
            dataSrc: 'data', //IMPORTANTE: indica de dónde sacar los registros

            beforeSend: function () {
                $(".card-body").LoadingOverlay("show");
            },
            complete: function () {
                $(".card-body").LoadingOverlay("hide");
            },
            error: function (xhr, status, error) {
                $(".card-body").LoadingOverlay("hide");
                console.error("Error al cargar datos:", error);
            }
        },
        columns: [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + 1 + meta.settings._iDisplayStart; // empieza en 1 y sigue en la paginación al correr la tabla
                }
            },
            { data: "legajo" },
            { data: "apellidoYNombre" },
            { data: "grado" },
            {
                data: "fechaEliminacion",
                render: function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        return formatearFecha(data); // Mostrar fecha legible
                    }
                    return data; // Para ordenar usar la fecha ISO original
                }
            },
            { data: null } // columna de botones
        ],
        columnDefs: [
            {
                targets: -1, // última columna
                responsivePriority: 1, // asegura que esta columna se muestre primero
                orderable: false,
                searchable: false,
                className: "text-center",
                responsivePriority: 1,
                render: function () {
                    return `
                        <button class="btn btn-info btn-ver btn-sm mr-1" title="Ver detalles">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="btn btn-warning btn-restituir btn-sm mr-1" title="Restituir">
                            <i class="fas fa-undo"></i>
                        </button>
                    `;
                }
            }
        ],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte_PersonalTrasladado',
                exportOptions: { columns: [0, 1, 2, 3, 4] }
            },
            'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });
});

//Boton para ver detalles del personal
$("#tbdataPersonalTrasladado tbody").on("click", ".btn-ver", function () {
    const fila = tablaData.row($(this).closest("tr")).data();
    const idPersonal = fila.idPersonal;

    $.ajax({
        type: "GET",
        url: `/api/v1/ApiPersonal/ObtenerPersonalParaEditar/${idPersonal}`, // misma ruta que usabas para editar
        success: function (personal) {
            // Llenar modal solo lectura
            $("#lblLegajo").text(personal.legajo || "---");
            $("#lblApellidoYNombre").text(personal.apellidoYNombre || "---");
            $("#lblGrado").text(personal.grado || "---");
            $("#lblChapa").text(personal.chapa || "---");
            $("#lblSexo").text(personal.sexo || "---");
            $("#lblFuncion").text(personal.funcion || "---");
            $("#lblHorario").text(personal.horario || "---");
            $("#lblRevista").text(personal.situacionRevista || "---");
            $("#lblFechaNacimiento").text(personal.fechaNacimiento || "---");
            $("#lblTelefono").text(personal.telefono || "---");
            $("#lblDNI").text(personal.dni || "---");
            $("#lblEmail").text(personal.email || "---");

            // Domicilio
            const domicilio = (personal.domicilios && personal.domicilios.length > 0) ? personal.domicilios[0] : {};
            $("#lblDomicilio").text(domicilio.calleBarrio || "---");
            $("#lblLocalidad").text(domicilio.localidad || "---");
            $("#lblCriaJurisdiccional").text(domicilio.comisariaJuris || "---");

            // Arma
            const arma = (personal.armas && personal.armas.length > 0) ? personal.armas[0] : { marca: "No Provista", numeroSerie: "No Provista" };
            $("#lblArmaMarca").text(arma.marca);
            $("#lblArmaNumeroSerie").text(arma.numeroSerie);

            // Imagen
            $("#imgPersonalPolicial").attr("src", personal.urlImagen || "/img/noimage.png");

            // Abrir modal
            $("#modalPersonalTrasladado").modal("show");
        },
        error: function () {
            alert("No se pudieron cargar los datos del personal.");
        }
    });
});

// Botón RESTITUIR para PERSONAL
$("#tbdataPersonalTrasladado tbody").on("click", ".btn-restituir", function () {

    const filaSeleccionada = $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();
    const idPersonal = data.idPersonal;

    swal({
        title: "¿Estás seguro?",
        text: `¿Desea restituir al personal "${data.apellidoYNombre}"?`,
        icon: "warning",
        buttons: {
            cancel: "Cancelar",
            confirm: {
                text: "Sí, restituir",
                className: "btn-success"
            }
        },
        dangerMode: true,
    }).then((respuesta) => {
        if (respuesta) {
            // Mostrar overlay de carga si lo tenés
            $(".showSweetAlert").LoadingOverlay("show");

            // Llamada al back-end para restituir
            $.ajax({
                type: "PUT",
                url: `/api/v1/ApiPersonal/restituir/${idPersonal}`, //aquí la ruta REST
                success: function (response) {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    if (response.estado) {
                        swal("Restituido!", response.mensaje, "success");
                        // Recargar DataTable sin perder el estado
                        tablaData.ajax.reload(null, false);
                    } else {
                        swal("Error", response.mensaje, "error");
                    }
                },
                error: function (err) {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    console.error(err);
                    swal("Error", "Error al intentar restituir al personal.", "error");
                }
            });
        }
    });

});



