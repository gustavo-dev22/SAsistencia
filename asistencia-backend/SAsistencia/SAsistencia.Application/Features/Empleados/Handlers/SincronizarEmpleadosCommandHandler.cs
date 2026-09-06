using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Empleados.Commands;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Empleados.Handlers
{
    public class SincronizarEmpleadosCommandHandler : IRequestHandler<SincronizarEmpleadosCommand, int>
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IOrganizacionRepository _organizacionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISasiAuthService _sasiService;

        public SincronizarEmpleadosCommandHandler(
            IEmpleadoRepository empleadoRepository,
            IOrganizacionRepository organizacionRepository,
            IUnitOfWork unitOfWork,
            ISasiAuthService sasiService)
        {
            _empleadoRepository = empleadoRepository;
            _organizacionRepository = organizacionRepository;
            _unitOfWork = unitOfWork;
            _sasiService = sasiService;
        }

        public async Task<int> Handle(SincronizarEmpleadosCommand request, CancellationToken cancellationToken)
        {
            // 1. VALIDACIÓN / AUTO-SINCRONIZACIÓN:
            // Comprobar si existen oficinas locales. Si no existen, sincronizarlas primero de forma automática.
            var idsOficinasLocales = await _organizacionRepository.ObtenerIdsOficinasExistentesAsync(cancellationToken);

            if (idsOficinasLocales.Count == 0)
            {
                // Traer y persistir oficinas primero
                var oficinasSasi = await _sasiService.ObtenerOficinasActivasAsync(request.Token, cancellationToken);
                var entidadesOficinas = oficinasSasi.Select(s => new Oficina
                {
                    Id = s.IdOficina,
                    Nombre = s.Nombre,
                    Sigla = s.Sigla,
                    OficinaPadreId = s.OficinaPadreId,
                    Activo = s.Activo,
                    FechaSincronizacion = DateTime.UtcNow
                }).ToList();

                await _organizacionRepository.SincronizarOficinasAsync(entidadesOficinas, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Actualizar la lista de IDs de oficinas disponibles
                idsOficinasLocales = await _organizacionRepository.ObtenerIdsOficinasExistentesAsync(cancellationToken);
            }

            // 2. OBTENER Y PROCESAR EMPLEADOS DE SASI
            var usuariosSasi = await _sasiService.ObtenerUsuariosSistemaAsync(21, request.Token, cancellationToken);
            var empleadosLocales = await _empleadoRepository.ObtenerTodosAsync(cancellationToken);

            var nuevosEmpleados = new List<Empleado>();

            foreach (var u in usuariosSasi)
            {
                var local = empleadosLocales.FirstOrDefault(e => e.UsuarioIdSasi == u.UsuarioId);

                // Validar que la oficina realmente exista en la BD local para no violar la FK
                int? oficinaIdValida = (u.IdOficina.HasValue && idsOficinasLocales.Contains(u.IdOficina.Value))
                    ? u.IdOficina.Value
                    : null;

                if (local == null)
                {
                    nuevosEmpleados.Add(new Empleado
                    {
                        UsuarioIdSasi = u.UsuarioId,
                        NombreCompleto = u.NombreCompleto,
                        Email = u.Email,
                        OficinaId = oficinaIdValida, // Asignación segura
                        CodigoQr = Guid.NewGuid().ToString("N")[..12].ToUpper(),
                        HabilitadoParaMarcar = true,
                        FechaSincronizacion = DateTime.UtcNow
                    });
                }
                else
                {
                    bool huboCambio = false;

                    if (local.OficinaId != oficinaIdValida)
                    {
                        local.OficinaId = oficinaIdValida;
                        huboCambio = true;
                    }
                    if (local.NombreCompleto != u.NombreCompleto)
                    {
                        local.NombreCompleto = u.NombreCompleto;
                        huboCambio = true;
                    }
                    if (local.Email != u.Email)
                    {
                        local.Email = u.Email;
                        huboCambio = true;
                    }

                    if (huboCambio)
                    {
                        await _empleadoRepository.ActualizarAsync(local, cancellationToken);
                    }
                }
            }

            if (nuevosEmpleados.Count > 0)
            {
                await _empleadoRepository.AgregarRangoAsync(nuevosEmpleados, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return nuevosEmpleados.Count;
        }
    }
}
