using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.VisualBasic;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class TipoDocumentoVentaServicio : ITipoDocumentoVentaServicio
    {
        public readonly IRepositorioGenerico<TipoDocumentoVenta> _repositorio;

        public TipoDocumentoVentaServicio(IRepositorioGenerico<TipoDocumentoVenta> repositorio)
        {
            _repositorio = repositorio; 
        }

        public async Task<List<TipoDocumentoVenta>> Lista()
        {

            IQueryable<TipoDocumentoVenta> query = await _repositorio.Consultar();
            return query.ToList();

        }
    }
}
