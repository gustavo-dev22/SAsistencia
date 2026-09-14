using SAsistencia.Application.Common.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        private static readonly TimeZoneInfo PeruTimeZone = ObtenerZonaHorariaPeru();

        private static TimeZoneInfo ObtenerZonaHorariaPeru()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
            }
        }

        public DateTime AhoraPeru => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PeruTimeZone);
        public DateOnly HoyPeru => DateOnly.FromDateTime(AhoraPeru);
        public TimeSpan HoraActualPeru => AhoraPeru.TimeOfDay;
    }
}
