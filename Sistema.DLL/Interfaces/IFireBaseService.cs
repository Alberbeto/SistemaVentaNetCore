using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IFireBaseService
    {
        Task<string> SubirStorage(Stream StreamFoto, string CarpetaNombre, string FotoNombre);
        Task<bool> EliminarStorage(string CarpetaNombre, string NombreFoto);
    }
}
