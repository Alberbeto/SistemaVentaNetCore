using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IProductoService
    {
        Task<List<Producto>> Lista();
        Task<Producto> Crear(Producto entidad, Stream fotoStream = null, string nombreFoto ="");

        Task<Producto> Editar(Producto entidad, Stream fotoStream = null, string nombreFoto = "");
        Task<bool> Eliminar(int idProducto);

        Task<Producto> ObtenerPorId(int idProducto);
    }
}
