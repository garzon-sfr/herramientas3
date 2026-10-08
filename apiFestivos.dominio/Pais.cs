using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace apiFestivos.dominio
{
    [Table("Pais")]
    public class Pais
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Nombre")]
        public required string? Nombre { get; set; }

        [JsonIgnore]
        public ICollection<Festivo>? Festivos { get; set; }
    }
}
