const Modelo_Producto = {

    idProducto :0,
    codigoBarra :"",
    marca :"",
    descripcion:"" ,   
    idCategoria :0,
    nombreCategoria:"", 
    stock :0,
    urlImagen:"" ,
    precio :0.0,
    esActivo:1
}

var tablaProductos;

$(document).ready(function () {
    tablaProductos = $('#tbdata').DataTable({
        responsive: true,
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [1, 2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    ListarProducto();
})


function ListarProducto() {

    $.ajax({
        url: '/Producto/Lista',
        type: 'get',
        dataType: 'json',
        success: function (response) {
            var datos = response.data;

            tablaProductos.clear();

            datos.forEach(function (item) {
                var fila = $('<tr><tr/>');

                
                fila.append('<td><img src="' + item.urlImagen + '"  width="50" height="50"></td>');
                fila.append('<td>' + item.codigoBarra + '</td>');
                fila.append('<td>' + item.marca + '</td>');
                fila.append('<td>' + item.descripcion + '</td>');
                fila.append('<td>' + item.stock + '</td>');              
                fila.append('<td>' + item.nombreCategoria + '</td>');
                fila.append('<td>' + item.precio + '</td>');
                fila.append('<td>' + item.esActivo + '</td>');
                fila.append(
                    '<td>' +
                    '<div style="display:flex; gap:5px; justify-content:center;">' +
                    '<button type="button" class="btn btn-primary" onclick="editarCategoria(' + item.idProducto + ')" title="Editar">' +
                    '<i class="fas fa-edit"></i>' +
                    '</button>' +
                    '<button type="button" class="btn btn-danger" onclick="eliminarCategoria(' + item.idProducto + ')" title="Eliminar">' +
                    '<i class="fas fa-trash"></i>' +
                    '</button>' +
                    '</div>' +
                    '</td>'
                );

                tablaProductos.row.add(fila);
            });

            tablaProductos.draw();
        }
    })

}

function MostrarModal(modelo = Modelo_Producto) {

    $("#txtId").val(modelo.idProducto);
    $("#txtCodigoBarra").val(modelo.codigoBarra);
    $("#txtMarca").val(modelo.marca);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboCategoria").val(modelo.idCategoria == 0 ? $("cboCategoria option:first").val() : modelo.idCategoria);
    $("#txtStock").val(modelo.stock);
    $("#txtPrecio").val(modelo.precio);
    $("#cboEstado").val(modelo.esActivo);
    $("#txtImagen").val("");
    $("#imgProducto").attr("src", modelo.urlImagen);

    $("#modalData").modal("show");

}

$("#abrilModal").on('click', function () {
    
    $.ajax({
        url: '/Producto/ListaCategoria',
        type: 'get',
        dataType: 'json',
        success: function (response) {
            var datos = response.data;

            datos.forEach(function (item) {
                $('#cboCategoria').append('<option value="' + item.idCategoria + '">' + item.descripcion + '</option>')
            });
        }
    })

    MostrarModal();
})


$("#btnGuardar").on('click', function () {

    var codigoBarra = $("#txtCodigoBarra").val();
    var marca = $("#txtMarca").val();
    var descripcion = $("#txtDescripcion").val();
    var stock = $("#txtStock").val();
    var precio = $("#txtPrecio").val();
    var categoria = $("#cboCategoria").val();
    var estado = $("#cboEstado").val();

    var codigoBarraBorder = document.getElementById("txtCodigoBarra");
    var marcaBorder = document.getElementById("txtMarca");
    var descripcionBorder = document.getElementById("txtDescripcion");
    var stockBorder = document.getElementById("txtStock");
    var precioBorder = document.getElementById("txtPrecio");

    if (!codigoBarra) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'el codigo de barra no puede estar vacio',
            icon: 'error'
        });

        codigoBarraBorder.style.border = "2px solid red";
        return;
    }


    if (!marca) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'la marca no puede estar vacio',
            icon: 'error'
        });

        marcaBorder.style.border = "2px solid red";
        return;
    }

    if (!descripcion) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'la descripcion no puede estar vacio',
            icon: 'error'
        });

        descripcionBorder.style.border = "2px solid red";
        return;
    }

    if (!stock) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'el stock no puede estar vacio',
            icon: 'error'
        });

        stockBorder.style.border = "2px solid red";
        return;
    }

    if (!precio) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'el precio no puede estar vacio',
            icon: 'error'
        });

        precioBorder.style.border = "2px solid red";
        return;
    }


    const modelo = structuredClone(Modelo_Producto);
    modelo["idProducto"] = parseInt($("#txtId").val());
    modelo["codigoBarra"] = $("#txtCodigoBarra").val();
    modelo["marca"] = $("#txtMarca").val();
    modelo["descripcion"] = $("#txtDescripcion").val();
    modelo["stock"] = $("#txtStock").val();
    modelo["precio"] = $("#txtPrecio").val();
    modelo["idCategoria"] = $("#cboCategoria").val();
    modelo["esActivo"] = $("#cboEstado").val();

    const inputFile = document.getElementById("txtImagen");

    const formData = new FormData();


    formData.append("foto", inputFile.files[0])
    formData.append("modelo", JSON.stringify(modelo))



    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idProducto == 0) {
        $.ajax({
            url: '/Producto/Crear',
            type: 'post',
            dataType: 'json',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");

                if (response.estado) {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: 'Se guardo correctamente el producto',
                        icon: 'success'
                    });
                    ListarProducto();
                    $("#modalData").modal("hide");
                } else {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: response.mensaje,
                        icon: 'error'
                    });
                }

            }
        })
    } else {
        $.ajax({
            url: '/Producto/Editar',
            type: 'put',
            dataType: 'json',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");

                if (response.estado) {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: 'Se Editado correctamente el producto',
                        icon: 'success'
                    });
                    ListarProducto();
                    $("#modalData").modal("hide");
                } else {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: response.mensaje,
                        icon: 'error'
                    });
                }

            }
        })
    }
  
})

