const Modelo_Categoria = {

    idCategoria:0,

    descripcion:"",

    esActivo:0
}

var tablaCategoria;



    $(document).ready(function () {

        tablaCategoria = $('#tbdata').DataTable({
            responsive: true,
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: '',
                    filename: 'Reporte Categorias',
                    exportOptions: {
                        columns: [1, 2]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });

        ListarCategorias();


    });



function ListarCategorias() {


    $.ajax({

        url: '/Categoria/Listar',
        type: 'get',
        dataType: 'json',
        success: function (response) {

            var datos = response.data;
            tablaCategoria.clear();

            
            datos.forEach(function (item) {

                var fila = $('<tr><tr/>');

                fila.append('<td>' + item.descripcion + '</td>')
                fila.append('<td>' + (item.esActivo == 1 ? '<span class="badge bg-success">Activo</span>' : '<span class="badge bg-danger">Inactivo</span>') + '</td>')
                fila.append(
                    '<td>' +
                    '<div style="display:flex; gap:5px; justify-content:center;">' +
                    '<button type="button" class="btn btn-primary" onclick="editarCategoria(' + item.idCategoria + ')" title="Editar">' +
                    '<i class="fas fa-edit"></i>' +
                    '</button>' +
                    '<button type="button" class="btn btn-danger" onclick="eliminarCategoria(' + item.idCategoria + ')" title="Eliminar">' +
                    '<i class="fas fa-trash"></i>' +
                    '</button>' +
                    '</div>' +
                    '</td>'
                );

                tablaCategoria.row.add(fila);
            })

            tablaCategoria.draw();

        }
        
    })
}

function MostrarModal(modelo = Modelo_Categoria) {

    $("#txtId").val(modelo.idCategoria);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboEstado").val(modelo.esActivo);

    $('#modalData').modal("show");
}

$("#modalCategoria").on('click', function () {

    MostrarModal();
})

$("#btnGuardar").on('click', function () {

    var descripcion = $("#txtDescripcion").val();
    var descripcioncolor = document.getElementById("txtDescripcion");

    if (!descripcion) {
        Swal.fire({
            title: 'MENSAJE',
            text: 'Debe agregar una categoria',
            icon: 'error'
        })

        descripcioncolor.style.border = '2px solid red';
    }

    const modelo = structuredClone(Modelo_Categoria);
    modelo["idCategoria"] = parseInt($("#txtId").val());
    modelo["descripcion"] = descripcion;
    modelo["esActivo"] = parseInt($("#cboEstado").val());
    const formData = new FormData();
    formData.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCategoria === 0) {
        $.ajax({
            url: '/Categoria/Crear',
            type: 'post',
            dataType: 'json',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {

                $("#modalData").find("div.modal-content").LoadingOverlay("hide");

                if (response.estado) {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: 'Se guardo correctamente la categoria',
                        icon: 'success'
                    })

                    ListarCategorias();
                    $('#modalData').modal("hide");
                } else {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: response.mensaje,
                        icon: 'error'
                    })
                }



            }

        })
    } else {
        $.ajax({
            url: '/Categoria/Editar',
            type: 'put',
            dataType: 'json',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {

                $("#modalData").find("div.modal-content").LoadingOverlay("hide");

                if (response.estado) {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: 'Se actualizo correctamente la categoria',
                        icon: 'success'
                    })

                    ListarCategorias();
                    $('#modalData').modal("hide");
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
 

})

function editarCategoria(idCategoria) {


    $.ajax({

        url: '/Categoria/Obtener',
        type: 'post',
        dataType: 'json',
        data: { idCategoria: idCategoria },
        success: function (response) {

            var datos = response.data;
            MostrarModal(datos);

        }


    })
}

function eliminarCategoria(idCategoria) {

    console.log(idCategoria);
    Swal.fire({
        title: "MENSAJE",
        text: "Desea Eliminar la categoria",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "SI"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Categoria/Eliminar',
                type: 'delete',
                dataType: 'json',
                data: { idCategoria: idCategoria },
                
                success: function (response) {

                  

                    if (response.estado) {
                        Swal.fire({
                            title: 'MENSAJE',
                            text: 'Se eliminado correctamente la categoria',
                            icon: 'success'
                        })

                        ListarCategorias();
                       
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



}