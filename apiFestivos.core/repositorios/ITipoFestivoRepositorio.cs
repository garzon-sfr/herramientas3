using apiFestivos.dominio;

namespace apiFestivos.core.repositorios
{
    public interface ITipoFestivoRepositorio
    {
        Task<IEnumerable<TipoFestivo>> ObtenerTodos();

        Task<TipoFestivo?> Obtener(int Id);

        Task<IEnumerable<TipoFestivo>> Buscar(
            int IndiceDato, string Texto);

        Task<TipoFestivo> Agregar(TipoFestivo TipoFestivo);

        Task<TipoFestivo?> Modificar(TipoFestivo TipoFestivo);

        Task<bool> Eliminar(int Id);
    }
}