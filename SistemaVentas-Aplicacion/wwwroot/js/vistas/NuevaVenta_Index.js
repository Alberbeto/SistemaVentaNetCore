
const Modelo_nuevaVenta = {
        idVenta:0,
        NumeroVenta: '', 
        IdTipoDocumentoVenta:0,
        TipoDocumentoVenta:'', 
        IdUsuario:0, 
        Usuario:'', 
        DocumentoCliente:'', 
        NombreCliente:'', 
        SubTotal:0, 
        ImpuestoTotal:0, 
        Total:0
}

$(document).ready(function () {

    ListarVistaVentas();

});

function ListarVistaVentas() {
    $.ajax({
        url: `/Venta/ListaTipoDetalleVenta`,
        type: 'get',
        dataType: 'json',
        success: function (response) {

            var datos = response.data;

            datos.forEach(function (item) {
                $("#cboTipoDocumentoVenta").append('<option value="' + item.idTipoDocumentoVenta + '">' + item.descripcion + '</option>')
            })
        }
    })

    $.ajax({
        url: `/Negocio/Obtener`,
        type: 'get',
        dataType: 'json',
        success: function (response) {

            if (response.estado) {

                var datos = response.objeto;
                $("#inputGroupSubTotal").text('SubTotal ' + '- ' + datos.simboloMoneda);
                $("#inputGroupIGV").text('IGV(' + datos.porcentajeImpuesto + ')' + '- ' + datos.simboloMoneda);
                $("#inputGroupTotal").text('Total ' + '- ' + datos.simboloMoneda);
            }
        }
    })

    $("#txtSubTotal").val("0.00");
    $("#txtIGV").val("0.00");
    $("#txtTotal").val("0.00");
    $("#txtDocumentoCliente").val('');
    $("#txtNombreCliente").val('');
    $("#tbProducto tbody").empty();
}

$("#cboBuscarProducto").select2({
    ajax: {
        url:'/Venta/ObtenerProducto',
        dataType: 'json',
        contentType:"application/json; charset=utf-8",
        delay: 250,
        data: function(params) {
            return {
                busqueda: params.term
            };
        },
        processResults: function(data) {
           
            return {
                results: data.data.map((item)=>({
                    id:item.idProducto,
                    text: item.descripcion,

                    marca:item.marca,
                    categoria:item.nombreCategoria,
                    urlImagen:item.urlImagen,
                    precio:parseFloat(item.precio)
                }))
            };
        },
       
    },
    language:"es",
    placeholder: 'Buscar Producto',
    minimumInputLength: 1,
    templateResult: formatoResultado
    
});

function formatoResultado(data){

    if(data.loading)
        return data.text;

    var contenedor = $(
    `<table width="100%">
    <tr>
        <td style="width:60px">
            <img style="height:60px; width:60px; margin-right:10px;"
                 src="${data.urlImagen}"
               
        </td>
        <td>
            <p style="font-weight:bolder; margin:2px">${data.marca}</p>
            <p style="margin:2px">${data.text}</p>
        </td>
    </tr>
</table>`
    );

    return contenedor;
}

let totalPrecios = 0;
let igv = 0;
let subtotal = 0;
let porcentaje = 18 / 100

function actualizarTotales() {
    subtotal = totalPrecios / (1 + porcentaje);
    igv = totalPrecios - subtotal;

    $("#txtSubTotal").val(subtotal.toFixed(2));
    $("#txtIGV").val(igv.toFixed(2));
    $("#txtTotal").val(totalPrecios.toFixed(2));
}

