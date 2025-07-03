using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Logica.Interfaces;

namespace TelegramBot.Logica.Servicios.HealthCheck
{
    public class ServicioDeSalud : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // Lógica simple: si todo va bien, OK
            // Podés validar una conexión, estado interno, etc.
            var estadoOk = true;

            if (estadoOk)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Servicio operativo"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Servicio con fallas"));
        }
    }
}

