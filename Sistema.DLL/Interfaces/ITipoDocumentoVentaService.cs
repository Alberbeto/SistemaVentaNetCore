using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ITipoDocumentoVentaService
    {
        Task<List<TipoDocumentoVenta>> Lista();
    }
}
