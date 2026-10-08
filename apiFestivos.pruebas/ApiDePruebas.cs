using apiFestivos.dominio;
using apiFestivos.infraestructura.Persistencia;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace apiFestivos.pruebas;

// SQLite relacional, exclusiva de cada prueba. Nunca conecta a GARZON/Festivos.
public sealed class ApiDePruebas : WebApplicationFactory<Program>
{
    private readonly SqliteConnection conexion = new("Data Source=:memory:");

    public ApiDePruebas() => conexion.Open();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(servicios =>
        {
            servicios.RemoveAll<FestivosContext>();
            servicios.RemoveAll<DbContextOptions<FestivosContext>>();
            servicios.RemoveAll<IDbContextOptionsConfiguration<FestivosContext>>();
            servicios.AddDbContext<FestivosContext>(opciones => opciones.UseSqlite(conexion));
        });
    }

    public HttpClient CrearCliente()
    {
        var cliente = CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        using var scope = Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<FestivosContext>();
        contexto.Database.EnsureCreated();
        contexto.Paises.Add(new Pais { Id = 1, Nombre = "Colombia" });
        contexto.TiposFestivo.AddRange(
            new TipoFestivo { Id = 1, Tipo = "Fijo" },
            new TipoFestivo { Id = 2, Tipo = "Ley de Puente festivo" },
            new TipoFestivo { Id = 3, Tipo = "Basado en el domingo de pascua" },
            new TipoFestivo { Id = 4, Tipo = "Basado en el domingo de pascua y Ley de Puente festivo" });
        contexto.Festivos.AddRange(DatosColombia());
        contexto.SaveChanges();
        return cliente;
    }

    public static Festivo[] DatosColombia() =>
    [
        Nuevo("Año nuevo", 1, 1, 1), Nuevo("Santos Reyes", 6, 1, 2),
        Nuevo("San José", 19, 3, 2), Nuevo("Jueves Santo", 0, 0, 3, -3),
        Nuevo("Viernes Santo", 0, 0, 3, -2), Nuevo("Domingo de Pascua", 0, 0, 3),
        Nuevo("Día del Trabajo", 1, 5, 1), Nuevo("Ascensión del Señor", 0, 0, 4, 40),
        Nuevo("Corpus Christi", 0, 0, 4, 61), Nuevo("Sagrado Corazón de Jesús", 0, 0, 4, 68),
        Nuevo("San Pedro y San Pablo", 29, 6, 2), Nuevo("Independencia Colombia", 20, 7, 1),
        Nuevo("Batalla de Boyacá", 7, 8, 1), Nuevo("Asunción de la Virgen", 15, 8, 2),
        Nuevo("Día de la Raza", 12, 10, 2), Nuevo("Todos los santos", 1, 11, 2),
        Nuevo("Independencia de Cartagena", 11, 11, 2), Nuevo("Inmaculada Concepción", 8, 12, 1),
        Nuevo("Navidad", 25, 12, 1)
    ];

    private static Festivo Nuevo(string nombre, int dia, int mes, int tipo, int diasPascua = 0) =>
        new() { IdPais = 1, Nombre = nombre, Dia = dia, Mes = mes, IdTipo = tipo, DiasPascua = diasPascua };

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) conexion.Dispose();
    }
}
