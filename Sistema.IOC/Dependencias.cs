using System;
using System.Collections.Generic;
using System.Text;
using SistemaVenta.DAL.DBContext;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.BLL.Implementacion;
using Azure.Core;

namespace SistemaVenta.IOC
{
    public static class Dependencias
    {
        public static void InyectarDependencias(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            service.AddScoped<IVentaRepository, VentaRepository>();
            service.AddScoped<ICorreoService, CorreoRepository>();
            service.AddScoped<IFireBaseService, FireBaseService>();
            service.AddScoped<IUtilidadesService, UtilidadesService>();
            service.AddScoped<IRolServices, RolService>();
            service.AddScoped<IUsuarioService, UsuarioService>();
        }


    }
}
