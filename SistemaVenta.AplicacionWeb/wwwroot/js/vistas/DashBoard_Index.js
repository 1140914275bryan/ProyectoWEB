$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");

    fetch("/DashBoard/ObtenerResumen")
        .then(response => {
            $("div.container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            if (responseJson.estado) {

                let d = responseJson.objeto;

                // --- TARJETAS ---
                $("#totalVenta").text(d.totalVentas);
                $("#totalIngresos").text(d.totalIngresos);
                $("#totalProductos").text(d.totalProductos);
                $("#totalCategorias").text(d.totalCategorias);

                // --- GRAFICO BARRAS: Ventas última semana ---
                let barchart_labels = d.ventasUltimaSemana.length > 0 ? d.ventasUltimaSemana.map(item => item.fecha) : ["Sin resultados"];
                let barchart_data = d.ventasUltimaSemana.length > 0 ? d.ventasUltimaSemana.map(item => item.total) : [0];

                let controlVenta = document.getElementById("charVentas");
                new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: { display: false },
                        scales: {
                            xAxes: [{ gridLines: { display: false }, maxBarThickness: 50 }],
                            yAxes: [{ ticks: { min: 0, maxTicksLimit: 5 } }]
                        }
                    }
                });

                // --- GRAFICO TORTA: Productos top ---
                let piechart_labels = d.productosTopUltimaSemana.length > 0 ? d.productosTopUltimaSemana.map(item => item.producto) : ["Sin resultados"];
                let piechart_data = d.productosTopUltimaSemana.length > 0 ? d.productosTopUltimaSemana.map(item => item.cantidad) : [0];

                let controlProducto = document.getElementById("charProductos");
                new Chart(controlProducto, {
                    type: 'doughnut',
                    data: {
                        labels: piechart_labels,
                        datasets: [{
                            data: piechart_data,
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', "#FF785B"],
                            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', "#FF5733"],
                            hoverBorderColor: "rgba(234, 236, 244, 1)",
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        tooltips: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyFontColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            caretPadding: 10,
                        },
                        legend: { display: true },
                        cutoutPercentage: 80,
                    }
                });

                // --- GRAFICO BARRAS DETALLE VENTA POR ID ---
                const idVentaEjemplo = 1; // Puedes cambiar este ID o hacer dinámico

                fetch(`/DashBoard/ObtenerDetalleVenta?idVenta=${idVentaEjemplo}`)
                    .then(res => res.json())
                    .then(json => {
                        if (json.estado) {
                            let detalle_labels = json.objeto.map(item => item.producto);
                            let detalle_data = json.objeto.map(item => item.cantidad);

                            let controlDetalle = document.getElementById("charDetalleVenta");
                            new Chart(controlDetalle, {
                                type: 'bar',
                                data: {
                                    labels: detalle_labels,
                                    datasets: [{
                                        label: "Cantidad Vendida",
                                        backgroundColor: "#f6c23e",
                                        hoverBackgroundColor: "#f4b619",
                                        borderColor: "#f6c23e",
                                        data: detalle_data,
                                    }]
                                },
                                options: {
                                    maintainAspectRatio: false,
                                    legend: { display: false },
                                    scales: {
                                        xAxes: [{ gridLines: { display: false }, maxBarThickness: 50 }],
                                        yAxes: [{ ticks: { min: 0, maxTicksLimit: 5 } }]
                                    }
                                }
                            });
                        } else {
                            console.warn("No se encontraron datos para la venta especificada.");
                        }
                    });
            }

        });
});

let chartDetalle = null;

$("#btnBuscarVenta").click(function () {
    let idVenta = parseInt($("#txtIdVenta").val());

    if (isNaN(idVenta) || idVenta <= 0) {
        toastr.warning("Ingrese un número de venta válido");
        return;
    }

    $("div.container-fluid").LoadingOverlay("show");

    fetch(`/DashBoard/ObtenerDetalleVenta?idVenta=${idVenta}`)
        .then(res => res.json())
        .then(json => {
            $("div.container-fluid").LoadingOverlay("hide");

            if (json.estado) {
                let detalle_labels = json.objeto.map(item => item.producto);
                let detalle_data = json.objeto.map(item => item.cantidad);

                // Destruir gráfico anterior si existe
                if (chartDetalle !== null) {
                    chartDetalle.destroy();
                }

                let controlDetalle = document.getElementById("charDetalleVenta");

                chartDetalle = new Chart(controlDetalle, {
                    type: 'bar',
                    data: {
                        labels: detalle_labels,
                        datasets: [{
                            label: "Cantidad Vendida",
                            backgroundColor: "#f6c23e",
                            hoverBackgroundColor: "#f4b619",
                            borderColor: "#f6c23e",
                            data: detalle_data,
                        }]
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: { display: false },
                        scales: {
                            xAxes: [{ gridLines: { display: false }, maxBarThickness: 50 }],
                            yAxes: [{ ticks: { beginAtZero: true } }]
                        }
                    }
                });
            } else {
                toastr.error(json.mensaje || "No se encontraron productos para esta venta");
            }
        })
        .catch(() => {
            $("div.container-fluid").LoadingOverlay("hide");
            toastr.error("Error al obtener los datos de la venta");
        });
});