function editarCategoria(idProducto) {

    
    $.ajax({
        url:'/Producto/ObtenerxId',
        type: 'post',
        dataType: 'json',
        data: { idProducto: idProducto },
        success: function (response) {
            
            var data = response.data;

            $("#txtId").val(data.idProducto);
            $("#txtCodigoBarra").val(data.codigoBarra);
            $("#txtMarca").val(data.marca);
            $("#txtDescripcion").val(data.descripcion);
            $("#txtStock").val(data.stock);
            $("#txtPrecio").val(data.precio);
            
            $("#cboEstado").val(data.esActivo);
            $("#txtImagen").attr("src", data.urlImagen);

            $.ajax({
                url: 'Producto/ListaCategoria',
                type: 'get',
                dataType: 'json',
                success: function (response) {
                    var datos = response.data;
                    
                    datos.forEach(function (item) {
                        $('#cboCategoria').append('<option value="' + item.idCategoria + '">' + item.descripcion + '</option>')
                    });

                    $('#cboCategoria').val(data.idCategoria);
                }
            })

            $("#modalData").modal("show");
        }
    })
}

function eliminarCategoria(idProducto) {

    Swal.fire({
        title: "MENSAJE",
        text: "Desea eliminar el producto?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "SI"
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: 'Producto/Eliminar',
                type: 'delete',
                data: { idProducto: idProducto },
                dataType: 'json',
                success: function (response) {

                    if (response.estado) {
                        Swal.fire({
                            title: 'MENSAJE',
                            text: 'Se ha eliminado correctamente el producto',
                            icon: 'success'
                        });
                        ListarProducto();
                    } else {
                        Swal.fire({
                            title: 'MENSAJE',
                            text: response.mensaje,
                            icon: 'success'
                        });
                    }
                }
            })
        }
    })
}


