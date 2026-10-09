


const Modelo_base = {
   idUsuario :0,
    nombre :"",
    correo :"",
    telefono :"",
    idRol :0,
    urlFoto :"",
    esActivo :1
}
var tablaUsuario;
$(document).ready(function () {

      tablaUsuario =$('#tbdata').DataTable({
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
  
    ListarUsuarios();


});

function ListarUsuarios() {

    $.ajax({
        url: 'Usuario/Lista',
        type: 'get',
        dataType: 'json',
        success: function (response) {

            var datos = response.data;
            tablaUsuario.clear();

            datos.forEach(function (item) {
                

                var fila = $('<tr></tr>');

                fila.append('<td style="display:none;>' + item.idUsuario + '</td>')
                fila.append('<td><img src="' + item.urlFoto + '"  width="50" height="50"></td>');
                fila.append('<td>' + item.nombre + '</td>')
                fila.append('<td>' + item.correo + '</td>')
                fila.append('<td>' + item.telefono + '</td>')
                fila.append('<td>' + item.nombreRol + '</td>')
                fila.append('<td>' + (item.esActivo == 1 ? '<span class="badge bg-success">Activo</span>' : '<span class="badge bg-danger">Inactivo</span>') + '</td>')
                fila.append(
                    '<td>' +
                    '<div style="display:flex; gap:5px; justify-content:center;">' +
                    '<button type="button" class="btn btn-primary" onclick="editarUsuario(' + item.idUsuario + ')" title="Editar">' +
                    '<i class="fas fa-edit"></i>' +
                    '</button>' +
                    '<button type="button" class="btn btn-danger" onclick="eliminarUsuario(' + item.idUsuario + ')" title="Eliminar">' +
                    '<i class="fas fa-trash"></i>' +
                    '</button>' +
                    '</div>' +
                    '</td>'
                );

                tablaUsuario.row.add(fila);
               
            });

            tablaUsuario.draw();

        }
    })
}

function MostrarModal(model = Modelo_base) {

    $("#txtId").val(model.idUsuario);
    $("#txtNombre").val(model.nombre);
    $("#txtCorreo").val(model.correo);
    $("#txtTelefono").val(model.telefono);
    $("#cboRol").val(model.idRol == 0 ? $("cboRol option:first").val() : modelo.idRol);
    $("#cboEstado").val(model.esActivo);
    $("#txtFoto").val("");
    $("#imgUsuario").attr("src", model.urlFoto);

    $("#modalData").modal("show");
}

$("#btnNuevo").on('click', function () {

    $.ajax({
        url: 'Usuario/ListarRoles',
        type: 'get',
        dataType: 'json',
        success: function (response) {

            var roles = response.data;
            
            $("#cboRol").empty();

            roles.forEach(function (item) {
                
               $("#cboRol").append('<option value= "' + item.idRol + '">' + item.descripcion  + '</option>')
            })
        }
    })
    MostrarModal();
})



function editarUsuario(idusuario) {

    
    $.ajax({
        url: 'Usuario/ObtenerPorId',
        type: 'post',
        dataType: 'json',
        data: { idUsuario: idusuario },
        success: function (response) {
            var datos = response.data;
            debugger;

            $("#txtId").val(datos.idUsuario);
            $("#txtNombre").val(datos.nombre);
            $("#txtCorreo").val(datos.correo);
            $("#txtTelefono").val(datos.telefono);

            $("#cboEstado").val(datos.esActivo);
            $("#imgUsuario").attr("src", datos.urlFoto);


            $.ajax({
                url: 'Usuario/ListarRoles',
                type: 'get',
                dataType: 'json',
                success: function (response) {

                    var roles = response.data;

                    $("#cboRol").empty();

                    roles.forEach(function (item) {

                        $("#cboRol").append('<option value= "' + item.idRol + '">' + item.descripcion + '</option>')
                    })
                    $("#cboRol").val(datos.idRol);
                }
            })
            $("#modalData").modal("show");




        }
    })

}



$("#btnGuardar").on('click', function () {

    
    var nombre=$("#txtNombre").val().trim();
    var correo =$("#txtCorreo").val().trim();
    var telefono = $("#txtTelefono").val().trim();

    if (!nombre && !correo && !telefono) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'Los campos no pueden estas vacios',
            icon: 'error'
        })
        return;

        
    }
    if (!nombre) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'El campo nombre no puede estar vacio',
            icon: 'error'
        })
        return;
    }

    if (!correo) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'El campo correo no puede estar vacio',
            icon: 'error'
        })
        return;
    }

    if (!telefono) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'El campo telefono no puede estar vacio',
            icon: 'error'
        })
        return;
    }

    const modelo = structuredClone(Modelo_base);

    modelo["idUsuario"] = parseInt($("#txtId").val());
    modelo["nombre"] = $("#txtNombre").val();
    modelo["correo"] = $("#txtCorreo").val();
    modelo["telefono"] = $("#txtTelefono").val();
    modelo["idRol"] = $("#cboRol").val();
    modelo["esActivo"] = $("#cboEstado").val();

    const inputFoto = document.getElementById("txtFoto")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idUsuario == 0) {

        $.ajax({
            url: 'Usuario/Crear',
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
                        text: 'Se registro el usuario de forma correcta',
                        icon: 'success'
                    })
                    $("#modalData").modal("hide");

                    ListarUsuarios();
                } else {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: 'Hubo un problema verificar',
                        icon: response.mensaje
                    })
                }
            }

        })
    } else {
        $.ajax({
            url: 'Usuario/Editar',
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
                        text: 'Se actualizo el usuario de forma correcta',
                        icon: 'success'
                    })
                    $("#modalData").modal("hide");

                    ListarUsuarios();
                } else {
                    Swal.fire({
                        title: 'MENSAJE',
                        text: 'Hubo un problema verificar',
                        icon: response.mensaje
                    })
                }
            }

        })
    }
   


})
function eliminarUsuario(idUsuario) {
    debugger;
    Swal.fire({
        title: "MENSAJE",
        text: "Desea eliminar el usuario!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "SI"    
    }).then((result) => {

        if (result.isConfirmed) {
            $.ajax({
                url: 'Usuario/Eliminar',
                type: 'delete',
                data: { idUsuario: idUsuario },
                dataType: 'json',
                success: function (response) {

                    if (response.estado) {
                        Swal.fire({
                            title: 'MENSAJE',
                            text: 'Se elimino el usuario de forma correcta',
                            icon: 'success'
                        })
                        ListarUsuarios();
                    } else {
                        Swal.fire({
                            title: 'MENSAJE',
                            text: reponse.mensaje,
                            icon: 'success'
                        })
                    }


                }
            })
        }
     
    });
}







