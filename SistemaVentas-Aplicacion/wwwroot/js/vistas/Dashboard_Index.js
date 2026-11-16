$(document).ready(function() {


    listarNegocio();

});


function listarNegocio() {

    $("div.container.fluid").LoadingOverlay("show");

    $.ajax({

        url: '/Dashboard/ObtenerResumen',
        type: 'get',
        dataType: 'json',
        success: function(response) {
            $("div.container.fluid").LoadingOverlay("hide");
            if (response.estado) {

                let datos = response.objeto;
                console.log(datos);

                $("#totalVenta").text(datos.totalVentas)
                $("#totalIngresos").text("S/."+datos.totalIngresos)
                $("#totalProductos").text(datos.totalProductos)
                $("#totalCategorias").text(datos.totalCategorias)

                //Obtener los texto y valores para nuestro grafico de Barras
                
                let barchar_labels;
                let barchar_data;

                if (datos.ventasUltimaSemana.length > 0) {
                    barchar_labels = datos.ventasUltimaSemana.map((item) => { return item.fecha })
                    barchar_data = datos.ventasUltimaSemana.map((item) => { return item.total })

                } else {
                    barchar_labels = ["sin resultado"];
                    barchar_data = [0];
                }

                //Obtener los texto y valores para nuestro grafico de pie

                let piechat_labels;
                let piechat_data;
                if (datos.productosTopUltimaSemana.length > 0) {
                    piechat_labels = datos.productosTopUltimaSemana.map((item) => { return item.producto })
                    piechat_data = datos.productosTopUltimaSemana.map((item) => { return item.cantidad })

                } else {
                    piechat_labels = ["sin resultado"];
                    piechat_data = [0];
                }


                let controlVenta = document.getElementById("charVentas");
                let myBarChart = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchar_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchar_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });

                let controlProducto = document.getElementById("charProductos");
                let myPieChart = new Chart(controlProducto, {
                    type: 'doughnut',
                    data: {
                        labels: piechat_labels,
                        datasets: [{
                            data: piechat_data,
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
                        legend: {
                            display: true
                        },
                        cutoutPercentage: 80,
                    },
                });




            }
        }
    })

} 