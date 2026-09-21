using SistemaVenta.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {

        public string GenerarClave()
        {
            try
            {
                string claveGenerada = Guid.NewGuid().ToString("N").Substring(0,6);

                return claveGenerada;

            }catch(Exception ex)
            {
                throw;
            }
        }
        public string ConvertiSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create()) { 
            
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result) { 
                
                
                   sb.Append(b.ToString("X2"));
                
                }
                    
            
            }

            return sb.ToString();
        }       
    }
}
