

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
                fila.append('<td><img src="' + item.urlFoto + '" alt="Foto" width="50" height="50"></td>');
                fila.append('<td>' + item.nombre + '</td>')
                fila.append('<td>' + item.correo + '</td>')
                fila.append('<td>' + item.telefono + '</td>')
                fila.append('<td>' + item.nombreRol + '</td>')
                fila.append('<td>' + (item.esActivo == 1 ? '<span class="badge bg-success">Activo</span>' : '<span class="badge bg-danger">Inactivo</span>') + '</td>')
                fila.append(
                    '<td>' +
                    '<div style="display:flex; gap:5px; justify-content:center;">' +
                    '<button type="button" class="btn btn-primary btn-editar" title="Editar">' +
                    '<i class="fas fa-edit"></i>' +
                    '</button>' +
                    '<button type="button" class="btn btn-danger btn-eliminar" title="Eliminar">' +
                    '<i class="fas fa-trash"></i>' +
                    '</button>' +
                    '</div>' +
                    '</td>'
                );


                tablaUsuario.row.add(fila).draw();


            });

        }
    })
}




