using apiFestivos.aplicacion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace apiFestivos.pruebas;

[TestClass]
public class FechasPruebas
{
    [DataTestMethod]
    [DataRow(1999, 4, 4)]
    [DataRow(2023, 4, 9)]
    [DataRow(2024, 3, 31)]
    [DataRow(2025, 4, 20)]
    [DataRow(2026, 4, 5)]
    public void PascuaSegunFormulaDelDocente(int año, int mes, int dia) =>
        Assert.AreEqual(new DateTime(año, mes, dia), ServicioFechas.ObtenerPascua(año));

    [DataTestMethod]
    [DataRow(9, 9)]
    [DataRow(10, 16)]
    [DataRow(11, 16)]
    [DataRow(12, 16)]
    [DataRow(13, 16)]
    [DataRow(14, 16)]
    [DataRow(15, 16)]
    public void LunesPermaneceYLosDemasDiasSeTrasladan(int dia, int esperado) =>
        Assert.AreEqual(new DateTime(2023, 1, esperado), ServicioFechas.SiguienteLunes(new DateTime(2023, 1, dia)));

    [DataTestMethod]
    [DataRow(2023, 2, 29, false)]
    [DataRow(2024, 2, 29, true)]
    [DataRow(2023, 2, 35, false)]
    [DataRow(2023, 13, 1, false)]
    [DataRow(0, 1, 1, false)]
    [DataRow(10000, 1, 1, false)]
    [DataRow(2023, 4, 31, false)]
    public void ValidaFechaSinDesbordamientos(int año, int mes, int dia, bool esperado) =>
        Assert.AreEqual(esperado, ServicioFechas.EsFechaValida(año, mes, dia));
}
