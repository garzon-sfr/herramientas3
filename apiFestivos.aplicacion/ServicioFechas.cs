namespace apiFestivos.aplicacion;

/// <summary>Fórmula suministrada por el docente para esta evaluación.</summary>
public static class ServicioFechas
{
    public static DateTime ObtenerInicioSemanaSanta(int año)
    {
        ValidarAño(año);
        int a = año % 19;
        int b = año % 4;
        int c = año % 7;
        int d = (19 * a + 24) % 30;
        int dias = d + (2 * b + 4 * c + 6 * d + 5) % 7;
        int dia = 15 + dias;
        int mes = 3;
        if (dia > 31)
        {
            dia -= 31;
            mes = 4;
        }
        return new DateTime(año, mes, dia);
    }

    public static DateTime AgregarDias(DateTime fecha, int dias) => fecha.AddDays(dias);
    public static DateTime ObtenerPascua(int año) => AgregarDias(ObtenerInicioSemanaSanta(año), 7);

    public static DateTime SiguienteLunes(DateTime fecha)
    {
        int diasLunes = ((int)DayOfWeek.Monday - (int)fecha.DayOfWeek + 7) % 7;
        return AgregarDias(fecha, diasLunes);
    }

    public static void ValidarAño(int año)
    {
        if (año is < 1 or > 9999)
            throw new ArgumentException("El año debe estar entre 1 y 9999.");
    }

    public static bool EsFechaValida(int año, int mes, int dia) =>
        año is >= 1 and <= 9999 && mes is >= 1 and <= 12 &&
        dia >= 1 && dia <= DateTime.DaysInMonth(año, mes);
}
