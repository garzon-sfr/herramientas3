using apiFestivos.core.servicios;
using apiFestivos.dominio.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ApiFestivos.Controladores;

[ApiController]
[Route("api/calendario")]
public class CalendarioControlador(IFestivoServicio servicio) : ControllerBase
{
    [HttpGet("festivos/{IdPais:int}/{Año:int}")]
    public async Task<ActionResult<IEnumerable<FestivoFechaDto>>> ObtenerFestivos(int IdPais, int Año) =>
        Ok(await servicio.ObtenerFestivos(IdPais, Año));

    [HttpGet("verificar/{IdPais:int}/{Año:int}/{Mes:int}/{Dia:int}")]
    public async Task<ActionResult<string>> Verificar(int IdPais, int Año, int Mes, int Dia) =>
        Ok(await servicio.EsFestivo(IdPais, Año, Mes, Dia) ? "Es Festivo" : "No es festivo");
}
