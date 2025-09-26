using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions; 

namespace SistemaVenta.DAL.Interfaces
{
    public interface IRepositorioGenerico<TEntity> where TEntity : class // este TEntity va a ser una clase
    {
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro); // se obtiene una expresion que sera una funcion que recibe un TEntity y devuelve un booleano y esa expresion se va a llamar filtro
        Task<TEntity>Crear(TEntity entidad); // se crea una entidad de tipo TEntity
        Task<bool>Editar(TEntity entidad); // se edita una entidad de tipo TEntity
        Task<bool>Eliminar(TEntity entidad); // se elimina una entidad de tipo TEntity
        Task<IQueryable<TEntity>>Consultar (Expression<Func<TEntity, bool>> filtro =null); // se consulta una entidad de tipo TEntity, si no se pasa un filtro se devuelve todo 
    }
}
