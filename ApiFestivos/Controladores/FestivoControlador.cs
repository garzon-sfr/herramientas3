using apiFestivos.core.servicios;
using apiFestivos.dominio;
using ApiFestivos.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace ApiFestivos.Controladores;

[ApiController]
[Route("api/festivos")]
public class FestivoControlador(IFestivoServicio servicio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Festivo>>> ObtenerTodos() =>
        Ok(await servicio.ObtenerTodos());

    [HttpGet("{Id:int}")]
    public async Task<ActionResult<Festivo>> Obtener(int Id)
    {
        var entidad = await servicio.Obtener(Id);
        return entidad is null ? NotFound(new { mensaje = "Registro no encontrado." }) : Ok(entidad);
    }

    [HttpGet("buscar/{IndiceDato:int}/{Texto}")]
    public async Task<ActionResult<IEnumerable<Festivo>>> Buscar(int IndiceDato, string Texto)
    {
        if (IndiceDato != 1 || string.IsNullOrWhiteSpace(Texto))
            return BadRequest(new { mensaje = "Use el índice 1 y un texto para buscar por Nombre." });
        return Ok(await servicio.Buscar(IndiceDato, Texto.Trim()));
    }

    [HttpGet("pais/{IdPais:int}")]
    public async Task<ActionResult<IEnumerable<Festivo>>> ObtenerPais(int IdPais) =>
        Ok(await servicio.ObtenerPais(IdPais));

    [HttpPost]
    [ProducesResponseType(typeof(Festivo), StatusCodes.Status201Created)]
    public async Task<ActionResult<Festivo>> Agregar(FestivoSolicitud solicitud)
    {
        var entidad = await servicio.Agregar(new Festivo { Nombre = solicitud.Nombre, IdPais = solicitud.IdPais, IdTipo = solicitud.IdTipo,
            Dia = solicitud.Dia, Mes = solicitud.Mes, DiasPascua = solicitud.DiasPascua });
        return CreatedAtAction(nameof(Obtener), new { Id = entidad.Id }, entidad);
    }

    [HttpPut("{Id:int}")]
    public async Task<ActionResult<Festivo>> Modificar(int Id, FestivoSolicitud solicitud)
    {
        var entidad = await servicio.Modificar(new Festivo { Id = Id, Nombre = solicitud.Nombre, IdPais = solicitud.IdPais, IdTipo = solicitud.IdTipo,
            Dia = solicitud.Dia, Mes = solicitud.Mes, DiasPascua = solicitud.DiasPascua });
        return entidad is null ? NotFound(new { mensaje = "Registro no encontrado." }) : Ok(entidad);
    }

    [HttpDelete("{Id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Eliminar(int Id) =>
        await servicio.Eliminar(Id) ? NoContent() : NotFound(new { mensaje = "Registro no encontrado." });
}
