using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using EcoPlantas.Data;
using EcoPlantas.Services.LLM;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace EcoPlantas.Services.SemanticKernel
{
    public class EcoPlantasSemanticKernelService
    {
        private readonly ApplicationDbContext _context;
        private readonly LlmProviderService _llm;
        private readonly Kernel _kernel;

        public EcoPlantasSemanticKernelService(
            ApplicationDbContext context,
            LlmProviderService llm)
        {
            _context = context;
            _llm = llm;

            // Construir el Kernel con un chat completion dummy que delega a Ollama
            _kernel = Kernel.CreateBuilder()
                .Build();

            // Registrar el plugin de EcoPlantas con funciones nativas
            _kernel.Plugins.AddFromObject(new EcoPlantasPlugin(context), "EcoPlantas");
        }

        /// <summary>
        /// Construye un contexto enriquecido usando los plugins del Kernel
        /// y delega la generación de respuesta al proveedor LLM configurado.
        /// </summary>
        public async Task<string> ProcesarConContextoAsync(string pregunta)
        {
            // Invocar funciones del plugin para enriquecer el contexto
            var totalKg = await InvocarFuncionAsync("EcoPlantas", "ObtenerTotalKgReciclados");
            var totalRegistros = await InvocarFuncionAsync("EcoPlantas", "ObtenerTotalRegistros");
            var plantasDisponibles = await InvocarFuncionAsync("EcoPlantas", "ObtenerProductosDisponibles");
            var contextoSistema = await InvocarFuncionAsync("EcoPlantas", "ObtenerContextoEcologico");

            // Construir prompt enriquecido con datos reales del Kernel
            var prompt = $"""
            {contextoSistema}

            Datos actuales del sistema (obtenidos via Semantic Kernel):
            - Total reciclado: {totalKg} kg
            - Registros de reciclaje: {totalRegistros}
            - Plantas disponibles: {plantasDisponibles}

            Pregunta del usuario:
            {pregunta}

            Responde de forma breve, amigable y útil en español.
            """;

            return await _llm.GenerarRespuestaAsync(prompt);
        }

        /// <summary>
        /// Devuelve información de estado del Kernel para el endpoint de prueba.
        /// </summary>
        public async Task<object> ObtenerEstadoKernelAsync()
        {
            var plugins = _kernel.Plugins
                .Select(p => new
                {
                    nombre = p.Name,
                    funciones = p.Select(f => f.Name).ToList()
                })
                .ToList();

            var totalKg = await InvocarFuncionAsync("EcoPlantas", "ObtenerTotalKgReciclados");
            var totalRegistros = await InvocarFuncionAsync("EcoPlantas", "ObtenerTotalRegistros");
            var plantas = await InvocarFuncionAsync("EcoPlantas", "ObtenerProductosDisponibles");

            return new
            {
                semanticKernelVersion = typeof(Kernel).Assembly.GetName().Version?.ToString(),
                kernelActivo          = true,
                pluginsRegistrados    = plugins,
                datosEnTiempoReal = new
                {
                    totalKgReciclados = totalKg,
                    totalRegistros,
                    plantasDisponibles = plantas
                }
            };
        }

        private async Task<string> InvocarFuncionAsync(string plugin, string funcion)
        {
            try
            {
                var resultado = await _kernel.InvokeAsync(plugin, funcion);
                return resultado.GetValue<string>() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    // Plugin nativo con funciones que el Kernel puede invocar
    public class EcoPlantasPlugin
    {
        private readonly ApplicationDbContext _context;

        public EcoPlantasPlugin(ApplicationDbContext context)
        {
            _context = context;
        }

        [KernelFunction, Description("Obtiene el total de kilogramos reciclados registrados en el sistema")]
        public async Task<string> ObtenerTotalKgReciclados()
        {
            var total = await _context.Reciclajes.SumAsync(r => (int?)r.Cantidad) ?? 0;
            return $"{total} kg";
        }

        [KernelFunction, Description("Obtiene el número total de registros de reciclaje en el sistema")]
        public async Task<string> ObtenerTotalRegistros()
        {
            var count = await _context.Reciclajes.CountAsync();
            return count.ToString();
        }

        [KernelFunction, Description("Obtiene la lista de plantas ecológicas disponibles en el catálogo")]
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

        [KernelFunction, Description("Genera el contexto ecológico del sistema EcoPlantas")]
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
