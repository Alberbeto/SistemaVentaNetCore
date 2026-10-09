using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVentas_Aplicacion.Models.VModels;
using SistemaVentas_Aplicacion.Utilidades.Response;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class ProductoController : Controller
    {

        private readonly IProductoService _productoService;
        private readonly IMapper _mapper;
        private readonly ICategoriaService _categoriaService;
       
        public ProductoController(IProductoService productoService, IMapper mapper, ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _mapper = mapper;
            _categoriaService = categoriaService;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Lista()
        {
            try
            {

                List<VMProducto> listProducto = _mapper.Map<List<VMProducto>>( await _productoService.Lista());
                return StatusCode(StatusCodes.Status200OK, new { data = listProducto });


            }catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet]


        public async Task<IActionResult> ListaCategoria()
        {
            try
            {
                List<VMCategoria> Listcategoria = _mapper.Map<List<VMCategoria>>(await _categoriaService.Lista());
                return StatusCode(StatusCodes.Status200OK, new { data = Listcategoria });

            }catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost]


        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gresponse = new GenericResponse<VMProducto>();

            try{

                VMProducto vmproducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
                string nombreFoto = "";
                Stream fotostream = null;
                if (foto != null)
                {
                    string codigo_foto = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(codigo_foto, extension);
                    fotostream = foto.OpenReadStream();
                }

                Producto productocreado = await _productoService.Crear(_mapper.Map<Producto>(vmproducto), fotostream, nombreFoto);
                vmproducto = _mapper.Map<VMProducto>(productocreado);

                gresponse.Estado =true;
                gresponse.Objeto = vmproducto;


            
            }catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }

        public async Task<IActionResult> Editar([FromForm] FormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gresponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto vmproducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;


                if (foto != null) {

                    string codigo_nombre = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(codigo_nombre, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Producto productoCreado = await _productoService.Editar(_mapper.Map<Producto>(vmproducto), fotoStream, nombreFoto);
                vmproducto = _mapper.Map<VMProducto>(productoCreado);

                gresponse.Estado = true;
                gresponse.Objeto = vmproducto;

            } catch (Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gresponse);
        }

        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<VMProducto> gresponse = new GenericResponse<VMProducto>();
            try
            {
                gresponse.Estado = await _productoService.Eliminar(idProducto);

                
            }catch(Exception ex)
            {

                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }
    }
}
