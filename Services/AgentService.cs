using EcoPlantas.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoPlantas.Services
{
    public class AgentService
    {
        private readonly ApplicationDbContext _context;
        private readonly OllamaService _ollamaService;

        public AgentService(
            ApplicationDbContext context,
            OllamaService ollamaService)
        {
            _context = context;
            _ollamaService = ollamaService;
        }

        public async Task<string> ProcesarPregunta(string pregunta)
        {
            pregunta = pregunta.ToLower();

            // CONSULTA DE PLANTAS
            if (pregunta.Contains("planta") ||
                pregunta.Contains("catalogo") ||
                pregunta.Contains("catálogo") ||
                pregunta.Contains("canjear"))
            {
                var plantas = await _context.Productos
                    .Select(p => p.Nombre)
                    .ToListAsync();

                var prompt = $"""
                Eres EcoBot, el asistente inteligente de EcoPlantas.

                Las plantas disponibles actualmente son:

                {string.Join(", ", plantas)}

                Pregunta del usuario:
                {pregunta}

                Responde de forma breve, amigable y útil.
                """;

                return await _ollamaService
                    .GenerarRespuestaAsync(prompt);
            }

            // CONSULTA DE ESTADÍSTICAS DE RECICLAJE
            if (pregunta.Contains("cuánto") ||
                pregunta.Contains("cuanto") ||
                pregunta.Contains("total reciclado") ||
                pregunta.Contains("kg") ||
                pregunta.Contains("cantidad reciclada"))
            {
                var totalKg = await _context.Reciclajes
                    .SumAsync(r => r.Cantidad);

                var totalRegistros = await _context.Reciclajes
                    .CountAsync();

                return $"Actualmente se han reciclado {totalKg} kg en un total de {totalRegistros} registros.";
            }

            // CONSULTA DE IMPACTO AMBIENTAL
            if (pregunta.Contains("impacto") ||
                pregunta.Contains("medio ambiente") ||
                pregunta.Contains("contaminación") ||
                pregunta.Contains("contaminacion") ||
                pregunta.Contains("plástico") ||
                pregunta.Contains("plastico") ||
                pregunta.Contains("reciclar"))
            {
                var promptImpacto = $"""
                Eres EcoBot, el asistente ecológico oficial de EcoPlantas.

                Explica de manera sencilla, educativa y breve los beneficios
                ambientales relacionados con la siguiente consulta:

                {pregunta}

                La respuesta debe ser apta para ciudadanos y estudiantes.
                """;

                return await _ollamaService
                    .GenerarRespuestaAsync(promptImpacto);
            }

            // RESPUESTA GENERAL CON IA
            var promptGeneral = $"""
            Eres EcoBot, el asistente inteligente de EcoPlantas.

            EcoPlantas es una plataforma del distrito de Cieneguilla
            que promueve el reciclaje y el canje de plantas ecológicas.

            Responde de manera breve, amigable y profesional.

            Pregunta:
            {pregunta}
            """;

            return await _ollamaService
                .GenerarRespuestaAsync(promptGeneral);
        }
    }
}