using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.DAL.DBContext;
using GestionAdminPolicial.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace GestionAdminPolicial.DAL.Implementacion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        
        private readonly GestionOfPolicialContext _dbcontext;

        public GenericRepository(GestionOfPolicialContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                TEntity entidad = await _dbcontext.Set<TEntity>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch 
            {
                // Si encuentra un error, que lo devuelva --> ;)
                throw;
            }
        }
        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                _dbcontext.Set<TEntity>().Add(entidad);
                await _dbcontext.SaveChangesAsync();
                return entidad;
            }
            catch
            {
                // Si encuentra un error, que lo devuelva --> ;)
                throw;
            }
        }

        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _dbcontext.Update(entidad);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Si encuentra un error, que lo devuelva --> ;)
                throw;
            }
        }

        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _dbcontext.Remove(entidad);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Si encuentra un error, que lo devuelva --> ;)
                throw;
            }
        }

        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            IQueryable<TEntity> queryEntidad = filtro == null?_dbcontext.Set<TEntity>() :
                _dbcontext.Set<TEntity>().Where(filtro);
            return queryEntidad;
        }

        public async Task<bool> Existe(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                // AsNoTracking asegura que lea siempre desde la base y no desde el contexto en memoria
                return await _dbcontext.Set<TEntity>().AsNoTracking().AnyAsync(filtro);
            }
            catch
            {
                throw;
            }
        }

        // Método PATCH: actualizar solo un campo
        public async Task<bool> ActualizarCampo(TEntity entidad, Expression<Func<TEntity, object>> property)
        {
            var entry = _dbcontext.Entry(entidad);

            if (entry.State == EntityState.Detached)
            {
                _dbcontext.Attach(entidad);
                entry = _dbcontext.Entry(entidad);
            }

            entry.Property(property).IsModified = true;

            // 🔐 Asegurarse que EF no intente guardar navegación
            entry.Reference("IdPersonalNavigation").CurrentValue = null;
            await _dbcontext.SaveChangesAsync();
            return true;
        }


    }
}
