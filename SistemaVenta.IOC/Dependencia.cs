using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.IOC
{
    public static class Dependencia // Clase estática para configurar las dependencias de la aplicación
    {
        public static void ConfigurarDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración de la cadena de conexión a la base de datos
            services.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL")); // Obtiene la cadena de conexión desde el archivo de configuración
            });  
            
            services.AddTransient(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>)); // Inyección de dependencias para el repositorio genérico
            services.AddScoped<IVentaRepositorio, VentaRepositorio>(); // Inyección de dependencias para el repositorio de ventas

            services.AddScoped<IServicioCorreo, ServicioCorreo>(); // Inyección de dependencias para el servicio de correo electrónico
            services.AddScoped<IFireBaseService, FireBaseService>(); // Inyección de dependencias para el servicio de FireBase

            services.AddScoped<IUtilidadesService, UtilidadesService>(); // Inyección de dependencias para el servicio de utilidades
            services.AddScoped<IRolService, RolService>(); // Inyección de dependencias para el servicio de roles

            services.AddScoped<IUsuarioService, UsuarioService>(); // Inyección de dependencias para el servicio de usuarios

            services.AddScoped<IServicioNegocio, ServicioNegocio>(); // Inyección de dependencias para el servicio de negocio

            services.AddScoped<ICategoriaServicio, CategoriaServicio>(); // Inyección de dependencias para el servicio de categorías

            services.AddScoped<IProductoServicio, ProductoServicio>(); // Inyección de dependencias para el servicio de productos

            services.AddScoped<ITipoDocumentoVentaServicio, TipoDocumentoVentaServicio>(); // 
            services.AddScoped<IVentaServicio, VentaServicio>();

            services.AddScoped<IDashBoardService, DashBoardServicio>();

            services.AddScoped<IMenuServicio, MenuServicio>();
        }
    }
}
