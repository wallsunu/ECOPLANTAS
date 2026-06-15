using System.ComponentModel;
using EcoPlantas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

namespace EcoPlantas.Services.SemanticKernel
{
    /// <summary>
    /// Plugin nativo de Semantic Kernel con funciones que el Kernel puede invocar
    /// para enriquecer las respuestas del asistente EcoBot con datos reales del sistema.
    /// </summary>
    public class EcoPlantasPlugin
    {
        private readonly ApplicationDbContext _context;

        // El Kernel resuelve esta dependencia desde sus Services al usar AddFromType<EcoPlantasPlugin>.
        public EcoPlantasPlugin(ApplicationDbContext context)
        {
            _context = context;
        }

        [KernelFunction]
        [Description("Obtiene el número total de registros de reciclaje realizados en el sistema EcoPlantas")]
        public async Task<string> ObtenerCantidadReciclajes()
        {
            var cantidad = await _context.Reciclajes.CountAsync();
            return cantidad.ToString();
        }

        [KernelFunction]
        [Description("Devuelve información general sobre la plataforma EcoPlantas, su propósito y su ámbito")]
        public Task<string> ObtenerInformacionEcoPlantas()
        {
            const string informacion = """
                EcoPlantas es una plataforma del distrito de Cieneguilla, Perú,
                que promueve el reciclaje responsable y el canje de plantas ecológicas.
                Los vecinos registran sus aportes de reciclaje y pueden canjearlos por
                plantas del catálogo, fomentando un estilo de vida más sostenible.
                """;
            return Task.FromResult(informacion);
        }

        [KernelFunction]
        [Description("Genera una recomendación ecológica práctica para el usuario, opcionalmente según el tipo de material a reciclar")]
        public Task<string> GenerarRecomendacionEcologica(
            [Description("Tipo de material sobre el que se desea una recomendación, por ejemplo: plástico, papel, vidrio. Puede ir vacío.")]
            string tipoMaterial = "")
        {
            var recomendacion = string.IsNullOrWhiteSpace(tipoMaterial)
                ? "Separa tus residuos por tipo (plástico, papel, vidrio) y entrégalos limpios y secos para maximizar su reciclaje."
                : tipoMaterial.Trim().ToLower() switch
                {
                    "plastico" or "plástico" => "Enjuaga los envases de plástico, aplástalos para ahorrar espacio y evita mezclarlos con restos orgánicos.",
                    "papel" or "carton" or "cartón" => "Mantén el papel y el cartón secos, retira cintas y grapas, y aplánalos antes de entregarlos.",
                    "vidrio" => "Entrega el vidrio sin tapas ni residuos líquidos; no es necesario quitar las etiquetas.",
                    _ => $"Para reciclar {tipoMaterial}, asegúrate de que esté limpio, seco y separado del resto de residuos."
                };

            return Task.FromResult(recomendacion);
        }

        [KernelFunction]
        [Description("Obtiene el total de kilogramos reciclados registrados en el sistema")]
        public async Task<string> ObtenerTotalKgReciclados()
        {
            var total = await _context.Reciclajes.SumAsync(r => (int?)r.Cantidad) ?? 0;
            return $"{total} kg";
        }

        [KernelFunction]
        [Description("Obtiene el número total de registros de reciclaje en el sistema")]
        public async Task<string> ObtenerTotalRegistros()
        {
            var count = await _context.Reciclajes.CountAsync();
            return count.ToString();
        }

        [KernelFunction]
        [Description("Obtiene la lista de plantas ecológicas disponibles en el catálogo")]
        public async Task<string> ObtenerProductosDisponibles()
        {
            var plantas = await _context.Productos
                .Where(p => p.Disponible)
                .Select(p => p.Nombre)
                .ToListAsync();
            return plantas.Any()
                ? string.Join(", ", plantas)
                : "No hay plantas disponibles actualmente";
        }

        [KernelFunction]
        [Description("Genera el contexto ecológico del sistema EcoPlantas")]
        public Task<string> ObtenerContextoEcologico()
        {
            const string contexto = """
                Eres EcoBot, el asistente inteligente de EcoPlantas.
                EcoPlantas es una plataforma del distrito de Cieneguilla, Perú,
                que promueve el reciclaje responsable y el canje de plantas ecológicas.
                Tu objetivo es ayudar a los vecinos a entender el impacto positivo
                de sus acciones de reciclaje y guiarlos hacia un estilo de vida más sostenible.
                """;
            return Task.FromResult(contexto);
        }
    }
}
