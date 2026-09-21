using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Implementacion
{
    public class RolService : IRolServices
    {
        private readonly IGenericRepository<Rol> _repository;

        public RolService(IGenericRepository<Rol> repository)
        {
            _repository = repository;
        }
        public async Task<List<Rol>> listar()
        {
            try
            {
                IQueryable<Rol> queryRol = await _repository.Consultar();
                return queryRol.ToList();
            }
            catch (Exception ex) {

                throw;
            }
        
        }
    }
}
