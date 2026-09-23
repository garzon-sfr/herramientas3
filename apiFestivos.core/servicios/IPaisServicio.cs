using apiFestivos.dominio;

namespace apiFestivos.core.servicios
{
    public interface IPaisServicio
    {
        Task<IEnumerable<Pais>> ObtenerTodos();

        Task<Pais?> Obtener(int Id);

        Task<IEnumerable<Pais>> Buscar(
            int IndiceDato, string Texto);

        Task<Pais> Agregar(Pais Pais);

        Task<Pais?> Modificar(Pais Pais);

        Task<bool> Eliminar(int Id);
    }
}