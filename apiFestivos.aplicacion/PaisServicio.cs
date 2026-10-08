using apiFestivos.core.repositorios;
using apiFestivos.core.servicios;
using apiFestivos.dominio;

namespace apiFestivos.aplicacion;

public class PaisServicio(IPaisRepositorio repositorio) : IPaisServicio
{
    public Task<IEnumerable<Pais>> ObtenerTodos() => repositorio.ObtenerTodos();
    public Task<Pais?> Obtener(int Id) => repositorio.Obtener(Id);
    public Task<IEnumerable<Pais>> Buscar(int IndiceDato, string Texto) =>
        repositorio.Buscar(IndiceDato, Texto);
    public Task<bool> Eliminar(int Id) => repositorio.Eliminar(Id);

    public Task<Pais> Agregar(Pais entidad)
    {
        Validar(entidad);
        if (entidad.Id != 0)
            throw new ArgumentException("El Id se genera al crear el registro.");
        return repositorio.Agregar(entidad);
    }

    public Task<Pais?> Modificar(Pais entidad)
    {
        Validar(entidad);
        return repositorio.Modificar(entidad);
    }

    private static void Validar(Pais entidad)
    {
        if (string.IsNullOrWhiteSpace(entidad.Nombre) || entidad.Nombre.Trim().Length > 100)
            throw new ArgumentException("Nombre es obligatorio y admite hasta 100 caracteres.");
        entidad.Nombre = entidad.Nombre.Trim();
    }
}
