using Microsoft.EntityFrameworkCore;
using SubastaYa.Data;
using System;
using System.Threading.Tasks;

namespace SubastaYa.Infrastructure.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Ajusta dinámicamente las ventanas temporales de las subastas seed al levantar la aplicación,
        /// asegurando que la subasta estándar cierre en ~25 min, la crítica en ~90 seg (alerta roja y anti-sniping)
        /// y la programada a +24 hs, independientemente de la fecha y hora en que se ejecute el sistema.
        /// </summary>
        public static async Task InicializarVentanasRelativasAsync(SubastaYaContext context)
        {
            var now = DateTime.UtcNow;

            // 1. Activa estándar (Id: 1): Cierre en ventana corta (20-30 min)
            var subasta1 = await context.Subastas.FirstOrDefaultAsync(s => s.Id == 1 && s.Estado == "ACTIVA");
            if (subasta1 != null)
            {
                subasta1.FechaInicio = now.AddHours(-1);
                subasta1.FechaFin = now.AddMinutes(25);
            }

            // 2. Activa crítica (Id: 2): Cierre en menos de 2 min (90 segundos para alerta visual roja < 60s y anti-sniping)
            var subasta2 = await context.Subastas.FirstOrDefaultAsync(s => s.Id == 2 && s.Estado == "ACTIVA");
            if (subasta2 != null)
            {
                subasta2.FechaInicio = now.AddHours(-1);
                subasta2.FechaFin = now.AddSeconds(90);
            }

            // 3. Próxima (Id: 3): Inicio programado a +24 hs (pujas bloqueadas)
            var subasta3 = await context.Subastas.FirstOrDefaultAsync(s => s.Id == 3 && s.Estado == "PROGRAMADA");
            if (subasta3 != null)
            {
                subasta3.FechaInicio = now.AddHours(24);
                subasta3.FechaFin = now.AddDays(7);
            }

            // 4. Vencida con ganador (Id: 4): Fecha fin pasada para liquidación del worker
            var subasta4 = await context.Subastas.FirstOrDefaultAsync(s => s.Id == 4 && s.Estado == "ACTIVA");
            if (subasta4 != null)
            {
                subasta4.FechaInicio = now.AddDays(-2);
                subasta4.FechaFin = now.AddHours(-2);
            }

            // 5. Vencida desierta (Id: 5): Fecha fin pasada sin ofertas
            var subasta5 = await context.Subastas.FirstOrDefaultAsync(s => s.Id == 5 && s.Estado == "ACTIVA");
            if (subasta5 != null)
            {
                subasta5.FechaInicio = now.AddDays(-2);
                subasta5.FechaFin = now.AddHours(-3);
            }

            await context.SaveChangesAsync();
        }
    }
}
