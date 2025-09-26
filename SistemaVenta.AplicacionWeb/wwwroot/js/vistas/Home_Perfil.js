
$(document).ready(function () {

    $(".container-fluid").LoadingOverlay("show");

    fetch("/Home/ObtenerUsuario")
        .then(response => {
            $(".container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto

                $("#imgFoto").attr("src", d.urlFoto)

                $("#txtNombre").val(d.nombre)
                $("#txtCorreo").val(d.correo)
                $("#txTelefono").val(d.telefono)
                $("#txtRol").val(d.nombreRol)         

            } else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
})

$("#btnGuardarCambios").click(function () {

        if ($("#txtCorreo").val().trim() == "") {
            toastr.warning("", "Debe completar el campo : correo ")
            $("#txtCorreo").focus()
            return;
        }
        if ($("#txTelefono").val().trim() == "") {
            toastr.warning("", "Debe completar el campo : Telefono ")
            $("#txTelefono").focus()
            return;
        }

        swal({
            title: "¿Desea Guardar Los Cambios?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-primary",
            confirmButtonText: "Si",
            cancelButtonText: "No",
            closeOnConfirm: false,
            closeOnCancel: true
        },
            function (respuesta) {
                if (respuesta) {
                    $(".showSweetAlert").LoadingOverlay("show");

                    let modelo = {
                        correo: $("#txtCorreo").val().trim(),
                        telefono: $("#txTelefono").val().trim()
                    }

                    fetch("/Home/GuardarPerfil", { // Enviar los datos del perfil para guardar
                        method: "POST", // Enviar los datos del perfil
                        headers: { "Content-Type": "application/json;charset=utf-8" }, // Especificar el tipo de contenido como JSON
                        body: JSON.stringify(modelo) // Enviar los datos del modelo como JSON
                    })
                        .then(response => { //  Procesar la respuesta del servidor

                            $(".showSweetAlert").LoadingOverlay("hide"); // Ocultar el overlay de carga
                            return response.ok ? response.json() : Promise.reject(response); // Verificar si la respuesta es correcta
                        })
                        .then(responseJson => { // Procesar la respuesta JSON 

                            if (responseJson.estado) { // Si la respuesta es exitosa                          

                                swal("Listo", "Los cambios fueron guardados", "success") // Mostrar mensaje de éxito
                            } else {
                                swal("Lo sentimos", responseJson.mensaje, "error") // Mostrar mensaje de error
                            }
                        })
                }
            }
        )
})


