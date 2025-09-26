using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions; //para poder usar expresiones lambda

namespace SistemaVenta.DAL.Implementacion
{
    public class RepositorioGenerico<TEntity> : IRepositorioGenerico<TEntity> where TEntity : class // este TEntity va a ser una clase
    {
        private readonly DbventaContext _dbContext;

        public RepositorioGenerico(DbventaContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                TEntity entidad = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filtro); // se obtiene la primera entidad que cumpla con el filtro, si no hay ninguna devuelve null
                return entidad; // devuelve la entidad encontrada
            }
            catch{
                throw; // para manejar excepciones
            }
        }

        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                _dbContext.Set<TEntity>().Add(entidad); // se agrega la entidad al contexto
                await _dbContext.SaveChangesAsync(); // se guardan los cambios de forma asincrona
                return entidad; // devuelve la entidad creada
            }
            catch
            {
                throw; // para manejar excepciones
            }
        }

        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _dbContext.Update(entidad); // se actualiza la entidad en el contexto
                await _dbContext.SaveChangesAsync(); // se guardan los cambios de forma asincrona
                return true; // devuelve true si se edito correctamente
            }
            catch
            {
                throw; // para manejar excepciones
            }
        }

        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _dbContext.Remove(entidad); // se elimina la entidad del contexto
                await _dbContext.SaveChangesAsync(); // se guardan los cambios de forma asincrona
                return true; // devuelve true si se elimino correctamente
            }
            catch
            {
                throw; // para manejar excepciones
            }
        }
        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            IQueryable<TEntity> queryEntidad = filtro == null? _dbContext.Set<TEntity>(): _dbContext.Set<TEntity>().Where(filtro); 
            // si no se pasa un filtro, devuelve todo, si se pasa un filtro, devuelve solo las entidades que cumplan con el filtro
            return queryEntidad;
        }

    }
}
