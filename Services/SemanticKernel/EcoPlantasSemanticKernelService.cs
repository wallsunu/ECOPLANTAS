using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using EcoPlantas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace EcoPlantas.Services.SemanticKernel
{
    public class EcoPlantasSemanticKernelService
    {
        private readonly ApplicationDbContext _context;
        private readonly OllamaService _ollamaService;
        private readonly Kernel _kernel;

        public EcoPlantasSemanticKernelService(
            ApplicationDbContext context,
            OllamaService ollamaService)
        {
            _context = context;
            _ollamaService = ollamaService;

            // Construir el Kernel exponiendo el DbContext para que el plugin pueda resolverlo
            var builder = Kernel.CreateBuilder();
            builder.Services.AddSingleton(context);
            _kernel = builder.Build();

            // Registrar el plugin de EcoPlantas con funciones nativas marcadas con [KernelFunction].
            // Se pasa _kernel.Services para que el plugin pueda resolver el ApplicationDbContext;
            // sin él, AddFromType usa un proveedor vacío y falla al activar el plugin.
            _kernel.Plugins.AddFromType<EcoPlantasPlugin>("EcoPlantasPlugin", _kernel.Services);
        }

        /// <summary>
        /// Construye un contexto enriquecido usando los plugins del Kernel
        /// y delega la generación de respuesta a OllamaService.
        /// </summary>
        public async Task<string> ProcesarConContextoAsync(string pregunta)
        {
            // Invocar funciones del plugin para enriquecer el contexto
            var totalKg = await InvocarFuncionAsync("EcoPlantasPlugin", "ObtenerTotalKgReciclados");
            var totalRegistros = await InvocarFuncionAsync("EcoPlantasPlugin", "ObtenerTotalRegistros");
            var plantasDisponibles = await InvocarFuncionAsync("EcoPlantasPlugin", "ObtenerProductosDisponibles");
            var contextoSistema = await InvocarFuncionAsync("EcoPlantasPlugin", "ObtenerContextoEcologico");

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

            return await _ollamaService.GenerarRespuestaAsync(prompt);
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

            var totalKg = await InvocarFuncionAsync("EcoPlantasPlugin", "ObtenerTotalKgReciclados");
            var totalRegistros = await InvocarFuncionAsync("EcoPlantasPlugin", "ObtenerTotalRegistros");
            var plantas = await InvocarFuncionAsync("EcoPlantasPlugin", "ObtenerProductosDisponibles");

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
}
