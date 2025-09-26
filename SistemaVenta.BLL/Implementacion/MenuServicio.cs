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
    public class MenuServicio : IMenuServicio
    {
        private readonly IRepositorioGenerico<Menu> _repositorioMenu;
        private readonly IRepositorioGenerico<RolMenu> _repositorioRolMenu;
        private readonly IRepositorioGenerico<Usuario> _repositorioUsuario;

        public MenuServicio(IRepositorioGenerico<Menu> repositorioMenu, IRepositorioGenerico<RolMenu> repositorioRolMenu,
            IRepositorioGenerico<Usuario> repositorioUsuario)
        {
            _repositorioMenu = repositorioMenu; 
            _repositorioRolMenu = repositorioRolMenu;
            _repositorioUsuario = repositorioUsuario;
        }

        public async Task<List<Menu>> ObtenerMenus(int idUsuario)
        {
            IQueryable<Usuario> tbUsuario = await _repositorioUsuario.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<RolMenu> tbRolMenu = await _repositorioRolMenu.Consultar();
            IQueryable<Menu> tbMenu = await _repositorioMenu.Consultar();

            IQueryable<Menu> MenuPadre = (from u in tbUsuario
                                          join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          join mpadre in tbMenu on m.IdMenuPadre equals mpadre.IdMenu
                                          select mpadre).Distinct().AsQueryable();// obtener todos los menu padres que le pertenecer al usuario segun el rol

            IQueryable<Menu> MenuHijos = (from u in tbUsuario
                                          join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          where m.IdMenu != m.IdMenuPadre
                                          select m).Distinct().AsQueryable(); // obtener todos los menu hijos que le pertenecer al usuario segun el rol

            List<Menu> listaMenu = (from mpadre in MenuPadre
                                    select new Menu()
                                    {
                                        Descripcion = mpadre.Descripcion,
                                        Icono = mpadre.Icono,
                                        Controlador = mpadre.Controlador,
                                        PaginaAccion = mpadre.PaginaAccion,
                                        InverseIdMenuPadreNavigation = (from mhijo in MenuHijos
                                                                        where mhijo.IdMenuPadre == mpadre.IdMenu
                                                                        select mhijo).ToList()
                                    }).ToList();
            return listaMenu;
        }
    }
}
