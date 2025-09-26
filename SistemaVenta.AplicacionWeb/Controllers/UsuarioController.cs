using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;
        private readonly IMapper _mapper;
        public UsuarioController(IUsuarioService usuarioServicio, IRolService rolServicio, IMapper mapper)
        {
            _usuarioServicio = usuarioServicio;
            _mapper = mapper;
            _rolServicio = rolServicio;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await _rolServicio.Lista(); // Obtenemos la lista de roles desde el servicio
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(lista);// Mapeamos la lista de roles a la vista modelo VMRol
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            var lista = await _usuarioServicio.Lista(); // Obtenemos la lista de usuarios desde el servicio
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(lista); // Mapeamos la lista de usuarios a la vista modelo VMUsuario
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista }); // Retornamos la lista de usuarios en formato JSON con código de estado 200 OK
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto,[FromForm]string modelo) // Metodo para crear un usuario
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo); // Se derializa el modelo JSON a VMUsuario

                string nombreFoto = "";// Variable para almacenar el nombre de la foto
                Stream fotoStream = null; // Variable para almacenar el stream de la foto

                if (foto != null){
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");// Generamos un nombre único para la foto
                    string extension = Path.GetExtension(foto.FileName); // Obtenemos la extensión de la foto
                    nombreFoto = string.Concat(nombre_en_codigo, extension); // Concatenamos el nombre único con la extensión
                    fotoStream = foto.OpenReadStream(); // Abrimos el stream de la foto
                }
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]"; // URL de la plantilla de correo para enviar la clave al usuario

                Usuario usuario_creado = await _usuarioServicio.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo); // Llamamos al servicio para crear el usuario

                vmUsuario = _mapper.Map<VMUsuario>(usuario_creado); // Enviar respuesta a la vista modelo VMUsuario
                gResponse.Estado = true; // Indicamos que la operación fue exitosa
                gResponse.Objeto = vmUsuario; // Asignamos el usuario creado a la respuesta
            }
            catch (Exception ex)
            {
                gResponse.Estado = false; // Indicamos que hubo un error
                gResponse.Mensaje = ex.Message; // Asignamos el mensaje de error a la respuesta
            }
            return StatusCode(StatusCodes.Status200OK, gResponse); // Retornamos la respuesta genérica con código de estado 200 OK
        }
    [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo) // Metodo para crear un usuario
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo); // Se derializa el modelo JSON a VMUsuario

                string nombreFoto = "";// Variable para almacenar el nombre de la foto
                Stream fotoStream = null; // Variable para almacenar el stream de la foto

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");// Generamos un nombre único para la foto
                    string extension = Path.GetExtension(foto.FileName); // Obtenemos la extensión de la foto
                    nombreFoto = string.Concat(nombre_en_codigo, extension); // Concatenamos el nombre único con la extensión
                    fotoStream = foto.OpenReadStream(); // Abrimos el stream de la foto
                }
                Usuario Usuario_editato = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto); // Llamamos al servicio para crear el usuario

                vmUsuario = _mapper.Map<VMUsuario>(Usuario_editato); // Enviar respuesta a la vista modelo VMUsuario
                gResponse.Estado = true; // Indicamos que la operación fue exitosa
                gResponse.Objeto = vmUsuario; // Asignamos el usuario creado a la respuesta
            }
            catch (Exception ex)
            {
                gResponse.Estado = false; // Indicamos que hubo un error
                gResponse.Mensaje = ex.Message; // Asignamos el mensaje de error a la respuesta
            }
            return StatusCode(StatusCodes.Status200OK, gResponse); // Retornamos la respuesta genérica con código de estado 200 OK
        }
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdUsuario) {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
              gResponse.Estado = await _usuarioServicio.Eliminar(IdUsuario);// Llamamos al servicio para eliminar el usuario por su ID
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
                        
    }
}
