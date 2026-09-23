
using Microsoft.EntityFrameworkCore;
using apiFestivos.core.repositorios;
using apiFestivos.dominio;
using apiFestivos.infraestructura.Persistencia;

namespace apiFestivos.infraestructura.Repositorios
{
    public class FestivoRepositorio : IFestivoRepositorio
    {
        private readonly FestivosContext contexto;

        public FestivoRepositorio(FestivosContext contexto)
        {
            this.contexto = contexto;
        }

        public async Task<IEnumerable<Festivo>> ObtenerTodos()
        {
            return await contexto.Festivos.ToListAsync();
        }

        public async Task<IEnumerable<Festivo>> ObtenerPais(int IdPais)
        {
            return await contexto.Festivos
                .Where(e => e.IdPais == IdPais)
                .ToListAsync();
        }

        public async Task<Festivo?> Obtener(int Id)
        {
            return await contexto.Festivos.FindAsync(Id);
        }

        public async Task<IEnumerable<Festivo>> Buscar(
            int IndiceDato, string Texto)
        {
            var consulta = contexto.Festivos.AsQueryable();

            switch (IndiceDato)
            {
                case 1:
                    consulta = consulta.Where(
                        e => e.Nombre.Contains(Texto));
                    break;

                default:
                    return Enumerable.Empty<Festivo>();
            }

            return await consulta.ToListAsync();
        }

        public async Task<Festivo> Agregar(Festivo Festivo)
        {
            contexto.Festivos.Add(Festivo);
            await contexto.SaveChangesAsync();
            return Festivo;
        }

        public async Task<Festivo?> Modificar(Festivo Festivo)
        {
            var entidad = await contexto.Festivos
                .FindAsync(Festivo.Id);

            if (entidad == null)
                return null;

            entidad.IdPais = Festivo.IdPais;
            entidad.Nombre = Festivo.Nombre;
            entidad.Dia = Festivo.Dia;
            entidad.Mes = Festivo.Mes;
            entidad.DiasPascua = Festivo.DiasPascua;
            entidad.IdTipo = Festivo.IdTipo;

            await contexto.SaveChangesAsync();
            return entidad;
        }

        public async Task<bool> Eliminar(int Id)
        {
            var entidad = await contexto.Festivos.FindAsync(Id);

            if (entidad == null)
                return false;

            contexto.Festivos.Remove(entidad);
            await contexto.SaveChangesAsync();

            return true;
        }
    }
}