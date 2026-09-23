
using Microsoft.EntityFrameworkCore;
using apiFestivos.core.repositorios;
using apiFestivos.dominio;
using apiFestivos.infraestructura.Persistencia;

namespace apiFestivos.infraestructura.Repositorios
{
    public class TipoFestivoRepositorio : ITipoFestivoRepositorio
    {
        private readonly FestivosContext contexto;

        public TipoFestivoRepositorio(FestivosContext contexto)
        {
            this.contexto = contexto;
        }

        public async Task<IEnumerable<TipoFestivo>> ObtenerTodos()
        {
            return await contexto.TiposFestivo.ToListAsync();
        }

        public async Task<TipoFestivo?> Obtener(int Id)
        {
            return await contexto.TiposFestivo.FindAsync(Id);
        }

        public async Task<IEnumerable<TipoFestivo>> Buscar(
            int IndiceDato, string Texto)
        {
            var consulta = contexto.TiposFestivo.AsQueryable();

            switch (IndiceDato)
            {
                case 1:
                    consulta = consulta.Where(
                        e => e.Tipo.Contains(Texto));
                    break;

                default:
                    return Enumerable.Empty<TipoFestivo>();
            }

            return await consulta.ToListAsync();
        }

        public async Task<TipoFestivo> Agregar(TipoFestivo TipoFestivo)
        {
            contexto.TiposFestivo.Add(TipoFestivo);
            await contexto.SaveChangesAsync();
            return TipoFestivo;
        }

        public async Task<TipoFestivo?> Modificar(TipoFestivo TipoFestivo)
        {
            var entidad = await contexto.TiposFestivo
                .FindAsync(TipoFestivo.Id);

            if (entidad == null)
                return null;

            entidad.Tipo = TipoFestivo.Tipo;

            await contexto.SaveChangesAsync();
            return entidad;
        }

        public async Task<bool> Eliminar(int Id)
        {
            var entidad = await contexto.TiposFestivo.FindAsync(Id);

            if (entidad == null)
                return false;

            contexto.TiposFestivo.Remove(entidad);
            await contexto.SaveChangesAsync();

            return true;
        }
    }
}