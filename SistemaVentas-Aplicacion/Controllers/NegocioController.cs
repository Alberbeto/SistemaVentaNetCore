using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Interfaces;
using SistemaVentas_Aplicacion.Utilidades.Response;
using SistemaVentas_Aplicacion.Models.VModels;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVentas_Aplicacion.Controllers
{


    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioservice;

        private readonly IFireBaseService _firebaseservice;

        public NegocioController(IMapper mapper, INegocioService negocioService, IFireBaseService firebaseservice)
        {
            _mapper = mapper;
            _negocioservice = negocioService;
            _firebaseservice = firebaseservice;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> gresponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio vMNegocio = _mapper.Map<VMNegocio>(await _negocioservice.Obtener());

                gresponse.Estado = true;
                gresponse.Objeto = vMNegocio;
            }
            catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gresponse);
        }

        [HttpPost]

        public async Task<IActionResult> Crear([FromForm] IFormFile logo, [FromForm] string modelo)
        {
            GenericResponse<VMNegocio> grenponse = new GenericResponse<VMNegocio>();
            try
            {
                VMNegocio vmnegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);
                string nombrefoto = "";
                Stream fotoStream = null;

                if (logo != null)
                {
                    string nombre_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombrefoto = string.Concat(nombre_codigo, extension);
                    fotoStream = logo.OpenReadStream();
                }

                Negocio negocio_creado = await _negocioservice.Crear(_mapper.Map<Negocio>(vmnegocio), fotoStream, nombrefoto);
                vmnegocio = _mapper.Map<VMNegocio>(negocio_creado);

                grenponse.Estado = true;
                grenponse.Objeto = vmnegocio;

            }
            catch(Exception ex)
            {
                grenponse.Estado = false;
                grenponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, grenponse);
        }
    }
}
