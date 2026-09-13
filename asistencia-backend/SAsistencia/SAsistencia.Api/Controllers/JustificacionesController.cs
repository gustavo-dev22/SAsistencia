using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Justificaciones.Commands;
using SAsistencia.Application.Features.Justificaciones.DTOs;
using SAsistencia.Application.Features.Justificaciones.Queries;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Domain.Entities;

namespace SAsistencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JustificacionesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IWebHostEnvironment _env;
    private readonly IJustificacionRepository _justificacionRepo;
    private readonly IUnitOfWork _unitOfWork;

    public JustificacionesController(
        ISender mediator,
        IWebHostEnvironment env,
        IJustificacionRepository justificacionRepo,
        IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _env = env;
        _justificacionRepo = justificacionRepo;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("tipos")]
    public async Task<ActionResult<List<TipoJustificacionDto>>> ListarTipos()
    {
        return Ok(await _mediator.Send(new GetTiposJustificacionQuery()));
    }

    [HttpPost("historial")]
    public async Task<ActionResult<PaginatedResult<JustificacionItemDto>>> ListarPaginadas([FromBody] FiltroJustificacionesRequest filtro)
    {
        return Ok(await _mediator.Send(new GetJustificacionesPaginadasQuery(filtro)));
    }

    [HttpPost("resolver")]
    public async Task<ActionResult> Resolver([FromBody] ResolverJustificacionRequest request)
    {
        var usuario = User.Identity?.Name ?? "SuperAdmin";
        var ok = await _mediator.Send(new ResolverJustificacionCommand(request, usuario));
        if (!ok) return BadRequest(new { mensaje = "No se pudo actualizar la justificación o ya fue procesada." });
        return Ok(new { mensaje = $"Justificación {request.Estado.ToLower()} correctamente." });
    }

    // Registro con carga de archivo PDF / Imagen
    [HttpPost("registrar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> RegistrarConArchivo(
        [FromForm] int empleadoId,
        [FromForm] int tipoJustificacionId,
        [FromForm] DateTime fechaInicio,
        [FromForm] DateTime fechaFin,
        [FromForm] string motivo,
        [FromForm] long? marcacionId,
        IFormFile? archivoSustento)
    {
        string? rutaGuardada = null;
        string? nombreOriginal = null;

        if (archivoSustento != null && archivoSustento.Length > 0)
        {
            var ext = Path.GetExtension(archivoSustento.FileName).ToLowerInvariant();
            var permitidas = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
            if (!permitidas.Contains(ext))
                return BadRequest(new { mensaje = "Formato no admitido. Suba PDF o imagen (PNG/JPG)." });

            var carpetaSustentos = Path.Combine(_env.ContentRootPath, "ArchivosSustento");
            if (!Directory.Exists(carpetaSustentos))
                Directory.CreateDirectory(carpetaSustentos);

            var nombreUnico = $"{Guid.NewGuid()}{ext}";
            var rutaFisica = Path.Combine(carpetaSustentos, nombreUnico);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivoSustento.CopyToAsync(stream);
            }

            rutaGuardada = nombreUnico;
            nombreOriginal = archivoSustento.FileName;
        }

        var entidad = new Justificacion
        {
            EmpleadoId = empleadoId,
            TipoJustificacionId = tipoJustificacionId,
            FechaInicio = fechaInicio.Date,
            FechaFin = fechaFin.Date,
            Motivo = motivo.Trim(),
            MarcacionId = marcacionId,
            RutaArchivo = rutaGuardada,
            NombreArchivoOriginal = nombreOriginal,
            Estado = "PENDIENTE",
            FechaRegistro = DateTime.UtcNow
        };

        await _justificacionRepo.AgregarAsync(entidad);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { mensaje = "Justificación radicada exitosamente bajo estado Pendiente." });
    }

    // Descarga segura del sustento
    [HttpGet("descargar/{id}")]
    public async Task<IActionResult> DescargarSustento(long id)
    {
        var justificacion = await _justificacionRepo.ObtenerPorIdAsync(id);
        if (justificacion == null || string.IsNullOrEmpty(justificacion.RutaArchivo))
            return NotFound("Archivo de sustento no encontrado.");

        var rutaFisica = Path.Combine(_env.ContentRootPath, "ArchivosSustento", justificacion.RutaArchivo);
        if (!System.IO.File.Exists(rutaFisica))
            return NotFound("El archivo ya no existe en el almacenamiento del servidor.");

        var bytes = await System.IO.File.ReadAllBytesAsync(rutaFisica);
        var ext = Path.GetExtension(justificacion.RutaArchivo).ToLowerInvariant();
        var contentType = ext == ".pdf" ? "application/pdf" : "image/jpeg";

        return File(bytes, contentType, justificacion.NombreArchivoOriginal ?? "Sustento" + ext);
    }
}