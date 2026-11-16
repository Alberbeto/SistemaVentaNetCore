using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IDashboardServices
    {
        Task<int> TotalVentasUltimasSemana();
        Task<string> TotalIngresoUltimaSemana();

        Task<int> TotalProductos();

        Task<int> TotalCategorias();

        Task<Dictionary<string, int>> VentasUltimasSemana();

        Task<Dictionary<string, int>> ProductoTopUltimasSemanas();
    }
}
