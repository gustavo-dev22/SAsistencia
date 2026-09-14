using MediatR;
using SAsistencia.Application.Common.Helpers;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Providers;
using SAsistencia.Application.Features.Marcaciones.Commands;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Marcaciones.Handlers
{
    public class RegistrarMarcaCommandHandler : IRequestHandler<RegistrarMarcaCommand, ResultadoMarcacionDto>
    {
        private readonly IEmpleadoRepository _empleadoRepo;
        private readonly IMarcacionRepository _marcacionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMarcacionNotifier _notifier;
        private readonly IFeriadoRepository _feriadoRepo;
        private readonly IParametroRepository _parametroRepo;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RegistrarMarcaCommandHandler(
            IEmpleadoRepository empleadoRepo,
            IMarcacionRepository marcacionRepo,
            IUnitOfWork unitOfWork,
            IMarcacionNotifier notifier,
            IFeriadoRepository feriadoRepo,
            IParametroRepository parametroRepo,
            IDateTimeProvider dateTimeProvider)
        {
            _empleadoRepo = empleadoRepo;
            _marcacionRepo = marcacionRepo;
            _unitOfWork = unitOfWork;
            _notifier = notifier;
            _feriadoRepo = feriadoRepo;
            _parametroRepo = parametroRepo;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ResultadoMarcacionDto> Handle(RegistrarMarcaCommand request, CancellationToken cancellationToken)
        {
            var idLimpio = request.Dto.Identificador.Trim();
            var ahoraPeru = _dateTimeProvider.AhoraPeru;

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

            bool esExonerado = empleado.Cargo?.ExoneradoMarcacion ?? false;

            // 2. VERIFICACIÓN Y BLOQUEO DE FERIADOS
            var feriadoHoy = await _feriadoRepo.ObtenerPorFechaAsync(ahoraPeru, cancellationToken);
            bool bloquearFeriados = await _parametroRepo.ObtenerValorAsync("BLOQUEAR_MARCACIONES_FERIADOS", false, cancellationToken);

            if (feriadoHoy != null && bloquearFeriados && !esExonerado)
            {
                return new ResultadoMarcacionDto
                {
                    Exito = false,
                    Mensaje = $"Hoy es día no laborable ({feriadoHoy.Nombre}). Marcación no autorizada."
                };
            }

            // 3. VERIFICACIÓN Y BLOQUEO DE DÍAS DE DESCANSO SEMANAL
            int diaActualNumero = (int)ahoraPeru.DayOfWeek; // 0 = Domingo, 1 = Lunes, ..., 6 = Sábado
            HashSet<int> diasLaborablesSet;

            if (empleado.Turno != null && !string.IsNullOrWhiteSpace(empleado.Turno.DiasSemana))
            {
                diasLaborablesSet = DiasSemanaHelper.ObtenerSetDias(empleado.Turno.DiasSemana);
            }
            else
            {
                var diasHabilesParam = await _parametroRepo.ObtenerValorAsync("DIAS_HABILES_SEMANA", "1,2,3,4,5", cancellationToken);
                diasLaborablesSet = DiasSemanaHelper.ObtenerSetDias(diasHabilesParam);
            }

            bool esDiaLaborable = diasLaborablesSet.Contains(diaActualNumero);
            bool bloquearDescansos = await _parametroRepo.ObtenerValorAsync("BLOQUEAR_MARCACIONES_DESCANSO", true, cancellationToken);

            if (!esDiaLaborable && bloquearDescansos && !esExonerado)
            {
                return new ResultadoMarcacionDto
                {
                    Exito = false,
                    Mensaje = "Hoy es su día de descanso semanal. Marcación no autorizada por su horario."
                };
            }

            // 4. Obtener marcas registradas hoy por el empleado
            var marcasHoy = await _marcacionRepo.ObtenerMarcacionesHoyAsync(empleado.Id, ahoraPeru, cancellationToken);
            var ultimaMarca = marcasHoy.LastOrDefault();

            // A. Cooldown dinámico parametrizado
            int cooldownSecs = await _parametroRepo.ObtenerValorAsync("COOLDOWN_MARCACION_SEGUNDOS", 60, cancellationToken);
            if (ultimaMarca != null && (ahoraPeru - ultimaMarca.FechaHoraMarcacion).TotalSeconds < cooldownSecs)
            {
                return new ResultadoMarcacionDto
                {
                    Exito = false,
                    Mensaje = $"Espere un momento. Última marca registrada a las {ultimaMarca.FechaHoraMarcacion:HH:mm:ss}."
                };
            }

            // B. Control de ciclo diario (Máximo 1 Entrada y 1 Salida)
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

            string tipoMarcacion = totalEntradas == 0 ? "ENTRADA" : "SALIDA";

            // 5. Evaluar Puntualidad (si pasó los bloqueos o si bloquearDescansos está en false)
            string estadoPuntualidad = "PUNTUAL";
            int minutosTardanza = 0;

            if (feriadoHoy != null)
            {
                estadoPuntualidad = feriadoHoy.EsCompensable ? "NO_LABORABLE_COMPENSABLE" : "FERIADO";
                minutosTardanza = 0;
            }
            else if (!esDiaLaborable)
            {
                estadoPuntualidad = "DESCANSO_SEMANAL";
                minutosTardanza = 0;
            }
            else if (esExonerado)
            {
                estadoPuntualidad = "EXONERADO";
            }
            else if (empleado.Turno != null && tipoMarcacion == "ENTRADA")
            {
                var horaActualTime = ahoraPeru.TimeOfDay;
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
                    estadoPuntualidad = "FUERA_TURNO";
                    minutosTardanza = (int)Math.Ceiling((horaActualTime - entradaProgramada).TotalMinutes);
                }
            }
            else if (empleado.Turno == null)
            {
                estadoPuntualidad = "SIN_TURNO";
            }

            // 6. Persistir en Base de Datos
            var nuevaMarca = new Marcacion
            {
                EmpleadoId = empleado.Id,
                TurnoId = empleado.TurnoId,
                FechaHoraMarcacion = ahoraPeru,
                TipoMarcacion = tipoMarcacion,
                EstadoPuntualidad = estadoPuntualidad,
                MinutosTardanza = minutosTardanza,
                MetodoRegistro = request.Dto.Metodo.ToUpper(),
                IpTerminal = request.IpTerminal,
                EsManual = false
            };

            await _marcacionRepo.AgregarAsync(nuevaMarca, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Notificación en Tiempo Real (SignalR)
            var dtoEnVivo = new MarcacionEnVivoDto
            {
                Id = nuevaMarca.Id,
                EmpleadoId = empleado.Id,
                NombreCompleto = empleado.NombreCompleto,
                Dni = empleado.Dni,
                OficinaNombre = empleado.Oficina?.Nombre,
                OficinaSigla = empleado.Oficina?.Sigla,
                CargoNombre = empleado.Cargo?.Nombre,
                Hora = ahoraPeru.ToString("HH:mm:ss"),
                TipoMarcacion = tipoMarcacion,
                EstadoPuntualidad = estadoPuntualidad,
                MinutosTardanza = minutosTardanza,
                MetodoRegistro = request.Dto.Metodo.ToUpper(),
                FechaHora = ahoraPeru
            };

            await _notifier.NotificarNuevaMarcacionAsync(dtoEnVivo, cancellationToken);

            return new ResultadoMarcacionDto
            {
                Exito = true,
                Mensaje = $"{tipoMarcacion} registrada con éxito.",
                NombreEmpleado = empleado.NombreCompleto,
                AreaEmpleado = empleado.Oficina != null ? $"{empleado.Oficina.Sigla} - {empleado.Oficina.Nombre}" : "Sede Central",
                CargoEmpleado = empleado.Cargo?.Nombre ?? "Colaborador",
                Hora = ahoraPeru.ToString("HH:mm:ss"),
                TipoMarcacion = tipoMarcacion,
                EstadoPuntualidad = estadoPuntualidad,
                MinutosTardanza = minutosTardanza
            };
        }
    }
}
