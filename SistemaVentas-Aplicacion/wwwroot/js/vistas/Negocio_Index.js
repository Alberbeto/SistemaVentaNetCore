const Model_negocio = {
   
    urlLogo : "",
    numeroDocumento : "", 
    nombre :"",
    correo :"",
    direccion : "",
    telefono :"",
    porcentajeImpuesto :0,
    simboloMoneda : ""
}

$(document).ready(function () {


    listarNegocio();

});


function listarNegocio() {


    $.ajax({

        url: '/Negocio/Obtener',
        type: 'get',
        dataType: 'json',
        success: function (response) {

            if (response.estado) {

                var datos = response.objeto;
               
                $("#txtNumeroDocumento").val(datos.numeroDocumento);
                $("#txtRazonSocial").val(datos.nombre);
                $("#txtCorreo").val(datos.correo);
                $("#txtDireccion").val(datos.direccion);
                $("#txTelefono").val(datos.telefono);
                $("#txtImpuesto").val(datos.porcentajeImpuesto);
                $("#txtSimboloMoneda").val(datos.simboloMoneda);

                $("#imgLogo").attr('src',datos.urlLogo);

            }
        }
      })

} 
    $("#btnGuardarCambios").on('click', function () {



        const modelo = structuredClone(Model_negocio);
        modelo["numeroDocumento"] = $("#txtNumeroDocumento").val();
        modelo["nombre"] = $("#txtRazonSocial").val();
        modelo["correo"] = $("#txtCorreo").val();
        modelo["direccion"] = $("#txtDireccion").val();
        modelo["telefono"] = $("#txTelefono").val();
        modelo["porcentajeImpuesto"] = $("#txtImpuesto").val();
        modelo["simboloMoneda"] = $("#txtSimboloMoneda").val();

        const inputFoto = document.getElementById("txtLogo");

        const formData = new FormData();
        formData.append("logo", inputFoto.files[0]);
        formData.append("modelo", JSON.stringify(modelo));
        


       

        $.ajax({

            url: '/Negocio/Crear',
            type: 'post',
            dataType: 'json',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
              
                if (response.estado) {
                    Swal.fire({

                        title: 'MENSAJE',
                        text: 'Se guardaron los cambios correctamente',
                        icon: 'success'

                    })
                } else {

                    Swal.fire({

                        title: 'MENSAJE',
                        text: response.mensaje,
                        icon: 'error'

                    })
                }
            }

  
        });
});





