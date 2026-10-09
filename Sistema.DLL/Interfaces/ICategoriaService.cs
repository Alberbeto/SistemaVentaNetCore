using System;
using System.Collections.Generic;
using System.Text;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ICategoriaService
    {

        Task<List<Categoria>> Lista();
        Task<Categoria> Crear(Categoria entidad);
        Task<bool> Editar(Categoria entidad);
        Task<bool> Eliminar(int idCategoria);
        Task<Categoria> ObtenerPorId(int idCategoria);
    }
}
