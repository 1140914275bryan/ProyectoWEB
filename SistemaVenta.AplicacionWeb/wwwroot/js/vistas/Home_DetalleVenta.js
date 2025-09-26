let chartDetalleVenta = null;

function crearGrafico(data) {
    const ctx = document.getElementById('graficoVenta').getContext('2d');

    if (chartDetalleVenta) {
        chartDetalleVenta.destroy();
    }

    chartDetalleVenta = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map(item => item.producto),
            datasets: [{
                label: 'Cantidad Vendida',
                data: data.map(item => item.cantidad),
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top'
                },
                title: {
                    display: true,
                    text: 'Detalle de Venta por Producto'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            }
        }
    });
}

$("#txtIdVenta").on("keyup", function (e) {
    if (e.key === "Enter") {
        const idVenta = $("#txtIdVenta").val().trim();

        if (idVenta === "") {
            toastr.warning("Debe ingresar un ID de venta");
            return;
        }

        fetch(`/DashBoard/ObtenerDetalleVenta?idVenta=${idVenta}`)
            .then(response => response.json())
            .then(data => {
                if (data.estado) {
                    crearGrafico(data.objeto);
                } else {
                    toastr.warning(data.mensaje || "No se encontraron productos");
                    if (chartDetalleVenta) chartDetalleVenta.destroy();
                }
            })
            .catch(error => {
                console.error(error);
                toastr.error("Error al obtener los datos");
            });
    }
});
