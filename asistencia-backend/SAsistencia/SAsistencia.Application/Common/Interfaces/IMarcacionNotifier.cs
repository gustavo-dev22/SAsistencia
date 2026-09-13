using SAsistencia.Application.Features.Marcaciones.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IMarcacionNotifier
    {
        Task NotificarNuevaMarcacionAsync(MarcacionEnVivoDto marcacion, CancellationToken cancellationToken = default);
    }
}
