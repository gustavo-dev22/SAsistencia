using System;
using System.Collections.Generic;
using System.Text;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Marcaciones.Commands;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Domain.Entities;
using MediatR;

namespace SAsistencia.Application.Features.Marcaciones.Handlers
{
    public class RegistrarMarcaCommandHandler : IRequestHandler<RegistrarMarcaCommand, ResultadoMarcacionDto>
    {
        private readonly IEmpleadoRepository _empleadoRepo;
        private readonly IMarcacionRepository _marcacionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMarcacionNotifier _notifier;

        public RegistrarMarcaCommandHandler(
            IEmpleadoRepository empleadoRepo,
            IMarcacionRepository marcacionRepo,
            IUnitOfWork unitOfWork,
            IMarcacionNotifier notifier)
        {
            _empleadoRepo = empleadoRepo;
            _marcacionRepo = marcacionRepo;
            _unitOfWork = unitOfWork;
            _notifier = notifier;
        }

        public async Task<ResultadoMarcacionDto> Handle(RegistrarMarcaCommand request, CancellationToken cancellationToken)
        {
            var idLimpio = request.Dto.Identificador.Trim();
            var ahora = DateTime.Now;

            // 1. Buscar colaborador con relaciones cargadas
            var empleado = await _empleadoRepo.ObtenerPorIdentificadorConRelacionesAsync(idLimpio, cancellationToken);

            if (empleado == null)
            {
                return new ResultadoMarcacionDto
                {
                    Exito = false,
                    Mensaje = "Identificación no encontrada en el padrón institucional."
                };
            }

            if (!empleado.HabilitadoParaMarcar)
            {
                return new ResultadoMarcacionDto
                {
                    Exito = false,
                    Mensaje = "El colaborador se encuentra con marcación bloqueada por administración."
                };
            }

            // 2. Obtener todas las marcas registradas hoy por el empleado
            var marcasHoy = await _marcacionRepo.ObtenerMarcacionesHoyAsync(empleado.Id, ahora, cancellationToken);
            var ultimaMarca = marcasHoy.LastOrDefault();

            // A. Cooldown de seguridad (Evitar doble marca por rebote en menos de 60 segundos)
            if (ultimaMarca != null && (ahora - ultimaMarca.FechaHoraMarcacion).TotalSeconds < 60)
            {
                return new ResultadoMarcacionDto
                {
                    Exito = false,
                    Mensaje = $"Espere un momento. Última marca registrada a las {ultimaMarca.FechaHoraMarcacion:HH:mm:ss}."
                };
            }

            // B. Control de ciclo diario (Máximo 1 ENTRADA y 1 SALIDA en jornada regular)
            int totalEntradas = marcasHoy.Count(m => m.TipoMarcacion == "ENTRADA");
            int totalSalidas = marcasHoy.Count(m => m.TipoMarcacion == "SALIDA");

            if (totalEntradas >= 1 && totalSalidas >= 1)
            {
                return new ResultadoMarcacionDto
                {
                    Exito = false,
                    Mensaje = "Jornada laboral completa. Ya cuenta con Entrada y Salida registradas el día de hoy."
                };
            }

            // C. Determinar el tipo estricto de marcación
            string tipoMarcacion = totalEntradas == 0 ? "ENTRADA" : "SALIDA";

            // 3. Evaluar Puntualidad con base al Turno asignado
            string estadoPuntualidad = "PUNTUAL";
            int minutosTardanza = 0;

            if (empleado.Cargo?.ExoneradoMarcacion == true)
            {
                estadoPuntualidad = "EXONERADO";
            }
            else if (empleado.Turno != null && tipoMarcacion == "ENTRADA")
            {
                var horaActualTime = ahora.TimeOfDay;
                var entradaProgramada = empleado.Turno.HoraEntrada;
                var limiteTolerancia = entradaProgramada.Add(TimeSpan.FromMinutes(empleado.Turno.ToleranciaEntradaMinutos));
                var limiteMaximoTardanza = entradaProgramada.Add(TimeSpan.FromMinutes(empleado.Turno.LimiteTardanzaMinutos));

                if (horaActualTime <= limiteTolerancia)
                {
                    estadoPuntualidad = (horaActualTime <= entradaProgramada) ? "PUNTUAL" : "TOLERANCIA";
                }
                else if (horaActualTime <= limiteMaximoTardanza)
                {
                    estadoPuntualidad = "TARDANZA";
                    minutosTardanza = (int)Math.Ceiling((horaActualTime - entradaProgramada).TotalMinutes);
                }
                else
                {
                    // Pasado el límite máximo (ej. marcar de noche cuando el turno era diurno)
                    estadoPuntualidad = "FUERA_TURNO";
                    minutosTardanza = (int)Math.Ceiling((horaActualTime - entradaProgramada).TotalMinutes);
                }
            }
            else if (empleado.Turno == null)
            {
                estadoPuntualidad = "SIN_TURNO";
            }

            // 4. Persistir Marcación mediante el repositorio
            var nuevaMarca = new Marcacion
            {
                EmpleadoId = empleado.Id,
                TurnoId = empleado.TurnoId,
                FechaHoraMarcacion = ahora,
                TipoMarcacion = tipoMarcacion,
                EstadoPuntualidad = estadoPuntualidad,
                MinutosTardanza = minutosTardanza,
                MetodoRegistro = request.Dto.Metodo.ToUpper(),
                IpTerminal = request.IpTerminal
            };

            await _marcacionRepo.AgregarAsync(nuevaMarca, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // DISPARO EN TIEMPO REAL: Notificar al Live Monitor
            var dtoEnVivo = new MarcacionEnVivoDto
            {
                Id = nuevaMarca.Id,
                EmpleadoId = empleado.Id,
                NombreCompleto = empleado.NombreCompleto,
                Dni = empleado.Dni,
                OficinaNombre = empleado.Oficina?.Nombre,
                OficinaSigla = empleado.Oficina?.Sigla,
                CargoNombre = empleado.Cargo?.Nombre,
                Hora = ahora.ToString("HH:mm:ss"),
                TipoMarcacion = tipoMarcacion,
                EstadoPuntualidad = estadoPuntualidad,
                MinutosTardanza = minutosTardanza,
                MetodoRegistro = request.Dto.Metodo.ToUpper(),
                FechaHora = ahora
            };

            await _notifier.NotificarNuevaMarcacionAsync(dtoEnVivo, cancellationToken);

            return new ResultadoMarcacionDto
            {
                Exito = true,
                Mensaje = $"{tipoMarcacion} registrada con éxito.",
                NombreEmpleado = empleado.NombreCompleto,
                AreaEmpleado = empleado.Oficina != null ? $"{empleado.Oficina.Sigla} - {empleado.Oficina.Nombre}" : "Sede Central",
                CargoEmpleado = empleado.Cargo?.Nombre ?? "Colaborador",
                Hora = ahora.ToString("HH:mm:ss"),
                TipoMarcacion = tipoMarcacion,
                EstadoPuntualidad = estadoPuntualidad,
                MinutosTardanza = minutosTardanza
            };
        }
    }
}
