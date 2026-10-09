using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repository;
        private readonly IFireBaseService _firebaservice;


        public ProductoService(IGenericRepository<Producto> repository, IFireBaseService firebaservice)
        {
            _repository = repository;
            _firebaservice = firebaservice;
        }

        public async Task<List<Producto>> Lista()
        {
            try
            {
                IQueryable<Producto> queryList = await _repository.Consultar();
                return queryList.Include(c => c.IdCategoriaNavigation).ToList();

            }catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Producto> ObtenerPorId(int idProducto)
        {
            try
            {
                IQueryable<Producto> queryProducto = await _repository.Consultar(p => p.IdProducto == idProducto);
                Producto productoencontrado =  queryProducto.Include(c=>c.IdCategoriaNavigation).First();
                return productoencontrado;

            }catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<Producto> Crear(Producto entidad, Stream fotoStream = null, string nombreFoto="")
        {
            try
            {
                Producto producto_encontrado = await _repository.Obtener(p => p.Descripcion == entidad.Descripcion);
                if(producto_encontrado != null)
                {
                    throw new TaskCanceledException("el producto ya se exite");
                }

                

                if(fotoStream != null)
                {
                    string url = await _firebaservice.SubirStorage(fotoStream, "carpeta_producto", nombreFoto);
                    entidad.UrlImagen = url;
                }

                Producto producto_creado = await _repository.Crear(entidad);

                if(producto_creado.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se puedo crear el producto");
                }

                IQueryable<Producto> query = await _repository.Consultar(p => p.IdProducto == producto_creado.IdProducto);
                producto_creado = query.Include(p => p.IdCategoriaNavigation).First();

                return producto_creado;

            } catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Producto> Editar(Producto entidad, Stream fotoStream = null, string nombreFoto = "")
        {
            try
            {
                Producto producto_encontrado = await _repository.Obtener(p => p.Descripcion == entidad.Descripcion && p.IdProducto != entidad.IdProducto);

                if(producto_encontrado == null)
                {
                    throw new TaskCanceledException("El producto no se encuentra");
                }

                IQueryable<Producto> query = await _repository.Consultar(p => p.IdProducto == entidad.IdProducto);
                Producto productoEditar = query.First();
                productoEditar.CodigoBarra = entidad.CodigoBarra;
                productoEditar.Marca = entidad.Marca;
                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.IdCategoria = entidad.IdCategoria;
                productoEditar.Stock = entidad.Stock;
                productoEditar.Precio = entidad.Precio;
                productoEditar.EsActivo = entidad.EsActivo;
               

                if(fotoStream != null)
                {
                    string url = await _firebaservice.SubirStorage(fotoStream, "carpeta_producto", producto_encontrado.NombreImagen);
                    producto_encontrado.UrlImagen = url;
                }

                bool respuesta = await _repository.Editar(producto_encontrado);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el producto");
                }

                Producto ProductoEditado = query.Include(p => p.IdCategoriaNavigation).First();
                return ProductoEditado;

            }catch(Exception ex)
            {
                throw;

            }
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto producto_encontrado = await _repository.Obtener(p => p.IdProducto == idProducto);

                if(producto_encontrado == null)
                {
                    throw new TaskCanceledException("El producto no existe");
                }
                string nombrefoto = producto_encontrado.NombreImagen;

                bool respuesta = await _repository.Eliminar(producto_encontrado);


                if (respuesta)
                {
                    await _firebaservice.EliminarStorage("carpeta_producto", nombrefoto);
                }

                return true;
            }catch(Exception ex)
            {
                throw;
            }
            
        }

        
    }
}
