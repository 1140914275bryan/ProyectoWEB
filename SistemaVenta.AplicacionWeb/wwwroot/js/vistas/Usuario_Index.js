
const MODELO_BASE = {
    idUsuario: 0,
    nombre: "",
    correo: "",
    telefono: "",
    idRol: 0,
    esActivo: 1,   
    urlFoto: "",
}

let tablaData;

$(document).ready(function () {

    fetch("/Usuario/ListaRoles")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboRol").append(
                        $("<option>").val(item.idRol).text(item.descripcion)
                    )
                })
            }
        })

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": { // Ajax para obtener los datos del servidor
            "url":'/Usuario/ListaUsuarios',
            "type": "GET",
            "datatype": "json"
        },         
         "columns": [
             { "data": "idUsuario","visible":false,"searchable":false},
             {
                 "data": "urlFoto", render: function (data) {
                     return `<img style=height:60px src=${data} class="rounded mx-auto d-block"/>`;
             }
         },
             { "data": "nombre" },
             { "data": "correo" },
             { "data": "telefono" },
             { "data": "nombreRol" },
             {
                 "data": "esActivo", render: function (data) {
                    if(data) 
                        return `<span class="badge badge-info">Activo</span>`;
                        else 
                        return `<span class="badge badge-danger">No Activo</span>`;   
                    }
             },
             {
                 "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                     '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                 "orderable": false,
                 "searchable": false,
                 "width": "80px"
             }
         ],
         order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [2,3,4,5,6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idUsuario)
    $("#txtNombre").val(modelo.nombre)
    $("#txtCorreo").val(modelo.correo)
    $("#txtTelefono").val(modelo.telefono)
    $("#cboRol").val(modelo.idRol == 0 ? $("#cboRol option:first").val() : modelo.idRol)
    $("#cboEstado").val(modelo.esActivo)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src", modelo.urlFoto)

    $("#modalData").modal("show")
}
    $("#btnNuevo").click(function() {
        mostrarModal()   
    })

$("#btnGuardar").click(function (){

    //debugger;

    const inputs = $("input.input-validad").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idUsuario"] = parseInt($("#txtId").val())
    modelo["nombre"] = $("#txtNombre").val()
    modelo["correo"] = $("#txtCorreo").val()
    modelo["telefono"] = $("#txtTelefono").val()
    modelo["idRol"] = $("#cboRol").val()
    modelo["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtFoto");

    const formData = new FormData();
    formData.append("foto", inputFoto.files[0]);
    formData.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idUsuario == 0) {
        fetch("/Usuario/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo", "Usuario creado correctamente", "success")

                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Usuario/Editar", { // Enviar los datos del usuario para editar
            method: "PUT", // Actualizar el usuario
            body: formData // Enviar los datos del usuario
        })
            .then(response => { //  Procesar la respuesta del servidor
                $("#modalData").find("div.modal-content").LoadingOverlay("hide"); // Ocultar el overlay de carga
                return response.ok ? response.json() : Promise.reject(response); // Verificar si la respuesta es correcta
            })
            .then(responseJson => { // Procesar la respuesta JSON 
                if (responseJson.estado) { // Si la respuesta es exitosa
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false) // Actualizar la fila seleccionada con los nuevos datos
                    filaSeleccionada = null; // Limpiar la fila seleccionada
                    $("#modalData").modal("hide") // Ocultar el modal
                    swal("Listo", "Usuario editado correctamente", "success") // Mostrar mensaje de éxito
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error") // Mostrar mensaje de error
                }
            })
    }
})

let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () { // Evento para editar un usuario

    if ($(this).closest("tr").hasClass("child")) { // Si la fila es una fila hija, seleccionamos la fila padre
        filaSeleccionada = $(this).closest("tr").prev(); // Seleccionamos la fila padre
    }else {
        filaSeleccionada = $(this).closest("tr"); // Seleccionamos la fila actual
    }

    const data = tablaData.row(filaSeleccionada).data(); // Obtener los datos de la fila seleccionada
    mostrarModal(data); // Mostrar el modal con los datos del usuario seleccionado
}) 



$("#tbdata tbody").on("click", ".btn-eliminar", function () { // Evento para editar un usuario
    let fila;
    if ($(this).closest("tr").hasClass("child")) { // Si la fila es una fila hija, seleccionamos la fila padre
        filaSeleccionada = $(this).closest("tr").prev(); // Seleccionamos la fila padre
    } else {
        filaSeleccionada = $(this).closest("tr"); // Seleccionamos la fila actual
    }

    const data = tablaData.row(fila).data(); // Obtener los datos de la fila seleccionada
    swal({
        title: "¿Esta seguro?",
        text: `Eliminar al Usuario"${data.nombre}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, Eliminar",
        cancelButtonText: "No cancelar",
        closeOnConfirm: false,
        closeOnCancel:true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Usuario/Eliminar?IdUsuario=${data.idUsuario}`, { // Enviar la solicitud para eliminar el usuario
                    method: "DELETE", // Actualizar el usuario
                })
                    .then(response => { //  Procesar la respuesta del servidor

                        $(".showSweetAlert").LoadingOverlay("hide"); // Ocultar el overlay de carga
                        return response.ok ? response.json() : Promise.reject(response); // Verificar si la respuesta es correcta
                    })
                    .then(responseJson => { // Procesar la respuesta JSON 

                        if (responseJson.estado) { // Si la respuesta es exitosa
                            tablaData.row(fila).remove().draw() // Eliminar la fila seleccionada de la tabla

                            swal("Listo", "Usuario eliminado correctamente", "success") // Mostrar mensaje de éxito
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error") // Mostrar mensaje de error
                        }
                    })
            }
        }
    )
}) 