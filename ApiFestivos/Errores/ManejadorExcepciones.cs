using apiFestivos.core.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiFestivos.Errores;

public class ManejadorExcepciones(ILogger<ManejadorExcepciones> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception error, CancellationToken cancellationToken)
    {
        var (estado, mensaje) = error switch
        {
            ArgumentException => (400, error.Message),
            KeyNotFoundException => (404, error.Message),
            ConflictoException => (409, error.Message),
            DbUpdateException { InnerException: SqlException sql } when sql.Number is 2601 or 2627 or 547 =>
                (409, "El registro está duplicado o tiene relaciones que impiden la operación."),
            InvalidOperationException { InnerException: SqlException } =>
                (503, "No fue posible acceder a la base de datos Festivos. Revise la conexión configurada."),
            SqlException => (503, "No fue posible acceder a la base de datos Festivos. Revise la conexión configurada."),
            _ => (500, "Ocurrió un error al procesar la solicitud.")
        };
        if (estado >= 500)
            logger.LogError(error, "Error al procesar {Ruta}", contexto.Request.Path);
        contexto.Response.StatusCode = estado;
        await contexto.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = estado,
            Title = mensaje,
            Instance = contexto.Request.Path
        }, cancellationToken);
        return true;
    }
}
