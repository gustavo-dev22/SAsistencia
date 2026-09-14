using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Providers
{
    public interface IDateTimeProvider
    {
        DateTime AhoraPeru { get; }
        DateOnly HoyPeru { get; }
        TimeSpan HoraActualPeru { get; }
    }
}
