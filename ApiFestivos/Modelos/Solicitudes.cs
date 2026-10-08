using System.ComponentModel.DataAnnotations;

namespace ApiFestivos.Modelos;

public class PaisSolicitud
{
    [Required, StringLength(100)]
    public required string Nombre { get; set; }
}

public class TipoFestivoSolicitud
{
    [Required, StringLength(100)]
    public required string Tipo { get; set; }
}

public class FestivoSolicitud
{
    [Range(1, int.MaxValue)]
    public int IdPais { get; set; }
    [Required, StringLength(100)]
    public required string Nombre { get; set; }
    public int Dia { get; set; }
    public int Mes { get; set; }
    [Range(-366, 366)]
    public int DiasPascua { get; set; }
    [Range(1, int.MaxValue)]
    public int IdTipo { get; set; }
}
