using apiFestivos.infraestructura.Persistencia;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using apiFestivos.dominio;
using apiFestivos.dominio.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace apiFestivos.pruebas;

[TestClass]
public class ApiPruebas
{
    private ApiDePruebas api = null!;
    private HttpClient cliente = null!;

    [TestInitialize]
    public void Iniciar() { api = new ApiDePruebas(); cliente = api.CrearCliente(); }
    [TestCleanup]
    public void Terminar() { cliente.Dispose(); api.Dispose(); }

    [DataTestMethod]
    [DataRow("2023/6/12", "Es Festivo")]
    [DataRow("2023/2/28", "No es festivo")]
    [DataRow("2023/4/6", "Es Festivo")]
    [DataRow("2023/4/7", "Es Festivo")]
    [DataRow("2023/4/9", "Es Festivo")]
    [DataRow("2023/1/6", "No es festivo")]
    [DataRow("2023/1/9", "Es Festivo")]
    public async Task VerificarEjemplosYCuatroModos(string fecha, string esperado)
    {
        var respuesta = await cliente.GetAsync($"/api/calendario/verificar/1/{fecha}");
        Assert.AreEqual(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.AreEqual(esperado, await respuesta.Content.ReadAsStringAsync());
    }

    [DataTestMethod]
    [DataRow("2023/2/35")]
    [DataRow("2023/2/29")]
    [DataRow("2023/0/1")]
    [DataRow("0/1/1")]
    [DataRow("10000/1/1")]
    public async Task FechaInvalidaDevuelve400(string fecha) =>
        Assert.AreEqual(HttpStatusCode.BadRequest, (await cliente.GetAsync($"/api/calendario/verificar/1/{fecha}")).StatusCode);

    [TestMethod]
    public async Task Listado2023CoincideConLos19RegistrosDelEnunciado()
    {
        var lista = (await cliente.GetFromJsonAsync<FestivoFechaDto[]>("/api/calendario/festivos/1/2023"))!;
        string[] fechas = ["2023-01-01", "2023-01-09", "2023-03-20", "2023-04-06", "2023-04-07", "2023-04-09",
            "2023-05-01", "2023-05-22", "2023-06-12", "2023-06-19", "2023-07-03", "2023-07-20", "2023-08-07",
            "2023-08-21", "2023-10-16", "2023-11-06", "2023-11-13", "2023-12-08", "2023-12-25"];
        CollectionAssert.AreEqual(fechas, lista.Select(f => f.Fecha.ToString("yyyy-MM-dd")).ToArray());
        Assert.AreEqual("Corpus Christi", lista.Single(f => f.Fecha == new DateOnly(2023, 6, 12)).Festivo);
    }

    [TestMethod]
    public async Task PaisInexistenteDevuelve404() =>
        Assert.AreEqual(HttpStatusCode.NotFound, (await cliente.GetAsync("/api/calendario/festivos/999/2023")).StatusCode);

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(10000)]
    public async Task AñoInvalidoDevuelve400(int año) =>
        Assert.AreEqual(HttpStatusCode.BadRequest, (await cliente.GetAsync($"/api/calendario/festivos/1/{año}")).StatusCode);

