using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaVenta.BLL.Implementacion
{
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repositorio;
        private readonly IFireBaseService _firebase;

        public NegocioService(IGenericRepository<Negocio> repositorio, IFireBaseService firebase)
        {
            _repositorio = repositorio;
            _firebase = firebase;
        }

        public async Task<Negocio> Obtener()
        {
            try{

                Negocio obtener_negocio = await _repositorio.Obtener(n => n.IdNegocio == 1);

                return obtener_negocio;
            }
            catch(Exception ex)
            {
                throw;
            }
           
        }

        public async Task<Negocio> Crear(Negocio entidad, Stream fotostream, string nombreArchivo)
        {
            try
            {
                Negocio negocio_encontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);

                negocio_encontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocio_encontrado.Nombre = entidad.Nombre;
                negocio_encontrado.Correo = entidad.Correo;
                negocio_encontrado.Direccion = entidad.Direccion;
                negocio_encontrado.Telefono = entidad.Telefono;
                negocio_encontrado.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                negocio_encontrado.SimboloMoneda = entidad.SimboloMoneda;
                negocio_encontrado.NombreLogo = entidad.NombreLogo == "" ? nombreArchivo : negocio_encontrado.NombreLogo;
                if (fotostream != null)
                {
                    string urlimagen = await _firebase.SubirStorage(fotostream, "carpeta_logo", nombreArchivo);
                    negocio_encontrado.UrlLogo = urlimagen;
                }

                await _repositorio.Editar(negocio_encontrado);
                if (negocio_encontrado.IdNegocio == 0)
                {
                    throw new Exception("No se pudo actualizar el negocio");
                }

                return negocio_encontrado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       
    }
}
