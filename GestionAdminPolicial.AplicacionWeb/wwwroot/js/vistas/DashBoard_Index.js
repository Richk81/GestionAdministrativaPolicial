document.addEventListener("DOMContentLoaded", function () {
    const apiBaseUrl = "/api/v1/ApiDashboard";

    // === 1. Cargar totales ===
    async function cargarTotales() {
        try {
            const response = await fetch(`${apiBaseUrl}/Totales`);
            if (!response.ok) throw new Error("Error al obtener totales");
            const data = await response.json();

            document.getElementById("totalPersonal").textContent = data.personalActivo ?? 0;
            document.getElementById("totalChalecos").textContent = data.chalecosActivos ?? 0;
            document.getElementById("totalEscopetas").textContent = data.escopetasActivas ?? 0;
            document.getElementById("totalRadios").textContent = data.radiosActivas ?? 0;
            document.getElementById("totalVehiculos").textContent = data.vehiculosActivos ?? 0;

            // Luego de cargar los totales, dibujamos el gráfico de distribución
            renderGraficoRecursos(data);
        } catch (error) {
            console.error("Error:", error);
        }
    }

    // === 2. Cargar Altas y Bajas por mes ===
    async function cargarAltasYBajas(anioActual) {
        try {
            const responseAltas = await fetch(`${apiBaseUrl}/AltasPorMes?anio=${anioActual}`);
            const responseTraslados = await fetch(`${apiBaseUrl}/TrasladosPorMes?anio=${anioActual}`);
            if (!responseAltas.ok || !responseTraslados.ok) throw new Error("Error al obtener datos de gráficos");

            const altas = await responseAltas.json();
            const traslados = await responseTraslados.json();

            renderGraficoAltasBajas(altas, traslados);
        } catch (error) {
            console.error("Error:", error);
        }
    }

    // === 3. Gráfico de Altas y Bajas ===
    function renderGraficoAltasBajas(altas, traslados) {
        const ctx = document.getElementById("chartAltasBajas").getContext("2d");

        new Chart(ctx, {
            type: "bar",
            data: {
                labels: [
                    "Ene", "Feb", "Mar", "Abr", "May", "Jun",
                    "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
                ],
                datasets: [
                    {
                        label: "Altas",
                        data: altas,
                        backgroundColor: "rgba(54, 162, 235, 0.6)",
                        borderColor: "rgba(54, 162, 235, 1)",
                        borderWidth: 1
                    },
                    {
                        label: "Traslados",
                        data: traslados,
                        backgroundColor: "rgba(255, 99, 132, 0.6)",
                        borderColor: "rgba(255, 99, 132, 1)",
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { precision: 0 }
                    }
                }
            }
        });
    }

    // === 4. Gráfico de Distribución de Recursos ===
    function renderGraficoRecursos(data) {
        const ctx = document.getElementById("chartRecursos").getContext("2d");

        new Chart(ctx, {
            type: "doughnut",
            data: {
                labels: ["Personal", "Chalecos", "Escopetas", "Radios", "Vehículos"],
                datasets: [{
                    data: [
                        data.personalActivo ?? 0,
                        data.chalecosActivos ?? 0,
                        data.escopetasActivas ?? 0,
                        data.radiosActivas ?? 0,
                        data.vehiculosActivos ?? 0
                    ],
                    backgroundColor: [
                        "#007bff", "#28a745", "#17a2b8", "#ffc107", "#343a40"
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: "bottom" }
                }
            }
        });
    }

    // === Inicializar ===
    const anioActual = new Date().getFullYear();
    cargarTotales();
    cargarAltasYBajas(anioActual);
});
