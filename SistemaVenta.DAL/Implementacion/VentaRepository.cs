using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbContext;

        public VentaRepository(DbventaContext dbContext) :base(dbContext) {

            _dbContext = dbContext;
        }
        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();

            using (var transaccion =  _dbContext.Database.BeginTransaction()){


                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad;

                        _dbContext.Productos.Update(productoEncontrado);
                    }

                    await _dbContext.SaveChangesAsync();


                    NumeroCorrelativo numCorrelativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion.Equals("Gestion")).First();
                    numCorrelativo.UltimoNumero = numCorrelativo.UltimoNumero + 1;
                    numCorrelativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(numCorrelativo);
                    await _dbContext.SaveChangesAsync();


                    string ceros = string.Concat(Enumerable.Repeat("0", numCorrelativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + numCorrelativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - numCorrelativo.CantidadDigitos.Value, numCorrelativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta;

                    await _dbContext.Venta.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();


                    ventaGenerada = entidad;

                    transaccion.Commit();

                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    throw;
                }
            
            }

            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>>Reporte(DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                List<DetalleVenta> entidad = await _dbContext.DetalleVenta.
                    Include(v => v.IdVentaNavigation).
                    ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation).
                    Include(v => v.IdVentaNavigation).
                    ThenInclude(u => u.IdUsuarioNavigation)
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro <= fechaInicio.Date && dv.IdVentaNavigation.FechaRegistro >= fechaFinal.Date).ToListAsync();

                return entidad;


            }catch(Exception ex)
            {
                throw;
            }
        }
    }
}
