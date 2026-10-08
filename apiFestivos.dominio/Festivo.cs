using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace apiFestivos.dominio
{
    [Table("Festivo")]
    public class Festivo
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("IdPais")]
        public int? IdPais { get; set; }

        [Column("Nombre")]
        public required string? Nombre { get; set; }

        [Column("Dia")]
        public int? Dia { get; set; }

        [Column("Mes")]
        public int? Mes { get; set; }

        [Column("DiasPascua")]
        public int? DiasPascua { get; set; }

        [Column("IdTipo")]
        public int? IdTipo { get; set; }

        [JsonIgnore]
        public Pais? Pais { get; set; }

        [JsonIgnore]
        public TipoFestivo? TipoFestivo { get; set; }
    }
}
