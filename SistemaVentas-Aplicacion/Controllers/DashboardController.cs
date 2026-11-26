using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVentas_Aplicacion.Models.VModels;
using SistemaVentas_Aplicacion.Utilidades.Response;

namespace SistemaVentas_Aplicacion.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardServices _dashboardservice;

        public DashboardController(IDashboardServices dashboardservice)
        {
            _dashboardservice = dashboardservice;
           
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gresponse = new GenericResponse<VMDashBoard>();
            try
            {
                VMDashBoard VMDashBoard = new VMDashBoard();
                VMDashBoard.TotalVentas = await _dashboardservice.TotalVentasUltimasSemana();
                VMDashBoard.TotalIngresos = await _dashboardservice.TotalIngresoUltimaSemana();
                VMDashBoard.TotalProductos = await _dashboardservice.TotalProductos();
                VMDashBoard.TotalCategorias = await _dashboardservice.TotalCategorias();

                List<VMVentasSemana> listaventasSemana = new List<VMVentasSemana>();
                List<VMProductosSemana> listaProductoSemana = new List<VMProductosSemana>();

                foreach(KeyValuePair<string,int> item in await _dashboardservice.VentasUltimasSemana())
                {
                    listaventasSemana.Add(new VMVentasSemana()
                    {
                        Fecha=item.Key,
                        Total = item.Value
                    });
                }

                foreach(KeyValuePair<string,int> item in await _dashboardservice.ProductoTopUltimasSemanas())
                {
                    listaProductoSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                VMDashBoard.VentasUltimaSemana = listaventasSemana;
                VMDashBoard.ProductosTopUltimaSemana = listaProductoSemana;

                gresponse.Estado = true;
                gresponse.Objeto = VMDashBoard;

            }catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }


            return StatusCode(StatusCodes.Status200OK, gresponse);
        }
    }
}
