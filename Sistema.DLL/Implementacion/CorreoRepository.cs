using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using SistemaVenta.DAL.Interfaces;
using System.Net.Mail;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoRepository : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repository;

        public CorreoRepository(IGenericRepository<Configuracion> repository)
        {
            _repository = repository;
        }
        public async Task<bool> EnviarCorreo(string correoDestino, string asunto, string mensaje)
        {
            IQueryable<Configuracion> entidad = await _repository.Consultar(c => c.Recurso == "Servicio_Correo");
            Dictionary<string, string> config = entidad.ToDictionary(keySelector: p => p.Propiedad, elementSelector: p => p.Valor);

            var credendiales = new NetworkCredential(config["correo"], config["clave"]);

            var correo = new MailMessage()
            {

                From = new MailAddress(config["correo"], config["alias"]),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true,

            };

            correo.To.Add(new MailAddress(correoDestino));


            var clienteServidor = new SmtpClient()
            {
                Port = int.Parse(config["puerto"]),
                Host = config["host"],
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true
            };

            clienteServidor.Send(correo);

            return true;
            
        }
    }
}
