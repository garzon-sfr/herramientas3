using apiFestivos.dominio;

namespace apiFestivos.core.repositorios
{
    public interface IPaisRepositorio
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