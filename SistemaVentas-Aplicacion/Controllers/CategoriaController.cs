using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVentas_Aplicacion.Models.VModels;
using SistemaVentas_Aplicacion.Utilidades.Response;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly IMapper _mapper;

        public CategoriaController(ICategoriaService categoriaService, IMapper mapper)
        {
            _categoriaService = categoriaService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]

        public async Task<IActionResult> Listar()
        {
            try
            {
                var lista = _categoriaService.Lista();
                List<VMCategoria> vmListaCategoria = _mapper.Map<List<VMCategoria>>(await lista);
                return StatusCode(StatusCodes.Status200OK, new { data = vmListaCategoria });

            }
            catch(Exception ex)
            {
                throw;
            }
        
        }

        [HttpPost]

        public async Task<IActionResult> Obtener(int idCategoria)
        {
            try
            {
                VMCategoria categoriaObtenida = _mapper.Map<VMCategoria>(await _categoriaService.ObtenerPorId(idCategoria));
                return StatusCode(StatusCodes.Status200OK, new { data = categoriaObtenida });
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        [HttpPost]

        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMCategoria> gresponse = new GenericResponse<VMCategoria>();

            try
            {
                VMCategoria vmcategoria = JsonConvert.DeserializeObject<VMCategoria>(modelo);

                Categoria categoriacreada = await _categoriaService.Crear(_mapper.Map<Categoria>(vmcategoria));

                vmcategoria = _mapper.Map<VMCategoria>(categoriacreada);

                gresponse.Estado = true;
                gresponse.Objeto = vmcategoria;

            }
            catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gresponse);
        }



        [HttpPut]

        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            GenericResponse<VMCategoria> gresponse = new GenericResponse<VMCategoria>();

            try
            {
                VMCategoria vmcategoria = JsonConvert.DeserializeObject<VMCategoria>(modelo);

                bool categoria_Editada = await _categoriaService.Editar(_mapper.Map<Categoria>(vmcategoria));

               

                if(categoria_Editada)
                {
                    gresponse.Estado = true;
                    gresponse.Objeto = vmcategoria;
                }

               
               
            }
            catch (Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }

        [HttpDelete]


        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            GenericResponse<VMCategoria> gresponse = new GenericResponse<VMCategoria>();

            try
            {
                bool respuesta = await _categoriaService.Eliminar(idCategoria);

                if (respuesta)
                {
                    gresponse.Estado = true;

                }


            }
            catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }
    }
}
