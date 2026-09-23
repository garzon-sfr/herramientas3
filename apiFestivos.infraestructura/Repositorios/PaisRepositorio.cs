
using Microsoft.EntityFrameworkCore;
using apiFestivos.core.repositorios;
using apiFestivos.dominio;
using apiFestivos.infraestructura.Persistencia;

namespace apiFestivos.infraestructura.Repositorios
{
    public class PaisRepositorio : IPaisRepositorio
    {
        private readonly FestivosContext contexto;

        public PaisRepositorio(FestivosContext contexto)
        {
            this.contexto = contexto;
        }

        public async Task<IEnumerable<Pais>> ObtenerTodos()
        {
            return await contexto.Paises.ToListAsync();
        }

        public async Task<Pais?> Obtener(int Id)
        {
            return await contexto.Paises.FindAsync(Id);
        }

        public async Task<IEnumerable<Pais>> Buscar(
            int IndiceDato, string Texto)
        {
            var consulta = contexto.Paises.AsQueryable();

            switch (IndiceDato)
            {
                case 1:
                    consulta = consulta.Where(
                        e => e.Nombre.Contains(Texto));
                    break;

                default:
                    return Enumerable.Empty<Pais>();
            }

            return await consulta.ToListAsync();
        }

        public async Task<Pais> Agregar(Pais Pais)
        {
            contexto.Paises.Add(Pais);
            await contexto.SaveChangesAsync();
            return Pais;
        }

        public async Task<Pais?> Modificar(Pais Pais)
        {
            var entidad = await contexto.Paises.FindAsync(Pais.Id);

            if (entidad == null)
                return null;

            entidad.Nombre = Pais.Nombre;

            await contexto.SaveChangesAsync();
            return entidad;
        }

        public async Task<bool> Eliminar(int Id)
        {
            var entidad = await contexto.Paises.FindAsync(Id);

            if (entidad == null)
                return false;

            contexto.Paises.Remove(entidad);
            await contexto.SaveChangesAsync();

            return true;
        }
    }
}