using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net;
using System.IO;


using Microsoft.EntityFrameworkCore;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.DAL.Interfaces;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class PersonalPolicialService : IPersonalPolicialService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<PersonalPolicial> _repositorio;
        //private readonly IGenericRepository<Domicilio> _repositorioDomicilio;
        //private readonly IGenericRepository<Arma> _repositorioArma;
        private readonly IFireBaseService _fireBaseService;

        // Constructor de la clase PersonalPolicialService
        public PersonalPolicialService(
            IGenericRepository<PersonalPolicial> repositorio,
            IGenericRepository<Domicilio> repositorioDomicilio,
            IGenericRepository<Arma> repositorioArma,
            IFireBaseService fireBaseService
            )
        {
            _repositorio = repositorio;
            //_repositorioDomicilio = repositorioDomicilio;
            //_repositorioArma = repositorioArma;
            _fireBaseService = fireBaseService;

        }

        //Lógica del método listar PersonalPolicial Activo
        public async Task<List<PersonalPolicial>> Lista()
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            query = query
                .Where(p => p.Trasladado == false) // activos
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation);

            return await query.ToListAsync();
        }

        //Lógica del método listar PersonalPolicial Trasladados (eliminados lógicamente)
        public async Task<List<PersonalPolicial>> ListaTrasladados()
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            query = query
                .Where(p => p.Trasladado == true) // trasladados
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation);

            return await query.ToListAsync();
        }

        // ✅ METODO para Crear un NUEVO PERSONAL POLICIAL (registrar nuevo)
        public async Task<PersonalPolicial> Crear(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {
                // Valores por defecto
                entidad.Trasladado = false;
                entidad.FechaEliminacion = null;
                entidad.NombreImagen = NombreFoto;

                // ✅ Subida de imagen a Firebase
                if (Foto != null)
                {
                    string urlImagen = await _fireBaseService.SubirStorage(Foto, "carpeta_personal", NombreFoto);
                    entidad.UrlImagen = urlImagen;
                }

                // ✅ Normalizar valores en Armas antes de guardar
                if (entidad.Armas != null && entidad.Armas.Any())
                {
                    foreach (var arma in entidad.Armas)
                    {
                        arma.NumeroSerie = string.IsNullOrEmpty(arma.NumeroSerie) ? "No Provista" : arma.NumeroSerie;
                        arma.Marca = string.IsNullOrEmpty(arma.Marca) ? "No Provista" : arma.Marca;
                    }
                }

                // ✅ Crear Personal con hijos (Armas + Domicilios) en UNA sola operación
                PersonalPolicial personalCreado = await _repositorio.Crear(entidad);

                if (personalCreado.IdPersonal == 0)
                    throw new TaskCanceledException("No se pudo crear el personal policial.");

                // ✅ Traer la entidad ya con sus relaciones
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == personalCreado.IdPersonal);

                personalCreado = await query
                    .Include(p => p.Domicilios)
                    .Include(p => p.Armas)
                    .Include(p => p.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                return personalCreado ?? throw new Exception("El personal policial no pudo ser recuperado después de crearse.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el personal policial: " + ex.Message, ex);
            }
        }


        // ✅ MÉTODO para Editar un PERSONAL POLICIAL existente
        public async Task<PersonalPolicial> Editar(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {
                // ✅ Buscar la entidad actual en BD
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == entidad.IdPersonal);

                PersonalPolicial personalExistente = await query
                    .Include(p => p.Domicilios)
                    .Include(p => p.Armas)
                    .Include(p => p.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (personalExistente == null)
                    throw new Exception("El personal policial no existe en la base de datos.");

                // ✅ Actualizar propiedades principales
                personalExistente.Legajo = entidad.Legajo;
                personalExistente.ApellidoYnombre = entidad.ApellidoYnombre;
                personalExistente.Grado = entidad.Grado;
                personalExistente.Chapa = entidad.Chapa;
                personalExistente.Sexo = entidad.Sexo;
                personalExistente.Funcion = entidad.Funcion;
                personalExistente.Horario = entidad.Horario;
                personalExistente.SituacionRevista = entidad.SituacionRevista;
                personalExistente.FechaNacimiento = entidad.FechaNacimiento;
                personalExistente.Telefono = entidad.Telefono;
                personalExistente.TelefonoEmergencia = entidad.TelefonoEmergencia;
                personalExistente.Dni = entidad.Dni;
                personalExistente.SubsidioSalud = entidad.SubsidioSalud;
                personalExistente.EstudiosCurs = entidad.EstudiosCurs;
                personalExistente.EstadoCivil = entidad.EstadoCivil;
                personalExistente.Especialidad = entidad.Especialidad;
                personalExistente.AltaEnDivision = entidad.AltaEnDivision;
                personalExistente.AltaEnPolicia = entidad.AltaEnPolicia;
                personalExistente.DestinoAnterior = entidad.DestinoAnterior;
                personalExistente.Email = entidad.Email;

                // ✅ Manejo de imagen (si viene nueva foto)
                if (Foto != null)
                {
                    string urlImagen = await _fireBaseService.SubirStorage(Foto, "carpeta_personal", NombreFoto);
                    personalExistente.NombreImagen = NombreFoto;
                    personalExistente.UrlImagen = urlImagen;
                }

                // ✅ Actualizar Armas existentes
                foreach (var arma in entidad.Armas)
                {
                    var armaExistente = personalExistente.Armas.FirstOrDefault(a => a.IdArma == arma.IdArma);
                    if (armaExistente != null)
                    {
                        armaExistente.Marca = string.IsNullOrEmpty(arma.Marca) ? "No Provista" : arma.Marca;
                        armaExistente.NumeroSerie = string.IsNullOrEmpty(arma.NumeroSerie) ? "No Provista" : arma.NumeroSerie;
                    }
                }

                // ✅ Actualizar Domicilios existentes
                foreach (var dom in entidad.Domicilios)
                {
                    var domExistente = personalExistente.Domicilios.FirstOrDefault(d => d.IdDomicilio == dom.IdDomicilio);
                    if (domExistente != null)
                    {
                        domExistente.CalleBarrio = string.IsNullOrEmpty(dom.CalleBarrio) ? "No Registrado" : dom.CalleBarrio;
                        domExistente.Localidad = string.IsNullOrEmpty(dom.Localidad) ? "No Registrado" : dom.Localidad;
                        domExistente.ComisariaJuris = string.IsNullOrEmpty(dom.ComisariaJuris) ? "No Registrado" : dom.ComisariaJuris;
                    }
                }

                // ✅ Guardar cambios
                bool respuesta = await _repositorio.Editar(personalExistente);

                if (!respuesta)
                    throw new Exception("No se pudo actualizar el personal policial.");

                // ✅ Recargar la entidad actualizada con sus relaciones
                IQueryable<PersonalPolicial> queryFinal = await _repositorio.Consultar(p => p.IdPersonal == personalExistente.IdPersonal);

                PersonalPolicial actualizado = await queryFinal
                    .Include(p => p.Domicilios)
                    .Include(p => p.Armas)
                    .Include(p => p.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (actualizado == null)
                    throw new Exception("El personal policial se actualizó pero no pudo ser recuperado.");

                return actualizado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el personal policial: " + ex.Message, ex);
            }
        }

        public async Task<bool> Trasladar(int idPersonal, int idUsuario)
        {
            try
            {
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == idPersonal);
                PersonalPolicial personalExistente = await query.FirstOrDefaultAsync();

                if (personalExistente == null)
                    throw new Exception("El personal policial no existe en la base de datos.");

                // 👇 Aquí aplicamos el traslado
                personalExistente.Trasladado = true;
                personalExistente.FechaEliminacion = DateTime.Now;
                personalExistente.IdUsuario = idUsuario;

                return await _repositorio.Editar(personalExistente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al trasladar personal: " + ex.Message, ex);
            }
        }

        public async Task<bool> Restituir(int idPersonal, int idUsuario)
        {
            try
            {
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == idPersonal);
                PersonalPolicial personal = await query.FirstOrDefaultAsync();

                if (personal == null)
                    throw new Exception("El personal no existe.");

                // 👇 Marcar como no trasladado y registrar quién hizo la acción
                personal.Trasladado = false;
                personal.FechaEliminacion = null;
                personal.IdUsuario = idUsuario;

                return await _repositorio.Editar(personal);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al restituir personal: " + ex.Message, ex);
            }
        }








        public async Task<PersonalPolicial> ObtenerPorId(int idPersonal)
        {
            // Obtenemos IQueryable desde el repositorio
            var query = await _repositorio.Consultar(p => p.IdPersonal == idPersonal);

            // Incluimos las relaciones
            var personal = query
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefault(); // materializamos la consulta

            return personal;
        }



    }
}
