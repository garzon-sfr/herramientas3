using apiFestivos.core.servicios;
using apiFestivos.dominio;
using ApiFestivos.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace ApiFestivos.Controladores;

[ApiController]
[Route("api/paises")]
public class PaisControlador(IPaisServicio servicio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pais>>> ObtenerTodos() =>
        Ok(await servicio.ObtenerTodos());

    [HttpGet("{Id:int}")]
    public async Task<ActionResult<Pais>> Obtener(int Id)
    {
        var entidad = await servicio.Obtener(Id);
        return entidad is null ? NotFound(new { mensaje = "Registro no encontrado." }) : Ok(entidad);
    }

    [HttpGet("buscar/{IndiceDato:int}/{Texto}")]
    public async Task<ActionResult<IEnumerable<Pais>>> Buscar(int IndiceDato, string Texto)
    {
        if (IndiceDato != 1 || string.IsNullOrWhiteSpace(Texto))
            return BadRequest(new { mensaje = "Use el índice 1 y un texto para buscar por Nombre." });
        return Ok(await servicio.Buscar(IndiceDato, Texto.Trim()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Pais), StatusCodes.Status201Created)]
    public async Task<ActionResult<Pais>> Agregar(PaisSolicitud solicitud)
    {
        var entidad = await servicio.Agregar(new Pais { Nombre = solicitud.Nombre });
        return CreatedAtAction(nameof(Obtener), new { Id = entidad.Id }, entidad);
    }

    [HttpPut("{Id:int}")]
    public async Task<ActionResult<Pais>> Modificar(int Id, PaisSolicitud solicitud)
    {
        var entidad = await servicio.Modificar(new Pais { Id = Id, Nombre = solicitud.Nombre });
        return entidad is null ? NotFound(new { mensaje = "Registro no encontrado." }) : Ok(entidad);
    }

    [HttpDelete("{Id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Eliminar(int Id) =>
        await servicio.Eliminar(Id) ? NoContent() : NotFound(new { mensaje = "Registro no encontrado." });
}
