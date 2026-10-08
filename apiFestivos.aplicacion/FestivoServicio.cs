using apiFestivos.core.Excepciones;
using apiFestivos.core.repositorios;
using apiFestivos.core.servicios;
using apiFestivos.dominio;
using apiFestivos.dominio.Dtos;

namespace apiFestivos.aplicacion;

public class FestivoServicio(
    IFestivoRepositorio repositorio,
    IPaisRepositorio paises,
    ITipoFestivoRepositorio tipos) : IFestivoServicio
{
    public Task<IEnumerable<Festivo>> ObtenerTodos() => repositorio.ObtenerTodos();
    public Task<Festivo?> Obtener(int Id) => repositorio.Obtener(Id);
    public Task<IEnumerable<Festivo>> Buscar(int IndiceDato, string Texto) =>
        repositorio.Buscar(IndiceDato, Texto);
    public Task<bool> Eliminar(int Id) => repositorio.Eliminar(Id);

    public async Task<IEnumerable<Festivo>> ObtenerPais(int IdPais)
    {
        await ValidarPais(IdPais);
        return await repositorio.ObtenerPais(IdPais);
    }

    public async Task<Festivo> Agregar(Festivo entidad)
    {
        if (entidad.Id != 0)
            throw new ArgumentException("El Id se genera al crear el registro.");
        await Validar(entidad);
        return await repositorio.Agregar(entidad);
    }

    public async Task<Festivo?> Modificar(Festivo entidad)
    {
        if (await repositorio.Obtener(entidad.Id) is null)
            return null;
        await Validar(entidad);
        return await repositorio.Modificar(entidad);
    }

    public async Task<IEnumerable<FestivoFechaDto>> ObtenerFestivos(int IdPais, int Año)
    {
        ServicioFechas.ValidarAño(Año);
        var definiciones = await ObtenerPais(IdPais);
        var fechas = new List<FestivoFechaDto>();
        foreach (var festivo in definiciones)
        {
            if (festivo.IdTipo is > 4)
                throw new ConflictoException($"Regla de cálculo pendiente para el tipo {festivo.IdTipo} usado por '{festivo.Nombre}'. El registro se conserva y puede gestionarse por CRUD.");
            ValidarDefinicion(festivo);
            // También incluye traslados desde diciembre y desplazamientos de Pascua
            // que cruzan el límite del año. No elimina festivos que coinciden en fecha.
            for (int añoBase = Math.Max(1, Año - 1); añoBase <= Math.Min(9999, Año + 1); añoBase++)
            {
                var fecha = CalcularFecha(festivo, añoBase);
                if (fecha?.Year == Año)
                    fechas.Add(new FestivoFechaDto(festivo.Nombre!, DateOnly.FromDateTime(fecha.Value)));
            }
        }
        return fechas.OrderBy(f => f.Fecha).ThenBy(f => f.Festivo).ToArray();
    }

    public async Task<bool> EsFestivo(int IdPais, int Año, int Mes, int Dia)
    {
        if (!ServicioFechas.EsFechaValida(Año, Mes, Dia))
            throw new ArgumentException("Fecha No valida");
        var fecha = new DateOnly(Año, Mes, Dia);
        return (await ObtenerFestivos(IdPais, Año)).Any(f => f.Fecha == fecha);
    }

    private async Task ValidarPais(int id)
    {
        if (await paises.Obtener(id) is null)
            throw new KeyNotFoundException($"No se encontró el país con Id={id}.");
    }

    private async Task Validar(Festivo entidad)
    {
        ValidarDefinicion(entidad);
        if (await paises.Obtener(entidad.IdPais.GetValueOrDefault()) is null)
            throw new ArgumentException("El país indicado no existe.");
        if (await tipos.Obtener(entidad.IdTipo.GetValueOrDefault()) is null)
            throw new ArgumentException("El tipo de festivo indicado no existe.");
        entidad.Nombre = entidad.Nombre!.Trim();
    }

    private static void ValidarDefinicion(Festivo entidad)
    {
        if (string.IsNullOrWhiteSpace(entidad.Nombre) || entidad.Nombre.Trim().Length > 100)
            throw new ArgumentException("Nombre es obligatorio y admite hasta 100 caracteres.");
        if (entidad.IdTipo is null or < 1)
            throw new ArgumentException("Debe indicar un tipo de festivo existente.");
        if (entidad.IdTipo is 1 or 2)
        {
            // 2000 permite definir un festivo para el 29 de febrero.
            if (!ServicioFechas.EsFechaValida(2000, entidad.Mes.GetValueOrDefault(), entidad.Dia.GetValueOrDefault()))
                throw new ArgumentException("El día y mes del festivo no forman una fecha válida.");
            if (entidad.DiasPascua.GetValueOrDefault() != 0)
                throw new ArgumentException("DiasPascua debe ser 0 para los tipos 1 y 2.");
        }
        else if (entidad.IdTipo is 3 or 4)
        {
            if (entidad.Mes.GetValueOrDefault() != 0 || entidad.Dia.GetValueOrDefault() != 0)
                throw new ArgumentException("Dia y Mes deben ser 0 para los tipos 3 y 4.");
            if (entidad.DiasPascua is < -366 or > 366)
                throw new ArgumentException("DiasPascua debe estar entre -366 y 366.");
        }
    }

    private static DateTime? CalcularFecha(Festivo festivo, int año)
    {
        if (festivo.IdTipo is 1 or 2 && !ServicioFechas.EsFechaValida(año, festivo.Mes.GetValueOrDefault(), festivo.Dia.GetValueOrDefault()))
            return null; // Un 29 de febrero no ocurre todos los años.
        try
        {
            var fecha = festivo.IdTipo is 1 or 2
                ? new DateTime(año, festivo.Mes.GetValueOrDefault(), festivo.Dia.GetValueOrDefault())
                : ServicioFechas.AgregarDias(ServicioFechas.ObtenerPascua(año), festivo.DiasPascua.GetValueOrDefault());
            return festivo.IdTipo is 2 or 4 ? ServicioFechas.SiguienteLunes(fecha) : fecha;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null; // Un traslado fuera de 1..9999 no pertenece al año consultado.
        }
    }
}
