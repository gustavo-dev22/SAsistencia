using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.AuditoriaMarcaciones.Commands
{
    public record RegistrarMarcaManualCommand(
        int EmpleadoId,
        DateTime Fecha,
        TimeSpan Hora,
        string TipoMarcacion, // ENTRADA o SALIDA
        string Motivo
    ) : IRequest<bool>;
}
