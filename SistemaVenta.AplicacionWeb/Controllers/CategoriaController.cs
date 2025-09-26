using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategoriaServicio _categoriaServico;
        public CategoriaController(IMapper mapper, ICategoriaServicio categoriaServico)
        {
            _mapper = mapper;
            _categoriaServico = categoriaServico;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCategoria> vmCategoriaLista = _mapper.Map<List<VMCategoria>>( await _categoriaServico.Lista()); // Obtiene la lista de categorias desde el servicio y la mapea a una lista de VMCategoria
            return StatusCode(StatusCodes.Status200OK,new {data = vmCategoriaLista}); // Devuelve la lista de categorias
        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody]VMCategoria modelo)
        {
            GenericResponse<VMCategoria> genericResponse = new GenericResponse<VMCategoria>(); // Crea una respuesta genérica para manejar el estado y los mensajes de la operación
            try
            {
                Categoria categoria_creada = await _categoriaServico.Crear(_mapper.Map<Categoria>(modelo)); // Crea una nueva categoria usando el servicio y mapea el modelo a Categoria
                modelo = _mapper.Map<VMCategoria>(categoria_creada); // Mapea la categoria creada a VMCategoria

               genericResponse.Estado = true; // Indica que la operación fue exitosa
               genericResponse.Objeto = modelo; // Asigna el objeto creado a la respuesta genérica

            }
            catch (Exception ex){
                genericResponse.Estado = false; 
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> genericResponse = new GenericResponse<VMCategoria>(); // Crea una respuesta genérica para manejar el estado y los mensajes de la operación
            try
            {
                Categoria categoria_editada = await _categoriaServico.Editar(_mapper.Map<Categoria>(modelo)); // Edita una categoria existente usando el servicio y mapea el modelo a Categoria
                modelo = _mapper.Map<VMCategoria>(categoria_editada); // Mapea la categoria editada a VMCategoria

                genericResponse.Estado = true; // Indica que la operación fue exitosa
                genericResponse.Objeto = modelo; // Asigna el objeto creado a la respuesta genérica

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdCategoria)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();
            try { 
             genericResponse.Estado = await _categoriaServico.Eliminar(IdCategoria); // Elimina una categoria usando el servicio y asigna el estado a la respuesta genérica              
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
    }
}
