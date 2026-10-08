using apiFestivos.core.repositorios;
using apiFestivos.core.servicios;
using apiFestivos.dominio;

namespace apiFestivos.aplicacion;

public class TipoFestivoServicio(ITipoFestivoRepositorio repositorio) : ITipoFestivoServicio
{
    public Task<IEnumerable<TipoFestivo>> ObtenerTodos() => repositorio.ObtenerTodos();
    public Task<TipoFestivo?> Obtener(int Id) => repositorio.Obtener(Id);
    public Task<IEnumerable<TipoFestivo>> Buscar(int IndiceDato, string Texto) =>
        repositorio.Buscar(IndiceDato, Texto);
    public Task<bool> Eliminar(int Id) => repositorio.Eliminar(Id);

    public Task<TipoFestivo> Agregar(TipoFestivo entidad)
    {
        Validar(entidad);
        if (entidad.Id != 0)
            throw new ArgumentException("El Id se genera al crear el registro.");
        return repositorio.Agregar(entidad);
    }

    public Task<TipoFestivo?> Modificar(TipoFestivo entidad)
    {
        Validar(entidad);
        return repositorio.Modificar(entidad);
    }

    private static void Validar(TipoFestivo entidad)
    {
        if (string.IsNullOrWhiteSpace(entidad.Tipo) || entidad.Tipo.Trim().Length > 100)
            throw new ArgumentException("Tipo es obligatorio y admite hasta 100 caracteres.");
        entidad.Tipo = entidad.Tipo.Trim();
    }
}
