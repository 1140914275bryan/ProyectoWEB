
const MODELO_BASE = {
    idCategoria: 0,
    descripcion: "",
    esActivo: 1,
}

let tablaData;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": { // Ajax para obtener los datos del servidor
            "url": '/Categoria/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCategoria", "visible": false, "searchable": false },
            { "data": "descripcion" },
            {
                "data": "esActivo", render: function (data) {
                    if (data)
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
                filename: 'Reporte Categorias',
                exportOptions: {
                    columns: [1,2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idCategoria)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#cboEstado").val(modelo.esActivo)

    $("#modalData").modal("show")
}
$("#btnNuevo").click(function () {
    mostrarModal()
})

$("#btnGuardar").click(function () {

    if ($("#txtDescripcion").val().trim() == "") {
        toastr.warning("", "Debe completar el campo : descripcion")
        $("#txtDescripcion").focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idCategoria"] = parseInt($("#txtId").val())
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["esActivo"] = $("#cboEstado").val()

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCategoria == 0) {
        fetch("/Categoria/Crear", {
            method: "POST",
            headers: { "Content-Type":"application/json;charset=utf-8"}, 
            body: JSON.stringify(modelo) // Enviar los datos del modelo como JSON
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo", "La categoria fue creada", "success")

                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Categoria/Editar", { // Enviar los datos de la categoria para editar
            method: "PUT", // Actualizar la categoria
            headers: { "Content-Type": "application/json;charset=utf-8" }, // Especificar el tipo de contenido como JSON
            body: JSON.stringify(modelo) // Enviar los datos del modelo como JSON
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
                    swal("Listo", "Categoria moficiada", "success") // Mostrar mensaje de éxito
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
    } else {
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
        text: `Eliminar Categoria"${data.descripcion}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, Eliminar",
        cancelButtonText: "No cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Categoria/Eliminar?IdCategoria=${data.idCategoria}`, { // Enviar la solicitud para eliminar el usuario
                    method: "DELETE", // Actualizar el usuario
                })
                    .then(response => { //  Procesar la respuesta del servidor

                        $(".showSweetAlert").LoadingOverlay("hide"); // Ocultar el overlay de carga
                        return response.ok ? response.json() : Promise.reject(response); // Verificar si la respuesta es correcta
                    })
                    .then(responseJson => { // Procesar la respuesta JSON 

                        if (responseJson.estado) { // Si la respuesta es exitosa
                            tablaData.row(fila).remove().draw() // Eliminar la fila seleccionada de la tabla

                            swal("Listo", "Categoria eliminada", "success") // Mostrar mensaje de éxito
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error") // Mostrar mensaje de error
                        }
                    })
            }
        }
    )
})