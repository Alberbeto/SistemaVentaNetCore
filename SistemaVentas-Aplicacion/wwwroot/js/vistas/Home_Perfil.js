const Modelo_Perfil = {

    correo: "",

    telefono: ""

   
}

$(document).ready(function() {


    obtenerUsuario();

})

function obtenerUsuario() {

    $.ajax({
        url: '/Home/ObtenerUsuario',
        dataType: 'json',
        type: 'get',
        success: function(response) {

            var datos = response;
           

            $("#txtNombre").val(datos.objeto.nombre);
            $("#txtCorreo").val(datos.objeto.correo);
            $("#txTelefono").val(datos.objeto.telefono);
            $("#txtRol").val(datos.objeto.nombreRol);
        }
    })


}

$("#btnGuardarCambios").on('click', function() {
 
    if ($("#txtCorreo").val() == "") {
        Swal.fire({
            'title': 'MENSAJE',
            'text': 'El campo correo no debe estar vacio',
            'icon':'error'
        })
        return;
    }


    if ($("#txTelefono").val() == "") {
        Swal.fire({
            'title': 'MENSAJE',
            'text': 'El campo telefono no debe estar vacio',
            'icon': 'error'
        })
        return;
    }

   

  
    const modelo = structuredClone(Modelo_Perfil);
    modelo["correo"] = $("#txtCorreo").val();
    
    modelo["telefono"] = $("#txTelefono").val();
    const formData = new FormData();
    formData.append("modelo", JSON.stringify(modelo));


    Swal.fire({
        title: "MENSAJE",
        text: "Desea realizar los cambios del perfil",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "SI"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Home/GuarPerfil',
                type: 'post',
                dataType: 'json',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {



                    if (response.estado) {
                        Swal.fire({
                            title: 'MENSAJE',
                            text: 'Se ha registrado actualizado correctamente los datos del perfil',
                            icon: 'success'
                        })

                        return;

                       

                    } else {
                        Swal.fire({
                            title: 'MENSAJE',
                            text: response.mensaje,
                            icon: 'error'
                        })
                        return;
                    }ñ
                }

            })
        }
    });

})


$("#btnCambiarClave").on('click',function() {
  
    if ($("#txtClaveActual").val() =="" || $("#txtClaveNueva").val()=="" || $("#txtConfirmarClave").val()=="") {

        Swal.fire({
            title: 'MENSAJE',
            text: 'Verificar que los campos no esten vacios',
            icon: 'error'
        })

        return;
    }

    if ( $("#txtClaveNueva").val() != $("#txtConfirmarClave").val()) {

        Swal.fire({
            title: 'MENSAJE',
            text: 'los claves no son iguales',
            icon: 'error'
        })

        return;
    }

    const modelo = {

        claveActual: $("#txtClaveActual").val(),
        claveNueva: $("#txtClaveNueva").val(),
        confirmarClave: $("#txtConfirmarClave").val()
    }

    $.ajax({
        url: '/Home/CambiarClave',
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(modelo),
        
        success: function(response) {

            if (response.estado) {
                Swal.fire({
                    title: 'MENSAJE',
                    text: 'Se ha registrado actualizado correctamente los datos del perfil',
                    icon: 'success'
                })
                $("#txtClaveActual, #txtClaveNueva, #txtConfirmarClave").val("");
                return;

            } else {
                Swal.fire({
                    title: 'MENSAJE',
                    text: response.mensaje,
                    icon: 'error'
                })
                return;
            }
        }

    })
})



