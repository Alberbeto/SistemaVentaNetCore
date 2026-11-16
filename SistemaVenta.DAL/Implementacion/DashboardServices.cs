using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SistemaVenta.DAL.Implementacion
{
    public class DashboardServices : IDashboardServices
    {

        private readonly IVentaRepository _ventarepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVenta;
        private readonly IGenericRepository<Categoria> _repositorioCategoria;
        private readonly IGenericRepository<Producto> _repositorioProducto;

        private DateTime fechaInicio = DateTime.Now;

        public DashboardServices(IVentaRepository ventarepository, IGenericRepository<DetalleVenta> detalleVenta,
            IGenericRepository<Categoria> repositorioCategoria, IGenericRepository<Producto> repositorioProducto)
        {
            _ventarepository = ventarepository;
            _detalleVenta = detalleVenta;
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;
            fechaInicio = fechaInicio.AddDays(-7);
        }
        public async Task<int> TotalVentasUltimasSemana()
        {
            try
            {
                IQueryable<Venta> venta = await _ventarepository.Consultar(v => v.FechaRegistro.Value.Date <= fechaInicio.Date);
                int total = venta.Count();

                return total;

            }catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<string> TotalIngresoUltimaSemana()
        {
            try
            {
                IQueryable<Venta> venta = await _ventarepository.Consultar(v => v.FechaRegistro.Value.Date <= fechaInicio.Date);
            decimal resultado = venta
                    .Select(v=>v.Total)
                    .Sum(v=>v.Value);

                return Convert.ToString(resultado, new CultureInfo("es-pE"));

            }
            catch (Exception ex)
            {
                throw;
            }
        }
      

        public async Task<int> TotalCategorias()
        {
            try
            {
                IQueryable<Categoria> categoria = await _repositorioCategoria.Consultar();
                int total = categoria.Count();

                return total;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

      

        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> producto = await _repositorioProducto.Consultar();
                int total = producto.Count();

                return total;

            }catch(Exception ex)
            {
                throw;
            }
        }

       
        public async Task<Dictionary<string, int>> VentasUltimasSemana()
        {
            try { 
            
                IQueryable<Venta> query = await _ventarepository.Consultar(v => v.FechaRegistro.Value.Date <= fechaInicio.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

                return resultado;
                    
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> ProductoTopUltimasSemanas()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _detalleVenta.Consultar();

                //var resultado2 = query
                //    .Include(v => v.IdVentaNavigation)
                //    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaInicio.Date)
                //    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count())
                //    .Select(dv => new { producto = dv.Key, total = dv.Count() });

                Dictionary<string, int> resultado = query
                    .Include(v=>v.IdVentaNavigation)
                    .Where(dv=>dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaInicio.Date)
                    .GroupBy(dv =>dv.DescripcionProducto).OrderByDescending(g => g.Count())
                    .Select(dv => new { producto = dv.Key, total = dv.Count() }).Take(4)
                    .ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total);

                return resultado;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
