using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Implementacion
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _repositorio;

        public CategoriaService(IGenericRepository<Categoria> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<Categoria>> Lista()
        {

            try
            {
                IQueryable<Categoria> query = await _repositorio.Consultar();
                return query.ToList();
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        public async Task<Categoria> ObtenerPorId(int idCategoria)
        {
            try
            {
                Categoria obtenerCategoria = await _repositorio.Obtener(c => c.IdCategoria == idCategoria);
                return obtenerCategoria;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoria_encotrada = await _repositorio.Obtener(c => c.Descripcion == entidad.Descripcion);

                if(categoria_encotrada != null)
                {
                    throw new TaskCanceledException("La categoría ya existe");
                }

                Categoria categoria_creada = await _repositorio.Crear(entidad);

                if(categoria_creada.IdCategoria == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la categoría");
                }

                return categoria_creada;

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoria_encotrada = await _repositorio.Obtener(c => c.Descripcion == entidad.Descripcion && c.IdCategoria != entidad.IdCategoria);

                if (categoria_encotrada != null)
                {
                    throw new TaskCanceledException("la categoria no existe");
                }

               Categoria categoria_editada = await _repositorio.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                categoria_editada.Descripcion = entidad.Descripcion;
                categoria_editada.EsActivo = entidad.EsActivo;

                bool respuesta = await _repositorio.Editar(categoria_editada);


                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar la categoría");
                }

                return respuesta;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoria_encotrada = await _repositorio.Obtener(c => c.IdCategoria == idCategoria);

                if (categoria_encotrada == null)
                {
                    throw new TaskCanceledException("No se encontró la categoría");
                }

                bool respuesta = await _repositorio.Eliminar(categoria_encotrada);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo eliminar la categoría");
                }

                return respuesta;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

       
    }
}
