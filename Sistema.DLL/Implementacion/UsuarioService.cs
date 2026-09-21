using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        public readonly IGenericRepository<Usuario> _repository;
        public readonly IFireBaseService _firebaseservice;
        public readonly ICorreoService _correoService;
        public readonly IUtilidadesService _utilidadesService;

        public UsuarioService(IGenericRepository<Usuario> repository, IFireBaseService firebaseservice
            , ICorreoService correoService, IUtilidadesService utilidadesService)
        {
            _repository = repository;
            _firebaseservice = firebaseservice;
            _correoService = correoService;
            _utilidadesService = utilidadesService;
        }
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> Listquery = await _repository.Consultar();
            return Listquery.Include(r => r.IdRolNavigation).ToList();
        }

       

        public async Task<Usuario> Crear(Usuario entidad, Stream FotoStream = null, string nombreFoto = "", string plantillaCorreoUrl = "")
        {
            Usuario usuarioEncontrado = await _repository.Obtener(u => u.Correo.Equals(entidad.Correo));

            if(usuarioEncontrado != null) {

                throw new TaskCanceledException("El correo ya Existe");           
            }

            try
            {
                string clavegenerada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertiSha256(clavegenerada);
                entidad.NombreFoto = nombreFoto;
                
               

                if (FotoStream != null)
                {
                    string urlFoto = await _firebaseservice.SubirStorage(FotoStream, "carpeta_usuario", entidad.NombreFoto);
                    entidad.UrlFoto = urlFoto;
                };

                Usuario nuevoUsuario = await _repository.Crear(entidad);
              
                if(nuevoUsuario.IdUsuario == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el usuario");
                }


                if (plantillaCorreoUrl != "")
                {
                    plantillaCorreoUrl = plantillaCorreoUrl.Replace("[correo]", nuevoUsuario.Correo).Replace("[clave]", nuevoUsuario.Clave);

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(plantillaCorreoUrl);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        using(Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader rstream = null;

                            if(response.CharacterSet == null)
                            {
                                rstream = new StreamReader(dataStream);
                            }
                            else
                            {
                                rstream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }

                            htmlCorreo = rstream.ReadToEnd();
                            response.Close();
                            rstream.Close();
                        }
                    }

                    if (htmlCorreo != "")
                    {
                        await _correoService.EnviarCorreo(nuevoUsuario.Correo, "Cuenta Creada", htmlCorreo);
                    }
                }

                IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == nuevoUsuario.IdUsuario);
                nuevoUsuario = query.Include(r => r.IdRolNavigation).First();

                return nuevoUsuario;

            }
            catch(Exception ex)
            {
                throw;
            }

        }

        public async Task<Usuario> Editar(Usuario entidad, Stream FotoStream = null, string nombreFoto = "")
        {
            Usuario usuarioEncontrado = await _repository.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if(usuarioEncontrado != null) {

                throw new TaskCanceledException("el correo ya existe");
            }

            try
            {
                IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                Usuario usuarioEditar = query.First();
                usuarioEditar.Nombre = entidad.Nombre;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.Telefono = entidad.Telefono;

                usuarioEditar.IdRol = entidad.IdRol;

                if (usuarioEditar.NombreFoto == "")
                {
                    usuarioEditar.NombreFoto = nombreFoto;
                }

                if (FotoStream != null)
                {
                    string url = await _firebaseservice.SubirStorage(FotoStream, "carpeta_usuario", usuarioEditar.NombreFoto);
                    usuarioEditar.UrlFoto = url;

                }

                bool respuesta = await _repository.Editar(usuarioEditar);

                if (!respuesta)
                {

                    throw new TaskCanceledException("No se Puede Editar el usuario");
                }

                Usuario usuarioEditado = query.Include(r => r.IdRolNavigation).First();

                return usuarioEditado;


            }
            catch(Exception ex)
            {
                throw;
            }

           
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await _repository.Obtener(u => u.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                {

                    throw new TaskCanceledException("No se Puede Eliminar el usuario");
                }

                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repository.Eliminar(usuario_encontrado);

                if (respuesta)
                {
                    await _firebaseservice.EliminarStorage("carpeta_usuario", nombreFoto);
                }

                return true;



            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada =  _utilidadesService.ConvertiSha256(clave);

            Usuario usuario_encontrado = await _repository.Obtener(u => u.Correo == correo && u.Clave == clave_encriptada);

            return usuario_encontrado;

        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == idUsuario);

            Usuario usuarioEncontrado = query.Include(r=>r.IdRolNavigation).First();

            return usuarioEncontrado;
        }
        
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await _repository.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if(usuario_encontrado == null)
                {
                    throw new TaskCanceledException("el usuario no existe");
                }

                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repository.Editar(usuario_encontrado);

                return respuesta;


            }catch(Exception ex)
            {
                throw;
            }
        }


        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuario_encontrado = await _repository.Obtener(u => u.IdUsuario == idUsuario);

                if(usuario_encontrado == null)
                {
                    throw new TaskCanceledException("el usuario no existe");
                }

                if (usuario_encontrado.Clave != _utilidadesService.ConvertiSha256(claveActual))
                {
                    throw new TaskCanceledException("la contraseña ingresada es incorrecta");
                }

                usuario_encontrado.Clave = _utilidadesService.ConvertiSha256(claveNueva);

                bool respuesta = await _repository.Editar(usuario_encontrado);

                return respuesta;

            }catch(Exception ex)
            {
                throw;
            }
        }



        public async Task<bool> RestablecerClave(string correo, string plantillaCorreoUrl)
        {
            try
            {
                Usuario usuario_encontrado = await _repository.Obtener(u => u.Correo == correo);

                if (usuario_encontrado == null)
                {
                    throw new TaskCanceledException("No encontramos al usuario ningun usuario asociado al correo");
                }

                string clave_generada = _utilidadesService.GenerarClave();

                usuario_encontrado.Clave = _utilidadesService.ConvertiSha256(clave_generada);

                plantillaCorreoUrl = plantillaCorreoUrl.Replace("[clave]", clave_generada);

                string htmlCorreo = "";



                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(plantillaCorreoUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if(response.StatusCode == HttpStatusCode.OK)
                {
                    using(Stream dataStream = response.GetResponseStream()){

                        StreamReader streamReader = null;

                        if(response.CharacterSet == null)
                        {
                            streamReader = new StreamReader(dataStream);
                        }
                        else
                        {
                            streamReader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                        }

                        htmlCorreo = streamReader.ReadToEnd();
                        response.Close();
                        streamReader.Close();
                    }
                }

                bool correo_enviado = false;

                if (htmlCorreo != "")
                {
                    await _correoService.EnviarCorreo(correo, "Contrasela reestablecida", htmlCorreo);
                    
                }

                if (!correo_enviado)
                {
                    throw new TaskCanceledException("Tenemos problemas por favor intentalo denuevo mas tarde");

                }

                bool respuesta = await _repository.Editar(usuario_encontrado);

                return respuesta;


            }
            catch(Exception ex)
            {
                throw;
            }
        }

       
    }
}
