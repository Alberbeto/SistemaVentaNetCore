const VISTA_BUSQUEDA = {

    busquedaFecha: () => {

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").show();
        $(".busqueda-venta").hide();

        $("#cboBuscarPor option:first").prop("selected",true)
    }, busquedaVenta: () => {

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").hide();
        $(".busqueda-venta").show();
    }

   
}

$(document).ready(function () {
    VISTA_BUSQUEDA["busquedaFecha"]();


    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({dateFormat:"dd/mm/yy"})
    $("#txtFechaFin").datepicker({dateFormat:"dd/mm/yy" })
})

$("#cboBuscarPor").on('change', function () {
    
    let valor = $("#cboBuscarPor").val();
    

    if (valor == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]();
    } else {
        VISTA_BUSQUEDA["busquedaVenta"]();
    }

})

$("#btnBuscar").on('click', function () {

    let valor = $("#cboBuscarPor").val();


    if (valor == "fecha") {

       
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {

            Swal.fire({
                title: 'MENSAJE',
                text: 'Las fechas no pueden estar vacias',
                icon: 'error'
            })
        };


    } else {

        
        if ($("#txtNumeroVenta").val().trim() == "") {

            Swal.fire({
                title: 'MENSAJE',
                text: 'Debe ingresar un numero de venta',
                icon: 'error'
            })
        };
    }

    let fechaInicio = $("#txtFechaInicio").val();
    let fechaFin = $("#txtFechaFin").val();
    let numeroVenta = $("#txtNumeroVenta").val();

    $(".card-body").find("div.row").LoadingOverlay("show");

    $.ajax({
        url: '/Venta/hIstorial', 
        dataType: 'json',
        type: 'post',
        data: {
            numeroVenta: numeroVenta,
            fechaInicio: fechaInicio,
            fechaFin: fechaFin

        },
        success: function (response) {
            $(".card-body").find("div.row").LoadingOverlay("hide");

            var datos = response.data;

            

            $("#tbventa tbody").html("");

            datos.forEach(function (item) {


                var fila = $('<tr></tr>');

                fila.append('<td>' + item.fechaRegistro + '</td>')
                fila.append('<td>' + item.numeroVenta + '</td>')
                
                fila.append('<td>' + item.tipoDocumentoVenta + '</td>')
                fila.append('<td>' + item.documentoCliente + '</td>')
                fila.append('<td>' + item.nombreCliente + '</td>')
                fila.append('<td>' + item.total + '</td>')
                var boton = $('<button type="button" class="btn btn-info btn-sm btn-ver">' +
                    '<i class="fa fa-eye"></i>' +
                    '</button>');

                // Guardar los datos de la venta
                boton.data("datos", item);

                fila.append(
                    $("<td></td>").append(boton)
                );

                $("#tbventa tbody").append(fila);
            })
            // <tr>
            //     <td>27/04/2022</td>
            //     <td>00001</td>
            //     <td>Boleta</td>
            //     <td>101010</td>
            //     <td>jose</td>
            //     <td>2000</td>
            //     <td><button class="btn btn-info btn-sm" data-toggle="modal" data-target="#modalData"><i class="fas fa-eye"></i> Ver Detalle</button></td>
            // </tr>
        }
    })

    $("#tbventa tbody").on("click", ".btn-info", function () {

        let resumen = $(this).data("datos");
        console.log(resumen);

        $("#txtFechaRegistro").val(resumen.fechaRegistro);
        $("#txtNumVenta").val(resumen.numeroVenta);
        $("#txtUsuarioRegistro").val(resumen.usuario);
        $("#txtTipoDocumento").val(resumen.tipoDocumentoVenta);
        $("#txtDocumentoCliente").val(resumen.documentoCliente);
        $("#txtNombreCliente").val(resumen.nombreCliente);
        $("#txtSubTotal").val(resumen.subTotal);
        $("#txtIGV").val(resumen.impuestoTotal);
        $("#txtSubTotal").val(resumen.subTotal);
        $("#txtTotal").val(resumen.total);

        $("#tbProductos tbody").html("");

        resumen.detalleVenta.forEach(function (item) {
            
            var fila = $('<tr></tr>');

            fila.append('<td>' + item.descripcionProducto + '</td>')
            fila.append('<td>' + item.cantidad + '</td>')

            fila.append('<td>' + item.precio + '</td>')
            fila.append('<td>' + item.total + '</td>')

            $("#tbProductos tbody").append(fila);
        })
       

        $("#modalData").modal("show");

    })
})