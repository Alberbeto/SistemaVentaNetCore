using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVentas_Aplicacion.Models.VModels;
using SistemaVentas_Aplicacion.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Xml.Schema;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioservice;

        private readonly IRolServices _rolservice;
        public UsuarioController(IMapper mapper, IUsuarioService usuarioService, IRolServices rolService)
        {
            _mapper = mapper;
            _usuarioservice = usuarioService;
            _rolservice = rolService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListarRoles()
        {
            
                var lista = await _rolservice.listar();
                List<VMRol> vmLstaRoles = _mapper.Map<List<VMRol>>(lista);

                return StatusCode(StatusCodes.Status200OK, new { data = vmLstaRoles });

           
        }

        [HttpGet]

        public async Task<IActionResult> Lista()
        {
            List<VMUsuario> lstUsuario = _mapper.Map<List<VMUsuario>>(await _usuarioservice.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = lstUsuario });
        }

        [HttpPost]

        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gresponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";

                Stream fotoStream = null;

                if(foto != null)
                {
                    string nombre_encodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_encodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuarioCreado = await _usuarioservice.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);

                gresponse.Estado = true;
                gresponse.Objeto = vmUsuario;

                


            }catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }

        [HttpPut]

        public async Task<IActionResult> Editar([FromForm] IFormFile foto , [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gresponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if( foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuario_editado = await _usuarioservice.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = _mapper.Map<VMUsuario>(usuario_editado);

                gresponse.Estado = true;
                gresponse.Objeto = vmUsuario;

                
            }
            catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }


        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gresponse = new GenericResponse<string>();

            try
            {
                gresponse.Estado = await _usuarioservice.Eliminar(idUsuario);

            }catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }
    }
}
