using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SistemaVenta.BLL.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositoryProducto;
        private readonly IVentaRepository _repositoryVenta;


        public VentaService(IGenericRepository<Producto> repositoryProducto, IVentaRepository repositoryVenta)
        {
            _repositoryProducto = repositoryProducto;
            _repositoryVenta = repositoryVenta;
        }
        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> queryProductos = await _repositoryProducto.Consultar(p =>
            p.EsActivo == true &&
            p.Stock > 0 &&
            string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda));

            return queryProductos.Include(p => p.IdCategoriaNavigation).ToList();
        }

      

        public async Task<Venta> Registar(Venta entidad)
        {
            try
            {
                return await _repositoryVenta.Registrar(entidad);

            }catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Venta>> Historial(string numeroVenta, string feachainicio, string fechafin)
        {
            IQueryable<Venta> query = await _repositoryVenta.Consultar();

            feachainicio = feachainicio is null ? "" : feachainicio;
            fechafin = fechafin is null ? "" : fechafin;

            if(feachainicio !="" && fechafin != ""){

                DateTime fech_inicio = DateTime.ParseExact(feachainicio, "dd/MM/yyyy", new CultureInfo("en-Pe"));
                DateTime fech_fin = DateTime.ParseExact(fechafin, "dd/MM/yyyy", new CultureInfo("en-Pe"));

                return query.Where(p =>
                p.FechaRegistro.Value.Date >= fech_inicio.Date &&
                p.FechaRegistro.Value.Date <= fech_fin.Date
                ).
                Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Include(un => un.IdUsuarioNavigation)
                .Include(dv => dv.DetalleVenta).ToList();

            }
            else
            {
                return query.Where(p =>
              p.NumeroVenta == numeroVenta
               ).
               Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
               .Include(un => un.IdUsuarioNavigation)
               .Include(dv => dv.DetalleVenta).ToList();
            }

        }
        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositoryVenta.Consultar(v=>v.NumeroVenta == numeroVenta);

            return query.Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Include(un => un.IdUsuarioNavigation)
                .Include(dv => dv.DetalleVenta).First();
        }

        public async Task<List<DetalleVenta>> Reporte(string feachainicio, string fechafin)
        {
            DateTime fech_inicio = DateTime.ParseExact(feachainicio, "dd/MM/yyyy", new CultureInfo("en-Pe"));
            DateTime fech_fin = DateTime.ParseExact(fechafin, "dd/MM/yyyy", new CultureInfo("en-Pe"));

            List<DetalleVenta> listaDetalle = await _repositoryVenta.Reporte(fech_inicio, fech_fin);
            return listaDetalle;

        }
    }
}
