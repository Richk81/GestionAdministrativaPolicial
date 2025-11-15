document.addEventListener("DOMContentLoaded", function () {

    const apiBaseUrl = "/api/v1/ApiDashboard";

    /* =======================================================
       1. CARGAR TOTALES
    ======================================================== */
    async function cargarTotales() {
        try {
            const response = await fetch(`${apiBaseUrl}/totales`);
            if (!response.ok) throw new Error("Error al obtener totales");

            const data = await response.json();

            document.getElementById("totalPersonal").textContent = data.totalPersonal ?? 0;
            document.getElementById("totalChalecos").textContent = data.totalChalecos ?? 0;
            document.getElementById("totalEscopetas").textContent = data.totalEscopetas ?? 0;
            document.getElementById("totalRadios").textContent = data.totalRadios ?? 0;
            document.getElementById("totalVehiculos").textContent = data.totalVehiculos ?? 0;


            // Gráfico de recursos
            renderGraficoRecursos(data);

        } catch (error) {
            console.error("Error:", error);
        }
    }

    /* =======================================================
       2. CARGAR ALTAS / BAJAS (TRASLADOS) POR MES
    ======================================================== */
    async function cargarAltasYBajas(anio) {
        try {
            const respAltas = await fetch(`${apiBaseUrl}/altas-personal/${anio}`);
            const respTraslados = await fetch(`${apiBaseUrl}/traslados-personal/${anio}`);

            if (!respAltas.ok || !respTraslados.ok) {
                throw new Error("Error al obtener datos de gráficos");
            }

            const altasDb = await respAltas.json();
            const trasDb = await respTraslados.json();

            // Convertimos los resultados en arrays de 12 meses
            const altas = normalizarMeses(altasDb);
            const traslados = normalizarMeses(trasDb);

            renderGraficoAltasBajas(altas, traslados);

        } catch (error) {
            console.error("Error:", error);
        }
    }

    /* =======================================================
       2.1. Normaliza los datos a 12 meses
    ======================================================== */
    function normalizarMeses(lista) {
        const meses = Array(12).fill(0);
        lista.forEach(item => {
            meses[item.mes - 1] = item.total;
        });
        return meses;
    }

    /* =======================================================
       3. GRÁFICO ALTAS / BAJAS
    ======================================================== */
    function renderGraficoAltasBajas(altas, traslados) {
        const ctx = document.getElementById("chartAltasBajas").getContext("2d");

        new Chart(ctx, {
            type: "bar",
            data: {
                labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun",
                    "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
                datasets: [
                    {
                        label: "Altas",
                        data: altas,
                        backgroundColor: "rgba(54, 162, 235, 0.6)"
                    },
                    {
                        label: "Traslados",
                        data: traslados,
                        backgroundColor: "rgba(255, 99, 132, 0.6)"
                    }
                ]
            },
            options: {
                responsive: true,
                scales: {
                    y: { beginAtZero: true, ticks: { precision: 0 } }
                }
            }
        });
    }

    /* =======================================================
       4. GRÁFICO DE DISTRIBUCIÓN DE RECURSOS
    ======================================================== */
    function renderGraficoRecursos(data) {
        const ctx = document.getElementById("chartRecursos").getContext("2d");

        new Chart(ctx, {
            type: "doughnut",
            data: {
                labels: ["Personal", "Chalecos", "Escopetas", "Radios", "Vehículos"],
                datasets: [{
                    data: [
                        data.totalPersonal ?? 0,
                        data.totalChalecos ?? 0,
                        data.totalEscopetas ?? 0,
                        data.totalRadios ?? 0,
                        data.totalVehiculos ?? 0
                    ],
                    backgroundColor: [
                        "#4e73df",
                        "#1cc88a",
                        "#36b9cc",
                        "#f6c23e",
                        "#e74a3b"
                    ],
                    hoverOffset: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: "bottom" }
                }
            }
        });
    }

    /* =======================================================
       INICIALIZAR DASHBOARD
    ======================================================== */
    const anioActual = new Date().getFullYear();
    cargarTotales();
    cargarAltasYBajas(anioActual);

});