$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;
    
    let existe = false;
    ;
    $("#tbProducto tbody tr").each(function () {
        if ($(this).attr("data-id") == data.id) {
            existe = true;
            return false;
        }
    })
   
    if (existe) {
        
        Swal.fire({
            title: "MENSAJE",
            text: "El producto ya existe",
            icon: "error"
        });
        $("#cboBuscarProducto").val('').trigger('change');
    } else {
        $("#modalData").modal("show");
        

        $("#btnCantidad").off("click").on("click", function () {


            var cantidad = $("#txtCantidad").val();
            var cantidadColor = document.getElementById("txtCantidad");
            if (!cantidad) { 
                Swal.fire({
                    title: 'Mensaje',
                    text: 'Debe ingresar la cantidad del producto',
                    icon: 'error'
                })

                cantidadColor.style.border = "2px solid red";
                return;

            }

            cantidadColor.style.border = "2px solid #ccc";
            var total = parseFloat(cantidad) * parseFloat(data.precio);
            
            $("#tbProducto tbody").append(`
            <tr data-id="${data.id}" data-total="${total}">
                <td>${data.text}</td> 
                <td>${cantidad}</td>
                <td>${data.precio}</td>
                <td>${total}</td>                
                <td>
                    <button type="button" class="btn btn-danger btnEliminar">
                        Eliminar
                    </button>
                </td>               
            </tr>
        `);
            totalPrecios = totalPrecios + total;
            subtotal = totalPrecios / (1 + porcentaje);
            igv = totalPrecios - subtotal;
            $("#txtSubTotal").val(parseFloat(subtotal.toFixed(2)));
            $("#txtIGV").val(parseFloat(igv.toFixed(2)));
            $("#txtTotal").val(parseFloat(totalPrecios.toFixed(4)));

            $("#txtCantidad").val('')
            $("#cboBuscarProducto").val('').trigger('change');
            $("#modalData").modal("hide");
        })
        
    }

});

$("#tbProducto").on("click", ".btnEliminar", function () {

    let fila = $(this).closest("tr");
    let total = parseFloat(fila.attr("data-total")) || 0;
    totalPrecios -= total;

    // Evitar residuos de decimales
    totalPrecios = Math.max(0, Math.round(totalPrecios * 100) / 100);
    fila.remove();
    actualizarTotales();

})

$("#btnTerminarVentar").on("click", function () {

    
    var tablaProducto = $("#tbProducto tbody tr");

    

    if (tablaProducto.length < 1) {

        Swal.fire({
            title: 'Mensaje',
            text: 'debe ingresar al menos un producto',
            icon: 'error'
        })


    } else {

        var detalleVenta = [];

        tablaProducto.each(function () {

            var fila = $(this);
            let filaid = $(this).closest("tr");
            let idproducto = parseFloat(filaid.attr("data-id"))
            var producto = {
                idProducto: idproducto,
                Producto: fila.find("td").eq(0).text().trim(),
                Cantidad: fila.find("td").eq(1).text().trim(),
                Precio: fila.find("td").eq(2).text().trim(),
                Total: fila.find("td").eq(3).text().trim()
            };

            detalleVenta.push(producto);
        });



        const modelo = structuredClone(Modelo_nuevaVenta);

        modelo["IdTipoDocumentoVenta"] = $("#cboTipoDocumentoVenta").val(),
            modelo["DocumentoCliente"] = $("#txtDocumentoCliente").val(),
            modelo["IdUsuario"] = parseInt(27),
            modelo["NombreCliente"] = $("#txtNombreCliente").val(),
            modelo["SubTotal"] = $("#txtSubTotal").val(),
            modelo["ImpuestoTotal"] = $("#txtIGV").val()
            modelo["Total"] = $("#txtTotal").val(),
            modelo["DetalleVenta"] = detalleVenta

        const formData = new FormData();

        formData.append("modelo", JSON.stringify(modelo));

        $("#btnTerminarVentar").LoadingOverlay("show");
        $.ajax({
            url: '/Venta/RegistrarVenta',
            type: 'post',
            dataType: 'json',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {

                $("#btnTerminarVentar").LoadingOverlay("hide");

                if (response.estado) {
                    
                    Swal.fire({
                        title: 'MENSAJE',
                        text: 'Se guardo correctamente la venta numero: ' + response.objeto.numeroVenta ,
                        icon: 'success'
                    })

                    ListarVistaVentas();
                   
                    
                } else {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: response.mensaje,
                        icon: 'error'
                    })
                }



            }

        })

    }
});








