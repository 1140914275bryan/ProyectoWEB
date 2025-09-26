using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class CategoriaServicio : ICategoriaServicio
    {
        private readonly IRepositorioGenerico<Categoria> _repositorio;
        public CategoriaServicio(IRepositorioGenerico<Categoria> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _repositorio.Consultar();
            return query.ToList();
        }
        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoria_creada = await _repositorio.Crear(entidad);
                if (categoria_creada.IdCategoria == 0)
                
                    throw new TaskCanceledException("No se pudo crear la categoria");
                    return categoria_creada;               
            }
            catch { throw; }
        }

        public async Task<Categoria> Editar(Categoria entidad)
        {
            try { 
            Categoria categoria_encontrada = await _repositorio.Obtener(c=> c.IdCategoria == entidad.IdCategoria);
                categoria_encontrada.Descripcion = entidad.Descripcion;
                categoria_encontrada.EsActivo = entidad.EsActivo;

                bool respuesta = await _repositorio.Editar(categoria_encontrada);

                if (!respuesta)     
                    throw new TaskCanceledException("No se pudo modificar la categoria");
                    return categoria_encontrada;
                
            }
            catch { throw; }
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
            Categoria categoria_eliminada = await _repositorio.Obtener(c => c.IdCategoria == idCategoria);
                if (categoria_eliminada == null)
                {
                    throw new TaskCanceledException("No se encontro la categoria a eliminar");
                }
                bool respuesta = await _repositorio.Eliminar(categoria_eliminada);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo eliminar la categoria");
                }
                return respuesta;
            }
            catch { throw; }
        }
    }
}
