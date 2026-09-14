using MediatR;
using SAsistencia.Application.Common.Helpers;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Providers;
using SAsistencia.Application.Common.Services;
using SAsistencia.Application.Features.ParametroGlobal.Commands;
using SAsistencia.Application.Features.ParametroGlobal.DTOs;
using SAsistencia.Application.Features.ParametroGlobal.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.ParametroGlobal.Handlers
{
    public class ParametroHandlers :
    IRequestHandler<GetParametrosGlobalesQuery, List<ParametroGlobalDto>>,
    IRequestHandler<GuardarParametrosBatchCommand, bool>
    {
        private readonly IParametroRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        public ParametroHandlers(IParametroRepository repo, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public async Task<List<ParametroGlobalDto>> Handle(GetParametrosGlobalesQuery request, CancellationToken cancellationToken)
        {
            var lista = await _repo.ObtenerTodosAsync(cancellationToken);
            return lista.Select(p => new ParametroGlobalDto
            {
                Id = p.Id,
                Clave = p.Clave,
                Valor = p.Valor,
                TipoDato = p.TipoDato,
                Categoria = p.Categoria,
                Etiqueta = p.Etiqueta,
                Descripcion = p.Descripcion,
                FechaModificacion = p.FechaModificacion.ToString("dd/MM/yyyy HH:mm"),
                UsuarioModificador = p.UsuarioModificador ?? "Sistema"
            }).ToList();
        }

        public async Task<bool> Handle(GuardarParametrosBatchCommand request, CancellationToken cancellationToken)
        {
            if (request.Dto.Parametros == null || request.Dto.Parametros.Count == 0)
                return false;

            bool huboCambios = false;

            foreach (var item in request.Dto.Parametros)
            {
                var param = await _repo.ObtenerPorClaveAsync(item.Clave, cancellationToken);
                if (param != null)
                {
                    // Si el parámetro es DIAS_HABILES_SEMANA, se sanitiza y normaliza defensivamente
                    string nuevoValorLimpio = item.Clave == "DIAS_HABILES_SEMANA"
                        ? DiasSemanaHelper.NormalizarDias(item.Valor)
                        : item.Valor.Trim();

                    // Solo actualizar la entidad si el valor es efectivamente diferente
                    if (param.Valor != nuevoValorLimpio)
                    {
                        param.Valor = nuevoValorLimpio;
                        param.FechaModificacion = _dateTimeProvider.AhoraPeru;
                        param.UsuarioModificador = _currentUserService.NombreCompleto;
                        await _repo.ActualizarAsync(param, cancellationToken);
                        huboCambios = true;
                    }
                }
            }

            if (huboCambios)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}
