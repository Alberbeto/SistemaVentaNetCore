using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IVentaService
    {
        Task<List<Producto>> ObtenerProductos(string busqueda);
        Task<Venta> Registar(Venta entidad);

        Task<List<Venta>> Historial(string numeroVenta, string feachainicio, string fechafin);

        Task<Venta> Detalle(string numeroVenta);

        Task<List<DetalleVenta>> Reporte(string feachainicio, string fechafin);
    }
}
