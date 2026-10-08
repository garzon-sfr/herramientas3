using apiFestivos.dominio;
using apiFestivos.dominio.Dtos;

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

        Task<IEnumerable<FestivoFechaDto>> ObtenerFestivos(int IdPais, int Año);

        Task<bool> EsFestivo(int IdPais, int Año, int Mes, int Dia);
    }
}