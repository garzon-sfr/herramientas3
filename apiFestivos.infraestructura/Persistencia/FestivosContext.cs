using apiFestivos.dominio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace apiFestivos.infraestructura.Persistencia
{
    public class FestivosContext : DbContext
    {
        public DbSet<Pais> Paises { get; set; }
        public DbSet<TipoFestivo> TiposFestivo { get; set; }
        public DbSet<Festivo> Festivos { get; set; }

        protected override void OnModelCreating(ModelBuilder constructor)
        {
            // Tabla PAIS
            constructor.Entity<Pais>(entidadPais =>
            {
                entidadPais.HasKey(e => e.Id); // Clave primaria
                entidadPais.HasIndex(e => e.Nombre).IsUnique(); // Indice
            });

            // Tabla TIPO FESTIVO
            constructor.Entity<TipoFestivo>(entidadTipoFestivo =>
            {
                entidadTipoFestivo.HasKey(e => e.Id); // Clave primaria
                entidadTipoFestivo.HasIndex(e => e.Tipo).IsUnique(); // Indice
            });

            // Tabla FESTIVO
            constructor.Entity<Festivo>(entidadFestivo =>
            {
                entidadFestivo.HasKey(e => e.Id); // Clave primaria
            });

            constructor.Entity<Festivo>()
                .HasOne(e => e.Pais)
                .WithMany()
                .HasForeignKey(e => e.IdPais); // Clave foránea

            constructor.Entity<Festivo>()
                .HasOne(e => e.TipoFestivo)
                .WithMany()
                .HasForeignKey(e => e.IdTipo); // Clave foránea
        }
    }
}