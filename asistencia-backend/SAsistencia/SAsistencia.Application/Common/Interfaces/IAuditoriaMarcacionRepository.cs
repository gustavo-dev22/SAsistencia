using SAsistencia.Application.Features.AuditoriaMarcaciones.DTOs;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IAuditoriaMarcacionRepository
    {
        Task<(List<AuditoriaMarcacion> Items, int Total)> ConsultarPaginadoAsync(FiltroAuditoriaRequest filtro, CancellationToken cancellationToken = default);
        Task AgregarAsync(AuditoriaMarcacion auditoria, CancellationToken cancellationToken = default);
    }
}
