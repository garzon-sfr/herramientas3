using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;

namespace apiFestivos.dominio
{
    [Table("TipoFestivo")]
    public class TipoFestivo
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Tipo")]
        public required string Tipo { get; set; }

        public ICollection<Festivo>? Festivos { get; set; }
    }
}
