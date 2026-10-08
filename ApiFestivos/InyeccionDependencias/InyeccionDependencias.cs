using apiFestivos.aplicacion;
using apiFestivos.core.repositorios;
using apiFestivos.core.servicios;
using apiFestivos.infraestructura.Persistencia;
using apiFestivos.infraestructura.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace ApiFestivos.InyeccionDependencias;

public static class InyeccionDependencias
{
    public static IServiceCollection AgregarDependencias(this IServiceCollection servicios, IConfiguration configuracion)
    {
        var conexion = configuracion.GetConnectionString("Festivos")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:Festivos.");
        servicios.AddDbContext<FestivosContext>(opciones => opciones.UseSqlServer(conexion));
        servicios.AddScoped<IPaisRepositorio, PaisRepositorio>();
        servicios.AddScoped<ITipoFestivoRepositorio, TipoFestivoRepositorio>();
        servicios.AddScoped<IFestivoRepositorio, FestivoRepositorio>();
        servicios.AddScoped<IPaisServicio, PaisServicio>();
        servicios.AddScoped<ITipoFestivoServicio, TipoFestivoServicio>();
        servicios.AddScoped<IFestivoServicio, FestivoServicio>();
        return servicios;
    }
}
