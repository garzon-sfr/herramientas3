using apiFestivos.dominio;

namespace apiFestivos.core.servicios
{
    public interface IFestivoServicio
    {
        Task<IEnumerable<Festivo>> ObtenerTodos();

        Task<IEnumerable<Festivo>> ObtenerPais(int IdPais);

        Task<Festivo?> Obtener(int Id);

        Task<IEnumerable<Festivo>> Buscar(
            int IndiceDato, string Texto);

        Task<Festivo> Agregar(Festivo Festivo);

        Task<Festivo?> Modificar(Festivo Festivo);

        Task<bool> Eliminar(int Id);
    }
}