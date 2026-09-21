using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IRolServices
    {
        Task<List<Rol>> listar();

    }
}
