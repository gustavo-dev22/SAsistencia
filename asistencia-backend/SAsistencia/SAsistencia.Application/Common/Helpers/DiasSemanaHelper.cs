using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SAsistencia.Application.Common.Helpers
{
    public static class DiasSemanaHelper
    {
        private const string DefaultDiasHabiles = "1,2,3,4,5";

        /// <summary>
        /// Limpia cualquier entrada (con puntos, guiones, espacios, letras o números fuera de rango),
        /// extrae solo los días válidos del 0 al 6, los ordena y elimina duplicados.
        /// Si el resultado es inválido o vacío, retorna "1,2,3,4,5".
        /// </summary>
        public static string NormalizarDias(string? entradaRaw)
        {
            if (string.IsNullOrWhiteSpace(entradaRaw))
                return DefaultDiasHabiles;

            // Extraer todos los dígitos individuales mediante Regex (soporta separadores como , . - ; / o espacios)
            var matches = Regex.Matches(entradaRaw, @"\b[0-6]\b");

            var diasValidos = matches
                .Select(m => int.Parse(m.Value))
                .Distinct()
                .OrderBy(d => d == 0 ? 7 : d) // Ordenar Lunes (1) a Domingo (0)
                .ToList();

            if (diasValidos.Count == 0)
                return DefaultDiasHabiles;

            return string.Join(",", diasValidos);
        }

        /// <summary>
        /// Convierte la cadena ya limpia a una lista de enteros para validación rápida en memoria.
        /// </summary>
        public static HashSet<int> ObtenerSetDias(string? entradaRaw)
        {
            var normalizado = NormalizarDias(entradaRaw);
            return normalizado
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToHashSet();
        }
    }
}
