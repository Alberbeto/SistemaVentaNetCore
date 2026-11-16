var tablaReporte;

$(document).ready(function() {

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")

    tablaReporte = $("#tbdata").DataTable({

        responsive: true,
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Ventas',
               
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    $.ajax({
        url: '/Reporte/reporteVenta?fechaInicio=01/01/1991&fechaFin=01/01/1991',
        type: 'GET',
        dataType: 'json',
        success: function(response) {

            var datos = response.data;

            tablaReporte.clear();

            datos.forEach(function(item) {

                var fila = $("<tr></tr>");

                fila.append('<td>' + item.fechaRegistro + '<td/>')
                fila.append('<td>' + item.numeroVenta + '<td/>')
                fila.append('<td>' + item.tipoDocumento + '<td/>')
                fila.append('<td>' + item.documentoCliente + '<td/>')
                fila.append('<td>' + item.nombreCliente + '<td/>')
                fila.append('<td>' + item.subTotalVenta + '<td/>')
                fila.append('<td>' + item.impuestoTotalVenta + '<td/>')
                fila.append('<td>' + item.totalVenta + '<td/>')
                fila.append('<td>' + item.producto + '<td/>')
                fila.append('<td>' + item.cantidad + '<td/>')
                fila.append('<td>' + item.precio + '<td/>')
                fila.append('<td>' + item.total + '<td/>')

                tablaReporte.row.add(fila);

            });

            tablaReporte.draw();
            

            
        }

    });

   
});


    $("#btnBuscar").on('click', function() {
        
      
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {

            Swal.fire({
                title: 'MENSAJE',
                text: 'Las fechas no pueden estar vacias',
                icon: 'error'
            });
            return;
        };


        var fechaInicio = $("#txtFechaInicio").val().trim();
        var fechaFin = $("#txtFechaFin").val().trim();


       

        $.ajax({
            url: `/Reporte/reporteVenta?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`,
            type: 'GET',
            dataType: 'json',
            success: function(response) {

                var datos = response.data;
                console.log(datos);
                tablaReporte.clear();

                datos.forEach(function(item) {

                    var fila = $("<tr></tr>");

                    fila.append('<td>' + item.fechaRegistro + '</td>')
                    fila.append('<td>' + item.numeroVenta + '</td>')
                    fila.append('<td>' + item.tipoDocumento + '</td>')
                    fila.append('<td>' + item.documentoCliente + '</td>')
                    fila.append('<td>' + item.nombreCliente + '</td>')
                    fila.append('<td>' + item.subTotalVenta + '</td>')
                    fila.append('<td>' + item.impuestoTotalVenta + '</td>')
                    fila.append('<td>' + item.totalVenta + '</td>')
                    fila.append('<td>' + item.producto + '</td>')
                    fila.append('<td>' + item.cantidad + '</td>')
                    fila.append('<td>' + item.precio + '</td>')
                    fila.append('<td>' + item.total + '</td>')

                    tablaReporte.row.add(fila);

                });

                tablaReporte.draw();



            }

        });
    })

