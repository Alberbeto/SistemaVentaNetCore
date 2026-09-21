using System;
using System.Collections.Generic;
using System.Text;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ICorreoService
    {
        Task<bool> EnviarCorreo(string correo,string asunto, string mensaje);
    }
}
