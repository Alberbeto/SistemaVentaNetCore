using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Interfaces
{
    public interface INegocioService
    {
        Task<Negocio> Obtener();

        Task<Negocio> Crear(Negocio entidad, Stream fotostream, string nombreArchivo);
    }
}