    [TestMethod]
    public async Task CrudPaisTipoYFestivoConPersistenciaYRelaciones()
    {
        var paisPost = await cliente.PostAsJsonAsync("/api/paises", new { nombre = "Pruebas" });
        Assert.AreEqual(HttpStatusCode.Created, paisPost.StatusCode);
        var pais = (await paisPost.Content.ReadFromJsonAsync<Pais>())!;
        Assert.IsNotNull(paisPost.Headers.Location);
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync(paisPost.Headers.Location)).StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.PutAsJsonAsync($"/api/paises/{pais.Id}", new { nombre = "Actualizado" })).StatusCode);
        Assert.AreEqual("Actualizado", (await cliente.GetFromJsonAsync<Pais>($"/api/paises/{pais.Id}"))!.Nombre);
        Assert.AreEqual(1, (await cliente.GetFromJsonAsync<Pais[]>("/api/paises/buscar/1/Actualizado"))!.Length);

        var tipoPost = await cliente.PostAsJsonAsync("/api/tiposfestivo", new { tipo = "Tipo adicional" });
        Assert.AreEqual(HttpStatusCode.Created, tipoPost.StatusCode);
        var tipo = (await tipoPost.Content.ReadFromJsonAsync<TipoFestivo>())!;
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.PutAsJsonAsync($"/api/tiposfestivo/{tipo.Id}", new { tipo = "Etiqueta" })).StatusCode);
        Assert.AreEqual("Etiqueta", (await cliente.GetFromJsonAsync<TipoFestivo>($"/api/tiposfestivo/{tipo.Id}"))!.Tipo);
        Assert.AreEqual(1, (await cliente.GetFromJsonAsync<TipoFestivo[]>("/api/tiposfestivo/buscar/1/Etiqueta"))!.Length);

        var festivoPost = await cliente.PostAsJsonAsync("/api/festivos", new { idPais = pais.Id, nombre = "Prueba", dia = 2, mes = 1, diasPascua = 0, idTipo = 1 });
        Assert.AreEqual(HttpStatusCode.Created, festivoPost.StatusCode);
        var festivo = (await festivoPost.Content.ReadFromJsonAsync<Festivo>())!;
        Assert.AreEqual(HttpStatusCode.Conflict, (await cliente.DeleteAsync($"/api/paises/{pais.Id}")).StatusCode);
        Assert.AreEqual(HttpStatusCode.Conflict, (await cliente.DeleteAsync("/api/tiposfestivo/1")).StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.PutAsJsonAsync($"/api/festivos/{festivo.Id}", new { idPais = pais.Id, nombre = "Prueba modificada", dia = 3, mes = 1, diasPascua = 0, idTipo = 2 })).StatusCode);
        var modificado = (await cliente.GetFromJsonAsync<Festivo>($"/api/festivos/{festivo.Id}"))!;
        Assert.AreEqual(3, modificado.Dia); Assert.AreEqual(2, modificado.IdTipo);
        Assert.AreEqual(1, (await cliente.GetFromJsonAsync<Festivo[]>($"/api/festivos/pais/{pais.Id}"))!.Length);
        Assert.AreEqual(1, (await cliente.GetFromJsonAsync<Festivo[]>("/api/festivos/buscar/1/modificada"))!.Length);
        foreach (var ruta in new[] { $"festivos/{festivo.Id}", $"paises/{pais.Id}", $"tiposfestivo/{tipo.Id}" })
        {
            Assert.AreEqual(HttpStatusCode.NoContent, (await cliente.DeleteAsync($"/api/{ruta}")).StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, (await cliente.GetAsync($"/api/{ruta}")).StatusCode);
        }
    }

    [DataTestMethod]
    [DataRow(31, 2, 1, 0, 1)]
    [DataRow(1, 1, 5, 0, 1)]
    [DataRow(0, 0, 3, 367, 1)]
    [DataRow(1, 1, 1, 0, 999)]
    [DataRow(1, 1, 1, 3, 1)]
    [DataRow(1, 1, 3, 0, 1)]
    public async Task RechazaFestivosInvalidos(int dia, int mes, int tipo, int dias, int pais)
    {
        var respuesta = await cliente.PostAsJsonAsync("/api/festivos", new { idPais = pais, nombre = "Inválido", dia, mes, idTipo = tipo, diasPascua = dias });
        Assert.AreEqual(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.AreEqual(19, (await cliente.GetFromJsonAsync<Festivo[]>("/api/festivos"))!.Length);
    }

    [TestMethod]
    public async Task RechazaNombreEnBlancoYRegistroInexistente()
    {
        Assert.AreEqual(HttpStatusCode.BadRequest, (await cliente.PostAsJsonAsync("/api/paises", new { nombre = " " })).StatusCode);
        Assert.AreEqual(HttpStatusCode.NotFound, (await cliente.PutAsJsonAsync("/api/paises/999", new { nombre = "No existe" })).StatusCode);
        Assert.AreEqual(HttpStatusCode.NotFound, (await cliente.DeleteAsync("/api/festivos/999")).StatusCode);
    }

    [TestMethod]
    public async Task TipoCincoSeConservaConCrudYExplicaElCalculoPendiente()
    {
        using (var scope = api.Services.CreateScope())
        {
            var contexto = scope.ServiceProvider.GetRequiredService<FestivosContext>();
            contexto.TiposFestivo.Add(new TipoFestivo { Id = 5, Tipo = "Ley Puente Festivo Viernes" });
            contexto.Paises.Add(new Pais { Id = 10, Nombre = "ECUADOR" });
            await contexto.SaveChangesAsync();
        }
        var creado = await cliente.PostAsJsonAsync("/api/festivos", new { idPais = 10, nombre = "Día del Trabajo", dia = 1, mes = 5, diasPascua = 0, idTipo = 5 });
        Assert.AreEqual(HttpStatusCode.Created, creado.StatusCode);
        var festivo = (await creado.Content.ReadFromJsonAsync<Festivo>())!;
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.PutAsJsonAsync($"/api/festivos/{festivo.Id}", new { idPais = 10, nombre = "Día del Trabajo", dia = 1, mes = 5, diasPascua = 0, idTipo = 5 })).StatusCode);
        Assert.AreEqual(1, (await cliente.GetFromJsonAsync<Festivo[]>("/api/festivos/pais/10"))!.Length);
        var calendario = await cliente.GetAsync("/api/calendario/festivos/10/2023");
        Assert.AreEqual(HttpStatusCode.Conflict, calendario.StatusCode);
        var problema = await calendario.Content.ReadFromJsonAsync<JsonElement>();
        StringAssert.Contains(problema.GetProperty("title").GetString(), "Regla de cálculo pendiente para el tipo 5");
        Assert.AreEqual(19, (await cliente.GetFromJsonAsync<FestivoFechaDto[]>("/api/calendario/festivos/1/2023"))!.Length);
        Assert.AreEqual(HttpStatusCode.NoContent, (await cliente.DeleteAsync($"/api/festivos/{festivo.Id}")).StatusCode);
    }

    [TestMethod]
    public async Task ColumnasNulasExistentesSeLeenSinModificarLaBase()
    {
        using (var scope = api.Services.CreateScope())
        {
            var contexto = scope.ServiceProvider.GetRequiredService<FestivosContext>();
            contexto.Paises.Add(new Pais { Nombre = null });
            contexto.TiposFestivo.Add(new TipoFestivo { Tipo = null });
            contexto.Festivos.Add(new Festivo { Nombre = null, IdPais = null, IdTipo = null });
            contexto.Festivos.Add(new Festivo { Nombre = "Pascua con columnas nulas", IdPais = 1, IdTipo = 3 });
            await contexto.SaveChangesAsync();
        }
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync("/api/paises")).StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync("/api/tiposfestivo")).StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync("/api/festivos")).StatusCode);
        var calendario = (await cliente.GetFromJsonAsync<FestivoFechaDto[]>("/api/calendario/festivos/1/2023"))!;
        Assert.IsTrue(calendario.Any(f => f.Festivo == "Pascua con columnas nulas" && f.Fecha == new DateOnly(2023, 4, 9)));
    }

    [TestMethod]
    public async Task DuplicadosNoModificanElCatalogo()
    {
        Assert.AreEqual(HttpStatusCode.Conflict, (await cliente.PostAsJsonAsync("/api/paises", new { nombre = "Colombia" })).StatusCode);
        Assert.AreEqual(HttpStatusCode.Conflict, (await cliente.PostAsJsonAsync("/api/tiposfestivo", new { tipo = "Fijo" })).StatusCode);
        Assert.AreEqual(HttpStatusCode.Conflict, (await cliente.PutAsJsonAsync("/api/tiposfestivo/2", new { tipo = "Fijo" })).StatusCode);
        Assert.AreEqual("Ley de Puente festivo", (await cliente.GetFromJsonAsync<TipoFestivo>("/api/tiposfestivo/2"))!.Tipo);
    }

    [TestMethod]
    public async Task PaisSinFestivosTieneCalendarioVacio()
    {
        var respuesta = await cliente.PostAsJsonAsync("/api/paises", new { nombre = "Sin calendario" });
        var pais = (await respuesta.Content.ReadFromJsonAsync<Pais>())!;
        Assert.AreEqual(0, (await cliente.GetFromJsonAsync<FestivoFechaDto[]>($"/api/calendario/festivos/{pais.Id}/2023"))!.Length);
        Assert.AreEqual("No es festivo", await cliente.GetStringAsync($"/api/calendario/verificar/{pais.Id}/2023/1/1"));
    }

    [TestMethod]
    public async Task BisiestosCoincidenciasYTrasladosEntreAños()
    {
        foreach (var def in new[] { new { nombre = "Bisiesto", dia = 29, mes = 2, tipo = 1 }, new { nombre = "Cierre trasladable", dia = 31, mes = 12, tipo = 2 } })
            Assert.AreEqual(HttpStatusCode.Created, (await cliente.PostAsJsonAsync("/api/festivos", new { idPais = 1, def.nombre, def.dia, def.mes, idTipo = def.tipo })).StatusCode);
        var a2023 = (await cliente.GetFromJsonAsync<FestivoFechaDto[]>("/api/calendario/festivos/1/2023"))!;
        Assert.IsFalse(a2023.Any(f => f.Festivo == "Bisiesto"));
        var a2024 = (await cliente.GetFromJsonAsync<FestivoFechaDto[]>("/api/calendario/festivos/1/2024"))!;
        Assert.IsTrue(a2024.Any(f => f.Festivo == "Bisiesto" && f.Fecha == new DateOnly(2024, 2, 29)));
        Assert.AreEqual(2, a2024.Count(f => f.Fecha == new DateOnly(2024, 1, 1)));
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync("/api/calendario/festivos/1/9999")).StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync("/api/calendario/festivos/1/1")).StatusCode);
    }

    [TestMethod]
    public async Task SwaggerExponeCrudYCalendarioYConservaWeatherForecast()
    {
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync("/swagger/index.html")).StatusCode);
        var documento = await cliente.GetFromJsonAsync<JsonElement>("/swagger/v1/swagger.json");
        var paths = documento.GetProperty("paths");
        foreach (var recurso in new[] { "paises", "tiposfestivo", "festivos" })
        {
            Assert.IsTrue(paths.GetProperty($"/api/{recurso}").TryGetProperty("post", out _));
            Assert.IsTrue(paths.GetProperty($"/api/{recurso}/{{Id}}").TryGetProperty("put", out _));
            Assert.IsTrue(paths.GetProperty($"/api/{recurso}/{{Id}}").TryGetProperty("delete", out _));
        }
        Assert.IsTrue(paths.TryGetProperty("/api/calendario/festivos/{IdPais}/{Año}", out _));
        Assert.AreEqual(HttpStatusCode.OK, (await cliente.GetAsync("/weatherforecast")).StatusCode);
    }
}
